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

        public async Task<bool> PlaceOrderAsync(CheckoutDto dto)
        {
            // CẢNH BÁO HIỆU NĂNG: 
            // GetAllAsync() sẽ tải toàn bộ bảng CartItem về RAM. Nếu có 1 triệu dòng, web sẽ sập.
            // Nên viết thêm hàm _cartRepo.GetListAsync(x => x.UserId == dto.UserId) trong Repository.
            var allCartItems = await _cartRepo.GetAllAsync();

            var myCartItems = allCartItems
                .Where(c => string.Equals(c.UserId, dto.UserId, StringComparison.OrdinalIgnoreCase))
                .ToList();

            // 2. Nếu giỏ hàng trống -> Trả về false
            if (myCartItems.Count == 0)
            {
                return false;
            }

            // Logic cập nhật số lượng phút chót (Giữ nguyên code của bạn)
            if (dto.Items != null && dto.Items.Any())
            {
                foreach (var itemDto in dto.Items)
                {
                    var itemInDb = myCartItems.FirstOrDefault(c =>
                        c.ProductId == itemDto.ProductId &&
                        c.SizeId == itemDto.SizeId &&
                        c.ColorId == itemDto.ColorId
                    );

                    if (itemInDb != null && itemInDb.Quantity != itemDto.Quantity)
                    {
                        itemInDb.Quantity = itemDto.Quantity;
                        await _cartRepo.UpdateAsync(itemInDb);
                    }
                }
            }

            // 3. Tạo Message gửi RabbitMQ
            var message = new OrderMessage
            {
                UserId = Guid.Parse(dto.UserId), 
                CustomerName = dto.CustomerName,
                CustomerPhone = dto.CustomerPhone,
                CustomerEmail = dto.CustomerEmail,
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

            try
            {

                await _producer.PublishOrderAsync(message);
                foreach (var item in myCartItems)
                {
                    await _cartRepo.DeleteAsync(item.Id);
                }

                return true; 
            }
            catch (Exception ex)
            {               
                return false; 
            }
        }
    }
}
