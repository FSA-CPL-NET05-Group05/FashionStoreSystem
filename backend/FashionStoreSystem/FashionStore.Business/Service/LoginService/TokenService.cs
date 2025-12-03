using FashionStore.Business.Interfaces.Interfaces.Login;
using FashionStore.Data.Interfaces.Interfaces.Login;
using FashionStore.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Service.LoginService
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        private readonly ILoginRepository _loginRepository;  // Inject ILoginRepository

        public TokenService(IConfiguration config, ILoginRepository loginRepository)
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]));
            _loginRepository = loginRepository;  // Initialize repository
        }

        public async Task<string> CreateToken(AppUser user)
        {
            // Lấy danh sách các roles của người dùng từ repository
            var roles = await _loginRepository.GetRolesAsync(user); // Gọi repository để lấy roles

            // Tạo claims cho token
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.GivenName, user.UserName),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())
        };

            // Thêm các roles vào claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));  // Thêm role vào claim
            }

            // Tạo SigningCredentials
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);

            // Tạo SecurityTokenDescriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };

            // Tạo token từ tokenDescriptor
            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);

            // Trả về token dưới dạng chuỗi
            return handler.WriteToken(token);
        }

      
    }

}
