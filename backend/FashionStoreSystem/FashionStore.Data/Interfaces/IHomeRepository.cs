using FashionStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Data.Interfaces
{
    public interface IHomeRepository
    {
        Task<IEnumerable<Product>> GetTopRatedProductsAsync(int count);
    }
}
