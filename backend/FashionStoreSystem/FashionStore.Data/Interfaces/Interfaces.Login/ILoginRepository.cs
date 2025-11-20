using FashionStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Data.Interfaces.Interfaces.Login
{
    public interface  ILoginRepository
    {
        // 1. Tìm user theo Username
        Task<AppUser?> GetByUsernameAsync(string username);

        // 2. Kiểm tra mật khẩu có đúng không
        Task<bool> CheckPasswordAsync(AppUser user, string password);

        // 3. Lấy roles (phục vụ cho phân quyền hoặc tạo token)
        Task<IList<string>> GetRolesAsync(AppUser user);
    }

}
