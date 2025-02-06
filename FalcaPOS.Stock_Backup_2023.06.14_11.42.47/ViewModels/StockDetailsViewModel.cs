using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Sales;
using FalcaPOS.Entites.Suppliers;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;


namespace FalcaPOS.Stock.ViewModels
{
    public class StockDetailsViewModel : BindableBase
    {
        private readonly IStockService stockService;
        private readonly INotificationService notificationService;
        private readonly Logger logger;
        private ObservableCollection<SlotProducts> Products;

        private ISupplierService supplierService;

        private readonly IEventAggregator eventAggregator;

        private readonly ProgressService ProgressService;

        public DelegateCommand RefreshCommand { get; private set; }

        public StockDetailsViewModel(ProgressService _ProgressService, IStockService StockService, INotificationService NotificationService, Logger Logger, ISupplierService SupplierService, IEventAggregator EventAggregator)
        {
            ProgressService = _ProgressService;

            stockService = StockService ?? throw new ArgumentNullException(nameof(StockService));

            notificationService = NotificationService ?? throw new ArgumentNullException(nameof(notificationService));

            logger = Logger ?? throw new ArgumentNullException(nameof(logger));

            SearchCommand = new DelegateCommand(SearchCommandExecute);

            supplierService = SupplierService ?? throw new ArgumentNullException(nameof(supplierService));

            eventAggregator = EventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            eventAggregator.GetEvent<SignalRSupplierAddedEvent>().Subscribe(NewSupplierAdded, ThreadOption.PublisherThread);

            eventAggregator.GetEvent<SignalRSupplierEnableDisableEvent>().Subscribe(SupplierEnableDisable, ThreadOption.PublisherThread);

            LoadSuppliersAsync();

            RefreshCommand = new DelegateCommand(RefreshGrid);

        }



