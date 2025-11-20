using FashionStore.Business.Dtos;
using FashionStore.Business.Interfaces.Interfaces.Login;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FashionStore.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILoginServices _loginService;

        // Inject LoginService qua constructor
        public AuthController(ILoginServices loginService)
        {
            _loginService = loginService;
        }

        // Đăng nhập
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
        {
            // Gọi phương thức LoginAsync từ LoginService
            var response = await _loginService.LoginAsync(loginRequest);

            // Kiểm tra nếu response là null (đăng nhập không thành công)
            if (response == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            // Trả về thông tin người dùng và token
            return Ok(response);
        }
    }
}
