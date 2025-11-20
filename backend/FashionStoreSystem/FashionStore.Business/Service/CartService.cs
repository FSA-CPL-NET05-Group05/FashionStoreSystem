using FashionStore.Business.Dtos;
using FashionStore.Business.Interfaces;
using FashionStore.Data.Interfaces;
using FashionStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Service
{
    public class CartService : ICartService
    {
        private readonly IGenericRepository<CartItem> _cartRepo;
        private readonly IProductRepository _productRepo;

        public CartService(IGenericRepository<CartItem> cartRepo, IProductRepository productRepo)
        {
            _cartRepo = cartRepo;
            _productRepo = productRepo;
        }

        public async Task<bool> AddToCartAsync(AddToCartDto dto)
        {

            var productSize = await _productRepo.GetProductSizeAsync(dto.ProductId, dto.ColorId, dto.SizeId);
            if (productSize == null) return false;

            var allItems = await _cartRepo.GetAllAsync();

            var existingItem = allItems.FirstOrDefault(c =>
                c.UserId == dto.UserId &&
                c.ProductId == dto.ProductId &&
                c.SizeId == dto.SizeId &&
                c.ColorId == dto.ColorId
            );

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
                await _cartRepo.UpdateAsync(existingItem);
            }
            else
            {
                var newItem = new CartItem
                {
                    UserId = dto.UserId,
                    ProductId = dto.ProductId,
                    SizeId = dto.SizeId,
                    ColorId = dto.ColorId,
                    Quantity = dto.Quantity
                };
                await _cartRepo.AddAsync(newItem);
            }

            return true; 
        }

        public async Task<IEnumerable<CartDto>> GetMyCartAsync(string userId)
        {

            var allItems = await _cartRepo.GetAllAsync();


            var myCartItems = allItems.Where(c => c.UserId == userId).ToList();


            var cartDtos = new List<CartDto>();

            foreach (var item in myCartItems)
            {

                var productSize = await _productRepo.GetProductSizeAsync(item.ProductId, item.ColorId, item.SizeId);

                if (productSize != null)
                {
                    cartDtos.Add(new CartDto
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        ProductName = productSize.Product.Name,
                        ProductImage = productSize.Product.ImageUrl,
                        Price = productSize.Product.Price,
                        SizeName = productSize.Size?.Name ?? "N/A",
                        ColorName = productSize.Color?.Name ?? "N/A"
                    });
                }
            }

            return cartDtos;
        }

        public async Task<bool> RemoveFromCartAsync(int cartItemId)
        {
            await _cartRepo.DeleteAsync(cartItemId);
            return true;
        }
    }
}
