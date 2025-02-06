using FalcaPOS.Entites.Manufacturers;
using FalcaPOS.Entites.ProductTypes;
using Prism.Mvvm;
using System;
using System.Collections.Generic;

namespace FalcaPOS.Entites.Sales
{
    public class SalesProduct : BindableBase
    {


        public int SelectedbySKUProductId { get; set; }
        public bool SelectedProduct { get; set; }

        public int StockProductId { get; set; }

        public int ExpiryProductCount { get; set; }

        public CategoryModel Category { get; set; }

        public ProductType ProductType { get; set; }

        public Manufacturer Manufacturer { get; set; }

        public string SKU { get; set; }
        public string ProductName { get; set; }

        public DateTime? WarrentyDate { get; set; }


        private string discountMode;
        public string DiscountMode
        {
            get { return discountMode; }
            set { SetProperty(ref discountMode, value); }
        }

        public float Discount { get; set; }

        public float ProductGST { get; set; }


        private float _productSellingPrice;
        public float ProductSellingPrice
        {
            get => _productSellingPrice;
            set => SetProperty(ref _productSellingPrice, value);
        }


        public float ProductTotal { get; set; }


        public float ProductDiscountPercent { get; set; }

        public float ProductDiscountFlat { get; set; }



        public List<SalesProductSpec> ProductSpecifications { get; set; }

        public string BarCode { get; set; }

        public float ProductRate { get; set; }

        public int Quantity { get; set; }

        public int AvailableQuantity { get; set; }

        public bool IsGroupTrackMode { get; set; }

        public int SellingQty { get; set; }



        private int _returnQty;
        public int ReturnQty
        {
            get { return _returnQty; }
            set { SetProperty(ref _returnQty, value); }
        }

        public int StockQty { get; set; }

        /// <summary>
        /// Extra discount added to product at sales time
        /// </summary>
        public float ExtraDiscount { get; set; }


        /// <summary>
        /// Service charge applicable to this product  <br/>
        /// Currently applicable only for product type <b>Fertilizer</b>        
        /// </summary>
        public bool IsServiceChargeApplicable { get; set; }


        /// <summary>
        /// Service Charge amount if applicable
        /// </summary>
        public float ServiceChargeAmount { get; set; }

        private bool _isreturnProduct;
        public bool IsReturnProduct { get { return _isreturnProduct; } set { SetProperty(ref _isreturnProduct, value); } }

        private int _returnDefectiveqty;
        public int ReturnDefectiveQty { get { return _returnDefectiveqty; } set { SetProperty(ref _returnDefectiveqty, value); } }

        public float SoldSellingPrice { get; set; }

        public string Remarks { get; set; }

        public bool? isreturn { get; set; }
        private bool _isChecked;
        public bool IsChecked { get { return _isChecked; } set { SetProperty(ref _isChecked, value); } }

        public DateTime? ExpiryDate { get; set; }

        public float MRP { get; set; }

        public int ProductId { get; set; }

        public string HSNcode { get; set; }

        public string Description { get; set; }

    }

    public class SalesProductDTO
    {
        public int StockProductId { get; set; }

        public int SelectedbySKUProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductType { get; set; }

        public string BrandName { get; set; }

        public string SKU { get; set; }

        public float ProductSellingPrice { get; set; }

        public string BarCode { get; set; }

        public int SellingQty { get; set; }

        public string DiscountMode { get; set; }

        public string DiscountDetails { get; set; }

        public float Discount { get; set; }

        public float ProductGST { get; set; }
        public float ProductTotal { get; set; }
        public bool IsGroupTrackMode { get; set; }

        public float ExtraDiscount { get; set; }

        public bool IsServiceChargeApplicable { get; set; }
        public float ServiceChargeAmount { get; set; }

        public DateTime? WarrentyDate { get; set; }


    }


}
