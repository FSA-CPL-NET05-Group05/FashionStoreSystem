using FashionStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace FashionStore.Data.Interfaces.Interfaces.Admin
{
    public interface IManagerAccountRepository
    {

        
        Task<List<AppUser>> GetAllAsync(CancellationToken ct = default);  // CancellationToken tín hiệu hủy tác vụ
        


    }
}
