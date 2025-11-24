using FashionStore.Data.DBContext;
using FashionStore.Data.Interfaces.Interfaces.Admin;
using FashionStore.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Data.Repositories.Repositories.Admin
{
    public class AdminProductSizeRepository : IAdminProductSizeRepository
    {


        private readonly ApplicationDBContext _context;

        public AdminProductSizeRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<ProductSize>> GetByProductIdAsync(int productId, CancellationToken ct = default)
        {
            return await _context.ProductSizes
                .Include(ps => ps.Product)
                .Include(ps => ps.Size)
                .Include(ps => ps.Color)
                .Where(ps => ps.ProductId == productId)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<ProductSize?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.ProductSizes
                .Include(ps => ps.Product)
                .Include(ps => ps.Size)
                .Include(ps => ps.Color)
                .AsNoTracking()
                .FirstOrDefaultAsync(ps => ps.Id == id, ct);
        }

        public async Task<ProductSize?> GetByCompositeKeyAsync(int productId, int sizeId, int colorId, CancellationToken ct = default)
        {
            return await _context.ProductSizes
                .FirstOrDefaultAsync(ps =>
                    ps.ProductId == productId &&
                    ps.SizeId == sizeId &&
                    ps.ColorId == colorId, ct);
        }

        public async Task<ProductSize> CreateAsync(ProductSize productSize, CancellationToken ct = default)
        {
            _context.ProductSizes.Add(productSize);
            await _context.SaveChangesAsync(ct);
            return productSize;
        }

        public async Task<bool> UpdateAsync(ProductSize productSize, CancellationToken ct = default)
        {
            _context.ProductSizes.Update(productSize);
            var affected = await _context.SaveChangesAsync(ct);
            return affected > 0;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var productSize = await _context.ProductSizes.FindAsync(new object[] { id }, ct);
            if (productSize == null) return false;

            _context.ProductSizes.Remove(productSize);
            var affected = await _context.SaveChangesAsync(ct);
            return affected > 0;
        }

        public async Task<bool> SizeExistsAsync(int sizeId, CancellationToken ct = default)
        {
            return await _context.Sizes.AnyAsync(s => s.Id == sizeId, ct);
        }

        public async Task<bool> ColorExistsAsync(int colorId, CancellationToken ct = default)
        {
            return await _context.Colors.AnyAsync(c => c.Id == colorId, ct);
        }




    }
}
