using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Data.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string UserId { get; set; }                   // Link tới AppUser
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        // Foreign Key
        public Product Product { get; set; }
        public AppUser User { get; set; }
    }
}
