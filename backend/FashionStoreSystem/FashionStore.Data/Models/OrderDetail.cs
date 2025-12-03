using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Data.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int SizeId { get; set; }
        public int ColorId { get; set; }

        // Foreign Key
        [ForeignKey("OrderId")]
        public Order Order { get; set; }
        public Product Product { get; set; }
        public Size Size { get; set; }
        public Color Color { get; set; }
    }
}
