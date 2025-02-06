using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Manufacturers;
using FalcaPOS.Entites.Products;
using FalcaPOS.Entites.ProductTypes;
using FalcaPOS.Entites.Stock;
using FalcaPOS.Entites.StockAge;
using FalcaPOS.Entites.Stores;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.Stock.ViewModels
{
    public class StockViewModel : BindableBase
    {

        private readonly IStoreStockService storeStockService;

        private readonly IProductTypeService productTypeService;

        public DelegateCommand StockSearchCommand { get; set; }

        StockModelRequest stockModelRequest = new StockModelRequest();

        public DelegateCommand<object> RefreshDataGrid { get; set; }
        public DelegateCommand<object> DownloadStockAgeStoreCommand { get; set; }

        private IEventAggregator eventAggregator { get; set; }

        private readonly IDialogService dialogService;

        public DelegateCommand<object> RowDoubleClickCommand { get; private set; }

        private readonly IStockService stockService;

        private readonly INotificationService notificationService;

        public DelegateCommand<object> PrintBarCodeCommand { get; private set; }

        private readonly ProgressService ProgressService;

        private readonly Logger logger;

        private readonly IStoreService storeService;

        private readonly IProductService productService;

        public DelegateCommand CategoryNameChange { get; private set; }
        public DelegateCommand SelectedSubCateoryChange { get; private set; }
        public DelegateCommand ManufacturerChange { get; private set; }

        private readonly Logger _logger;

        private readonly INotificationService _notificationService;

        private readonly IStockAgeService _stockAgeService;

        public DelegateCommand DownloadStockAgeCommand { get; private set; }
        public DelegateCommand<string> SearchTextChangedCommand { get; private set; }


        private ICommonService _commonService;

        public DelegateCommand SearchProductSelectionChangedCommand { get; private set; }



        public StockViewModel(ProgressService _ProgressService, IEventAggregator EventAggregator, IDialogService DialogService, IStoreStockService StorestockService, IStockService StockService, INotificationService NotificationService, Logger Logger,

            IProductTypeService ProductTypeService,
             IProductService ProductService,
            IStoreService StoreService,
           IStockAgeService stockAgeService, ICommonService commonService


            )
        {
            ProgressService = _ProgressService;

            stockModelRequest.Status = "Stock";

            stockModelRequest.Location = AppConstants.StoreName;

            dialogService = DialogService;

            eventAggregator = EventAggregator;

            storeStockService = StorestockService ?? throw new ArgumentNullException(nameof(StorestockService));

            stockService = StockService ?? throw new ArgumentNullException(nameof(StockService));

            notificationService = NotificationService ?? throw new ArgumentNullException(nameof(NotificationService));

            //Dont load initial Stock let them search and get--as per magesh comment
            //LoadData(stockModelRequest);

            //StockSerachFlyout = new DelegateCommand<object>(OpenStockSearchflyoutOpen);

            RefreshDataGrid = new DelegateCommand<object>(RefreshGrid);
            DownloadStockAgeStoreCommand = new DelegateCommand<object>(DownloadStockAgeStore);

            // eventAggregator.GetEvent<StoreStockSearch>().Subscribe(LoadData);

            //RowDoubleClickCommand = new DelegateCommand<object>(DisplayProductInfo);

            productTypeService = ProductTypeService ?? throw new ArgumentNullException(nameof(ProductTypeService));

            StockSearchCommand = new DelegateCommand(GetStockSearch);

            PrintBarCodeCommand = new DelegateCommand<object>(PrintBarCode);

            productTypeService = ProductTypeService ?? throw new ArgumentNullException(nameof(ProductTypeService));

            productService = ProductService ?? throw new ArgumentNullException(nameof(ProductService));

            storeService = StoreService ?? throw new ArgumentNullException(nameof(StoreService));
            logger = Logger ?? throw new ArgumentNullException(nameof(Logger));

            _stockAgeService = stockAgeService;
            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));

            _notificationService = NotificationService ?? throw new ArgumentNullException(nameof(NotificationService));

            CategoryNameChange = new DelegateCommand(LoadSubCategories);

            SelectedSubCateoryChange = new DelegateCommand(LoadManufacturer);

            ManufacturerChange = new DelegateCommand(LoadProductName);

            SearchTextChangedCommand = new DelegateCommand<string>(SearchTextChanged);

            SearchProductSelectionChangedCommand = new DelegateCommand(SearchSelectionChanged);

            GlobalUser = AppConstants.LoggedInStoreInfo.StoreId == 1;
            if (!GlobalUser)
                SelectedStore = AppConstants.LoggedInStoreInfo;

            LoadCategories();

            LoadStores();
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
                    var _result = await productService.SearchProducts(_searchText, _cancellationTokenSource.Token);

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


        private async void DownloadStockAgeStore(object obj)
        {

            try
            {
                await ProgressService.StartProgressAsync();
                var _result = await _stockAgeService.GetStockAgeReportStore(DateTime.Now.AddYears(-3).ToString("dd-MM-yyyy"), DateTime.Now.ToString("dd-MM-yyyy"));
                if (_result != null && _result.IsSuccess)
                {
                    foreach (var item in _result.Data)
                    {
                        int nofDays = (DateTime.Now.Date - item.PurchaseDate.Date).Days;
                        if (nofDays == 0) item.Today = item.TotalSelligPrice;
                        if (nofDays >= 1 && nofDays <= 15) item.Days_1_15 = item.TotalSelligPrice;
                        if (nofDays >= 16 && nofDays <= 30) item.Days_15_30 = item.TotalSelligPrice;
                        if (nofDays >= 31 && nofDays <= 45) item.Days_31_45 = item.TotalSelligPrice;
                        if (nofDays >= 46 && nofDays <= 60) item.Days_46_60 = item.TotalSelligPrice;
                        if (nofDays >= 61 && nofDays <= 75) item.Days_61_75 = item.TotalSelligPrice;
                        if (nofDays >= 76 && nofDays <= 90) item.Days_76_90 = item.TotalSelligPrice;
                        if (nofDays >= 91 && nofDays <= 105) item.Days_91_105 = item.TotalSelligPrice;
                        if (nofDays >= 106 && nofDays <= 120) item.Days_106_120 = item.TotalSelligPrice;
                        if (nofDays > 120) item.Above120Days = item.TotalSelligPrice;
                    }

                    var groupByResult = from result in _result.Data
                                        group result by result.SKU into s
                                        select new { sku = s.Key, was = s.Sum(x => x.Qty * x.SellingPrice), qty = s.Sum(x => x.Qty) };


                    List<StockAgeSellingDataDuration> WACResult = new List<StockAgeSellingDataDuration>();

                    foreach (var item in _result.Data)
                    {


                        var Weighted = groupByResult != null ? groupByResult.FirstOrDefault(x => x.sku == item.SKU) != null ? groupByResult.FirstOrDefault(x => x.sku == item.SKU) : null : null;
                        var was = Weighted != null ? Weighted.was / Weighted.qty : 0;
                        var totalSelling = (item.Qty * (was != 0 ? was : item.SellingPrice));
                        WACResult.Add(new StockAgeSellingDataDuration()
                        {

                            Supplier = item.Supplier,
                            Department = item.Department,
                            Brand = item.Brand,
                            Product = item.Product,
                            PurchaseDate = item.PurchaseDate,
                            Units = item.Units,
                            SKU = item.SKU,
                            Qty = item.Qty,
                            SellingPrice = was != 0 ? was : item.SellingPrice,
                            TotalSelligPrice = totalSelling,
                            Store = "NA",
                        });

                        foreach (var ite in WACResult)
                        {
                            int nofDays = (DateTime.Now - ite.PurchaseDate).Days;
                            if (nofDays == 0) ite.Today = ite.TotalSelligPrice;
                            if (nofDays >= 1 && nofDays <= 15) ite.Days_1_15 = ite.TotalSelligPrice;
                            if (nofDays >= 16 && nofDays <= 30) ite.Days_15_30 = ite.TotalSelligPrice;
                            if (nofDays >= 31 && nofDays <= 45) ite.Days_31_45 = ite.TotalSelligPrice;
                            if (nofDays >= 46 && nofDays <= 60) ite.Days_46_60 = ite.TotalSelligPrice;
                            if (nofDays >= 61 && nofDays <= 75) ite.Days_61_75 = ite.TotalSelligPrice;
                            if (nofDays >= 76 && nofDays <= 90) ite.Days_76_90 = ite.TotalSelligPrice;
                            if (nofDays >= 91 && nofDays <= 105) ite.Days_91_105 = ite.TotalSelligPrice;
                            if (nofDays >= 106 && nofDays <= 120) ite.Days_106_120 = ite.TotalSelligPrice;
                            if (nofDays > 120) ite.Above120Days = ite.TotalSelligPrice;
                        }
                    }

                    bool _export = _commonService.ExportToXL(_result.Data, WACResult, "SellingPrice", "WeightedAveragePrice", "POSStockAgeingReport_Store", false);
                    if (_export)
                    {

                        _notificationService.ShowMessage("Stock age is exported successfully and file is exported to C:\\FALCAPOS\\PosReports folder", Common.NotificationType.Success);

                    }
                    else
                    {
                        _notificationService.ShowMessage("File export failed , try again", Common.NotificationType.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            finally
            {

                await ProgressService.StopProgressAsync();
            }


        }

        private async void LoadProductName()
        {
            try
            {

                SelectedProduct = null;
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
                logger?.LogError("error in load product", ex);

            }

        }


        public void GetStockSearch()
        {

            bool _allIsNull = (SelectedProductSearch == null &&
                  SelectedCategoryName == null &&
                  SelectedSubCateory == null &&
                  selectedManufacturer == null &&
                  selectedProduct == null);
            if (GlobalUser && _allIsNull && SelectedStore == null)
            {
                ShowMessage("Search cannot be empty", NotificationType.Error);
                return;
            }
            else if (!GlobalUser && _allIsNull)
            {
                ShowMessage("Search cannot be empty", NotificationType.Error);
                return;
            }
            //if (String.IsNullOrEmpty(item.InvoiceFromDate) && !string.IsNullOrEmpty(item.InvoiceToDate))
            //{

            //    ShowMessage("Please enter from Date", NotificationType.Error);
            //    return;

            //}

            //if (!String.IsNullOrEmpty(item.InvoiceFromDate) && !string.IsNullOrEmpty(item.InvoiceToDate))
            //{
            //    DateTime dt1 = Convert.ToDateTime(item.InvoiceFromDate);
            //    DateTime dt2 = Convert.ToDateTime(item.InvoiceToDate);
            //    if (dt2 < dt1)
            //    {
            //        ShowMessage("From Date should be less than or equal to To Date", NotificationType.Error);
            //        return;
            //    }
            //}



            //StockModelRequest modelRequest = new StockModelRequest()
            //{
            //     Category =SelectedCategoryName.CategoryName,
            //    ProductType = SelectedSubCateory?.SubCategoryName,
            //    ProductName = selectedProduct?.Name,
            //    Brand = SelectedManufacturer?.Name,
            //    //Location = SelectedStore?.Name,//AppConstants.UserName,
            //    //Status = string.IsNullOrEmpty(item.Status) ? null : item.Status,

            //    //InvoiceFromDate = string.IsNullOrEmpty(item.InvoiceFromDate) ? null : item.InvoiceFromDate,
            //    //InvoiceToDate = string.IsNullOrEmpty(item.InvoiceToDate) ? null : item.InvoiceToDate,

            //};
            //if (SelectedStore != null && modelRequest != null && SelectedStore.StoreId > 0)
            //{
            //    modelRequest.StoreId = SelectedStore.StoreId.ToString();
            //}

            LoadData();

        }



        private void ShowMessage(string msg, NotificationType notificationType) =>
                    eventAggregator.GetEvent<NotifyMessage>().Publish(new Common.Models.ToastMessage
                    {
                        Message = msg,
                        MessageType = notificationType
                    });


        private void PrintBarCode(object obj)
        {
            if (obj is StoreStockModelResponse stock)
            {
                dialogService.ShowDialog("BarCodeDialog", new DialogParameters($"productId={stock.productid}&price={stock.Sellingprice ?? default}"),
                    (r) => { });
            }


        }



        public async void LoadData()
        {
            try
            {
                var _model = new StockSearchModel()
                {
                    Category = SelectedProductSearch != null ? (SelectedProductSearch.CategoryID) : SelectedCategoryName?.Id,
                    IsParent = true,
                    Brand = SelectedProductSearch != null ? (SelectedProductSearch.BrandID) : selectedManufacturer?.ManufacturerId,
                    ProductName = SelectedProductSearch != null ? (SelectedProductSearch.ProductId) : selectedProduct?.ProductId,
                    ProductType = SelectedProductSearch != null ? (SelectedProductSearch.SubcategoryID) : SelectedSubCateory?.Id,
                    Status = "Stock",
                    StoreId = GlobalUser ? (SelectedStore?.StoreId.ToString()) : SelectedStore.StoreId.ToString()
                };
                await ProgressService.StartProgressAsync();



                List<StoreStockModelResponse> data = new List<StoreStockModelResponse>();

                var _result = await storeStockService.GetStoreStockSearch(_model);
                if (_result != null)
                {

                    if (_result.Tables.Count > 0)
                    {
                        var _rows = _result.Tables[0].Rows.Cast<DataRow>().ToList();
                        if (_rows.Count > 0)
                        {
                            _rows.ForEach(x =>
                            {


                                data.Add(new StoreStockModelResponse()
                                {
                                    productid = x.Field<long>("id"),
                                    Producttype = x.Field<string>("producttype"),
                                    AvailableUnits = x.Field<long>("qty"),
                                    SoldQty = x.Field<long>("soldqty"),
                                    Brand = x.Field<string>("brand"),
                                    ProductName = x.Field<string>("productname"),
                                    Category = x.Field<string>("Category"),
                                    Status = x.Field<string>("status"),
                                    Barcode = x.Field<string>("barcode"),
                                    Expirydate = x.Field<string>("expirydate"),
                                    Serialno = x.Field<string>("serialno"),
                                    Sellingprice = x.Field<double>("sellingprice"),
                                    Store = x.Field<String>("store"),
                                    //Name = x.Field<string>("name"),
                                    //Value = x.Field<string>("value")
                                    IsSellingPriceUpdated = x.Field<bool>("issellingpriceupdate"),
                                    HSN = x.Field<string>("hsn"),
                                    LotNumber = x.Field<string>("lotnumber"),
                                    ProductSKU = x.Field<string>("sku"),
                                });

                            });
                        }



                    }
                }

                BackendStock = new ObservableCollection<StoreStockModelResponse>(data);
                if (data.ToList().Count > 0)
                {
                    TotalCount = "Total Count " + data.ToList().Count();
                }
                else
                {
                    BackendStock = new ObservableCollection<StoreStockModelResponse>(data);
                    TotalCount = null;
                    _notificationService.ShowMessage("No Data Found", NotificationType.Error);
                }

                await ProgressService.StopProgressAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
            finally
            {
                await ProgressService.StopProgressAsync();
            }
        }


        private async void LoadSubCategories()
        {
            if (SelectedCategoryName != null)
            {
                try
                {
                    await Task.Run(async () =>
                    {
                        var _result = await productTypeService.GetSubCategory(SelectedCategoryName.Id, new CancellationToken());
                        SubCateories = new ObservableCollection<SubCategoryModel>();
                        if (_result.IsSuccess && _result?.Data != null)
                            SubCateories = new ObservableCollection<SubCategoryModel>(_result.Data);
                        //SelectedProductType = ProductTypes[0];
                    });
                }
                catch (Exception ex)
                {
                    logger?.LogError("error in load product type", ex);
                }
            }
        }

        private async void LoadCategories()
        {

            try
            {

                await Task.Run(async () =>
                {
                    var _result = await productTypeService.GetAllCategory();
                    Cateories = new ObservableCollection<CategoryModel>();
                    if (_result.IsSuccess && _result?.Data != null)
                        Cateories = new ObservableCollection<CategoryModel>(_result.Data.ToList());
                });
            }
            catch (Exception ex)
            {
                logger?.LogError("error in load product type", ex);
            }
        }


        public void RefreshGrid(object obj)
        {
            SelectedProductSearch = null;
            SelectedCategoryName = null;
            SelectedSubCateory = null;
            selectedManufacturer = null;
            selectedProduct = null;
            BackendStock = null;

            if (AppConstants.LoggedInStoreInfo.StoreId == 1)
                SelectedStore = null;
            BackendStock = null;
        }


        private async void LoadStores()
        {
            if (AppConstants.USER_ROLES != null && AppConstants.USER_ROLES.Any())
            {
                if (AppConstants.LoggedInStoreInfo.StoreId == 1)
                {

                    await Task.Run(async () =>
                    {
                        var _result = await storeService.GetStores();
                        if (_result != null)
                        {
                            Stores = new ObservableCollection<Store>(_result);
                        }

                    });

                }
                else
                    Stores = new ObservableCollection<Store>(new List<Store>() { AppConstants.LoggedInStoreInfo });


            }


        }


        private ObservableCollection<StoreStockModelResponse> backendstock = new ObservableCollection<StoreStockModelResponse>();
        public ObservableCollection<StoreStockModelResponse> BackendStock
        {
            get
            {
                return backendstock;
            }
            set
            {
                SetProperty(ref backendstock, value);
            }
        }


        private ObservableCollection<ProductDetails> productDetailsList = new ObservableCollection<ProductDetails>();
        public ObservableCollection<ProductDetails> ProductDetailsList
        {
            get { return productDetailsList; }
            set { SetProperty(ref productDetailsList, value); }
        }

        private string _totalcount;

        public string TotalCount
        {
            get { return _totalcount; }
            set { SetProperty(ref _totalcount, value); }
        }

        private ObservableCollection<CategoryModel> _cateories = new ObservableCollection<CategoryModel>();
        public ObservableCollection<CategoryModel> Cateories
        {
            get { return _cateories; }
            set { SetProperty(ref _cateories, value); }
        }

        private ObservableCollection<SubCategoryModel> _subCateories = new ObservableCollection<SubCategoryModel>();
        public ObservableCollection<SubCategoryModel> SubCateories
        {
            get { return _subCateories; }
            set { SetProperty(ref _subCateories, value); }
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

        private CategoryModel _selectedCategoryName;
        public CategoryModel SelectedCategoryName
        {
            get { return _selectedCategoryName; }
            set { SetProperty(ref _selectedCategoryName, value); }
        }

        private SubCategoryModel _selectedSubCateory;
        public SubCategoryModel SelectedSubCateory
        {
            get { return _selectedSubCateory; }
            set { SetProperty(ref _selectedSubCateory, value); }
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


        private bool _globalUser;

        public bool GlobalUser
        {
            get { return _globalUser; }
            set { SetProperty(ref _globalUser, value); }
        }



        private CancellationTokenSource _cancellationTokenSource;

        private async void LoadManufacturer()
        {

            try
            {
                if (_cancellationTokenSource != null)
                    _cancellationTokenSource.Cancel();

                _cancellationTokenSource = new CancellationTokenSource();

                SelectedManufacturer = null;

                SelectedProduct = null;

                Manufacturers = null;


                if (SelectedSubCateory == null) return;

                await Task.Run(async () =>
                {
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    var _result = await productTypeService.GetProductTypeManufacturers(SelectedSubCateory.Id, _cancellationTokenSource.Token);

                    if (_result != null && _result.Any())
                    {
                        Manufacturers = new ObservableCollection<Manufacturer>(_result);

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
                    var _result = await productService.GetSKUStockProductSearch(SelectedProductSearch.ProductId);

                    if (_result != null && _result.IsSuccess && _result.Data != null)
                    {
                        var _productStockCount = await productService.GetStockbySKU(_result.Data.ProductSKU);

                        Application.Current?.Dispatcher?.Invoke(() =>
                        {

                            SelectedSearchProduct = _result.Data;



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

                logger.LogError("Error in getting product information", _ex);
            }

        }

        private ProductDetails _selectedSearchProduct;
        public ProductDetails SelectedSearchProduct
        {
            get => _selectedSearchProduct;
            set => SetProperty(ref _selectedSearchProduct, value);

        }
    }
}
