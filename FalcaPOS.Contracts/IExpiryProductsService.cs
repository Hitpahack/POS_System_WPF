using FalcaPOS.Common.Helper;
using FalcaPOS.Entites.Stock;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{
    public interface IExpiryProductsService
    {
        Task<Response<IEnumerable<ExpiryStockProductViewModel>>> GetCurrentMonth(CancellationToken token = default(CancellationToken));

        Task<Response<IEnumerable<ExpiryStockProductViewModel>>> GetNextMonth(CancellationToken token = default(CancellationToken));

        Task<Response<IEnumerable<ExpiryStockProductViewModel>>> GetNext3Month(CancellationToken token = default(CancellationToken));

        Task<Response<IEnumerable<ExpiryStockProductViewModel>>> GetNext6Month(CancellationToken token = default(CancellationToken));

        Task<Response<IEnumerable<ExpiryStockProductViewModel>>> ExpiredProduct(CancellationToken token = default(CancellationToken));

        Task<Response<string>> ExpiryUpdateDateProduct(int StockProductId, string ExpiryDate, CancellationToken token = default(CancellationToken));
        
        Task<Response<string>> ClearExpiryProductsCacheOnRefresh(CancellationToken token = default);
    }
}
