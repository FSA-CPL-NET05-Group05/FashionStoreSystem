using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Dtos.Dtos.Admin
{
    public class LockAccountRequest
    {

        [Required(ErrorMessage = "Lý do khóa là bắt buộc")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Lý do phải từ 5-500 ký tự")]
        public string Reason { get; set; } = null!;


    }
}
