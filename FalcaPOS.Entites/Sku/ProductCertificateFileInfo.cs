using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace FalcaPOS.Entites.Sku
{
    public class ProductCertificateFileInfo
    {
        public Guid FileId { get; set; }
        public String Size { get; set; }
        public int? FileremoteSrcID { get; set; }
        public string FileName { get; set; }

        public SKUGeneric SKUmodel { get; set; }
    }

    public class ProductCertificateSearch
    {
        public int supplierId { get; set; }
        public int departmentId { get; set; }
        public int manufactureID { get; set; }
        public int storeId { get; set; }


    }

    public class SKUGeneric
    {
        public string Number { get; set; }

        public string IssueDate { get; set; }


        public string Generic { get; set; }

        public int Supplierid { get; set; }
    }


    public class ViewSKUSearch
    {
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public int? BrandId { get; set; }
        public int? StoreId { get; set; }
        public bool Expired { get; set; }
        public bool NonExpired { get; set; }
        public bool NoCertificate { get; set; }
    }


    public class ProductCertificateView :BindableBase
    {
        public String Store { get; set; }
        public String Category { get; set; }
        public String SubCategory { get; set; }
        public String Brand { get; set; }
        public String Number { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ValidUpto { get; set; }

        public int BrandId { get; set; }
        public int SubCategoryId { get; set; }
        public int CategoryId { get; set; }
        public int StoreId { get; set; }



        private bool _noCertificateGrid;

        public bool NoCertificateGridnoDownload
        {
            get { return _noCertificateGrid; }
            set { SetProperty(ref _noCertificateGrid, value); }
        }

        public bool ExpiredGrid { get; set; }
        public bool ValidCertificateGrid { get; set; }

        public int? PictureId { get; set; }

        private bool _isedit;

        public bool IsEdit
        {
            get { return _isedit; }
            set { SetProperty(ref _isedit , value); }
        }

    }

    public class SKUModelVm
    {
        public IEnumerable<SKUvm> ViewSKUModelLists { get; set; }
    }

    public class SKUProductVm
    {
        public IEnumerable<ViewSKUProductVm> ViewSKUProductLists { get; set; }
    }


    public class SKUvm
    {
        public string Number { get; set; }

        public string Generic { get; set; }

        public string Department { get; set; }

        public string SupplierName { get; set; }

        public string Brand { get; set; }

        public string StoreName { get; set; }

        public DateTime IssueDate { get; set; }


        public int BrandId { get; set; }

        public int StoreId { get; set; }

        public int ProductCout { get; set; }
    }

    public class ViewSKUProductSearch
    {

        public int Storeid { get; set; }

        public bool Expired { get; set; }

        public bool ThisWeek { get; set; }

        public bool Next15Days { get; set; }

        public bool Next30Days { get; set; }

        public string Number { get; set; }

        public bool All { get; set; }
    }

    public class ViewSKUProductVm
    {

        public string Name { get; set; }

        public string ProductSKU { get; set; }

        public int? SerailNumber { get; set; }
        public string LicenseNumber { get; set; }

        public string ValidUp { get; set; }
    }
}
