using FalcaPOS.Common.Helper;
using FalcaPOS.Entites.Sales;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{
    public interface IRSPservice
    {
        Task<Response<List<BusinessModelResponse>>> GetRSPSummarySearch(int? StoreId, string FromDate, string ToDate, CancellationToken token = default(CancellationToken));
    }
}
