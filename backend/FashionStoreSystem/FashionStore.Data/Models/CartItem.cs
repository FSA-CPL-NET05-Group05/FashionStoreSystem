using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Data.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public int ColorId { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key & Navigation Properties
        public Product Product { get; set; }
        public AppUser User { get; set; }

        [ForeignKey("SizeId")]
        public Size Size { get; set; }

        [ForeignKey("ColorId")]
        public Color Color { get; set; }
    }
}
