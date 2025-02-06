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
    public class ExpriedProductViewModel : BindableBase
    {

        private readonly IExpiryProductsService _expiryProductService;

        private readonly INotificationService _notificationService;

        private readonly IStoreService _storeService;

        private readonly ProgressService _progressService;

        private readonly Logger _logger;

        public DelegateCommand<object> RefreshCommand { get; private set; }

        public DelegateCommand<object> ExpiredProductsDataLoadingCommand { get; private set; }

        public DelegateCommand<object> ExpirProductCommandPopup { get; private set; }

        private ICommonService _commonService;

        public DelegateCommand<object> UpdateExpiyDateCommand { get; private set; }
        public DelegateCommand<object> ResetCommand { get; private set; }

        private readonly IEventAggregator _eventAggregator;



        public ExpriedProductViewModel(IEventAggregator eventAggregator, ICommonService commonService, Logger logger, IExpiryProductsService expiryProductService, IStoreService storeService, INotificationService notificationService, ProgressService progressService)
        {

            _expiryProductService = expiryProductService ?? throw new ArgumentNullException(nameof(expiryProductService));

            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));

            _progressService = progressService;

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            RefreshCommand = new DelegateCommand<object>(LoadExpired);

            ExpiredProductsDataLoadingCommand = new DelegateCommand<object>(ExpiredProductsDataLoading);


            ExpirProductCommandPopup = new DelegateCommand<object>(ExpiryPopup);

            ResetCommand = new DelegateCommand<object>(Reset);

            UpdateExpiyDateCommand = new DelegateCommand<object>(UpdateExpiryDate);
            

            EditOptionEnabled = (AppConstants.USER_ROLES[0] == AppConstants.ROLE_PURCHASE_MANAGER || AppConstants.USER_ROLES[0] == AppConstants.ROLE_FINANCE || AppConstants.USER_ROLES[0]==AppConstants.ROLE_CONTROL_MANAGER) ? true : false;
            //it should Global converter
            GlobalStore = (AppConstants.USER_ROLES[0] == AppConstants.ROLE_STORE_PERSON || AppConstants.USER_ROLES[0] == AppConstants.ROLE_BACKEND) ? false : true;
            LoadExpired(null);

            IsExportEnabled = false;

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _eventAggregator.GetEvent<ExpiredEvent>().Subscribe(x => LoadExpired(null));

            _eventAggregator.GetEvent<ExpiryExportEvent>().Subscribe(x => ExpiryProductExport());
        }

        private void ExpiredProductsDataLoading(object obj)
        {
            if (obj != null)
            {
                ResetTelerikGridFilters.ClearTelerikGridViewFilters(obj);
            }
        }

        private void Reset(object obj)
        {
            NewExpiryDate = null;
        }

        public async void LoadExpired(object obj)
        {
            try
            {
                await Task.Run(async () =>
                {

                    var _result = await _expiryProductService.ExpiredProduct();

                    if (_result != null && _result.IsSuccess && _result.Data != null)
                    {
                        Application.Current?.Dispatcher?.Invoke(() =>
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
                                    SalesInvoice = NumberOfdays(item.DateOfExpiry),
                                    StockProductId = item.StockProductId,
                                    //reusing props
                                    IsSelected = (AppConstants.USER_ROLES[0] == AppConstants.ROLE_PURCHASE_MANAGER || AppConstants.USER_ROLES[0] == AppConstants.ROLE_FINANCE) ? true : false,
                                    Zone = item.Zone,
                                    Territory = item.Territory,
                                });
                            }
                            ExpiredProduct = new ObservableCollection<ExpiryStockProductViewModel>(stockProductViewModels);
                            TotalProduct = "Total " + _result.Data.Count() + " Products";
                            IsExportEnabled = true;

                        });
                    }

                });

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);

            }
        }

        public void ExpiryProductExport()
        {
            try
            {
                if (ExpiredProduct != null && ExpiredProduct.Count > 0)
                {
                    List<ExpiryProductExportModle> expiryProductExports = new List<ExpiryProductExportModle>();

                    foreach (var item in ExpiredProduct)
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
                            Zone=item.Zone,
                            Territory=item.Territory,
                        });
                    }


                    bool _export = _commonService.ExportToXL(expiryProductExports, "ExpiredReport", false);
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

        public async void ExpiryPopup(object obj)
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
                    await DialogHost.Show(updateExpiryPopup, "RootDialog", ExpirypopupColsingEventHandler);
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private async void ExpirypopupColsingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {

                var _viewmodel = (ExpriedProductViewModel)eventArgs.Parameter;
                if (_viewmodel != null)
                {

                    await Task.Run(async () =>
                    {

                        var _result = await _expiryProductService.ExpiryUpdateDateProduct(_viewmodel.ExpiryProductPopup.StockProductId, NewExpiryDate);

                        if (_result != null && _result.IsSuccess)
                        {
                            _notificationService.ShowMessage(_result.Data, Common.NotificationType.Success);
                            LoadExpired(null);
                            return;
                        }
                        else
                        {
                            _notificationService.ShowMessage(_result.Error, Common.NotificationType.Error);
                            return;
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

            }
        }

        public void UpdateExpiryDate(object param)
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

        private ExpiryStockProductViewModel _expiryProductPopup;

        public ExpiryStockProductViewModel ExpiryProductPopup
        {
            get => _expiryProductPopup;
            set => SetProperty(ref _expiryProductPopup, value);
        }


        private string _newExpiryDate;
        public string NewExpiryDate
        {
            get => _newExpiryDate;
            set => SetProperty(ref _newExpiryDate, value);
        }


        private ObservableCollection<ExpiryStockProductViewModel> _expiredProduct;

        public ObservableCollection<ExpiryStockProductViewModel> ExpiredProduct
        {
            get => _expiredProduct;
            set => SetProperty(ref _expiredProduct, value);
        }

        private string _totalProduct;

        public string TotalProduct
        {
            get => _totalProduct;
            set => SetProperty(ref _totalProduct, value);
        }


        public string NumberOfdays(DateTime? expirydate)
        {
            var timespan = DateTime.UtcNow - expirydate.Value.Date;
            return timespan.Days == 0 ? "Today" : timespan.Duration().Humanize(maxUnit: TimeUnit.Year, precision: 2, minUnit: TimeUnit.Day) + (timespan.Days > 0 ? " before" : " after");

        }

        private bool _isExportEnabled;
        public bool IsExportEnabled
        {
            get => _isExportEnabled;
            set => _ = SetProperty(ref _isExportEnabled, value);
        }

        private bool _editOPtionEnabled;

        public bool EditOptionEnabled
        {
            get { return _editOPtionEnabled; }
            set { SetProperty(ref _editOPtionEnabled, value); }
        } 
        private bool _GlobalStore;

        public bool GlobalStore
        {
            get => _GlobalStore; 
            set => SetProperty(ref _GlobalStore, value); 
        }

    }

    public class ExpiryProductExportModle
    {
        public string Category { get; set; }
        public string SubCategory { get; set; }
        //public string DepartCode { get; set; }
        public string Brand { get; set; }
        public string ProductName { get; set; }
        public string ProductSKU { get; set; }
        public string Status { get; set; }
        public int Qty { get; set; }
        public string ExpiredDate { get; set; }
        public string Days { get; set; }
        public string StoreName { get; set; }
        public string Zone { get; set; }
        public string Territory { get; set; }
    }
}
