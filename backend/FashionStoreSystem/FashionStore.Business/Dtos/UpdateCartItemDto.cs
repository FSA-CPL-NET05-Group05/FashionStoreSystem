using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Dtos
{
    public class UpdateCartItemDto
    {
        [Required]
        public int CartItemId { get; set; } 

        [Required]
        [Range(1, 1000, ErrorMessage = "Quantity must be at least 1")]
        public int NewQuantity { get; set; }
    }
}
