using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Dtos
{
    public class CheckoutDto
    {
        public Guid UserId { get; set; }

        [Required]
        public string CustomerName { get; set; }
        [Required]
        public string CustomerPhone { get; set; }
        [Required] 
        public string CustomerEmail { get; set; }
        public List<CheckoutItemDto> Items { get; set; } = new List<CheckoutItemDto>();
    }
}
