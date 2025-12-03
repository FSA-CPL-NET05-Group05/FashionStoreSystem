using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FashionStore.Business.Dtos
{
    public class CheckoutDto
    {
        [JsonIgnore]
        public string? UserId { get; set; }

        [Required(ErrorMessage = "Customer Name is required")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must start with 0 and contain exactly 10 digits")]
        public string CustomerPhone { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email format")]
        public string CustomerEmail { get; set; }

        [MinLength(1, ErrorMessage = "Order must contain at least one item")]
        public List<CheckoutItemDto> Items { get; set; } = new List<CheckoutItemDto>();
    }
}
