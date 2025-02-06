using FalcaPOS.Contracts;
using FalcaPOS.Entites.Manufacturers;
using FalcaPOS.Entites.ProductTypes;
using FalcaPOS.Entites.Sales;
using FalcaPOS.Sales.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FalcaPOS.Sales.Models
{
    public class SalesProductModel : BindableBase
    {
        private readonly INotificationService _notificationService;

        public SalesProductModel(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public int ExpiryProductCount { get; set; }
        public int StockProductId { get; set; }

        public int SelectedbySKUProductId { get; set; }

        public ProductType ProductType { get; set; }

        public Manufacturer Manufacturer { get; set; }

        public string SKU { get; set; }
        public string ProductName { get; set; }

        public DateTime? WarrentyDate { get; set; }


        private string _discountMode;
        public string DiscountMode
        {
            get { return _discountMode; }
            set
            {
                SetProperty(ref _discountMode, value);

                //if (value == "Percent")
                //{
                //    ProductDiscountPercent = Discount;
                //}else if(value== "Flat")
                //{
                //    ProductDiscountFlat = Discount;
                //}
            }
        }

        private string _couponType;
        public string CouponType
        {
            get { return _couponType; }
            set
            {
                SetProperty(ref _couponType, value);
            }
        }

        private string _couponName;
        public string CouponName
        {
            get { return _couponName; }
            set
            {
                SetProperty(ref _couponName, value);
            }
        }

        private string _couponCode;
        public string CouponCode
        {
            get { return _couponCode; }
            set
            {
                SetProperty(ref _couponCode, value);
            }
        }

        private string _discountDetails;
        public string DiscountDetails
        {
            get { return _discountDetails; }
            set
            {
                SetProperty(ref _discountDetails, value);
            }
        }

        private float _productDiscountPercent;

        public float ProductDiscountPercent
        {
            get { return _productDiscountPercent; }
            set
            {
                SetProperty(ref _productDiscountPercent, value);
                //if (value != 0)
                //{
                //    ProductDiscountFlat = 0;
                //}
            }
        }

        private float _productDiscountFlat;
        public float ProductDiscountFlat
        {
            get { return _productDiscountFlat; }
            set
            {
                SetProperty(ref _productDiscountFlat, value);
                //if (value != 0)
                //{
                //    ProductDiscountPercent = 0;
                //}
            }
        }


        private float _discount;
        public float Discount {
            get { return _discount; }

            set {

                SetProperty(ref _discount, value);
            }
        }

        private float _productGST;
        public float ProductGST
        {
            get { return _productGST; }
            set { SetProperty(ref _productGST, value); }
        }

        private float _productSellingPrice;
        public float ProductSellingPrice
        {
            get { return _productSellingPrice; }
            set { SetProperty(ref _productSellingPrice, value); }
        }

        private float _productTotal;
        public float ProductTotal
        {
            get { return _productTotal; }
            set { SetProperty(ref _productTotal, value); }
        }

        //public float ProductDiscountPercent { get; set; }

        //public float ProductDiscountFlat { get; set; }

        public string BarCode { get; set; }


        public List<SalesProductSpec> ProductSpecifications { get; set; }

        private int _availableQuantity;
        public int AvailableQuantity
        {
            get { return _availableQuantity; }
            set { SetProperty(ref _availableQuantity, value); }
        }

        private bool _isGroupTrackMode;


        public bool IsGroupTrackMode
        {
            get { return _isGroupTrackMode; }
            set { SetProperty(ref _isGroupTrackMode, value); }
        }

        private bool _isSellingQty;


        public bool IsSellingQty
        {
            get { return _isSellingQty; }
            set { SetProperty(ref _isSellingQty, value); }
        }


        private int _sellingQty;
        public int SellingQty
        {
            get { return _sellingQty; }
            set
            {
                if (value <= 0)
                {
                    value = 1;
                }
                if (value > AvailableQuantity)
                {
                    value = AvailableQuantity;

                    _notificationService.ShowMessage($"Max available quantity for {ProductName} is {AvailableQuantity}", Common.NotificationType.Error);
                }
                SetProperty(ref _sellingQty, value);
                CardProductTotal = (value * ProductSellingPrice);
                CardServiceChargesTotal = (value * (decimal)ServiceChargeAmount);

                PriceCalculateEvent?.Invoke(this, null);
            }
        }


        public event EventHandler<PriceCalculateEventArgs> PriceCalculateEvent;

        /// <summary>
        /// Base selling price of the product
        /// </summary>
        public float AcutalSellingPrice { get; set; }

        private float _productManualDiscount;
        public float ProductManualDiscount
        {
            get { return _productManualDiscount; }
            set
            {

                if (value <= 0)
                {
                    value = 0;
                }
                if (value >= AcutalSellingPrice)
                {
                   // value = 0;
                    //_notificationService.ShowMessage("Discount cannot be greater than selling price", Common.NotificationType.Error);
                }
                //Discount issue commanded
                //if (value >= (AcutalSellingPrice - ProductRate))
                //{
                //    value = 0;
                //    _notificationService.ShowMessage("Discount should not go below purchase rate", Common.NotificationType.Error);
                //}
                //new selling Price becomes                 
                SetProperty(ref _productManualDiscount, value);
                //ProductSellingPrice = AcutalSellingPrice - value;
                PriceCalculateEvent?.Invoke(this, null);
                CardProductTotal = (SellingQty * ProductSellingPrice);
            }
        }



        private bool _isServiceChargeApplicable;
        /// <summary>
        /// Service charge applicable to this product  <br/>
        /// Currently applicable only for product type <b>Fertilizer</b>        
        /// </summary>

        public bool IsServiceChargeApplicable
        {
            get { return _isServiceChargeApplicable; }
            set { SetProperty(ref _isServiceChargeApplicable, value); }
        }




        private float _serviceChargeAmount;
        /// <summary>
        /// Service Charge amount if applicable
        /// </summary>
        public float ServiceChargeAmount
        {
            get { return _serviceChargeAmount; }

            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                if (value >= ProductSellingPrice)
                {
                    value = 0;
                    _notificationService.ShowMessage("Service charge should not be greater than or equal to Selling price", Common.NotificationType.Error);
                }
                SetProperty(ref _serviceChargeAmount, value);
                PriceCalculateEvent?.Invoke(this, null);
                CardServiceChargesTotal = ((decimal)value * SellingQty);

            }
        }

        private float _cardProductTotal;
        public float CardProductTotal
        {
            get { return _cardProductTotal; }
            set { SetProperty(ref _cardProductTotal, value); }
        }

        private decimal _cardServiceChargesTotal;
        public decimal CardServiceChargesTotal
        {
            get { return _cardServiceChargesTotal; }
            set { SetProperty(ref _cardServiceChargesTotal, value); }
        }

        private float _productRate;

        public float ProductRate
        {
            get { return _productRate; }
            set { SetProperty(ref _productRate, value); }
        }

        private int _productid;

        public int ProductId
        {
            get => _productid;
            set => SetProperty(ref _productid , value);
        }

       
    }

}
