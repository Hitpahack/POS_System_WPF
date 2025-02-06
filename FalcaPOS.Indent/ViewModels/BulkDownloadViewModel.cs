using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.AddInventory;
using FalcaPOS.Entites.Indent;
using FalcaPOS.Entites.Stores;
using FalcaPOS.Entites.Suppliers;
using FalcaPOS.Indent.Views;
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
using CheckBox = System.Windows.Controls.CheckBox;

namespace FalcaPOS.Indent.ViewModels
{
    public class BulkDownloadViewModel : BindableBase
    {


        private readonly INotificationService _notificationService;

        private readonly ProgressService _ProgressService;

        private readonly Logger _logger;
        public DelegateCommand<object> SelectBankCommand { get; private set; }

        public DelegateCommand<object> UnSelectBankCommand { get; private set; }

        public DelegateCommand<object> SelectedSupplierCommand { get; private set; }

        public DelegateCommand ExportCommand { get; private set; }

        private readonly ICommonService _commonService;

        private readonly ISupplierService _supplierService;

        private readonly IIndentService _indentService;

        public DelegateCommand SearchCommand { get; private set; }

        public DelegateCommand ResetCommand { get; private set; }

        public DelegateCommand<object> AdjustCommand { get; private set; }

        public DelegateCommand<object> SelectedCreditCommand { get; private set; }

        public DelegateCommand<object> UnSelectedCreditCommand { get; private set; }
        public DelegateCommand<object> CloseSelectedCreditCommand { get; private set; }

        public DelegateCommand<object> RedeemedAmountChangeCommand { get; private set; }

        public DelegateCommand<object> PaymentUpdateCommand { get; private set; }


        public DelegateCommand<object> SelectedIndentCommand { get; private set; }


        private readonly IStoreService _storeService;


        public DelegateCommand<object> ConfirmCommand { get; private set; }


        public DelegateCommand<object> OnCheckedCommand { get; private set; }

        public DelegateCommand<object> OnUnCheckedCommand { get; private set; }

        public DelegateCommand SupplierRefreshCommand { get; private set; }


        public DelegateCommand<object> DownloadExcelCommand { get; private set; }


        private Dictionary<int, SupplierTDSStatus> supplierTDSStatuses = new Dictionary<int, SupplierTDSStatus>();

        public BulkDownloadViewModel(IEventAggregator eventAggregator, IStoreService storeService, IIndentService indentService, ICommonService commonService, ISupplierService supplierService, INotificationService notificationService, ProgressService ProgressService, Logger logger)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _ProgressService = ProgressService ?? throw new ArgumentNullException(nameof(ProgressService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));

