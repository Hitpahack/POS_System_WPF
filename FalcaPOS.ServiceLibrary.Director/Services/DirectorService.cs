using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Director;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.ServiceLibrary.Director.Services
{
    public class DirectorService : IDirectorService
    {

        private readonly INotificationService _notificationService;

        private readonly Logger _logger;

        public DirectorService(INotificationService notificationService, Logger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }


        public async Task<Response<IEnumerable<PurchaseRateModel>>> GetPurchaseRateListServices(PurchaseRateSearchModel Search, CancellationToken token = default)
        {

            try
            {

                var _result = await HttpHelper.PostAsync<PurchaseRateSearchModel, Response<IEnumerable<PurchaseRateModel>>>(AppConstants.GET_PURCHASE_RATE_LIST, Search, AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            return new Response<IEnumerable<PurchaseRateModel>>
            {
                IsSuccess = false,
                Error = "No records found."
            };


        }

        public async Task<Response<StoreAssertModel>> GetStoreAssert(PurchaseRateSearchModel Search, CancellationToken token = default)
        {

            try
            {

                var _result = await HttpHelper.PostAsync<PurchaseRateSearchModel, Response<StoreAssertModel>>(AppConstants.GET_STORE_ASSERT, Search, AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            return new Response<StoreAssertModel>
            {
                IsSuccess = false,
                Error = "No records found."
            };


        }

    }
}
