using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.ProductTypes;
using FalcaPOS.Entites.Sku;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.Sku.ViewModels
{
    public class ViewSKUViewModel : BindableBase
    {

        public DelegateCommand<object> ProductCertificateCheckedCommand { get; private set; }


        public DelegateCommand<object> ProductViewCommand { get; private set; }

        public DelegateCommand ClearDepartCommand { get; private set; }


        private readonly Logger _logger;

        private readonly ISkuService _skuService;

        private readonly INotificationService _notificationService;

        public DelegateCommand<int?> GetPDFCommand { get; private set; }

        private readonly IInvoiceFileService _invoiceFileService;

        private readonly ProgressService _progressService;

        private readonly IProductTypeService _productTypeService;

        public DelegateCommand<object> SelectionChangeCommand { get; private set; }

        public DelegateCommand<object> RefreshCommand { get; private set; }

        public DelegateCommand<object> SearchCommand { get; private set; }

        public ViewSKUViewModel(IStoreService storeService, ProgressService progressService, IInvoiceFileService invoiceFileService, INotificationService notificationService, ISkuService skuService, Logger logger, IProductTypeService productTypeService)
        {

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _skuService = skuService ?? throw new ArgumentNullException(nameof(skuService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _invoiceFileService = invoiceFileService ?? throw new ArgumentNullException(nameof(invoiceFileService));

            ProductCertificateCheckedCommand = new DelegateCommand<object>(SearchProductFilterEvent);

            ProductViewCommand = new DelegateCommand<object>(ProcutCommand);


            GetPDFCommand = new DelegateCommand<int?>(GetPdfDownload);

            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));

            DepartmentList = new List<ProductCertificateDeprtmentList>();

            ExpiryDurationList = new List<ProductCertificateDeprtmentList>();

            _productTypeService = productTypeService ?? throw new ArgumentNullException(nameof(productTypeService));

            DepartmentList.AddRange(GetCertificateProduct().Select(x => new ProductCertificateDeprtmentList()
            {
                Name = x
            }));

            ExpiryDurationList.AddRange(new List<String>() { "EXPIRED", "THIS WEEK EXPIRY", "NEXT 15 DAYS EXPIRY", "NEXT 30 DAYS EXPIRY", "ALL" }.Select(x => new ProductCertificateDeprtmentList()
            {
                Name = x
            }));

            LoadCategory();

            SelectionChangeCommand = new DelegateCommand<object>(CategoryChange);

            RefreshCommand = new DelegateCommand<object>(Refresh);

            SearchCommand = new DelegateCommand<object>(SearchProductFilterEvent);


        }

        private async void SearchProductFilterEvent(object obj)
        {
            try
            {

                var name = (ProductCertificateDeprtmentList)obj;

                
                if (AppConstants.USER_ROLES[0] == AppConstants.ROLE_STORE_PERSON.ToString())
                    ViewSkuSearch.Storeid = AppConstants.LoggedInStoreInfo.StoreId;
                else
                    ViewSkuSearch.Storeid = 0;

                if (SelectedCategory == null) {
                    _notificationService.ShowMessage("Please select any one Category", Common.NotificationType.Error);

                    SKUProductView = null;

                    Number = null;

                    SKUView = null;

                    return;
                }

                if (SelectedSubCategory==null)
                {
                    _notificationService.ShowMessage("Please select any one Sub Category", Common.NotificationType.Error);

                    SKUProductView = null;

                    Number = null;

                    SKUView = null;

                    return;
                }
                headerChange(name.Name);

                if (ViewSkuSearch.Expired == false && ViewSkuSearch.ThisWeek == false && ViewSkuSearch.Next15Days == false && ViewSkuSearch.Next30Days == false && ViewSkuSearch.All == false)
                {
                    _notificationService.ShowMessage("Please select any one duration", Common.NotificationType.Error);

                    SKUProductView = null;

                    Number = null;

                    SKUView = null;

                    return;
                }
                _viewSkuSearch.SubCategoryId = SelectedSubCategory.Id;

                await _progressService.StartProgressAsync();

                await Task.Run(async () =>
                {

                    var _result = await _skuService.GetViewSKU(ViewSkuSearch);

                    if (_result != null && _result.IsSuccess)
                    {
                        var result = _result.Data.ViewSKUModelLists.ToList();

                        var _groupResult = (from store in result
                                            group store by new { store.StoreId, store.BrandId } into g
                                            select new
                                            {
                                                storeid = g.Key.StoreId,
                                                brandid = g.Key.BrandId,
                                                list = g.FirstOrDefault()
                                            });
                        if (_groupResult != null)
                        {
                            List<SKUvm> sKUvms = new List<SKUvm>();

                            foreach (var item in _groupResult)
                            {
                                sKUvms.Add(item.list);
                            }

                            SKUView = sKUvms;

                            SKUProductView = null;

                            Number = null;

                        }


                       
                    }
                    else
                    {
                        _notificationService.ShowMessage(_result.Error, Common.NotificationType.Error);

                        SKUView = null;

                        SKUProductView = null;

                        Number = null;

                       
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

        private async void ProcutCommand(object obj)
        {

            try
            {
                var _product = ((SKUvm)obj);

                ViewSKUProductSearch viewSKUProductSearch = new ViewSKUProductSearch()
                {
                    Number = _product.Number,
                    Storeid = _product.StoreId,
                    All = ViewSkuSearch.All,
                    Expired = ViewSkuSearch.Expired,
                    Next15Days = ViewSkuSearch.Next15Days,
                    Next30Days = ViewSkuSearch.Next30Days,
                    ThisWeek = ViewSkuSearch.ThisWeek,


                };





                var _result = await _skuService.GetViewSKUProduct(viewSKUProductSearch);

                if (_result != null && _result.IsSuccess)
                {
                    var result = _result.Data.ViewSKUProductLists.ToList();

                    SKUProductView = result;
                }
                else
                {
                    _notificationService.ShowMessage("No Record Found", Common.NotificationType.Error);
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private void headerChange(string ProductType) {

            switch (ProductType.ToLower()) {
              
                case "expired":
                    if (ViewSkuSearch.Expired == false)
                        ViewSkuSearch.Expired = true;
                    else
                        ViewSkuSearch.Expired = false;
                    break;
                case "this week expiry":
                    if (ViewSkuSearch.ThisWeek == false)
                        ViewSkuSearch.ThisWeek = true;
                    else
                        ViewSkuSearch.ThisWeek = false;
                    break;
                case "next 15 days expiry":
                    if (ViewSkuSearch.Next15Days == false)
                        ViewSkuSearch.Next15Days = true;
                    else
                        ViewSkuSearch.Next15Days = false;
                    break;
                case "next 30 days expiry":
                    if (ViewSkuSearch.Next30Days == false)
                        ViewSkuSearch.Next30Days = true;
                    else
                        ViewSkuSearch.Next30Days = false;
                    break;
                case "all":
                    if (ViewSkuSearch.All == false)
                        ViewSkuSearch.All = true;
                    else
                        ViewSkuSearch.All = false;
                    break;

                default:
                    break;

            }
        }


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

        private List<SKUvm> _sKUView;

        public List<SKUvm> SKUView
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

        public async void CategoryChange(object obj) {
            try {

                var _viewModel = (ViewSKUViewModel)obj;
                if (_viewModel != null && _viewModel.SelectedCategory != null) {
                    await Task.Run(async () => {
                        var _result = await _productTypeService.GetSubCategory(_viewModel.SelectedCategory.Id);

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
                SelectedDuration = null;
            }
            catch (Exception _ex) {
                _logger.LogError(_ex.Message);
            }
        }


    }

    public class ProductCertificateDeprtmentList : BindableBase
    {
        private String _name;

        public String Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private bool _isChecked;

        public bool IsChecked
        {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value);
        }


       
    }

}
