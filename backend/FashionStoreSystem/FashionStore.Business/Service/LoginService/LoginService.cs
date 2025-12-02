using FashionStore.Business.Dtos;
using FashionStore.Business.Interfaces.Interfaces.Login;
using FashionStore.Data.Interfaces.Interfaces.Login;
using FashionStore.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Service.LoginService
{
    public class LoginService : ILoginServices
    {
        private readonly ILoginRepository _loginRepository;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;

        public LoginService(ILoginRepository loginRepository, ITokenService tokenService, SignInManager<AppUser> signInManager)
        {
            _loginRepository = loginRepository;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequest)
        {
            // Lấy thông tin người dùng từ repository theo username
            var user = await _loginRepository.GetByUsernameAsync(loginRequest.Username);

            if (user == null)
            {
                // Username không tồn tại
                throw new UnauthorizedAccessException("INVALID_CREDENTIALS");
            }

            // ✅ KIỂM TRA TÀI KHOẢN BỊ KHÓA TRƯỚC
            if (user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
            {
                throw new UnauthorizedAccessException("ACCOUNT_LOCKED");
            }

            // Kiểm tra mật khẩu người dùng
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);

            if (result.Succeeded)
            {
                // Nếu đăng nhập thành công, tạo token và trả về thông tin người dùng và token
                var token = await _tokenService.CreateToken(user);
                var roles = await _loginRepository.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? "User";

                return new LoginResponseDTO
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Role = role,
                    FullName = user.FullName,
                    Token = token
                };
            }

            // Sai mật khẩu
            throw new UnauthorizedAccessException("INVALID_CREDENTIALS");
        }
    }
    }
