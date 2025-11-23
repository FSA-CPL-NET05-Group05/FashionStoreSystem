using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Dtos
{
    // FeedbackDTO.cs
    public class FeedbackDTO
    {
        public string UserName { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    // FeedbackCreateDTO.cs
    public class FeedbackCreateDTO
    {
        public int ProductId { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
    }

}
