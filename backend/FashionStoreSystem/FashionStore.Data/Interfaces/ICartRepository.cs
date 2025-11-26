using FashionStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Data.Interfaces
{
    public interface ICartRepository : IGenericRepository<CartItem>
    {
        Task<List<CartItem>> GetUserCartWithDetailsAsync(string userId);
        Task<CartItem?> GetExistingItemAsync(string userId, int productId, int sizeId, int colorId);
        Task<List<CartItem>> GetCartOfUserAsync(string userId);
        Task<List<CartItem>> GetCartItemsWithDetailsAsync(string userId);
        Task DeleteCartItemsOfUserAsync(string userId);
    }

}
