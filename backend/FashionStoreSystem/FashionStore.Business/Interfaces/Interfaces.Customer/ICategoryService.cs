using FashionStore.Business.Dtos.Dtos.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Interfaces.Interfaces.Customer
{
   public  interface ICategoryService
    {
        Task<List<CategoryDTO>> GetAllCategoriesAsync();
    }
}
