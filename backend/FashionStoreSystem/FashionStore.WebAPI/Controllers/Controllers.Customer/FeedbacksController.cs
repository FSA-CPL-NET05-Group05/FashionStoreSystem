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

      
    }
}
