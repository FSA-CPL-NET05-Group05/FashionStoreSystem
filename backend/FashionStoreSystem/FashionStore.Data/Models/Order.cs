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
        public string? UserId { get; set; }             // Null nếu là Guest mua
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }              // Pending, Cancelled

        public string ReceiverName { get; set; }        // Tên người nhận
        public string ReceiverAddress { get; set; }     // Địa chỉ nhận
        public string ReceiverPhone { get; set; }       // SĐT liên hệ

        // Foreign Key
        public AppUser? User { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
