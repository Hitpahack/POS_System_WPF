using System;
using System.ComponentModel;

namespace FalcaPOS.Reports.Model
{
    public class POGRNReport :ReportBaseModel
    {
        public string SKUCode { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Technical { get; set; }
        public string Brand { get; set; }
        public string Product { get; set; }
        //public string ProductSKU { get; set; }
        public string PackingSize { get; set; }
        public string UOM { get; set; }
        public string State { get; set; }
        public string Zone { get; set; }

        [DisplayName("Territory")]
        public string Area { get; set; }
        public string StoreName { get; set; }
        public string StoreCode { get; set; }
        public string PoNumber { get; set; }
        public string PODate { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string Vendor { get; set; }
        public int PoQuantity { get; set; }
        public int GRNQuantity { get; set; }
        public int SoldQuantity { get; set; }
        public decimal SoldAmount { get; set; }
        public string GRNumber { get; set; }
        public string BatchNumber { get; set; }
        public decimal Volume { get; set; }

        [DisplayName("Active PO")]
        public bool IsActive { get; set; }

        [DisplayName("GST Percentage")]
        public decimal Gst { get; set; }

        [DisplayName("GST Value Per Quantity")]
        public decimal GstValue { get; set; }

        [DisplayName("Total GST Value")]
        public decimal TotalGst { get; set; }
        
        [DisplayName("Cost Value (Tax Exclusive) Per Quantity")]
        public decimal CTaxExclusive { get; set; }

        [DisplayName("Cost Value (Tax Inclusive) Per Quantity")]
        public decimal CTaxInclusive { get; set; }

        [DisplayName("Cost Value (Tax Exclusive) whole Volume")]
        public decimal CTaxEWVolume { get; set; }

        [DisplayName("Cost Value (Tax Inclusive) whole Volume")]
        public decimal CTaxIWVolume { get; set; }

        public double PurchaseDiscount { get; set; }

        [DisplayName("Additional Purchase Cost (Frieght+Logistics+Hamali)")]
        public double AddPurchaseCost { get; set; }

        [DisplayName("Net Realised Purchase Cost")]
        public double NetPurchaseCost { get; set; }

        [DisplayName("Last Date Purchase Invoice")]
        public string LastDatePurchase { get; set; }

        [DisplayName("Days Since Last Purchase Invoice")]
        public int DaySincePurchase { get; set; }
        public string FiscalDay { get; set; }
        public string FiscalDayOfMonth { get; set; }
        public string FiscalDayOfTheQuarter { get; set; }
        public string FiscalDayOfTheYear { get; set; }
        public string FiscalMonth { get; set; }
        public string FiscalMonthOfTheQuarter { get; set; }
        public string FiscalMonthOfTheYear { get; set; }
        public string FinancialYear { get; set; }
        public string FiscalQuarter { get; set; }

    }
}
