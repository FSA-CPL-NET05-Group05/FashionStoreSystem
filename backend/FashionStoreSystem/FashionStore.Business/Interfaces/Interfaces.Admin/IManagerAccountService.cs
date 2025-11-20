using FashionStore.Business.Common.Common.Admin;
using FashionStore.Business.Dtos.Dtos.Admin;
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
        Task<AccountDTO?> GetByIdAsync(Guid id, CancellationToken ct = default);

        
        Task<OperationResult> LockUserAsync(Guid targetId, Guid performedById, string reason, CancellationToken ct = default);  // performedById: ID của admin thực hiện hành động , targetId: ID của user bị khóa
        Task<OperationResult> UnlockUserAsync(Guid targetId, Guid performedById, string? reason = null, CancellationToken ct = default);

        




    }
}
