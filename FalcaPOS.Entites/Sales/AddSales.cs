using FalcaPOS.Entites.Customers;
using System;
using System.Collections.Generic;

namespace FalcaPOS.Entites.Sales
{
    public class AddSales
    {


        public string InvoiceNumber { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public float GrossTotal { get; set; }


        public string DiscountType { get; set; }

        public float Discount { get; set; }

        public float GST { get; set; }

        public float Total { get; set; }

        public string SalesType { get; set; }

        public DateTime? CreatedDate { get; set; }

        public SalesPayment SalesPayment { get; set; }

        public List<SalesProduct> SalesProducts { get; set; }

        public bool IsOldCustomer { get; set; }

        public int CustomerId { get; set; }

        public CustomerDetails CustomerDetails { get; set; }

        public bool IsSepcialDiscount { get; set; }

        public float SpecialDiscountAmount { get; set; }

        public string Remarks { get; set; }
        /// <summary>
        /// Total Service charge for the sale
        /// </summary>
        public decimal TotalServiceCharges { get; set; }

        /// <summary>
        /// Update customer address
        /// </summary>
        public bool UpdateCustomerAddress { get; set; }

        public bool IsAppOrderCustomer { get; set; }


        public int AppOrderId { get; set; }

    }

    public class Product
    {
        public string DepartmentName { get; set; }
        public string ProductName { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public double SellingPrice { get; set; }
        public int OrderQty { get; set; }
        public int StockQty { get; set; }


    }


    public class AppOrderModel
    {
        public int AppOrderId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
        public string OrderDate { get; set; }
        public int StoreID { get; set; }
        public int SelectedIndex { get; set; }

        public bool IsLinear { get; set; }

        public bool IsProcess { get; set; }
        public List<Product> Products { get; set; }
        public Address ShippingAddress { get; set; }
    }

    public enum AppOrderStatus
    {
        processing = 0,
        intransit = 1,
        delivery = 2,

    }

    public class POSCustomerToDsc
    {
        public string Name { get; set; }

        public string Phone { get; set; }

        public string Alternatephone { get; set; }

        public string GstNumber { get; set; }

        public string Email { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public string District { get; set; }

        public string State { get; set; }

        public int? Pincode { get; set; }


    }

    public class SalesModel
    {

        public float GrossTotal { get; set; }


        public string DiscountType { get; set; }

        public float Discount { get; set; }

        public float GST { get; set; }

        public float Total { get; set; }

        public string SalesType { get; set; }

        public SalesPaymentModel SalesPayment { get; set; }

        public int CustomerId { get; set; }

        public bool IsOldCustomer { get; set; }

        public bool UpdateCustomerAddress { get; set; }

        public CustomerModel CustomerDetails { get; set; }

        public List<SalesProductDTO> SalesProducts { get; set; }

        //public string Remarks { get; set; }

        public decimal TotalServiceCharges { get; set; }

        public float SpecialDiscountAmount { get; set; }

        public bool IsAppOrderCustomer { get; set; }

        public int AppOrderId { get; set; }
        public bool IsSepcialDiscount { get; set; }

        public String CouponCode { get; set; }

        public string AppliedCoupons { get; set; }
        public float CouponAmount { get; set; }

        public String BillingId { get; set; }

        public String CouponId { get; set; } 

        public bool IsFutureOfferAlsoCreated { get; set; }

    }
}
