using FalcaPOS.Common.Helper;
using FalcaPOS.Entites.StockAge;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{
    public interface IStockAgeService
    {
        Task<Response<List<StockAgeDataDuration>>> GetStockAgeReport(string FromDate, string ToDate);

        Task<Response<List<StockAgeSellingDataDuration>>> GetStockAgeReportStore(string FromDate, string ToDate);
    }
}
