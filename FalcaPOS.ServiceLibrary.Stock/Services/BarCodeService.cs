using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.ServiceLibrary.Stock.Services
{
    public class BarCodeService : IBarCodeService
    {
        private readonly INotificationService _notificationService;

        public BarCodeService(INotificationService notificationService)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public async Task<Response<string>> GetBarCode(int productId, CancellationToken token = default(CancellationToken))
        {

            try
            {
                var _result = await HttpHelper.GetAsync<Response<string>>
                    ($"{AppConstants.GET_PRODUCT_BARCODE}/{productId}", AppConstants.ACCESS_TOKEN, token);

                if (_result != null && !_result.IsSuccess && _result.Error.IsValidString())
                {
                    _notificationService.ShowMessage(_result.Error, Common.NotificationType.Error);
                }

                return _result;
            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, Common.NotificationType.Error);
            }

            return new Response<string>
            {
                IsSuccess = false
            };
        }
    }
}
