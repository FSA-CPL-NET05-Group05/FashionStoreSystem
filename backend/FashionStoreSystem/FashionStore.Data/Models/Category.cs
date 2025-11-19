using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Data.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }                  // Ví dụ: Áo thun, Quần Jean

        // Foreign Key
        public ICollection<Product> Products { get; set; }
    }
}
