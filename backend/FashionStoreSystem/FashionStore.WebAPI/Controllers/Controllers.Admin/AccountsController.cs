using FashionStore.Business.Dtos.Dtos.Admin;
using FashionStore.Business.Interfaces.Interfaces.Admin;
using FashionStore.Shared.Shared.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FashionStore.WebAPI.Controllers.Controllers.Admin
{

    [ApiController]
    [Route("api/Accounts")]
    [Authorize(Roles = "Admin")]

    public class AccountsController : Controller
    {


        private readonly IManagerAccountService _service;

        public AccountsController(IManagerAccountService service)
        {
            _service = service;
        }

        private Guid GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(idClaim, out var id)) return id;

            else
            {
                return Guid.Empty;
            }
        }


        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] AccountQueryParameters parameters, CancellationToken ct)
        {
            var result = await _service.GetPagedAsync(parameters, ct);
            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var list = await _service.GetAllAsync(ct);
            return Ok(list);
        }

        [HttpPut("{id:guid}/lock")]
        public async Task<IActionResult> Lock(Guid id, [FromBody] LockAccountRequest request, CancellationToken ct)
        {
            var performerId = GetCurrentUserId();
            if (performerId == Guid.Empty) return Forbid();

            var result = await _service.LockUserAsync(id, performerId, request.Reason, ct);
            if (!result.Success) return BadRequest(new { result.Message });

            var dto = await _service.GetByIdAsync(id, ct);
            return Ok(new { Message = result.Message, Account = dto });
        }

        [HttpPut("{id:guid}/unlock")]
        public async Task<IActionResult> Unlock(Guid id, [FromBody] UnlockAccountRequest? request, CancellationToken ct)
        {
            var performerId = GetCurrentUserId();
            if (performerId == Guid.Empty) return Forbid();

            var result = await _service.UnlockUserAsync(id, performerId, request?.Reason, ct);
            if (!result.Success) return BadRequest(new { result.Message });

            var dto = await _service.GetByIdAsync(id, ct);
            return Ok(new { Message = result.Message, Account = dto });
        }




    }
}
