using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.ServiceLibrary.AddInventory.Services
{
    public class StockService : IStockService
    {
        

        public async Task<bool> AddStockProduct(AddStockProduct stockProduct, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var _result = await HttpHelper.PostAsync(AppConstants.ADD_STOCK_PRODUCT, stockProduct, AppConstants.TOKEN,cancellationToken);                              

                return _result;

            }
            catch (Exception _ex)
            {

            }
            return false;

        }
    }
}
