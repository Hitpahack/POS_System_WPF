using FalcaPOS.Common.Helper;
using FalcaPOS.Entites.Stores;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{
    public interface IStoreService
    {
        Task<IEnumerable<Store>> GetStores(string queryString = null);
        Task<IEnumerable<Entites.Stores.StoreLicense>> GetStoreLicense(int StoreId, CancellationToken token = default(CancellationToken));
        Task<IEnumerable<Store>> GetStoreDetailsbyuser(int userid, string role, CancellationToken token = default(CancellationToken));
        Task<Response<string>> AddStore(Store store, CancellationToken token = default(CancellationToken));
       Task<Response<string>> UpdateStore(Store store, CancellationToken token = default(CancellationToken));

        Task<Response<string>> GetStoreInvoiceFormat(int districtId, CancellationToken token = default(CancellationToken));

    }
}
