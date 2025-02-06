using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.StockAge;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FalcaPOS.ServiceLibrary.StockAge.Services
{
    internal class StockAgeService : IStockAgeService
    {
        public async Task<Response<List<StockAgeDataDuration>>> GetStockAgeReport(string FromDate, string ToDate)
        {
            var _result = await HttpHelper.GetAsync<Response<List<StockAgeDataDuration>>>($"{AppConstants.STOCKAGE_GETSTOCKAGEREPORT}/{FromDate}/{ToDate}", AppConstants.ACCESS_TOKEN);
            if (_result != null)
                return _result;
            return null;

        }

        public async Task<Response<List<StockAgeSellingDataDuration>>> GetStockAgeReportStore(string FromDate, string ToDate)
        {
            var _result = await HttpHelper.GetAsync<Response<List<StockAgeSellingDataDuration>>>($"{AppConstants.STOCKAGE_GETSTOCKAGEREPORT_STORE}/{FromDate}/{ToDate}", AppConstants.ACCESS_TOKEN);
            if (_result != null)
                return _result;
            return null;

        }
    }
}
