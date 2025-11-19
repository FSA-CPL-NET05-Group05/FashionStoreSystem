using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Dtos
{
    public class CreateOrderDto
    {
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public int ColorId { get; set; }
        public int Quantity { get; set; }

        // Thông tin nhận hàng
        public string? ReceiverName { get; set; }
        public string? ReceiverAddress { get; set; }
        public string? ReceiverPhone { get; set; }

        // Nếu user đã login
        public Guid? UserId { get; set; }
    }
}
