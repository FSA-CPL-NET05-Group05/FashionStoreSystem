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
        private readonly ICartRepository _cart;

        public CartService(IGenericRepository<CartItem> cartRepo, IProductRepository productRepo, ICartRepository cart)
        {
            _cartRepo = cartRepo;
            _productRepo = productRepo;
            _cart = cart;
        }

        public async Task<bool> AddToCartAsync(AddToCartDto dto)
        {
            var productSize = await _productRepo.GetProductSizeAsync(dto.ProductId, dto.ColorId, dto.SizeId);
            if (productSize == null) return false;

            var existingItem = await _cartRepo.FirstOrDefaultAsync(c =>
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
                await _cartRepo.AddAsync(new CartItem
                {
                    UserId = dto.UserId,
                    ProductId = dto.ProductId,
                    SizeId = dto.SizeId,
                    ColorId = dto.ColorId,
                    Quantity = dto.Quantity
                });
            }

            return true;
        }


        public async Task<IEnumerable<CartDto>> GetMyCartAsync(string userId)
        {
            var items = await _cart.GetUserCartWithDetailsAsync(userId);

            return items.Select(item => new CartDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                ProductName = item.Product.Name,
                ProductImage = item.Product.ImageUrl,
                Price = item.Product.Price,
                SizeName = item.Size.Name,
                ColorName = item.Color.Name
            });
        }

        public async Task<bool> RemoveFromCartAsync(int cartItemId, string userId)
        {
            var cartItem = await _cartRepo.GetByIdAsync(cartItemId);
            if (cartItem == null || cartItem.UserId != userId)
                return false;

            await _cartRepo.DeleteAsync(cartItemId);
            return true;
        }

        public async Task<bool> UpdateCartItemQuantityAsync(UpdateCartItemDto dto)
        {
            var cartItem = await _cartRepo.GetByIdAsync(dto.CartItemId);
            if (cartItem == null) return false;

            cartItem.Quantity = dto.NewQuantity;
            await _cartRepo.UpdateAsync(cartItem);
            return true;
        }
    }
}
