using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Data.Models
{
    public class AccountLockHistory
    {

        public Guid Id { get; set; }
        public string TargetUserId { get; set; } = null!; // User bị khóa/mở
        public string PerformedByUserId { get; set; } = null!; // Admin thực hiện
        public string Action { get; set; } = null!; // "Lock" hoặc "Unlock"
        public string? Reason { get; set; } // Lý do (bắt buộc khi Lock, optional khi Unlock)
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

       
        public AppUser? TargetUser { get; set; }
        public AppUser? PerformedByUser { get; set; }



    }
}
