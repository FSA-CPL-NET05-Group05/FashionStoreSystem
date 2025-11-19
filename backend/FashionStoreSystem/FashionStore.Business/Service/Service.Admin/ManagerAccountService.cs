using FashionStore.Business.Dtos;
using FashionStore.Business.Interfaces.Interfaces.Admin;
using FashionStore.Data.Interfaces.Interfaces.Admin;
using FashionStore.Data.Models;
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

        public ManagerAccountService(IManagerAccountRepository repo)
        {
            _repo = repo;
        }


        // Map thủ công AppUser -> AccountDto để tránh leak fields nhạy cảm
        private AccountDTO MapToDto(AppUser u)
        {
            return new AccountDTO
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                // Nếu AppUser có FullName property, map trực tiếp; nếu không, set null
                FullName = (u.GetType().GetProperty("FullName") != null) ? (string?)u.GetType().GetProperty("FullName")!.GetValue(u) : null,
                LockoutEnd = u.LockoutEnd,
                IsLocked = u.LockoutEnd.HasValue && u.LockoutEnd.Value > DateTimeOffset.UtcNow
            };
        }

        public async Task<List<AccountDTO>> GetAllAsync(CancellationToken ct = default)
        {
            List<AppUser> users = await _repo.GetAllAsync(ct);
            // Nếu cần paging/filtration, thêm params vào interface và repo
            return users.Select(MapToDto).ToList();
        }

        
    }
}
