using FashionStore.Business.Dtos;
using FashionStore.Business.Interfaces;
using FashionStore.Business.Messaging;
using FashionStore.Data.Interfaces;
using FashionStore.Data.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FashionStore.Business.Service
{
    public class OrderService : IOrderService
    {
        private readonly IRabbitMqProducer _producer;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IRabbitMqProducer producer, IUnitOfWork unitOfWork, ILogger<OrderService> logger)
        {
            _producer = producer;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> PlaceOrderAsync(CheckoutDto dto)
        {
            var userCartItems = await _unitOfWork.Carts.GetCartOfUserAsync(dto.UserId);

            if (userCartItems.Count == 0)
            {
                return false;
            }

            bool isCartUpdated = false;

            if (dto.Items != null && dto.Items.Any())
            {
                foreach (var itemDto in dto.Items)
                {
                    var itemInDb = userCartItems.FirstOrDefault(c =>
                        c.ProductId == itemDto.ProductId &&
                        c.SizeId == itemDto.SizeId &&
                        c.ColorId == itemDto.ColorId
                    );

                    if (itemInDb != null && itemInDb.Quantity != itemDto.Quantity)
                    {
                        itemInDb.Quantity = itemDto.Quantity;
                        _unitOfWork.Carts.Update(itemInDb);
                        isCartUpdated = true;
                    }
                }
            }

            if (isCartUpdated)
            {
                await _unitOfWork.CompleteAsync();
            }

            var message = new OrderMessage
            {
                UserId = Guid.Parse(dto.UserId),
                CustomerName = dto.CustomerName,
                CustomerPhone = dto.CustomerPhone,
                CustomerEmail = dto.CustomerEmail,
                Items = new List<OrderItemMessage>()
            };

            foreach (var item in userCartItems)
            {
                message.Items.Add(new OrderItemMessage
                {
                    ProductId = item.ProductId,
                    SizeId = item.SizeId,
                    ColorId = item.ColorId,
                    Quantity = item.Quantity
                });
            }

            try
            {
                await _producer.PublishOrderAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when send message RabbitMQ");
                return false;
            }
        }


    }
}