using FashionStore.Business.Dtos;
using FashionStore.Business.Dtos.Dtos.Customer;
using FashionStore.Data.Models;
using FashionStore.Shared.Shared.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Interfaces.Interfaces.Customer
{
    public  interface ICustomerProductService
    {
        Task<PagedResult<ProductDTO>> GetProductsAsync(string? searchTerm, int? categoryId, string? sortOrder, int pageNumber, int pageSize, CancellationToken ct);
        Task<ProductDetailDTO?> GetProductByIdAsync(int productId);

    }
}
 