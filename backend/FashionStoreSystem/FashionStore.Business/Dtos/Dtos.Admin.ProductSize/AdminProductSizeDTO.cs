using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Dtos.Dtos.Admin.ProductSize
{
    public class AdminProductSizeDTO
    {

        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;

        public int SizeId { get; set; }
        public string SizeName { get; set; } = string.Empty; // "S", "M", "L"

        public int ColorId { get; set; }
        public string ColorName { get; set; } = string.Empty; // "Red", "Blue"

        public int Stock { get; set; }


    }
}
