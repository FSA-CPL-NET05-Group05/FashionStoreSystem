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
    public class CategoryRepository : ICategoryRepository
    {
      
           private readonly ApplicationDBContext _context;

        public CategoryRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }
    }
    
}
