using FashionStore.Data.DBContext;
using FashionStore.Data.Interfaces.Interfaces.Customer;
using FashionStore.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Data.Repositories.CustomerRepository
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly ApplicationDBContext _context;

        public FeedbackRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Feedback>> GetFeedbacksByProductIdAsync(int productId)
        {
            return await _context.Feedbacks
                .Include(f => f.User)
                .Where(f => f.ProductId == productId)
                .OrderByDescending(f => f.CreatedDate)
                .ToListAsync();
        }

        public async Task AddFeedbackAsync(Feedback feedback)
        {
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();
            await _context.Entry(feedback)
       .Reference(f => f.User)
       .LoadAsync();
        }
        public async Task<bool> HasUserPurchasedAsync(string userId, int productId)
        {
            return await _context.Orders
         .AnyAsync(o => o.UserId == userId &&
                        o.OrderDetails.Any(od => od.ProductId == productId));
        }

        public async Task<int> CountUserPurchasesAsync(string userId, int productId)
        {
            return await _context.OrderDetails
                .Where(od => od.ProductId == productId &&
                             od.Order.UserId == userId)
                .CountAsync();
        }

        public async Task<int> CountUserFeedbacksAsync(string userId, int productId)
        {
            return await _context.Feedbacks
                .Where(f => f.UserId == userId && f.ProductId == productId)
                .CountAsync();
        }
    }
}
