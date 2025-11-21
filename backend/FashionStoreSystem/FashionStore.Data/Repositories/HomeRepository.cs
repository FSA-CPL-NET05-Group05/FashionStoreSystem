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

        public async Task<IEnumerable<Product>> GetProductsHomeAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetTopRatedProductsAsync(int count)
        {
            return await _context.Products
                    .AsNoTracking()
                    .Include(p => p.Feedbacks)
                    .OrderByDescending(p => p.Feedbacks.Any() ? p.Feedbacks.Average(f => f.Rating) : 0)
                    .Take(count)
                    .Select(p => new Product
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        ImageUrl = p.ImageUrl,
                        CategoryId = p.CategoryId
                    })
                    .ToListAsync();
        }
    }
}
