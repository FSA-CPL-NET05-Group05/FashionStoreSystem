using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Dtos.Dtos.Admin.Product
{
    public class AdminProductDTO
    {

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; } // Join Category

        // Tổng số stock trong kho (tính từ ProductSizes)
        public int TotalStock { get; set; }

        // Để biết chi tiết trong kho gồn có size , số lượng từng product
        public List<ProductVariantDTO> Variants { get; set; } = new();
    }

    public class ProductVariantDTO
    {
        public int Id { get; set; } // ProductSizeId
        public int SizeId { get; set; }
        public string SizeName { get; set; } = string.Empty; // "S", "M", "L", "XL"
        public int ColorId { get; set; }
        public string ColorName { get; set; } = string.Empty; // "Red", "Blue", "Black"
        public int Stock { get; set; }
    }





}
