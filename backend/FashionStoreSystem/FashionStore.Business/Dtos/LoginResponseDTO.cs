using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Dtos
{
    public  class LoginResponseDTO
    {
        
        public string? Username { get; set; }
        public string Token { get; set; }
    }
}
