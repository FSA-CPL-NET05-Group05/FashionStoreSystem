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
    public class FeedbackService : IFeedbackService
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

    
    }
}
