//using FalcaPOS.Common.Constants;
//using FalcaPOS.Common.Helper;
//using FalcaPOS.Contracts;
//using Prism.Events;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace FalcaPOS.ServiceLibrary.AddInventory.Services
//{
//    public class InvoiceGenerateService : IInvoiceGenerateService
//    {
//        private readonly INotificationService _notificationService;

//        public InvoiceGenerateService(INotificationService notificationService)
//        {
//            _notificationService = notificationService;
//        }

//        public async Task<IEnumerable<string>> GenerateInvoiceNumber(int count = 1, CancellationToken token = default(CancellationToken))
//        {
//            try
//            {
//                token.ThrowIfCancellationRequested();

//                var _result = await HttpHelper
//                    .GetAsync<IEnumerable<string>>
//                    ($"{ AppConstants.GENERATE_INVOICE_NUMBERS}/{count}", AppConstants.TOKEN, token);

//                return _result;


//            }
//            catch (Exception _ex)
//            {
//                _notificationService.ShowMessage(_ex.Message, Common.NotificationType.Error);

//                return Enumerable.Empty<string>();
//            }

//        }
//    }
//}
