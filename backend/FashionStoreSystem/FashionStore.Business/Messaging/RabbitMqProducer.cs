using FashionStore.Business.Interfaces;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FashionStore.Business.Messaging
{
    public class RabbitMqProducer : IRabbitMqProducer
    {
        private readonly IConnection _connection;
        private readonly string _queueName;

        public RabbitMqProducer(IConnection connection, IConfiguration configuration)
        {
            _connection = connection;
            _queueName = configuration["RabbitMq:OrderQueue"] ?? "order_queue";
        }

        public async Task PublishOrderAsync(OrderMessage message)
        {
            using var channel = await _connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(_queueName, true, false, false, null);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties { Persistent = true };

            await channel.BasicPublishAsync("", _queueName, false, properties, body);
        }

    }
}
