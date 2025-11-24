using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Shared.Shared.Admin.ProductSize
{
    public class CreateProductSizeRequest
    {

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ProductId không hợp lệ")]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Phải chọn Size")]
        public int SizeId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Phải chọn Color")]
        public int ColorId { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải >= 0")]
        public int Stock { get; set; }


    }
}
