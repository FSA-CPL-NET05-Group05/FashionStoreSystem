using FashionStore.Business.Interfaces.Interfaces.Admin;
using FashionStore.Shared.Shared.Admin.Product;
using Microsoft.AspNetCore.Mvc;

namespace FashionStore.WebAPI.Controllers.Controllers.Admin
{


    [ApiController]
    [Route("api/AdminProducts")]
    public class AdminProductsController : ControllerBase
    {
        private readonly IAdminProductService _service;

        public AdminProductsController(IAdminProductService service)
        {
            _service = service;
        }

        
        /// Lấy danh sách sản phẩm có paging, search, filter
        
        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] ProductQueryParameters parameters, CancellationToken ct)
        {
            var result = await _service.GetPagedAsync(parameters, ct);
            return Ok(result);
        }

        
        /// Lấy chi tiết 1 sản phẩm
        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var product = await _service.GetByIdAsync(id, ct);
            if (product == null) return NotFound(new { Message = "Sản phẩm không tồn tại" });
            return Ok(product);
        }

       
        /// Tạo sản phẩm mới
       
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken ct)
        {
            var result = await _service.CreateAsync(request, ct);
            if (!result.Success) return BadRequest(new { result.Message });

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        
        /// Cập nhật sản phẩm
        
        [HttpPut("{id:int}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, request, ct);
            if (!result.Success) return BadRequest(new { result.Message });

            return Ok(new { result.Message });
        }

        
        /// Xóa sản phẩm
        
        [HttpDelete("{id:int}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Success) return BadRequest(new { result.Message });

            return Ok(new { result.Message });
        }
    }
}
