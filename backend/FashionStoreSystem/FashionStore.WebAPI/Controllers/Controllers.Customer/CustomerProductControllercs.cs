using FashionStore.Business.Dtos;
using FashionStore.Business.Interfaces;
using FashionStore.Business.Interfaces.Interfaces.Customer;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FashionStore.API.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class CustomerProductController : ControllerBase
    {
        private readonly ICustomerProductService _productService;

        // Constructor nhận đối tượng ICustomerProductService qua Dependency Injection
        public CustomerProductController(ICustomerProductService productService)
        {
            _productService = productService;
        }

        // Phương thức GET để lấy danh sách sản phẩm với phân trang
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] string? searchTerm,
                                                     [FromQuery] int? categoryId,
                                                     [FromQuery] string? sortOrder,  // nhận sortOrder từ query string
                                                     [FromQuery] int pageNumber,
                                                     [FromQuery] int pageSize,
                                                     CancellationToken ct)
        {
            // Gọi service để lấy danh sách sản phẩm đã phân trang
            var products = await _productService.GetProductsAsync(searchTerm, categoryId, sortOrder, pageNumber, pageSize, ct);

            // Kiểm tra nếu không có sản phẩm nào
            if (products.Items.Count == 0)
            {
                // Trả về thông báo không tìm thấy sản phẩm
                return NotFound(new { message = "Không tìm thấy sản phẩm nào." });
            }


            // Trả về kết quả phân trang sản phẩm dưới dạng 200 OK
            return Ok(products);
        }

    }
}
