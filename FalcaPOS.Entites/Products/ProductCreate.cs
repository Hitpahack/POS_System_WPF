using FalcaPOS.Entites.Manufacturers;
using FalcaPOS.Entites.ProductTypes;
using FalcaPOS.Entites.Sku;
using Prism.Mvvm;
using System;

namespace FalcaPOS.Entites.Products
{
    public class ProductDetails : BindableBase
    {

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }

        }

        public string Description { get; set; }

        public bool Isenabled { get; set; }


        public int ProductTypeManufacturerId { get; set; }

        public int ProductId { get; set; }



        public CategoryModel Category { get; set; }
        public ProductType ProductType { get; set; }

        public Manufacturer Manufacturer { get; set; }

        public string InventoryTrackMode { get; set; }
 

        public string ProductSKU { get; set; }

        public DateTime CreateDatetime { get; set; }

        public string SubUnitType { get; set; }

    }


}
