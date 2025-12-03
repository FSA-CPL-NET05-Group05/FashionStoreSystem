using FashionStore.Business.Dtos.Dtos.Admin;
using FashionStore.Business.Dtos.Dtos.Admin.Product;
using FashionStore.Shared.Shared.Admin;
using FashionStore.Shared.Shared.Admin.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Interfaces.Interfaces.Admin
{
   public interface IAdminProductService
    {

        Task<PagedResult<AdminProductDTO>> GetPagedAsync(ProductQueryParameters parameters, CancellationToken ct = default);
        Task<AdminProductDTO?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<OperationResult<AdminProductDTO>> CreateAsync(CreateProductRequest request, CancellationToken ct = default);
        Task<OperationResult> UpdateAsync(int id, UpdateProductRequest request, CancellationToken ct = default);
        Task<OperationResult> DeleteAsync(int id, CancellationToken ct = default);


    }
}
