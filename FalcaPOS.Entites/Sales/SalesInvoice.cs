using FalcaPOS.Entites.Customers;
using Prism.Mvvm;
using System;
using System.Collections.Generic;

namespace FalcaPOS.Entites.Sales
{
    public class SalesInvoice : BindableBase
    {
        //public int Id { get; set; }


        //public string InvoiceNumber { get; set; }


        //public decimal InvoiceTotal { get; set; }


        //public string CustomerName { get; set; }


        //public string Phone { get; set; }


        //public string Email { get; set; }


        //public decimal InvoiceDiscount { get; set; }


        //public int ItemsQty { get; set; }

        public int SalesId { get; set; }

        public string InvoiceNumber { get; set; }

        public DateTime InvoiceDate { get; set; }

        public float GrossTotal { get; set; }


        public string DiscountType { get; set; }

        public float Discount { get; set; }

        public float GST { get; set; }

        public float Total { get; set; }

        public string SalesType { get; set; }

        public DateTime? CreatedDate { get; set; }

        public SalesPayment SalesPayment { get; set; }

        private List<SalesProduct> salesProducts;
        public List<SalesProduct> SalesProducts
        {
            get
            {
                return salesProducts;
            }

            set
            {
                SetProperty(ref salesProducts, value);
            }
        }

        public bool IsOldCustomer { get; set; }

        public int CustomerId { get; set; }

        private CustomerDetails _customerDetails;
        public CustomerDetails CustomerDetails
        {
            get
            {
                return _customerDetails;
            }
            set
            {
                SetProperty(ref _customerDetails, value);
            }
        }
        public float SpecialDiscountAmount { get; set; }

        public string Remarks { get; set; }

        public string OrderTacknBy { get; set; }

        public bool IsSelectedShallow { get; set; }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, IsSelectedShallow); }
        }

        private List<ProductViewModel> _productDetail;
        public List<ProductViewModel> productDetail
        {
            get { return _productDetail; }
            set { SetProperty(ref _productDetail, value); }
        }

        public double Cash { get; set; }

        public double Card { get; set; }

        public double UPI { get; set; }

        public int StockQty { get; set; }


        public decimal TotalServiceCharges { get; set; }

        public double? Cheque { get; set; }

        public string ChequeNumber { get; set; }

        public string ChequeDate { get; set; }

        public string RealizeDate { get; set; }

        public float PayableAmount { get; set; }

        public bool IsChequeupdatebtn { get; set; }



    }

    public class ProductViewModel
    {
        public string Department { get; set; }
        public string Brand { get; set; }
        public string ProductName { get; set; }

        public int Qty { get; set; }
        public float Rate { get; set; }
        public float GST { get; set; }
        public float Discount { get; set; }
        public float SellingPrice { get; set; }

        public float ExtraDiscount { get; set; }


        public float ProductTotal { get; set; }

        public string Return { get; set; }
        public List<SalesProductSpec> ProductSpecifications { get; set; }
    }



    public class CreditSalesViewModel
    {
        public int SalesId { get; set; }

        public string InvoiceNumber { get; set; }

        public DateTime InvoiceDate { get; set; }

        public string CustomerName { get; set; }

        public string Phone { get; set; }


        public float GrossTotal { get; set; }

        public string OrderTacknBy { get; set; }

        public double? Cash { get; set; }

        public double? Card { get; set; }

        public double? UPI { get; set; }

        public double? Cheque { get; set; }

        public string ChequeNumber { get; set; }


        public string ChequeDate { get; set; }

        public float PayableAmount { get; set; }





    }

    public class UpdateCreditSales
    {
        public string InvoiceNumber { get; set; }

        public string ChequeNumber { get; set; }

        public string ChequeDate { get; set; }

        public string Remarks { get; set; }


    }

    public class FinanceCreditSalesViewModel
    {
        public int SalesId { get; set; }

        public string InvoiceNumber { get; set; }

        public DateTime InvoiceDate { get; set; }

        public string CustomerName { get; set; }

        public string Phone { get; set; }


        public float GrossTotal { get; set; }

        public string OrderTacknBy { get; set; }

        public double? Cash { get; set; }

        public double? Card { get; set; }

        public double? UPI { get; set; }

        public double? Cheque { get; set; }

        public string ChequeNumber { get; set; }


        public string ChequeDate { get; set; }

        public float PayableAmount { get; set; }

        public string RealizeDate { get; set; }

        public string Remarks { get; set; }

        public bool IsChequeupdatebtn { get; set; }


    }

}
