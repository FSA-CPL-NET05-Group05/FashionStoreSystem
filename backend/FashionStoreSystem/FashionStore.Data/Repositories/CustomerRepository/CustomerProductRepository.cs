using FashionStore.Data.DBContext;
using FashionStore.Data.Interfaces;
using FashionStore.Data.Interfaces.Interfaces.Customer;

using FashionStore.Data.Models;
using FashionStore.Shared.Shared.Admin;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FashionStore.Data.Repositories
{
    public class CustomerProductRepository : ICustomerProductRepository
    {
        private readonly ApplicationDBContext _context;

        public CustomerProductRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Product>> GetProductsAsync(string? searchTerm, int? categoryId, bool? sortByPriceAsc, int pageNumber, int pageSize, CancellationToken ct)
        {
            var query = _context.Products.AsQueryable();

            // Lọc theo searchTerm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm));
            }

            // Lọc theo categoryId
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            // Nếu sortByPriceAsc không phải null, thực hiện sắp xếp theo giá
            if (sortByPriceAsc.HasValue)
            {
                if (sortByPriceAsc.Value)
                {
                    query = query.OrderBy(p => p.Price); // Sắp xếp từ thấp đến cao
                }
                else
                {
                    query = query.OrderByDescending(p => p.Price); // Sắp xếp từ cao đến thấp
                }
            }
            else
            {
                // Nếu không sắp xếp theo giá, giữ nguyên thứ tự hiện tại (hoặc sắp xếp theo ID nếu cần)
                query = query.OrderBy(p => p.Id); // Sắp xếp theo ID (hoặc để mặc định không sắp xếp)
            }

            // Lấy tổng số sản phẩm
            var totalCount = await query.CountAsync(ct);

            // Phân trang
            var products = await query
                .Skip((pageNumber - 1) * pageSize)  // Bỏ qua sản phẩm theo số trang
                .Take(pageSize)  // Lấy số lượng sản phẩm theo pageSize
                .ToListAsync(ct);

            return new PagedResult<Product>
            {
                TotalCount = totalCount,
                Page = pageNumber,
                PageSize = pageSize,
                Items = products
            };
        }
        public async Task<Product> GetProductByIdAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductSizes)
                    .ThenInclude(ps => ps.Size)
                .Include(p => p.ProductSizes)
                    .ThenInclude(ps => ps.Color)
                .FirstOrDefaultAsync(p => p.Id == productId);
        }
    }
}
