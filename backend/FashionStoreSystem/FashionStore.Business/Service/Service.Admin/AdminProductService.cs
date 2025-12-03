using FashionStore.Business.Dtos.Dtos.Admin;
using FashionStore.Business.Dtos.Dtos.Admin.Product;
using FashionStore.Business.Interfaces.Interfaces.Admin;
using FashionStore.Data.Interfaces;
using FashionStore.Data.Interfaces.Interfaces.Admin;
using FashionStore.Shared.Shared.Admin;
using FashionStore.Shared.Shared.Admin.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Service.Service.Admin
{
    public class AdminProductService : IAdminProductService
    {

        private readonly IAdminProductRepository _repo;

        public AdminProductService(IAdminProductRepository repo)
        {
            _repo = repo;
        }

        private AdminProductDTO MapToDto(Data.Models.Product p)
        {
            return new AdminProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name,
                TotalStock = p.ProductSizes?.Sum(ps => ps.Stock) ?? 0,


                // Map Images
                Images = p.Images?.Select(img => img.Url).ToList() ?? new()

            };
        }

        public async Task<PagedResult<AdminProductDTO>> GetPagedAsync(ProductQueryParameters parameters,CancellationToken ct = default)
        {
            var (items, totalCount) = await _repo.GetPagedAsync(parameters, ct);
            var dtos = items.Select(MapToDto).ToList();

            return new PagedResult<AdminProductDTO>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = parameters.Page,
                PageSize = parameters.PageSize
            };
        }

        public async Task<AdminProductDTO?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var product = await _repo.GetByIdWithDetailsAsync(id, ct);
            if (product == null) return null;
            return MapToDto(product);
        }

        public async Task<OperationResult<AdminProductDTO>> CreateAsync(CreateProductRequest request,
           CancellationToken ct = default)
        {
            // Validation
            if (!await _repo.CategoryExistsAsync(request.CategoryId, ct))
                return OperationResult<AdminProductDTO>.Fail("Category không tồn tại");

            // 1. Tạo Product
            var product = new Data.Models.Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                ImageUrl = request.ImageUrl,
                CategoryId = request.CategoryId
            };

            var created = await _repo.CreateAsync(product, ct);

            // 2. Thêm ảnh phụ (nếu có)
            if (request.AdditionalImages != null && request.AdditionalImages.Any())
            {
                await _repo.AddImagesAsync(created.Id, request.AdditionalImages, ct);
            }

            // 3. Load lại đủ thông tin
            var fullProduct = await _repo.GetByIdWithDetailsAsync(created.Id, ct);
            var dto = MapToDto(fullProduct!);

            return OperationResult<AdminProductDTO>.Ok("Tạo sản phẩm thành công", dto);
        }

        public async Task<OperationResult> UpdateAsync(
            int id,
            UpdateProductRequest request,
            CancellationToken ct = default)
        {
           
            var product = await _repo.GetByIdAsync(id, ct);
            if (product == null)
                return OperationResult.Fail("Sản phẩm không tồn tại");

           
            if (!await _repo.CategoryExistsAsync(request.CategoryId, ct))
                return OperationResult.Fail("Category không tồn tại");

            // 3. Update thông tin 
            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.ImageUrl = request.ImageUrl;
            product.CategoryId = request.CategoryId;

            var ok = await _repo.UpdateAsync(product, ct);
            if (!ok) return OperationResult.Fail("Cập nhật thất bại");

            // 4. Update ảnh phụ
            // Xóa tất cả ảnh cũ
            await _repo.RemoveAllImagesAsync(id, ct);

            // Thêm ảnh mới (nếu có)
            if (request.AdditionalImages != null && request.AdditionalImages.Any())
            {
                await _repo.AddImagesAsync(id, request.AdditionalImages, ct);
            }

            return OperationResult.Ok("Cập nhật sản phẩm thành công");
        }

        public async Task<OperationResult> DeleteAsync(int id, CancellationToken ct = default)
        {
            if (!await _repo.ExistsAsync(id, ct))
                return OperationResult.Fail("Sản phẩm không tồn tại");

            // DeleteBehavior.Cascade sẽ tự động xóa Images
            var ok = await _repo.DeleteAsync(id, ct);
            if (!ok) return OperationResult.Fail("Xóa thất bại");

            return OperationResult.Ok("Xóa sản phẩm thành công");
        }







    }
}
