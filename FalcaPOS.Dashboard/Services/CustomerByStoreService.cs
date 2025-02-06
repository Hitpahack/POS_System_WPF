using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Entites.Dashboard;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Dashboard.Services
{
    public class CustomerByStoreService : ICustomerByStoreService
    {
        private readonly Logger _logger;

        public CustomerByStoreService(Logger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<Response<DashboardCustomer>> GetCustomerByStore(int districtId = 0, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper
                    .GetAsync<Response<DashboardCustomer>>
                    ($"{AppConstants.GET_CUSTOMERS_BY_STORE}?districtId={districtId}", AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (OperationCanceledException)
            {
                _logger.LogError("Operation was cancelled by user get customer by store");
            }

            catch (Exception _ex)
            {
                _logger.LogError("Error in getting customer by stre", _ex);
            }

            return new Response<DashboardCustomer>
            {
                IsSuccess = false,
                Error = "An error occurred ,try again"
            };

        }

    }
}
