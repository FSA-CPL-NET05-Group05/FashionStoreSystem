using FashionStore.Data.Models;
using FashionStore.Shared.Shared.Admin.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Data.Interfaces.Interfaces.Admin
{
   public interface IAdminProductRepository
    {

        // Paging + Search + Filter
        Task<(List<Product> Items, int TotalCount)> GetPagedAsync(ProductQueryParameters parameters,CancellationToken ct = default);

        Task<Product?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Product?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default); // Include Category
        Task<Product> CreateAsync(Product product, CancellationToken ct = default);
        Task<bool> UpdateAsync(Product product, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
        Task<bool> CategoryExistsAsync(int categoryId, CancellationToken ct = default);


        // Quản lý ProductImages
        Task AddImagesAsync(int productId, List<string> imageUrls, CancellationToken ct = default);
        Task RemoveAllImagesAsync(int productId, CancellationToken ct = default);



    }
}
