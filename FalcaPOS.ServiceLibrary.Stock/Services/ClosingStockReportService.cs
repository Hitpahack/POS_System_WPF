using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Stock;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.ServiceLibrary.Stock.Services
{
    public class ClosingStockReportService : IClosingStockReportService
    {


        public ClosingStockReportService(Logger logger)
        {

        }

        public async Task<Response<List<ClosingStockDetails>>> ClosingStocks(ClosingStockSearch search, CancellationToken token)
        {

            try
            {
                token.ThrowIfCancellationRequested();

                Response<List<ClosingStockDetails>> _result = await HttpHelper.PostAsync<ClosingStockSearch, Response<List<ClosingStockDetails>>>
                    (AppConstants.GET_CLOSING_STOCK_REPORT, search, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
            }
            return new Response<List<ClosingStockDetails>>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };

        }
    }
}
