using System.ComponentModel;

namespace FalcaPOS.Reports.Model
{
    public class InventoryReport : ReportBaseModel
    {
        public string SKUCode { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Technical { get; set; }
        public string Brand { get; set; }
        public string Product { get; set; }
        //public string ProductSKU { get; set; }
        public string PackingSize { get; set; }
        public string PackingSizeNumber { get; set; }
        public string UOM { get; set; }
        public string State { get; set; }
        public string Region { get; set; }
        public string Territory { get; set; }
        public string StoreName { get; set; }
        public int Quantity { get; set; }
        public string StoreCode { get; set; }
        public decimal Volume { get; set; }
        public decimal Gst { get; set; }
        public decimal GstValue { get; set; }

        [DisplayName("Cost Value (Tax Exclusive) Per Quantity")]
        public decimal CTaxExclusive { get; set; }

        [DisplayName("Cost Value (Tax Inclusive) Per Quantity")]
        public decimal CTaxInclusive { get; set; }

        [DisplayName("Cost Value (Tax Exclusive) whole Volume")]
        public decimal CTaxEWVolume { get; set; }

        [DisplayName("Cost Value (Tax Inclusive) whole Volume")]
        public decimal CTaxIWVolume { get; set; }
        public float SellingPrice { get; set; }
        public decimal InventoryValue { get; set; }
        public string LastDateSale { get; set; }
        [DisplayName("Days Since Last Sale Invoice")]
        public int Days { get; set; }
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

    public class InventoryReport2 : ReportBaseModel
    {
        public string SKUCode { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Technical { get; set; }
        public string Brand { get; set; }
        public string Product { get; set; }
        //public string ProductSKU { get; set; }
        public string PackingSize { get; set; }
        public string PackingSizeNumber { get; set; }
        public string UOM { get; set; }
        public string State { get; set; }
        public string Region { get; set; }
        public string Territory { get; set; }
        public string StoreName { get; set; }
        public int Quantity { get; set; }
        public string StoreCode { get; set; }
        public float SellingPrice { get; set; }
        public decimal InventoryValue { get; set; }
        public string LastDateSale { get; set; }
        [DisplayName("Days Since Last Sale Invoice")]
        public int Days { get; set; }
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

      