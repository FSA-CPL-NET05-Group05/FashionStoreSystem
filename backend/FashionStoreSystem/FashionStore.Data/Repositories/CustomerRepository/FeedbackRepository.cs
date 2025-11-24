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
        }
      
    }
}
