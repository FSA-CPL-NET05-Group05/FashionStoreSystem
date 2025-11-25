using FashionStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Data.Interfaces.Interfaces.Customer
{
    public interface  ICategoryRepository
    {
        Task<List<Category>> GetAllCategoriesAsync();
    }
}
