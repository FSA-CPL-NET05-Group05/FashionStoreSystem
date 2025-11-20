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
    }
}
