using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Data.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }                // Lưu đường dẫn ảnh
        public int Stock { get; set; }                      // Số lượng tồn kho
        public int CategoryId { get; set; }

        // Foreign Key
        public Category Category { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }
    }
}
