using FashionStore.Business.Dtos;
using FashionStore.Business.Dtos.Dtos.Customer;
using FashionStore.Business.Interfaces.Interfaces.Customer;
using FashionStore.Data.Interfaces.Interfaces.Customer;
using FashionStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Service.Customer.Service
{
    public class FeedbackService:IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;

        public FeedbackService(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }

        public async Task<IEnumerable<FeedbackDTO>> GetFeedbacksByProductIdAsync(int productId)
        {
            var feedbacks = await _feedbackRepository.GetFeedbacksByProductIdAsync(productId);
            return feedbacks.Select(f => new FeedbackDTO
            {
                UserName = f.User?.UserName ?? "Unknown",
                Comment = f.Comment,
                Rating = f.Rating,
                CreatedDate = f.CreatedDate
            });
        }

        public async Task<FeedbackDTO> CreateFeedbackAsync(string userId, FeedbackCreateDTO dto)
        {
            // 1) Validate dữ liệu từ DTO
            if (string.IsNullOrWhiteSpace(dto.Comment))
                throw new ArgumentException("Comment cannot be empty.");

            if (dto.Rating < 1 || dto.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5.");

            // 2) Kiểm tra user đã mua product chưa
            bool hasPurchased = await _feedbackRepository
                .HasUserPurchasedAsync(userId, dto.ProductId);

            if (!hasPurchased)
                throw new UnauthorizedAccessException(
                    "You must purchase this product before leaving a review."
                );

            // 3) Tạo Feedback entity
            var feedback = new Feedback
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Comment = dto.Comment,
                Rating = dto.Rating,
                CreatedDate = DateTime.Now
            };

            // 4) Lưu vào DB + load User lại từ DB
            await _feedbackRepository.AddFeedbackAsync(feedback);

            // 5) Trả về DTO cho frontend
            return new FeedbackDTO
            {
                UserName = feedback.User?.UserName ?? "Unknown",
                Comment = feedback.Comment,
                Rating = feedback.Rating,
                CreatedDate = feedback.CreatedDate
            };
        }
    }
}
