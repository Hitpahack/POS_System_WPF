using FalcaPOS.Common.Helper;
using FalcaPOS.Entites.Stock;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{
    public interface IClosingStockReportService
    {
        Task<Response<List<ClosingStockDetails>>> ClosingStocks(ClosingStockSearch search, CancellationToken token = default);
    }
}
