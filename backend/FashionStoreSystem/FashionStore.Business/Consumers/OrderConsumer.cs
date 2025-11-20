using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using FashionStore.Business.Messaging;
using Microsoft.Extensions.DependencyInjection;
using FashionStore.Data.Interfaces;
using FashionStore.Data.Models;

namespace FashionStore.Business.Consumers
{
    public class OrderConsumer : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly ILogger<OrderConsumer> _logger;
        private readonly IServiceProvider _serviceProvider;

        public OrderConsumer(IConnection connection, ILogger<OrderConsumer> logger, IServiceProvider serviceProvider)
        {
            _connection = connection;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var channel = await _connection.CreateChannelAsync();
            await channel.QueueDeclareAsync("order_queue", true, false, false, null);
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var messageJson = Encoding.UTF8.GetString(body);
                var orderMessage = JsonSerializer.Deserialize<OrderMessage>(messageJson);

                if (orderMessage != null)
                {
                    await ProcessOrder(orderMessage);
                }
            };

            await channel.BasicConsumeAsync("order_queue", true, consumer);
            while (!stoppingToken.IsCancellationRequested) await Task.Delay(1000, stoppingToken);
        }

        private async Task ProcessOrder(OrderMessage message)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var productRepo = scope.ServiceProvider.GetRequiredService<IProductRepository>();
                var orderRepo = scope.ServiceProvider.GetRequiredService<IGenericRepository<Order>>();
                var orderDetailRepo = scope.ServiceProvider.GetRequiredService<IGenericRepository<OrderDetail>>();
                var cartRepo = scope.ServiceProvider.GetRequiredService<IGenericRepository<CartItem>>();

                try
                {
                    var newOrder = new Order
                    {
                        UserId = message.UserId.ToString(),
                        OrderDate = DateTime.Now,
                        Status = "Processing",
                        TotalAmount = 0 

                    };

                    await orderRepo.AddAsync(newOrder); 

                    decimal totalAmount = 0;


                    foreach (var item in message.Items)
                    {

                        var productSize = await productRepo.GetProductSizeAsync(item.ProductId, item.ColorId, item.SizeId);
                        if (productSize == null) continue;


                        bool isSuccess = await productRepo.DeductStockAsync(productSize.Id, item.Quantity);

                        if (isSuccess)
                        {
                            var detail = new OrderDetail
                            {
                                OrderId = newOrder.Id, 
                                ProductId = item.ProductId,
                                ColorId = item.ColorId,
                                SizeId = item.SizeId,
                                Quantity = item.Quantity,
                                Price = productSize.Product.Price
                            };
                            await orderDetailRepo.AddAsync(detail);


                            totalAmount += (detail.Price * detail.Quantity);

                            try
                            {
                                var allCartItems = await cartRepo.GetAllAsync();
                                var itemToDelete = allCartItems.FirstOrDefault(c =>
                                    c.UserId == message.UserId.ToString() &&
                                    c.ProductId == item.ProductId &&
                                    c.SizeId == item.SizeId &&
                                    c.ColorId == item.ColorId
                                );
                                if (itemToDelete != null)
                                {
                                    await cartRepo.DeleteAsync(itemToDelete.Id);
                                    _logger.LogInformation($"Removed Item {itemToDelete.Id} from cart.");
                                }
                            }
                            catch { }
                        }
                        else
                        {
                            _logger.LogWarning($"Sản phẩm {item.ProductId} hết hàng.");
                        }
                    }
                    newOrder.TotalAmount = totalAmount;
                    newOrder.Status = "Success";
                    await orderRepo.UpdateAsync(newOrder);

                    _logger.LogInformation($"Đơn hàng {newOrder.Id} hoàn tất. Tổng tiền: {totalAmount}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi nghiêm trọng khi xử lý đơn hàng");
                }
            }
        }
    }
}
