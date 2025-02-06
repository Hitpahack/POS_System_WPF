using FalcaPOS.Common.Helper;
using FalcaPOS.Entites.Director;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{
    public interface IDirectorService
    {
        Task<Response<IEnumerable<PurchaseRateModel>>> GetPurchaseRateListServices(PurchaseRateSearchModel Search, CancellationToken token = default(CancellationToken));

        Task<Response<StoreAssertModel>> GetStoreAssert(PurchaseRateSearchModel Search, CancellationToken token = default(CancellationToken));
    }
}
