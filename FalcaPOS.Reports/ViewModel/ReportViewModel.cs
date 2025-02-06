using AutoMapper;
using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Reports;
using FalcaPOS.Entites.StockAge;
using FalcaPOS.Reports.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup.Localizer;
using Telerik.Windows.Data;

namespace FalcaPOS.Reports.ViewModel
{
    public partial class ReportViewModel : BindableBase
    {
        private bool _globalUser;

        private bool _filterdatadownload;

        /// <summary>
        /// This boolean is used to track whether the value selected from the Choose Columns Combo Box is All or not.
        /// </summary>
        private bool _isSelectAll;

        public bool IsFilterDataDownload
        {
            get { return _filterdatadownload; }
            set { SetProperty(ref _filterdatadownload, value); }
        }

        private bool _alldatadownload;

        public bool IsAllDataDownload
        {
            get { return _alldatadownload; }
            set { SetProperty(ref _alldatadownload, value); }
        }

        /// <summary>
        /// Stores the value of Selected Item for Choose Columns Combo box.
        /// </summary>
        private string _selectedItem;

        /// <summary>
        /// Gets and Sets the Selected Item in Choose Columns in Combo Box.
        /// </summary>
        public string SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }


        private List<String> _report;

        public List<String> ReportsList
        {
            get { return _report; }
            set { SetProperty(ref _report, value); }
        }

        private String _selectedreport;

        public String SelectedReport
        {
            get { return _selectedreport; }
            set { SetProperty(ref _selectedreport, value); }
        }


        private bool _isTallyReport;

        public bool isTallyReport
        {
            get { return _isTallyReport; }
            set { SetProperty(ref _isTallyReport, value); }
        }


        private string _fromTallyInvoiceDate;
        public string FromTallyInvoiceDate
        {
            get { return _fromTallyInvoiceDate; }
            set { SetProperty(ref _fromTallyInvoiceDate, value); }
        }

        private string _toTallyInvoiceDate;
        public string ToTallyInvoiceDate
        {
            get { return _toTallyInvoiceDate; }
            set { SetProperty(ref _toTallyInvoiceDate, value); }
        }



        /// <summary>
        /// Stores the observable collection of column data of selected report columns
        /// </summary>
        private ObservableCollection<ColumnData> _selectedReportColumns;

        /// <summary>
        /// Gets and sets the observable collection of SelectedReportColumns
        /// </summary>
        public ObservableCollection<ColumnData> SelectedReportColumns
        {
            get { return _selectedReportColumns; }
            set { SetProperty(ref _selectedReportColumns, value); }
        }

        private QueryableCollectionView _queryableCollectionView;

        public QueryableCollectionView ReportQueryableCollectionView
        {
            get { return _queryableCollectionView; }
            set { SetProperty(ref _queryableCollectionView, value); }
        }



        private ObservableCollection<ReportBaseModel> _reportbasemodel;

        public ObservableCollection<ReportBaseModel> ReportBaseModel
        {
            get { return _reportbasemodel; }
            set { SetProperty(ref _reportbasemodel, value); }
        }


        private bool _isFilterApplied;

        public bool IsFilterApplied
        {
            get { return _isFilterApplied; }
            set { SetProperty(ref _isFilterApplied, value); }
        }


        private int _noofColumns;

        public int NoofColumns
        {
            get { return _noofColumns; }
            set { SetProperty(ref _noofColumns, value); }
        }

        /// <summary>
        /// This holds the OncheckedCommand that gets called when any checkbox in the Choose Columns combo box is checked.
        /// </summary>
        public DelegateCommand<object> OnCheckedCommand { get; private set; }

        /// <summary>
        /// This holds the OncheckedCommand that gets called when any checkbox in the Choose Columns combo box is unchecked.
        /// </summary>
        public DelegateCommand<object> OnUnCheckedCommand { get; private set; }

        private List<ReportConfiguration> _reportConfiguration = null;
        public DelegateCommand DownloadReportCommand { get; private set; }
        public DelegateCommand ReportNameChangeCommand { get; private set; }
        public DelegateCommand SearchSelectedReportCommand { get; private set; }
        public DelegateCommand<object> AutoGeneratingColumnCommand { get; private set; }
        public DelegateCommand<object> FilteredCommand { get; private set; }
        public DelegateCommand<object> CheckedColumnsEvent { get; private set; }

        /// <summary>
        /// Stores the Delegate Command that is called when the SelectionChangedEvent occurs in Choose Columns Combo Box.
        /// </summary>
        public DelegateCommand<object> SelectionChangedCommand { get; private set; }

        public DelegateCommand<object> RefreshReportDataGridandPanal { get; private set; }

        private Mapper autoMapper;

        private readonly Logger _logger;

        private readonly INotificationService _notificationService;

        private readonly ProgressService _progressService;

        private readonly IReportServices _reportServices;

