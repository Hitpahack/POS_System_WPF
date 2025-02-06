using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Stock;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.ServiceLibrary.ExpiryProducts.Services
{
    public class ExpiryProductService : IExpiryProductsService
    {
        private readonly INotificationService _notificationService;

        private readonly Logger _logger;

        public ExpiryProductService(INotificationService notificationService, Logger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public async Task<Response<IEnumerable<ExpiryStockProductViewModel>>> GetCurrentMonth(CancellationToken token = default)
        {

            try
            {

                var _result = await HttpHelper.GetAsync<Response<IEnumerable<ExpiryStockProductViewModel>>>(AppConstants.GET_CURRENT_MONTH, AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            return new Response<IEnumerable<ExpiryStockProductViewModel>>
            {
                IsSuccess = false,
                Error = "No records found."
            };


        }

        public async Task<Response<IEnumerable<ExpiryStockProductViewModel>>> GetNextMonth(CancellationToken token = default)
        {

            try
            {

                var _result = await HttpHelper.GetAsync<Response<IEnumerable<ExpiryStockProductViewModel>>>(AppConstants.GET_NEXT_MONTH, AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            return new Response<IEnumerable<ExpiryStockProductViewModel>>
            {
                IsSuccess = false,
                Error = "No records found."
            };


        }

        public async Task<Response<IEnumerable<ExpiryStockProductViewModel>>> GetNext3Month(CancellationToken token = default)
        {

            try
            {

                var _result = await HttpHelper.GetAsync<Response<IEnumerable<ExpiryStockProductViewModel>>>(AppConstants.GET_NEXT3_MONTH, AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            return new Response<IEnumerable<ExpiryStockProductViewModel>>
            {
                IsSuccess = false,
                Error = "No records found."
            };


        }
        public async Task<Response<IEnumerable<ExpiryStockProductViewModel>>> GetNext6Month(CancellationToken token = default)
        {

            try
            {

                var _result = await HttpHelper.GetAsync<Response<IEnumerable<ExpiryStockProductViewModel>>>(AppConstants.GET_NEXT6_MONTH, AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            return new Response<IEnumerable<ExpiryStockProductViewModel>>
            {
                IsSuccess = false,
                Error = "No records found."
            };


        }

        public async Task<Response<IEnumerable<ExpiryStockProductViewModel>>> ExpiredProduct(CancellationToken token = default)
        {

            try
            {

                var _result = await HttpHelper.GetAsync<Response<IEnumerable<ExpiryStockProductViewModel>>>(AppConstants.GET_EXPIRED_PRODUCT, AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            return new Response<IEnumerable<ExpiryStockProductViewModel>>
            {
                IsSuccess = false,
                Error = "No records found."
            };


        }

        public async Task<Response<string>> ExpiryUpdateDateProduct(int StockProductId, string ExpiryDate, CancellationToken token = default)
        {

            try
            {

                var _result = await HttpHelper.PostAsync<string, Response<string>>($"{AppConstants.UPDATE_EXPIRY_DATE}/{StockProductId}", ExpiryDate, AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "No records found."
            };


        }

        /// <summary>
        /// This service is used to call the API that clears the Cache of Expiry Products on Clicking Refresh.
        /// </summary>
        /// <param name="token">Cancellation token</param>
        /// <returns>Returns data to indicate whether the task was successful or not.</returns>
        public async Task<Response<string>> ClearExpiryProductsCacheOnRefresh(CancellationToken token = default)
        {
            try
            {
                // Calls the API to clear cache and stores the result.
                var _result = await HttpHelper.GetAsync<Response<string>>($"{AppConstants.CLEAR_EXPIRY_CACHE_ON_REFRESH}", AppConstants.ACCESS_TOKEN, token);
                return _result;
            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            // Returns error in case of any exceptions.
            return new Response<string>
            {
                IsSuccess = false,
                Error = "An error occured."
            };
        }
    }
}
