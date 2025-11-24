using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Shared.Shared.Admin.Product
{
    public class CreateProductRequest
    {

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Tên sản phẩm từ 3-200 ký tự")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Mô tả tối đa 1000 ký tự")]
        public string? Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải > 0")]
        public decimal Price { get; set; }

        [Url(ErrorMessage = "URL ảnh không hợp lệ")]
        public string? ImageUrl { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Phải chọn Category")]
        public int CategoryId { get; set; }




    }
}
