using FashionStore.Business.Dtos;
using FashionStore.Business.Interfaces;
using FashionStore.Data.Interfaces;
using FashionStore.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FashionStore.Business.Service
{
    public class CartService : ICartService
    {

        private readonly IUnitOfWork _unitOfWork;
        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AddToCartAsync(AddToCartDto dto)
        {

            var productSize = await _unitOfWork.Products.GetProductSizeAsync(dto.ProductId, dto.ColorId, dto.SizeId);
            if (productSize == null) return false;
            var existingItem = await _unitOfWork.Carts.GetExistingItemAsync(dto.UserId, dto.ProductId, dto.SizeId, dto.ColorId);

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
                _unitOfWork.Carts.Update(existingItem);
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

                _unitOfWork.Carts.Add(newItem);
            }

            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<IEnumerable<CartDto>> GetMyCartAsync(string userId)
        {
            var items = await _unitOfWork.Carts.GetUserCartWithDetailsAsync(userId);

            return items.Select(item => new CartDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                ProductName = item.Product.Name,
                ProductImage = item.Product.ImageUrl,
                Price = item.Product.Price,
                SizeName = item.Size?.Name,   
                ColorName = item.Color?.Name
            });
        }

        public async Task<bool> RemoveFromCartAsync(int cartItemId, string userId)
        {
            var cartItem = await _unitOfWork.Carts.GetByIdAsync(cartItemId);

            if (cartItem == null || cartItem.UserId != userId)
                return false;

            _unitOfWork.Carts.Delete(cartItem);

            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> UpdateCartItemQuantityAsync(UpdateCartItemDto dto)
        {
            var cartItem = await _unitOfWork.Carts.GetByIdAsync(dto.CartItemId);
            if (cartItem == null) return false;

            cartItem.Quantity = dto.NewQuantity;

            _unitOfWork.Carts.Update(cartItem);

            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}