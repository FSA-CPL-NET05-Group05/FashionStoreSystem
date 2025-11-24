using FashionStore.Business.Interfaces.Interfaces.Admin;
using FashionStore.Shared.Shared.Admin.ProductSize;
using Microsoft.AspNetCore.Mvc;

namespace FashionStore.WebAPI.Controllers.Controllers.Admin
{

    [ApiController]
    [Route("api/products/{productId}/stocks")]

    public class AdminProductStocksController : ControllerBase
    {
        private readonly IAdminProductSizeService _service;

        public AdminProductStocksController(IAdminProductSizeService service)
        {
            _service = service;
        }

        
        /// Lấy danh sách stock của 1 product
        
        [HttpGet]
        public async Task<IActionResult> GetByProductId(int productId, CancellationToken ct)
        {
            var stocks = await _service.GetByProductIdAsync(productId, ct);
            return Ok(stocks);
        }

        
        /// Thêm stock cho product (chọn Size, Color, nhập số lượng)
       
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(int productId, [FromBody] CreateProductSizeRequest request, CancellationToken ct)
        {
            // Override productId từ route
            request.ProductId = productId;

            var result = await _service.CreateAsync(request, ct);
            if (!result.Success) return BadRequest(new { result.Message });

            return CreatedAtAction(nameof(GetById), new { productId, id = result.Data!.Id }, result.Data);
        }

        
        /// Lấy chi tiết 1 ProductSize
        
        [HttpGet("{id:int}", Name = nameof(GetById))]
        public async Task<IActionResult> GetById(int productId, int id, CancellationToken ct)
        {
            var stock = await _service.GetByIdAsync(id, ct);
            if (stock == null || stock.ProductId != productId)
                return NotFound(new { Message = "Stock không tồn tại" });

            return Ok(stock);
        }

       
        /// Cập nhật stock
        
        [HttpPut("{id:int}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int productId, int id, [FromBody] UpdateProductSizeRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Success) return BadRequest(new { result.Message });

            return Ok(new { result.Message });
        }

        
        /// Xóa stock
        
        [HttpDelete("{id:int}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int productId, int id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Success) return BadRequest(new { result.Message });

            return Ok(new { result.Message });
        }
    }
}
