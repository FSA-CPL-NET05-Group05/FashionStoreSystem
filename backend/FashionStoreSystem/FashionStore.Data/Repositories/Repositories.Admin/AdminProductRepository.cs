using FashionStore.Data.DBContext;
using FashionStore.Data.Interfaces.Interfaces.Admin;
using FashionStore.Shared.Shared.Admin.Product;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Data.Repositories.Repositories.Admin
{
    public class AdminProductRepository : IAdminProductRepository
    {

        private readonly ApplicationDBContext _context;

        public AdminProductRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<(List<Models.Product> Items, int TotalCount)> GetPagedAsync(ProductQueryParameters parameters, CancellationToken ct = default)
        {
            //var query = _context.Products
            //    .Include(p => p.Category)
            //    .Include(p => p.ProductSizes) 
            //    .AsNoTracking()
            //    .AsQueryable();

            var query = _context.Products
                        .Include(p => p.Category)
                        .Include(p => p.ProductSizes)        
                        .ThenInclude(ps => ps.Size)       
                        .Include(p => p.ProductSizes)       
                        .ThenInclude(ps => ps.Color)     
                        .AsNoTracking()
                        .AsQueryable();



            // 1. SEARCH - Tìm theo tên sản phẩm
            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                var searchLower = parameters.Search.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(searchLower));
            }

            // 2. FILTER - Lọc theo CategoryId
            if (parameters.CategoryId.HasValue && parameters.CategoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == parameters.CategoryId.Value);
            }

            // 3. SORT
            query = parameters.SortBy.ToLower() switch
            {
                "price" => parameters.SortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.Price)
                    : query.OrderBy(p => p.Price),

                "categoryname" => parameters.SortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.Category.Name)
                    : query.OrderBy(p => p.Category.Name),

                _ => parameters.SortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(p => p.Id)
                    : query.OrderBy(p => p.Id)
            };

            // 4. COUNT
            var totalCount = await query.CountAsync(ct);

            // 5. PAGING
            var items = await query
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync(ct);

            return (items, totalCount);
        }

        public async Task<Models.Product?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id, ct);
        }

        public async Task<Models.Product?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductSizes)
                    .ThenInclude(ps => ps.Size)
                .Include(p => p.ProductSizes)
                    .ThenInclude(ps => ps.Color)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id, ct);
        }

        public async Task<Models.Product> CreateAsync(Models.Product product, CancellationToken ct = default)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync(ct);
            return product;
        }

        public async Task<bool> UpdateAsync(Models.Product product, CancellationToken ct = default)
        {
            _context.Products.Update(product);
            var affected = await _context.SaveChangesAsync(ct);
            return affected > 0;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var product = await _context.Products.FindAsync(new object[] { id }, ct);
            if (product == null) return false;

            _context.Products.Remove(product);
            var affected = await _context.SaveChangesAsync(ct);
            return affected > 0;
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            return await _context.Products.AnyAsync(p => p.Id == id, ct);
        }

        public async Task<bool> CategoryExistsAsync(int categoryId, CancellationToken ct = default)
        {
            return await _context.Categories.AnyAsync(c => c.Id == categoryId, ct);
        }





    }
}
