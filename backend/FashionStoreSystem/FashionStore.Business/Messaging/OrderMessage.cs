using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Messaging
{
    public class OrderItemMessage
    {
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public int ColorId { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderMessage
    {
        public Guid UserId { get; set; }
        public List<OrderItemMessage> Items { get; set; } = new List<OrderItemMessage>();
    }
}
