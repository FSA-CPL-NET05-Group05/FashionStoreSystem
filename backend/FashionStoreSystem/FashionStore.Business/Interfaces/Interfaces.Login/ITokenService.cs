using FashionStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Interfaces.Interfaces.Login
{
    public  interface  ITokenService
    {
        string CreateToken( AppUser user);
    }
}
