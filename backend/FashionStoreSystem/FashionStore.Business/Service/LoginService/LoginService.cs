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
            var user = await _loginRepository.GetByUsernameAsync(loginRequest.Username);

            if (user == null)
            {
                throw new Exception("Invalid username or password");
            }

            // ✅ KIỂM TRA LOCKOUT TRƯỚC
            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
            {
                throw new Exception("Account banned");
            }

            // Kiểm tra password
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, lockoutOnFailure: false);

            // ✅ Kiểm tra kỹ các trường hợp
            if (result.IsLockedOut)
            {
                throw new Exception("Account banned");
            }

            if (result.IsNotAllowed)
            {
                throw new Exception("Account not allowed to sign in");
            }

            if (!result.Succeeded)
            {
                throw new Exception("Invalid username or password");
            }

            // Đăng nhập thành công
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
    }
    }
