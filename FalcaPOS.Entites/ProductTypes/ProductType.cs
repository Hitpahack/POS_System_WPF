using System;
using System.ComponentModel.DataAnnotations;

namespace FalcaPOS.Entites.ProductTypes
{
    public class ProductType
    {

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime Createddate { get; set; }

        public bool Isenabled { get; set; }

        public int? ProductTypeId { get; set; }

        public string ProductTypeShortCode { get; set; }

        public int? HSNCode { get; set; }

        public string DeptCode { get; set; }

        public string CategoryName { get; set; }




    }

    public class CreateProductTypeModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string DeptCode { get; set; }

    }


    public class CategoryModel
    {
        public int Id { get; set; }

        public string CategoryName { get; set; }

        public bool IsEnable { get; set; }

        public bool IsCertificate { get; set; }


    }

    public class SubCategoryModel
    {
        public int Id { get; set; }

        public string SubCategoryName { get; set; }

        public bool IsEnable { get; set; }

        public int CategoryId { get; set; }


    }



    
}
