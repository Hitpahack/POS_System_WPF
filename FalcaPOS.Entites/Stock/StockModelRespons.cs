using FalcaPOS.Entites.EwayBill;
using FalcaPOS.Entites.Manufacturers;
using FalcaPOS.Entites.ProductTypes;
using FalcaPOS.Entites.Sales;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;

namespace FalcaPOS.Entites.Stock
{
    public class StockModelRespons
    {
        public string SupplierName { get; set; }
        public string Producttype { get; set; }
        public string ProductName { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public string Serialno { get; set; }
        public bool? Qadone { get; set; }
        public int? Qty { get; set; }
        public DateTime? Dateofmanufacturing { get; set; }
        public DateTime? Expirydate { get; set; }
        public string WarantyService { get; set; }
        public string Location { get; set; }
        public string Barcode { get; set; }
        public string Status { get; set; }

        public double? Rate { get; set; }
        public string Discountmode { get; set; }
        public double? Discount { get; set; }
        public double? Total { get; set; }
        public double? Sellingprice { get; set; }
        public string InvocieNo { get; set; }
        public DateTime InvocieDate { get; set; }
        public string Name { get; set; }

        public string Value { get; set; }


    }

    public class StoreStockModelResponse : BindableBase
    {

        private bool _isSelectedProduct;
        public bool IsSelectedProduct { get { return _isSelectedProduct; } set { SetProperty(ref _isSelectedProduct, value); } }
        public long productid { get; set; }
        public string Producttype { get; set; }
        public string Category { get; set; }
        public string ProductName { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public string Serialno { get; set; }

        public string Expirydate { get; set; }
        public string WarantyService { get; set; }
        public string Location { get; set; }
        public string Barcode { get; set; }
        public string Status { get; set; }

        public long AvailableUnits { get; set; }

        public long SoldQty { get; set; }
        public bool HasInformation { get; set; }
        public double? Sellingprice { get; set; }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _value;
        public string Value { get { return _value; } set { SetProperty(ref _value, value); } }

        //private List<StockRowDetail> _stockRowDetails;


        //hsn and lot number

        public string HSN { get; set; }

        public string LotNumber { get; set; }

        private StockProductInformation _stockProductInformation;
        public StockProductInformation StockProductInformation
        {
            get { return _stockProductInformation; }

            set { SetProperty(ref _stockProductInformation, value); }
        }



        public bool IsSelectedShallow { get; set; }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, IsSelectedShallow); }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        private bool _isSellingPriceUpdated;

        public bool IsSellingPriceUpdated
        {
            get { return _isSellingPriceUpdated; }
            set { SetProperty(ref _isSellingPriceUpdated, value); }
        }


        //product sku number

        public string ProductSKU { get; set; }

        public string Departcode { get; set; }

        //public List<StockRowDetail> stockRowDetails
        //{
        //    get
        //    {
        //        return _stockRowDetails;
        //    }
        //    set
        //    {
        //        SetProperty(ref _stockRowDetails, value);
        //    }
        //}
        public String Store { get; set; }

        public long StoreId { get; set; }

        public long? ParentRef { get; set; }
    }

    //public class StockRowDetail:BindableBase
    //{

    //    private string _name;
    //    public string Name
    //    {
    //        get { return _name; }
    //        set { SetProperty(ref _name, value); }
    //    }

    //    private string _value;
    //    public string Value { get { return _value; } 
    //        set { SetProperty(ref _value, value); } }



    //}

    public class StockProductInformation
    {
        public int StockProductId { get; set; }

        public ProductType ProductType { get; set; }

        public Manufacturer Manufacturer { get; set; }


        public string ProductName { get; set; }

        public DateTime? WarrentyDate { get; set; }



        public string DiscountMode { get; set; }

        public float Discount { get; set; }

        public float ProductGST { get; set; }


        public float ProductSellingPrice { get; set; }


        public float ProductTotal { get; set; }


        public float ProductDiscountPercent { get; set; }

        public float ProductDiscountFlat { get; set; }



