using FashionStore.Business.Interfaces.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FashionStore.WebAPI.Controllers.Controllers.Admin
{

    [ApiController]
    [Route("api/Accounts")]
    //[Authorize(Roles = "Admin")]

    public class AccountsController : Controller
    {


        private readonly IManagerAccountService _service;

        public AccountsController(IManagerAccountService service)
        {
            _service = service;
        }

        private Guid GetCurrentUserId()  // Sẽ nhận vào ID của admin đang đăng nhập, là claim 
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(idClaim, out var id) ? id : Guid.Empty;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var list = await _service.GetAllAsync(ct);
            return Ok(list);
        }


        [HttpPut("{id:guid}/lock")]
        public async Task<IActionResult> Lock(Guid id, CancellationToken ct)
        {
            var performerId = GetCurrentUserId();
            if (performerId == Guid.Empty)
            {
                
                performerId = new Guid("4221fd09-2787-43a1-8abd-d72694dc3146");
                
            }

            var result = await _service.LockUserAsync(id, performerId, ct);
            if (!result.Success) return BadRequest(new { result.Message });

           
            var dto = await _service.GetByIdAsync(id, ct);
            return Ok(new { Message = result.Message, Account = dto });
        }

        [HttpPut("{id:guid}/unlock")]
        public async Task<IActionResult> Unlock(Guid id, CancellationToken ct)
        {
            var performerId = GetCurrentUserId();
            if (performerId == Guid.Empty)
            {
                
                performerId = new Guid("4221fd09-2787-43a1-8abd-d72694dc3146");
                
            }

            var result = await _service.UnlockUserAsync(id, performerId, ct);
            if (!result.Success) return BadRequest(new { result.Message });

            var dto = await _service.GetByIdAsync(id, ct);
            return Ok(new { Message = result.Message, Account = dto });
        }





    }
}
