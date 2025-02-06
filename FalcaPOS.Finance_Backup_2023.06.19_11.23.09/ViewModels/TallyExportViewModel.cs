using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Finance;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.Finance.ViewModels
{
    public class TallyExportViewModel : BindableBase
    {
        private readonly Logger _logger;

        private CancellationTokenSource _cancellationTokenSource;

        private readonly INotificationService _notificationService;
        public DelegateCommand ExportTallyCommand { get; private set; }

        public DelegateCommand RestTallyCommand { get; private set; }

        public DelegateCommand<object> OpenExcelFileCommand { get; private set; }



        private readonly IFinanceService _financeService;

        private readonly ICommonService _commonService;

        private readonly ProgressService _ProgressService;

        DirectoryInfo directory = new DirectoryInfo(@"C:\FALCAPOS\TallyReports"); //Assuming Test is your Folder


        public TallyExportViewModel(ProgressService ProgressService, ICommonService commonService, IFinanceService financeService, Logger logger, INotificationService notificationService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            ExportTallyCommand = new DelegateCommand(LoadData);

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _financeService = financeService ?? throw new ArgumentNullException(nameof(financeService));

            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));


            _ProgressService = ProgressService ?? throw new ArgumentNullException(nameof(ProgressService));

            RestTallyCommand = new DelegateCommand(ResetInputData);

            OpenExcelFileCommand = new DelegateCommand<object>(OpenExcelfile);
            ExportFolder = ApplicationSettings.TALLYREPORTS_PATH;

            LoadRecentDownload();

            FromDateLabel = "Date(From)";
            ToDateLabel = "Date(To)";


        }

        public async void LoadData()
        {
            try
            {
                if (string.IsNullOrEmpty(FromInvoiceDate))
                {
                    //empty collection 

                    _notificationService.ShowMessage("Please select from  date", NotificationType.Error);

                    return;
                }

                if (string.IsNullOrEmpty(ToInvoiceDate))
                {
                    _notificationService.ShowMessage("Please select to  date", NotificationType.Error);
                    return;
                }

                if (string.IsNullOrEmpty(Type))
                {
                    _notificationService.ShowMessage("Please select the Voucher type", NotificationType.Error);
                    return;
                }



                if (!String.IsNullOrEmpty(FromInvoiceDate) && !string.IsNullOrEmpty(ToInvoiceDate))
                {
                    DateTime dt1 = Convert.ToDateTime(FromInvoiceDate);
                    DateTime dt2 = Convert.ToDateTime(ToInvoiceDate);
                    if (dt2 < dt1)
                    {
                        _notificationService.ShowMessage("From Date should be less than or equal to To Date", NotificationType.Error);
                        return;
                    }
                }

                TallyExportSearchModel model = new TallyExportSearchModel()
                {
                    FromInvoiceDate = FromInvoiceDate,
                    ToInvoiceDate = ToInvoiceDate,
                    Type = Type,
                };


                await _ProgressService.StartProgressAsync();

                await Task.Run(async () =>
                {
                    var result = await _financeService.GetTallyExport(model);

                    if (result != null && result.IsSuccess)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (Type == "Purchase")
                            {
                                if (result != null && result.Data.Count() > 0)
                                {
                                    List<TallyExportPurchaseOrderModel> purchaseOrderModels = new List<TallyExportPurchaseOrderModel>();
                                    var lastinvoiceno = "";
                                    foreach (var item in result.Data)
                                    {
                                        Match m = Regex.Match(Convert.ToString(item.Roundoff), @"[-]");
                                        var roundiff = "";

                                        if (m.Success)
                                        {
                                            roundiff = Regex.Replace(Convert.ToString(item.Roundoff), @"[-]", "");
                                        }

                                        TallyExportPurchaseOrderModel purchaseOrderModel = new TallyExportPurchaseOrderModel()
                                        {
                                            Type = item.type,
                                            ReferEPR = item.NoasEPR,
                                            DateEPR = item.DateasEPR.ToString("dd-MM-yyyy"),
                                            InvoiceNo = item.InvoiceNo,
                                            InvoiceDate = item.InvoiceDate,
                                            SupplierName = item.SupplierName,
                                            GSTIN = item.GSTIN,
                                            PurchaseLedger = GetLedgerCalculation("Purchase", AppConstants.PURCHASE_LEDGER, Convert.ToInt32(item.GST)),
                                            Pincode = item.Pincode,
                                            State = item.State,
                                            District = item.District,
                                            ProductName = item.SKU + "-" + item.ProductName,
                                            Godown = (item.StoreName != null && item.StoreName != "") ? item.StoreName.Remove(0, 3) : null,
                                            SKU = item.SKU,
                                            Qty = item.SubQty,
                                            Rate = item.Rate,
                                            SubTotal = item.SubTotal,
                                            GST = item.GST,
                                            CGST = item.StoreState == item.State ? item.CGST : 0,//gst calucation for between two state
                                            SGST = item.StoreState == item.State ? item.SGST : 0,//gst calucation for between two state
                                            IGST = item.StoreState == item.State ? 0 : (item.CGST + item.SGST),  //gst calucation for between two state
                                            Address = item.Address,
                                            HSN = item.HsnCode,
                                            Bill = item.Bill,
                                            Units = item.Units,
                                            StoreName = item.StoreName,
                                            LedgerCGST = item.StoreState == item.State ? GetLedgerCalculation("Purchase", AppConstants.LEDGER_CGST, Convert.ToInt32(item.GST)) : "",//gst calucation for between two state
                                            LedgerSGST = item.StoreState == item.State ? GetLedgerCalculation("Purchase", AppConstants.LEDGER_SGST, Convert.ToInt32(item.GST)) : "",//gst calucation for between two state
                                            LedgerIGST = item.StoreState == item.State ? "" : GetLedgerCalculation("Purchase", AppConstants.PURCHASE_BETWEEN_TWO_STATS, Convert.ToInt32(item.GST)),//gst calucation for between two state
                                            Transportcharges = item.TransportCharges,
                                            Roundoff = lastinvoiceno == item.InvoiceNo ? 0 : item.Roundoff,//take round off one time only same invoice multiple product
                                            TotalAmount = lastinvoiceno == item.InvoiceNo ? Math.Round(item.TotalAmount, 0, MidpointRounding.AwayFromZero) : m.Success == true ? (Math.Round(item.TotalAmount, 0, MidpointRounding.AwayFromZero)) : (Math.Round(item.TotalAmount + item.Roundoff, 0, MidpointRounding.AwayFromZero)),
                                            BeforeGST = item.BeforeGST,
                                            AfterGST = item.AfterGST,
                                            Others = item.Others,
                                            ExistingSKU=item.ExistingSKU,
                                            DepartmentName= item.DepartmentName,

                                        };
                                        lastinvoiceno = item.InvoiceNo;
                                        purchaseOrderModels.Add(purchaseOrderModel);


                                    }
                                    TallyExportPurchase = purchaseOrderModels;
                                    ExportResultToExcel();

                                }

                            }
                            if (Type == "Sales")
                            {
                                if (result != null && result.Data.Count() > 0)
                                {
                                    List<TallyExportSalesModel> salesOrderModels = new List<TallyExportSalesModel>();
                                    var lastInvoiceNo = "";
                                    foreach (var item in result.Data)
                                    {

                                        TallyExportSalesModel salesOrderModel = new TallyExportSalesModel()
                                        {
                                            Type = item.type,
                                            CostCenter = item.StoreName,
                                            ReferEPR = item.NoasEPR,
                                            DateEPR = item.DateasEPR.ToString("dd-MM-yyyy"),
                                            InvoiceNo = item.InvoiceNo,
                                            InvoiceDate = item.InvoiceDate,
                                            SalesLedger = GetLedgerCalculation("Sales", AppConstants.SALES_LEDGER, Convert.ToInt32(item.GST)),
                                            CustomerName = item.CustomerName,
                                            Bill = item.Bill,
                                            GSTIN = item.GSTIN,
                                            Address = item.Address,
                                            Pincode = Convert.ToString(item.Pincode),
                                            State = item.State,
                                            District = item.District,
                                            ProductName = item.SKU + "-" + item.ProductName,
                                            SKU = item.SKU,
                                            Godown = (item.StoreName != null && item.StoreName != "") ? item.StoreName.Remove(0, 3) : null,
                                            Qty = item.Qty,
                                            Units = item.Units,
                                            HSN = item.HsnCode,
                                            Rate = item.Rate,
                                            SubTotal = item.SubTotal,
                                            TotalAmount = item.TotalAmount,
                                            GST = item.GST,
                                            CGST = item.CGST,
                                            SGST = item.SGST,
                                            IGST = item.IGST,
                                            Cash = lastInvoiceNo == item.InvoiceNo ? 0 : item.Cash,
                                            UPI = lastInvoiceNo == item.InvoiceNo ? 0 : item.UPI,
                                            Cheque = lastInvoiceNo == item.InvoiceNo ? 0 : item.Cheque,
                                            UTRNumber = lastInvoiceNo == item.InvoiceNo ? null : item.UTRNumber,

                                            LedgerCGST = GetLedgerCalculation("Sales", AppConstants.LIABILITY_CGST, Convert.ToInt32(item.GST)),
                                            LedgerSGST = GetLedgerCalculation("Sales", AppConstants.LIABILITY_SGST, Convert.ToInt32(item.GST)),
                                             ExistingSKU = item.ExistingSKU,

                                        };
                                        TallyExportSalesModel serviceorder = new TallyExportSalesModel();

                                        if (item.ServiceCharges != 0)
                                        {
                                            serviceorder.Type = "Sales F-Shops-Service";
                                            serviceorder.CostCenter = item.StoreName;
                                            serviceorder.ReferEPR = item.InvoiceNo + "s";
                                            serviceorder.DateEPR = item.DateasEPR.ToString("dd-MM-yyyy");
                                            serviceorder.InvoiceNo = item.InvoiceNo + "s";
                                            serviceorder.InvoiceDate = item.InvoiceDate;
                                            serviceorder.SalesLedger = "31000001-Income from advisory Service-Taxable-18%";
                                            serviceorder.CustomerName = item.CustomerName;
                                            serviceorder.Bill = item.Bill;
                                            serviceorder.Address = item.Address;
                                            serviceorder.Pincode = Convert.ToString(item.Pincode);
                                            serviceorder.State = item.State;
                                            serviceorder.District = item.District;
                                            serviceorder.Qty = item.Qty;
                                            serviceorder.SubTotal = Math.Round(Math.Round((decimal)item.ServiceCharges / (decimal)(1 + (18.0 / 100.0)), 2, MidpointRounding.AwayFromZero) * item.Qty, 2, MidpointRounding.AwayFromZero);
                                            serviceorder.GST = 18;//fixed 18% services order
                                            serviceorder.CGST = Math.Round((((serviceorder.SubTotal / 100) * 18) / 2), 2);
                                            serviceorder.SGST = Math.Round((((serviceorder.SubTotal / 100) * 18) / 2), 2);
                                            serviceorder.LedgerCGST = "12110015-CGST Output Tax Liability-9%";
                                            serviceorder.LedgerSGST = "12110019-SGST Output Tax Liability-9%";
                                            serviceorder.TotalAmount = (item.ServiceCharges * item.Qty);


                                        }
                                        lastInvoiceNo = item.InvoiceNo;
                                        salesOrderModels.Add(salesOrderModel);
                                        if (serviceorder.InvoiceNo != null)
                                            salesOrderModels.Add(serviceorder);


                                    }
                                    TallyExportSales = salesOrderModels;
                                    ExportResultToExcel();

                                }

                            }


                        });

                    }
                    else
                    {
                        _notificationService.ShowMessage(result.Error, Common.NotificationType.Error);
                      
                    }

                   
                });

              
            }

            catch (Exception _ex)
            {
                _logger.LogError("Error in data export", _ex);               
            }
            finally {
                await _ProgressService.StopProgressAsync();
            }
        }

        private string _fromInvoiceDate;
        public string FromInvoiceDate
        {
            get { return _fromInvoiceDate; }
            set { SetProperty(ref _fromInvoiceDate, value); }
        }

        private string _toInvoiceDate;
        public string ToInvoiceDate
        {
            get { return _toInvoiceDate; }
            set { SetProperty(ref _toInvoiceDate, value); }
        }

        private string _type;

        public string Type
        {
            get { return _type; }
            set
            {

                SetProperty(ref _type, value);
                if (Type == "Purchase")
                {
                    FromDateLabel = "Purchase Date(From)";
                    ToDateLabel = "Purchase Date(To)";

                }
                else
                {
                    FromDateLabel = "Sales Date(From)";
                    ToDateLabel = "Sales Date(To)";
                }

            }
        }

        public string _fileName { get; set; }

        public String _exportFolder;
        public String ExportFolder
        {
            get { return _exportFolder; }

            set { SetProperty(ref _exportFolder, value); }
        }

        private void ExportResultToExcel()
        {
            try
            {
                _fileName = null;
                if (Type == "Purchase")
                {

                    if (TallyExportPurchase == null || !TallyExportPurchase.Any())
                    {
                        _notificationService.ShowMessage("No records found to export", Common.NotificationType.Error);

                        return;
                    }



                    if (!_fileName.IsValidString())
                    {
                        _fileName = FromInvoiceDate + "-" + ToInvoiceDate + "-Purchase";
                    }
                    _fileName += Guid.NewGuid().ToString().Substring(0, 3);

                    bool _result = _commonService.ExportToXL(TallyExportPurchase, _fileName, skipfileName: true, tablename: "Falca POS Tally Report", FilePath: ApplicationSettings.TALLYREPORTS_PATH.ToString());

                    if (_result)
                    {

                        _notificationService.ShowMessage("File exported to folder C:\\FALCAPOS\\TallyReports", Common.NotificationType.Success);
                        LoadRecentDownload();
                    }
                    else
                    {
                        _notificationService.ShowMessage("File export failed , try again", Common.NotificationType.Error);
                    }

                }
                if (Type == "Sales")
                {

                    if (TallyExportSales == null || !TallyExportSales.Any())
                    {
                        _notificationService.ShowMessage("No records found to export", Common.NotificationType.Error);

                        return;
                    }



                    if (!_fileName.IsValidString())
                    {
                        _fileName = FromInvoiceDate + "-" + ToInvoiceDate + "-Sales";
                    }
                    _fileName += Guid.NewGuid().ToString().Substring(0, 3);

                    bool _result = _commonService.ExportToXL(TallyExportSales, _fileName, skipfileName: true, tablename: "Falca POS Tally Report", FilePath: ApplicationSettings.TALLYREPORTS_PATH.ToString());

                    if (_result)
                    {

                        _notificationService.ShowMessage("File exported to folder C:\\FALCAPOS\\TallyReports", Common.NotificationType.Success);
                        LoadRecentDownload();
                    }
                    else
                    {
                        _notificationService.ShowMessage("File export failed , try again", Common.NotificationType.Error);
                    }

                }




            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            finally {

            }



        }

        public string GetLedgerCalculation(string Type, string LadgerType, int GST)
        {

            try
            {
                var _Ledgers = new ParseJSON().ParseJson<List<Ledgers>>("Ledgers");
                if (_Ledgers != null && _Ledgers.Count > 0)
                {
                    var _gst = Convert.ToString(GST) == "Nil" ? "Nil" : Convert.ToString(GST) == "0" ? "Nil" : Convert.ToString(GST) + "%";
                    //purchase taxs
                    if (LadgerType == AppConstants.PURCHASE_LEDGER)
                    {
                        var _ledgerHead = _Ledgers.Where(x => x.Types == Type).FirstOrDefault().PurchaseLedger.Where(x => x.GSTRate == _gst).FirstOrDefault();
                        if (_ledgerHead != null)
                            return _ledgerHead.LedgerHead;

                    }
                    else if (LadgerType == AppConstants.LEDGER_CGST)
                    {
                        var _ledgerHead = _Ledgers.Where(x => x.Types == Type).FirstOrDefault().LedgerCGST.Where(x => x.GSTRate == _gst).FirstOrDefault();
                        if (_ledgerHead != null)
                            return _ledgerHead.LedgerHead;

                    }
                    else if (LadgerType == AppConstants.LEDGER_SGST)
                    {
                        var _ledgerHead = _Ledgers.Where(x => x.Types == Type).FirstOrDefault().LedgerSGST.Where(x => x.GSTRate == _gst).FirstOrDefault();
                        if (_ledgerHead != null)
                            return _ledgerHead.LedgerHead;
                    }
                    //sales Taxs
                    else if (LadgerType == AppConstants.SALES_LEDGER)
                    {
                        var _ledgerHead = _Ledgers.Where(x => x.Types == Type).FirstOrDefault().SalesLedger.Where(x => x.GSTRate == _gst).FirstOrDefault();
                        if (_ledgerHead != null)
                            return _ledgerHead.LedgerHead;
                    }
                    else if (LadgerType == AppConstants.LIABILITY_CGST)
                    {
                        var _ledgerHead = _Ledgers.Where(x => x.Types == Type).FirstOrDefault().LiabilityCGST.Where(x => x.GSTRate == _gst).FirstOrDefault();
                        if (_ledgerHead != null)
                            return _ledgerHead.LedgerHead;
                    }
                    else if (LadgerType == AppConstants.LIABILITY_SGST)
                    {
                        var _ledgerHead = _Ledgers.Where(x => x.Types == Type).FirstOrDefault().LiabilitySGST.Where(x => x.GSTRate == _gst).FirstOrDefault();
                        if (_ledgerHead != null)
                            return _ledgerHead.LedgerHead;
                    }
                    if (LadgerType == AppConstants.PURCHASE_BETWEEN_TWO_STATS)
                    {
                        var _ledgerHead = _Ledgers.Where(x => x.Types == Type).FirstOrDefault().PurchaseBetweentwostates.Where(x => x.GSTRate == _gst).FirstOrDefault();
                        if (_ledgerHead != null)
                            return _ledgerHead.LedgerHead;

                    }

                }
                return null;
            }
            catch (Exception _ex)
            {
                _logger.LogError("getting error in GST Ledger", _ex);
                return null;
            }
            finally {

            }


        }

        public void ResetInputData()
        {
            try
            {
                FromInvoiceDate = null;
                ToInvoiceDate = null;
                Type = null;
                FromDateLabel = "Date(From)";
                ToDateLabel = "Date(To)";


            }
            catch (Exception _ex)
            {
                _logger.LogError("getting error in Reset input data method", _ex);
            }
        }

        public void LoadRecentDownload()
        {
            try
            {
                List<RecentDownload> recentDownloads = new List<RecentDownload>();

                FileInfo[] Files = directory.GetFiles("*.xls").OrderByDescending(x => x.CreationTime).ToArray(); //Getting xl files
                int i = 1;
                foreach (FileInfo file in Files)
                {
                    if (i <= 10 && file.Length != 0)
                    {

                        //remove duplicate file

                        Match m = Regex.Match(file.Name, @"[~$]");
                        if (!m.Success)
                        {
                            Double mbsize = file.Length / (1024 * 1024);
                            recentDownloads.Add(new RecentDownload()
                            {
                                FileName = file.Name,
                                FileSize = mbsize != 0 ? Convert.ToString(mbsize) + "MB" : Convert.ToString(file.Length / 1024) + "KB",
                                DownloadDate = file.CreationTime.ToString("dd-MM-yyyy") + " (" + DateTimeHumanizer.HumanizeString(file.CreationTime) + ")"
                            });
                            i++;
                        }



                    }

                }


                RecentDownloads = recentDownloads;

            }
            catch (Exception _ex)
            {
                _logger.LogError("getting error in LoadRecentDownload", _ex);
            }
        }

        public void OpenExcelfile(object filename)
        {
            try
            {


                FileInfo fi = new FileInfo(directory + "\\" + filename);
                if (fi.Exists)
                {
                    System.Diagnostics.Process.Start(directory + "\\" + filename);
                }
                else
                {
                    _notificationService.ShowMessage("File Not Found", Common.NotificationType.Error);
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError("getting error in open excel file", _ex);
            }
        }




        private List<RecentDownload> _recentDownloads;

        public List<RecentDownload> RecentDownloads
        {
            get { return _recentDownloads; }
            set { SetProperty(ref _recentDownloads, value); }
        }

        private List<TallyExportPurchaseOrderModel> _TallyExportPurchase;
        public List<TallyExportPurchaseOrderModel> TallyExportPurchase
        {
            get => _TallyExportPurchase;
            set => _ = SetProperty(ref _TallyExportPurchase, value);
        }

        private List<TallyExportSalesModel> _TallyExportSales;
        public List<TallyExportSalesModel> TallyExportSales
        {
            get => _TallyExportSales;
            set => _ = SetProperty(ref _TallyExportSales, value);
        }


        private string _fromdatelabel;

        public string FromDateLabel
        {
            get { return _fromdatelabel; }
            set { SetProperty(ref _fromdatelabel, value); }
        }

        private string _todatelabel;

        public string ToDateLabel
        {
            get { return _todatelabel; }
            set { SetProperty(ref _todatelabel, value); }
        }


    }

    public class TallyExportPurchaseOrderModel
    {
        [DisplayName("Voucher Type")]
        public string Type { get; set; }

        [DisplayName("Cost Centre/Classes")]
        public string StoreName { get; set; }

        [DisplayName("Reference No as per ERP")]
        public string ReferEPR { get; set; }

        [DisplayName("Date as per ERP")]
        public string DateEPR { get; set; }

        [DisplayName("Invoice No")]
        public string InvoiceNo { get; set; }

        [DisplayName("Invoice Date")]
        public DateTime InvoiceDate { get; set; }

        [DisplayName("Purchase Ledger")]
        public string PurchaseLedger { get; set; }

        [DisplayName("Supplier Name")]
        public string SupplierName { get; set; }


        [DisplayName("Supplier (Bill From)")]
        public string Bill { get; set; }

        public string GSTIN { get; set; }

        [DisplayName("Address fields as per DSC/POS")]
        public string Address { get; set; }

        public int Pincode { get; set; }

        public string District { get; set; }

        public string State { get; set; }

        public string DepartmentName { get; set; }
        public string ProductName { get; set; }

        public string Godown { get; set; }

        [DisplayName("Existing SKU(Old SKU)")]
        public string ExistingSKU { get; set; }

        public string SKU { get; set; }


        public string Units { get; set; }

        [DisplayName("HSN Code")]
        public string HSN { get; set; }

        public int Qty { get; set; }

        public decimal Rate { get; set; }

        [DisplayName("Sub Total")]
        public decimal SubTotal { get; set; }

        [DisplayName("GST Rate")]
        public decimal GST { get; set; }

        [DisplayName("Input CGST")]
        public decimal CGST { get; set; }

        [DisplayName("Input SGST")]
        public decimal SGST { get; set; }

        [DisplayName("Input IGST")]
        public decimal IGST { get; set; }

        public decimal Roundoff { get; set; }

        [DisplayName("Total Invoice Amount")]
        public decimal TotalAmount { get; set; }

        [DisplayName("Input Credit Ledger-CGST")]
        public string LedgerCGST { get; set; }

        [DisplayName("Input Credit Ledger-SGST")]
        public string LedgerSGST { get; set; }

        [DisplayName("Input Credit Ledger-IGST")]
        public string LedgerIGST { get; set; }

        [DisplayName("Sourcing Employee")]
        public string SourceEmployee { get; set; }

        [DisplayName("Freight Charges Clearing")]
        public decimal Transportcharges { get; set; }

        [DisplayName(" Loading and Unloading Charges Clearing")]
        public string LoadandUnloadCharge { get; set; }

        [DisplayName("Gunny Bags Expenses Clearing")]
        public string GunnyBags { get; set; }

        [DisplayName("Weighbridge Charges Clearing")]
        public string Weighbridge { get; set; }

        [DisplayName("APMC Cess Clearing")]
        public string APMCCess { get; set; }

        [DisplayName("Lives Deduct Charges Clearing")]
        public string LivesDeduct { get; set; }

        [DisplayName("Discount-Before GST")]
        public decimal BeforeGST { get; set; }


        [DisplayName("Discount-After GST")]
        public decimal AfterGST { get; set; }


        [DisplayName("Farmer Advance Clearing")]
        public string FarmerAdvance { get; set; }

        [DisplayName("Transporter Advance Clearing")]
        public string TransporterAdvance { get; set; }

        [DisplayName("Others")]
        public decimal Others { get; set; }
    }

    public class TallyExportSalesModel
    {
        [DisplayName("Voucher Type")]
        public string Type { get; set; }

        [DisplayName("Cost Centre/Classes")]
        public string CostCenter { get; set; }

        [DisplayName("Sales No as per ERP")]
        public string ReferEPR { get; set; }

        [DisplayName("Date as per ERP")]
        public string DateEPR { get; set; }

        [DisplayName("Sales Invoice No")]
        public string InvoiceNo { get; set; }

        [DisplayName("Invoice Date")]
        public DateTime InvoiceDate { get; set; }

        [DisplayName("Sales Ledger")]
        public string SalesLedger { get; set; }

        public string CustomerName { get; set; }

        [DisplayName("Customer (Bill To)")]
        public string Bill { get; set; }

        public string GSTIN { get; set; }

        [DisplayName("Address fields as per DSC/POS")]
        public string Address { get; set; }

        public string Pincode { get; set; }

        public string District { get; set; }

        public string State { get; set; }

        public string ProductName { get; set; }

        [DisplayName("Existing SKU(Old SKU)")]
        public string ExistingSKU { get; set; }
        public string SKU { get; set; }

        public string Godown { get; set; }
        public string Units { get; set; }

        [DisplayName("HSN Code")]
        public string HSN { get; set; }
        public int Qty { get; set; }

        public decimal Rate { get; set; }

        [DisplayName("Sub Total")]
        public decimal SubTotal { get; set; }

        [DisplayName("GST Rate")]
        public decimal GST { get; set; }



        [DisplayName("Output CGST")]
        public decimal CGST { get; set; }

        [DisplayName("Output SGST")]
        public decimal SGST { get; set; }

        [DisplayName("Output IGST")]
        public decimal IGST { get; set; }

        [DisplayName("Total Invoice Amount")]
        public decimal TotalAmount { get; set; }

        [DisplayName("CGST LEDGER")]
        public string LedgerCGST { get; set; }

        [DisplayName("SGST LEDGER")]
        public string LedgerSGST { get; set; }

        [DisplayName("IGST LEDGER")]
        public string LedgerIGST { get; set; }

        [DisplayName("Sourcing Employee")]
        public string SourceEmployee { get; set; }

        [DisplayName("Cash")]
        public decimal Cash { get; set; }

        [DisplayName("UPI")]
        public decimal UPI { get; set; }

        public decimal Cheque { get; set; }

        [DisplayName("UTRnumber")]
        public string UTRNumber { get; set; }


    }

    public class Ledgers
    {
        public string Types { get; set; }
        public List<Ledger> PurchaseLedger { get; set; }

        public List<Ledger> PurchaseBetweentwostates { get; set; }
        public List<Ledger> LedgerCGST { get; set; }
        public List<Ledger> LedgerSGST { get; set; }

        public List<Ledger> SalesLedger { get; set; }
        public List<Ledger> LiabilityCGST { get; set; }
        public List<Ledger> LiabilitySGST { get; set; }
    }
    public class Ledger
    {
        public string GSTRate { get; set; }
        public string LedgerHead { get; set; }
        public string Remarks { get; set; }
    }

    public class RecentDownload
    {
        public string FileName { get; set; }

        public string FileSize { get; set; }

        public string DownloadDate { get; set; }
    }

}
