using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Dtos.Dtos.Customer
{
    public class FeedbackCreateDTO
    {
        // FeedbackCreateDTO.cs
       
            public int ProductId { get; set; }
            public string Comment { get; set; }
            public int Rating { get; set; }
        }
    
}
