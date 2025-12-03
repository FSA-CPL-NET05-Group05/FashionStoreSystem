using FashionStore.Business.Dtos.Dtos.Admin;
using FashionStore.Business.Dtos.Dtos.Admin.ProductSize;
using FashionStore.Business.Interfaces.Interfaces.Admin;
using FashionStore.Data.Interfaces;
using FashionStore.Data.Interfaces.Interfaces.Admin;
using FashionStore.Shared.Shared.Admin.ProductSize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Service.Service.Admin
{
    public class AdminProductSizeService : IAdminProductSizeService
    {

        private readonly IAdminProductSizeRepository _repo;
        private readonly IAdminProductRepository _productRepo;

        public AdminProductSizeService(IAdminProductSizeRepository repo, IAdminProductRepository productRepo)
        {
            _repo = repo;
            _productRepo = productRepo;
        }

        private AdminProductSizeDTO MapToDto(Data.Models.ProductSize ps)
        {
            return new AdminProductSizeDTO
            {
                Id = ps.Id,
                ProductId = ps.ProductId,
                ProductName = ps.Product?.Name ?? "",
                SizeId = ps.SizeId,
                SizeName = ps.Size?.Name ?? "",
                ColorId = ps.ColorId,
                ColorName = ps.Color?.Name ?? "",
                Stock = ps.Stock
            };
        }

        public async Task<List<AdminProductSizeDTO>> GetByProductIdAsync(int productId, CancellationToken ct = default)
        {
            var items = await _repo.GetByProductIdAsync(productId, ct);
            return items.Select(MapToDto).ToList();
        }

        public async Task<AdminProductSizeDTO?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var item = await _repo.GetByIdAsync(id, ct);
            if (item == null) return null;
            return MapToDto(item);
        }

        public async Task<OperationResult<AdminProductSizeDTO>> CreateAsync(
            CreateProductSizeRequest request,
            CancellationToken ct = default)
        {
            // Business validation
            if (!await _productRepo.ExistsAsync(request.ProductId, ct))
                return OperationResult<AdminProductSizeDTO>.Fail("Product không tồn tại");

            if (!await _repo.SizeExistsAsync(request.SizeId, ct))
                return OperationResult<AdminProductSizeDTO>.Fail("Size không tồn tại");

            if (!await _repo.ColorExistsAsync(request.ColorId, ct))
                return OperationResult<AdminProductSizeDTO>.Fail("Color không tồn tại");

            // Kiểm tra trùng lặp (unique constraint)
            var existing = await _repo.GetByCompositeKeyAsync(request.ProductId, request.SizeId, request.ColorId, ct);
            if (existing != null)
                return OperationResult<AdminProductSizeDTO>.Fail("Kết hợp Product-Size-Color đã tồn tại");

            var productSize = new Data.Models.ProductSize
            {
                ProductId = request.ProductId,
                SizeId = request.SizeId,
                ColorId = request.ColorId,
                Stock = request.Stock
            };

            var created = await _repo.CreateAsync(productSize, ct);

            
            var dto = await GetByIdAsync(created.Id, ct);

            return OperationResult<AdminProductSizeDTO>.Ok("Thêm vào kho thành công", dto);
        }

        public async Task<OperationResult> UpdateAsync(
            int id,
            UpdateProductSizeRequest request,
            CancellationToken ct = default)
        {
            var productSize = await _repo.GetByIdAsync(id, ct);
            if (productSize == null)
                return OperationResult.Fail("ProductSize không tồn tại");

            productSize.Stock = request.Stock;

            var ok = await _repo.UpdateAsync(productSize, ct);
            if (!ok) return OperationResult.Fail("Cập nhật thất bại");

            return OperationResult.Ok("Cập nhật stock thành công");
        }

        public async Task<OperationResult> DeleteAsync(int id, CancellationToken ct = default)
        {
            var ok = await _repo.DeleteAsync(id, ct);
            if (!ok) return OperationResult.Fail("ProductSize không tồn tại hoặc xóa thất bại");

            return OperationResult.Ok("Xóa ProductSize thành công");
        }



    }
}
