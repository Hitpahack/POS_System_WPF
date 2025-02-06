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
    public class CurrentMonthViewModel : BindableBase
    {
        private readonly IExpiryProductsService _expiryProductService;

        private readonly INotificationService _notificationService;

        private readonly IStoreService _storeService;

        private readonly ProgressService _progressService;
        public DelegateCommand RefreshCommand { get; private set; }

        public DelegateCommand<object> CurrentMonthsExpiredProductsDataLoadingCommand { get; private set; }

        private ICommonService _commonService;

        private readonly Logger _logger;

        private readonly IEventAggregator _eventAggregator;

        public DelegateCommand<object> CurrentMonthExpiryProductCommandPopup { get; private set; }

        public DelegateCommand<object> UpdateExpiyDateCommand { get; private set; }
        public DelegateCommand<object> ResetCommand { get; private set; }

        public CurrentMonthViewModel(IEventAggregator eventAggregator, Logger logger, ICommonService commonService, IExpiryProductsService expiryProductService, IStoreService storeService, INotificationService notificationService, ProgressService progressService)
        {
            _expiryProductService = expiryProductService ?? throw new ArgumentNullException(nameof(expiryProductService));
            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _progressService = progressService;

            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            RefreshCommand = new DelegateCommand(LoadCurrentMonth);

            CurrentMonthExpiryProductCommandPopup = new DelegateCommand<object>(CurrentMonthExpiryPopup);

            ResetCommand = new DelegateCommand<object>(Reset);

            UpdateExpiyDateCommand = new DelegateCommand<object>(UpdateCurrentMonthExpiryDate);

            IsEditOptionEnabled = AppConstants.USER_ROLES[0] == AppConstants.ROLE_CONTROL_MANAGER ? true : false;

            LoadCurrentMonth();

            CurrentMonthsExpiredProductsDataLoadingCommand = new DelegateCommand<object>(CurrentMonthsExpiredProductsDataLoading);

            IsExportEnabled = false;

            GlobalStore = (AppConstants.USER_ROLES[0] == AppConstants.ROLE_STORE_PERSON || AppConstants.USER_ROLES[0] == AppConstants.ROLE_BACKEND) ? false : true;

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _eventAggregator.GetEvent<CurrentExpiryEvent>().Subscribe(x => LoadCurrentMonth());

            _eventAggregator.GetEvent<ExpiryCurrentEvent>().Subscribe(x => ExpiryProductExport());
        }

        private void CurrentMonthsExpiredProductsDataLoading(object obj)
        {
            if (obj != null)
            {
                ResetTelerikGridFilters.ClearTelerikGridViewFilters(obj);
            }
        }

        /// <summary>
        /// Resets the New Expiry Date value to null.
        /// </summary>
        /// <param name="obj"></param>
        private void Reset(object obj)
        {
            NewExpiryDate = null;
        }

        public async void LoadCurrentMonth()
        {
            try
            {
                await _progressService.StartProgressAsync();

                await Task.Run(async () =>
                {

                    var _result = await _expiryProductService.GetCurrentMonth();

                    if (_result != null && _result.IsSuccess && _result.Data != null)
                    {
                        Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            List<ExpiryStockProductViewModel> stockProductViewModels = new List<ExpiryStockProductViewModel>();

                            foreach (var item in _result.Data)
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
                                    Zone=item.Zone,
                                    Territory=item.Territory,
                                });
                            }
                            CurrentMonth = new ObservableCollection<ExpiryStockProductViewModel>(stockProductViewModels);
                            TotalProduct = "Total " + _result.Data.Count() + " Products";

                            IsExportEnabled = true;
                        });
                    }

                });

            }
            catch (Exception _ex)
            {


            }
            finally
            {
                await _progressService.StopProgressAsync();
            }

        }

        public string NumberOfdays(DateTime? expirydate)
        {
            try
            {
                if (expirydate.Value.Day == DateTime.UtcNow.Day)
                {
                    return "Today";
                }
                else if (expirydate.Value.Day == DateTime.UtcNow.Date.AddDays(1).Day)
                {
                    return "Tomorrow";
                }
                else if (expirydate.Value.Day > DateTime.UtcNow.Day)
                {
                    var timespan = DateTime.UtcNow - expirydate.Value.Date;
                    return timespan.Duration().Humanize(maxUnit: TimeUnit.Year, precision: 2, minUnit: TimeUnit.Day) + " after";

                }
                else
                {
                    var timespan = DateTime.UtcNow - expirydate.Value.Date;
                    return timespan.Duration().Humanize(maxUnit: TimeUnit.Year, precision: 2, minUnit: TimeUnit.Day) + " before";

                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// This method updates the expiry date of the products that are under current month expiry date 
        /// </summary>
        /// <param name="param"></param>
        public void UpdateCurrentMonthExpiryDate(object param)
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

        /// <summary>
        /// Opens the Expiry date update popup.
        /// </summary>
        /// <param name="obj"></param>
        public async void CurrentMonthExpiryPopup(object obj)
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
                    await DialogHost.Show(updateExpiryPopup, "RootDialog", CurrentMonthExpirypopupClosingEventHandler);
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        /// <summary>
        /// This method is the closing event handler for Expiry popup.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="eventArgs">event args</param>
        private async void CurrentMonthExpirypopupClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {
                var _viewmodel = (CurrentMonthViewModel)eventArgs.Parameter;
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
                                LoadCurrentMonth();
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

        private ObservableCollection<ExpiryStockProductViewModel> _currentMonth;

        public ObservableCollection<ExpiryStockProductViewModel> CurrentMonth
        {
            get => _currentMonth;
            set => SetProperty(ref _currentMonth, value);
        }

        private string _totalProduct;
        public string TotalProduct
        {
            get => _totalProduct;
            set => SetProperty(ref _totalProduct, value);
        }

        public void ExpiryProductExport()
        {
            try
            {
                if (CurrentMonth != null && CurrentMonth.Count > 0)
                {
                    List<ExpiryProductExportModle> expiryProductExports = new List<ExpiryProductExportModle>();

                    foreach (var item in CurrentMonth)
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


                    bool _export = _commonService.ExportToXL(expiryProductExports, "ExpiryCurrentMonthReport", false, "CurrentMonth", ApplicationSettings.EXPIRY_PATH.ToString());
                    if (_export)
                    {

                        _notificationService.ShowMessage("Product exported successfully and file is exported to C:\\FALCAPOS\\PosReports folder", Common.NotificationType.Success);

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
