using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Stock;
using System;
using System.Data;
using System.Threading.Tasks;

namespace FalcaPOS.ServiceLibrary.Stock.Services
{
    public class StoreStockService : IStoreStockService
    {
        public async Task<DataSet> GetStoreStockSearch(StockSearchModel stockModelRequest)
        {
            try
            {

                var _result = await HttpHelper.PostAsyncDataSet<StockSearchModel, DataSet>(AppConstants.GET_STORE_SEARCH, stockModelRequest, AppConstants.ACCESS_TOKEN);

                return _result;
            }
            catch (Exception _ex)
            {

            }
            return null;
        }


        public async Task<StoreStockModelResponse> GetStockDetails(long ProductID)
        {
            try
            {

                var _result = await HttpHelper.PostAsync<long, StoreStockModelResponse>(AppConstants.GET_PRODUCT_DETAILS, ProductID, AppConstants.ACCESS_TOKEN);

                return _result;
            }
            catch (Exception _ex)
            {

            }
            return null;
        }
    }
}
