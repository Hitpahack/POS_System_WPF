using FalcaPOS.Common;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.AddInventory;
using FalcaPOS.Entites.Stores;
using FalcaPOS.Entites.Suppliers;
using FalcaPOS.Invoice.Views;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.Invoice.ViewModels
{
    public class CreditNoteSummaryViewModel : BindableBase
    {
        private CancellationTokenSource _cancellationTokenSource;


        private readonly Logger _logger;

        private readonly INotificationService _notificationService;

        private readonly ProgressService _ProgressService;

        private readonly ISupplierService _supplierService;

        private readonly IStoreService _storeService;

        public DelegateCommand SearchCommand { get; private set; }

        public DelegateCommand ResetCommand { get; private set; }

        private readonly IPurchaseInvoiceService _purchaseInvoiceService;

        public DelegateCommand ExportCommand { get; private set; }

        private readonly ICommonService _commonService;

        public DelegateCommand<object> ViewCommand { get; private set; }

        public DelegateCommand<object> ReedemViewCommand { get; private set; }
        public CreditNoteSummaryViewModel(ICommonService commonService, IPurchaseInvoiceService purchaseInvoiceService, IStoreService storeService, Logger logger, INotificationService notificationService, ISupplierService supplierService, ProgressService ProgressService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _ProgressService = ProgressService;

            _supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));

            _purchaseInvoiceService = purchaseInvoiceService ?? throw new ArgumentNullException(nameof(purchaseInvoiceService));

            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            ExportCommand = new DelegateCommand(ExportResultToExcel);

            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));


            LoadSuppliers();

            LoadStoresAsync();

            SearchCommand = new DelegateCommand(SearchData);

            ResetCommand = new DelegateCommand(ResetData);

            IsExportEnabled = false;

            ViewCommand = new DelegateCommand<object>(ViewSummaryPopup);

            ReedemViewCommand = new DelegateCommand<object>(ViewSummaryPopupReedem);


        }

        public async void SearchData()
        {
            try
            {
                if (SelectedStore == null)
                {
                    _notificationService.ShowMessage("Please select Store", Common.NotificationType.Error);
                    return;
                }

                if (SelectedSupplier == null)
                {
                    _notificationService.ShowMessage("Please select Supplier", Common.NotificationType.Error);
                    return;
                }

                if (!string.IsNullOrEmpty(FromDate) || !string.IsNullOrEmpty(ToDate))
                {
                    if (string.IsNullOrEmpty(FromDate))
                    {
                        _notificationService.ShowMessage("Please enter From Date", NotificationType.Error);
                        return;
                    }

                    if (string.IsNullOrEmpty(ToDate))
                    {
                        _notificationService.ShowMessage("Please enter To Date", NotificationType.Error);
                        return;
                    }


                }

                if (!String.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    DateTime dt1 = Convert.ToDateTime(FromDate);
                    DateTime dt2 = Convert.ToDateTime(ToDate);
                    if (dt2 < dt1)
                    {
                        _notificationService.ShowMessage("From Date should be less than or equal to To Date", NotificationType.Error);
                        return;
                    }
                }

                var _result = await _purchaseInvoiceService.GetCreditNoteSummaryList((int)SelectedSupplier.SupplierId, SelectedStore.StoreId, FromDate, ToDate);

                if (_result != null && _result.IsSuccess && _result.Data.Count() > 0)
                {
                    var summaryList = new ObservableCollection<SummaryViewModelCreidtNote>(_result.Data);

                    SummaryDetails = summaryList;

                    List<SummaryModel> summaryModels = new List<SummaryModel>();

                    if (summaryList != null && summaryList.Count > 0)
                    {
                        //details export credit note level
                        List<CreditNoteSummaryModelExport> returnModels = new List<CreditNoteSummaryModelExport>();

                        foreach (var item in SummaryDetails)
                        {
                            returnModels.Add(new CreditNoteSummaryModelExport()
                            {
                                CreditNoteNo = item.CreditNoteNumber,
                                CreditNoteDate = item.CreditNoteDate,
                                StoreName = item.StoreName,
                                SupplierName = item.SupplierName,
                                TotalAmount = item.Total,
                                AdjustedAmount = (float)item.RedeemedAmount,
                                BalanceAmount = (float)item.Total - (float)item.RedeemedAmount
                            });

                        }

                        returnModels.Add(new CreditNoteSummaryModelExport()
                        {
                            SupplierName = "Total",
                            TotalAmount = returnModels.Sum(x => x.TotalAmount),
                            AdjustedAmount = returnModels.Sum(x => x.AdjustedAmount),
                            BalanceAmount = returnModels.Sum(x => x.BalanceAmount),
                        });

                        DetailsPopupsExport = returnModels;

                        //overall supplier level export

                        var results = from p in summaryList
                                      group p by p.SupplierId into g
                                      select new { SupplierId = g.Key, SupplierList = g.ToList() };

                        foreach (var item in results)
                        {
                            summaryModels.Add(new SummaryModel()
                            {


                                SupplierName = item.SupplierList.Where(x => x.SupplierId == item.SupplierId).FirstOrDefault().SupplierName,
                                Total = item.SupplierList.Sum(x => x.Total),
                                AdujstedAmount = (float)item.SupplierList.Sum(x => x.RedeemedAmount),
                                Balance = (float)item.SupplierList.Sum(x => x.Total) - (float)item.SupplierList.Sum(x => x.RedeemedAmount)
                            });

                        }

                        summaryModels.Add(new SummaryModel()
                        {
                            SupplierName = "Total",
                            Total = summaryModels.Sum(x => x.Total),
                            AdujstedAmount = summaryModels.Sum(x => x.AdujstedAmount),
                            Balance = summaryModels.Sum(x => x.Balance),
                        });

                        SummaryList = new ObservableCollection<SummaryModel>(summaryModels);

                        ExportSummaryList = summaryModels;

                        IsExportEnabled = true;

                    }

                }
                else
                {
                    _notificationService.ShowMessage(_result.Error, Common.NotificationType.Error);
                    SummaryList = null;
                    return;
                }



            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
                await _ProgressService.StopProgressAsync();
            }
            finally
            {

            }
        }

        public void ResetData()
        {
            try
            {
                SelectedSupplier = null;
                SelectedStore = null;
                DetailsPopups = null;
                DetailsPopupsExport = null;
                SummaryDetails = null;
                SummaryList = null;
                ExportSummaryList = null;
                IsExportEnabled = false;
                FromDate = null;
                ToDate = null;


            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }


        private async void LoadStoresAsync()
        {
            try
            {
                await Task.Run(async () =>
                {

                    var _result = await _storeService.GetStores();

                    if (_result != null && _result.Count() > 0)
                    {
                        Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            Stores = new ObservableCollection<Store>(_result.Where(x => x.Parent_ref == null).ToList());

                            Stores.Insert(0, new Store { StoreId = -1, Name = "All" });
                            Stores.Insert(1, new Store() { StoreId = -2, Name = "All F-Shop" });
                            Stores.Insert(2, new Store() { StoreId = -3, Name = "All RSP" });
                        });
                    }

                });

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private async void LoadSuppliers()
        {

            try
            {
                _logger.LogInformation("Gettig suppliers ");

                await Task.Run(async () =>
                {
                    var _result = await _supplierService.GetSuppliers();

                    if (_result != null)
                    {
                        Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            Suppliers = new ObservableCollection<SuppliersViewModel>(_result);

                            Suppliers.Insert(0, new SuppliersViewModel { Isenabled = false, SupplierId = -1, Name = "All" });
                        });
                    }

                });

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in loding suplliers", _ex);
            }
        }

        private void ExportResultToExcel()
        {
            try
            {
                _fileName = null;


                if (ExportSummaryList == null || !ExportSummaryList.Any())
                {
                    _notificationService.ShowMessage("No Data Found", Common.NotificationType.Error);

                    return;
                }


                if (!_fileName.IsValidString())
                {
                    _fileName = "Summary";
                }
                _fileName += Guid.NewGuid().ToString().Substring(0, 3);

                IsExportEnabled = false;

                //bool _result = _commonService.ExportToXL(ExportSummaryList, _fileName, skipfileName: true, tablename: "CreditNotSummary", FilePath: ApplicationSettings.CREDITNOTE_PATH.ToString());

                bool _result = _commonService.ExportToXL(ExportSummaryList.ToList(), DetailsPopupsExport.ToList(), "CreditNotSummary", "SummaryDetails", FilePath: ApplicationSettings.CREDITNOTE_PATH.ToString());


                if (_result)
                {

                    _notificationService.ShowMessage("File exported to folder C:\\FALCAPOS\\CreditNoteSummary", Common.NotificationType.Success);

                }
                else
                {
                    _notificationService.ShowMessage("File export failed , try again", Common.NotificationType.Error);
                }

                IsExportEnabled = true;







            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }



        }




        public async void ViewSummaryPopup(object obj)
        {
            try
            {
                var viewmodel = ((SummaryModel)obj);

                if (viewmodel != null)
                {
                    CreditNoteSummarypopup creditNoteSummarypopup = new CreditNoteSummarypopup();

                    creditNoteSummarypopup.DataContext = this;

                    var supplierid = SummaryDetails.Where(x => x.SupplierName == viewmodel.SupplierName).FirstOrDefault();

                    var _result = await _purchaseInvoiceService.GetCreditNoteSummaryDetails(supplierid.SupplierId, SelectedStore.StoreId, FromDate, ToDate);

                    if (_result != null && _result.IsSuccess && _result.Data.Count() > 0)
                    {
                        var summaryList = new ObservableCollection<SummaryDetailsViewModelCreditNote>(_result.Data);

                        List<CreditNoteSummaryModel> returnModels = new List<CreditNoteSummaryModel>();

                        foreach (var item in summaryList)
                        {
                            returnModels.Add(new CreditNoteSummaryModel()
                            {
                                CreditNoteNo = item.CreditNoteNumber,
                                CreditNoteDate = item.CreditNoteDate,
                                StoreName = item.StoreName,
                                SupplierName = item.SupplierName,
                                TotalAmount = item.Total,
                                AdjustedAmount = (float)item.RedeemedAmount,
                                BalanceAmount = (float)item.Total - (float)item.RedeemedAmount,
                                AdjustModels = item.AdjustModels,
                                IsBtnEnable = item.RedeemedAmount == 0 ? false : true
                            });

                        }

                        returnModels.Add(new CreditNoteSummaryModel()
                        {
                            IsBtnEnable = false,
                            TotalAmount = returnModels.Sum(x => x.TotalAmount),
                            AdjustedAmount = returnModels.Sum(x => x.AdjustedAmount),
                            BalanceAmount = returnModels.Sum(x => x.BalanceAmount),
                            AdjustModels = null
                        });


                        DetailsPopups = new ObservableCollection<CreditNoteSummaryModel>(returnModels);
                        AdjustModels = null;

                        await DialogHost.Show(creditNoteSummarypopup, "RootDialog", ColsingEventHandler);
                    }
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }


        private void ColsingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {

                var _viewmodel = (CreditNoteSummaryViewModel)eventArgs.Parameter;
                if (_viewmodel != null)
                {
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

        public void ViewSummaryPopupReedem(object obj)
        {
            try
            {
                var viewmodel = ((CreditNoteSummaryModel)obj);

                if (viewmodel != null)
                {
                    if (viewmodel.AdjustModels.Count > 0 && viewmodel.AdjustModels != null)
                    {
                        AdjustModels = new ObservableCollection<AdjustModel>(viewmodel.AdjustModels);
                    }
                    else
                    {
                        AdjustModels = null;
                    }

                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }



        private ObservableCollection<Store> _stores;
        public ObservableCollection<Store> Stores
        {
            get { return _stores; }
            set { SetProperty(ref _stores, value); }
        }

        private Store _selectedStore;

        public Store SelectedStore
        {
            get { return _selectedStore; }
            set
            {
                SetProperty(ref _selectedStore, value);
            }
        }

        private ObservableCollection<SuppliersViewModel> _suppliers;

        public ObservableCollection<SuppliersViewModel> Suppliers
        {
            get => _suppliers;
            set => SetProperty(ref _suppliers, value);
        }


        private SuppliersViewModel _selectedSupplier;

        public SuppliersViewModel SelectedSupplier
        {
            get => _selectedSupplier;
            set => SetProperty(ref _selectedSupplier, value);

        }



        private ObservableCollection<SummaryModel> _summaryList;

        public ObservableCollection<SummaryModel> SummaryList
        {
            get { return _summaryList; }
            set { SetProperty(ref _summaryList, value); }
        }


        private List<SummaryModel> _exportsummaryList;

        public List<SummaryModel> ExportSummaryList
        {
            get { return _exportsummaryList; }
            set { SetProperty(ref _exportsummaryList, value); }
        }

        public string _fileName { get; set; }

        public String _exportFolder;
        public String ExportFolder
        {
            get { return _exportFolder; }

            set { SetProperty(ref _exportFolder, value); }
        }

        private bool _isExportEnabled;
        public bool IsExportEnabled
        {
            get => _isExportEnabled;
            set => _ = SetProperty(ref _isExportEnabled, value);
        }

        private ObservableCollection<SummaryViewModelCreidtNote> _summarydetails;

        public ObservableCollection<SummaryViewModelCreidtNote> SummaryDetails
        {
            get { return _summarydetails; }
            set { SetProperty(ref _summarydetails, value); }
        }

        private ObservableCollection<CreditNoteSummaryModel> _detailsPopups;

        public ObservableCollection<CreditNoteSummaryModel> DetailsPopups
        {
            get { return _detailsPopups; }
            set { SetProperty(ref _detailsPopups, value); }
        }

        private List<CreditNoteSummaryModelExport> _detailsPopupsExport;

        public List<CreditNoteSummaryModelExport> DetailsPopupsExport
        {
            get { return _detailsPopupsExport; }
            set { SetProperty(ref _detailsPopupsExport, value); }
        }


        private ObservableCollection<AdjustModel> _adjustmodels;
        public ObservableCollection<AdjustModel> AdjustModels
        {
            get { return _adjustmodels; }
            set { SetProperty(ref _adjustmodels, value); }
        }

        private string _fromDate;

        public string FromDate
        {
            get { return _fromDate; }
            set { SetProperty(ref _fromDate, value); }
        }

        private string _toDate;

        public string ToDate
        {
            get { return _toDate; }
            set { SetProperty(ref _toDate, value); }
        }

    }

    public class SummaryModel
    {

        public string SupplierName { get; set; }


        public float Total { get; set; }

        public float AdujstedAmount { get; set; }

        public float Balance { get; set; }
    }

    public class CreditNoteSummaryModelExport
    {
        public string SupplierName { get; set; }

        public string StoreName { get; set; }

        public string CreditNoteNo { get; set; }

        public string CreditNoteDate { get; set; }

        public float TotalAmount { get; set; }

        public float AdjustedAmount { get; set; }

        public float BalanceAmount { get; set; }


    }

}
