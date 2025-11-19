using FashionStore.Business.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Business.Interfaces.Interfaces.Admin
{
    public interface IManagerAccountService
    {

        Task<List<AccountDTO>> GetAllAsync(CancellationToken ct = default);
        




    }
}
