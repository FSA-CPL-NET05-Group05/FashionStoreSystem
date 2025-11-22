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

                // Thêm để map dữ liệu có được để xem chi tiết

                Variants = p.ProductSizes?.Select(ps => new ProductVariantDTO
                {
                    Id = ps.Id,
                    SizeId = ps.SizeId,
                    SizeName = ps.Size?.Name ?? "",
                    ColorId = ps.ColorId,
                    ColorName = ps.Color?.Name ?? "",
                    Stock = ps.Stock
                }).ToList() ?? new()



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

        public async Task<OperationResult<AdminProductDTO>> CreateAsync(
            CreateProductRequest request,
            CancellationToken ct = default)
        {
            // Business validation
            if (!await _repo.CategoryExistsAsync(request.CategoryId, ct))
                return OperationResult<AdminProductDTO>.Fail("Category không tồn tại");

            var product = new Data.Models.Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                ImageUrl = request.ImageUrl,
                CategoryId = request.CategoryId
            };

            var created = await _repo.CreateAsync(product, ct);
            var dto = MapToDto(created);

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

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.ImageUrl = request.ImageUrl;
            product.CategoryId = request.CategoryId;

            var ok = await _repo.UpdateAsync(product, ct);
            if (!ok) return OperationResult.Fail("Cập nhật thất bại");

            return OperationResult.Ok("Cập nhật sản phẩm thành công");
        }

        public async Task<OperationResult> DeleteAsync(int id, CancellationToken ct = default)
        {
            if (!await _repo.ExistsAsync(id, ct))
                return OperationResult.Fail("Sản phẩm không tồn tại");

            var ok = await _repo.DeleteAsync(id, ct);
            if (!ok) return OperationResult.Fail("Xóa thất bại");

            return OperationResult.Ok("Xóa sản phẩm thành công");
        }







    }
}
