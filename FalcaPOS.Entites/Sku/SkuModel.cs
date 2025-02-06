using FalcaPOS.Entites.Products;
using Prism.Mvvm;
using System;
using System.Collections.Generic;

namespace FalcaPOS.Entites.Sku
{

    public class ProductTypesViewModel : BindableBase
    {
        public int Id { get; set; }

        private string _productType;

        public string ProductType
        {
            get { return _productType; }
            set { SetProperty(ref _productType, value); }
        }
        private string _productTypewithDeptcode;

        public string ProductTypeWithDeptcode
        {
            get { return _productTypewithDeptcode; }
            set { SetProperty(ref _productTypewithDeptcode, value); }
        }
        public string DeptCode { get; set; }

       

        private List<ViewModel> _products;

        public List<ViewModel> Products
        {
            get { return _products; }
            set { SetProperty(ref _products, value); }
        }

        private string _productCount;

        public string ProductCount
        {
            get { return _productCount = Convert.ToString((FirstColumnProducts?.Count +SecondColumnProducts?.Count)); }
            set { SetProperty(ref _productCount, value); }
        }
       

        private List<ViewModel> _firstColumnProducts;

        public List<ViewModel> FirstColumnProducts {
            get => _firstColumnProducts; 
            set => SetProperty(ref _firstColumnProducts , value); 
        }

        private List<ViewModel> _secondColumnProducts;

        public List<ViewModel> SecondColumnProducts {
            get => _secondColumnProducts;
            set => SetProperty(ref _secondColumnProducts, value);
        }

    }

    public class ViewModel : BindableBase
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public string ProductName { get; set; }



        private int? _count;
        public int? Count
        {
            get { return _count; }
            set
            {
                SetProperty(ref _count, value);
            }
        }
        public string BrandName { get; set; }
        public int ServerCount { get; set; }

        //using for only UI
        public string _storesub;
        public string StoreSub
        {
            get { return _storesub = !string.IsNullOrEmpty(StoreSubUnit) ? "/" + StoreSubUnit : ""; }
            set { SetProperty(ref _storesub, value); }
        }

        public string StoreSubUnit { get; set; }

        public string ServerSubUnit { get; set; }


        private bool _isMatch;

        public bool IsMatch
        {
            get
            {
                return _isMatch = Count == ServerCount ? true : false;
            }
            set
            {
                SetProperty(ref _isMatch, value);
            }
        }

        private bool _isEnable;

