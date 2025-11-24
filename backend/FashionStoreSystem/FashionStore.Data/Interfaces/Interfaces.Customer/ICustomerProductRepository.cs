using FashionStore.Data.Models;
using FashionStore.Shared.Shared.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Data.Interfaces.Interfaces.Customer
{
    public  interface ICustomerProductRepository
    {
        Task<PagedResult<Product>> GetProductsAsync(string? searchTerm, int? categoryId, bool? sortByPriceAsc, int pageNumber, int pageSize, CancellationToken ct);
        Task<Product> GetProductByIdAsync(int productId);
    }
}
