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
        private readonly IChannel _channel; 
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

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
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
                    var productSize = await productRepo.GetProductSizeAsync(message.ProductId, message.ColorId, message.SizeId);

                    if (productSize == null)
                    {
                        _logger.LogError("No suitable product found!");
                        return;
                    }

                    bool isSuccess = await productRepo.DeductStockAsync(productSize.Id, message.Quantity);

                    if (isSuccess)
                    {
                        // Tạo Order
                        var newOrder = new Order
                        {
                            UserId = message.UserId.ToString(),
                            OrderDate = DateTime.Now,
                            Status = "Success",
                            TotalAmount = message.Quantity * productSize.Product.Price,

                        };

                        await orderRepo.AddAsync(newOrder);

                        // Tạo OrderDetail
                        var detail = new OrderDetail
                        {
                            OrderId = newOrder.Id,
                            ProductId = message.ProductId,
                            ColorId = message.ColorId,
                            SizeId = message.SizeId,
                            Quantity = message.Quantity,
                            Price = productSize.Product.Price
                        };

                        await orderDetailRepo.AddAsync(detail);

                        try
                        {
                            var allCartItems = await cartRepo.GetAllAsync();

                            var itemToDelete = allCartItems.FirstOrDefault(c =>
                                c.UserId == message.UserId.ToString() &&
                                c.ProductId == message.ProductId &&
                                c.SizeId == message.SizeId &&
                                c.ColorId == message.ColorId
                            );

                            if (itemToDelete != null)
                            {
                                // Xóa sản phẩm khỏi giỏ hàng
                                await cartRepo.DeleteAsync(itemToDelete.Id);
                                _logger.LogInformation($"Đã xóa sản phẩm khỏi giỏ hàng: CartItemID {itemToDelete.Id}");
                            }
                        }
                        catch (Exception exCart)
                        {
                            // Nếu xóa giỏ hàng lỗi thì log cảnh báo nhưng không rollback đơn hàng
                            _logger.LogWarning($"Order thành công nhưng lỗi xóa giỏ hàng: {exCart.Message}");
                        }

                        _logger.LogInformation($"The order has been successfully created for User {message.UserId}");
                    }
                    else
                    {
                        _logger.LogWarning($"Out of stock! User {message.UserId} miss purchase.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error when processing order");
                }
            }
        }
    }
}
