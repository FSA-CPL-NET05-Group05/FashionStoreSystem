using FashionStore.Business.Dtos.Dtos.Admin;
using FashionStore.Business.Dtos.Dtos.Admin.ProductSize;
using FashionStore.Shared.Shared.Admin.ProductSize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Interfaces.Interfaces.Admin
{
    public interface IAdminProductSizeService
    {
        Task<List<AdminProductSizeDTO>> GetByProductIdAsync(int productId, CancellationToken ct = default);
        Task<AdminProductSizeDTO?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<OperationResult<AdminProductSizeDTO>> CreateAsync(CreateProductSizeRequest request, CancellationToken ct = default);
        Task<OperationResult> UpdateAsync(int id, UpdateProductSizeRequest request, CancellationToken ct = default);
        Task<OperationResult> DeleteAsync(int id, CancellationToken ct = default);


    }
}
