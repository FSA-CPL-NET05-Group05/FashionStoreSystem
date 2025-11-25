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
            // Validate
            if (string.IsNullOrWhiteSpace(dto.Comment))
                throw new ArgumentException("Comment cannot be empty.");

            if (dto.Rating < 1 || dto.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5.");

            // 1) Tổng số lần đã mua
            int purchaseCount = await _feedbackRepository.CountUserPurchasesAsync(userId, dto.ProductId);

            if (purchaseCount == 0)
                throw new UnauthorizedAccessException("You must purchase this product before leaving a review.");

            // 2) Tổng số lần đã feedback
            int feedbackCount = await _feedbackRepository.CountUserFeedbacksAsync(userId, dto.ProductId);

            if (feedbackCount >= purchaseCount)
                throw new UnauthorizedAccessException("You have already left all allowed reviews for this product.");

            // 3) Tạo feedback
            var feedback = new Feedback
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Comment = dto.Comment,
                Rating = dto.Rating,
                CreatedDate = DateTime.Now
            };

            await _feedbackRepository.AddFeedbackAsync(feedback);

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
