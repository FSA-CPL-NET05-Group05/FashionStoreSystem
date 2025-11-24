using FashionStore.Business.Dtos;
using FashionStore.Business.Dtos.Dtos.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Interfaces.Interfaces.Customer
{
    public interface IFeedbackService
    {
        Task<IEnumerable<FeedbackDTO>> GetFeedbacksByProductIdAsync(int productId);
        Task<FeedbackDTO> CreateFeedbackAsync(string userId, FeedbackCreateDTO dto);
    }
}
