using FalcaPOS.Entites.ProductTypes;
using Prism.Mvvm;
using System;

namespace FalcaPOS.Entites.Products
{
    public class ProductViewModel : BindableBase
    {
        public string Name { get; set; }


        public string Description { get; set; }

        public bool Isenabled { get; set; }

        public int ProductTypeManufacturerId { get; set; }

        public string ProductSKU { get; set; }


        public BrandViewModel Manufacturer { get; set; }

        public SubCategoryModel SubCategory { get; set; }

        public DepartmentViewModel ProductType { get; set; }


        public DateTime? CreatedDate { get; set; }

        public int? ProductId { get; set; }

        public string InventoryTrackMode { get; set; }

        public string SubUnitType { get; set; }

        private bool _isDefectiveqty;
        public bool IsDefectiveQty
        {
            get => _isDefectiveqty;
            set => SetProperty(ref _isDefectiveqty, value);
        }


        private int defectiveqty;
        public int DefectiveQty
        {
            get => defectiveqty;
            set => SetProperty(ref defectiveqty, value);
        }
        private String _productStockCount;
        public String ProductStockCount
        {
            get => _productStockCount;
            set => SetProperty(ref _productStockCount, value);
        }

        private CategoryModel _category;

        public CategoryModel Category
        {
            get { return _category; }
            set { SetProperty(ref _category , value); }
        }

      


    }

    public class BrandViewModel
    {

        public string Name { get; set; }


        public string Description { get; set; }


        public bool Isenabled { get; set; }

        public DateTime? Createdate { get; set; }


        public int? ManufacturerId { get; set; }

    }
    public class DepartmentViewModel
    {
        public string Name { get; set; }


        public string Description { get; set; }


        public bool Isenabled { get; set; }

        public int? ProductTypeId { get; set; }

        public int? HSNCode { get; set; }

        public string DeptCode { get; set; }
    }
    public class InventaryProductViewModel : ProductViewModel
    {
        public string Number { get; set; }

        public int FromStoreStock { get; set; }
    }
}
