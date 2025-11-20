using FashionStore.Data.DBContext;
using FashionStore.Data.Interfaces.Interfaces.Admin;
using FashionStore.Data.Models;
using FashionStore.Shared.Shared.Admin;
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


        public async Task<(List<AppUser> Items, int TotalCount)> GetPagedAsync(AccountQueryParameters parameters,CancellationToken ct = default)
        {
            var query = _context.Users.AsNoTracking().AsQueryable();

            // 1. SEARCH - Tìm theo username hoặc email
            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                var searchLower = parameters.Search.ToLower();
                query = query.Where(u =>
                    u.UserName.ToLower().Contains(searchLower) ||
                    u.Email.ToLower().Contains(searchLower));
            }

            // 2. FILTER - Lọc theo trạng thái khóa
            if (parameters.IsLocked.HasValue)
            {
                var now = DateTimeOffset.UtcNow;
                if (parameters.IsLocked.Value)
                {
                    // Chỉ lấy user đang bị khóa
                    query = query.Where(u => u.LockoutEnd.HasValue && u.LockoutEnd.Value > now);
                }
                else
                {
                    // Chỉ lấy user đang hoạt động
                    query = query.Where(u => !u.LockoutEnd.HasValue || u.LockoutEnd.Value <= now);
                }
            }

            // 3. SORT - Sắp xếp
            query = parameters.SortBy.ToLower() switch
            {
                "email" => parameters.SortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(u => u.Email)
                    : query.OrderBy(u => u.Email),

                "createddate" => parameters.SortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(u => EF.Property<DateTime>(u, "CreatedDate")) 
                    : query.OrderBy(u => EF.Property<DateTime>(u, "CreatedDate")),

                _ => parameters.SortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(u => u.UserName)
                    : query.OrderBy(u => u.UserName)
            };

            // Đếm tổng số bản ghi
            var totalCount = await query.CountAsync(ct);

            // 5. PAGING
            var items = await query
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync(ct);

            return (items, totalCount);
        }





        // Lấy tất cả User
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
