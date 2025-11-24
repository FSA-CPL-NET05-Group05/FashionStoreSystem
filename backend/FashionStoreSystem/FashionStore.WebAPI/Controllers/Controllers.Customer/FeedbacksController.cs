using FashionStore.Business.Dtos.Dtos.Customer;
using FashionStore.Business.Interfaces.Interfaces.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FashionStore.WebAPI.Controllers.Controllers.Customer
{

    [Route("api/[controller]")]
    [ApiController]
    public class FeedbacksController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbacksController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        // Lấy tất cả feedback của sản phẩm
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetFeedbacks(int productId)
        {
            var feedbacks = await _feedbackService.GetFeedbacksByProductIdAsync(productId);
            return Ok(feedbacks);
        }

        // Thêm feedback
        [HttpPost]
        [Authorize(Roles = "Customer")] // Chỉ cho phép khách hàng đã đăng nhập
        public async Task<IActionResult> AddFeedback([FromBody] FeedbackCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Lấy userId từ JWT
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                    return Unauthorized();

                // Gọi Service
                var added = await _feedbackService.CreateFeedbackAsync(userId, dto);

                return Ok(added);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message }); // dữ liệu sai
            }
        }
    }
}
