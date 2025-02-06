using FalcaPOS.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FalcaPOS.Entites.Finance
{
    public class FinanceSale
    {
        [DisplayName("SalesType")]
        public string SalesType { get; set; }

        [DisplayName("Invoice Number")]
        public string InvNo { get; set; }

        [DisplayName("Invoice Date")]
        public DateTime InvDate { get; set; }
        public string Zone { get; set; }
        public string Territory { get; set; }

        [DisplayName("Store Name")]
        public string Store { get; set; }

        [DisplayName("Store Location")]
        public string Location { get; set; }

        [DisplayName("Billing Name")]
        public string BillingName { get; set; }
        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }

        [DisplayName("GSTIN")]
        public String GSTIN { get; set; }

        [DisplayName("Brand Name")]
        public string BrandName { get; set; }


        [DisplayName("Product Name")]
        public string ProductName { get; set; }

        [DisplayName("Category")]
        public string Category { get; set; }

        [DisplayName("Sub Category")]
        public string ItemName { get; set; }

        [DisplayName("HSN Code")]
        public string HSNCode { get; set; }


        [DisplayName("UOM")]
        public string UOM { get; set; }


        [DisplayName("Quantity")]
        public int Qty { get; set; }


        [DisplayName("Rate Per(Inc GST)")]
        public double Rate { get; set; }

        [DisplayName("Rate Per(WithOut GST)")]
        public decimal RatePerQty { get; set; }

        [DisplayName("Tax Rate %")]
        public int TaxRate { get; set; }

        [DisplayName("Taxable Value")]
        public decimal TaxableValue { get; set; }

        [DisplayName("CGST")]
        public decimal CGST { get; set; }

        [DisplayName("SGST")]
        public decimal SGST { get; set; }

        [DisplayName("IGST")]
        public decimal IGST { get; set; }

        [DisplayName("RedeemedCoupon")]
        public String RedeemedCoupon { get; set; }

        [DisplayName("Total")]
        public double Total { get; set; }

        [IgnoreProperty]
        public bool IsServiceInvoice { get; set; }

        [IgnoreProperty]
        public string Printer { get; set; }

        public String FiscalDay { get; set; }

        public string FiscalDayoftheMonth { get; set; }

        public string FiscalDayoftheQuater { get; set; }

        public string Fiscaldayoftheyear { get; set; }

        public String FiscalMonth { get; set; }
        public string FiscalMonthoftheQuarter { get; set; }
        public String FiscalMonthoftheYear { get; set; }
        public String FiscalQuarter { get; set; }
        public String FinancialYear { get; set; }


    }

    public class FinanceSalesSummery
    {

        [DisplayName("Category")]
        public string Category { get; set; }

        [DisplayName("Total Sales Invoice")]
        public int TotalSalesInvoice { get; set; }

        [DisplayName("Customer Count")]
        public int CustomerCount { get; set; }


        [DisplayName("Total Service Charge")]
        public decimal TotalServiceCharge { get; set; }

        [DisplayName("Total Sales")]
        public decimal TotalSales { get; set; }

        [DisplayName("Total")]
        public decimal Total { get; set; }
    }

    public class FinanceSalesResult
    {
        public IEnumerable<FinanceSale> Sales { get; set; }

        public IEnumerable<FinanceSalesSummery> Summery { get; set; }
    }
}
