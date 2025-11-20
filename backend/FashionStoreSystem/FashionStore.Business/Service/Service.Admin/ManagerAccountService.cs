
using FashionStore.Business.Dtos.Dtos.Admin;
using FashionStore.Business.Interfaces.Interfaces.Admin;
using FashionStore.Data.Interfaces.Interfaces.Admin;
using FashionStore.Data.Models;
using FashionStore.Shared.Shared.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Service.Service.Admin
{
    public class ManagerAccountService : IManagerAccountService
    {


        private readonly IManagerAccountRepository _repo;
        private readonly UserManager<AppUser> _userManager;
        private const string AdminRole = "Admin";

        public ManagerAccountService(IManagerAccountRepository repo, UserManager<AppUser> userManager)
        {
            _repo = repo;
            _userManager = userManager;

        }

        private AccountDTO MapToDto(AppUser u)
        {
            return new AccountDTO
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                FullName = (u.GetType().GetProperty("FullName") != null) ? (string?)u.GetType().GetProperty("FullName")!.GetValue(u) : null,
                LockoutEnd = u.LockoutEnd,
                IsLocked = u.LockoutEnd.HasValue && u.LockoutEnd.Value > DateTimeOffset.UtcNow
            };
        }


        // Paging
        public async Task<PagedResult<AccountDTO>> GetPagedAsync(
            AccountQueryParameters parameters,
            CancellationToken ct = default)
        {
            var (items, totalCount) = await _repo.GetPagedAsync(parameters, ct);

            var dtos = items.Select(MapToDto).ToList();

            return new PagedResult<AccountDTO>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = parameters.Page,
                PageSize = parameters.PageSize
            };
        }



        public async Task<List<AccountDTO>> GetAllAsync(CancellationToken ct = default)
        {
            var users = await _repo.GetAllAsync(ct);
            return users.Select(MapToDto).ToList();
        }

        public async Task<AccountDTO?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var user = await _repo.GetByIdAsync(id, ct);
            if (user == null) return null;
            return MapToDto(user);
        }

        public async Task<OperationResult> LockUserAsync(Guid targetId, Guid performedById, string reason, CancellationToken ct = default)
        {
            if (targetId == performedById)
                return OperationResult.Fail("Admin cannot lock itself.");

            var target = await _userManager.FindByIdAsync(targetId.ToString());
            if (target == null)
                return OperationResult.Fail("User does not exist.");

            var ok = await _repo.LockUserAsync(targetId, ct);
            if (!ok)
                return OperationResult.Fail("Lock failed.");

            // Lưu lịch sử
            await _repo.AddLockHistoryAsync(targetId.ToString(), performedById.ToString(), "Lock", reason, ct);

            return OperationResult.Ok("Account has been locked.");
        }


        public async Task<OperationResult> UnlockUserAsync(Guid targetId, Guid performedById, string reason, CancellationToken ct = default)
        {
            var target = await _userManager.FindByIdAsync(targetId.ToString());
            if (target == null) return OperationResult.Fail("User does not exist.");

            var ok = await _repo.UnlockUserAsync(targetId, ct);
            if (!ok)
            {

                return OperationResult.Fail("Unlock failed.");
            }

            await _repo.AddLockHistoryAsync(targetId.ToString(), performedById.ToString(), "Unlock", reason, ct);

            return OperationResult.Ok("Account has been unlocked.");
        }

    }
}
