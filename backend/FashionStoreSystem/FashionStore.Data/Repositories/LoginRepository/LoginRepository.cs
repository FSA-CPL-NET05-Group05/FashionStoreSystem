using FashionStore.Data.Interfaces.Interfaces.Login;
using FashionStore.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Data.Repositories.LoginRepository
{
    public  class LoginRepository:ILoginRepository
    {
        private readonly UserManager<AppUser> _userManager;

        public LoginRepository(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        // 1. Tìm user theo Username
        public async Task<AppUser?> GetByUsernameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        // 2. Kiểm tra password có đúng không
        public async Task<bool> CheckPasswordAsync(AppUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        // 3. Lấy các role của user
        public async Task<IList<string>> GetRolesAsync(AppUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }
    }

}
