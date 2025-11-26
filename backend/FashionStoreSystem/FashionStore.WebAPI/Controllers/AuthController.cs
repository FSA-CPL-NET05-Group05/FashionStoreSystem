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

        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var response = await _loginService.LoginAsync(loginRequest);
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Trả về 401 với message từ exception
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}