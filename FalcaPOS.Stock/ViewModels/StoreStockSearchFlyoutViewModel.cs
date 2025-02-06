using AutoMapper;
using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Attributes;
using FalcaPOS.Entites.Dtos;
using FalcaPOS.Entites.Manufacturers;
using FalcaPOS.Entites.Products;
using FalcaPOS.Entites.ProductTypes;
using FalcaPOS.Entites.Stock;
using FalcaPOS.Entites.Stores;
using MahApps.Metro.Controls;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.Stock.ViewModels
{
    public class StoreStockSearchFlyoutViewModel : BindableBase
    {

        private readonly IProductTypeService productTypeService;

        private readonly IStoreService storeService;

        private readonly IProductService productService;

        private readonly IEventAggregator eventAggregator;

        private readonly Logger logger;

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

        private GridLength width;

        public GridLength Width
        {
            get { return width; }
            set { width = value; }

        }
        private int height;

        public int Height
        {
            get { return height; }
            set { height = value; }

        }
        private StockModelRequest stockModelItem = new StockModelRequest();
        public StockModelRequest StockModelItem
        {
            get
            {
                return stockModelItem;
            }
            set
            {
                SetProperty(ref stockModelItem, value);
            }
        }

        private List<string> reference;
        public List<string> Reference
        {
            get { return reference; }
            set { SetProperty(ref reference, value); }
        }
        public DelegateCommand ProductTypeChange { get; private set; }


        public DelegateCommand ManufacturerChange { get; private set; }

        public DelegateCommand<object> ProductNameChange { get; private set; }

        public DelegateCommand<StockModelRequest> StockSearch { get; private set; }
        public DelegateCommand<StockModelRequest> Reset { get; private set; }

        public DelegateCommand<string> SearchTextChangedCommand { get; private set; }

        private readonly INotificationService _notificationService;

        private readonly IProductService _productService;
        public DelegateCommand SearchProductSelectionChangedCommand { get; private set; }


        public StoreStockSearchFlyoutViewModel(IEventAggregator EventAggregator,
            IProductService ProductService,
            IProductTypeService ProductTypeService,
            IStoreService StoreService,
            Logger Logger, INotificationService notificationService)
        {
            eventAggregator = EventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            logger = Logger ?? throw new ArgumentNullException(nameof(Logger));

            productTypeService = ProductTypeService ?? throw new ArgumentNullException(nameof(ProductTypeService));

            productService = ProductService ?? throw new ArgumentNullException(nameof(ProductService));

            storeService = StoreService ?? throw new ArgumentNullException(nameof(StoreService));

            Width = GridLength.Auto;

            Height = 200;

            Position = Position.Top;

            eventAggregator.GetEvent<StoreSearchFlyout>().Subscribe(HandleFlyOutOpen);

            ProductTypeChange = new DelegateCommand(LoadManufacturer);

            eventAggregator.GetEvent<AddProductTypeEvent>().Subscribe(AddProductType);

            ManufacturerChange = new DelegateCommand(LoadProductName);

            StockSearch = new DelegateCommand<StockModelRequest>(GetStockSearch);

            Reset = new DelegateCommand<StockModelRequest>(ResetStockSerarchInput);

            NameVisibility = Visibility.Collapsed;

            ValueVisibility = Visibility.Collapsed;

            eventAggregator.GetEvent<SignalRProductTypeAddedEvent>().Subscribe(NewProductTypeAdded, ThreadOption.PublisherThread);

            eventAggregator.GetEvent<SignalRProductTypeEnableDisableEvent>().Subscribe(ProductTypeEnabledDisabled, ThreadOption.PublisherThread);

            eventAggregator.GetEvent<SignalRStoreAddedEvent>().Subscribe(NewStoreAdded, ThreadOption.PublisherThread);

            LoadReference();

            LoadProductTypes();

            LoadStores();

            eventAggregator.GetEvent<SignalRProductEnableDisableAddEvent>().Subscribe(LoadProductName);

            SearchTextChangedCommand = new DelegateCommand<string>(SearchTextChanged);

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _productService = productService ?? throw new ArgumentNullException(nameof(productService));

            SearchProductSelectionChangedCommand = new DelegateCommand(SearchSelectionChanged);







        }

        private void NewStoreAdded(object str)
        {
            try
            {
                logger.LogInformation("New Store added  from store stock search  flyout");

                if (AppConstants.USER_ROLES.Contains(AppConstants.ROLE_BACKEND))
                {
                    logger.LogInformation("Skipping store addition for backend user in store stock search flyout");

                    return;
                }

                logger.LogInformation("New Store added  from store stock search  flyout");

                if (str is Store _str)
                {
                    Application.Current?.Dispatcher?.Invoke(delegate
                    {
                        if (Stores == null)
                        {
                            logger.LogInformation("Stores collection is null in store stock search  flyout");

                            Stores = new ObservableCollection<Store> { _str };

                            return;
                        }

                        if (Stores.Any(x => x.Name == _str.Name))
                        {
                            logger.LogInformation($"Store already added in store stock search  flyout");

                            return;
                        }
                        Stores.Add(_str);
                        Stores = new ObservableCollection<Store>(Stores.OrderBy(x => x.Name));
                    });


                }
            }
            catch (Exception _ex)
            {
                logger?.LogError("error in adding new  store to list in purRateFlyout", _ex);
            }

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
                            //sort
                            ProductTypes = new ObservableCollection<ProductType>(ProductTypes.OrderBy(x => x.Name));
                        }


                    });

                }
            }
            catch (Exception _ex)
            {
                logger?.LogError("Error in stock flyout search product type disabled", _ex);
            }
        }

        private void HandleFlyOutOpen(StockFlyout isopen)
        {


            IsOpen = isopen.IsOpen;

            if (isopen.IsOpen)
            {
                StockModelItem.SerialNo = string.Empty;
                StockModelItem.InvoiceToDate = string.Empty;
                StockModelItem.InvoiceFromDate = string.Empty;
                StockModelItem.InvoiceNo = string.Empty;
                StockModelItem.Status = string.Empty;
                StockModelItem.Brand = string.Empty;
                StockModelItem.ProductName = string.Empty;
                StockModelItem.ProductType = string.Empty;
                SelectedStore = null;
                StockModelItem.IsParent = isopen.IsParent;
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
                            logger.LogInformation("Product type are empty in stock search flyout");
                            ProductTypes = new ObservableCollection<ProductType> { _ptype };
                            return;
                        }
                        logger.LogInformation($"New product type was added {_ptype.Name}");

                        if (ProductTypes.Any(x => x.ProductTypeId == _ptype.ProductTypeId))
                        {
                            logger.LogInformation("Product type id already contained in list");

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
                logger?.LogError("Error in adding new product type in stock search fly out", _ex);
            }

        }

        private void AddProductType(object obj)
        {
            if (obj is ProductType producttype)
            {
                ProductTypes?.Add(producttype);
            }

        }

        private async void LoadStores()
        {
            if (AppConstants.USER_ROLES != null && AppConstants.USER_ROLES.Any())
            {
                if (AppConstants.USER_ROLES.Contains("admin") || AppConstants.USER_ROLES.Contains("falcadirector") || AppConstants.USER_ROLES.Contains("finance"))
                {

                    await Task.Run(async () =>
                    {
                        var _result = await storeService.GetStores();
                        if (_result != null)
                        {
                            Stores = new ObservableCollection<Store>(_result);
                            Stores.Insert(0, new Store { StoreId = 0, Name = "All" });
                        }

                    });

                }
                else if (AppConstants.USER_ROLES.Contains("backend"))
                    Stores = new ObservableCollection<Store>(new List<Store>() { AppConstants.LoggedInStoreInfo });
                else if (AppConstants.USER_ROLES.Contains("storeperson") && !StockModelItem.IsParent)
                {
                    await Task.Run(async () =>
                    {
                        var _result = await storeService.GetStores();
                        if (_result != null)
                        {
                            Stores = new ObservableCollection<Store>(_result.Where(x => x.Parent_ref == AppConstants.LoggedInStoreInfo.StoreId).ToList());
                            Stores.Insert(0, new Store { StoreId = 0, Name = "All", IsParent = false, Parent_ref = AppConstants.LoggedInStoreInfo.StoreId });
                        }

                    });
                }
                else if (!StockModelItem.IsParent)
                {
                    await Task.Run(async () =>
                    {
                        var _result = await storeService.GetStores();
                        if (_result != null)
                        {
                            Stores = new ObservableCollection<Store>(_result.Where(x => x.IsParent == false).ToList());
                            Stores.Insert(0, new Store { StoreId = 0, Name = "All", IsParent = false });
                        }

                    });
                }
            }


        }

        public void ResetStockSerarchInput(StockModelRequest item)
        {
            item.SerialNo = string.Empty;
            item.InvoiceToDate = string.Empty;
            item.InvoiceFromDate = string.Empty;
            item.InvoiceNo = string.Empty;
            item.Status = string.Empty;
            item.Brand = string.Empty;
            item.ProductName = string.Empty;
            item.ProductType = string.Empty;
            SelectedStore = null;
            SelectedSearchProduct = null;
            SelectedProductSearch = null;
        }
        public void GetStockSearch(StockModelRequest item)
        {
            if (string.IsNullOrEmpty(item.SerialNo)
                && string.IsNullOrEmpty(item.SupplierName)
                && string.IsNullOrEmpty(item.ProductType)
                && string.IsNullOrEmpty(item.ProductName)
                && string.IsNullOrEmpty(item.Brand)
                && string.IsNullOrEmpty(item.Location)
                && string.IsNullOrEmpty(item.InvoiceNo)
                && string.IsNullOrEmpty(item.Status)
                && string.IsNullOrEmpty(item.InvoiceToDate)
                && string.IsNullOrEmpty(item.InvoiceFromDate)
                && SelectedStore == null && SelectedSearchProduct == null)
            {
                //MessageBox.Show("Select Anything To Search!!!..");
                ShowMessage("Search cannot be empty", NotificationType.Error);
                return;
            }

            if (String.IsNullOrEmpty(item.InvoiceFromDate) && !string.IsNullOrEmpty(item.InvoiceToDate))
            {

                ShowMessage("Please enter from Date", NotificationType.Error);
                return;

            }

            if (!String.IsNullOrEmpty(item.InvoiceFromDate) && !string.IsNullOrEmpty(item.InvoiceToDate))
            {
                DateTime dt1 = Convert.ToDateTime(item.InvoiceFromDate);
                DateTime dt2 = Convert.ToDateTime(item.InvoiceToDate);
                if (dt2 < dt1)
                {
                    ShowMessage("From Date should be less than or equal to To Date", NotificationType.Error);
                    return;
                }
            }



            StockModelRequest modelRequest = new StockModelRequest()
            {
                SerialNo = string.IsNullOrEmpty(item.SerialNo) ? null : item.SerialNo,
                ProductType = SelectedSearchProduct == null ? string.IsNullOrEmpty(item.ProductType) ? null : item.ProductType : SelectedSearchProduct.ProductType.Name,
                ProductName = SelectedSearchProduct == null ? string.IsNullOrEmpty(item.ProductName) ? null : item.ProductName : SelectedSearchProduct.Name,
                Brand = SelectedSearchProduct == null ? string.IsNullOrEmpty(item.Brand) ? null : item.Brand : SelectedSearchProduct.Manufacturer.Name,
                Location = SelectedStore?.Name,//AppConstants.UserName,
                Status = string.IsNullOrEmpty(item.Status) ? null : item.Status,

                InvoiceFromDate = string.IsNullOrEmpty(item.InvoiceFromDate) ? null : item.InvoiceFromDate,
                InvoiceToDate = string.IsNullOrEmpty(item.InvoiceToDate) ? null : item.InvoiceToDate,

                IsParent = StockModelItem.IsParent


            };
            if (SelectedStore != null && modelRequest != null && SelectedStore.StoreId > 0)
            {
                modelRequest.StoreId = SelectedStore.StoreId.ToString();
            }

            eventAggregator.GetEvent<StoreStockSearch>().Publish(modelRequest);

            this.IsOpen = false;
        }

        private void ShowMessage(string msg, NotificationType notificationType) =>
                    eventAggregator.GetEvent<NotifyMessage>().Publish(new Common.Models.ToastMessage
                    {
                        Message = msg,
                        MessageType = notificationType
                    });



        private async void LoadProductName()
        {
            try
            {

                SelectedProduct = null;

                AttributesSelectedList?.Clear();

                ProductDetailsList = null;

                if (SelectedManufacturer == null) return;

                await Task.Run(async () =>
                {

                    var _result = await productService.GetenabledProducts($"SortBy=name&ProducttypeManufacturerId={SelectedManufacturer.ProductTypeManufacturerId}&IncludeAttributes=true");

                    if (_result != null && _result.Any())
                    {
                        ProductDetailsList = new ObservableCollection<ProductDetails>(_result);
                        //SelectedProduct = ProductDetailsList[0];
                    }
                });

            }
            catch (Exception ex)
            {
                logger?.LogError("error in loadproduct", ex);

            }

        }

        public void LoadReference()
        {
            List<string> Referenceitem = new List<string>();
            Referenceitem.Add("Stock");
            Referenceitem.Add("Sold");
            Referenceitem.Add("CreditNote");
            Reference = Referenceitem;
        }

        private async void LoadProductTypes()
        {

            try
            {

                await Task.Run(async () =>
                {
                    var _result = await productTypeService.GetProductTypes("isenabled=true");

                    if (_result != null && _result.Any())
                    {
                        ProductTypes = new ObservableCollection<ProductType>(_result);
                        //SelectedProductType = ProductTypes[0];
                    }
                    else
                        ProductTypes = new ObservableCollection<ProductType>();
                });
            }
            catch (Exception ex)
            {
                logger?.LogError("error in load product type", ex);
            }
        }

        private ObservableCollection<ProductDetails> productDetailsList = new ObservableCollection<ProductDetails>();
        public ObservableCollection<ProductDetails> ProductDetailsList
        {
            get { return productDetailsList; }
            set { SetProperty(ref productDetailsList, value); }
        }


        private ObservableCollection<ProductType> productTypes = new ObservableCollection<ProductType>();
        public ObservableCollection<ProductType> ProductTypes
        {
            get { return productTypes; }
            set { SetProperty(ref productTypes, value); }
        }
        private Manufacturer selectedManufacturer;
        public Manufacturer SelectedManufacturer
        {
            get { return selectedManufacturer; }
            set { SetProperty(ref selectedManufacturer, value); }
        }

        private ProductDetails selectedProduct;
        public ProductDetails SelectedProduct
        {
            get { return selectedProduct; }
            set { SetProperty(ref selectedProduct, value); }
        }

        public List<ProductAttributeMapping> AttributesSelectedList { get; set; } = new List<ProductAttributeMapping>();

        private CancellationTokenSource _cancellationTokenSource;

        private async void LoadManufacturer()
        {


            try
            {
                if (_cancellationTokenSource != null)
                    _cancellationTokenSource.Cancel();

                _cancellationTokenSource = new CancellationTokenSource();

                AttributesSelectedList?.Clear();

                SelectedManufacturer = null;

                SelectedProduct = null;

                Manufacturers = null;


                if (SelectedProductType == null) return;

                await Task.Run(async () =>
                {
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    var _result = await productTypeService.GetProductTypeManufacturers(SelectedProductType.ProductTypeId.Value, _cancellationTokenSource.Token);

                    if (_result != null && _result.Any())
                    {
                        //Manufacturers = new ObservableCollection<Manufacturer>(_result);
                        Manufacturers = new ObservableCollection<Manufacturer>(_result.OrderBy(x => x.Name).ToList());

                        //SelectedManufacturer = _result.ToList().FirstOrDefault();
                    }
                }, _cancellationTokenSource.Token);

            }
            catch (Exception ex)
            {
                logger?.LogError("error in load manufature", ex);
            }


        }
        private ObservableCollection<Manufacturer> manufacturers;
        public ObservableCollection<Manufacturer> Manufacturers
        {
            get { return manufacturers; }
            set { SetProperty(ref manufacturers, value); }
        }
        private ProductType selectedProductType;
        public ProductType SelectedProductType
        {
            get { return selectedProductType; }
            set { SetProperty(ref selectedProductType, value); }
        }
        private ObservableCollection<Store> stores;
        public ObservableCollection<Store> Stores
        {
            get { return stores; }
            set { SetProperty(ref stores, value); }
        }

        private Store _selectedStore;
        public Store SelectedStore
        {
            get { return _selectedStore; }
            set { SetProperty(ref _selectedStore, value); }
        }


        private Visibility nameVisibility;

        public Visibility NameVisibility
        {
            get { return nameVisibility; }
            set { SetProperty(ref nameVisibility, value); }
        }

        private Visibility valueVisibility;

        public Visibility ValueVisibility
        {
            get { return valueVisibility; }
            set { SetProperty(ref valueVisibility, value); }
        }

        private async void SearchTextChanged(string _searchText)
        {
            try
            {
                //SelectedProduct = null;

                if (!_searchText.IsValidString() || _searchText.Length < 3 || SelectedProductSearch != null)
                {
                    return;
                }


                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                }

                _cancellationTokenSource = new CancellationTokenSource();


                ProductsSearchList?.Clear();


                await Task.Run(async () =>
                {
                    var _result = await _productService.SearchProducts(_searchText, _cancellationTokenSource.Token);

                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        if (_result != null && _result.IsSuccess && _result.Data != null)
                        {
                            ProductsSearchList = new ObservableCollection<ProductSearchModel>(_result.Data);
                            IsDropDownOpen = true;
                        }
                        else
                        {
                            _notificationService.ShowMessage(_result?.Error ?? "No Search Results found", NotificationType.Error);
                        }
                    }, System.Windows.Threading.DispatcherPriority.Normal, _cancellationTokenSource.Token);
                }, _cancellationTokenSource.Token);


            }
            catch (OperationCanceledException)
            {
                logger.LogWarning($"Task was cancelled in product search text {_searchText}");
            }
            catch (Exception _ex)
            {
                logger.LogError($"Error in searching product by text {_searchText}", _ex);
            }
        }

        private async void SearchSelectionChanged()
        {

            try
            {
                if (SelectedProductSearch == null)
                {
                    SelectedSearchProduct = null;
                    return;
                }


                await Task.Run(async () =>
                {
                    var _result = await _productService.GetSKUStockProductSearch(SelectedProductSearch.ProductId);

                    if (_result != null && _result.IsSuccess && _result.Data != null)
                    {
                        var _productStockCount = await _productService.GetStockbySKU(_result.Data.ProductSKU,null);

                        Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            var _mapper = MapperConfig.InitializeAutomapper();

                            SelectedSearchProduct = _mapper.Map<ProductDetails>(_result.Data);



                        });
                    }
                    else
                    {
                        _notificationService.ShowMessage(_result.Error, Common.NotificationType.Error);
                        return;
                    }

                });

            }
            catch (Exception _ex)
            {

                logger.LogError("Error in gettting product information", _ex);
            }

        }



        private ProductSearchModel _selectedProductSearch;
        public ProductSearchModel SelectedProductSearch
        {
            get => _selectedProductSearch;
            set => SetProperty(ref _selectedProductSearch, value);
        }

        private ObservableCollection<ProductSearchModel> _productsSearchList;
        public ObservableCollection<ProductSearchModel> ProductsSearchList
        {
            get => _productsSearchList;
            set => SetProperty(ref _productsSearchList, value);
        }

        private bool _isDropDownOpen;
        public bool IsDropDownOpen
        {
            get => _isDropDownOpen;
            set => SetProperty(ref _isDropDownOpen, value);
        }

        private ProductDetails _selectedSearchProduct;
        public ProductDetails SelectedSearchProduct
        {
            get => _selectedSearchProduct;
            set => SetProperty(ref _selectedSearchProduct, value);

        }

    }
}
