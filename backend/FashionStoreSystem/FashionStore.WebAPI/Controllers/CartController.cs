using FashionStore.Business.Dtos;
using FashionStore.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FashionStore.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            var result = await _cartService.AddToCartAsync(dto);

            if (result)
            {
                return Ok(new { message = "Add to cart successfully!" });
            }

            return BadRequest("The product does not exist or an error has occurred.");
        }

        [HttpGet("get-my-cart/{userId}")]
        public async Task<IActionResult> GetMyCart(string userId)
        {
            var myCart = await _cartService.GetMyCartAsync(userId);
            return Ok(myCart);
        }

        [HttpPut("update-quantity")]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateCartItemDto dto)
        {
            var result = await _cartService.UpdateCartItemQuantityAsync(dto);

            if (!result)
            {
                return NotFound(new { message = "Cart item not found." });
            }

            return Ok(new { message = "Cart updated successfully." });
        }

        [HttpDelete("remove/{cartItemId}")]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            var isDeleted = await _cartService.RemoveFromCartAsync(cartItemId);

            if (!isDeleted)
            {
                return NotFound(new { message = "The product does not exist in the cart or has been removed." });
            }

            return Ok(new { message = "Product removed from cart successfully." });
        }
    }
}
