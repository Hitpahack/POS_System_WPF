using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.AddInventory;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.ServiceLibrary.AddInventory.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly INotificationService _notificationService;

        private readonly Logger _logger;

        public InvoiceService(INotificationService notificationService, Logger logger)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Response<AddStockProductViewModel>> GetInvoiceDetails(int invoiceID, CancellationToken token = default(CancellationToken))
        {
            try
            {
                token.ThrowIfCancellationRequested();

                if (invoiceID <= 0)
                {
                    return new Response<AddStockProductViewModel>
                    {
                        IsSuccess = true,
                        Error = "Invalid invoice id"
                    };
                }

                var _result = await HttpHelper.GetAsync<Response<AddStockProductViewModel>>
                        ($"{AppConstants.GET_INVOICE_DETAILS}/{invoiceID}", AppConstants.ACCESS_TOKEN, token);


                return _result;

            }
            catch (OperationCanceledException _ex)
            {
                return new Response<AddStockProductViewModel>
                {
                    Error = _ex.Message,
                    IsSuccess = false
                };
            }
            catch (Exception _Ex)
            {

                _notificationService.ShowMessage(_Ex.Message, Common.NotificationType.Error);

                return new Response<AddStockProductViewModel>
                {
                    Error = _Ex.Message,
                    IsSuccess = false
                };
            }

        }

        public async Task<Response<IEnumerable<InvoiceListViewModel>>> GetInvoices(CancellationToken token = default(CancellationToken))
        {

            try
            {
                var _result = await HttpHelper
                    .GetAsync<Response<IEnumerable<InvoiceListViewModel>>>
                    (AppConstants.GET_INVOICE_LIST, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _Ex)
            {
                _notificationService.ShowMessage($"{_Ex.Message}", Common.NotificationType.Error);

                return new Response<IEnumerable<InvoiceListViewModel>>
                {
                    IsSuccess = false,
                };
            }

        }

        public async Task<Response<string>> UpdateInvoiceDetails(int invoiceID, UpdateInvoiceViewModel updateInvoice, CancellationToken token = default(CancellationToken))
        {

            if (invoiceID <= 0)
            {
                _notificationService.ShowMessage("Invalid Invoice Id", Common.NotificationType.Error);

                return new Response<string> { IsSuccess = false };
            }


            try
            {

                var _result = await HttpHelper.
                    PutAsync<UpdateInvoiceViewModel, Response<string>>
                    ($"{AppConstants.UPDATE_INVOICE_DETAILS}/{invoiceID}", updateInvoice, AppConstants.ACCESS_TOKEN, token);
                return _result;
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception _Ex)
            {
                _logger.LogError("Error in update invoice", _Ex);

                _notificationService.ShowMessage("An error occurred ,try again", Common.NotificationType.Error);

            }
            return new Response<string> { IsSuccess = false };

        }



        public async Task<Response<InvoiceVm>> GetDefectiveLsit(InvoiceModelRequest modelRequest, CancellationToken token = default(CancellationToken))
        {

            try
            {
                var _result = await HttpHelper
                    .PostAsync<InvoiceModelRequest, Response<InvoiceVm>>
                    (AppConstants.GET_DEFECTIVE_LIST, modelRequest, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _Ex)
            {
                _notificationService.ShowMessage($"{_Ex.Message}", Common.NotificationType.Error);

                return new Response<InvoiceVm>
                {
                    IsSuccess = false,
                };
            }

        }

        public async Task<Response<IEnumerable<StockProductViewModel>>> GetSalesDefectiveLsit(CancellationToken token = default(CancellationToken))
        {

            try
            {
                var _result = await HttpHelper
                    .GetAsync<Response<IEnumerable<StockProductViewModel>>>
                    (AppConstants.GET_SALES_DEFECTIVE_LIST, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _Ex)
            {
                _notificationService.ShowMessage($"{_Ex.Message}", Common.NotificationType.Error);

                return new Response<IEnumerable<StockProductViewModel>>
                {
                    IsSuccess = false,
                };
            }

        }


        public async Task<Response<List<StockProductViewModel>>> GetProductDetails(string InoviceNo, CancellationToken token = default(CancellationToken))
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper
                    .PostAsync<string, Response<List<StockProductViewModel>>>
                    (AppConstants.GET_DEFECTIVEPRODUCT_DETAILS, InoviceNo, AppConstants.ACCESS_TOKEN, token);


                //var _result = await HttpHelper.PostAsync<string, Response<List<StockProductViewModel>>>
                //       { AppConstants.GET_DEFECTIVEPRODUCT_DETAILS},InoviceNo, AppConstants.TOKEN, token);


                return _result;

            }
            catch (OperationCanceledException _ex)
            {
                return new Response<List<StockProductViewModel>>
                {
                    Error = _ex.Message,
                    IsSuccess = false
                };
            }
            catch (Exception _Ex)
            {

                _notificationService.ShowMessage(_Ex.Message, Common.NotificationType.Error);

                return new Response<List<StockProductViewModel>>
                {
                    Error = _Ex.Message,
                    IsSuccess = false
                };
            }

        }

    }
}
