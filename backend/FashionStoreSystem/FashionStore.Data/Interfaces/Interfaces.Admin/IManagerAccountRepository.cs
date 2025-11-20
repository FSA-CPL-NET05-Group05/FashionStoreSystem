using FashionStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace FashionStore.Data.Interfaces.Interfaces.Admin
{
    public interface IManagerAccountRepository
    {

        
        Task<List<AppUser>> GetAllAsync(CancellationToken ct = default);  // CancellationToken tín hiệu hủy tác vụ
        Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<bool> LockUserAsync(Guid id, CancellationToken ct = default);
        Task<bool> UnlockUserAsync(Guid id, CancellationToken ct = default);

        // Thêm method lưu lịch sử
        Task AddLockHistoryAsync(string targetUserId, string performedByUserId, string action, string? reason, CancellationToken ct = default);




    }
}
