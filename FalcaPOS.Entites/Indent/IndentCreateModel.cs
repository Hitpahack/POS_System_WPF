using FalcaPOS.Entites.AddInventory;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.Suppliers;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace FalcaPOS.Entites.Indent
{
    public class IndentCreateModel
    {

        public string PoNumber { get; set; }
        public string Type { get; set; }
        public IEnumerable<IndentProduct> Products { get; set; }

        public int StoreId { get; set; }

    }

    public class IndentProduct
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }

    public class IndentViewModel : BindableBase
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private string _poNumber;
        public string PoNumber
        {
            get { return _poNumber; }
            set { SetProperty(ref _poNumber, value); }
        }

        private string _date;
        public string Date
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
        }

        private string _remark;
        public string Remark
        {
            get { return _remark; }
            set { SetProperty(ref _remark, value); }
        }

        private string _storeName;
        public string StoreName
        {
            get { return _storeName; }
            set { SetProperty(ref _storeName, value); }
        }

        public int IndentStoreId { get; set; }

        private string _status;
        public string Status { get { return _status; } set { SetProperty(ref _status, value); } }

        public string Type { get; set; }
        public string ApprovedBy { get; set; }

        public string CreatedBy { get; set; }
        public bool IsSelectedShallow { get; set; }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        public bool HasInformation { get; set; }

        private IEnumerable<IndentViewProduct> _product;
        public IEnumerable<IndentViewProduct> Products
        {
            get { return _product; }
            set { SetProperty(ref _product, value); }
        }


        private List<ActivityLogs> _activityLog;
        public List<ActivityLogs> AcivityLogs
        {
            get { return _activityLog; }
            set
            {
                SetProperty(ref _activityLog, value);
            }
        }


        private IndentStatus _indentstatus;
        public IndentStatus Indentstatus
        {
            get { return _indentstatus; }
            set { SetProperty(ref _indentstatus, value); }
        }

        public int SupplierId { get; set; }

        private string _supplierName;
        public string SupplierName { get { return _supplierName; } set { SetProperty(ref _supplierName, value); } }

        private DateTime? _arrivingdate;
        public DateTime? ArrivingDate { get { return _arrivingdate; } set { SetProperty(ref _arrivingdate, value); } }

        private string date;
        public string ArriDate { get { return date; } set { SetProperty(ref date, value); } }

        private String _creditperiod;
        public String CreditPeriod { get { return _creditperiod; } set { SetProperty(ref _creditperiod, value); } }

        private string _trackingId;
        public string TrackingId { get { return _trackingId; } set { SetProperty(ref _trackingId, value); } }

        private SupplierDetails _supplierDetails;
        public SupplierDetails SupplierDetails { get { return _supplierDetails; } set { SetProperty(ref _supplierDetails, value); } }

        private int _selectedIndex;
        public int SelectedIndex { get { return _selectedIndex; } set { SetProperty(ref _selectedIndex, value); } }


        public List<IndentInventoryDetails> _indentInventoryDetails;

        public List<IndentInventoryDetails> IndentInventoryDetails { get { return _indentInventoryDetails; } set { SetProperty(ref _indentInventoryDetails, value); } }

        public List<IndentInventoryDetails> _indentInventoryDCDetails;

        public List<IndentInventoryDetails> IndentInventoryDCDetails { get { return _indentInventoryDCDetails; } set { SetProperty(ref _indentInventoryDCDetails, value); } }


        private string _gstType;

        public string GSTType
        {
            get { return _gstType; }
            set { SetProperty(ref _gstType, value); }
        }


        private bool _isInclusiveGST;

        public bool IsInclusiveGST
        {
            get { return _isInclusiveGST; }
            set { SetProperty(ref _isInclusiveGST, value); }
        }

        public string ProperStatus { get; set; }

        public DateTime? CreateDate { get; set; }

        public AddressViewModel SelecedSupplierAddress { get; set; }

        private List<BankDetails> bankDetails;

        public List<BankDetails> BankDetails
        {
            get { return bankDetails; }
            set { SetProperty(ref bankDetails, value); }
        }

        private float _totalAmount;

        public float TotalAmount
        {
            get { return _totalAmount; }
            set { SetProperty(ref _totalAmount, value); }
        }



        private string _paymnetdate;
        public string PaymentDate
        {
            get { return _paymnetdate; }
            set { SetProperty(ref _paymnetdate, value); }
        }

        private string _refrenceNo;
        public string RefrenceNo
        {
            get { return _refrenceNo; }

            set { SetProperty(ref _refrenceNo, value); }
        }

        private ObservableCollection<FileUploadInfo> fileUploadListInfo;
        public ObservableCollection<FileUploadInfo> FileUploadListInfo
        {
            get { return fileUploadListInfo; }
            set { SetProperty(ref fileUploadListInfo, value); }


        }

        private float _advisoryCharges;

        public float AdvisoryCharges
        {
            get { return _advisoryCharges; }
            set { SetProperty(ref _advisoryCharges, value); }
        }

        private FileAttachment _fileAttachment;
        public FileAttachment FileAttachments
        {
            get { return _fileAttachment; }

            set { SetProperty(ref _fileAttachment, value); }

        }


        private float _payableAmount;

        public float PayableAmount
        {
            get { return _payableAmount; }
            set { SetProperty(ref _payableAmount, value); }
        }

        public bool IsFullPaid { get; set; }

        private bool _isupdatebtnvisility;
        public bool IsUpateBtnVisiblity
        {
            get { return _isupdatebtnvisility; }
            set { SetProperty(ref _isupdatebtnvisility, value); }

        }

        private bool _isrefrenceno;
        public bool IsRefrenceNo
        {
            get { return _isrefrenceno; }
            set { SetProperty(ref _isrefrenceno, value); }
        }

        private bool _ispaymentdate;
        public bool IsPaymentDate
        {
            get { return _ispaymentdate; }
            set { SetProperty(ref _ispaymentdate, value); }

        }

        private bool _isProformaDownloadbtnEnabled;
        public bool IsProformaDownloadbtnEnabled
        {
            get { return _isProformaDownloadbtnEnabled; }
            set { SetProperty(ref _isProformaDownloadbtnEnabled, value); }
        }


        public DateTime? CreateDatetime { get; set; }


        private float _totalEstimatedAmount;
        public float TotalEstimantedAmount
        {
            get { return _totalEstimatedAmount; }

            set { SetProperty(ref _totalEstimatedAmount, value); }
        }

        private int _totalAvailableQty;

        public int TotalAvailableQty
        {
            get { return _totalAvailableQty; }
            set { SetProperty(ref _totalAvailableQty, value); }
        }

        private BankDetails _selectedbankdetail;
        public BankDetails SelectedBankDetail { get { return _selectedbankdetail; } set { SetProperty(ref _selectedbankdetail, value); } }


        private List<StoreReturnModel> _selectedreturnModels;
        public List<StoreReturnModel> SelectedReturnModels
        {
            get { return _selectedreturnModels; }

            set { SetProperty(ref _selectedreturnModels, value); }

        }

        private float _finalpayableAmount;

        public float FinalPayableAmount
        {
            get { return _finalpayableAmount; }
            set { SetProperty(ref _finalpayableAmount, value); }
        }



        private bool _istotalEstimatedAmount;


        public bool IsTotalEstimantedAmount
        {
            get { return _istotalEstimatedAmount; }

            set { SetProperty(ref _istotalEstimatedAmount, value); }
        }

        private List<PartialPayment> _paymentsList;
        public List<PartialPayment> PaymentsList
        {
            get { return _paymentsList; }
            set { SetProperty(ref _paymentsList, value); }

        }
        public float CNAmount { get; set; }

        private float _balance;
        public float Balance { get { return _balance; } set { SetProperty(ref _balance, value); } }

        private bool? _paymentStatus;
        public bool? PaymentStatus
        {
            get => _paymentStatus;
            set => SetProperty(ref _paymentStatus, value);
        }

        public string InvoiceNo { get; set; }

        private float _tds;

        public float TDS
        {
            get { return _tds; }
            set { SetProperty(ref _tds, value); }
        }


    }

    public class IndentViewProduct : BindableBase
    {

        public int SerailNumber { get; set; }
        public int ProductId { get; set; }

        public String SKU { get; set; }
        public string ProductName { get; set; }
        public string BrandName { get; set; }

        [DisplayName("DepartmentName")]
        public string ProductType { get; set; }

        public int Quantity { get; set; }

        private float _estimatedprice;
        public float EstimatedPrice
        {
            get { return _estimatedprice; }
            set
            {

                SetProperty(ref _estimatedprice, value);

            }
        }

        private int _availableQty;
        public int AvailableQty { get { return _availableQty; } set { SetProperty(ref _availableQty, value); } }

        public int ReceivedQty { get; set; }

        public int ChangeQty { get; set; }

        private bool _isEstimatePrice;
        public bool IsEstimatePrice
        {
            get { return _isEstimatePrice; }
            set
            {
                _isEstimatePrice = value;
                RaisePropertyChanged("IsEstimatePrice");
            }
        }

        private ObservableCollection<GSTslabs> _gstslabs = new ObservableCollection<GSTslabs>();

        public ObservableCollection<GSTslabs> GSTslabs
        {
            get => _gstslabs;
            set
            {
                _gstslabs = value;
                RaisePropertyChanged("GSTslabs");
            }
        }


        private GSTslabs _selectedGSTslab;

        public GSTslabs SelectedGSTslab
        {
            get => _selectedGSTslab;
            set
            {
                _selectedGSTslab = value;
                RaisePropertyChanged("SelectedGSTslab");
            }
        }


        private bool _isInclusiveGst;

        public bool IsInclusiveGst
        {
            get => _isInclusiveGst;
            set
            {
                _isInclusiveGst = value;
                RaisePropertyChanged("IsInclusiveGst");


            }
        }
        private bool _isInclusiveGstdatagrid;

        public bool IsInclusiveGstdatagrid
        {
            get => _isInclusiveGstdatagrid;
            set
            {
                _isInclusiveGstdatagrid = value;
                RaisePropertyChanged("IsInclusiveGstdatagrid");


            }
        }

        private string _subUnitTypr;

        public string SubUnitType
        {
            get { return _subUnitTypr; }
            set { SetProperty(ref _subUnitTypr, value); }
        }

        private float _productTotal;
        public float ProductTotal
        {
            get { return _productTotal; }
            set { SetProperty(ref _productTotal, value); }
        }

        public int InventoryReceivedQty { get; set; }

        private int _posStockQty;

        public int POSStockQty
        {
            get => _posStockQty;

            set => SetProperty(ref _posStockQty, value);

        }

        private int? _tmQty;
        public int? TmQty
        {
            get => _tmQty;
            set => SetProperty(ref _tmQty, value);
        }

        private int? _rmQty;
        public int? RmQty
        {
            get => _rmQty;
            set => SetProperty(ref _rmQty, value);
        }

        private decimal _rsp;
        public decimal Rsp
        {
            get => _rsp;
            set => SetProperty(ref _rsp, value);

        }

        private bool isRmQtyVisible;

        public bool IsRamQtyVisible
        {
            get => isRmQtyVisible;
            set => SetProperty(ref isRmQtyVisible, value);
        }

        private string _eta;

        public string ETA
        {
            get => _eta;
            set => SetProperty(ref _eta, value);
        }



    }

    public class ActivityLogs
    {
        public DateTime Date { get; set; }
        public string Status { get; set; }

        public string UserAndRole { get; set; }
    }


    public class AddSupplierToIndent : BindableBase
    {

        private List<IndentViewProduct> _products;

        public List<IndentViewProduct> Products
        {
            get { return _products; }
            set
            {
                _products = value;
                RaisePropertyChanged("Products");
            }
        }


        private SuppliersViewModel _selectedSupplier;

        public SuppliersViewModel SelectedSupplier
        {
            get { return _selectedSupplier; }
            set { SetProperty(ref _selectedSupplier, value); }
        }

        private string _selectedSupplierName;

        public string SelectedSupplierName
        {
            get { return _selectedSupplierName; }
            set { SetProperty(ref _selectedSupplierName, value); }
        }

        private int _supplierid;
        public int SupplierId { get { return _supplierid = (int)SelectedSupplier.SupplierId; } set { SetProperty(ref _supplierid, value); } }

        private string _arrivingDate;

        public string ArrivingDate
        {
            get { return _arrivingDate; }
            set { SetProperty(ref _arrivingDate, value); }
        }


        private String _creditPeriod;

        public String CreditPeriod
        {
            get { return _creditPeriod; }
            set
            {

                SetProperty(ref _creditPeriod, value);
                if (_creditPeriod == "0")
                {
                    PaymentDate = "Today";
                }
                else
                {
                    PaymentDate = DateTime.Now.AddDays(Convert.ToDouble(_creditPeriod)).ToString("dd-MM-yyyy");
                }

            }
        }


        private string _gstType;

        public string GSTType
        {
            get { return _gstType; }
            set
            {
                _gstType = value;
                RaisePropertyChanged("GSTType");
                if (_gstType == "Inclusive GST")
                {
                    if (Products != null && Products.Count > 0)
                    {
                        foreach (var item in Products)
                        {
                            item.IsInclusiveGst = true;
                            item.SelectedGSTslab = null;
                        };
                    }
                    IsInClusive = true;
                }
                else
                {
                    if (Products != null && Products.Count > 0)
                    {
                        foreach (var item in Products)
                        {
                            item.IsInclusiveGst = false;
                        };
                    }
                    IsInClusive = false;
                }
            }
        }

        private float _totalAmount;

        public float TotalAmount
        {
            get { return _totalAmount; }
            set { SetProperty(ref _totalAmount, value); }
        }

        private List<BankDetails> bankDetails;

        public List<BankDetails> BankDetails
        {
            get { return bankDetails; }
            set { SetProperty(ref bankDetails, value); }
        }

        private BankDetails selectedbankDetail;

        public BankDetails SelectedBankDetail
        {
            get { return selectedbankDetail; }
            set { SetProperty(ref selectedbankDetail, value); }
        }

        private string _paymnetdate;
        public string PaymentDate
        {
            get { return _paymnetdate; }
            set { SetProperty(ref _paymnetdate, value); }
        }

        private string _refrenceNo;
        public string RefrenceNo
        {
            get { return _refrenceNo; }

            set { SetProperty(ref _refrenceNo, value); }
        }

        private ObservableCollection<FileUploadInfo> fileUploadListInfo;
        public ObservableCollection<FileUploadInfo> FileUploadListInfo
        {
            get { return fileUploadListInfo; }
            set { SetProperty(ref fileUploadListInfo, value); }


        }

        private float _payableAmount;

        public float PayableAmount
        {
            get { return _payableAmount; }
            set { SetProperty(ref _payableAmount, value); }
        }

        private float _transportCharges;

        public float TransportCharges
        {
            get { return _transportCharges; }
            set
            {

                SetProperty(ref _transportCharges, value);
                //if (IsTDSApplicable)
                //{
                //    if ((Math.Round( PurchaseAmount) - CreditNoteAmount) > TdsLimit)
                //    {
                //        var _tdsAmount = ((TotalAmount + TransportCharges) / 100) * TdsDeduct;
                //        TdsDeductAmount = (float)Math.Round(_tdsAmount,2);
                //        PayableAmount = (TotalAmount + TransportCharges) - TdsDeductAmount;
                //        NetPayableWithOutGst = (WithOutGst + TransportCharges);

                //    }
                //    else if ((Math.Round(PurchaseAmount) - CreditNoteAmount) + TotalAmount + TransportCharges > TdsLimit)
                //    {
                //        var _tdsAmount = ((((Math.Round(PurchaseAmount) - CreditNoteAmount) + TotalAmount + TransportCharges) -TdsLimit) / 100) * TdsDeduct;
                //       TdsDeductAmount = (float)Math.Round(_tdsAmount,2);
                //       PayableAmount = (TotalAmount + TransportCharges) - (float)Math.Round(_tdsAmount,2);
                //       NetPayableWithOutGst = (WithOutGst + TransportCharges);
                //    }
                //    else
                //    {
                //        PayableAmount = (TotalAmount + TransportCharges);
                //        NetPayableWithOutGst = (WithOutGst + TransportCharges);
                //        IsTDSApplicable = false;
                //    }
                //    partialPayment.Percentage = null;
                //    partialPayment.Price = 0;
                //    partialPayment.PaymentTotal = 0;
                //    partialPayment.PaymentDate = null;

                //}
                //else
                //{
                //    if ((Math.Round(PurchaseAmount) - CreditNoteAmount) + TotalAmount + TransportCharges > TdsLimit)
                //    {
                //        var _tdsAmount = ((((Math.Round(PurchaseAmount) - CreditNoteAmount) + TotalAmount + TransportCharges) - TdsLimit) / 100) * TdsDeduct;
                //        TdsDeductAmount = (float)Math.Round(_tdsAmount, 2);
                //        PayableAmount = (TotalAmount + TransportCharges) - (float)Math.Round(_tdsAmount, 2);
                //        NetPayableWithOutGst = (WithOutGst + TransportCharges);
                //        IsTDSApplicable = true;
                //    }
                //    else
                //    {
                //        PayableAmount = (TotalAmount + TransportCharges);
                //        NetPayableWithOutGst = (WithOutGst + TransportCharges);
                //    }
                //    partialPayment.Percentage = null;
                //    partialPayment.Price = 0;
                //    partialPayment.PaymentTotal = 0;
                //    partialPayment.PaymentDate = null;

                //}

                //}
                //else
                //{
                //    if ((Math.Round(PurchaseAmount) - CreditNoteAmount) + TotalAmount + TransportCharges > TdsLimit)
                //    {
                //        var _tdsAmount = ((((Math.Round(PurchaseAmount) - CreditNoteAmount) + TotalAmount + TransportCharges) - TdsLimit) / 100) * TdsDeduct;
                //        TdsDeductAmount = (float)Math.Round(_tdsAmount, 2);
                //        PayableAmount = (TotalAmount + TransportCharges) - (float)Math.Round(_tdsAmount, 2);
                //        NetPayableWithOutGst = (WithOutGst + TransportCharges);
                //        IsTDSApplicable = true;
                //    }
                //    else
                //    {
                //        PayableAmount = (TotalAmount + TransportCharges);
                //        NetPayableWithOutGst = (WithOutGst + TransportCharges);
                //    }
                //    partialPayment.Percentage = null;
                //    partialPayment.Price = 0;
                //    partialPayment.PaymentTotal = 0;
                //    partialPayment.PaymentDate = null;

                //}

                PayableAmount = (TotalAmount);
                NetPayableWithOutGst = (WithOutGst);
                IsEnableTransportPayer=TransportCharges>0?true:false;
            }
        }

        private string _paymenttype;

        public string PaymentType
        {
            get { return _paymenttype; }
            set
            {

                SetProperty(ref _paymenttype, value);

            }
        }

        public FileAttachment FileAttachments { get; set; }


        public int TotalRequestQty { get; set; }

        public int TotalAvailableQty { get; set; }

        private List<StoreReturnModel> _returnModels;
        public List<StoreReturnModel> ReturnModels
        {
            get { return _returnModels; }

            set { SetProperty(ref _returnModels, value); }

        }

        private List<StoreReturnModel> _selectedreturnModels;
        public List<StoreReturnModel> SelectedReturnModels
        {
            get { return _selectedreturnModels; }

            set { SetProperty(ref _selectedreturnModels, value); }

        }

        private float _finalpayableAmount;

        public float FinalPayableAmount
        {
            get { return _finalpayableAmount; }
            set { SetProperty(ref _finalpayableAmount, value); }
        }

        private ObservableCollection<PartialPayment> partialPayments;

        public ObservableCollection<PartialPayment> PartialPayments
        {
            get { return partialPayments; }
            set { SetProperty(ref partialPayments, value); }
        }

        private PartialPayment partialPayment;

        public PartialPayment PartialPayment
        {
            get { return partialPayment; }
            set
            {
                SetProperty(ref partialPayment, value);

            }
        }

        private float _WithOutGst;

        public float WithOutGst
        {
            get { return _WithOutGst; }
            set { SetProperty(ref _WithOutGst, value); }
        }

        private float _gstAmount;

        public float GSTAmount
        {
            get { return _gstAmount; }
            set { SetProperty(ref _gstAmount, value); }
        }

        private float _netPayableWithOutGst;

        public float NetPayableWithOutGst
        {
            get { return _netPayableWithOutGst; }
            set { SetProperty(ref _netPayableWithOutGst, value); }
        }

        private bool _isInclusive;

        public bool IsInClusive
        {
            get { return _isInclusive; }
            set { SetProperty(ref _isInclusive, value); }
        }

        public List<IndentInventoryDetails> InventoryDetails { get; set; }

        public IndentInventoryDetails InventoryDetail { get; set; }

        public InvoiceAgainstModel InvoiceAgainst { get; set; }

        private string _againstType;
        public string AgainstType
        {
            get { return _againstType; }

            set { SetProperty(ref _againstType, value); }
        }

        public bool IsAgainstType { get; set; }

        private float _tdsDeductAmount;

        public float TdsDeductAmount
        {
            get => _tdsDeductAmount; 
            set => SetProperty(ref _tdsDeductAmount , value);
        }

        private bool _isTDSApplicable;

        public bool IsTDSApplicable
        {
            get => _isTDSApplicable;
            set => SetProperty(ref _isTDSApplicable, value);
        }

        private double _tdsDeduct;

        public double TdsDeduct
        {
            get =>  _tdsDeduct; 
            set => SetProperty( ref _tdsDeduct, value); 
        }

        private double _tdsLimit;

        public double TdsLimit
        {
            get => _tdsLimit;
            set => SetProperty(ref _tdsLimit, value);
        }
        private double _purchaseAmount;

        public double PurchaseAmount
        {
            get => _purchaseAmount;
            set => SetProperty(ref _purchaseAmount, value);
        }

        private double _creditNoteAmount;

        public double CreditNoteAmount
        {
            get => _creditNoteAmount;
            set => SetProperty(ref _creditNoteAmount, value);
        }

        private bool isEnableTransportPayer;

        public bool IsEnableTransportPayer
        {
            get { return isEnableTransportPayer; }
            set { SetProperty(ref isEnableTransportPayer , value); }
        }

    }

    public enum IndentStatus
    {
        created = 0,
        review = 1,
        approve = 2,
        addsupplier = 3,
        placed = 4,
        intransit = 5,
        received = 6,
        fullpaid = 7,
        closed = 8,
        sahayapending = 9,
        cancelled = 10
    }

    public class SupplierDetails : BindableBase
    {

        public int SupplierId { get; set; }

        private string _suppliername;
        public string SupplierName { get { return _suppliername; } set { SetProperty(ref _suppliername, value); } }

        private DateTime? _arrivingdate;
        public DateTime? ArrivingDate { get { return _arrivingdate; } set { SetProperty(ref _arrivingdate, value); } }

        private String _creditperiod;
        public String CreditPeriod { get { return _creditperiod; } set { SetProperty(ref _creditperiod, value); } }

        private string _trackingid;
        public string TrackingId { get { return _trackingid != null ? _trackingid : "Yet to add"; } set { SetProperty(ref _trackingid, value); } }

        private string _status;
        public string Status { get { return _status; } set { SetProperty(ref _status, value); } }

        private string date;
        public string ArriDate { get { return date = ArrivingDate?.ToString("dd-MM-yyy") ?? "Yet to add"; } set { SetProperty(ref date, value); } }


        private string _gst;

        public string GST
        {
            get { return _gst; }
            set { SetProperty(ref _gst, value); }
        }

        private string _category;

        public string Category
        {
            get { return _category; }
            set { SetProperty(ref _category, value); }
        }


        private string _district;

        public string District
        {
            get { return _district; }
            set { SetProperty(ref _district, value); }
        }

        private string _state;

        public string State
        {
            get { return _state; }
            set { SetProperty(ref _state, value); }
        }


        public DateTime? CreateDatetime { get; set; }

    }



    public class IndentInventoryDetails : BindableBase
    {
        private String _invoiceno;
        private DateTime? _invoicedate;
        private DateTime? _posentrydate;
        private int _qty;
        private String _status;
        public String InvoiceNo
        {
            get { return _invoiceno; }
            set { SetProperty(ref _invoiceno, value); }
        }

        public DateTime? InvoiceDate
        {
            get { return _invoicedate; }
            set { SetProperty(ref _invoicedate, value); }
        }

        public DateTime? POSEntryDate
        {
            get { return _posentrydate; }
            set { SetProperty(ref _posentrydate, value); }
        }

        public int Qty
        {
            get { return _qty; }
            set { SetProperty(ref _qty, value); }
        }
        public String Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }


        public int SubQty { get; set; }

        private int _fileID;

        public int FileID
        {
            get { return _fileID; }
            set { SetProperty(ref _fileID, value); }
        }


        public bool IsInventoryDownloadBtnEnabled
        {
            get; set;
        }

        public string DcNo
        {
            get; set;
        }

        public DateTime? DcDate { get; set; }

        public string PictureName { get; set; }

        public float TotalAmount { get; set; }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        private bool _isPaid;
        public bool IsPaid
        {
            get { return _isPaid; }
            set { SetProperty(ref _isPaid, value); }

        }
        public String GRNNo { get; set; }

    }

    public class GSTslabs : BindableBase
    {
        private string _name;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                SetProperty(ref _name, value);
            }
        }

        private float _gstValue;

        public float GstValue
        {
            get => _gstValue;
            set
            {
                SetProperty(ref _gstValue, value);
                Name = $"{value}%";
            }
        }

        private string _gstType;
        public string GstType
        {
            get { return _gstType; }
            set { SetProperty(ref _gstType, value); }
        }
    }

    public class IndentSignalrViewModel
    {
        public int Id { get; set; }
        public string PoNumber { get; set; }

        public string Date { get; set; }

        public string Status { get; set; }
        //public string Remark { get; set; }

        public string StoreName { get; set; }
        public string Type { get; set; }

        //public string ApprovedBy { get; set; }

        public string CreatedBy { get; set; }
        public IEnumerable<IndentViewProduct> Products { get; set; }

        public List<ActivityLogs> AcivityLogs { get; set; }

        // public IndentStatus Indentstatus { get; set; }

        //public int SupplierId { get; set; }

        public string SupplierName { get; set; }
        public DateTime? ArrivingDate { get; set; }

        public string CreditPeriod { get; set; }

        public string TrackingId { get; set; }


        // public DateTime? CreateDatetime { get; set; }

        public SupplierDetails SupplierDetails { get; set; }


        public List<IndentInventoryDetails> IndentInventoryDetails { get; set; }

        //public DateTime? CreateDate { get; set; }

        //public AddressViewModel SelecedSupplierAddress { get; set; }


        //public BankDetails SelectedBankDetail { get; set; }

        //public string PaymentDate
        //{
        //    get; set;
        //}


        //public string RefrenceNo
        //{
        //    get; set;
        //}

        public bool IsFullPaid { get; set; }


        public float AdvisoryCharges
        {
            get; set;
        }

        public FileAttachment FileAttachments { get; set; }

        //public List<StoreReturnModel> SelectedReturnModels
        //{
        //    get; set;
        //}

        public List<PartialPayment> PaymentsList { get; set; }


    }

    public class BulkPaymentDownloadModel : BindableBase
    {
        private bool _isselected;

        public bool IsSelected
        {
            get { return _isselected; }
            set { SetProperty(ref _isselected, value); }
        }

        public int SupplierId { get; set; }

        public string SupplierName { get; set; }

        public float CNAmount { get; set; }

        private List<BankDetails> _bankDetails;

        public List<BankDetails> BankDetails
        {
            get { return _bankDetails; }
            set { SetProperty(ref _bankDetails, value); }
        }

        private BankDetails _selectedbankDetail = new Entites.Suppliers.BankDetails();

        public BankDetails SelectedBankDetail
        {
            get { return _selectedbankDetail; }
            set { SetProperty(ref _selectedbankDetail, value); }
        }

        private List<IndentDownloadModel> _indentLists;

        public List<IndentDownloadModel> IndentLists
        {
            get { return _indentLists; }
            set { SetProperty(ref _indentLists, value); }
        }

        public string SupplierPhone { get; set; }

        public List<StoreReturnModel> ReturnModels { get; set; }

        private bool _isSelectedBank;

        public bool IsSelectedBank
        {
            get { return _isSelectedBank; }
            set { SetProperty(ref _isSelectedBank, value); }
        }



    }


    public class PartialPayment : BindableBase
    {
        public PartialPayment()
        {
            PercentageList = new List<int>()
            {
                10,20,30,40,50,60,70,80,90
            };
        }

        private float _price;
        public float Price
        {
            get { return _price; }
            set { SetProperty(ref _price, value); }
        }

        private string _paymentdate;

        public string PaymentDate
        {
            get { return _paymentdate; }
            set { SetProperty(ref _paymentdate, value); }
        }


        private int? _percent;
        public int? Percentage { get { return _percent; } set { SetProperty(ref _percent, value); } }


        private float _paymentTotal;

        public float PaymentTotal
        {
            get { return _paymentTotal; }
            set
            {
                SetProperty(ref _paymentTotal, value);

            }
        }

        private List<int> _percentageList;

        public List<int> PercentageList
        {
            get { return _percentageList; }
            set { SetProperty(ref _percentageList, value); }
        }

        public string PayrefNo { get; set; }

        public string PaidDate { get; set; }

        private float _netPaidAmount;

        public float NetPaidAmount
        {
            get { return _netPaidAmount = (float)(PaymentTotal - (SelectedReturnModels != null ? SelectedReturnModels.Sum(x => x.Total) : 0)); }
            set
            {

                SetProperty(ref _netPaidAmount, value);

            }
        }


        private List<StoreReturnModel> _selectedreturnModels;
        public List<StoreReturnModel> SelectedReturnModels
        {
            get { return _selectedreturnModels; }

            set { SetProperty(ref _selectedreturnModels, value); }

        }

        private float _tdsAmount;

        public float TdsAmount
        {
            get => _tdsAmount;
            set => SetProperty(ref _tdsAmount , value);
        }



    }

    public class BulkDownloadExport
    {
        [DisplayName("CUSTOMER_CODE")]
        public string CustomerCode { get; set; }

        [DisplayName("CUSTOMER_NAME")]
        public string CustomerName { get; set; }

        [DisplayName("DEBIT_ACCOUNT_NO")]
        public string DebitAccountNo { get; set; }

        [DisplayName("PRODUCT_CODE")]
        public string ProductCode { get; set; }

        [DisplayName("GrossTotal")]
        public float GrossTotal { get; set; }

        [DisplayName("TDS")]
        public float TDS { get; set; }

        [DisplayName("AMOUNT")]
        public float Amount { get; set; }

        [DisplayName("BENEFICIARY_NAME")]
        public string BeneficiaryName { get; set; }

        [DisplayName("BENEFICIARY_BRANCH_CODE")]
        public string BeneficiaryBranchCode { get; set; }

        [DisplayName("CREDIT_ACCOUT_NO")]
        public string CreditAccountNo { get; set; }

        [DisplayName("EMAIL_ID")]
        public string EmailId { get; set; }

        public string StoreName { get; set; }

        [DisplayName("PHONE")]
        public string Phone { get; set; }

        [DisplayName("PAYMENT_REF_NO")]
        public string PaymentRefNo { get; set; }

        public string UTR { get; set; }

        public string PaymentDate { get; set; }

        public string InvoiceNo { get; set; }

    }

    public class BulkPaymentUpdateModel
    {
        public string PoNumber { get; set; }

        public float PayAmount { get; set; }
        public string PaymentDate { get; set; }

        public string UTR { get; set; }
        public BankDetails BankDetails { get; set; }



    }

    public class InvoiceAgainstModel : BindableBase
    {
        private float _totalInvoiceAmount;
        public float TotalInvoiceAmount
        {
            get { return _totalInvoiceAmount; }
            set { SetProperty(ref _totalInvoiceAmount, value); }
        }

        private float _totalPartiallyPaid;
        public float TotalPartiallyPaid
        {
            get { return _totalPartiallyPaid; }
            set { SetProperty(ref _totalPartiallyPaid, value); }
        }

        private float _totalTdsDeducted;
        public float TotalTdsDeducted
        {
            get { return _totalTdsDeducted; }
            set { SetProperty(ref _totalTdsDeducted, value); }
        }

        private float _remainTdsDeduct;
        public float RemainTdsDeduct
        {
            get { return _remainTdsDeduct; }
            set { SetProperty(ref _remainTdsDeduct, value); }
        }

        private float _balancePay;
        public float BalancePay
        {
            get { return _balancePay; }
            set { SetProperty(ref _balancePay, value); }
        }
    }



    public class UpdateAdjustmentModel
    {
        public int SupplierId { get; set; }

        public float PayableAmount { get; set; }
        public List<SelectedRetunes> SelectedRetunes { get; set; }
    }

    public class SelectedRetunes
    {
        public int Id { get; set; }
        public string CreditNoteDate { get; set; }

        public string CreditNoteNumber { get; set; }
        public Decimal RedeemedAmount { get; set; }

    }

    public class PendingPaymentSupplier : BindableBase
    {
        public int SupplierId { get; set; }

        public string SupplierName { get; set; }

        public string SupplierType { get; set; }

        private bool _isEnable;
        public bool IsEnable
        {
            get => _isEnable;
            set => SetProperty(ref _isEnable, value);
        }
    }

    public class AddSupplierModel : BindableBase
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }



        private List<AddSupplierToIndentProduct> _product;
        public List<AddSupplierToIndentProduct> Products
        {
            get { return _product; }
            set { SetProperty(ref _product, value); }
        }



        public int SupplierId { get; set; }


        //private DateTime? _arrivingdate;
        //public DateTime? ArrivingDate
        //{
        //    get { return _arrivingdate; }
        //    set { SetProperty(ref _arrivingdate, value); }
        //}

        private String _creditperiod;
        public String CreditPeriod
        {
            get { return _creditperiod; }
            set { SetProperty(ref _creditperiod, value); }
        }

        private string _gstType;
        public string GSTType
        {
            get { return _gstType; }
            set { SetProperty(ref _gstType, value); }
        }





        private ObservableCollection<FileUploadInfo> fileUploadListInfo;
        public ObservableCollection<FileUploadInfo> FileUploadListInfo
        {
            get { return fileUploadListInfo; }
            set { SetProperty(ref fileUploadListInfo, value); }


        }

        private float _advisoryCharges;

        public float AdvisoryCharges
        {
            get { return _advisoryCharges; }
            set { SetProperty(ref _advisoryCharges, value); }
        }

        public bool IsFullPaid { get; set; }

        private List<AddPartialPayment> _paymentsList;
        public List<AddPartialPayment> PaymentsList
        {
            get { return _paymentsList; }
            set { SetProperty(ref _paymentsList, value); }
            
        }

        private List<DiscountModel> _discounts;

        public List<DiscountModel> Discounts
        {
            get { return _discounts; }
            set { SetProperty( ref _discounts , value); }
        }
        public string TC { get; set; }

        public string TransportChargesPayer { get; set; }


    }

    public class AddSupplierToIndentProduct
    {
        public int ProductId { get; set; }

        public decimal EstimatedPrice { get; set; }
        public int AvailableQty { get; set; }

        public GSTslabs SelectedGSTslab { get; set; }

        public string ETA { get; set; }

    }

    public class AddPartialPayment
    {
        public string PaymentDate { get; set; }
        public float PaymentTotal { get; set; }
        public float TdsAmount {  get; set; }
    }

    public class ICICExport
    {

        [DisplayName("Debit Ac No")]
        public string DebitAccNo { get; set; }

        [DisplayName("Beneficiary Ac No")]
        public string BeneficiaryAccNo { get; set; }

        [DisplayName("Beneficiary Name")]
        public string BeneficiaryName { get; set; }

        [DisplayName("GrossTotal")]
        public float GrossTotal { get; set; }
        [DisplayName("TDS")]
        public float TDS { get; set; }
        [DisplayName("Amt")]
        public float Amount { get; set; }

        [DisplayName("Pay Mode")]
        public string PaymentMode { get; set; }

        [DisplayName("Date(DD-MON-YYYY)")]
        public string Date { get; set; }

        public string IFSC { get; set; }

        [DisplayName("Pay Location")]
        public string PayLocation { get; set; }

        [DisplayName("Print Location")]
        public string PrintLocation { get; set; }

        [DisplayName("Bene Mobile no")]
        public string BeneMobileNo { get; set; }

        [DisplayName("Bene email id")]
        public string BeneEmailId { get; set; }

        [DisplayName("Bene add1")]
        public string BeneAdd1 { get; set; }

        [DisplayName("Bene add2")]
        public string BeneAdd2 { get; set; }

        [DisplayName("Bene add3")]
        public string BeneAdd3 { get; set; }

        [DisplayName("Bene add4")]
        public string BeneAdd4 { get; set; }

        [DisplayName("Add details 1")]
        public string AddDetails1 { get; set; }

        [DisplayName("Add details 2")]
        public string AddDetails2 { get; set; }

        [DisplayName("Add details 3")]
        public string AddDetails3 { get; set; }

        [DisplayName("Add details 4")]
        public string AddDetails4 { get; set; }

        [DisplayName("Add details 5")]
        public string AddDetails5 { get; set; }
        public string Remarks { get; set; }
        public string UTR { get; set; }

        public string InvoiceNo { get; set; }

    }

    public class IndentReviewProduct
    {
        public int ProductId { get; set; }

        public int Qty { get; set; }

        public int TmQty { get; set; }


    }

    public class IndentApproveProduct
    {
        public int ProductId { get; set; }

        public int Qty { get; set; }

        public int RmQty { get; set; }

        public decimal Rsp { get; set; }


    }

    public class InvoiceDetailsModel
    {
        public string InvoiceNo { get; set; }

        public int InvoiceQty { get; set; }

        public float InvoiceTotal { get; set; }
    }

    public class IndentDownloadModel:BindableBase
    {
        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        private int _id;
        public int Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private string _poNumber;
        public string PoNumber
        {
            get { return _poNumber; }
            set { SetProperty(ref _poNumber, value); }
        }

        private string _date;
        public string Date
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
        }

       

        private string _storeName;
        public string StoreName
        {
            get { return _storeName; }
            set { SetProperty(ref _storeName, value); }
        }

       
        private IEnumerable<IndentViewProduct> _product;
        public IEnumerable<IndentViewProduct> Products
        {
            get { return _product; }
            set { SetProperty(ref _product, value); }
        }


        public int SupplierId { get; set; }

        private string _supplierName;
        public string SupplierName { get { return _supplierName; } set { SetProperty(ref _supplierName, value); } }


        private float _totalAmount;

        public float TotalAmount
        {
            get { return _totalAmount; }
            set { SetProperty(ref _totalAmount, value); }
        }

        private float _advisoryCharges;

        public float AdvisoryCharges
        {
            get { return _advisoryCharges; }
            set { SetProperty(ref _advisoryCharges, value); }
        }

        private float _payableAmount;

        public float PayableAmount
        {
            get { return _payableAmount; }
            set { SetProperty(ref _payableAmount, value); }
        }
        private string _paymentDate;

        public string PaymentDate
        {
            get { return _paymentDate; }
            set { SetProperty(ref _paymentDate, value); }
        }

        private float _totalEstimatedAmount;
        public float TotalEstimantedAmount
        {
            get { return _totalEstimatedAmount; }

            set { SetProperty(ref _totalEstimatedAmount, value); }
        }

        // Stores the status of the PO.
        private string _status;

        public string Status
        {
            get { return _status; }

            set { SetProperty(ref _status, value); }
        }


        private List<StoreReturnModel> _selectedreturnModels;
        public List<StoreReturnModel> SelectedReturnModels
        {
            get { return _selectedreturnModels; }

            set { SetProperty(ref _selectedreturnModels, value); }

        }

        private float _finalpayableAmount;

        public float FinalPayableAmount
        {
            get { return _finalpayableAmount; }
            set { SetProperty(ref _finalpayableAmount, value); }
        }

        public string CreatedBy { get; set; }


        public float CNAmount { get; set; }

      
        public string InvoiceNo { get; set; }

        private float _tds;

        public float TDS
        {
            get { return _tds; }
            set { SetProperty(ref _tds, value); }
        }
        public List<InvoiceDetailsModel> invoiceDetails { get; set; }

        private int _poQty;

        public int PoQty
        {
            get { return _poQty; } 
            set { SetProperty(ref _poQty , value); }
        }

        

    }
 public class DiscountModel
 {
    public string Name { get; set; }
    public float Value { get; set; }
    public int Percentage { get; set; }
 }

}

