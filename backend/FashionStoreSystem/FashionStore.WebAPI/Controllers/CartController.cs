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
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? throw new UnauthorizedAccessException("User ID not found in token");
        }

        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            dto.UserId = GetCurrentUserId();

            var result = await _cartService.AddToCartAsync(dto);

            if (result)
            {
                return Ok(new { message = "Add to cart successfully!" });
            }

            return BadRequest("The product does not exist or an error has occurred.");
        }

        [HttpGet("get-my-cart")]
        public async Task<IActionResult> GetMyCart()
        {
            var userId = GetCurrentUserId();

            var myCart = await _cartService.GetMyCartAsync(userId);
            return Ok(myCart);
        }

        [HttpPut("update-quantity")]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateCartItemDto dto)
        {
            dto.UserId = GetCurrentUserId();

            var result = await _cartService.UpdateCartItemQuantityAsync(dto);

            if (!result)
            {
                return NotFound(new { message = "Cart item not found or you do not own this item." });
            }

            return Ok(new { message = "Cart updated successfully." });
        }

        [HttpDelete("remove/{cartItemId}")]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            var userId = GetCurrentUserId();
            var isDeleted = await _cartService.RemoveFromCartAsync(cartItemId, userId);

            if (!isDeleted)
            {
                return NotFound(new { message = "Item not found or access denied." });
            }

            return Ok(new { message = "Product removed from cart successfully." });
        }
    }
}
