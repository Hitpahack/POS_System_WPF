using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Manufacturers;
using FalcaPOS.Entites.ProductTypes;
using FalcaPOS.Entites.Sku;
using FalcaPOS.Entites.Stores;
using FalcaPOS.Sku.View;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Telerik.Windows.Documents.Spreadsheet.Expressions.Functions;

namespace FalcaPOS.Sku.ViewModels
{
    public class ViewSKUViewModel : BindableBase
    {

        public DelegateCommand<object> ProductCertificateCheckedCommand { get; private set; }
        public DelegateCommand<object> AddNewProductCertificateCommand { get; private set; }


        public DelegateCommand<object> ProductViewCommand { get; private set; }

        public DelegateCommand ClearDepartCommand { get; private set; }


        private readonly Logger _logger;

        private readonly ISkuService _skuService;

        private readonly INotificationService _notificationService;
        private readonly IStoreService _storeService;

        public DelegateCommand<int?> GetPDFCommand { get; private set; }

        private readonly IInvoiceFileService _invoiceFileService;

        private readonly ProgressService _progressService;

        private readonly IProductTypeService _productTypeService;

        public DelegateCommand<object> SelectionChangeCommand { get; private set; }
        private readonly IEventAggregator _eventAggregator;
        public DelegateCommand<object> ProductCertificateSubcategoryChangeEvent { get; private set; }

        public DelegateCommand<object> RefreshCommand { get; private set; }

        public DelegateCommand<object> SearchCommand { get; private set; }

        public ViewSKUViewModel(IStoreService storeService, ProgressService progressService, IInvoiceFileService invoiceFileService, INotificationService notificationService, ISkuService skuService, Logger logger, IProductTypeService productTypeService,IEventAggregator eventAggregator)
        {

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _skuService = skuService ?? throw new ArgumentNullException(nameof(skuService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _invoiceFileService = invoiceFileService ?? throw new ArgumentNullException(nameof(invoiceFileService));
            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            ProductCertificateCheckedCommand = new DelegateCommand<object>(SearchProductFilterEvent);

            _eventAggregator = eventAggregator;

            GetPDFCommand = new DelegateCommand<int?>(GetPdfDownload);

            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));

            DepartmentList = new List<ProductCertificateDeprtmentList>();

            ExpiryDurationList = new List<ProductCertificateDeprtmentList>();

            _productTypeService = productTypeService ?? throw new ArgumentNullException(nameof(productTypeService));

           

            ExpiryDurationList.AddRange(new List<String>() { "EXPIRED", "NO CERTIFICATE", "VALID CERTIFICATE" }.Select(x => new ProductCertificateDeprtmentList()
            {
                Name = x
            }));
            LoadStores();
            LoadCategory();
            

            SelectionChangeCommand = new DelegateCommand<object>(CategoryChange);
            ProductCertificateSubcategoryChangeEvent = new DelegateCommand<object>(SubCategoryChange);

            RefreshCommand = new DelegateCommand<object>(Refresh);

            SearchCommand = new DelegateCommand<object>(SearchProductFilterEvent);
            AddNewProductCertificateCommand = new DelegateCommand<object>(AddNewProductCertificatePopup);

            GlobalStore = (AppConstants.USER_ROLES[0] == AppConstants.ROLE_STORE_PERSON || AppConstants.USER_ROLES[0] == AppConstants.ROLE_BACKEND) ? false : true;


            _eventAggregator.GetEvent<ProductCertificateRefreshEvent>().Subscribe(() =>
            {
                SearchProductFilterEvent(new object());

            });
            SKUView = new ObservableCollection<ProductCertificateView>();

        }

