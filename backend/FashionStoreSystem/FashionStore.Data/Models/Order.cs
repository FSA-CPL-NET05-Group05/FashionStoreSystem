using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Data.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string? UserId { get; set; }             
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }              


        // Foreign Key
        public AppUser? User { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
