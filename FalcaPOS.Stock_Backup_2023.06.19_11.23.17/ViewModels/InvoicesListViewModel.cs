using FalcaPOS.Common.Events;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Stock;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.Stock.ViewModels
{
    public class InvoicesListViewModel : BindableBase
    {
        private readonly IStockService stockService;

        private readonly IEventAggregator eventAggregator;

        private readonly Logger logger;

        private readonly INotificationService notificationService;

        private readonly IDialogService dialogService;

        public DelegateCommand RefreshInvoiceCommand { get; private set; }

        public InvoicesListViewModel(IDialogService DialogService,
            IStockService StockService,
            Logger Logger,
            INotificationService NotificationService, IEventAggregator EventAggregator)
        {
            UpdateInvoiceCommand = new DelegateCommand<UserInvoiceListViewModel>(UpdateInvoice);

            dialogService = DialogService ?? throw new ArgumentNullException(nameof(DialogService));

            stockService = StockService ?? throw new ArgumentNullException(nameof(StockService));

            logger = Logger ?? throw new ArgumentNullException(nameof(Logger));

            notificationService = NotificationService ?? throw new ArgumentNullException(nameof(NotificationService));

            eventAggregator = EventAggregator ?? throw new ArgumentNullException(nameof(EventAggregator));

            eventAggregator.GetEvent<RefreshInvoices>().Subscribe(() => GetInvoices());

            RefreshInvoiceCommand = new DelegateCommand(() => GetInvoices(true));

            GetInvoices();


        }


        private async void GetInvoices(bool isManualRefresh = false)
        {
            try
            {

                Invoices?.Clear();

                await Task.Run(async () =>
                {

                    var _result = await stockService.GetBackendUserInvoices();

                    if (_result != null && _result.IsSuccess)
                    {
                        Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            Invoices = new ObservableCollection<UserInvoiceListViewModel>(_result.Data);

                            logger.LogInformation($"Getting backend user invoices list success --counnt{Invoices.Count}");
                        });
                    }
                    else
                    {
                        if (isManualRefresh)
                        {
                            notificationService.ShowMessage($"{_result?.Error ?? "An error occurred,try again"}", Common.NotificationType.Error);
                        }
                        logger.LogError($"Getting backend user invoice failed --{_result?.Error}");
                    }

                });
            }
            catch (Exception _ex)
            {
                logger.LogError("Error in getting backend user invoices", _ex);
            }

        }



        private void UpdateInvoice(UserInvoiceListViewModel invoiceObj)
        {
            try
            {
                if (invoiceObj is UserInvoiceListViewModel invoice)
                {

                    //allow only invoices which have same dc and invoice number to be updated.

                    if (invoice.DcNumber?.Trim()?.ToLower() != invoice.InvoiceNumber?.Trim()?.ToLower())
                    {
                        logger.LogWarning($"update invoice number  {invoice.InvoiceNumber} not possible for dc number {invoice.DcNumber} since they are diffrent");

                        notificationService.ShowMessage("Invoice number is already updated", Common.NotificationType.Information);

                        return;
                    }

                    var _date = invoice.DcNumberDate == null ? "" : invoice.DcNumberDate.Value.ToString("dd MMM yyyy");

                    dialogService.ShowDialog("UpdateInvoiceDialog",
                        parameters: new DialogParameters
                        ($"invoiceId={invoice.InvoiceId}&dcnumber={invoice.DcNumber}&supplierName={invoice.SupplierName}&storeName={invoice.StoreName}&dcDate={_date}"),
                        callback: (dialogResult) =>
                        {
                            if (dialogResult != null && dialogResult.Result == ButtonResult.OK)
                            {
                                //update success;
                                //remove old invoince and add updated one //or refresh grid
                                GetInvoices();
                            }

                        });
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error in opening update invoice dialog", ex);
            }
        }

        public DelegateCommand<UserInvoiceListViewModel> UpdateInvoiceCommand { get; private set; }



        private ObservableCollection<UserInvoiceListViewModel> invoices;


        public ObservableCollection<UserInvoiceListViewModel> Invoices
        {
            get { return invoices; }
            set { SetProperty(ref invoices, value); }
        }
    }

}
