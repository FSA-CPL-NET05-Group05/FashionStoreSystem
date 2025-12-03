using FashionStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Data.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {   
        Task<ProductSize?> GetProductSizeAsync(int productId, int colorId, int sizeId);
        Task<bool> DeductStockAsync(int productSizeId, int quantity);
    }
}
