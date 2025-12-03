using FashionStore.Data.DBContext;
using FashionStore.Data.Interfaces;
using FashionStore.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Data.Repositories
{
    public class CartRepository : GenericRepository<CartItem>, ICartRepository
    {
        private readonly ApplicationDBContext _context;

        public CartRepository(ApplicationDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<CartItem?> GetExistingItemAsync(string userId, int productId, int sizeId, int colorId)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(c =>
                    c.UserId == userId &&
                    c.ProductId == productId &&
                    c.SizeId == sizeId &&
                    c.ColorId == colorId);
        }

        public async Task<List<CartItem>> GetUserCartWithDetailsAsync(string userId)
        {
            return await _context.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Product)
                .Include(c => c.Size)
                .Include(c => c.Color)
                .ToListAsync();

        }

        public async Task<List<CartItem>> GetCartOfUserAsync(string userId)
        {
            return await _context.CartItems
                .Where(c => c.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<CartItem>> GetCartItemsWithDetailsAsync(string userId)
        {
            return await _context.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Product)
                .Include(c => c.Size)
                .Include(c => c.Color)
                .ToListAsync();
        }

        public async Task DeleteCartItemsOfUserAsync(string userId)
        {

            var items = await _context.CartItems
                                      .Where(c => c.UserId == userId)
                                      .ToListAsync();
            if (items.Any())
            {
                _context.CartItems.RemoveRange(items);
            }
        }
    }

}
