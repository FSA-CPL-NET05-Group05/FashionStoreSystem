using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Dtos
{
    public class CartDto
    {
        public int Id { get; set; } 
        public int ProductId { get; set; }
        public string ProductName { get; set; } 
        public string ProductImage { get; set; } 
        public decimal Price { get; set; }      

        public string SizeName { get; set; }    
        public string ColorName { get; set; }  
        public int Quantity { get; set; }
        public decimal TotalPrice => Price * Quantity;  
    }
}
