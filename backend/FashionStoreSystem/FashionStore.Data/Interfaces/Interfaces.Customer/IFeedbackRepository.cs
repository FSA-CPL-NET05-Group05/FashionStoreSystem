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
       
    }
}
