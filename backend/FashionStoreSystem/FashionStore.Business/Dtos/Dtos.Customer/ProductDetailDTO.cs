using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Dtos.Dtos.Customer
{
    public  class ProductDetailDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }
        public decimal Price { get; set; }

        public string CategoryName { get; set; }
        public int TotalStock { get; set; }

        public List<string> Images { get; set; } = new();
        public List<ProductVarianDTO> Variants { get; set; }
    }
}