        private async void AddNewProductCertificatePopup(object obj)
        {
            try
            {
                if (obj is ProductCertificateView)
                {
                    var _currentRow = (ProductCertificateView)obj;
                    AddNewProductCertificate addnewproductcertificate = new AddNewProductCertificate();
                    var _dataContext =(AddNewProductCertificateViewModel) addnewproductcertificate.DataContext;
                    if (_dataContext != null)
                    {
                        _dataContext.Brand =_currentRow.Brand;
                        _dataContext.SubcategoryId = _currentRow.SubCategoryId;
                        _dataContext.BrandId = _currentRow.BrandId;
                        _dataContext.StoreId = _currentRow.StoreId;
                        await DialogHost.Show(addnewproductcertificate, "RootDialog", AddNewProductCertificateEventClosing);
                    }
                    
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private void AddNewProductCertificateEventClosing(object sender, DialogClosingEventArgs eventArgs)
        {
           
        }

        private async void LoadStores()
        {
            try
            {
                await Task.Run(async () => {
                    var _result = await _storeService.GetStores();

                    Application.Current.Dispatcher.Invoke(() => {
                        if (_result != null)
                            StoreList = new ObservableCollection<Store>(_result);
                        else
                            StoreList = new ObservableCollection<Store>();
                    });

                });
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private async void SubCategoryChange(object obj)
        {
            try
            {
                if (SelectedSubCategory != null)
                    await Task.Run(async () =>
                    {
                        var _result = await _productTypeService.GetProductTypeManufacturers(SelectedSubCategory.Id);
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (_result != null)
                            {
                                Brand = new ObservableCollection<Manufacturer>(_result.OrderBy(manufacturer => manufacturer.Name));

                              
                            }
                            else
                                Brand = new ObservableCollection<Manufacturer>();
                        });

                    });
            }

            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private async void SearchProductFilterEvent(object obj)
        {
            try
            {

                if(!IsExpired && !IsNoProductCertificate && !IsValidCertificate)
                {
                    _notificationService.ShowMessage("Please select a type",NotificationType.Warning);
                    return;
                }
                if (IsNoProductCertificate)
                {
                    if (SelectedCategory == null)
                    {
                        _notificationService.ShowMessage("Please select a category", NotificationType.Warning);
                        return;
                    }
                    if (SelectedSubCategory == null)
                    {
                        _notificationService.ShowMessage("Please select a subcategory", NotificationType.Warning);
                        return;
                    }
                    if (SelectedBrand == null)
                    {
                        _notificationService.ShowMessage("Please select a brand", NotificationType.Warning);
                        return;
                    }
                }


                if (AppConstants.USER_ROLES[0] == AppConstants.ROLE_STORE_PERSON.ToString())
                    ViewSkuSearch.StoreId = AppConstants.LoggedInStoreInfo.StoreId;
                else
                    ViewSkuSearch.StoreId = SelectedStore?.StoreId;

                await _progressService.StartProgressAsync();

                await Task.Run(async () =>
                {
                    var _params = new ViewSKUSearch()
                    {
                        CategoryId = SelectedCategory?.Id,
                        SubCategoryId = SelectedSubCategory?.Id,
                        BrandId = SelectedBrand?.ManufacturerId,
                        StoreId = ViewSkuSearch.StoreId,
                        Expired = IsExpired,
                        NonExpired = IsValidCertificate,
                        NoCertificate = IsNoProductCertificate,

                    };
                    var _result = await _skuService.GetViewSKU(_params);
                    if (_result != null && _result.IsSuccess && _result.Data.Count>0)
                    {
                        bool _editActionEnable = false;
                        _editActionEnable = GlobalStore;
                        SKUView = new ObservableCollection<ProductCertificateView>(_result.Data);
                        SKUView.Select(x => { x.NoCertificateGridnoDownload = !_params.NoCertificate; x.ExpiredGrid = IsExpired; x.ValidCertificateGrid = IsValidCertificate; x.IsEdit = _editActionEnable; return x; }).ToList();
                    }
                    else
                    {
                        SKUView = new ObservableCollection<ProductCertificateView>();
                        _notificationService.ShowMessage("No records found", NotificationType.Success);
                        return;
                    }
                });


            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);

            }
            finally
            {

                await _progressService.StopProgressAsync();
            }
        }

        //private async void ProcutCommand(object obj)
        //{

        //    try
        //    {
        //        var _product = ((SKUvm)obj);

        //        ViewSKUProductSearch viewSKUProductSearch = new ViewSKUProductSearch()
        //        {
        //            Number = _product.Number,
        //            Storeid = _product.StoreId,
        //        };

        //        var _result = await _skuService.GetViewSKUProduct(viewSKUProductSearch);

        //        if (_result != null && _result.IsSuccess)
        //        {
        //            var result = _result.Data.ViewSKUProductLists.ToList();

        //            SKUProductView = result;
        //        }
        //        else
        //        {
        //            _notificationService.ShowMessage("No Record Found", Common.NotificationType.Error);
        //        }

        //    }
        //    catch (Exception _ex)
        //    {
        //        _logger.LogError(_ex.Message);
        //    }
        //}

       
        private async void GetPdfDownload(int? fileId)
        {
            try
            {
                if (fileId != null && fileId.Value > 0)
                {
                    var _result = await _invoiceFileService.DownloadFile(fileId.Value);

                    if (_result != null)
                    {
                        if (_result != null && _result.IsSuccess && _result.Data != null && _result.Data.FileStream != null)
                        {
                            SaveFileDialog sd = new SaveFileDialog
                            {
                                CreatePrompt = true,
                                OverwritePrompt = true,
                                DefaultExt = "zip",
                            };
                            var _saveFile = sd.ShowDialog();

                            if (_saveFile == true && sd.FileName.IsValidString())
                            {
                                sd.FileName = Path.ChangeExtension(sd.FileName, "zip");

                                File.WriteAllBytes(sd.FileName, _result.Data.FileStream);
                            }
                        }
                        else
                        {
                            _notificationService.ShowMessage(_result?.Error ?? "An error occurred, try again", Common.NotificationType.Error);
                        }
                    }
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);

            }
        }


        private List<ProductCertificateDeprtmentList> _departmentList;
        public List<ProductCertificateDeprtmentList> DepartmentList
        {
            get { return _departmentList; }
            set { SetProperty(ref _departmentList, value); }
        }

        private List<ProductCertificateDeprtmentList> _expiryDurationList;
        public List<ProductCertificateDeprtmentList> ExpiryDurationList
        {
            get { return _expiryDurationList; }
            set { SetProperty(ref _expiryDurationList, value); }
        }





        private List<string> GetCertificateProduct()
        {
            if (ApplicationSettings.CategoryCertificate != null && ApplicationSettings.CategoryCertificate.Any())
            {
                return new List<string>(ApplicationSettings.CategoryCertificate).Select(x => x.ToUpper()).ToList();

            }
            return null;

        }

        private ObservableCollection<ProductCertificateView> _sKUView;

        public  ObservableCollection<ProductCertificateView> SKUView
        {
            get { return _sKUView; }
            set { SetProperty(ref _sKUView, value); }
        }


        private List<ViewSKUProductVm> _sKUProductView;

        public List<ViewSKUProductVm> SKUProductView
        {
            get => _sKUProductView;
            set => SetProperty(ref _sKUProductView, value);
        }


        private ViewSKUSearch _viewSkuSearch = new ViewSKUSearch();

        public ViewSKUSearch ViewSkuSearch
        {
            get => _viewSkuSearch;
            set => SetProperty(ref _viewSkuSearch, value);
        }

        private string _number;

        public string Number
        {
            get { return _number; }
            set { SetProperty(ref _number, value); }
        }


        private bool _isExpired;
        private bool _isNoProductCertificate;
        private bool _isValidcertificate;

        public bool IsExpired
        {
            get { return _isExpired; }
            set { SetProperty(ref _isExpired, value);
                if (value)
                {
                    IsNoProductCertificate = false;
                    IsValidCertificate = false;
                }
            }
        }
        

        public bool IsNoProductCertificate
        {
            get { return _isNoProductCertificate; }
            set { SetProperty(ref _isNoProductCertificate, value);
                if (value)
                {
                    IsValidCertificate = false;
                    IsExpired = false;
                }
            }
        }

        public bool IsValidCertificate
        {
            get { return _isValidcertificate; }
            set { SetProperty(ref _isValidcertificate, value);
                if (value)
                {
                    IsNoProductCertificate = false;
                    IsExpired = false;
                }
             }
        }



        private async void LoadCategory() {

            try {
                await Task.Run(async () => {
                    var _result = await _productTypeService.GetAllCategory();

                    Application.Current.Dispatcher.Invoke(() => {
                        if (_result != null && _result.IsSuccess)
                            CategoryList = new ObservableCollection<CategoryModel>(_result.Data.Where(x=>x.IsCertificate).ToList());
                        else
                            CategoryList = new ObservableCollection<CategoryModel>();
                    });

                });
            }
            catch (Exception _ex) {
                _logger.LogError(_ex.Message);
            }
        }

        private ObservableCollection<CategoryModel> _categoryList = new ObservableCollection<CategoryModel>();

        public ObservableCollection<CategoryModel> CategoryList {
            get => _categoryList;
            set => SetProperty(ref _categoryList, value);
        }

        private ObservableCollection<SubCategoryModel> _subCategory;
        public ObservableCollection<SubCategoryModel> SubCategory {
            get => _subCategory;
            set => SetProperty(ref _subCategory, value);
        }

        private SubCategoryModel _selectedSubCategory;
        public SubCategoryModel SelectedSubCategory {
            get => _selectedSubCategory;
            set => SetProperty(ref _selectedSubCategory, value);
        }


        private ObservableCollection<Manufacturer> _brand;
        public ObservableCollection<Manufacturer> Brand
        {
            get => _brand;
            set => SetProperty(ref _brand, value);
        }

        private Manufacturer _selectedbrand;
        public Manufacturer SelectedBrand
        {
            get => _selectedbrand;
            set => SetProperty(ref _selectedbrand, value);
        }


        private ObservableCollection<Store> _store;
        public ObservableCollection<Store> StoreList
        {
            get => _store;
            set => SetProperty(ref _store, value);
        }

        private Store _selectedstore;
        public Store SelectedStore
        {
            get => _selectedstore;
            set => SetProperty(ref _selectedstore, value);
        }

        private bool _GlobalStore;
        public bool GlobalStore
        {
            get => _GlobalStore;
            set => SetProperty(ref _GlobalStore, value);
        }



        public async void CategoryChange(object obj) {
            try {
                Brand = null;
                if (SelectedCategory != null) {
                    await Task.Run(async () => {
                        var _result = await _productTypeService.GetSubCategory(SelectedCategory.Id);
                        Application.Current.Dispatcher.Invoke(() => {
                            if (_result != null && _result.IsSuccess) {
                                SubCategory = new ObservableCollection<SubCategoryModel>(_result.Data);
                            }
                            else
                                SubCategory = new ObservableCollection<SubCategoryModel>();
                        });

                    });
                }

            }
            catch (Exception _ex) {
                _logger.LogError(_ex.Message);
            }
        }

        private CategoryModel _selectedCategory;

        public CategoryModel SelectedCategory {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        private ProductCertificateDeprtmentList _duration;

        public ProductCertificateDeprtmentList SelectedDuration {
            get { return _duration; }
            set { SetProperty(ref _duration, value); }
        }

        public void Refresh(object obj) {
            try {

                LoadCategory();
                SelectedCategory = null;
                SelectedSubCategory = null;
                SelectedBrand  = null;
                SelectedStore  = null;
                SelectedDuration = null;
                IsExpired = false;
                IsNoProductCertificate = false;
                IsValidCertificate = false;
                SKUView.Clear();
            }
            catch (Exception _ex) {
                _logger.LogError(_ex.Message);
            }
        }


    }

    

}
