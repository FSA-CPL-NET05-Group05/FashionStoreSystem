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

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequest) // Thay vì nhận username và password, nhận LoginRequestDTO
        {
            // Lấy thông tin người dùng từ repository theo username
            var user = await _loginRepository.GetByUsernameAsync(loginRequest.Username);

            if (user == null)
            {
                return null; // Trả về null nếu người dùng không tồn tại
            }

            // Kiểm tra mật khẩu người dùng
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);

            if (result.Succeeded)
            {
                // Nếu đăng nhập thành công, tạo token và trả về thông tin người dùng và token
                var token = await _tokenService.CreateToken(user);
                return new LoginResponseDTO
                {
                    Username = user.UserName,
                    Password=user.PasswordHash,
                    Token = token // Trả về token
                };
            }

            return null; // Trả về null nếu mật khẩu không đúng
        }
    }
    }
