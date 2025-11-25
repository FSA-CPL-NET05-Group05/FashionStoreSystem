using FashionStore.Business.Dtos;
using FashionStore.Business.Interfaces.Interfaces.Login;
using FashionStore.Business.Interfaces.Interfaces.Logout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FashionStore.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILoginServices _loginService;
        private readonly ILogoutService _logoutService;

        // Inject LoginService qua constructor
        public AuthController(ILoginServices loginService,ILogoutService logoutService)
        {
            _loginService = loginService;
            _logoutService = logoutService;
        }

        // Đăng nhập
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
        {
            // Gọi phương thức LoginAsync từ LoginService
            var response = await _loginService.LoginAsync(loginRequest);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Kiểm tra nếu response là null (đăng nhập không thành công)
            if (response == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            // Trả về thông tin người dùng và token
            return Ok(response);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var result = await _logoutService.LogoutAsync();
            return Ok(new { message = "Logout success" });
        }

    }
}
