using FashionStore.Business.Dtos;
using FashionStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Interfaces
{
    public interface ICartService
    {
        Task<bool> AddToCartAsync(AddToCartDto dto);
        Task<IEnumerable<CartDto>> GetMyCartAsync(string userId);
        Task<bool> RemoveFromCartAsync(int cartItemId, string UserId);
        Task<bool> UpdateCartItemQuantityAsync(UpdateCartItemDto dto);
    }
}
