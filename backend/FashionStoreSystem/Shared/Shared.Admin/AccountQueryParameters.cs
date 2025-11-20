using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Shared.Shared.Admin
{
    public class AccountQueryParameters
    {


        [Range(1, int.MaxValue, ErrorMessage = "Page phải >= 1")]
        public int Page { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "PageSize phải từ 1-100")]
        public int PageSize { get; set; } = 2;


        public string? Search { get; set; }


        public bool? IsLocked { get; set; } // null = all, true = locked only, false = active only



        public string SortBy { get; set; } = "UserName"; // UserName, Email, CreatedDate
        public string SortOrder { get; set; } = "asc"; // asc hoặc desc




    }
}
