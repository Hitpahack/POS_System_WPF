using System.ComponentModel;

namespace FalcaPOS.Entites.Stock
{
    public class ClosingStockDetails
    {
        public string StoreName { get; set; }
        public string Zone { get; set; }
        public string Territory { get; set; }

        [DisplayName("Product Name")]
        public string ProductName { get; set; }

        [DisplayName("Technical Name")]
        public string TechnicalName { get; set; }

        [DisplayName("Brand")]
        public string Brand { get; set; }

        [DisplayName("Category")]
        public string Category { get; set; }

        [DisplayName("Sub Category")]
        public string ProductType { get; set; }

        [DisplayName("Existing SKU(Old SKU)")]
        public string ExistingSKU { get; set; }

        [DisplayName("SKU")]
        public string SKU { get; set; }

        //[DisplayName("DepartCode")]
        //public string DepartCode { get; set; }

        //public int ProductId { get; set; }

        [DisplayName("Opening Stock Quantity")]
        public int ClosingStockQty { get; set; }

        [DisplayName("Purchase Stock Quantity")]
        public int PurchaseStockQty { get; set; }

        [DisplayName("Sold Quantity")]
        public int SoldQty { get; set; }

        [DisplayName("Total Stock")]
        public int TotalStock { get; set; }


        public decimal WAC { get; set; }

        public decimal WACValue { get; set; }

        public string LastSoldDate { get; set; }

        public int StockTransfer { get; set; }

        public int SalesReturn { get; set; }
    }
}
