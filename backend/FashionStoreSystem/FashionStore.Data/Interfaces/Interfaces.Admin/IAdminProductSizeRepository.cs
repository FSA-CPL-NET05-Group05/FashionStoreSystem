using FashionStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Data.Interfaces.Interfaces.Admin
{
    public interface IAdminProductSizeRepository
    {

        Task<List<ProductSize>> GetByProductIdAsync(int productId, CancellationToken ct = default);  // Lấy các productsize sản phẩm của một sản phẩm dựa trên productId
        Task<ProductSize?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<ProductSize?> GetByCompositeKeyAsync(int productId, int sizeId, int colorId, CancellationToken ct = default);
        Task<ProductSize> CreateAsync(ProductSize productSize, CancellationToken ct = default);
        Task<bool> UpdateAsync(ProductSize productSize, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> SizeExistsAsync(int sizeId, CancellationToken ct = default);
        Task<bool> ColorExistsAsync(int colorId, CancellationToken ct = default);



    }
}
