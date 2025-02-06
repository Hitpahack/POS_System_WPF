using FalcaPOS.Entites.Attributes;
using FalcaPOS.Entites.Common;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FalcaPOS.Entites.AddInventory
{
    public class StockProductViewModel : BindableBase
    {


        public string InvoiceNo { get; set; }

        public string SalesInvoice { get; set; }
        public string DeptCode { get; set; }

        public string ProductSKU { get; set; }

        public int ProductTypeId { get; set; }

        public string ProductTypeName { get; set; }


        public int ManufacturerId { get; set; }

        public string ManufactureName { get; set; }



        public int ProductId { get; set; }

        public string ProductName { get; set; }


        public int? HSNCode { get; set; }
        public DateTime DateOfManufacture { get; set; }


        public DateTime? DateOfExpiry { get; set; }

        public string Location { get; set; }

        public string WarrantyService { get; set; }



        public int Quantity { get; set; }

        public int DefectiveQuantity { get; set; }

        public float ProductRate { get; set; }

        public float LandingRate { get; set; }

        public float ProductDiscount { get; set; }

        private float _productTotal;
        public float ProductTotal
        {
            get { return _productTotal; }
            set
            {
                SetProperty(ref _productTotal, value);

            }
        }

        public float ProductSellingPrice { get; set; }

        public string ProductDiscountMode { get; set; }

        public bool IsSerialNumberManual { get; set; }

        public List<string> SerialNumbers { get; set; }

        public int? StroreId { get; set; }

        public string StoreName { get; set; }

        public string ProductUniqGuid { get; set; }

        public float ProductMRP { get; set; }

        public float ProductGST { get; set; }

        public List<ProductAttributeMapping> AttributesSelectedList { get; set; }
        public double Misc { get; set; }

        public string BaseunitType { get; set; }

        public string SubunitType { get; set; }

        private int _productSubQty;
        public int ProductSubQty
        {
            get { return _productSubQty; }
            set { SetProperty(ref _productSubQty, value); }
        }

        public bool IsGroupTrackMode { get; set; }

        public string InventoryTrackMode { get; set; }

        public string Lotnumber { get; set; }
        public List<AttributeMap> DefectiveGroup { get; set; }
        public string HSN { get; set; }
        public int SubDefectiveQty { get; set; }
        public string Status { get; set; }


        public int SalesDefectiveQty { get; set; }


        /// <summary>
        /// Discount applied to product from invoice level.
        /// </summary>
        public float ProductInvoiceDiscount { get; set; }

        /// <summary>
        /// Discount applied rate of the product.
        /// </summary>
        public float ProductDiscountRate { get; set; }

        /// <summary>
        /// GST applied for single product.
        /// </summary>
        public float ProductGSTperQuantity { get; set; }


        public string SupplierName { get; set; }

        private bool _isSeleced;
        public bool IsSelected
        {
            get => _isSeleced;
            set => SetProperty(ref _isSeleced, value);
        }

        public int StockProductId { get; set; }

        private int _returnQty;
        public int ReturnQty
        {
            get { return _returnQty; }

            set
            {
                SetProperty(ref _returnQty, value);
                RowTotal = 0;
                if (_returnQty > 0)
                {
                    RowTotal = (((ProductRate * ProductGST) / 100) * _returnQty) + ((ProductRate * _returnQty));
                    ProductTotal = ((((ProductRate * ProductGST) / 100) + ProductRate) * ReturnQty);
                }

            }
        }

        private float _rowTotal;
        public float RowTotal
        {
            get => _rowTotal;

            set => SetProperty(ref _rowTotal, value);
        }

        public DateTime CreatedDatetime { get; set; }

        public string InvoiceDate { get; set; }

        private bool _isReadOnly = false;
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set => SetProperty(ref _isReadOnly, value);
        }

        public int SupplierId { get; set; }
        public int RemainingStock { get; set; }

        public float RSP { get; set; }

    }


    public class StoreReturnModel : BindableBase
    {
        public int Id { get; set; }

        private bool _isselected;
        public bool IsSelected
        {
            get => _isselected;

            set => SetProperty(ref _isselected, value);
        }


        private string _creditnoteDate;
        public string CreditNoteDate
        {
            get => _creditnoteDate;

            set => SetProperty(ref _creditnoteDate, value);
        }

        private string _creditnoteNumber;
        public string CreditNoteNumber
        {
            get => _creditnoteNumber;

            set => SetProperty(ref _creditnoteNumber, value);
        }



        private float _total;
        public float Total
        {
            get => _total;
            set => SetProperty(ref _total, value);
        }
        public string Remark { get; set; }

        public int SupplierId { get; set; }

        public string Status { get; set; }

        public string SupplierName { get; set; }

        public string StoreName { get; set; }

        private ObservableCollection<StockProductViewModel> _purhcaseReturnProduct;
        public ObservableCollection<StockProductViewModel> PurhcaseReturnProduct
        {
            get => _purhcaseReturnProduct;
            set => SetProperty(ref _purhcaseReturnProduct, value);
        }



        private int _fileID;

        public int FileID
        {
            get => _fileID;
            set => SetProperty(ref _fileID, value);
        }


        private bool _isEnableApprovebtn;

        public bool IsEnableApproveBtn
        {
            get => _isEnableApprovebtn;
            set => SetProperty(ref _isEnableApprovebtn, value);
        }

        private bool isApproved;

        public bool IsApproved
        {
            get => isApproved;
            set => SetProperty(ref isApproved, value);
        }

        public DateTime CreatedDatetime { get; set; }


        private bool _isEnableApproveEditbtn;

        public bool IsEnableApproveEditBtn
        {
            get => _isEnableApproveEditbtn;
            set => SetProperty(ref _isEnableApproveEditbtn, value);
        }

        private bool _isEnableApproveSavebtn;

        public bool IsEnableApproveSaveBtn
        {
            get => _isEnableApproveSavebtn;
            set => SetProperty(ref _isEnableApproveSavebtn, value);
        }

        private bool _isEdited;
        public bool IsEdited
        {
            get => _isEdited;
            set => SetProperty(ref _isEdited, value);
        }

        private ObservableCollection<FileUploadInfo> fileUploadListInfo;
        public ObservableCollection<FileUploadInfo> FileUploadListInfo
        {
            get => fileUploadListInfo;
            set => SetProperty(ref fileUploadListInfo, value);
        }

        private decimal _redeemedamount;

        public decimal RedeemedAmount
        {
            get { return _redeemedamount; }
            set
            {
                SetProperty(ref _redeemedamount, value);
                if (value > (decimal)Total) RedeemedAmount = 0;
                if (value > 0)
                {
                    RedeemedTotal = (float)value;
                    RedeemedTotal = RedeemedTotal < 0 ? Total : RedeemedTotal;
                }

                else
                    RedeemedTotal = Total;

            }
        }

        private float _redeemedTotal;

        public float RedeemedTotal
        {
            get => _redeemedTotal;
            set => SetProperty(ref _redeemedTotal, value);
        }


        private bool _isEnablePostApprovebtn;

        public bool IsEnablePostApproveBtn
        {
            get => _isEnablePostApprovebtn;
            set => SetProperty(ref _isEnablePostApprovebtn, value);
        }

        private bool _isEnableProductEditbtn;

        public bool IsEnableProductEditbtn
        {
            get => _isEnableProductEditbtn;
            set => SetProperty(ref _isEnableProductEditbtn, value);
        }

        private bool _isEnableUpdateBtn;

        public bool IsEnableUpdateBtn
        {
            get => _isEnableUpdateBtn;
            set => SetProperty(ref _isEnableUpdateBtn, value);
        }

        private List<AdjustModel> _adjustmodels;
        public List<AdjustModel> AdjustModels
        {
            get => _adjustmodels;
            set => SetProperty(ref _adjustmodels, value);
        }

        public ObservableCollection<StockProductViewModel> DeleteReturnProduct { get; set; }
    }


    public class AdjustModel
    {
        public string PoNumber { get; set; }
        public float Amount { get; set; }
        public string AjustedDate { get; set; }
    }

    public class EditProductModel : BindableBase
    {


        public string InvoiceNo { get; set; }

        public string SalesInvoice { get; set; }
        public string DeptCode { get; set; }

        public string ProductSKU { get; set; }

        public int ProductTypeId { get; set; }

        public string ProductTypeName { get; set; }


        public int ManufacturerId { get; set; }

        public string ManufactureName { get; set; }



        public int ProductId { get; set; }

        public string ProductName { get; set; }


        public int? HSNCode { get; set; }
        public DateTime DateOfManufacture { get; set; }


        public DateTime? DateOfExpiry { get; set; }

        public string Location { get; set; }

        public string WarrantyService { get; set; }



        public int Quantity { get; set; }

        public int DefectiveQuantity { get; set; }

        public float ProductRate { get; set; }

        public float ProductDiscount { get; set; }

        private float _productTotal;
        public float ProductTotal
        {
            get { return _productTotal; }
            set
            {
                SetProperty(ref _productTotal, value);

            }
        }

        public float ProductSellingPrice { get; set; }

        public string ProductDiscountMode { get; set; }

        public bool IsSerialNumberManual { get; set; }

        public List<string> SerialNumbers { get; set; }

        public int? StroreId { get; set; }

        public string StoreName { get; set; }

        public string ProductUniqGuid { get; set; }

        public float ProductMRP { get; set; }

        public float ProductGST { get; set; }

        public List<ProductAttributeMapping> AttributesSelectedList { get; set; }
        public double Misc { get; set; }

        public string BaseunitType { get; set; }

        public string SubunitType { get; set; }

        private int _productSubQty;
        public int ProductSubQty
        {
            get { return _productSubQty; }
            set { SetProperty(ref _productSubQty, value); }
        }

        public bool IsGroupTrackMode { get; set; }

        public string InventoryTrackMode { get; set; }

        public string Lotnumber { get; set; }
        public List<AttributeMap> DefectiveGroup { get; set; }
        public string HSN { get; set; }
        public int SubDefectiveQty { get; set; }
        public string Status { get; set; }


        public int SalesDefectiveQty { get; set; }


        /// <summary>
        /// Discount applied to product from invoice level.
        /// </summary>
        public float ProductInvoiceDiscount { get; set; }

        /// <summary>
        /// Discount applied rate of the product.
        /// </summary>
        public float ProductDiscountRate { get; set; }

        /// <summary>
        /// GST applied for single product.
        /// </summary>
        public float ProductGSTperQuantity { get; set; }


        public string SupplierName { get; set; }

        private bool _isSeleced;
        public bool IsSelected { get { return _isSeleced; } set { SetProperty(ref _isSeleced, value); } }

        public int StockProductId { get; set; }

        private int _returnQty;
        public int ReturnQty
        {
            get { return _returnQty; }

            set
            {
                SetProperty(ref _returnQty, value);
                RowTotal = 0;
                if (_returnQty > 0)
                {
                    RowTotal = (((ProductRate * ProductGST) / 100) * _returnQty) + ((ProductRate * _returnQty));
                    ProductTotal = ((((ProductRate * ProductGST) / 100) + ProductRate) * ReturnQty);
                }

            }
        }

        private float _rowTotal;
        public float RowTotal
        {
            get => _rowTotal;

            set => SetProperty(ref _rowTotal, value);
        }

        public DateTime CreatedDatetime { get; set; }

        public string InvoiceDate { get; set; }

        private bool _isReadOnly = true;
        public bool IsReadOnly
        {
            get => _isReadOnly;

            set => SetProperty(ref _isReadOnly, value);

        }



    }

    public class CreditNoteSummaryModel : BindableBase
    {
        public string SupplierName { get; set; }

        public string StoreName { get; set; }

        public string CreditNoteNo { get; set; }

        public string CreditNoteDate { get; set; }

        public float TotalAmount { get; set; }

        public float AdjustedAmount { get; set; }

        public float BalanceAmount { get; set; }


        private List<AdjustModel> _adjustmodels;
        public List<AdjustModel> AdjustModels
        {
            get => _adjustmodels;
            set => SetProperty(ref _adjustmodels, value);
        }

        private bool _isbtnEnable;

        public bool IsBtnEnable
        {
            get => _isbtnEnable;
            set => SetProperty(ref _isbtnEnable, value);
        }
    }

    public class InvoiceProductViewModel
    {


        public string ProductTypeName { get; set; }


        public string ManufactureName { get; set; }

        public string ProductName { get; set; }

        public float ProductRate { get; set; }

        public float LandingRate { get; set; }
        public float ProductSellingPrice { get; set; }

        public float ProductMRP { get; set; }

        public double Misc { get; set; }

        public int ProductSubQty { get; set; }

        public string DeptCode { get; set; }

        public string ProductSKU { get; set; }


    }
    public class PurchaseReturnProductViewModel : BindableBase
    {


        public string ProductTypeName { get; set; }
        public string ManufactureName { get; set; }
        public string ProductName { get; set; }
        public float ProductRate { get; set; }
        public float ProductSellingPrice { get; set; }
        public int? StroreId { get; set; }
        public string ProductUniqGuid { get; set; }
        public float ProductMRP { get; set; }
        public float ProductGST { get; set; }
        public double Misc { get; set; }
        public int ProductSubQty { get; set; }
        public string Lotnumber { get; set; }
        public string DeptCode { get; set; }
        public string ProductSKU { get; set; }
        public string InvoiceNo { get; set; }
        public string SupplierName { get; set; }
        public int StockProductId { get; set; }
        public int SupplierId { get; set; }

        private int _returnQty;
        public int ReturnQty
        {
            get { return _returnQty; }

            set
            {
                SetProperty(ref _returnQty, value);
                RowTotal = 0;
                if (_returnQty > 0)
                {
                    RowTotal = (((ProductRate * ProductGST) / 100) * _returnQty) + ((ProductRate * _returnQty));
                    ProductTotal = ((((ProductRate * ProductGST) / 100) + ProductRate) * ReturnQty);
                }

            }
        }

        private float _rowTotal;
        public float RowTotal
        {
            get => _rowTotal;
            set => SetProperty(ref _rowTotal, value);
        }

        private float _productTotal;
        public float ProductTotal
        {
            get => _productTotal;
            set => SetProperty(ref _productTotal, value);

        }

        private bool _isSeleced;
        public bool IsSelected
        {
            get => _isSeleced;
            set => SetProperty(ref _isSeleced, value);
        }

    }

    public class StoreReturnModels
    {
        public float Total { get; set; }
        public string Remark { get; set; }
        public int SupplierId { get; set; }
        public ObservableCollection<CreateReturnProductModel> PurhcaseReturnProduct { get; set; }

    }

    public class CreateReturnProductModel
    {
        public int? StroreId { get; set; }
        public string ProductSKU { get; set; }
        public int StockProductId { get; set; }
        public int ReturnQty { get; set; }
    }


    public class EditStoreReturnModel
    {

        public int Id { get; set; }


        public int SupplierId { get; set; }

        public float Total { get; set; }



        public ObservableCollection<SendbackEditProductModel> PurhcaseReturnProduct { get; set; }


        public ObservableCollection<DeleteProductModel> DeleteReturnProduct { get; set; }
    }

    public class SendbackEditProductModel
    {

        public float ProductRate { get; set; }


        public int? StoreId { get; set; }

        public string ProductSKU { get; set; }

        public int StockProductId { get; set; }

        public int ReturnQty { get; set; }
    }

    public class DeleteProductModel
    {


        public int ProductId { get; set; }


        public int StockProductId { get; set; }

    }

    public class SummaryViewModelCreidtNote
    {
        public int Id { get; set; }
        public string CreditNoteDate { get; set; }

        public string CreditNoteNumber { get; set; }
        public float Total { get; set; }
        public string Remark { get; set; }
        public int SupplierId { get; set; }
        public string Status { get; set; }
        public string StoreName { get; set; }
        public string SupplierName { get; set; }
        public int FileID { get; set; }

        public bool IsApproved { get; set; }

        public DateTime CreatedDatetime { get; set; }

        public int StoreId { get; set; }


        public Decimal RedeemedAmount { get; set; }


    }


    public class SummaryDetailsViewModelCreditNote : SummaryViewModelCreidtNote
    {

        public List<AdjustModel> AdjustModels { get; set; }


    }



}






