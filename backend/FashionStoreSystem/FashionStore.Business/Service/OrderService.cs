using FashionStore.Business.Dtos;
using FashionStore.Business.Interfaces;
using FashionStore.Business.Messaging;
using FashionStore.Data.Interfaces;
using FashionStore.Data.Models;
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
        private readonly IGenericRepository<CartItem> _cartRepo;

        public OrderService(IRabbitMqProducer producer, IGenericRepository<CartItem> cartRepo)
        {
            _producer = producer;
            _cartRepo = cartRepo;
        }

        public async Task PlaceOrderAsync(CheckoutDto dto)
        {

            var allCartItems = await _cartRepo.GetAllAsync();
            var myCartItems = allCartItems.Where(c => c.UserId == dto.UserId.ToString()).ToList();

            if (myCartItems.Count == 0) return; 

            var message = new OrderMessage
            {
                UserId = dto.UserId,
                Items = new List<OrderItemMessage>()
            };

            foreach (var item in myCartItems)
            {
                message.Items.Add(new OrderItemMessage
                {
                    ProductId = item.ProductId,
                    SizeId = item.SizeId,
                    ColorId = item.ColorId,
                    Quantity = item.Quantity
                });
            }

            await _producer.PublishOrderAsync(message);
        }
    }
}