        private void SupplierEnableDisable(object obj)
        {
            if (obj is SuppliersViewModel _splr)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (Suppliers == null)
                        {
                            Suppliers = new ObservableCollection<SuppliersViewModel>();
                        }

                        //remove from list of suppliers.
                        var _exSupplier = Suppliers.FirstOrDefault(x => x.SupplierId == _splr.SupplierId);

                        if (_exSupplier != null)
                        {
                            _ = Suppliers.Remove(_exSupplier);
                        }
                        if (!_splr.Isenabled)
                        {
                            return;
                        }

                        Suppliers.Add(_splr);

                        Suppliers = new ObservableCollection<SuppliersViewModel>(Suppliers.OrderBy(x => x.Name));
                    });

                }
                catch (Exception ex)
                {
                    logger?.LogError("Error in supplier enable disable", ex);
                }
            }
        }

        private void NewSupplierAdded(object obj)
        {
            if (obj is SuppliersViewModel _suplr)
            {
                try
                {
                    Application.Current.Dispatcher?.Invoke(() =>
                    {

                        if (Suppliers == null)
                        {
                            Suppliers = new ObservableCollection<SuppliersViewModel> { _suplr };

                            return;
                        }

                        if (Suppliers.Any(x => x.SupplierId == _suplr.SupplierId))
                        {
                            var _exstSupplier = Suppliers.FirstOrDefault(x => x.SupplierId == _suplr.SupplierId);

                            if (_exstSupplier != null)
                            {
                                Suppliers.Remove(_exstSupplier);
                            }
                        }

                        Suppliers.Add(_suplr);

                        Suppliers = new ObservableCollection<SuppliersViewModel>(Suppliers.OrderBy(x => x.Name));


                    });
                }
                catch (Exception ex)
                {
                    logger.LogError("Error in adding supplier ", ex);
                }
            }

        }

        private ObservableCollection<SuppliersViewModel> suppliers;
        public ObservableCollection<SuppliersViewModel> Suppliers
        {
            get => suppliers;
            set => SetProperty(ref suppliers, value);
        }

        private SuppliersViewModel selectedSupplier;
        public SuppliersViewModel SelectedSupplier
        {
            get => selectedSupplier;
            set => SetProperty(ref selectedSupplier, value);
        }

        private async void LoadSuppliersAsync()
        {
            try
            {
                await Task.Run(async () =>
                {
                    var _suppliers = await supplierService.GetSuppliers("isenabled=true");

                    if (_suppliers != null && _suppliers.Count() > 0)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Suppliers = new ObservableCollection<SuppliersViewModel>(_suppliers);
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                logger.LogError("Error in loading suppliers", ex);

            }
        }

        private string invoiceNo;


        private string searchText;

        private string invoiceDate;

        private int? invoiceQty;

        private string suppliername;

        private int totalStock;

        private int totalSold;

        public DelegateCommand SearchCommand { get; set; }

        public string SearchText
        {
            get { return searchText; }
            set { SetProperty(ref searchText, value); }
        }
        public string InvoiceNo
        {
            get { return invoiceNo; }
            set { SetProperty(ref invoiceNo, value); }
        }
        public string InvoiceDate
        {
            get { return invoiceDate; }
            set { SetProperty(ref invoiceDate, value); }
        }

        public int? InvoiceQty
        {
            get { return invoiceQty; }
            set { SetProperty(ref invoiceQty, value); }
        }

        public string SupplierName
        {
            get { return suppliername; }
            set { SetProperty(ref suppliername, value); }
        }
        public int TotalStock
        {
            get { return totalStock; }
            set { SetProperty(ref totalStock, value); }
        }
        public int TotalSold
        {
            get { return totalSold; }
            set { SetProperty(ref totalSold, value); }
        }


        public ObservableCollection<SlotProducts> SlotProducts
        {
            get { return Products; }
            set
            {
                SetProperty(ref Products, value);
            }
        }

        private CancellationTokenSource _cancellationTokenSource;


        private async void SearchCommandExecute()
        {
            if (SelectedSupplier == null)
            {

                notificationService.ShowMessage("Select a supplier", Common.NotificationType.Error);

                return;

            }

            if (!SearchText.IsValidString())
            {
                notificationService.ShowMessage("Enter valid invoice number", Common.NotificationType.Error);

                return;
            }


            try
            {
                ClearData();

                logger.LogInformation($"User search for invoice details {SearchText}");

                if (_cancellationTokenSource != null)
                    _cancellationTokenSource.Cancel();

                _cancellationTokenSource = new CancellationTokenSource();

                await ProgressService.StartProgressAsync();

                await Task.Run(async () =>
                {

                    var _result = await stockService.GetSlotWiseInvoiceProductDetails(SearchText, (int)SelectedSupplier.SupplierId, _cancellationTokenSource.Token);

                    if (_result?.Data != null && _result.IsSuccess)
                    {

                        Application.Current.Dispatcher?.Invoke(() =>
                        {
                            _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                            InvoiceNo = _result.Data.InvoiceNo;
                            InvoiceDate = _result.Data.InvoiceDate?.ToString("dd-MM-yyyy");
                            InvoiceQty = _result.Data.InvoiceQty;
                            SupplierName = _result.Data.SupplierName;
                            //var d = _result.Data.ProductDetails.SelectMany(x => x.ProductInvoices).GroupBy(x => x.Status).Select(x => new { Key = x.Key, Count = x.Count() });
                            TotalStock = _result.Data.TotalStock - _result.Data.TotalSold;//d.Where(x => x.Key.Contains("Stock")).Select(x => x.Count).FirstOrDefault();
                            TotalSold = _result.Data.TotalSold; //d.Where(x => x.Key.Contains("Sold")).Select(x => x.Count).FirstOrDefault();
                            SlotProducts = new ObservableCollection<SlotProducts>(_result.Data.ProductDetails);

                        }, System.Windows.Threading.DispatcherPriority.Normal, _cancellationTokenSource.Token);

                        await ProgressService.StopProgressAsync();
                    }
                    else
                    {
                        notificationService.ShowMessage($"{_result.Error ?? "No data found"}", Common.NotificationType.Error);
                        await ProgressService.StopProgressAsync();
                    }


                }, _cancellationTokenSource.Token);
                await ProgressService.StopProgressAsync();

            }
            catch (OperationCanceledException)
            {
                logger.LogError("Error in loading serach");
            }
            catch (Exception ex)
            {
                logger.LogError("Error in loading serach", ex);
            }

        }

        private void ClearData()
        {
            SlotProducts = null;
        }

        public void RefreshGrid()
        {
            ClearData();
            SelectedSupplier = null;
            SearchText = null;
        }
    }


}
