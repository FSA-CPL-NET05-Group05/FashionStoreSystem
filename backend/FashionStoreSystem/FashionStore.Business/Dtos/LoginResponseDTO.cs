using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Dtos
{
    public class LoginResponseDTO
    {
        public string Id { get; set; } = null!;          
        public string Username { get; set; } = null!;
        public string Role { get; set; } = null!;        
        public string FullName { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
