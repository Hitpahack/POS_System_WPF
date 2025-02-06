using FalcaPOS.Entites.ProductTypes;

namespace FalcaPOS.Entites.Manufacturers
{
    public class Manufacturer
    {
        public int ManufacturerId { get; set; }

        public string Name { get; set; }

        public int ProductTypeManufacturerId { get; set; }

        public ProductType ProductType { get; set; }

        public bool Isenabled { get; set; }

        public CategoryModel Category { get; set; }

    }

    
}
