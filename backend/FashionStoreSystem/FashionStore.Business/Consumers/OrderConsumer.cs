using FashionStore.Business.Messaging;
using FashionStore.Data.DBContext;
using FashionStore.Data.Interfaces;
using FashionStore.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace FashionStore.Business.Consumers
{
    public class OrderConsumer : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly ILogger<OrderConsumer> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly string _queueName;

        public OrderConsumer(IConnection connection, ILogger<OrderConsumer> logger, IServiceProvider serviceProvider,IConfiguration configuration)
        {
            _connection = connection;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _queueName = configuration["RabbitMq:OrderQueue"] ?? "order_queue";
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            using var channel = await _connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(_queueName, true, false, false, null);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (modelCosumer, BasicDeliverEventArgs) =>
            {
                try
                {
                    var body = BasicDeliverEventArgs.Body.ToArray();
                    var messageJson = Encoding.UTF8.GetString(body);
                    var orderMessage = JsonSerializer.Deserialize<OrderMessage>(messageJson);

                    if (orderMessage != null)
                    {
                        await ProcessOrder(orderMessage);
                    }
                    await channel.BasicAckAsync(BasicDeliverEventArgs.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message from RabbitMQ");
                }
            };
            await channel.BasicConsumeAsync(_queueName, false, consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task ProcessOrder(OrderMessage message)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                try
                {
                    await unitOfWork.BeginTransactionAsync();

                    string userIdStr = message.UserId.ToString();

                    var newOrder = new Order
                    {
                        UserId = userIdStr,
                        OrderDate = DateTime.Now,
                        Status = "Pending",
                        TotalAmount = 0,
                        CustomerName = message.CustomerName,
                        CustomerPhone = message.CustomerPhone,
                        CustomerEmail = message.CustomerEmail
                    };


                    unitOfWork.Orders.Add(newOrder);

                    decimal totalAmount = 0;
                    bool hasAnyItemSuccess = false;

                    foreach (var item in message.Items)
                    {
                        var productSize = await unitOfWork.Products.GetProductSizeAsync(item.ProductId, item.ColorId, item.SizeId);

                        if (productSize == null) continue;

                        bool isSuccess = await unitOfWork.Products.DeductStockAsync(productSize.Id, item.Quantity);

                        if (isSuccess)
                        {
                            var detail = new OrderDetail
                            {
                                Order = newOrder,
                                ProductId = item.ProductId,
                                ColorId = item.ColorId,
                                SizeId = item.SizeId,
                                Quantity = item.Quantity,
                                Price = productSize.Product.Price
                            };

                            unitOfWork.OrderDetails.Add(detail);

                            totalAmount += (detail.Price * detail.Quantity);
                            hasAnyItemSuccess = true;

                            var cartItem = await unitOfWork.Carts.GetExistingItemAsync(userIdStr, item.ProductId, item.SizeId, item.ColorId);

                            if (cartItem != null)
                            {
                                unitOfWork.Carts.Delete(cartItem);
                            }
                        }
                    }

                    if (hasAnyItemSuccess)
                    {
                        newOrder.TotalAmount = totalAmount;
                        newOrder.Status = "Completed";
                    }
                    else
                    {
                        newOrder.Status = "Failed";
                    }

                    await unitOfWork.CommitTransactionAsync();

                    _logger.LogInformation($"Order processed: {newOrder.Status}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Transaction failed.");
                    await unitOfWork.RollbackTransactionAsync();
                }
            }
        }
    }
}
