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

        
    }

}