        public List<SalesProductSpec> ProductSpecifications { get; set; }
    }

    public class StockModelRequest : BindableBase
    {

        public string _suppliername;
        public string SupplierName
        {
            get { return _suppliername; }
            set { SetProperty(ref _suppliername, value); }
        }

        public string _category;
        public string Category
        {
            get { return _category; }
            set { SetProperty(ref _category, value); }
        }

        public string _productType;
        public string ProductType
        {
            get { return _productType; }
            set { SetProperty(ref _productType, value); }
        }
        public string _brand;
        public string Brand
        {
            get { return _brand; }
            set { SetProperty(ref _brand, value); }
        }
        private string _productName;
        public string ProductName
        {
            get { return _productName; }
            set { SetProperty(ref _productName, value); }

        }

        private string _location;
        public string Location
        {
            get { return _location; }
            set { SetProperty(ref _location, value); }
        }

        private string _status;
        public string Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }

        private string _serialno;

        public string SerialNo
        {
            get { return _serialno; }
            set { SetProperty(ref _serialno, value); }
        }

        private string _invocieno;
        public string InvoiceNo
        {
            get { return _invocieno; }
            set { SetProperty(ref _invocieno, value); }
        }
        private string _invoiceFromDate;
        public string InvoiceFromDate
        {
            get { return _invoiceFromDate; }
            set { SetProperty(ref _invoiceFromDate, value); }
        }
        private string _invoiceToDate;
        public string InvoiceToDate
        {
            get { return _invoiceToDate; }
            set { SetProperty(ref _invoiceToDate, value); }
        }


        public string StoreId { get; set; }

