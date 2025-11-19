using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Data.Models
{
    public class Color
    {
        public int Id { get; set; }
        public string Name { get; set; }     // Ví dụ: Red, Blue, Black
        public string? Code { get; set; }    // Mã màu

        public ICollection<ProductSize> ProductSizes { get; set; }
    }
}
