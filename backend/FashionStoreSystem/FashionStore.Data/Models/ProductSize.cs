using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Data.Models
{
    public class ProductSize
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int SizeId { get; set; }
        public Size Size { get; set; }

        public int Stock { get; set; }
    }
}
