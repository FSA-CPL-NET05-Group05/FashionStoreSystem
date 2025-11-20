using FashionStore.Data.DBContext;
using FashionStore.Data.Interfaces.Interfaces.Admin;
using FashionStore.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Data.Repositories.Repositories.Admin
{
    public class ManagerAccountRepository : IManagerAccountRepository
    {

        private readonly ApplicationDBContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ManagerAccountRepository(ApplicationDBContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;  // là Identity UserManager, một service đặc biệt của Identity Framework
        }


         // Lấy tất User
        public async Task<List<AppUser>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Users.AsNoTracking().ToListAsync(ct);
        }


        public async Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id.ToString(), ct);
            
        }

        public async Task<bool> LockUserAsync(Guid id, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return false;

            await _userManager.SetLockoutEnabledAsync(user, true);
            // Khóa vĩnh viễn: DateTimeOffset.MaxValue 
            // hoặc tạm thời: DateTimeOffset.UtcNow.AddDays(30)
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            return true;
        }

        public async Task<bool> UnlockUserAsync(Guid id, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return false;

            await _userManager.SetLockoutEndDateAsync(user, null);
            return true;
        }

        // Thêm method lưu lịch sử
        public async Task AddLockHistoryAsync(string targetUserId, string performedByUserId, string action, string? reason, CancellationToken ct = default)
        {
            var history = new AccountLockHistory
            {
                Id = Guid.NewGuid(),
                TargetUserId = targetUserId,
                PerformedByUserId = performedByUserId,
                Action = action,
                Reason = reason,
                Timestamp = DateTimeOffset.UtcNow
            };

            _context.AccountLockHistories.Add(history);
            await _context.SaveChangesAsync(ct);
        }



    }

}
