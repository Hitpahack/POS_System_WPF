using Prism.Mvvm;
using System;

namespace FalcaPOS.Entites.Stock
{
    public class ExpiryStockProductViewModel : BindableBase
    {
        public string Category { get; set; }
        public string ProductTypeName { get; set; }

        public string ManufactureName { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public DateTime? DateOfExpiry { get; set; }

        public string DateOfExpiryAsString { get; set; }
        public int Quantity { get; set; }

        public int StroreId { get; set; }

        public string StoreName { get; set; }

        public string Status { get; set; }

        public string DeptCode { get; set; }

        public string ProductSKU { get; set; }
        public int ProductSubQty { get; set; }

        public int StockProductId { get; set; }
        public string SalesInvoice { get; set; }

        private bool _isSeleced;
        public bool IsSelected
        {
            get => _isSeleced;
            set => SetProperty(ref _isSeleced, value);
        }

        public string Zone { get; set; }
        public string Territory { get; set; }
        // public FiscalModel FiscalModel { get; set; }

    }
    //public class FiscalModel  {
    //    public string FiscalDay { get; set; }
    //    public int FiscalDayOfMonth { get; set; }
    //    public int FiscalDayOfTheQuarter { get; set; }
    //    public int FiscalDayOfTheYear { get; set; }
    //    public string FiscalMonth { get; set; }
    //    public string FiscalMonthOfTheQuarter { get; set; }
    //    public string FiscalMonthOfTheYear { get; set; }
    //    public string FinancialYear { get; set; }
    //    public string FiscalQuarter { get; set; }
    //}
}
