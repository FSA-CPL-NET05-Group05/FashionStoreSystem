using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FashionStore.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        [HttpGet("products-home")]
        public async Task<IActionResult> GetProductsHomeAsync()
        {
            var products = await _homeService.GetTopRatedProductsAsync();
            return Ok(products);
        }
    }
}
