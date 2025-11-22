using FashionStore.Data.DBContext;
using FashionStore.Data.Interfaces;
using FashionStore.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Data.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ApplicationDBContext _context;
        public ProductRepository(ApplicationDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ProductSize?> GetProductSizeAsync(int productId, int colorId, int sizeId)
        {
            return await _context.ProductSizes
                .Include(x => x.Product)
                .Include(p => p.Size)   
                .Include(p => p.Color)
                .FirstOrDefaultAsync(x => x.ProductId == productId && x.ColorId == colorId && x.SizeId == sizeId);
        }

        public async Task<bool> DeductStockAsync(int productSizeId, int quantity)
        {

            int rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE ProductSizes SET Stock = Stock - {quantity} WHERE Id = {productSizeId} AND Stock >= {quantity}"
            );

            return rowsAffected > 0;
        }
    }
}
