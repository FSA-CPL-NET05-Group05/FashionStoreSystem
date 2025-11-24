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
                try
                {
                    var body = ea.Body.ToArray();
                    var messageJson = Encoding.UTF8.GetString(body);
                    var orderMessage = JsonSerializer.Deserialize<OrderMessage>(messageJson);

                    if (orderMessage != null)
                    {
                        await ProcessOrder(orderMessage);
                    }
                    await channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message from RabbitMQ");
                }
            };
            await channel.BasicConsumeAsync("order_queue", false, consumer);

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

                    string userIdStr = message.UserId.ToString();

                    var newOrder = new Order
                    {
                        UserId = userIdStr,
                        OrderDate = DateTime.Now,
                        Status = "Success",
                        TotalAmount = 0,
                        CustomerName = message.CustomerName,
                        CustomerPhone = message.CustomerPhone,
                        CustomerEmail = message.CustomerEmail

                    };

                    await orderRepo.AddAsync(newOrder);

                    decimal totalAmount = 0;
                    bool hasAnyItemSuccess = false;


                    var allCartItems = await cartRepo.GetAllAsync();
                    var userCartItems = allCartItems
                        .Where(c => string.Equals(c.UserId, userIdStr, StringComparison.OrdinalIgnoreCase))
                        .ToList();


                    foreach (var item in message.Items)
                    {
                        var productSize = await productRepo.GetProductSizeAsync(item.ProductId, item.ColorId, item.SizeId);

                        if (productSize == null || productSize.Product == null)
                        {
                            _logger.LogWarning($"Product not found or invalid: {item.ProductId}");
                            continue;
                        }

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
                            hasAnyItemSuccess = true;

                            var itemToDelete = userCartItems.FirstOrDefault(c =>
                                c.ProductId == item.ProductId &&
                                c.SizeId == item.SizeId &&
                                c.ColorId == item.ColorId
                            );

                            if (itemToDelete != null)
                            {
                                await cartRepo.DeleteAsync(itemToDelete.Id);
                                _logger.LogInformation($"Removed item {itemToDelete.Id} from cart.");
                            }
                        }
                        else
                        {
                            _logger.LogWarning($"Product {item.ProductId} out of stock.");
                        }
                    }

                    if (hasAnyItemSuccess)
                    {
                        newOrder.TotalAmount = totalAmount;
                        newOrder.Status = "Success";
                        await orderRepo.UpdateAsync(newOrder);
                        _logger.LogInformation($"Order {newOrder.Id} created successfully for {newOrder.CustomerName}.");
                    }
                    else
                    {
                        newOrder.Status = "Failed";
                        newOrder.TotalAmount = 0;
                        await orderRepo.UpdateAsync(newOrder);
                        _logger.LogWarning($"Order {newOrder.Id} failed. No items processed.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Serious error while processing order");
                }
            }
        }
    }
}
