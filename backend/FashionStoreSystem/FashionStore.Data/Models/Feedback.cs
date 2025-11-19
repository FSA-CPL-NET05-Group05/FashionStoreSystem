using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Data.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }                 // 1-5 sao
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Foreign Key
        public AppUser User { get; set; }
        public Product Product { get; set; }
    }
}
