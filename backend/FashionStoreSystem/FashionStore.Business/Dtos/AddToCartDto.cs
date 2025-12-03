using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FashionStore.Business.Dtos
{
    public class AddToCartDto
    {
        [JsonIgnore]
        public string? UserId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int SizeId { get; set; }
        [Required]
        public int ColorId { get; set; }
        [Required]
        public int Quantity { get; set; }  
    }
}
