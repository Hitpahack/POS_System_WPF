using FalcaPOS.Entites.Stock;
using System;
using System.ComponentModel;

namespace FalcaPOS.Entites.StockAge
{
    public class StockAgeData
    {
        [DisplayName("POS entry date")]
        public DateTime PurchaseDate { get; set; }
        public String Supplier { get; set; }

        public string Category { get; set; }

        [DisplayName("Sub Category")]
        public String SubCategory { get; set; }
        public String SKU { get; set; }
        public String Brand { get; set; }
        public String Units { get; set; }

        [DisplayName("ProductName")]
        public String Product { get; set; }
        public String Store { get; set; }
        public string Zone { get; set; }
        public string Territory { get; set; }
        public string FiscalDay { get; set; }
        public int FiscalDayOfMonth { get; set; }
        public int FiscalDayOfTheQuarter { get; set; }
        public int FiscalDayOfTheYear { get; set; }
        public string FiscalMonth { get; set; }
        public string FiscalMonthOfTheQuarter { get; set; }
        public string FiscalMonthOfTheYear { get; set; }
        public string FinancialYear { get; set; }
        public string FiscalQuarter { get; set; }

    }


    public class StockAgeCostRateData : StockAgeData
    {
        public int Qty { get; set; }
        public double Cost { get; set; }
        public double TotalCost { get; set; }
       
    }

    public class StockAgeSellingPriceData : StockAgeData
    {
        public int Qty { get; set; }
        public double SellingPrice { get; set; }
        public double TotalSellingPrice { get; set; }
       
    }

    public class StockAgeDataDuration : StockAgeCostRateData
    {
        public double Today { get; set; }

        [DisplayName("1-15 Days")]
        public double Days_1_15 { get; set; }

        [DisplayName("16-30 Days")]
        public double Days_15_30 { get; set; }
        [DisplayName("31-45 Days")]
        public double Days_31_45 { get; set; }
        [DisplayName("46-60 Days")]
        public double Days_46_60 { get; set; }
        [DisplayName("61-75 Days")]
        public double Days_61_75 { get; set; }
        [DisplayName("76-90 Days")]
        public double Days_76_90 { get; set; }
        [DisplayName("91-105 Days")]
        public double Days_91_105 { get; set; }
        [DisplayName("106-120 Days")]
        public double Days_106_120 { get; set; }
        [DisplayName("Above120Days")]
        public double Above120Days { get; set; }

    }

    public class StockAgeSellingDataDuration : StockAgeSellingPriceData
    {
        public double Today { get; set; }

        [DisplayName("1-15 Days")]
        public double Days_1_15 { get; set; }

        [DisplayName("16-30 Days")]
        public double Days_15_30 { get; set; }
        [DisplayName("31-45 Days")]
        public double Days_31_45 { get; set; }
        [DisplayName("46-60 Days")]
        public double Days_46_60 { get; set; }
        [DisplayName("61-75 Days")]
        public double Days_61_75 { get; set; }
        [DisplayName("76-90 Days")]
        public double Days_76_90 { get; set; }
        [DisplayName("91-105 Days")]
        public double Days_91_105 { get; set; }
        [DisplayName("106-120 Days")]
        public double Days_106_120 { get; set; }
        [DisplayName("Above120Days")]
        public double Above120Days { get; set; }

    }

}
