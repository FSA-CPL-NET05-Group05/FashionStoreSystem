using FashionStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Data.Interfaces.Interfaces.Customer
{
    public interface IFeedbackRepository
    {
        Task<IEnumerable<Feedback>> GetFeedbacksByProductIdAsync(int productId);
        Task AddFeedbackAsync(Feedback feedback);
        Task<bool> HasUserPurchasedAsync(string userId, int productId);
        Task<int> CountUserPurchasesAsync(string userId, int productId);
        Task<int> CountUserFeedbacksAsync(string userId, int productId);
    }
}
