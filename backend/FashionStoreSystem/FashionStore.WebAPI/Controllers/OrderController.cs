using FashionStore.Business.Dtos;
using FashionStore.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FashionStore.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("place-order")]
        public async Task<IActionResult> PlaceOrder([FromBody] CheckoutDto dto)
        {
          
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID is missing.");
            }

            dto.UserId = userId;
            var result = await _orderService.PlaceOrderAsync(dto);

            if (!result)
            {
                return BadRequest("Failed to place order (Cart empty or invalid).");
            }

            return Accepted(new { message = "Your order is being processed." });
        }
    }
}
