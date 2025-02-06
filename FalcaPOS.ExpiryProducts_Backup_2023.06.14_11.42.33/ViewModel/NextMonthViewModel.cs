using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Stock;
using Humanizer;
using Humanizer.Localisation;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.ExpiryProducts.ViewModel
{
    public class NextMonthViewModel : BindableBase
    {
        private readonly IExpiryProductsService _expiryProductService;

        private readonly INotificationService _notificationService;

        private readonly IStoreService _storeService;

        private readonly ProgressService _progressService;

        public DelegateCommand RefreshCommand { get; private set; }

        private readonly Logger _logger;

        private ICommonService _commonService;


        private readonly IEventAggregator _eventAggregator;


        public NextMonthViewModel(IEventAggregator eventAggregator, Logger logger, ICommonService commonService, IExpiryProductsService expiryProductService, IStoreService storeService, INotificationService notificationService, ProgressService progressService)
        {
            _expiryProductService = expiryProductService ?? throw new ArgumentNullException(nameof(expiryProductService));

            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _progressService = progressService;

            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));


            RefreshCommand = new DelegateCommand(LoadNextMonth);

            LoadNextMonth();


            IsExportEnabled = false;

            GlobalStore = (AppConstants.USER_ROLES[0] == AppConstants.ROLE_STORE_PERSON || AppConstants.USER_ROLES[0] == AppConstants.ROLE_BACKEND) ? false : true;

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _eventAggregator.GetEvent<NextExpiryEvent>().Subscribe(x => LoadNextMonth());

            _eventAggregator.GetEvent<ExpiryNextEvent>().Subscribe(x => ExpiryProductExport());

            
        }
        public async void LoadNextMonth()
        {
            try
            {

                await Task.Run(async () =>
                {

                    var _result = await _expiryProductService.GetNextMonth();

                    if (_result != null && _result.IsSuccess)
                    {
                        Application.Current?.Dispatcher?.Invoke(() =>
                        {

                            if (_result != null && _result.IsSuccess && _result.Data != null)
                            {
                                var result = _result.Data;
                                List<ExpiryStockProductViewModel> stockProductViewModels = new List<ExpiryStockProductViewModel>();

                                foreach (var item in result)
                                {
                                    stockProductViewModels.Add(new ExpiryStockProductViewModel()
                                    {
                                        Category = item.Category,
                                        ProductTypeName = item.ProductTypeName,
                                        ManufactureName = item.ManufactureName,
                                        ProductSKU = item.ProductSKU,
                                        ProductName = item.ProductName,
                                        ProductSubQty = item.ProductSubQty,
                                        Status = item.Status,
                                        StoreName = item.StoreName,
                                        DateOfExpiry = item.DateOfExpiry,
                                        DeptCode = item.DeptCode,
                                        SalesInvoice = NumberOfdays(item.DateOfExpiry)
                                    });
                                }
                                NextMonth = new ObservableCollection<ExpiryStockProductViewModel>(stockProductViewModels);
                                TotalProduct = "Total " + _result.Data.Count() + " Products";
                                IsExportEnabled = true;
                            }
                           
                        });
                    }
                    
                });

            }
            catch (Exception)
            {


            }
        }

        public string NumberOfdays(DateTime? expirydate)
        {
            var timespan = DateTime.UtcNow - expirydate.Value.Date;
            return timespan.Days == 0 ? "Today" : timespan.Duration().Humanize(maxUnit: TimeUnit.Year, precision: 2, minUnit: TimeUnit.Day) + (timespan.Days > 0 ? " before" : " after");


        }

        private ObservableCollection<ExpiryStockProductViewModel> _nextMonth;

        public ObservableCollection<ExpiryStockProductViewModel> NextMonth
        {
            get { return _nextMonth; }
            set { SetProperty(ref _nextMonth, value); }
        }

        private string _totalProduct;

        public string TotalProduct
        {
            get { return _totalProduct; }
            set { SetProperty(ref _totalProduct, value); }
        }

        public void ExpiryProductExport()
        {
            try
            {
                if (NextMonth != null && NextMonth.Count > 0)
                {
                    List<ExpiryProductExportModle> expiryProductExports = new List<ExpiryProductExportModle>();

                    foreach (var item in NextMonth)
                    {
                        expiryProductExports.Add(new ExpiryProductExportModle()
                        {
                            Category = item.Category,
                            SubCategory = item.ProductTypeName,
                            DepartCode = item.DeptCode,
                            Brand = item.ManufactureName,
                            ProductName = item.ProductName,
                            ProductSKU = item.ProductSKU,
                            Status = item.Status,
                            Qty = item.ProductSubQty,
                            Days = item.SalesInvoice,
                            ExpiredDate = item.DateOfExpiry.Value.ToString("dd-MM-yyyy"),
                            StoreName = item.StoreName,


                        });
                    }


                    bool _export = _commonService.ExportToXL(expiryProductExports, "NextMonthReport", false, "NextMonth", ApplicationSettings.EXPIRY_PATH.ToString());
                    if (_export)
                    {

                        _notificationService.ShowMessage("Product is exported successfully and file is exported to C:\\FALCAPOS\\PosReports folder", Common.NotificationType.Success);

                    }
                    else
                    {
                        _notificationService.ShowMessage("File export failed , try again", Common.NotificationType.Error);
                    }

                }
                else
                {
                    _notificationService.ShowMessage("No Expiry Product", Common.NotificationType.Error);
                    return;
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private bool _isExportEnabled;
        public bool IsExportEnabled
        {
            get => _isExportEnabled;
            set => _ = SetProperty(ref _isExportEnabled, value);
        }

        private bool _GlobalStore;

        public bool GlobalStore {
            get => _GlobalStore;
            set => SetProperty(ref _GlobalStore, value);
        }

    }
}
