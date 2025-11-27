using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Shared.Shared.Admin.Product
{
    public class ProductQueryParameters
    {
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;


        public int PageSize { get; set; } = 20;

        // Tìm kiếm theo tên sản phẩm
        public string? Search { get; set; }

        // Lọc theo CategoryId
        public int? CategoryId { get; set; }

        // Sort
        public string SortBy { get; set; } = "Name"; // Name, Price, CategoryName
        public string SortOrder { get; set; } = "asc"; // asc, desc

    }
}
