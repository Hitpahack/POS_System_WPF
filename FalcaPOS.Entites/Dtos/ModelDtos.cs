using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalcaPOS.Entites.Dtos {
    internal class ModelDtos {
    }

    public class ProductDTO {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public bool IsEnable { get; set; }
        public int ProductTypeManufacturerId { get; set; }
        public string ProductSKU { get; set; }
        public string SubUnitType { get; set; }
        public CategoryDTO Category { get; set; }
        public SubCategoryDTO ProductType { get; set; }
        public ManufactureDTO Manufacturer { get; set; }
    }

    public class CategoryDTO {
        public int Id { get; set; }
        public string CategoryName { get; set; }

    }

    public class SubCategoryDTO {
        public int? ProductTypeId { get; set; }
        public string Name { get; set; }

    }

    public class ManufactureDTO {
        public int? ManufacturerId { get; set; }
        public string Name { get; set; }
    }
}