            _supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));

            _indentService = indentService ?? throw new ArgumentNullException();

            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            SelectBankCommand = new DelegateCommand<object>(SelectedBankDetails);

            UnSelectBankCommand = new DelegateCommand<object>(UnSelectedBankDetails);

            SelectedSupplierCommand = new DelegateCommand<object>(SelectAllIndent);

            ExportCommand = new DelegateCommand(ExportResultToExcel);

            SearchCommand = new DelegateCommand(SearchData);

            ResetCommand = new DelegateCommand(ResetData);


            LoadSuppliers();

            IsExportEnabled = false;

            BulkPaymentModels = new ObservableCollection<BulkPaymentDownloadModel>();

            AdjustCommand = new DelegateCommand<object>(Adjusted);

            SelectedCreditCommand = new DelegateCommand<object>(SelectedCreditNoteDetails);

            UnSelectedCreditCommand = new DelegateCommand<object>(UnSelectedCreditNoteDetails);

            CloseSelectedCreditCommand = new DelegateCommand<object>(CloseSelectedCreditNoteDetails);

            RedeemedAmountChangeCommand = new DelegateCommand<object>(RedeemedAmountChange);

            PaymentUpdateCommand = new DelegateCommand<object>(PaymentUpdate);

            SelectedIndentCommand = new DelegateCommand<object>(SelectIndividualIndent);

            LoadStoresAsync();

            ConfirmCommand = new DelegateCommand<object>(ConfirmPopup);

            OnCheckedCommand = new DelegateCommand<object>(checkboxCommand);

            SelectedSupplier = new PendingPaymentSupplier();


            OnUnCheckedCommand = new DelegateCommand<object>(UncheckboxCommand);

            _ = eventAggregator.GetEvent<BulkPaymentUpdateChangeEvent>().Subscribe(SearchData);

            SupplierRefreshCommand = new DelegateCommand(RefreshSupplier);

            DownloadExcelCommand = new DelegateCommand<object>(DownloadExcel);

            // This is used to subscribe to the Search data method when this event is published.
            eventAggregator.GetEvent<IndentStatusChangeFromSahayaInFinance>().Subscribe(SearchData, ThreadOption.PublisherThread);
        }

        public void ConfirmPopup(object obj)
        {
            try
            {
                var TargetClose = ((Button)(obj));
                var dynamicCommand = TargetClose.Command;
                dynamicCommand.CanExecute(true);
                dynamicCommand.Execute(this);
            }
            //pass Model as Arguments
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }
        public async void Adjusted(object pay)
        {
            try
            {
                var viewModel = (IndentDownloadModel)(pay);

                var _bankListResult = await _indentService.GetBankDetailsList(viewModel.SupplierId, viewModel.Id);

                if (_bankListResult != null && _bankListResult.IsSuccess)
                {
                    SuppliersViewModel supplier = new SuppliersViewModel()
                    {
                        SupplierId = viewModel.SupplierId,
                        Name = viewModel.SupplierName,

                    };
                    PoId = viewModel.Id;
                    AdjustedPaymentPopup payment = new AdjustedPaymentPopup();
                    payment.DataContext = this;
                    AddSupplierToIndent.SelectedSupplier = supplier;
                    AddSupplierToIndent.SelectedSupplierName = viewModel.SupplierName;
                    AddSupplierToIndent.BankDetails = _bankListResult.Data.ListOfBankDetails;
                    AddSupplierToIndent.ReturnModels = _bankListResult.Data.ReturnModels;
                    AddSupplierToIndent.SelectedReturnModels = null;
                    var payAmount = (viewModel.PayableAmount);
                    AddSupplierToIndent.PaymentDate = viewModel.PaymentDate;
                    AddSupplierToIndent.PayableAmount = payAmount;
                    AddSupplierToIndent.FinalPayableAmount = payAmount;
                    await DialogHost.Show(payment, "RootDialog", AddPaymentClosingEventHandler);
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }
        public async void SearchData()
        {
            try
            {
                DateTime? dt1 = null;
                DateTime? dt2 = null;
                if (!string.IsNullOrEmpty(FromDate))
                {
                    dt1 = DateTime.Parse(FromDate);
                }
                if (!string.IsNullOrEmpty(ToDate))
                {
                    dt2 = DateTime.Parse(ToDate);
                }
               
                if (SelectedStore == null)
                {
                    _notificationService.ShowMessage("please select store", Common.NotificationType.Error);
                    return;
                }

                if (!Suppliers.Where(x => x.IsEnable).Any())
                {
                    _notificationService.ShowMessage("please select supplier", Common.NotificationType.Error);
                    return;
                }
                if (string.IsNullOrEmpty(FromDate))
                {
                    _notificationService.ShowMessage("please enter from date", Common.NotificationType.Error);
                    return;
                }
                if (string.IsNullOrEmpty(ToDate))
                {
                    _notificationService.ShowMessage("please enter to date", Common.NotificationType.Error);
                    return;
                }

                if (dt1 != null && dt2 != null)
                {
                    if (dt2 < dt1)
                    {
                        _notificationService.ShowMessage("From date should be less than or equal to To date", NotificationType.Error);
                        return;
                    }
                }

                if (BulkPaymentModels != null && BulkPaymentModels.Count > 0)
                {
                    BulkPaymentModels.Clear();
                }

                //var _isAlreadyAdded = BulkPaymentModels.Any(x => x.SupplierId == SelectedSupplier.SupplierId);
                //if (_isAlreadyAdded)
                //{
                //    _notificationService.ShowMessage("Already added this supplier please check", Common.NotificationType.Error);
                //    return;
                //}

                await _ProgressService.StartProgressAsync();

                var _result = await _indentService.GetBulkPaymentList(Suppliers.Where(x => x.IsEnable && x.SupplierId != 0).Select(x => x.SupplierId).ToList(), SelectedStore.StoreId,FromDate,ToDate);

                if (_result != null && _result.IsSuccess)
                {
                    //if (Suppliers.Where(x=>x.Isenabled && x.SupplierId==0).FirstOrDefault()!=null)
                    //{
                    //    if (BulkPaymentModels != null && BulkPaymentModels.Count > 0)
                    //    {
                    //        BulkPaymentModels.Clear();
                    //    }
                    //}

                    //if (SelectedStore.StoreId == 0)
                    //{
                    //    if (BulkPaymentModels != null && BulkPaymentModels.Count > 0)
                    //    {
                    //        BulkPaymentModels.Clear();
                    //    }
                    //}

                    if (BulkPaymentModels != null && BulkPaymentModels.Count > 0)
                    {
                        BulkPaymentModels.Clear();
                    }

                    foreach (var item in _result.Data)
                    {
                        BulkPaymentDownloadModel bulkPaymentDownloadModel = new BulkPaymentDownloadModel();
                        bulkPaymentDownloadModel.SupplierId = item.SupplierId;
                        bulkPaymentDownloadModel.SupplierName = item.SupplierName;
                        bulkPaymentDownloadModel.BankDetails = item.BankDetails != null ? item.BankDetails.Count > 1 ? item.BankDetails : item.BankDetails.Select(x => { x.IsSelected = true; return x; }).ToList() : item.BankDetails;
                        bulkPaymentDownloadModel.SelectedBankDetail = bulkPaymentDownloadModel.BankDetails.Where(x => x.IsSelected).FirstOrDefault() != null ? bulkPaymentDownloadModel.BankDetails.Where(x => x.IsSelected).FirstOrDefault() : null;
                        bulkPaymentDownloadModel.CNAmount = item.CNAmount;
                        bulkPaymentDownloadModel.ReturnModels = item.ReturnModels;
                        bulkPaymentDownloadModel.SupplierPhone = item.SupplierPhone;
                        bulkPaymentDownloadModel.IsSelectedBank = bulkPaymentDownloadModel.SelectedBankDetail != null ? bulkPaymentDownloadModel.SelectedBankDetail.AccountNo != null ? true : false : false;
                        List<IndentDownloadModel> indentViewModels = new List<IndentDownloadModel>();
                        foreach (var ite in item.IndentLists)
                        {
                            var payAmount = ((float)Math.Round(EstimateCalculation(ite.Products), 0, MidpointRounding.AwayFromZero) + ite.AdvisoryCharges);
                            var remainAmountToPay = ite.PayableAmount - (ite.SelectedReturnModels.Sum(x => x.Total));

                            if ((payAmount) > 0)
                            {
                                indentViewModels.Add(new IndentDownloadModel()
                                {
                                    PoNumber = ite.PoNumber,
                                    Date = ite.Date,
                                    StoreName = ite.StoreName,
                                    SupplierId = ite.SupplierId,
                                    SupplierName = ite.SupplierName,
                                    Id = ite.Id,
                                    CNAmount = ite.CNAmount,
                                    TDS = ite.TDS,
                                    TotalEstimantedAmount = payAmount,
                                    PayableAmount = remainAmountToPay,
                                    PaymentDate=ite.PaymentDate,
                                    FinalPayableAmount = payAmount,
                                    //reusing this props
                                    CreatedBy = (payAmount == ite.PayableAmount) ? "Full" : "Partial",
                                    //reusing this totalamount as reedmeedtotal aginst this indent
                                    TotalAmount = (ite.SelectedReturnModels.Sum(x => x.Total)),
                                    InvoiceNo = ite.InvoiceNo,
                                    PoQty= ite.Products.Sum(x=>x.AvailableQty),
                                    invoiceDetails=ite.invoiceDetails,
                                    Status = ite.Status

                                });

                            }


                        }
                        bulkPaymentDownloadModel.IndentLists = indentViewModels;


                        BulkPaymentModels.Add(bulkPaymentDownloadModel);
                    }

                }
                else
                {
                    _notificationService.ShowMessage(_result.Error, Common.NotificationType.Error);


                }

                await _ProgressService.StopProgressAsync();

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
                BulkPaymentModels.Clear();
                SelectedSupplier = null;
                SelectedStore = null;
                IsExportEnabled = false;
                SelectedBank = null;
                FromDate = null;
                ToDate = null;
                if (Suppliers != null && Suppliers.Count > 0)
                {
                    foreach (var item in Suppliers)
                    {
                        item.IsEnable = false;
                    }
                }
                supplierTDSStatuses.Clear();


            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public float EstimateCalculation(object obj)
        {
            try
            {
                float totalAmount = 0;
                var view = (List<IndentViewProduct>)obj;
                foreach (var item in view)
                {
                    if (item.SelectedGSTslab != null && item.SelectedGSTslab?.GstType=="Exclusive")
                    {
                        totalAmount += ((((item.AvailableQty * item.EstimatedPrice) / 100) * item.SelectedGSTslab.GstValue) + item.AvailableQty * item.EstimatedPrice);

                    }
                    else
                    {
                        totalAmount += (item.AvailableQty * item.EstimatedPrice);

                    }
                }

                return totalAmount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return 0;
            }

        }

        public void SelectAllIndent(object obj)
        {
            try
            {

                var _model = (BulkPaymentDownloadModel)obj;

                if (_model != null && BulkPaymentModels!=null && BulkPaymentModels.Count>0)
                {
                    var bankList = BulkPaymentModels.Where(x => x.SupplierId == _model.SupplierId).FirstOrDefault().BankDetails;

                    if (bankList == null || bankList.Count == 0)
                    {
                        _notificationService.ShowMessage("Please add a bank", Common.NotificationType.Error);

                        BulkPaymentModels.Where(x => x.SupplierId == _model.SupplierId).FirstOrDefault().IsSelected = false;

                        return;
                    }

                    BulkPaymentModels.Where(x => x.SupplierId == _model.SupplierId).FirstOrDefault()?.IndentLists.ForEach(x => x.IsSelected = _model.IsSelected);
                    var _selectedIndent = BulkPaymentModels.Where(x => x.IsSelected == true).FirstOrDefault()?.IndentLists.Where(x => x.IsSelected == true).ToList();
                    IsExportEnabled = (_selectedIndent != null && _selectedIndent.Count > 0) == true ? true : false;
                    //make TDS call here 
                    try
                    {
                        GetSupplierTDS(_model.SupplierId);

                    }
                    catch (Exception _ex)
                    {

                        _logger.LogError(_ex.Message);
                    }
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }



        public async void GetSupplierTDS(int supplierId)
        {
            try
            {
                if (!supplierTDSStatuses.ContainsKey(supplierId))
                {
                    var _result = await _supplierService.GetSupplierTDSDetails(supplierId);

                    if (_result != null && _result.IsSuccess)
                    {
                        supplierTDSStatuses.Add(supplierId, _result.Data);
                    }
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }


        private ObservableCollection<BulkPaymentDownloadModel> _bulkPaymentModels;
        public ObservableCollection<BulkPaymentDownloadModel> BulkPaymentModels
        {
            get => _bulkPaymentModels;

            set => SetProperty(ref _bulkPaymentModels, value);
        }

        private ObservableCollection<PendingPaymentSupplier> _suppliers;

        public ObservableCollection<PendingPaymentSupplier> Suppliers
        {
            get => _suppliers;
            set => SetProperty(ref _suppliers, value);
        }


        private PendingPaymentSupplier _selectedSupplier;

        public PendingPaymentSupplier SelectedSupplier
        {
            get => _selectedSupplier;
            set => SetProperty(ref _selectedSupplier, value);

        }



        public void SelectedBankDetails(object obj)
        {
            try
            {
                var _bankDetail = (BankDetails)obj;
                if (_bankDetail != null)
                {

                    BulkPaymentModels.Where(x => x.SupplierId == _bankDetail.SupplierId).FirstOrDefault().BankDetails.Where(x => x.BankDetailsId != _bankDetail.BankDetailsId).ToList().ForEach(x => { x.IsSelected = false; });
                    BulkPaymentModels.Where(x => x.SupplierId == _bankDetail.SupplierId).FirstOrDefault().SelectedBankDetail = _bankDetail;
                    BulkPaymentModels.Where(x => x.SupplierId == _bankDetail.SupplierId).FirstOrDefault().IsSelectedBank = true;
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }
        public void UnSelectedBankDetails(object obj)
        {
            try
            {
                var _bankDetail = (BankDetails)obj;
                if (_bankDetail != null)
                {
                    BulkPaymentModels.Where(x => x.SupplierId == _bankDetail.SupplierId).FirstOrDefault().SelectedBankDetail = null;
                    BulkPaymentModels.Where(x => x.SupplierId == _bankDetail.SupplierId).FirstOrDefault().IsSelectedBank = false;

                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
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
        private async void ExportResultToExcel()
        {
            try
            {
                if (SelectedBank == null)
                {
                    _notificationService.ShowMessage("Please select bank", NotificationType.Error);
                    return;
                }

                if (SelectedBank == "SBI")
                {
                    _fileName = null;
                    List<CNAmountPopup> AmountPopups = new List<CNAmountPopup>();
                    if (BulkPaymentModels != null && BulkPaymentModels.Count > 0)
                    {

                        SBIBulkExport.Clear();

                        foreach (var item in BulkPaymentModels)
                        {
                            var _selectedIntent = item.IndentLists != null ? item.IndentLists.Where(x => x.IsSelected == true).ToList() : null;


                            if (_selectedIntent != null && _selectedIntent.Count > 0)
                            {

                                foreach (var it in _selectedIntent)
                                {
                                    // Checks if the payment date of the indent is greater than the current date from the server
                                    if (Convert.ToDateTime(it.PaymentDate) > Convert.ToDateTime(AppConstants.CurrentDate))
                                    {
                                        _notificationService.ShowMessage("One or more selected indents contain future payable dates. Please unselect them.", Common.NotificationType.Error);
                                        return;
                                    }

                                    // Checks if the status of any selected indent is sahaya pending
                                    if (it.Status == "sahayapending")
                                    {
                                        _notificationService.ShowMessage("One or more selected indents is in sahaya pending. Please unselect them or reload the search.", Common.NotificationType.Error);
                                        return;
                                    }

                                    if (it.SupplierId == item.SupplierId)
                                    {
                                        if (item.SelectedBankDetail == null || string.IsNullOrEmpty(item.SelectedBankDetail?.AccountNo))
                                        {
                                            _notificationService.ShowMessage("Please select supplier Bank", Common.NotificationType.Error);

                                            return;
                                        }

                                        SBIBulkExport.Add(new BulkDownloadExport()
                                        {

                                            CustomerCode = "12345",
                                            CustomerName = "FALCA E SOLUTION PRIVATE LIMITED",
                                            DebitAccountNo = ApplicationSettings.FALCA_DEBIT_ACCOUT.ToString(),
                                            GrossTotal=it.TDS+it.PayableAmount,
                                            TDS=it.TDS,
                                            Amount = it.PayableAmount,
                                            ProductCode = "NEFT",
                                            BeneficiaryName = item.SupplierName,
                                            BeneficiaryBranchCode = item.SelectedBankDetail.IFSC,
                                            CreditAccountNo = item.SelectedBankDetail.AccountNo,
                                            EmailId = "finance@falcasolutions.com",
                                            StoreName = it.StoreName,
                                            PaymentRefNo = it.PoNumber,
                                            Phone = item.SupplierPhone,
                                            InvoiceNo = it.InvoiceNo,

                                        });
                                    }

                                }
                                List<StoreReturnModel> StoreReturnModel = new List<StoreReturnModel>();

                                if (item.CNAmount > 0)
                                {
                                    AmountPopups.Add(new CNAmountPopup()
                                    {

                                        Suppplier = item.SupplierName,
                                        SelectedReturnModels = item.ReturnModels
                                    });
                                }


                            }

                        }
                    }



                    if (SBIBulkExport == null || !SBIBulkExport.Any())
                    {
                        _notificationService.ShowMessage("Please select the indent to export", Common.NotificationType.Error);

                        return;
                    }

                    if (AmountPopups != null && AmountPopups.Count > 0)
                    {

                        BulkDownloadCNAmountPopup bulkDownloadCNAmount = new BulkDownloadCNAmountPopup();
                        bulkDownloadCNAmount.DataContext = this;
                        CNAmountPopups = new ObservableCollection<CNAmountPopup>(AmountPopups);
                        await DialogHost.Show(bulkDownloadCNAmount, "RootDialog", EventHandler);

                    }
                    else
                    {
                        if (!_fileName.IsValidString())
                        {
                            _fileName = "SBI Bulk";
                        }
                        _fileName += Guid.NewGuid().ToString().Substring(0, 3);

                        IsExportEnabled = false;

                        bool _result = _commonService.ExportToXL(SBIBulkExport, _fileName, skipfileName: true, tablename: "Falca POS SBI bulk Payment", FilePath: ApplicationSettings.BULKPAYMENT_PATH.ToString());

                        if (_result)
                        {

                            _notificationService.ShowMessage("File exported to folder C:\\FALCAPOS\\BulkPayment\\"+_fileName, Common.NotificationType.Success);

                        }
                        else
                        {
                            _notificationService.ShowMessage("File export failed , try again", Common.NotificationType.Error);
                        }

                        IsExportEnabled = true;
                    }
                }
                else if (SelectedBank == "ICICI")
                {
                    _fileName = null;
                    List<CNAmountPopup> AmountPopups = new List<CNAmountPopup>();
                    if (BulkPaymentModels != null && BulkPaymentModels.Count > 0)
                    {

                        ICICBulkExport.Clear();

                        foreach (var item in BulkPaymentModels)
                        {
                            var _selectedIntent = item.IndentLists != null ? item.IndentLists.Where(x => x.IsSelected == true).ToList() : null;


                            if (_selectedIntent != null && _selectedIntent.Count > 0)
                            {

                                foreach (var it in _selectedIntent)
                                {
                                    // Checks if the payment date of the indent is greater than the current date from the server
                                    if (Convert.ToDateTime(it.PaymentDate) > Convert.ToDateTime(AppConstants.CurrentDate))
                                    {
                                        _notificationService.ShowMessage("One or more selected indents contain future payable dates. Please unselect them.", Common.NotificationType.Error);
                                        return;
                                    }

                                    // Checks if the status of any selected indent is sahaya pending
                                    if (it.Status == "sahayapending")
                                    {
                                        _notificationService.ShowMessage("One or more selected indents is in sahaya pending. Please unselect them or reload the search.", Common.NotificationType.Error);
                                        return;
                                    }

                                    if (it.SupplierId == item.SupplierId)
                                        {
                                        if (item.SelectedBankDetail == null || string.IsNullOrEmpty(item.SelectedBankDetail?.AccountNo))
                                        {
                                            _notificationService.ShowMessage("Please select supplier Bank", Common.NotificationType.Error);

                                            return;
                                        }

                                        ICICBulkExport.Add(new ICICExport()
                                        {


                                            DebitAccNo = ApplicationSettings.ICIC_FALCA_DEBIT_ACNO.ToString(),
                                            BeneficiaryAccNo = item.SelectedBankDetail.AccountNo,
                                            BeneMobileNo = item.SupplierPhone,
                                            IFSC = item.SelectedBankDetail.IFSC,
                                            BeneficiaryName = item.SupplierName,
                                            GrossTotal=it.TDS+it.PayableAmount,
                                            TDS=it.TDS,
                                            Amount = it.PayableAmount,
                                            PaymentMode = "N",
                                            BeneEmailId = "finance@falcasolutions.com",
                                            Remarks = it.PoNumber,
                                            InvoiceNo=it.InvoiceNo,

                                        });
                                    }

                                }
                                List<StoreReturnModel> StoreReturnModel = new List<StoreReturnModel>();

                                if (item.CNAmount > 0)
                                {
                                    AmountPopups.Add(new CNAmountPopup()
                                    {

                                        Suppplier = item.SupplierName,
                                        SelectedReturnModels = item.ReturnModels
                                    });
                                }


                            }

                        }
                    }



                    if (ICICBulkExport == null || !ICICBulkExport.Any())
                    {
                        _notificationService.ShowMessage("Please select the indent to export", Common.NotificationType.Error);

                        return;
                    }

                    if (AmountPopups != null && AmountPopups.Count > 0)
                    {

                        BulkDownloadCNAmountPopup bulkDownloadCNAmount = new BulkDownloadCNAmountPopup();
                        bulkDownloadCNAmount.DataContext = this;
                        CNAmountPopups = new ObservableCollection<CNAmountPopup>(AmountPopups);
                        await DialogHost.Show(bulkDownloadCNAmount, "RootDialog", EventHandler);

                    }
                    else
                    {
                        if (!_fileName.IsValidString())
                        {
                            _fileName = "ICICI Bulk";
                        }
                        _fileName += Guid.NewGuid().ToString().Substring(0, 3);

                        IsExportEnabled = false;

                        bool _result = _commonService.ExportToXL(ICICBulkExport, _fileName, skipfileName: true, tablename: "Falca POS ICICI bulk Payment", FilePath: ApplicationSettings.BULKPAYMENT_PATH.ToString());

                        if (_result)
                        {

                            _notificationService.ShowMessage("File exported to folder C:\\FALCAPOS\\BulkPayment\\"+_fileName, Common.NotificationType.Success);

                        }
                        else
                        {
                            _notificationService.ShowMessage("File export failed , try again", Common.NotificationType.Error);
                        }

                        IsExportEnabled = true;
                    }
                }


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
                _logger.LogInformation("Getting suppliers ");

                await Task.Run(async () =>
                {
                    var _result = await _indentService.GetPendingPaymentSupplier();

                    if (_result != null && _result.IsSuccess)
                    {
                        Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            Suppliers = new ObservableCollection<PendingPaymentSupplier>(_result.Data.OrderBy(x => x.SupplierName));

                            Suppliers.Insert(0, new PendingPaymentSupplier { IsEnable = false, SupplierId = 0, SupplierName = "All" });
                        });
                    }

                });

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in loading suppliers", _ex);
            }
        }

        private async void AddPaymentClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {

                var _viewModel = (BulkDownloadViewModel)eventArgs.Parameter;
                if (_viewModel != null)
                {
                    await _ProgressService.StartProgressAsync();

                    await Task.Run(async () =>
                    {

                        AddSupplierToIndent.SelectedReturnModels?.Select(x => { x.RedeemedAmount = (x.RedeemedAmount == 0 ? (decimal)x.Total : x.RedeemedAmount); return x; }).ToList();
                        List<SelectedRetunes> selectedRetunes = new List<SelectedRetunes>();
                        foreach (var item in AddSupplierToIndent.SelectedReturnModels)
                        {
                            selectedRetunes.Add(new SelectedRetunes()
                            {
                                Id = item.Id,
                                CreditNoteDate = item.CreditNoteDate,
                                CreditNoteNumber = item.CreditNoteNumber,
                                RedeemedAmount = item.RedeemedAmount
                            }
                         );
                        }
                        var _result = await _indentService.CreditNoteAdjustment(PoId, new UpdateAdjustmentModel() { SupplierId = AddSupplierToIndent.SupplierId, PayableAmount = AddSupplierToIndent.FinalPayableAmount, SelectedRetunes = selectedRetunes });
                        Application.Current?.Dispatcher?.Invoke(async () =>
                        {

                            if (_result != null && _result.IsSuccess)
                            {
                                _notificationService.ShowMessage(_result?.Data ?? AppConstants.CommonError, Common.NotificationType.Success);
                                ResetData();

                            }
                            else
                            {
                                _notificationService.ShowMessage(_result?.Error ?? AppConstants.CommonError, Common.NotificationType.Error);
                            }

                            await _ProgressService.StopProgressAsync();
                        });


                    });

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            finally
            {
                await _ProgressService.StopProgressAsync();
            }
        }

        private AddSupplierToIndent _addSupplierToIndent = new AddSupplierToIndent();
        public AddSupplierToIndent AddSupplierToIndent
        {
            get => _addSupplierToIndent;

            set => SetProperty(ref _addSupplierToIndent, value);

        }

        public void SelectedCreditNoteDetails(object obj)
        {
            try
            {
                var _returnModel = (StoreReturnModel)obj;

                if (_returnModel != null && AddSupplierToIndent.FinalPayableAmount > 0)
                {

                    var _selectedCreditNote = AddSupplierToIndent.ReturnModels.Where(x => x.CreditNoteNumber == _returnModel.CreditNoteNumber).FirstOrDefault();
                    if (_selectedCreditNote != null)
                        _selectedCreditNote.RedeemedAmount = 0;
                    var _selectedReturnList = AddSupplierToIndent.ReturnModels.Where(x => x.IsSelected == true).ToList();

                    var totalSelectedAmount = _selectedReturnList.Where(x => x.IsSelected).Sum(x => x.Total);

                    if (AddSupplierToIndent.PayableAmount < totalSelectedAmount)
                    {
                        //foreach (var item in AddSupplierToIndent.ReturnModels)
                        //{
                        //    if (item.Id == _returnmodel.Id)
                        //    {
                        //        item.IsSelected = false;

                        //    }
                        //}

                        //_notificationService.ShowMessage("Credit amount sholud not greater then Net Amount", Common.NotificationType.Error);

                        //return;
                        foreach (var item in _selectedReturnList)
                        {
                            if (item.Id == _returnModel.Id)
                            {
                                item.RedeemedAmount = (decimal)AddSupplierToIndent.FinalPayableAmount;
                                item.IsEdited = true;

                            }
                        }
                        AddSupplierToIndent.SelectedReturnModels = _selectedReturnList;
                        AddSupplierToIndent.FinalPayableAmount = 0;


                    }
                    else
                    {
                        if (_selectedReturnList != null && _selectedReturnList.Count > 0)
                        {
                            AddSupplierToIndent.SelectedReturnModels = _selectedReturnList;

                            AddSupplierToIndent.FinalPayableAmount = AddSupplierToIndent.PayableAmount - totalSelectedAmount;
                        }
                    }






                }
                else
                {
                    foreach (var item in AddSupplierToIndent.ReturnModels)
                    {
                        if (item.Id == _returnModel.Id)
                        {
                            item.IsSelected = false;

                        }
                    }
                }


            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }
        public void UnSelectedCreditNoteDetails(object obj)
        {
            try
            {

                var _returnModel = (StoreReturnModel)obj;

                if (_returnModel != null)
                {

                    var selectedReturnList = AddSupplierToIndent.ReturnModels.Where(x => x.IsSelected == true).ToList();

                    if (selectedReturnList != null && selectedReturnList.Count > 0)
                    {
                        AddSupplierToIndent.SelectedReturnModels = selectedReturnList;
                        var totalSelectedAmount = selectedReturnList.Where(x => x.IsSelected).Sum(x => x.Total);
                        if (AddSupplierToIndent.PayableAmount < totalSelectedAmount)
                        {
                            AddSupplierToIndent.FinalPayableAmount = 0;
                        }
                        else
                        {
                            AddSupplierToIndent.FinalPayableAmount = AddSupplierToIndent.PayableAmount - totalSelectedAmount;
                        }

                    }
                    else
                    {
                        AddSupplierToIndent.SelectedReturnModels = null;

                        AddSupplierToIndent.FinalPayableAmount = AddSupplierToIndent.PayableAmount;
                    }

                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public void CloseSelectedCreditNoteDetails(object obj)
        {
            try
            {

                var _returnModel = (StoreReturnModel)obj;

                if (_returnModel != null)
                {

                    var selectedReturnList = AddSupplierToIndent.SelectedReturnModels.Where(x => x.CreditNoteNumber != _returnModel.CreditNoteNumber).ToList();


                    AddSupplierToIndent.SelectedReturnModels = selectedReturnList;

                    var totalSelectedAmount = selectedReturnList.Where(x => x.IsSelected).Sum(x => x.RedeemedTotal);


                    foreach (var item in AddSupplierToIndent.ReturnModels)
                    {
                        if (item.Id == _returnModel.Id)
                        {
                            item.IsSelected = false;
                        }
                    }

                    AddSupplierToIndent.FinalPayableAmount = AddSupplierToIndent.PayableAmount - totalSelectedAmount;

                }



            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private void RedeemedAmountChange(object obj)
        {
            try
            {
                var _selectedReturnList = AddSupplierToIndent.ReturnModels.Where(x => x.IsSelected == true).ToList();

                var totalSelectedAmount = _selectedReturnList.Where(x => x.IsSelected).Sum(x => x.RedeemedTotal);

                //if (AddSupplierToIndent.PayableAmount < totalselectedamount)
                //{
                //    foreach (var item in AddSupplierToIndent.ReturnModels)
                //    {
                //        if (item.Id == _returnmodel.Id)
                //        {
                //            item.IsSelected = false;

                //        }
                //    }

                //    _notificationService.ShowMessage("Credit amount sholud not greater then Net Amount", Common.NotificationType.Error);

                //    return;
                //}



                if (_selectedReturnList != null && _selectedReturnList.Count > 0)
                {
                    if (AddSupplierToIndent.PayableAmount >= totalSelectedAmount)
                    {
                        AddSupplierToIndent.SelectedReturnModels = _selectedReturnList;

                        AddSupplierToIndent.FinalPayableAmount = AddSupplierToIndent.PayableAmount - totalSelectedAmount;
                    }

                }



            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

            }
        }

        private int _poId;

        public int PoId
        {
            get { return _poId; }
            set { SetProperty(ref _poId, value); }
        }

        public void PaymentUpdate(object param)
        {
            try
            {
                if (AddSupplierToIndent.SelectedReturnModels == null || AddSupplierToIndent.SelectedReturnModels.Count == 0)
                {
                    _notificationService.ShowMessage("Please adjust credit note", Common.NotificationType.Error);

                    return;
                }

                if (AddSupplierToIndent.SelectedReturnModels.Sum(x => x.RedeemedTotal) > AddSupplierToIndent.PayableAmount)
                {
                    _notificationService.ShowMessage("Please check adjusted amount", Common.NotificationType.Error);

                    return;
                }

                var TargetClose = ((Button)(param));
                var dynamicCommand = TargetClose.Command;
                dynamicCommand.CanExecute(true);
                //pass Model as Arguments
                dynamicCommand.Execute(this);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }


        public  void SelectIndividualIndent(object obj)
        {
            try
            {
                var _model = (IndentDownloadModel)obj;

                if (_model != null)
                {
                    if (BulkPaymentModels?.Count > 0)
                    {
                        IsExportEnabled = BulkPaymentModels.Any(x => x.IndentLists.Any(y => y.IsSelected == true));
                        //if (!_model.IsSelected)
                        //    BulkPaymentModels.Where(x => x.SupplierId == _model.SupplierId).FirstOrDefault().IsSelected = false;
                        //GetSupplierTDS(_model.SupplierId);
                        //if (_model.IsSelected)
                        //{
                        //    if (supplierTDSStatuses.ContainsKey(_model.SupplierId))
                        //    {
                        //        var _currentSupplierTDS = supplierTDSStatuses[_model.SupplierId];
                        //        if (_currentSupplierTDS.IsTDSApplicable)
                        //        {
                        //            _model.TDS = (float)((_model.PayableAmount + _model.CNAmount) * _currentSupplierTDS.TDSLimit);
                        //        }
                        //    }

                        //}
                        //else
                        //    _model.TDS = 0;
                       
                    }
                }
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

                            Stores.Insert(0, new Store { StoreId = 0, Name = "All" });
                        });
                    }

                });

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private ObservableCollection<Store> _stores;
        public ObservableCollection<Store> Stores
        {
            get => _stores;
            set => SetProperty(ref _stores, value);
        }

        private Store _selectedStore;

        public Store SelectedStore
        {
            get => _selectedStore;
            set => SetProperty(ref _selectedStore, value);

        }

        private ObservableCollection<CNAmountPopup> _cnAmountPopups;

        public ObservableCollection<CNAmountPopup> CNAmountPopups
        {
            get => _cnAmountPopups;
            set => SetProperty(ref _cnAmountPopups, value);
        }

        private List<BulkDownloadExport> _sbiBulkExport = new List<BulkDownloadExport>();

        public List<BulkDownloadExport> SBIBulkExport
        {
            get => _sbiBulkExport;
            set => SetProperty(ref _sbiBulkExport, value);
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


        private void EventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {

                var _viewModel = (BulkDownloadViewModel)eventArgs.Parameter;

                if (_viewModel != null)
                {



                    if (!_fileName.IsValidString())
                    {
                        if (SelectedBank == "SBI")
                        {
                            _fileName = "SBI Bulk";
                            _fileName += Guid.NewGuid().ToString().Substring(0, 3);

                            IsExportEnabled = false;

                            bool _result = _commonService.ExportToXL(SBIBulkExport, _fileName, skipfileName: true, tablename: "Falca POS SBI bulk Payment", FilePath: ApplicationSettings.BULKPAYMENT_PATH.ToString());

                            if (_result)
                            {

                                _notificationService.ShowMessage("File exported to folder C:\\FALCAPOS\\BulkPayment\\"+_fileName, Common.NotificationType.Success);

                            }
                            else
                            {
                                _notificationService.ShowMessage("File export failed , try again", Common.NotificationType.Error);
                            }
                        }
                        else if (SelectedBank == "ICICI")
                        {
                            _fileName = "ICICI Bulk";
                            _fileName += Guid.NewGuid().ToString().Substring(0, 3);

                            IsExportEnabled = false;

                            bool _result = _commonService.ExportToXL(ICICBulkExport, _fileName, skipfileName: true, tablename: "Falca POS ICICI bulk Payment", FilePath: ApplicationSettings.BULKPAYMENT_PATH.ToString());

                            if (_result)
                            {

                                _notificationService.ShowMessage("File exported to folder C:\\FALCAPOS\\BulkPayment\\"+_fileName, Common.NotificationType.Success);

                            }
                            else
                            {
                                _notificationService.ShowMessage("File export failed , try again", Common.NotificationType.Error);
                            }
                        }

                    }


                    IsExportEnabled = true;

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


        public void checkboxCommand(object obj)
        {
            try
            {
                var content = ((CheckBox)obj);
                if (content.IsChecked == true && content.Content.ToString() == "All")
                {
                    foreach (var item in Suppliers)
                    {
                        item.IsEnable = true;
                    }

                }

                if (Suppliers.Where(x => x.IsEnable).ToList().Count > 1)
                    SelectedSupplier = new PendingPaymentSupplier { SupplierName = "Multiple", IsEnable = false, SupplierId = 00, SupplierType = "Dealer" };
                else
                    SelectedSupplier = Suppliers.Where(x => x.IsEnable).FirstOrDefault();



            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }
        public void UncheckboxCommand(object obj)
        {
            try
            {
                var content = ((CheckBox)obj);

                if (content.IsChecked == false)
                {
                    if (content.Content != null)
                    {
                        if (content.Content.ToString() == "All")
                        {
                            foreach (var item in Suppliers)
                            {
                                item.IsEnable = false;
                            }
                        }
                    }


                }

                if (Suppliers.Where(x => x.IsEnable).ToList().Count > 1)
                    SelectedSupplier = new PendingPaymentSupplier { SupplierName = "Multiple", IsEnable = false, SupplierId = 00, SupplierType = "Dealer" };
                else
                    SelectedSupplier = Suppliers.Where(x => x.IsEnable).FirstOrDefault();



            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public void RefreshSupplier()
        {
            try
            {
                LoadSuppliers();
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }


        public void DownloadExcel(object obj)
        {
            try
            {

                if (string.IsNullOrEmpty(SelectedBank))
                {
                    _notificationService.ShowMessage("Please select bank", NotificationType.Error);
                    return;

                }
                var TargetClose = ((Button)(obj));
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


        private string _selectedBank;

        public string SelectedBank
        {
            get => _selectedBank;
            set => SetProperty(ref _selectedBank, value);
        }

        private List<ICICExport> _icicBulkExport = new List<ICICExport>();

        public List<ICICExport> ICICBulkExport
        {
            get => _icicBulkExport;
            set => SetProperty(ref _icicBulkExport, value);
        }



    }
    public class CNAmountPopup : BindableBase
    {

        public string Suppplier { get; set; }

        private List<StoreReturnModel> _selectedreturnModels;
        public List<StoreReturnModel> SelectedReturnModels
        {
            get { return _selectedreturnModels; }

            set { SetProperty(ref _selectedreturnModels, value); }

        }

    }





}