        private bool _isParent;
        public bool IsParent
        {
            get { return _isParent; }
            set { SetProperty(ref _isParent, value); }
        }





    }

    public class StockTrnasferModel : BindableBase
    {

        public string TransferOrderNo { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
        public string Date { get; set; }

        public List<StockTransferProduct> StockTransferList { get; set; }
        public float TransportCharges { get; set; }
        public float Others { get; set; }

        public int? FileID { get; set; }

        private string _stReceiptNumber;
        public string STReceiptNumber
        {
            get => _stReceiptNumber;
            set => SetProperty(ref _stReceiptNumber, value);
        }


        public string STReceiptDate { get; set; }

        private List<StockTransferProduct> _stockTransferProducts;
        public List<StockTransferProduct> StockTransferProducts
        {
            get => _stockTransferProducts;

            set => SetProperty(ref _stockTransferProducts, value);
        }


        private string _productcode;
        public string ProductCode
        {
            get => _productcode;
            set => SetProperty(ref _productcode, value);
        }

        private bool _isexpand;

        public bool IsExpand
        {
            get => _isexpand;
            set => SetProperty(ref _isexpand, value);
        }

        public string DateHumnaizer { get; set; }
        public int? ToParent_ref { get; set; }

        public int TransferId { get; set; }

        public int FromStoreId { get; set; }

        public int ToStoreId { get; set; }

        public string STNumber { get; set; }

        public string STDate { get; set; }

        public string SRNumber { get; set; }

        public string SRDate { get; set; }



    }

    public class StockTransferProduct : BindableBase
    {

        public long ProductId { get; set; }
        public string Department { get; set; }
        public string Brand { get; set; }
        public string ProductName { get; set; }
        public string ProductSKU { get; set; }

        public int TransferQty { get; set; }

        public int StockQty { get; set; }
        public float SellingPrice { get; set; }

        public string BarCode { get; set; }

        public int StockProuductId { get; set; }

        private float _newSellingPrice;
        public float NewSellingPrice
        {
            get { return _newSellingPrice; }
            set { SetProperty(ref _newSellingPrice, value); }
        }

        public float Mrp { get; set; }
        public string HsnCode { get; set; }

        public string Description { get; set; }
    }


    public class StockFlyout
    {
        public bool IsOpen { get; set; }

        public bool IsParent { get; set; }
    }

    public class StockSearchModel
    {
        public int? Category { get; set; }
        public int? ProductType { get; set; }
        public int? ProductName { get; set; }
        public int? Brand { get; set; }
        public string Status { get; set; }
        public string SerialNo { get; set; }
        public string InvoiceFromDate { get; set; }
        public string InvoiceToDate { get; set; }

        public string StoreId { get; set; }
        public bool IsParent { get; set; }

    }

    public class StockTranferRequestModel
    {
        public string TransferOrderNo { get; set; }
        public string Date { get; set; }
        public int FromStoreId { get; set; }
        public List<StockTransferProductModel> stockTransferProducts { get; set; }
        public int ToStoreId { get; set; }


    }


    public class StockTransferProductModel
    {
        public int ProductId { get; set; }

        public int TransferQty { get; set; }


    }


    public class TransferViewModel : BindableBase
    {
        private string _STNumner;
        public string STNumber
        {
            get => _STNumner;
            set => SetProperty(ref _STNumner, value);
        }

        public string STDate { get; set; }

        public string SRNumber { get; set; }

        public string SRDate { get; set; }

        public int? ToParent_ref { get; set; }

        public string FromLocation { get; set; }

        public string ToLocation { get; set; }

        public int FromStoreId { get; set; }

        public int ToStoreId { get; set; }

        public int TransferId { get; set; }

        public string DateHumnaizer { get; set; }

        public string ProductCode { get; set; }

        private List<StockTransferProductViewModel> _stockTransferProducts;
        public List<StockTransferProductViewModel> stockTransferProducts
        {
            get => _stockTransferProducts;
            set => SetProperty(ref _stockTransferProducts, value);
        }

        public List<StockTransferProduct> StockTransferList { get; set; }



        public string FromAddress { get; set; }

        public int FromPinCode { get; set; }

        public string FromState { get; set; }

        public string FromCity { get; set; }

        public string ToAddress { get; set; }

        public int ToPinCode { get; set; }

        public string ToState { get; set; }

        public string ToCity { get; set; }

        public string FromFalcaGstIn { get; set; }

        public string ToFalcaGstIn { get; set; }

        private bool _isWayBillGenerated;

        public bool IsWayBillGenerated
        {
            get { return _isWayBillGenerated; }
            set { SetProperty(ref _isWayBillGenerated, value); }
        }

        public string EwayBillUrl { get; set; }
        public StockTransferTranportDetails TranportDetails { get; set; }

    }

    public class StockTransferProductViewModel
    {
        public long ProductId { get; set; }
        public string Department { get; set; }
        public string Brand { get; set; }
        public string ProductName { get; set; }
        public string ProductSKU { get; set; }

        public int TransferQty { get; set; }

        public int St_ref { get; set; }

        public int StockQty { get; set; }
        public float SellingPrice { get; set; }

        public string BarCode { get; set; }

        public int StockProuductId { get; set; }

        public string HsnCode { get; set; }

        public float Rate { get; set; }

        public float Gst { get; set; }

        public float CGST { get; set; }

        public float SGST { get; set; }

        public float IGST { get; set; }
        public string Description { get; set; }
    }

    public class StockTransferDTO
    {

        public string STNumber { get; set; }

        public List<StockTransferProductDTO> StockTransferList { get; set; }
        public string SRNumber { get; set; }
        public string STDate { get; set; }


        public int? ToParent_ref { get; set; }

    }

    public class StockTransferProductDTO
    {

        public long ProductId { get; set; }

        public int TransferQty { get; set; }

        public int StockProuductId { get; set; }

    }

    public class ReceiverViewModel
    {
        public string SRNumber { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }

        public string SRDate { get; set; }
        public List<ReciverProductViewModel> StockTransferList { get; set; }

        public int? ToParent_ref { get; set; }

        public int TransferId { get; set; }

        public float TransportCharges { get; set; }

        public List<string> TransportChargesPayers { get; set; }

        public string TransportChargesPayer { get; set; }
        public float Others { get; set; }

        public int? FileID { get; set; }
        public string EwayBill { get; set; }

    }

    public class ReciverProductViewModel : BindableBase
    {

        public long ProductId { get; set; }
        public string Department { get; set; }
        public string Brand { get; set; }
        public string ProductName { get; set; }
        public string ProductSKU { get; set; }

        public int TransferQty { get; set; }

        public float SellingPrice { get; set; }
        public int StockProuductId { get; set; }

        public float Mrp { get; set; }

        private float _newSellingPrice;
        public float NewSellingPrice
        {
            get { return _newSellingPrice; }
            set { SetProperty(ref _newSellingPrice, value); }
        }

        public string CategoryName { get; set; }
        private bool _isWayBillGenerated;

        public bool IsWayBillGenerated
        {
            get { return _isWayBillGenerated; }
            set { SetProperty(ref _isWayBillGenerated, value); }
        }

        public string EwayBillUrl { get; set; }

    }

    public class ReceiverUpdateModel
    {

        public int TransferId { get; set; }
        public float TransportCharges { get; set; }
        public float Others { get; set; }
        public string TransportChargesPayer { get; set; }

        public List<ReceiverUpdateProduct> ReceiverUpdateProduct { get; set; }

    }

    public class ReceiverUpdateProduct
    {
        public int StockProuductId { get; set; }

        public float NewSellingPrice { get; set; }

        public long ProductId { get; set; }

    }

    public class TransferCompletedViewModel:BindableBase
    {
        public int StockTransferId { get; set; }
        public string STNumber { get; set; }

        public string STDate { get; set; }

        public string SRNumber { get; set; }

        public string SRDate { get; set; }

        public int? ToParent_ref { get; set; }

        public string FromLocation { get; set; }

        public string ToLocation { get; set; }

        public float TransportCharges { get; set; }

        public string TransportChargesPayer { get; set; }


        public bool IsButtonVisible
        {
            get {
                return TransportCharges==0 && Others==0 ? false : true;
            }

        }

        public float Others { get; set; }

        public int? FileID { get; set; }

        public List<TransferProductCompletedViewModel> transferProducts { get; set; }

        private string _status;
        public string Status
        {
            get { return _status; }
            set {
                _status = value;
                if(_status!=null)
                    IsColumnVisible =( _status.ToLower() == "cancelled" || STNumber==null )? false : true;
            }

        }

        private bool _isColumnVisible;

        public bool IsColumnVisible
        {
            get { return _isColumnVisible; }
            set {
                SetProperty(ref _isColumnVisible , value);

            }
        }
        private bool _isWayBillGenerated;

        public bool IsWayBillGenerated
        {
            get { return _isWayBillGenerated; }
            set { SetProperty(ref _isWayBillGenerated, value); }
        }

        public string EwayBillUrl { get; set; }
        public string Remarks { get; set; }
        public StockTransferTranportDetails EWayBillModel { get; set; }
    }


    public class TransferProductCompletedViewModel:BindableBase
    {

        public long ProductId { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Brand { get; set; }
        public string ProductName { get; set; }
        public string ProductSKU { get; set; }
        public string OldSKU { get; set; }

        public string HSNCode { get; set; }
        public int AvailableQty { get; set; }

        private int _transferQty;
        public int TransferQty
        {
            get { return _transferQty; }

            set { SetProperty(ref _transferQty,value); }

        }
        public float SellingPrice { get; set; }
        public float Rate { get; set; }
        public int GST { get; set; }
        public int StockProuductId { get; set; }

        private bool _isReadOnly;
        public bool IsReadonlyQty
        {
            get { return _isReadOnly; }

            set { SetProperty( ref _isReadOnly,value); }
        }

        public string Remarks { get; set; }

    }

    public class StockTransferTranportDetails
    {
        public string DocumentName { get; set; }
        public int? Id { get; set; }
        public string EwaybillNo { get; set; }
        public string Url { get; set; }
        public string DocumentNo { get; set; }
        public  DateTime? DocumentDate { get; set; }
        public string VehicleNo { get; set; }
        public string VehicleType { get; set; }
    }

}

