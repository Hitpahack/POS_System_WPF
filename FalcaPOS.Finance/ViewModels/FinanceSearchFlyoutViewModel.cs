using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Finance;
using FalcaPOS.Entites.ProductTypes;
using FalcaPOS.Entites.Sales;
using FalcaPOS.Entites.Stores;
using MahApps.Metro.Controls;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.Finance.ViewModels
{
    public class FinanceSearchFlyoutViewModel : BindableBase
    {

        private readonly IEventAggregator _eventAggregator;

        private readonly IStoreService _storeService;

        private readonly INotificationService _notificationService;

        private readonly IProductTypeService _productTypeService;

        private readonly Logger _logger;

        public DelegateCommand SearchFlyOutFinanceCommand { get; private set; }
        public DelegateCommand<object> RefreshFlyOutFinanceCommand { get; private set; }
        public DelegateCommand CloseFlyOutFinanceCommand { get; private set; }

        public DelegateCommand<object> CategorySelectionChangeCommand { get; private set; }

        public FinanceSearchFlyoutViewModel(IEventAggregator eventAggregator, IStoreService storeService, INotificationService notificationService, IProductTypeService productTypeService, Logger logger)
        {
            Width = GridLength.Auto;

            Height = new GridLength(1200);

            Position = Position.Top;

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _eventAggregator.GetEvent<SearchFinanceFlyoutEvent>().Subscribe(OpenFlyOut);

            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            SearchFlyOutFinanceCommand = new DelegateCommand(SearchFinanceFlyout);

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            RefreshFlyOutFinanceCommand = new DelegateCommand<object>(ResetFormData);

            CloseFlyOutFinanceCommand = new DelegateCommand(CloseFinanceFlyOut);

            _productTypeService = productTypeService ?? throw new ArgumentNullException(nameof(productTypeService));


            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _eventAggregator.GetEvent<SignalRProductTypeAddedEvent>().Subscribe(NewProductTypeAdded, ThreadOption.PublisherThread);

            _eventAggregator.GetEvent<SignalRProductTypeEnableDisableEvent>().Subscribe(ProductTypeEnabledDisabled, ThreadOption.PublisherThread);

            CategorySelectionChangeCommand = new DelegateCommand<object>(CategoryChange);


            LoadProductTypesAsync();
            if ((AppConstants.USER_ROLES[0] == AppConstants.ROLE_TERRITORY_MANAGER) || (AppConstants.USER_ROLES[0] == AppConstants.ROLE_REGIONAL_MANAGER))
                LoadStoresByUser();
            else     
            LoadStoresAsync();
        }
        

            private void ResetFormData(object obj)
            {
            FromDate = null;
            ToDate = null;
            InvoiceNumber = null;
            SelectedStore = null;
            SelectedProductType = null; 
            SelectedSubCategory = null;
            SubCategory.Clear();
            SelectedCategory=null;
            }
    
        private void ProductTypeEnabledDisabled(object obj)
        {
            try
            {
                if (obj is ProductType _pType)
                {
                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        if (!_pType.Isenabled)
                        {
                            //disabled remove from dropdown.
                            var _existtingProductType = ProductTypes?.FirstOrDefault(x => x.ProductTypeId == _pType.ProductTypeId);

                            if (_existtingProductType != null)
                            {
                                ProductTypes?.Remove(_existtingProductType);

                                _logger.LogInformation($"Product type was disabled remove from finance search flyout {_existtingProductType.Name} ");
                            }
                        }
                        else
                        {
                            if (ProductTypes == null)
                            {
                                ProductTypes = new ObservableCollection<ProductType>();
                            }
                            var _existtingProductType = ProductTypes?.FirstOrDefault(x => x.ProductTypeId == _pType.ProductTypeId);

                            //remove
                            if (_existtingProductType != null)
                            {
                                ProductTypes?.Remove(_existtingProductType);
                            }
                            //add
                            ProductTypes.Add(_pType);
                            _logger.LogInformation($"Adding product type to list finance search flyout dept name {_pType.Name}");
                            //sort
                            ProductTypes = new ObservableCollection<ProductType>(ProductTypes.OrderBy(x => x.Name));
                        }


                    });

                }
            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in stock flyout search product type disabled", _ex);
            }
        }
        private void NewProductTypeAdded(object obj)
        {
            try
            {

                //Hub event new product type was added
                //context maybe in worker thread . switch to UI thread.
                if (obj is ProductType _ptype)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (ProductTypes == null)
                        {
                            _logger.LogInformation("Product type are empty in fin search flyout");
                            ProductTypes = new ObservableCollection<ProductType> { _ptype };
                            return;
                        }
                        _logger.LogInformation($"New product type was added from finace flyout {_ptype.Name}");

                        if (ProductTypes.Any(x => x.ProductTypeId == _ptype.ProductTypeId))
                        {
                            _logger.LogInformation("Product type id already contained in list finance search flyout");

                            return;
                        }

                        ProductTypes.Add(_ptype);
                        //order by names
                        ProductTypes = new ObservableCollection<ProductType>(ProductTypes.OrderBy(x => x.Name));
                    });
                }
            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in adding new product type in stock search fly out", _ex);
            }
        }
        private ObservableCollection<ProductType> _productTypes = new ObservableCollection<ProductType>();

        public ObservableCollection<ProductType> ProductTypes
        {
            get { return _productTypes; }
            set { SetProperty(ref _productTypes, value); }
        }



        private ObservableCollection<CategoryModel> _categoryList = new ObservableCollection<CategoryModel>();

        public ObservableCollection<CategoryModel> CategoryList
        {
            get => _categoryList;
            set => SetProperty(ref _categoryList, value);
        }

        private async void LoadProductTypesAsync()
        {
            try
            {
                await Task.Run(async () =>
                {
                    await Task.Run(async () =>
                    {
                        var _result = await _productTypeService.GetAllCategory();

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (_result != null && _result.IsSuccess)
                                CategoryList = new ObservableCollection<CategoryModel>(_result.Data);
                            else
                                CategoryList = new ObservableCollection<CategoryModel>();
                        });

                    });

                });
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in loading product types in add new product", _ex);
            }

        }

        private void CloseFinanceFlyOut()
        {
            IsOpen = false;
            ResetFormData();
        }

        private void OpenFlyOut()
        {
            IsOpen = true;

            //ResetFormData();

        }

        private void ResetFormData()
        {
            FromDate = null;
            ToDate = null;
            InvoiceNumber = null;
            SelectedStore = null;
            SelectedProductType = null;
        }

        private void SearchFinanceFlyout()
        {
            //validate ..
            if (FromDate == null && ToDate == null && SelectedStore == null && string.IsNullOrWhiteSpace(InvoiceNumber) && SelectedCategory == null && SelectedSubCategory == null)
            {
                _notificationService.ShowMessage("Search cannot be empty", Common.NotificationType.Error);

                return;
            }

            if (SelectedStore != null && FromDate == null)
            {
                //check for from data mandate

                _notificationService.ShowMessage("Please select from date.", Common.NotificationType.Error);

                return;
            }


            if (FromDate == null && ToDate != null)
            {
                //check for from data mandate

                _notificationService.ShowMessage("Please select from date.", Common.NotificationType.Error);

                return;
            }

            if (FromDate != null && ToDate != null)
            {
                //check for from  date cannot be more than to data


                if (FromDate > ToDate)
                {
                    _notificationService.ShowMessage("From date should be less than or equal to To date", Common.NotificationType.Error);

                    return;
                }
            }


            //case 3 if from date is there then to date is required.. Bug 855

            if (FromDate != null && ToDate == null)
            {
                _notificationService.ShowMessage("Please select to date", Common.NotificationType.Error);

                return;
            }


            _eventAggregator.GetEvent<SearchFinanceEvent>()
                .Publish(new FinanceSearch
                {
                    FromDate = FromDate,
                    InvoiceNumber = InvoiceNumber,
                    ToDate = ToDate,
                    StoreId = SelectedStore?.StoreId,
                    StoreName = SelectedStore?.Name,
                    ProductTypeId = SelectedSubCategory?.Id,
                    CategoryId = SelectedCategory?.Id
                });

            IsOpen = false;

            //ResetFormData();

        }

        private string _invoiceNumber;
        public string InvoiceNumber
        {
            get { return _invoiceNumber; }
            set { SetProperty(ref _invoiceNumber, value); }
        }

        private ProductType _selectedProductType;
        public ProductType SelectedProductType
        {
            get { return _selectedProductType; }
            set { SetProperty(ref _selectedProductType, value); }
        }

        private ObservableCollection<Store> _stores;
        public ObservableCollection<Store> Stores
        {
            get { return _stores; }
            set { SetProperty(ref _stores, value); }
        }
        private async void LoadStoresAsync()
        {
            try
            {
                await Task.Run(async () =>
                {

                    var _result = await _storeService.GetStores();

                    if (_result != null && _result.Count() > 0)
                    {
                        Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            Stores = new ObservableCollection<Store>(_result.Where(x => x.Parent_ref == null));

                            Stores.Insert(0, new Store { StoreId = 0, Name = "All" });
                        });
                    }

                });

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private async void LoadStoresByUser() {
            try {
                await Task.Run(async () => {
                    var _result = await _storeService.GetStoreDetailsbyuser(AppConstants.UserId, AppConstants.USER_ROLES[0]);
                    Application.Current?.Dispatcher?.Invoke(() => {
                        Stores = new ObservableCollection<Store>(_result);
                    });
               
                });

            }
            catch (Exception _ex) {
                _logger.LogError(_ex.Message);
            }
        }

        private string header;
        public string Header
        {
            get { return this.header; }
            set { SetProperty(ref this.header, value); }
        }

        private bool isOpen;
        public bool IsOpen
        {
            get { return this.isOpen; }
            set { SetProperty(ref this.isOpen, value); }
        }

        private Position position;
        public Position Position
        {
            get { return this.position; }
            set { SetProperty(ref this.position, value); }

        }



        private GridLength _width;
        public GridLength Width
        {
            get { return _width; }
            set { SetProperty(ref _width, value); }

        }


        private GridLength _height;
        public GridLength Height
        {
            get { return _height; }
            set { SetProperty(ref _height, value); }

        }

        private Store _selectedStore;
        public Store SelectedStore
        {
            get { return _selectedStore; }
            set { SetProperty(ref _selectedStore, value); }
        }

        private DateTime? _fromDate;
        public DateTime? FromDate
        {
            get { return _fromDate; }
            set { SetProperty(ref _fromDate, value); }
        }

        private DateTime? _toDate;
        public DateTime? ToDate
        {
            get { return _toDate; }
            set { SetProperty(ref _toDate, value); }
        }

        private CategoryModel _selectedCategory;


        public CategoryModel SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                SetProperty(ref _selectedCategory, value);

            }
        }

        private ObservableCollection<SubCategoryModel> _subCategory;
        public ObservableCollection<SubCategoryModel> SubCategory
        {
            get => _subCategory;
            set => SetProperty(ref _subCategory, value);
        }

        private SubCategoryModel _selectedSubCategory;
        public SubCategoryModel SelectedSubCategory
        {
            get => _selectedSubCategory;
            set => SetProperty(ref _selectedSubCategory, value);
        }

        public async void CategoryChange(object obj)
        {
            try
            {

                var _viewModel = (FinanceSearchFlyoutViewModel)obj;
                if (_viewModel != null && _viewModel.SelectedCategory != null)
                {
                    await Task.Run(async () =>
                    {
                        var _result = await _productTypeService.GetSubCategory(_viewModel.SelectedCategory.Id);

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (_result != null && _result.IsSuccess)
                            {
                                SubCategory = new ObservableCollection<SubCategoryModel>(_result.Data);
                            }
                            else
                                SubCategory = new ObservableCollection<SubCategoryModel>();


                        });

                    });
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }


    }
}
