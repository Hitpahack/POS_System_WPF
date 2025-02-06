using System;
using System.ComponentModel.DataAnnotations;

namespace FalcaPOS.Entites.ProductTypes
{
    public class ProductTypeManufacturer
    {

        public string Name { get; set; }

        public int productTypeId { get; set; }

        public bool isEnabled { get; set; }

        public string ProductTypeManufactrerShortCode { get; set; }

        public bool MapToProductType { get; set; }

        public int BrandId { get; set; }

        public DateTime CreateDatetime { get; set; }

    }


    public class CreateManufactureModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int productTypeId { get; set; }

        [Required]
        public bool MapToProductType { get; set; }

        public int BrandId { get; set; }
    }
}
