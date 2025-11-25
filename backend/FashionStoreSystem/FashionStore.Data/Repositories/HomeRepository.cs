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
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDBContext _context;

        public HomeRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetTopRatedProductsAsync(int count)
        {
            var query = _context.Products.AsNoTracking().AsQueryable();
            query = query.Where(p => p.Feedbacks.Any() && p.Feedbacks.Average(f => f.Rating) >= 4);
            query = query.OrderByDescending(p => p.Feedbacks.Average(f => f.Rating));
            query = query.Take(count);
            var result = query.Select(p => new Product
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId
            });
            return await result.ToListAsync();
        }
    }
}
