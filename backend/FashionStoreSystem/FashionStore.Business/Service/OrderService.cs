using FashionStore.Business.Dtos;
using FashionStore.Business.Interfaces;
using FashionStore.Business.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Service
{
    public class OrderService : IOrderService
    {
        private readonly IRabbitMqProducer _producer;

        public OrderService(IRabbitMqProducer producer)
        {
            _producer = producer;
        }

        public async Task PlaceOrderAsync(CreateOrderDto dto)
        {
            var message = new OrderMessage
            {
                UserId = dto.UserId ?? Guid.Empty, 
                ProductId = dto.ProductId,
                ColorId = dto.ColorId, 
                SizeId = dto.SizeId,  
                Quantity = dto.Quantity,
            };

            await _producer.PublishOrderAsync(message);
        }
    }
}