        private readonly ICommonService _commonService;
        private readonly IStockAgeService _stockAgeService;
        private readonly IFinanceService _financeService;
        public ReportViewModel(INotificationService notificationService,
            ProgressService progressService, Logger logger,
            IReportServices reportServices, ICommonService commonService, IStockAgeService stockAgeService, IFinanceService financeService)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));
            _financeService = financeService ?? throw new ArgumentNullException(nameof(financeService));

            DownloadReportCommand = new DelegateCommand(DownloadReportCommandEvent);

            _reportServices = reportServices ?? throw new ArgumentNullException(nameof(reportServices));

            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));
            _stockAgeService = stockAgeService ?? throw new ArgumentNullException(nameof(stockAgeService));

            _reportConfiguration = new ParseJSON().ParseJson<List<ReportConfiguration>>("ReportConfiguration");
            if (_reportConfiguration == null || _reportConfiguration.Count == 0)
            {
                _notificationService.ShowMessage("Some error occured", NotificationType.Error);
                return;
            }
            IsFilterApplied = false; NoofColumns = 0;
            ReportsList = _reportConfiguration.Where(x => x.role.Contains(AppConstants.USER_ROLES[0])).Select(x => x.name).ToList();
            ReportNameChangeCommand = new DelegateCommand(ReportNameChangeCommandEvent);
            SearchSelectedReportCommand = new DelegateCommand(SearchSelectedReportCommandEvent);
            AutoGeneratingColumnCommand = new DelegateCommand<object>(AutoGeneratingColumnCommandEvent);
            FilteredCommand = new DelegateCommand<object>(FilteredCommandEvent);
            CheckedColumnsEvent = new DelegateCommand<object>(CheckedColumnsEventMethod);
            RefreshReportDataGridandPanal = new DelegateCommand<object>(RefreshReportDataGridandPanalEvent);
            OnCheckedCommand = new DelegateCommand<object>(CheckboxCommand);
            OnUnCheckedCommand = new DelegateCommand<object>(UncheckboxCommand);
            SelectionChangedCommand = new DelegateCommand<object>(SelectionChangedEvent);
            autoMapper = AutoMapperInitialize();
            _isSelectAll = true;
            SelectedItem = null;
            isTallyReport = false;
             
            // Stores a bool value to check if the logged in person is global user or not. If not a global user, sets it to true. Else, it's false
            _globalUser = (AppConstants.USER_ROLES[0] == "storeperson" || AppConstants.USER_ROLES[0] == "backend") ? false : true;

        }

        /// <summary>
        /// This method is called when there's a selection changed event in Choose Columns Combo box.
        /// </summary>
        /// <param name="obj">Object</param>
        private void SelectionChangedEvent(object obj)
        {
            // Convert the object to SelectionChangedEventArgs
            var content = (SelectionChangedEventArgs)obj;
            if (content.AddedItems.Count != 0)
            {
                // Convert the Data to Column Data to access the properties
                var data = (ColumnData)content.AddedItems[0];
                if (data.Selected)
                {
                    data.Selected = false;
                }
                else
                {
                    data.Selected = true;
                }
            }
            SelectedItem = null;
        }

        /// <summary>
        /// This method is called when the checkbox in the choose columns combo box is unchecked.
        /// </summary>
        /// <param name="obj">object.</param>
        private void UncheckboxCommand(object obj)
        {
            try
            {
                var content = (CheckBox)obj;

                // Checks if the IsChecked value is false and content is not null
                if (content.IsChecked == false && content.Content != null)
                {
                    // If the content is 'All'
                    if (content.Content.ToString() == "All" && _isSelectAll)
                    {
                        // Sets the Selected property of all the columns to false.
                        foreach (var selectedReportColumn in SelectedReportColumns)
                        {
                            selectedReportColumn.Selected = false;
                            _isSelectAll = false;
                        }
                    }
                    else
                    {
                        if (SelectedReportColumns.Where(x => x.Selected == true && x.Name != "All").ToList().Count > 1)
                        {
                            _isSelectAll = false;
                            SelectedReportColumns[0].Selected = false;
                        }
                    }
                    _isSelectAll = true;
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        /// <summary>
        /// This method is called when the checkbox in the choose columns combo box is checked.
        /// </summary>
        /// <param name="obj">object</param>
        private void CheckboxCommand(object obj)
        {
            try
            {
                var content = (CheckBox)obj;

                // Checks if the IsChecked value is true and content is not null
                if (content.IsChecked == true && content.Content != null)
                {
                    // If the content is 'All'
                    if (content.Content.ToString() == "All" && _isSelectAll)
                    {
                        // Sets the Selected property of all the columns to false.
                        foreach (var selectedReportColumn in SelectedReportColumns)
                        {
                            selectedReportColumn.Selected = true;
                        }
                    }

                    else
                    {
                        if (SelectedReportColumns.Where(x => x.Selected == true && x.Name != "All").ToList().Count == (SelectedReportColumns.Count - 1))
                        {
                            _isSelectAll = true;
                            SelectedReportColumns[0].Selected = true;
                        }
                    }
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private void RefreshReportDataGridandPanalEvent(object obj)
        {
            try
            {
                SelectedReport = null;
                SelectedReportColumns = null;
                ReportQueryableCollectionView = null;
                ReportBaseModel = null;
                IsAllDataDownload = false;
                IsFilterDataDownload = false;
                IsFilterApplied = false;
                NoofColumns = 0;
                _isSelectAll = true;
                FromTallyInvoiceDate = null;
                ToTallyInvoiceDate = null;
                isTallyReport = false;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private void CheckedColumnsEventMethod(object obj)
        {
            if (SelectedReportColumns != null)
            {
                NoofColumns = SelectedReportColumns.Count(x => x.Selected);

                if (SelectedReportColumns.Any(x => x.Name == "All" && x.Selected == true))
                {
                    NoofColumns = NoofColumns - 1;
                }
            }

        }

        private void FilteredCommandEvent(object obj)
        {
            if (obj != null)
            {
                var _isFilterApplied = (Telerik.Windows.Controls.GridView.GridViewFilteredEventArgs)obj;
                IsFilterApplied = (_isFilterApplied != null && _isFilterApplied.Added.Count() > 0);
            }

        }

        private void AutoGeneratingColumnCommandEvent(object obj)
        {
            if (obj != null)
            {
                var column = ((Telerik.Windows.Controls.GridViewBoundColumnBase)((Telerik.Windows.Controls.GridViewAutoGeneratingColumnEventArgs)obj).Column).Header;
                var selectedColumn = SelectedReportColumns.Where(x => x.Name == column.ToString()).FirstOrDefault();
                if (selectedColumn != null && !selectedColumn.Selected)
                {
                    ((Telerik.Windows.Controls.GridViewAutoGeneratingColumnEventArgs)obj).Column.IsVisible = false;
                }
            }
        }

        private void DownloadReportCommandEvent()
        {
            try
            {
                if (!IsFilterDataDownload && !IsAllDataDownload)
                {
                    _notificationService.ShowMessage("Please select a download option", NotificationType.Error);
                    return;
                }

                var _filteredColumns = SelectedReportColumns.Where(x => x.Selected == false && x.Name.ToLower() != "all").Select(x =>
                {
                    return x.PropName;
                }).ToList().ToArray<string>();

                if (IsFilterDataDownload)
                {
                    //_commonService.ExportToXL<QueryableCollectionView>(ReportQueryableCollectionView, null, true);

                    var _reportQueryableCollectionView = JsonConvert.SerializeObject(ReportQueryableCollectionView, Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new IgnorePropertiesResolver(_filteredColumns)
                    });
                    if (_reportQueryableCollectionView != null)
                    {
                        var _childObj = JsonConvert.DeserializeObject<List<ExpandoObject>>(_reportQueryableCollectionView);
                        if (_childObj != null)
                        {
                            Export<ExpandoObject>(_childObj, SelectedReport.Contains("Tally") ? SelectedReport : $"{SelectedReport} Report");
                        }
                    }
                }
                else
                {
                    var _ReportBaseModel = JsonConvert.SerializeObject(ReportBaseModel, Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new IgnorePropertiesResolver(_filteredColumns)
                    });
                    if (_ReportBaseModel != null)
                    {
                        var _childObj = JsonConvert.DeserializeObject<List<ExpandoObject>>(_ReportBaseModel);
                        if (_childObj != null)
                        {
                            Export<ExpandoObject>(_childObj, SelectedReport.Contains("Tally") ? SelectedReport : $"{SelectedReport} Report");
                        }
                    }
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }


        public void Export<T>(List<T> data, string fileName)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            Dictionary<String, String> propDisplayColumns = new Dictionary<string, string>();
            SelectedReportColumns.Where(x => x.Selected == true && x.Name.ToLower() != "all").ToList().ForEach(x =>
            {
                if (!propDisplayColumns.Keys.Contains(x.PropName))
                    propDisplayColumns.Add(x.PropName, x.Name);

            });

            var _IsDownload = _commonService.ExportObjectToXL<T>(data, SelectedReportColumns.Where(x => x.Selected == true && x.Name.ToLower() != "all").Select(x => x.PropName).ToList(), propDisplayColumns, fileName);
            if (_IsDownload)
            {
                _notificationService.ShowMessage("File exported to folder C:\\FALCAPOS\\PosReports", Common.NotificationType.Success);
            }
            else
            {
                _notificationService.ShowMessage("File export failed , try again", Common.NotificationType.Error);
            }
        }

        private async void SearchSelectedReportCommandEvent()
        {
            if (SelectedReport == null)
            {
                _notificationService.ShowMessage("Please select a report type", NotificationType.Error);
                return;
            }
            if (SelectedReportColumns.Count(x => x.Selected) == 0 || SelectedReportColumns.Count(x => x.Selected) < 5)
            {
                _notificationService.ShowMessage("Please select minimum 5 column", NotificationType.Error);
                return;
            }

            try
            {
                ReportBaseModel = null;
                ReportQueryableCollectionView = null;
                IsFilterApplied = false;
                IsAllDataDownload = false;
                 
                await _progressService.StartProgressAsync();

                switch (SelectedReport)
                {
                    case "Inventory":
                       
                        var _result = await _reportServices.GetInventoryReport();
                        if (_result != null && _result.IsSuccess)
                        {
                            if (AppConstants.USER_ROLES[0].Contains("regionalmanager") || AppConstants.USER_ROLES[0].Contains("territorymanager"))
                            {
                                var _currentReport = autoMapper.Map<List<InventoryReportModelPM>, List<InventoryReport2>>(_result.Data.ToList());

                                Application.Current?.Dispatcher?.Invoke(() =>
                                {
                                    ReportBaseModel = new ObservableCollection<ReportBaseModel>(_currentReport);
                                    ReportQueryableCollectionView = new QueryableCollectionView(ReportBaseModel);
                                });

                            }
                            else if (AppConstants.USER_ROLES[0].Contains("purchasemanager") || AppConstants.USER_ROLES[0].Contains("falcadirector") || AppConstants.USER_ROLES[0].Contains("controlmanager"))
                            {
                                var _currentReport = autoMapper.Map<List<InventoryReportModelPM>, List<InventoryReport>>(_result.Data.ToList());

                                Application.Current?.Dispatcher?.Invoke(() =>
                                {
                                    ReportBaseModel = new ObservableCollection<ReportBaseModel>(_currentReport);
                                    ReportQueryableCollectionView = new QueryableCollectionView(ReportBaseModel);
                                });
                            }

                        }
                        break;


                    case "PO GRN":
                       
                        DateAndBoolResult dateAndBoolResultPogrn = ValidateFromAndToDate();
                        if (dateAndBoolResultPogrn.IsValid)
                        {
                            var _poGRNresult = await _reportServices.GetPOGRNReport(dateAndBoolResultPogrn.FromDate,dateAndBoolResultPogrn.ToDate);
                            if (_poGRNresult != null && _poGRNresult.IsSuccess)
                            {
                                var POGRNList = autoMapper.Map<List<InventoryReportPoGrnModel>, List<POGRNReport>>(_poGRNresult.Data.ToList());

                                Application.Current?.Dispatcher?.Invoke(() =>
                            {
                                ReportBaseModel = new ObservableCollection<ReportBaseModel>(POGRNList);
                                ReportQueryableCollectionView = new QueryableCollectionView(ReportBaseModel);
                            });
                            }
                        }
                        break;

                    case "Stock Ageing(Weighted Average Price)":
                        
                        var _sAResultCostRate = await _stockAgeService.GetStockAgeReport(DateTime.Now.AddYears(-3).ToString("dd-MM-yyyy"), DateTime.Now.ToString("dd-MM-yyyy"));
                            if (_sAResultCostRate != null && _sAResultCostRate.IsSuccess)
                            {
                                var groupByResult = from resulttemp in _sAResultCostRate.Data
                                                    group resulttemp by resulttemp.SKU into s
                                                    select new { sku = s.Key, was = s.Sum(x => x.Qty * x.Cost), qty = s.Sum(x => x.Qty) };

                                List<StockAgeDataDuration> WAPResult = new List<StockAgeDataDuration>();

                                foreach (var item in _sAResultCostRate.Data)
                                {
                                    // Calculates the weighted average for each item in the result.
                                    var Weighted = groupByResult != null ? groupByResult.FirstOrDefault(x => x.sku == item.SKU) != null ? groupByResult.FirstOrDefault(x => x.sku == item.SKU) : null : null;
                                    var was = Weighted != null ? Weighted.was / Weighted.qty : 0;
                                    var totalCost = (item.Qty * (was != 0 ? was : item.Cost));
                                    WAPResult.Add(new StockAgeDataDuration()
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
                                        Cost = was != 0 ? Math.Round(was,2) : Math.Round(item.Cost,2),
                                        TotalCost = Math.Round(totalCost,2),
                                        Store = _globalUser? "NA" : item.Store,
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

                                    // Updates the no. of days
                                    foreach (var ite in WAPResult)
                                    {
                                        int nofDays = (DateTime.Now - ite.PurchaseDate).Days;
                                        if (nofDays == 0) ite.Today = ite.Cost;
                                        if (nofDays >= 1 && nofDays <= 15) ite.Days_1_15 = ite.Cost;
                                        if (nofDays >= 16 && nofDays <= 30) ite.Days_15_30 = ite.Cost;
                                        if (nofDays >= 31 && nofDays <= 45) ite.Days_31_45 = ite.Cost;
                                        if (nofDays >= 46 && nofDays <= 60) ite.Days_46_60 = ite.Cost;
                                        if (nofDays >= 61 && nofDays <= 75) ite.Days_61_75 = ite.Cost;
                                        if (nofDays >= 76 && nofDays <= 90) ite.Days_76_90 = ite.Cost;
                                        if (nofDays >= 91 && nofDays <= 105) ite.Days_91_105 = ite.Cost;
                                        if (nofDays >= 106 && nofDays <= 120) ite.Days_106_120 = ite.Cost;
                                        if (nofDays > 120) ite.Above120Days = ite.Cost;
                                    }

                                }
                                var WeightedAverageStockAgeingResult = autoMapper.Map<List<StockAgeDataDuration>, List<StockAgeReportInvoiceRate>>(WAPResult.ToList());

                                Application.Current?.Dispatcher?.Invoke(() =>
                                {
                                    ReportBaseModel = new ObservableCollection<ReportBaseModel>(WeightedAverageStockAgeingResult);
                                    ReportQueryableCollectionView = new QueryableCollectionView(ReportBaseModel);
                                });
                            }
                        break;

                    case "Stock Ageing(Invoice Rate)":
                       
                        var SAResultCostRate = await _stockAgeService.GetStockAgeReport(DateTime.Now.AddYears(-3).ToString("dd-MM-yyyy"), DateTime.Now.ToString("dd-MM-yyyy"));
                            if (SAResultCostRate != null && SAResultCostRate.IsSuccess)
                            {
                                foreach (var item in SAResultCostRate.Data)
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
                                var StockAgeingResut = autoMapper.Map<List<StockAgeDataDuration>, List<StockAgeReportInvoiceRate>>(SAResultCostRate.Data.ToList());

                                Application.Current?.Dispatcher?.Invoke(() =>
                                {
                                    ReportBaseModel = new ObservableCollection<ReportBaseModel>(StockAgeingResut);
                                    ReportQueryableCollectionView = new QueryableCollectionView(ReportBaseModel);
                                });
                            }
                        break;

                    case "Tally Report (Sales)":
                        DateAndBoolResult _validateSalesdateRange = ValidateFromAndToDate();
                        if (_validateSalesdateRange.IsValid)
                        {

                            var salesTallyReportResult = await _financeService.GetTallyExport(new Entites.Finance.TallyExportSearchModel()
                            {
                                FromInvoiceDate = FromTallyInvoiceDate,
                                ToInvoiceDate = ToTallyInvoiceDate,
                                Type = "Sales"

                            });
                            if (salesTallyReportResult != null && salesTallyReportResult.IsSuccess && salesTallyReportResult.Data.Count() > 0)
                            {

                                List<TallyExportSalesModelReport> salesOrderModels = new List<TallyExportSalesModelReport>();
                                var lastInvoiceNo = "";
                                foreach (var item in salesTallyReportResult.Data)
                                {

                                    TallyExportSalesModelReport salesOrderModel = new TallyExportSalesModelReport()
                                    {
                                        Type = item.type,
                                        CostCenter = item.StoreName,
                                        ReferEPR = item.NoasEPR,
                                        DateEPR = item.DateasEPR.ToString("dd-MM-yyyy"),
                                        InvoiceNo = item.InvoiceNo,
                                        InvoiceDate = item.InvoiceDate.ToString("dd-MM-yyyy"),
                                        //SalesLedger = item.StoreState.ToLower() == item.State.ToLower() ? GetLedgerCalculation("Sales", AppConstants.SALES_LEDGER, Convert.ToInt32(item.GST), (item.StoreType)) : GetLedgerCalculation("Sales", AppConstants.SALES_OTHER_STATE, Convert.ToInt32(item.GST),(item.StoreType)),
                                        SalesLedger = !string.IsNullOrEmpty(item.FalcaGSTIN) ? (string.IsNullOrEmpty(item.GSTIN) || item.GSTIN.Substring(0, 2) == item.FalcaGSTIN.Substring(0, 2)) ? GetLedgerCalculation("Sales", AppConstants.SALES_LEDGER, Convert.ToInt32(item.GST), (item.StoreType)): GetLedgerCalculation("Sales", AppConstants.SALES_OTHER_STATE, Convert.ToInt32(item.GST), (item.StoreType)) : "",
                                        CustomerName = item.CustomerName,
                                        Bill = item.Bill,
                                        GSTIN = item.GSTIN,
                                        Address = item.Address,
                                        Pincode = Convert.ToString(item.Pincode),
                                        State = item.State,
                                        District = item.District,
                                        ProductName = item.SKU + "-" + item.ProductName,
                                        SKU = item.SKU,
                                        Godown = item.Godown,//(item.StoreName != null && item.StoreName != "") ? item.StoreName.Remove(0, 3) : null,
                                        Qty = item.Qty,
                                        Units = item.Units,
                                        HSN = item.HsnCode,
                                        Rate = item.Rate,
                                        SubTotal = item.SubTotal,
                                        TotalAmount = item.TotalAmount,
                                        StoreType = item.StoreType,
                                        GST = item.GST,
                                        //CGST =  item.StoreState.ToLower() == item.State.ToLower() ? item.CGST : 0,
                                        //SGST = item.StoreState.ToLower() == item.State.ToLower() ? item.SGST : 0,
                                        //IGST = item.StoreState.ToLower() == item.State.ToLower() ? 0 : (item.CGST + item.SGST),

                                        CGST =!string.IsNullOrEmpty(item.FalcaGSTIN)? (string.IsNullOrEmpty(item.GSTIN)|| item.GSTIN.Substring(0,2)==item.FalcaGSTIN.Substring(0,2))? item.CGST : 0:0,
                                        SGST = !string.IsNullOrEmpty(item.FalcaGSTIN) ? (string.IsNullOrEmpty(item.GSTIN) || item.GSTIN.Substring(0, 2) == item.FalcaGSTIN.Substring(0, 2)) ? item.SGST : 0 : 0,
                                        IGST = (!string.IsNullOrEmpty(item.FalcaGSTIN) && !string.IsNullOrEmpty(item.GSTIN))?(item.GSTIN.Substring(0, 2) != item.FalcaGSTIN.Substring(0, 2))? (item.CGST + item.SGST):0:0,
                                        Cash = lastInvoiceNo == item.InvoiceNo ? 0 : item.Cash,
                                        UPI = lastInvoiceNo == item.InvoiceNo ? 0 : item.UPI,
                                        Cheque = lastInvoiceNo == item.InvoiceNo ? 0 : item.Cheque,
                                        UTRNumber = lastInvoiceNo == item.InvoiceNo ? null : item.UTRNumber,
                                        //LedgerCGST = item.StoreState.ToLower() == item.State.ToLower() ? GetLedgerCalculation("Sales", AppConstants.LIABILITY_CGST, Convert.ToInt32(item.GST), (item.StoreType)) : "",
                                        //LedgerSGST = item.StoreState.ToLower() == item.State.ToLower() ? GetLedgerCalculation("Sales", AppConstants.LIABILITY_SGST, Convert.ToInt32(item.GST), (item.StoreType)) : "",
                                        //LedgerIGST = item.StoreState.ToLower() == item.State.ToLower() ? "" : GetLedgerCalculation("Sales", AppConstants.LIABILITY_IGST, Convert.ToInt32(item.GST), (item.StoreType)),
                                        LedgerCGST = !string.IsNullOrEmpty(item.FalcaGSTIN) ? (string.IsNullOrEmpty(item.GSTIN) || item.GSTIN.Substring(0, 2) == item.FalcaGSTIN.Substring(0, 2)) ? GetLedgerCalculation("Sales", AppConstants.LIABILITY_CGST, Convert.ToInt32(item.GST), (item.StoreType)) : "":"",
                                        LedgerSGST = !string.IsNullOrEmpty(item.FalcaGSTIN) ? (string.IsNullOrEmpty(item.GSTIN) || item.GSTIN.Substring(0, 2) == item.FalcaGSTIN.Substring(0, 2)) ? GetLedgerCalculation("Sales", AppConstants.LIABILITY_SGST, Convert.ToInt32(item.GST), (item.StoreType)) : "":"",
                                        LedgerIGST = (!string.IsNullOrEmpty(item.FalcaGSTIN) && !string.IsNullOrEmpty(item.GSTIN)) ? (item.GSTIN.Substring(0, 2) != item.FalcaGSTIN.Substring(0, 2))? GetLedgerCalculation("Sales", AppConstants.LIABILITY_IGST, Convert.ToInt32(item.GST), (item.StoreType)):"":"",

                                        ExistingSKU = item.ExistingSKU,
                                        StoreState =item.StoreState

                                    };
                                    TallyExportSalesModelReport serviceOrder = new TallyExportSalesModelReport();

                                    if (item.ServiceCharges != 0)
                                    {
                                        serviceOrder.Type = "Sales F-Shops-Service";
                                        serviceOrder.CostCenter = item.StoreName;
                                        serviceOrder.ReferEPR = item.InvoiceNo + "s";
                                        serviceOrder.DateEPR = item.DateasEPR.ToString("dd-MM-yyyy");
                                        serviceOrder.InvoiceNo = item.InvoiceNo + "s";
                                        serviceOrder.InvoiceDate = item.InvoiceDate.ToString("dd-MM-yyyy");
                                        serviceOrder.SalesLedger = GetLedgerCalculation("Services", AppConstants.SERVICES_LEDGER, Convert.ToInt32(18),(item.StoreType));
                                        serviceOrder.CustomerName = item.CustomerName;
                                        serviceOrder.Bill = item.Bill;
                                        serviceOrder.Address = item.Address;
                                        serviceOrder.Pincode = Convert.ToString(item.Pincode);
                                        serviceOrder.State = item.State;
                                        serviceOrder.District = item.District;
                                        serviceOrder.Qty = item.Qty;
                                        serviceOrder.SubTotal = Math.Round(Math.Round((decimal)item.ServiceCharges / (decimal)(1 + (18.0 / 100.0)), 2, MidpointRounding.AwayFromZero) * item.Qty, 2, MidpointRounding.AwayFromZero);
                                        serviceOrder.GST = 18;//fixed 18% services order
                                        serviceOrder.CGST = Math.Round((((serviceOrder.SubTotal / 100) * 18) / 2), 2);
                                        serviceOrder.SGST = Math.Round((((serviceOrder.SubTotal / 100) * 18) / 2), 2);
                                        serviceOrder.LedgerCGST = GetLedgerCalculation("Services", AppConstants.SERVICES_CGST, Convert.ToInt32(18), (item.StoreType));
                                        serviceOrder.LedgerSGST = GetLedgerCalculation("Services", AppConstants.SERVICES_SGST, Convert.ToInt32(18), (item.StoreType));
                                        serviceOrder.TotalAmount = (item.ServiceCharges * item.Qty);
                                        serviceOrder.StoreType = item.StoreType;
                                        serviceOrder.StoreState = item.StoreState;

                                    }
                                    lastInvoiceNo = item.InvoiceNo;
                                    salesOrderModels.Add(salesOrderModel);
                                    if (serviceOrder.InvoiceNo != null)
                                        salesOrderModels.Add(serviceOrder);


                                }
                                Application.Current?.Dispatcher?.Invoke(() =>
                                {
                                    ReportBaseModel = new ObservableCollection<ReportBaseModel>(salesOrderModels);
                                    ReportQueryableCollectionView = new QueryableCollectionView(ReportBaseModel);
                                });
                            }
                        }
                        break;

                    case "Tally Report (Purchase)":
                        DateAndBoolResult _validatePurchasedateRange = ValidateFromAndToDate();
                        if (_validatePurchasedateRange.IsValid)
                        {

                            var result = await _financeService.GetTallyExport(new Entites.Finance.TallyExportSearchModel()
                            {
                                FromInvoiceDate = FromTallyInvoiceDate,
                                ToInvoiceDate = ToTallyInvoiceDate,
                                Type = "Purchase"

                            });
                            if (result != null && result.Data.Count() > 0)
                            {
                                List<TallyExportPurchaseOrderModelReport> purchaseOrderModels = new List<TallyExportPurchaseOrderModelReport>();
                                var _lastInvoiceNo = "";
                                foreach (var item in result.Data)
                                {
                                    Match m = Regex.Match(Convert.ToString(item.Roundoff), @"[-]");
                                    var roundDiff = "";

                                    if (m.Success)
                                    {
                                        roundDiff = Regex.Replace(Convert.ToString(item.Roundoff), @"[-]", "");
                                    }

                                    TallyExportPurchaseOrderModelReport purchaseOrderModel = new TallyExportPurchaseOrderModelReport()
                                    {
                                        Type = item.type,
                                        //ReferEPR = item.NoasEPR,
                                        DateEPR = item.DateasEPR.ToString("dd-MM-yyyy"),
                                        InvoiceNo = item.InvoiceNo,
                                        InvoiceDate = item.InvoiceDate.ToString("dd-MM-yyyy"),
                                        SupplierName = item.SupplierName,
                                        GSTIN = item.GSTIN,
                                        PurchaseLedger = item.StoreState.ToLower() == item.State.ToLower() ? GetLedgerCalculation("Purchase", AppConstants.PURCHASE_LEDGER, Convert.ToInt32(item.GST), (item.StoreType)) : GetLedgerCalculation("Purchase", AppConstants.PURCHASE_OTHER_STATE, Convert.ToInt32(item.GST), (item.StoreType)),
                                        Pincode = item.Pincode,
                                        State = item.State,
                                        District = item.District,
                                        ProductName = item.SKU + "-" + item.ProductName,
                                        Godown = item.Godown,//(item.StoreName != null && item.StoreName != "") ? item.StoreName.Remove(0, 3) : null,
                                        SKU = item.SKU,
                                        Qty = item.SubQty,
                                        Rate = item.Rate,
                                        SubTotal = item.SubTotal,
                                        GST = item.GST,
                                        CGST = item.StoreState.ToLower() == item.State.ToLower() ? item.CGST : 0,//gst calucation for between two state
                                        SGST = item.StoreState.ToLower() == item.State.ToLower() ? item.SGST : 0,//gst calucation for between two state
                                        IGST = item.StoreState.ToLower() == item.State.ToLower() ? 0 : (item.CGST + item.SGST),  //gst calucation for between two state
                                        Address = item.Address,
                                        HSN = item.HsnCode,
                                        Bill = item.Bill,
                                        Units = item.Units,
                                        StoreName = item.StoreName,
                                        LedgerCGST = item.StoreState.ToLower() == item.State.ToLower() ? GetLedgerCalculation("Purchase", AppConstants.LEDGER_CGST, Convert.ToInt32(item.GST), (item.StoreType)) : "",//gst calucation for between two state
                                        LedgerSGST = item.StoreState.ToLower() == item.State.ToLower() ? GetLedgerCalculation("Purchase", AppConstants.LEDGER_SGST, Convert.ToInt32(item.GST), (item.StoreType)) : "",//gst calucation for between two state
                                        LedgerIGST = item.StoreState.ToLower() == item.State.ToLower() ? "" : GetLedgerCalculation("Purchase", AppConstants.LEDGER_IGST, Convert.ToInt32(item.GST), (item.StoreType)),//gst calucation for between two state
                                        Transportcharges = item.TransportCharges,
                                        Roundoff = _lastInvoiceNo == item.InvoiceNo ? 0 : item.Roundoff,//take round off one time only same invoice multiple product
                                        TotalAmount = _lastInvoiceNo == item.InvoiceNo ? Math.Round(item.TotalAmount, 0, MidpointRounding.AwayFromZero) : m.Success == true ? (Math.Round(item.TotalAmount, 0, MidpointRounding.AwayFromZero)) : (Math.Round(item.TotalAmount + item.Roundoff, 0, MidpointRounding.AwayFromZero)),
                                        BeforeGST = item.BeforeGST,
                                        AfterGST = item.AfterGST,
                                        Others = item.Others,
                                        ExistingSKU = item.ExistingSKU,
                                        DepartmentName = item.DepartmentName,
                                        ERPuniqueNo = item.ERPuniqueNo,
                                        CategoryName = item.CategoryName,
                                        StoreType = item.StoreType,
                                        TDS = item.TDS,
                                        StoreState=item.StoreState,
                                    };
                                    _lastInvoiceNo = item.InvoiceNo;
                                    purchaseOrderModels.Add(purchaseOrderModel);


                                }
                                Application.Current?.Dispatcher?.Invoke(() =>
                                {
                                    ReportBaseModel = new ObservableCollection<ReportBaseModel>(purchaseOrderModels);
                                    ReportQueryableCollectionView = new QueryableCollectionView(ReportBaseModel);
                                });
                            }
                        }
                        break;
                    case "Top List (Items)":
                       
                        DateAndBoolResult dateAndBoolResultItems = ValidateFromAndToDate();
                        if (dateAndBoolResultItems.IsValid)
                        {
                            var _itemResult = await _reportServices.GetTopListItems(dateAndBoolResultItems.FromDate, dateAndBoolResultItems.ToDate);
                            if (_itemResult != null && _itemResult.IsSuccess)
                            {
                                var _items = autoMapper.Map<List<TopListItemDTO>, List<TopListItemModel>>(_itemResult.Data.ToList());

                                Application.Current?.Dispatcher?.Invoke(() =>
                                {
                                    ReportBaseModel = new ObservableCollection<ReportBaseModel>(_items);
                                    ReportQueryableCollectionView = new QueryableCollectionView(ReportBaseModel);
                                });
                            }
                            else
                            {
                                _notificationService.ShowMessage("No record found", Common.NotificationType.Error);
                            }
                        }
                        break;
                    case "Top List (ProductGroup)":
                      
                        DateAndBoolResult dateAndBoolResultProductGroup = ValidateFromAndToDate();
                        if (dateAndBoolResultProductGroup.IsValid)
                        {
                            var _brandResult = await _reportServices.GetTopListBrand(dateAndBoolResultProductGroup.FromDate, dateAndBoolResultProductGroup.ToDate);
                            if (_brandResult != null && _brandResult.IsSuccess)
                            {

                                var _brand = autoMapper.Map<List<TopListBrandDTO>, List<TopListBrandModel>>(_brandResult.Data.ToList());

                                Application.Current?.Dispatcher?.Invoke(() => {
                                    ReportBaseModel = new ObservableCollection<ReportBaseModel>(_brand);
                                    ReportQueryableCollectionView = new QueryableCollectionView(ReportBaseModel);
                                });
                            }
                            else
                            {
                                _notificationService.ShowMessage("No record found", Common.NotificationType.Error);
                            }
                        }
                        
                        break;
                    case "Top List (Category)":
                       
                        DateAndBoolResult dateAndBoolResultCategory = ValidateFromAndToDate();
                        if (dateAndBoolResultCategory.IsValid)
                        {
                            var _categoryResult = await _reportServices.GetTopListCategories(dateAndBoolResultCategory.FromDate, dateAndBoolResultCategory.ToDate);
                            if (_categoryResult != null && _categoryResult.IsSuccess)
                            {

                                var TopListCategories = autoMapper.Map<List<TopListCategoryDTO>, List<TopListCategoryModel>>(_categoryResult.Data.ToList());

                                Application.Current?.Dispatcher?.Invoke(() =>
                                {
                                    ReportBaseModel = new ObservableCollection<ReportBaseModel>(TopListCategories);
                                    ReportQueryableCollectionView = new QueryableCollectionView(ReportBaseModel);
                                });
                            }
                            else
                            {
                                _notificationService.ShowMessage("No record found", Common.NotificationType.Error);
                            }
                        }
                        
                        break;
                    case "Top List (Transactions)":
                      
                        DateAndBoolResult dateAndBoolResultTransaction = ValidateFromAndToDate();
                        if (dateAndBoolResultTransaction.IsValid)
                        {
                            var _transactionResult = await _reportServices.GetTopListTransactions(dateAndBoolResultTransaction.FromDate, dateAndBoolResultTransaction.ToDate);
                            if (_transactionResult != null && _transactionResult.IsSuccess)
                            {

                                var topListTransactions = autoMapper.Map<List<TopListTransactionsDTO>, List<TopListTransactionsModel>>(_transactionResult.Data.ToList());

                                Application.Current?.Dispatcher?.Invoke(() =>
                                {
                                    ReportBaseModel = new ObservableCollection<ReportBaseModel>(topListTransactions);
                                    ReportQueryableCollectionView = new QueryableCollectionView(ReportBaseModel);
                                });
                            }
                            else
                            {
                                _notificationService.ShowMessage("No record found", Common.NotificationType.Error);
                            }
                        }
                         
                        break;
                    default:
                        break;
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            finally
            {
                await _progressService.StopProgressAsync();
            }
        }

        private void ReportNameChangeCommandEvent()
        {
            if (SelectedReport != null)
            {
                SelectedReportColumns = null;
                ReportQueryableCollectionView = null;
                ReportBaseModel = null;
                IsAllDataDownload = false;
                IsFilterDataDownload = false;
                IsFilterApplied = false;
                NoofColumns = 0;

                var _reportJson = _reportConfiguration.Where(x => x.name == SelectedReport && x.role.Contains(AppConstants.USER_ROLES[0])).ToList().FirstOrDefault();
                var _listofColums = _reportJson.columns;
                var _listofPrpopColumns = _reportJson.displaycolumns;
                FromTallyInvoiceDate = null;
                ToTallyInvoiceDate = null;
                if (_listofColums!= null)
                {
                    // SelectedReportColumns = new List<ColumnData>();
                    SelectedReportColumns = new ObservableCollection<ColumnData>(_listofColums.Select(x => new ColumnData() { Name = x, PropName = GetPropName(x, _listofPrpopColumns) ?? x }))  ;

                    SelectedReportColumns.Insert(0, new ColumnData { Selected = false, Name = "All" });

                    // SelectedReportColumns = SelectedReportColumns.OrderByDescending(x => x.Name == "All").ThenBy(x => x.Name).ToList();


                    //foreach (var  col in _listofColums)
                    //{
                    //    var c = col;
                    //    SelectedReportColumns.Add(new ColumnData { Name = c});
                    //}
                    // SelectedReportColumns = new List<ColumnData>(_listofColums.Select(x => new ColumnData() { Name = x }));

                }
                isTallyReport = (SelectedReport == "Tally Report (Sales)" || SelectedReport == "Tally Report (Purchase)" || SelectedReport == "Top List (Items)" || SelectedReport == "Top List (ProductGroup)" || SelectedReport == "Top List (Category)" || SelectedReport == "Top List (Transactions)" || SelectedReport == "PO GRN");
            }
        }

        /// <summary>
        /// This method is used to validate the from and to date selected by the user.
        /// </summary>
        /// <returns></returns>
        private DateAndBoolResult ValidateFromAndToDate()
        {
            if (string.IsNullOrEmpty(FromTallyInvoiceDate))
            {
                _notificationService.ShowMessage("Please select from  date", NotificationType.Error);
                return new DateAndBoolResult { FromDate = new DateTime(), ToDate = new DateTime(), IsValid = false };
            }

            if (string.IsNullOrEmpty(ToTallyInvoiceDate))
            {
                _notificationService.ShowMessage("Please select to  date", NotificationType.Error);
                return new DateAndBoolResult { FromDate = new DateTime(), ToDate = new DateTime(), IsValid = false };
            }
            DateTime _fromDateBrand = Convert.ToDateTime(FromTallyInvoiceDate);
            DateTime _toDateBrand = Convert.ToDateTime(ToTallyInvoiceDate);
            if (!string.IsNullOrEmpty(FromTallyInvoiceDate) && !string.IsNullOrEmpty(ToTallyInvoiceDate))
            {
                if (_toDateBrand < _fromDateBrand)
                {
                    _notificationService.ShowMessage("From Date should be less than or equal to To Date", NotificationType.Error);
                    return new DateAndBoolResult { FromDate = new DateTime(), ToDate = new DateTime(), IsValid = false };
                }
            }
            return new DateAndBoolResult { FromDate = _fromDateBrand, ToDate = _toDateBrand, IsValid = true };
        }

        private string GetPropName(string displayname, List<Displaycolumn> _listofPrpopColumns)
        {
            if (_listofPrpopColumns != null && _listofPrpopColumns.Count > 0)
            {
                return _listofPrpopColumns.Where(x => x.displayname == displayname).Select(x => x.propname).FirstOrDefault();
            }
            return displayname;
        }
        //Automapper inintilzed

        public Mapper AutoMapperInitialize()
        {
            return new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<InventoryReportModelPM, InventoryReport>();
                cfg.CreateMap<InventoryReportModelPM, InventoryReport2>();
                cfg.CreateMap<InventoryReportPoGrnModel, POGRNReport>();
                cfg.CreateMap<StockAgeSellingDataDuration, StockAgeReportSellingPrice>();
                cfg.CreateMap<StockAgeDataDuration, StockAgeReportInvoiceRate>();
                cfg.CreateMap<TopListItemDTO, TopListItemModel>();
                cfg.CreateMap<TopListBrandDTO, TopListBrandModel>();
                cfg.CreateMap<TopListCategoryDTO, TopListCategoryModel>();
                cfg.CreateMap<TopListTransactionsDTO, TopListTransactionsModel>();
            }));

        }





    }


    public class ReportConfiguration
    {
        public string name { get; set; }
        public List<String> role { get; set; }
        public List<string> columns { get; set; }
        public List<Displaycolumn> displaycolumns { get; set; }
    }

    public class Displaycolumn
    {
        public string displayname { get; set; }
        public string propname { get; set; }
    }

    public class ColumnData : BindableBase
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private bool _selected;

        public bool Selected
        {
            get { return _selected; }
            set { SetProperty(ref _selected, value); }
        }

        public String PropName { get; set; }

    }


    public class IgnorePropertiesResolver : DefaultContractResolver
    {
        private readonly HashSet<string> _ignoreProps;
        public IgnorePropertiesResolver(params string[] propNamesToIgnore)
        {
            _ignoreProps = new HashSet<string>(propNamesToIgnore);
        }
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            if (_ignoreProps.Contains(property.PropertyName))
            {
                property.ShouldSerialize = _ => false;
            }
            return property;
        }
    }

    /// <summary>
    /// This class holds the from date, to date and validation of from and to date.
    /// </summary>
    class DateAndBoolResult
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool IsValid { get; set; }
    }
}