        public bool IsEnable
        {
            get { return _isEnable; }

            set { SetProperty(ref _isEnable, value); }
        }

    }

    public class DailyStockSearch
    {
        public string SelectedDate { get; set; }
        public int StoreId { get; set; }

    }

    public class SKUModel
    {

        public string Producttype { get; set; }



    }




    public class SkuSheetProductTypeViewModel
    {
        public int Id { get; set; }
        public string ProductType { get; set; }
        public string DeptCode { get; set; }
        public List<SkuSheetProductViewModel> Products { get; set; }
    }

    public class SkuSheetProductViewModel
    {
        public string Sku { get; set; }
        public string ProductName { get; set; }
        public string BrandName { get; set; }
    }


    public class CreateProductModel {
        public string Name { get; set; }

        public string Description { get; set; }

        public string SubUnitType { get; set; }

        public int ProductTypeManufacturerId { get; set; }

        public string TechnicalName { get; set; }

        public string PackingSize { get; set; }

        public string UOM { get; set; }

        public string Type { get; set; }

        public string ProductSKU { get; set; }

        public string Hsn { get; set; }

        public string Warranty { get; set; }

        public float GST { get; set; }
        public decimal MinMargin { get; set; }


    }


    public class CreateProductCertificateModel : CreateProductModel
    {
        public string ValidUpto { get; set; }

        public bool LifeTime { get; set; }

        public int? SerailNumber { get; set; }
        public string LicenseNumber { get; set; }
    }

    public class CreateSKUModel
    {
        public string Number { get; set; }

        public string IssueDate { get; set; }

        public string Generic { get; set; }

        public int SupplierId { get; set; }

        public int ProductTypeId { get; set; }

        public string ProductTypeName { get; set; }

        public int BrandId { get; set; }

        public int? PictureId { get; set; }

        public List<CreateProductCertificateModel> productCertificateModels { get; set; }

        public string CategoryName { get; set; }

        public int StoreId { get; set; }

    }

    public class TypeViewModel : DepartmentViewModel
    {
        public int RequestId { get; set; }
        public int StoreId { get; set; }

        public string StoreName { get; set; }

        public DateTime CreatedDate { get; set; }

        public string ProductLastSKU { get; set; }

        public SKUViewModelVM SKUViewModel { get; set; }

        public String HumanizerDate { get; set; }
    }

    public class SKUViewModelVM
    {
        public string Number { get; set; }

        public string IssueDate { get; set; }
        public string ValidUpto { get; set; }

        public string Generic { get; set; }

        public int Supplierid { get; set; }

        public string SupplierName { get; set; }

        public string BrandName { get; set; }

        public int? PictureId { get; set; }

        public string HeaderName { get; set; }

        public bool GenericVisibilty { get; set; }

        public bool PrincipalVisibilty { get; set; }


        public string AttachmentHeaderName { get; set; }

        public string SearchHeaderName { get; set; }


        public List<ProductCertificateViewModel> ProductsList { get; set; }
    }


    public class ProductCertificateViewModel : ProductViewModel
    {

        public int? SerailNumber { get; set; }


        public string LicenseNumber { get; set; }

        private DateTime? _validupto;

        public DateTime? ValidUpto
        {
            get { return _validupto; }
            set
            {
                SetProperty(ref _validupto, value);
                if (value.HasValue && LifeTime)
                    LifeTime = false;

            }
        }

        public string ValidUptoTextColor { get { return ValidUpto != null ? (ValidUpto.Value.Date.Month >= DateTime.Now.Date.Month && ValidUpto.Value.Date.Month <= DateTime.Now.Date.Month) ? "ThisMonth" : "NextMonth" : ""; } }
        public string ValidUptoText { get { return ValidUpto != null ? ValidUpto.Value.ToString("dd-MM-yyyy") : "LifeTime"; } }
        public bool ValidUptoVisiblity { get; set; }

        public bool LifeTimeVisiblity { get; set; }



        private bool _lifetime;
        public bool LifeTime
        {
            get { return _lifetime; }
            set
            {
                SetProperty(ref _lifetime, value);
                if (value && ValidUpto.HasValue)
                    ValidUpto = null;
            }
        }

        public bool IsCertificate { get; set; }

        public string TechnicalName { get; set; }

        public string PackingSize { get; set; }

        public string UOM { get; set; }

        public string Type { get; set; }
    }

    public class SKURequestApproveModel
    {
        public string DepartName { get; set; }

        public int ProductTypeId { get; set; }

        public int StoreId { get; set; }

        public List<SKURequestApproveProductModel> sKURequestApproves { get; set; }

        public string CategoryName { get; set; }


    }

    public class SKURequestApproveProductModel
    {
        public int ProductId { get; set; }

        public string ProductSKU { get; set; }

        public bool IsEnable { get; set; }

        public string TechnicalName { get; set; }

        public string PackingSize { get; set; }

        public string UOM { get; set; }

        public string Type { get; set; }
    }

    public class UpdateSKVModel
    {
        public string Number { get; set; }

        public string IssueDate { get; set; }
        public string ValidUpto { get; set; }

        public string Generic { get; set; }

        public int Supplierid { get; set; }

        public string SupplierName { get; set; }

        public int? PictureId { get; set; }

        public List<UpdateSKUProduct> UpdateSKUProducts { get; set; }
    }

    public class UpdateSKUProduct
    {
        public int ProductId { get; set; }
        public int SerailNumber { get; set; }
        public string LicenseNumber { get; set; }

        public bool LifeTime { get; set; }
        public DateTime? ValidUpto { get; set; }
    }

    public class NewProductV2:BindableBase
    {
        public int ProductTypeId { get; set; }
        public int ProductId { get; set; }
        public String SKU { get; set; }
        public String Brand { get; set; }
        public String ProductName { get; set; }
      // public String ShortName { get; set; }
        public String TechnicalName { get; set; }
        public String Category { get; set; }
        public String SubCategory { get; set; }
        public String PackingSize { get; set; }
        public String UOM { get; set; }
        public String TradeOrOwn { get; set; }
        public double GST { get; set; }

        private decimal _minMargin;
        public decimal MinMargin 
        {
            get { return _minMargin; }

            set { SetProperty(ref _minMargin, value);  }
        
        }
        public string HSN { get; set; }
        public string Warranty { get; set; }

        private string _remarks;
        public string Remarks
        {
            get { return _remarks; }

            set { SetProperty(ref _remarks, value); }

        }
    }


}
