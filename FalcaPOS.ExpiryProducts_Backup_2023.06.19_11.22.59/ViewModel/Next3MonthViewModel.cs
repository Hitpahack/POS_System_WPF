﻿using FalcaPOS.Common;
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
    public class Next3MonthViewModel : BindableBase
    {
        private readonly IExpiryProductsService _expiryProductService;

        private readonly INotificationService _notificationService;

        private readonly IStoreService _storeService;

        private readonly ProgressService _progressService;

        public DelegateCommand RefreshCommand { get; private set; }

        public DelegateCommand Next3ProductCommand { get; private set; }

        private ICommonService _commonService;

        private readonly Logger _logger;

        private readonly IEventAggregator _eventAggregator;

        public Next3MonthViewModel(IEventAggregator eventAggregator, Logger logger, ICommonService commonService, IExpiryProductsService expiryProductService, IStoreService storeService, INotificationService notificationService, ProgressService progressService)
        {
            _expiryProductService = expiryProductService ?? throw new ArgumentNullException(nameof(expiryProductService));

            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _progressService = progressService;

            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));


            RefreshCommand = new DelegateCommand(LoadNext3Month);

            LoadNext3Month();


            IsExportEnabled = false;

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _eventAggregator.GetEvent<ThreeExpiryEvent>().Subscribe(x => LoadNext3Month());


            _eventAggregator.GetEvent<ExpiryThreeEvent>().Subscribe(x => ExpiryProductExport());
        }
        public async void LoadNext3Month()
        {
            try
            {

                await Task.Run(async () =>
                {

                    var _result = await _expiryProductService.GetNext3Month();

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
                                Next3Month = new ObservableCollection<ExpiryStockProductViewModel>(stockProductViewModels);
                                TotalProduct = "Total " + _result.Data.Count() + " Products";
                                IsExportEnabled = true;
                            }
                            else
                            {


                            }


                        });
                    }
                    else

                    {


                    }


                });

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);

            }
        }

        public string NumberOfdays(DateTime? expirydate)
        {
            var timespan = DateTime.UtcNow - expirydate.Value.Date;
            return timespan.Duration().Humanize(maxUnit: TimeUnit.Year, precision: 2, minUnit: TimeUnit.Day) + " after";

        }

        private ObservableCollection<ExpiryStockProductViewModel> _next3Month;

        public ObservableCollection<ExpiryStockProductViewModel> Next3Month
        {
            get => _next3Month;
            set => SetProperty(ref _next3Month, value);
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
                if (Next3Month != null && Next3Month.Count > 0)
                {
                    List<ExpiryProductExportModle> expiryProductExports = new List<ExpiryProductExportModle>();

                    foreach (var item in Next3Month)
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


                    bool _export = _commonService.ExportToXL(expiryProductExports, "Next3MonthReport", false, "Next3Month", ApplicationSettings.EXPIRY_PATH.ToString());
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
                    _notificationService.ShowMessage("No Expiry Prpduct", Common.NotificationType.Error);
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

    }
}
