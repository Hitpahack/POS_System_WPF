using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Stock;
using FalcaPOS.ExpiryProducts.View;
using Humanizer;
using Humanizer.Localisation;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FalcaPOS.ExpiryProducts.ViewModel
{
    public class NextMonthViewModel : BindableBase
    {
        private readonly IExpiryProductsService _expiryProductService;

        private readonly INotificationService _notificationService;

        private readonly IStoreService _storeService;

        private readonly ProgressService _progressService;

        public DelegateCommand<object> NextMonthsExpiredProductsDataLoadingCommand { get; private set; }

        public DelegateCommand RefreshCommand { get; private set; }

        private readonly Logger _logger;

        private ICommonService _commonService;

        private readonly IEventAggregator _eventAggregator;

        public DelegateCommand<object> NextMonthExpiryProductCommandPopup { get; private set; }

        public DelegateCommand<object> UpdateExpiyDateCommand { get; private set; }
        public DelegateCommand<object> ResetCommand { get; private set; }

        public NextMonthViewModel(IEventAggregator eventAggregator, Logger logger, ICommonService commonService, IExpiryProductsService expiryProductService, IStoreService storeService, INotificationService notificationService, ProgressService progressService)
        {
            _expiryProductService = expiryProductService ?? throw new ArgumentNullException(nameof(expiryProductService));

            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _progressService = progressService;

            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            NextMonthsExpiredProductsDataLoadingCommand = new DelegateCommand<object>(NextMonthsExpiredProductsDataLoading);

            RefreshCommand = new DelegateCommand(LoadNextMonth);

            NextMonthExpiryProductCommandPopup = new DelegateCommand<object>(NextMonthExpiryPopup);

            ResetCommand = new DelegateCommand<object>(Reset);

            UpdateExpiyDateCommand = new DelegateCommand<object>(UpdateNextMonthExpiryDate);

            IsEditOptionEnabled = AppConstants.USER_ROLES[0] == AppConstants.ROLE_CONTROL_MANAGER ? true : false;

            LoadNextMonth();

            IsExportEnabled = false;

            GlobalStore = (AppConstants.USER_ROLES[0] == AppConstants.ROLE_STORE_PERSON || AppConstants.USER_ROLES[0] == AppConstants.ROLE_BACKEND) ? false : true;

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _eventAggregator.GetEvent<NextExpiryEvent>().Subscribe(x => LoadNextMonth());

            _eventAggregator.GetEvent<ExpiryNextEvent>().Subscribe(x => ExpiryProductExport());
        }

        private void NextMonthsExpiredProductsDataLoading(object obj)
        {
            if (obj != null)
            {
                ResetTelerikGridFilters.ClearTelerikGridViewFilters(obj);
            }
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
                                        DateOfExpiryAsString = item.DateOfExpiry.Value.ToShortDateString(),
                                        DeptCode = item.DeptCode,
                                        StockProductId = item.StockProductId,
                                        SalesInvoice = NumberOfdays(item.DateOfExpiry),
                                        Zone = item.Zone,
                                        Territory = item.Territory,
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

        private void Reset(object obj)
        {
            NewExpiryDate = null;
        }

        public void UpdateNextMonthExpiryDate(object param)
        {
            try
            {
                if (string.IsNullOrEmpty(NewExpiryDate))
                {
                    _notificationService.ShowMessage("Please enter New Expiry Date", Common.NotificationType.Error);
                    return;
                }
                var _dt = Convert.ToDateTime(NewExpiryDate);
                if (_dt <= DateTime.Now)
                {
                    _notificationService.ShowMessage("New ExpiryDate should be above the Today Date", Common.NotificationType.Error);
                    return;
                }

                var TargetClose = ((Button)(param));
                var dynamicCommand = TargetClose.Command;
                dynamicCommand.CanExecute(true);
                //pass Model as Arguments
                dynamicCommand.Execute(this);
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public async void NextMonthExpiryPopup(object obj)
        {
            try
            {
                var _expiryModel = (ExpiryStockProductViewModel)obj;
                if (_expiryModel != null)
                {
                    ExpiryProductPopup = _expiryModel;
                    UpdateExpiryPopup updateExpiryPopup = new UpdateExpiryPopup();
                    updateExpiryPopup.DataContext = this;
                    NewExpiryDate = null;
                    await DialogHost.Show(updateExpiryPopup, "RootDialog", NextThreeMonthExpirypopupClosingEventHandler);
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private async void NextThreeMonthExpirypopupClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {
                var _viewmodel = (NextMonthViewModel)eventArgs.Parameter;
                if (_viewmodel != null)
                {
                    await Task.Run(async () =>
                    {
                        var _result = await _expiryProductService.ExpiryUpdateDateProduct(_viewmodel.ExpiryProductPopup.StockProductId, NewExpiryDate);

                        if (_result != null && _result.IsSuccess)
                        {
                            // Use Dispatcher to update UI on the UI thread
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                _notificationService.ShowMessage(_result.Data, Common.NotificationType.Success);
                                LoadNextMonth();
                            });
                        }
                        else
                        {
                            // Use Dispatcher to update UI on the UI thread
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                _notificationService.ShowMessage(_result.Error, Common.NotificationType.Error);
                            });
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            finally
            {
                // Any cleanup code if needed
            }
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
                            //DepartCode = item.DeptCode,
                            Brand = item.ManufactureName,
                            ProductName = item.ProductName,
                            ProductSKU = item.ProductSKU,
                            Status = item.Status,
                            Qty = item.ProductSubQty,
                            Days = item.SalesInvoice,
                            ExpiredDate = item.DateOfExpiry.Value.ToString("dd-MM-yyyy"),
                            StoreName = item.StoreName,
                            Zone = item.Zone,
                            Territory = item.Territory,
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

        private ExpiryStockProductViewModel _expiryProductPopup;

        /// <summary>
        /// This property gets and sets the expirystockproductviewmodel
        /// </summary>
        public ExpiryStockProductViewModel ExpiryProductPopup
        {
            get => _expiryProductPopup;
            set => SetProperty(ref _expiryProductPopup, value);
        }


        private string _newExpiryDate;

        /// <summary>
        /// This property gets and sets the new expiry date.
        /// </summary>
        public string NewExpiryDate
        {
            get => _newExpiryDate;
            set => SetProperty(ref _newExpiryDate, value);
        }

        private bool _isEditOptionEnabled;

        /// <summary>
        /// This property gets and sets the IsEditOptionEnabled values.
        /// </summary>
        public bool IsEditOptionEnabled
        {
            get { return _isEditOptionEnabled; }
            set { SetProperty(ref _isEditOptionEnabled, value); }
        }

    }
}
