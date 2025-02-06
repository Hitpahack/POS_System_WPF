using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Stores;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Threading;
using System.Windows;
using Prism.Events;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common;
using FalcaPOS.Entites.Products;
using FalcaPOS.Entites.ProductTypes;

namespace FalcaPOS.Store.ViewModels
{
    public class EditStoreLicenseViewModel : BindableBase
    {
        private readonly IProductTypeService _productTypeService;

        private readonly Logger _logger;

        private CancellationTokenSource _cancellationTokenSource;
        public Guid StoreLicenseGUIDId { get; set; }

        public EditStoreLicenseViewModel(IProductTypeService productTypeService, Logger logger, StoreLicense StoreLicense)
        {
            StoreLicenseGUIDId = Guid.NewGuid();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _productTypeService = productTypeService ?? throw new ArgumentNullException(nameof(productTypeService));
            if (StoreLicense != null) {
                WholesaleLicense = StoreLicense.WholesaleLicense;
                NormalLicense = StoreLicense.NormalLicense;
                CategoryName = StoreLicense.CategoryName;
            }
          
            LoadProductTypes();
        }
        private async void LoadProductTypes()
        {

            try
            {
                await Task.Run(async () =>
                {
                    var _result = await _productTypeService.GetAllLicenseCategory();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (_result != null && _result.IsSuccess)
                        {
                            _result.Data.Select(x => { x.IsCertificate = true; return x; }).ToList();
                            CategoryList = new ObservableCollection<CategoryModel>(_result.Data);
                           if(CategoryName!=null)
                            SelectedCategory = CategoryList.FirstOrDefault(x=>x.CategoryName==CategoryName);
                        }

                        else
                            CategoryList = new ObservableCollection<CategoryModel>();
                    });

                });
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in loading CategoryList in Store License", _ex);
            }
        }
        private ObservableCollection<CategoryModel> _categoryList;
        public ObservableCollection<CategoryModel> CategoryList
        {
            get => _categoryList;
            set => SetProperty(ref _categoryList, value);
        }

        private bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set
            {
                SetProperty(ref _hasError, value);
            }
        }

        private CategoryModel _selectedCategory;
        public CategoryModel SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);

        }

        private string _categoryName;
        public string CategoryName {
            get => _categoryName;
            set => SetProperty(ref _categoryName, value);
        }

        private string _wholesaleLicense;
        public string WholesaleLicense {
            get => _wholesaleLicense;
            set => SetProperty(ref _wholesaleLicense,value);
        }

        private string _normalLicense;
        public string NormalLicense 
        {
            get => _normalLicense;
            set => SetProperty(ref _normalLicense, value);
        }
    }
}
