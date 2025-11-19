using FashionStore.Business.Interfaces.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var list = await _service.GetAllAsync(ct);
            return Ok(list);
        }





    }
}
