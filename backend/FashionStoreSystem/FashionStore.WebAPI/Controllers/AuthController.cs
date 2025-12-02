using FashionStore.Business.Dtos;
using FashionStore.Business.Interfaces.Interfaces.Login;

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


        // Inject LoginService qua constructor
        public AuthController(ILoginServices loginService)
        {
            _loginService = loginService;

        }

        // Đăng nhập
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Gọi phương thức LoginAsync từ LoginService
                var response = await _loginService.LoginAsync(loginRequest);

                // Trả về thông tin người dùng và token
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                // ✅ PHÂN BIỆT 2 TRƯỜNG HỢP
                if (ex.Message == "ACCOUNT_LOCKED")
                {
                    return Unauthorized(new
                    {
                        message = "Your account has been locked. Please contact support.",
                        code = "ACCOUNT_LOCKED"
                    });
                }
                else if (ex.Message == "INVALID_CREDENTIALS")
                {
                    return Unauthorized(new
                    {
                        message = "Invalid username or password",
                        code = "INVALID_CREDENTIALS"
                    });
                }

                return Unauthorized(new { message = "Authentication failed" });
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }
    }
    }
