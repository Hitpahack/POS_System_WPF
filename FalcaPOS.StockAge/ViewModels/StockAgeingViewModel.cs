using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.StockAge;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FalcaPOS.StockAge.ViewModels
{

    public class StockAgeingViewModel : BindableBase
    {

        private IEventAggregator _eventAggregator;

        private readonly Logger _logger;

        private readonly INotificationService _notificationService;

        private readonly IStockAgeService _stockAgeService;
        public DelegateCommand DownloadStockAgeCommand { get; private set; }
       
        private ICommonService _commonService;

        private readonly ProgressService _progressService;
        public StockAgeingViewModel(IEventAggregator eventAggregator, Logger logger, IStockAgeService stockAgeService, ICommonService commonService, INotificationService notificationService, ProgressService progressService)
        {
            _eventAggregator = eventAggregator;
            _logger = logger;
            _stockAgeService = stockAgeService;
            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));
            DownloadStockAgeCommand = new DelegateCommand(DownloadData);
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));


        }



        private async void DownloadStockAgeing()
        {

            try
            {
                if (string.IsNullOrEmpty(FromDate))
                {
                    _notificationService.ShowMessage("Please enter From Date", Common.NotificationType.Error);
                    return;
                }
                if (string.IsNullOrEmpty(ToDate))
                {
                    _notificationService.ShowMessage("Please enter To Date", Common.NotificationType.Error);
                    return;
                }
                if (FromDate != null && ToDate != null)
                {

                    if (Convert.ToDateTime(ToDate) < Convert.ToDateTime(FromDate))
                    {
                        _notificationService.ShowMessage("To Date can not be less  than From Date", Common.NotificationType.Error);
                        return;
                    }
                }
                await _progressService.StartProgressAsync();

                var _result = await _stockAgeService.GetStockAgeReport(FromDate, ToDate);
                if (_result != null && _result.IsSuccess)
                {
                    foreach (var item in _result.Data)
                    {
                        int nofDays = (DateTime.Now.Date - item.PurchaseDate.Date).Days;
                        if (nofDays == 0) item.Today = item.TotalCost;
                        if (nofDays >= 1 && nofDays <= 15) item.Days_1_15 = item.TotalCost;
                        if (nofDays >= 16 && nofDays <= 30) item.Days_15_30 = item.TotalCost;
                        if (nofDays >= 31 && nofDays <= 45) item.Days_31_45 = item.TotalCost;
                        if (nofDays >= 46 && nofDays <= 60) item.Days_46_60 = item.TotalCost;
                        if (nofDays >= 61 && nofDays <= 75) item.Days_61_75 = item.TotalCost;
                        if (nofDays >= 76 && nofDays <= 90) item.Days_76_90 = item.TotalCost;
                        if (nofDays >= 91 && nofDays <= 105) item.Days_91_105 = item.TotalCost;
                        if (nofDays >= 106 && nofDays <= 120) item.Days_106_120 = item.TotalCost;
                        if (nofDays > 120) item.Above120Days = item.TotalCost;
                    }

                    var groupByResult = from result in _result.Data
                                        group result by result.SKU into s
                                        select new { sku = s.Key, wac = s.Sum(x => x.Qty * x.Cost), qty = s.Sum(x => x.Qty) };


                    List<StockAgeDataDuration> WACResult = new List<StockAgeDataDuration>();

                    foreach (var item in _result.Data)
                    {


                        var Weighted = groupByResult != null ? groupByResult.FirstOrDefault(x => x.sku == item.SKU) != null ? groupByResult.FirstOrDefault(x => x.sku == item.SKU) : null : null;
                        var wac = Weighted != null ? Weighted.wac / Weighted.qty : 0;
                        var totalCost = (item.Qty * (wac != 0 ? wac : item.Cost));
                        WACResult.Add(new StockAgeDataDuration()
                        {

                            Supplier = item.Supplier,
                            Category = item.Category,
                            SubCategory = item.SubCategory,
                            Brand = item.Brand,
                            Product = item.Product,
                            PurchaseDate = item.PurchaseDate,
                            Units = item.Units,
                            SKU = item.SKU,
                            Qty = item.Qty,
                            Cost = wac != 0 ? wac : item.Cost,
                            TotalCost = totalCost,
                            Store = "NA",
                            Zone = item.Zone,
                            Territory = item.Territory,
                            FinancialYear = item.FinancialYear,
                            FiscalDay = item.FiscalDay,
                            FiscalDayOfMonth = item.FiscalDayOfMonth,
                            FiscalDayOfTheQuarter = item.FiscalDayOfTheQuarter,
                            FiscalDayOfTheYear = item.FiscalDayOfTheYear,
                            FiscalMonth = item.FiscalMonth,
                            FiscalMonthOfTheQuarter = item.FiscalMonthOfTheQuarter,
                            FiscalMonthOfTheYear = item.FiscalMonthOfTheYear,
                            FiscalQuarter = item.FiscalQuarter,
                        });

                        foreach (var ite in WACResult)
                        {
                            int nofDays = (DateTime.Now - ite.PurchaseDate).Days;
                            if (nofDays == 0) ite.Today = ite.TotalCost;
                            if (nofDays >= 1 && nofDays <= 15) ite.Days_1_15 = ite.TotalCost;
                            if (nofDays >= 16 && nofDays <= 30) ite.Days_15_30 = ite.TotalCost;
                            if (nofDays >= 31 && nofDays <= 45) ite.Days_31_45 = ite.TotalCost;
                            if (nofDays >= 46 && nofDays <= 60) ite.Days_46_60 = ite.TotalCost;
                            if (nofDays >= 61 && nofDays <= 75) ite.Days_61_75 = ite.TotalCost;
                            if (nofDays >= 76 && nofDays <= 90) ite.Days_76_90 = ite.TotalCost;
                            if (nofDays >= 91 && nofDays <= 105) ite.Days_91_105 = ite.TotalCost;
                            if (nofDays >= 106 && nofDays <= 120) ite.Days_106_120 = ite.TotalCost;
                            if (nofDays > 120) ite.Above120Days = ite.TotalCost;
                        }
                    }

                    bool _export = _commonService.ExportToXL(_result.Data, WACResult, "Invoice Rate", "WeightedAveragePrice", "POSStockAgeingReport", false);
                    if (_export)
                    {

                        _notificationService.ShowMessage("Stock age is exported successfully and file is exported to C:\\FALCAPOS\\PosReports folder", Common.NotificationType.Success);

                    }
                    else
                    {
                        _notificationService.ShowMessage("File export failed , try again", Common.NotificationType.Error);
                    }
                }
                else
                    _notificationService.ShowMessage(_result.Error, NotificationType.Error);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            finally
            {
                await _progressService.StopProgressAsync();

            }
        }

        private async void DownloadStockAgeingStore()
        {

            try
            {
                if (string.IsNullOrEmpty(FromDate))
                {
                    _notificationService.ShowMessage("Please enter From Date", Common.NotificationType.Error);
                    return;
                }
                if (string.IsNullOrEmpty(ToDate))
                {
                    _notificationService.ShowMessage("Please enter To Date", Common.NotificationType.Error);
                    return;
                }
                if (FromDate != null && ToDate != null)
                {

                    if (Convert.ToDateTime(ToDate) < Convert.ToDateTime(FromDate))
                    {
                        _notificationService.ShowMessage("To Date can not be less  than From Date", Common.NotificationType.Error);
                        return;
                    }
                }
                await _progressService.StartProgressAsync();

                var _result = await _stockAgeService.GetStockAgeReportStore(FromDate, ToDate);
                if (_result != null && _result.IsSuccess)
                {
                    foreach (var item in _result.Data)
                    {
                        int nofDays = (DateTime.Now.Date - item.PurchaseDate.Date).Days;
                        if (nofDays == 0) item.Today = item.TotalSellingPrice;
                        if (nofDays >= 1 && nofDays <= 15) item.Days_1_15 = item.TotalSellingPrice;
                        if (nofDays >= 16 && nofDays <= 30) item.Days_15_30 = item.TotalSellingPrice;
                        if (nofDays >= 31 && nofDays <= 45) item.Days_31_45 = item.TotalSellingPrice;
                        if (nofDays >= 46 && nofDays <= 60) item.Days_46_60 = item.TotalSellingPrice;
                        if (nofDays >= 61 && nofDays <= 75) item.Days_61_75 = item.TotalSellingPrice;
                        if (nofDays >= 76 && nofDays <= 90) item.Days_76_90 = item.TotalSellingPrice;
                        if (nofDays >= 91 && nofDays <= 105) item.Days_91_105 = item.TotalSellingPrice;
                        if (nofDays >= 106 && nofDays <= 120) item.Days_106_120 = item.TotalSellingPrice;
                        if (nofDays > 120) item.Above120Days = item.TotalSellingPrice;
                    }

                    var groupByResult = from result in _result.Data
                                        group result by result.SKU into s
                                        select new { sku = s.Key, was = s.Sum(x => x.Qty * x.SellingPrice), qty = s.Sum(x => x.Qty) };


                    List<StockAgeSellingDataDuration> WACResult = new List<StockAgeSellingDataDuration>();

                    foreach (var item in _result.Data)
                    {


                        var Weighted = groupByResult != null ? groupByResult.FirstOrDefault(x => x.sku == item.SKU) != null ? groupByResult.FirstOrDefault(x => x.sku == item.SKU) : null : null;
                        var was = Weighted != null ? Weighted.was / Weighted.qty : 0;
                        var totalSelling = (item.Qty * (was != 0 ? was : item.SellingPrice));
                        WACResult.Add(new StockAgeSellingDataDuration()
                        {

                            Supplier = item.Supplier,
                            Category = item.Category,
                            SubCategory = item.SubCategory,
                            Brand = item.Brand,
                            Product = item.Product,
                            PurchaseDate = item.PurchaseDate,
                            Units = item.Units,
                            SKU = item.SKU,
                            Qty = item.Qty,
                            SellingPrice = was != 0 ? was : item.SellingPrice,
                            TotalSellingPrice = totalSelling,
                            Store = "NA",
                            Zone = item.Zone,
                            Territory = item.Territory,
                            FinancialYear = item.FinancialYear,
                            FiscalDay = item.FiscalDay,
                            FiscalDayOfMonth = item.FiscalDayOfMonth,
                            FiscalDayOfTheQuarter = item.FiscalDayOfTheQuarter,
                            FiscalDayOfTheYear = item.FiscalDayOfTheYear,
                            FiscalMonth = item.FiscalMonth,
                            FiscalMonthOfTheQuarter = item.FiscalMonthOfTheQuarter,
                            FiscalMonthOfTheYear = item.FiscalMonthOfTheYear,
                            FiscalQuarter = item.FiscalQuarter,


                        });

                        foreach (var ite in WACResult)
                        {
                            int nofDays = (DateTime.Now - ite.PurchaseDate).Days;
                            if (nofDays == 0) ite.Today = ite.TotalSellingPrice;
                            if (nofDays >= 1 && nofDays <= 15) ite.Days_1_15 = ite.TotalSellingPrice;
                            if (nofDays >= 16 && nofDays <= 30) ite.Days_15_30 = ite.TotalSellingPrice;
                            if (nofDays >= 31 && nofDays <= 45) ite.Days_31_45 = ite.TotalSellingPrice;
                            if (nofDays >= 46 && nofDays <= 60) ite.Days_46_60 = ite.TotalSellingPrice;
                            if (nofDays >= 61 && nofDays <= 75) ite.Days_61_75 = ite.TotalSellingPrice;
                            if (nofDays >= 76 && nofDays <= 90) ite.Days_76_90 = ite.TotalSellingPrice;
                            if (nofDays >= 91 && nofDays <= 105) ite.Days_91_105 = ite.TotalSellingPrice;
                            if (nofDays >= 106 && nofDays <= 120) ite.Days_106_120 = ite.TotalSellingPrice;
                            if (nofDays > 120) ite.Above120Days = ite.TotalSellingPrice;
                        }
                    }

                    bool _export = _commonService.ExportToXL(_result.Data, WACResult, "SellingPrice", "WeightedAveragePrice", "POSStockAgeingReport_Store", false);
                    if (_export)
                    {

                        _notificationService.ShowMessage("Stock age is exported successfully and file is exported to C:\\FALCAPOS\\PosReports folder", Common.NotificationType.Success);

                    }
                    else
                    {
                        _notificationService.ShowMessage("File export failed , try again", Common.NotificationType.Error);
                    }
                }
                else
                    _notificationService.ShowMessage(_result.Error, NotificationType.Error);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            finally
            {
                await _progressService.StopProgressAsync();

            }
        }

        public void DownloadData()
        {
            try
            {
                if (AppConstants.USER_ROLES[0] == AppConstants.ROLE_STORE_PERSON)
                    DownloadStockAgeingStore();
                else
                    DownloadStockAgeing();
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private string _fromDate;

        public string FromDate
        {
            get => _fromDate;
            set => SetProperty(ref _fromDate, value);
        }

        private string _toDate;

        public string ToDate
        {
            get => _toDate;
            set => SetProperty(ref _toDate, value);
        }

    }


}
