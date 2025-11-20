using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Dtos.Dtos.Admin
{
    public class AccountDTO
    {

        public string Id { get; set; } 
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }

        // Trạng thái khoá (true: đang khoá)
        public bool IsLocked { get; set; }

        // Thời điểm khoá 
        public DateTimeOffset? LockoutEnd { get; set; }

        public string Status => IsLocked ? "Locked" : "IsActive";
    }
}
