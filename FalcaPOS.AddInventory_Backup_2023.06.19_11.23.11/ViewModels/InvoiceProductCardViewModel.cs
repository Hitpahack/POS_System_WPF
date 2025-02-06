using FalcaPOS.Contracts;
using FalcaPOS.Entites.Attributes;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FalcaPOS.AddInventory.ViewModels
{
    public class InvoiceProductCardViewModel : BindableBase
    {
        private readonly INotificationService _notificationService;


        public InvoiceProductCardViewModel(INotificationService notificationService)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            AttributesSelectedList = new List<ProductAttributeMapping>();

        }


        private string _productTypeName;
        public string ProductTypeName
        {
            get { return _productTypeName; }
            set { SetProperty(ref _productTypeName, value); }
        }


        private string _manufactureName;
        public string ManufactureName
        {
            get { return _manufactureName; }
            set { SetProperty(ref _manufactureName, value); }
        }


        private int _productId;
        public int ProductId
        {
            get { return _productId; }
            set { SetProperty(ref _productId, value); }
        }

        private string _productName;
        public string ProductName
        {
            get { return _productName; }
            set { SetProperty(ref _productName, value); }
        }

        private DateTime _dateOfManufacture;
        public DateTime DateOfManufacture
        {
            get { return _dateOfManufacture; }
            set { SetProperty(ref _dateOfManufacture, value); }
        }

        private DateTime? _dateOfExpiry;
        public DateTime? DateOfExpiry
        {
            get { return _dateOfExpiry; }
            set { SetProperty(ref _dateOfExpiry, value); }
        }

        private string _location;
        public string Location
        {
            get { return _location; }
            set { SetProperty(ref _location, value); }
        }


        private string _warrantyService;
        public string WarrantyService
        {
            get { return _warrantyService; }
            set { SetProperty(ref _warrantyService, value); }
        }


        private int _quantity;
        public int Quantity
        {
            get { return _quantity; }
            set { SetProperty(ref _quantity, value); }
        }

        private int _defectiveQuantity;
        public int DefectiveQuantity
        {
            get { return _defectiveQuantity; }
            set { SetProperty(ref _defectiveQuantity, value); }
        }

        private float _productRate;
        public float ProductRate
        {
            get { return _productRate; }
            set { SetProperty(ref _productRate, value); }
        }

        private float _productDiscount;
        public float ProductDiscount
        {
            get { return _productDiscount; }
            set { SetProperty(ref _productDiscount, value); }
        }

        private float _invocieProductDiscount;
        public float InvoiceProductDiscount
        {
            get { return _invocieProductDiscount; }
            set { SetProperty(ref _invocieProductDiscount, value); }
        }

        private bool _isinvocieProductDiscount;
        public bool isInvoiceProductDiscount
        {
            get { return _isinvocieProductDiscount; }
            set { SetProperty(ref _isinvocieProductDiscount, value); }
        }

        private float _productDiscountRate;
        public float ProductDiscountRate
        {
            get { return _productDiscountRate; }
            set { SetProperty(ref _productDiscountRate, value); }
        }

        private string _invoicediscountHeader;
        public string InvoiceDiscountHeader
        {
            get { return _invoicediscountHeader; }
            set { SetProperty(ref _invoicediscountHeader, value); }
        }

        public int? HSNCode { get; set; }

        private string _productUniqGuid;
        public string ProductUniqGuid
        {
            get { return _productUniqGuid; }
            set { SetProperty(ref _productUniqGuid, value); }
        }

        public string HSN { get; set; }

        public string LotNumber { get; set; }



        private float _productDiscountPerecent;


        public float ProductDiscountPerecent
        {
            get { return _productDiscountPerecent; }
            set
            {
                SetProperty(ref _productDiscountPerecent, value);
                //    if (InvoiceRate <= 0)
                //        return;

                //if (ProductSellingPrice == 0)
                //{
                //    value = 0;
                //}

                //if (value < 0 || value > 100)
                //{
                //    value = 0;
                //}


                //if (value > 0)
                //{
                //    CalCulateInvoiceDiscountPercent();
                //}
                //if (value == 0 && ProductDiscountFlat == 0)
                //{
                //    ProductDiscount = 0;
                //    CalulateInvoiceRoundOff();
                //}
            }
        }
        //public float ProductDiscountPerecent
        //{
        //    get { return _productDiscountPerecent; }
        //    set { SetProperty(ref _productDiscountPerecent, value); }
        //}

        private float _productGST;
        public float ProductGST
        {
            get { return _productGST; }
            set
            {
                SetProperty(ref _productGST, value);
                //if (InvoiceProductDiscount > 0)
                //{
                //    ProductGSTperQuantity = (ProductGST > 0 ? ((ProductDiscountRate * ProductGST) / 100) : 0);
                //}
                //else
                //{
                //    ProductGSTperQuantity = (ProductGST > 0 ? ((ProductRate * ProductGST) / 100) : 0);
                //}

            }
        }
        private float _productGSTperQuantity;
        public float ProductGSTperQuantity
        {
            get { return _productGSTperQuantity; }
            set
            {
                SetProperty(ref _productGSTperQuantity, value);

            }
        }

        private float _productMRP;
        public float ProductMRP
        {
            get { return _productMRP; }
            set
            {
                SetProperty(ref _productMRP, value);
                //if (value>=100 ||ProductSellingPrice<=0)
                //{
                //    value = 0;

                //}

                //CalculateGST();


            }
        }

        //private void CalculateGST()
        //{

        //    if (ProductSellingPrice > 0 && (ProductGST>0 && ProductGST<100))
        //    {
        //        //club product discount part.
        //        ProductTotal = (ProductSellingPrice - ProductDiscount);

        //        if (ProductTotal == 0)
        //        {
        //            float gst1 = ((ProductSellingPrice) * ProductGST) / 100;

        //            if (gst1 <= 0)
        //            {
        //                ProductTotal = (ProductSellingPrice - ProductDiscount);
        //            }
        //            else
        //            {
        //                ProductTotal = ProductTotal + gst1;
        //            }
        //            return;
        //        }

        //        float gst=((ProductSellingPrice-ProductDiscount)  * ProductGST)/ 100;

        //        if (gst <= 0)
        //        {
        //            ProductTotal = (ProductSellingPrice - ProductDiscount);
        //        }
        //        else
        //        {
        //            ProductTotal = ProductTotal + gst;
        //        }

        //    }
        //    else
        //    {
        //        ProductTotal = (ProductSellingPrice - ProductDiscount);
        //    }


        //}

        private float _productDiscountFlat;

        public float ProductDiscountFlat
        {
            get { return _productDiscountFlat; }
            set
            {

                if (value <= 0)
                {
                    //                    _notificationService.ShowMessage("Please give valid discount", Common.NotificationType.Error);
                    SetProperty(ref _productDiscountFlat, value);

                    calculateActulPrice();
                    return;
                }


                if (value >= ProductSellingPrice)
                {
                    value = 0;
                    SetProperty(ref _productDiscountFlat, value);
                    _notificationService.ShowMessage("Please give valid discount", Common.NotificationType.Error);
                    calculateActulPrice();
                    return;
                }
                //Discount issue commanded 
                //if (value >= Margin)
                //{
                //    value = 0;
                //    _notificationService.ShowMessage("Discount should not go below purchase rate", Common.NotificationType.Error);

                //}
                //if (value < 0)
                //    ProductDiscountFlat = 0;
                //if (value > 0)
                //    ProductSellingPrice = (ProductSellingPrice - ProductDiscountFlat);
                //if (value == 0)
                //    ProductSellingPrice = ProductOriginalSellingPrice;

                SetProperty(ref _productDiscountFlat, value);
                CalculateDiscount();

            }
        }

        public bool IsUpdating { get; set; }

        private void CalculateDiscount()
        {
            IsUpdating = true;
            ProductSellingPrice = (ProductSellingPrice - ProductDiscountFlat);
            IsUpdating = false;


        }

        private float _productTotal;
        public float ProductTotal
        {
            get { return _productTotal; }
            set
            {
                SetProperty(ref _productTotal, value);
            }
        }

        private float _productSellingPrice;
        public float ProductSellingPrice
        {
            get { return _productSellingPrice; }
            set
            {

                if (value <= 0)
                {
                    value = 0;
                    SetProperty(ref _productSellingPrice, value);
                    ResetPrice();
                    Margin = 0;
                    MarginPercentage = 0;
                    // CalculateMargin();
                    return;
                }
                else
                {

                    SetProperty(ref _productSellingPrice, value);
                    calculateActulPrice();
                    CalculateMargin();

                    if (IsUpdating)
                    {
                        ////CalculateDiscount();
                        ///noting todo
                    }
                    else
                    {
                        //reset if the selling price was changed.
                        ProductDiscountFlat = 0;

                    }
                }

                //if ((ProductMRP>0) && value > (ProductMRP + ProductGSTperQuantity))
                //{
                //    ProductSellingPrice = 0;
                //    _notificationService.ShowMessage("Selling Price should be less than MRP", Common.NotificationType.Error);                    
                //}
                //if (value < 0)
                //    ProductSellingPrice = 0;
                //if (value > 0)

                //if (ProductDiscountFlat == 0 && ProductSellingPrice !=0)
                //{
                //    if (ProductOriginalSellingPrice == 0)
                //        ProductOriginalSellingPrice = value;
                //    else if(ProductDiscountFlat == 0 &&  ProductOriginalSellingPrice != ProductSellingPrice )
                //        ProductSellingPrice = ProductOriginalSellingPrice;
                //}

            }
        }

        private void CalculateMargin()
        {
            if (InvoiceProductDiscount > 0)
            {
                Margin = (float)(ActualPrice - ProductDiscountRate - Misc);
                MarginPercentage = (Margin / ProductDiscountRate) * 100;
            }
            else
            {
                Margin = (float)(ActualPrice - ProductRate - Misc);
                MarginPercentage = (Margin / ProductRate) * 100;
            }

        }

        private void calculateActulPrice()
        {
            ActualPrice = (float)Convert.ToDecimal(String.Format("{0:0.00}", (ProductSellingPrice / (1 + ProductGST / 100))));
        }

        private void ResetPrice()
        {
            //ProductMRP = 0;
            ActualPrice = 0;
            ProductDiscountFlat = 0;

        }

        private float _actualPrice;
        public float ActualPrice
        {
            get { return _actualPrice; }
            set
            {
                SetProperty(ref _actualPrice, value);
                if (value > 0)
                    CalculateMargin();
            }
        }


        private string _productDiscountMode;
        public string ProductDiscountMode
        {
            get { return _productDiscountMode; }
            set { SetProperty(ref _productDiscountMode, value); }
        }

        private ObservableCollection<string> _serialNumbers;
        public ObservableCollection<string> SerialNumbers
        {
            get { return _serialNumbers; }
            set { SetProperty(ref _serialNumbers, value); }
        }
        public List<ProductAttributeMapping> AttributesSelectedList { get; set; }




        private double _misc;
        public double Misc
        {


            get { return _misc; }
            set { SetProperty(ref _misc, value); }
        }

        private float _margin;
        public float Margin
        {
            get { return _margin; }
            set { SetProperty(ref _margin, value); }
        }

        private float _marginPercentage;
        public float MarginPercentage
        {
            get { return _marginPercentage; }
            set { SetProperty(ref _marginPercentage, value); }
        }

        //public string BaseunitType { get; set; }

        //public string SubunitType { get; set; }

        //public int ProductSubQty { get; set; }

        //public bool IsGroupTrackMode { get; set; }


        private string _baseUnitType;
        public string BaseUnitType
        {
            get { return _baseUnitType; }
            set { SetProperty(ref _baseUnitType, value); }
        }

        private string _subUnitType;
        public string SubUnitType
        {
            get { return _subUnitType; }
            set { SetProperty(ref _subUnitType, value); }
        }


        private int _productSubQty;
        public int ProductSubQty
        {
            get { return _productSubQty; }
            set { SetProperty(ref _productSubQty, value); }
        }

        private bool _isGroupTrackMode;
        public bool IsGroupTrackMode
        {
            get { return _isGroupTrackMode; }
            set { SetProperty(ref _isGroupTrackMode, value); }
        }

        private string _inventoryTrackMode;
        public string InventoryTrackMode
        {
            get { return _inventoryTrackMode; }
            set { SetProperty(ref _inventoryTrackMode, value); }
        }

        private bool _isGroupdefective;
        public bool IsGroupdefective
        {
            get { return _isGroupdefective; }
            set { SetProperty(ref _isGroupdefective, value); }
        }

        private int _defectiveSubQty;
        public int DefectiveSubQty
        {
            get { return _defectiveSubQty; }
            set { SetProperty(ref _defectiveSubQty, value); }
        }


        private ObservableCollection<Dictionary<AttributeMap, AttributeMap>> _defectiveList = new ObservableCollection<Dictionary<AttributeMap, AttributeMap>>();
        public ObservableCollection<Dictionary<AttributeMap, AttributeMap>> DefectiveList
        {
            get { return _defectiveList; }
            set { SetProperty(ref _defectiveList, value); }
        }


        private string _deptCode;
        public string DeptCode
        {
            get { return _deptCode; }
            set { SetProperty(ref _deptCode, value); }
        }

        private string _productSKU;
        public string ProductSKU
        {
            get { return _productSKU; }
            set { SetProperty(ref _productSKU, value); }
        }

        public string GSTHeaderQty { get; set; }

        public string GSTHeaderPer { get; set; }

        public float OrginalProductSellingPrice { get; set; }

        public float RSP { get; set; }
    }
}
