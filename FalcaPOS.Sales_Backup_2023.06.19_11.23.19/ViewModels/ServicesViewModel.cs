using FalcaPOS.Common;
using FalcaPOS.Common.Events;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Sales;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Sales.ViewModels
{
    public class ServicesViewModel : BindableBase
    {
        private readonly ISalesService _salesService;

        private readonly IEventAggregator _eventAggregator;

        public DelegateCommand InvoiceSearchCommand { get; private set; }
        public DelegateCommand ResetServiceCommand { get; private set; }

        public DelegateCommand<object> RemoveProductCommand { get; private set; }



        public ServicesViewModel(ISalesService salesService, IEventAggregator eventAggregator)
        {
            _salesService = salesService;
            _eventAggregator = eventAggregator;
            InvoiceSearchCommand = new DelegateCommand(async () => await InvoiceSearch());

            RemoveProductCommand = new DelegateCommand<object>(RemoveProduct);

            ResetServiceCommand = new DelegateCommand(ResetService);
        }

        private void ResetService()
        {
            ClearFormData();
        }

        private void RemoveProduct(object obj)
        {
            if (InvoiceDetails != null && (obj is SalesProduct _salesProduct))
            {

                InvoiceDetails.SalesProducts.Remove(_salesProduct);
                InvoiceDetails = InvoiceDetails;

            }
        }

        private void ShowAlert(string msg, NotificationType notificationType)
            => _eventAggregator.GetEvent<NotifyMessage>()
                               .Publish(new Common.Models.ToastMessage
                               {
                                   Message = msg,
                                   MessageType = notificationType
                               });


        private CancellationTokenSource _cancellationToken;


        private async Task InvoiceSearch()
        {
            if (string.IsNullOrWhiteSpace(InvoiceNumber))
            {
                ShowAlert("Enter valid invoice number", NotificationType.Error);
                return;
            }

            InvoiceDetails = null;

            if (_cancellationToken != null)
            {
                _cancellationToken.Cancel();

            }
            _cancellationToken = new CancellationTokenSource();


            await Task.Run(async () =>
            {
                var _result = await _salesService
                                    .GetServiceProducts(InvoiceNumber, _cancellationToken.Token);

                if (_result != null)
                    InvoiceDetails = _result;


            });

        }



        #region Props

        private AddSales _invoiceDetails;
        public AddSales InvoiceDetails
        {
            get { return _invoiceDetails; }
            set { SetProperty(ref _invoiceDetails, value); }
        }

        private bool _isOldCustomer;
        public bool IsOldCustomer
        {
            get { return _isOldCustomer; }
            set
            {
                SetProperty(ref _isOldCustomer, value);
                ClearFormData();
            }
        }

        private void ClearFormData()
        {
            InvoiceNumber = null;
            InvoiceDetails = null;
        }

        private string _invoiceNumber;
        public string InvoiceNumber
        {
            get { return _invoiceNumber; }
            set { SetProperty(ref _invoiceNumber, value); }
        }





        #endregion
    }
}
