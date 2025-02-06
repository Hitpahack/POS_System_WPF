namespace FalcaPOS.Entites.Products
{
    public class ProductSearchModel
    {

        public int CategoryID { get; set; }
        public int SubcategoryID { get; set; }
        public int BrandID { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }

        public string ProductSKU { get; set; }

        public string ProductType { get; set; }

        public string DeptCode { get; set; }

        public string Brand { get; set; }
        public string Category { get; set; }

        public string SubUnitType { get; set; }
        public override string ToString()
        {
            return ProductName;
            return $"{ProductName} (SKU {ProductSKU})";
        }
        public int StoreID { get; set; }

        public bool IsDepartEnabled { get; set; }

        public int PoQty { get; set; }

        public float PoRate { get; set; }

        public string PoMsg { get; set; }

    }
}
