using FalcaPOS.Common;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Director;
using FalcaPOS.Entites.Manufacturers;
using FalcaPOS.Entites.Products;
using FalcaPOS.Entites.ProductTypes;
using FalcaPOS.Entites.Stores;
using FalcaPOS.Entites.Suppliers;
using MahApps.Metro.Controls;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.Director.ViewModel
{
    public class PurchaseRateSearchFlyoutViewModel : BindableBase
    {

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

        private int _width;

        public int Width
        {
            get { return _width; }
            set { _width = value; }

        }
        private int _height;

        public int Height
        {
            get { return _height; }
            set { _height = value; }

        }
        private readonly ISupplierService _supplierService;


        public DelegateCommand<PurchaseRateSearchModel> Search { get; private set; }
        public DelegateCommand<PurchaseRateSearchModel> Reset { get; private set; }

        private readonly IEventAggregator _eventAggregator;
        private readonly IProductTypeService _productTypeService;
        private readonly IStoreService _storeService;
        private readonly IProductService _productService;
        public DelegateCommand ProductTypeChange { get; private set; }
        public DelegateCommand ManufacturerChange { get; private set; }
        public DelegateCommand<object> ProductNameChange { get; private set; }

        private readonly Logger _logger;
        public PurchaseRateSearchFlyoutViewModel(IProductService productService,
                                                 IProductTypeService productTypeService,
                                                 IStoreService storeService,
                                                 IEventAggregator eventAggregator,
                                                 ISupplierService supplierService,
                                                 Logger logger)
        {

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            Search = new DelegateCommand<PurchaseRateSearchModel>(PurchaseSearch);

            Reset = new DelegateCommand<PurchaseRateSearchModel>(PurchaseRest);

            _supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));

            _productTypeService = productTypeService ?? throw new ArgumentNullException(nameof(productTypeService));

            _productService = productService ?? throw new ArgumentNullException(nameof(productService));

            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            this.Width = 1600;

            this.Height = 220;

            this.Position = Position.Top;

            _eventAggregator.GetEvent<PurchaseRateSearchFlyOut>().Subscribe((isopen) =>
            {
                this.IsOpen = isopen;
                if (!isopen)
                {

                    SelectedSupplier = null;
                    SearchModelItem.FromInvoiceDate = null;
                    SearchModelItem.ToInvocieDate = null;
                    SelectedProductType = null;
                    SelectedManufacturer = null;
                    SelectedProduct = null;
                    SelectedStore = null;
                    SearchModelItem.SKU = null;
                }

            });


            ProductTypeChange = new DelegateCommand(LoadManufacturer);

            ManufacturerChange = new DelegateCommand(LoadProductName);

            _eventAggregator.GetEvent<SignalRStoreAddedEvent>().Subscribe(NewStoreAdded, ThreadOption.PublisherThread);

            _eventAggregator.GetEvent<SignalRProductTypeEnableDisableEvent>().Subscribe(ProducttypeEnabledDisabled, ThreadOption.PublisherThread);

            _eventAggregator.GetEvent<SignalRProductTypeAddedEvent>().Subscribe(NewProducttypeAdded, ThreadOption.PublisherThread);

            _eventAggregator.GetEvent<SignalRSupplierAddedEvent>().Subscribe(NewSupplierAdded, ThreadOption.PublisherThread);

            _eventAggregator.GetEvent<SignalRSupplierEnableDisableEvent>().Subscribe(SupplierEnableDisable, ThreadOption.PublisherThread);


            LoadSuppliers();

            LoadStores();

            LoadProductTypes();

        }


        private void SupplierEnableDisable(object obj)
        {
            if (obj is SuppliersViewModel _splr)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {


                        if (Suppliers == null)
                        {
                            Suppliers = new ObservableCollection<SuppliersViewModel>();
                        }

                        //remove from list of suppliers.
                        var _exSupplier = Suppliers.FirstOrDefault(x => x.SupplierId == _splr.SupplierId);

                        if (_exSupplier != null)
                        {
                            Suppliers.Remove(_exSupplier);
                        }

                        if (!_splr.Isenabled)
                        {
                            return;
                        }
                        else
                        {
                            Suppliers.Add(_splr);

                            Suppliers = new ObservableCollection<SuppliersViewModel>(Suppliers.OrderBy(x => x.Name));

                        }
                    });

                }
                catch (Exception _ex)
                {
                    _logger?.LogError("Error in supplier enable disable", _ex);
                }
            }
        }

        private void NewSupplierAdded(object obj)
        {
            if (obj is SuppliersViewModel _suplr)
            {
                try
                {
                    Application.Current.Dispatcher?.Invoke(() =>
                    {

                        if (Suppliers == null)
                        {
                            Suppliers = new ObservableCollection<SuppliersViewModel> { _suplr };

                            return;
                        }

                        if (Suppliers.Any(x => x.SupplierId == _suplr.SupplierId))
                        {
                            var _exstSupplier = Suppliers.FirstOrDefault(x => x.SupplierId == _suplr.SupplierId);

                            if (_exstSupplier != null)
                            {
                                Suppliers.Remove(_exstSupplier);
                            }
                        }

                        Suppliers.Add(_suplr);

                        Suppliers = new ObservableCollection<SuppliersViewModel>(Suppliers.OrderBy(x => x.Name));


                    });
                }
                catch (Exception _ex)
                {
                    _logger.LogError("Error in adding supplier ", _ex);
                }
            }

        }



        private void ProducttypeEnabledDisabled(object obj)
        {
            try
            {
                if (obj is ProductType _pType)
                {
                    Application.Current?.Dispatcher?.Invoke(() =>
                    {

                        var _existingType = ProductTypes?.FirstOrDefault(x => x.ProductTypeId == _pType.ProductTypeId);

                        if (_existingType != null)
                        {
                            ProductTypes?.Remove(_existingType);
                        }

                        if (_pType.Isenabled)
                        {
                            if (ProductTypes == null)
                            {
                                ProductTypes = new ObservableCollection<ProductType>();
                            }
                            ProductTypes?.Add(_pType);
                            ProductTypes = new ObservableCollection<ProductType>(ProductTypes?.OrderBy(x => x.Name));
                        }
                    });

                }
            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in product type enable disable ", _ex);
            }
        }

        private void NewProducttypeAdded(object obj)
        {
            try
            {
                Application.Current?.Dispatcher?.Invoke(() =>
                {

                    if (obj is ProductType _type)
                    {
                        if (ProductTypes == null)
                            ProductTypes = new ObservableCollection<ProductType>();

                        if (ProductTypes.Any(x => x.ProductTypeId == _type.ProductTypeId))
                        {
                            return;
                        }
                        ProductTypes.Add(_type);

                        ProductTypes = new ObservableCollection<ProductType>(ProductTypes.OrderBy(x => x.Name));
                    }
                });

            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in new product type added signalr event", _ex);
            }
        }


        private void NewStoreAdded(object str)
        {
            try
            {
                _logger.LogInformation("New Store added  from purchase rate search flyout");

                if (str is Store _str)
                {
                    Application.Current?.Dispatcher?.Invoke(delegate
                    {

                        if (Stores == null)
                        {
                            _logger.LogInformation("Stores collection is null in purRateFlyout");

                            Stores = new ObservableCollection<Store> { _str };

                            return;
                        }

                        if (Stores.Any(x => x.Name == _str.Name))
                        {
                            _logger.LogInformation($"Store already added in purRateFlyout");

                            return;
                        }
                        Stores.Add(_str);
                        Stores = new ObservableCollection<Store>(Stores.OrderBy(x => x.Name));
                    });


                }
            }
            catch (Exception _ex)
            {
                _logger?.LogError("error in adding new  store to list in purRateFlyout", _ex);
            }

        }

        public void PurchaseRest(PurchaseRateSearchModel purchase)
        {
            SelectedSupplier = null;
            purchase.FromInvoiceDate = null;
            purchase.ToInvocieDate = null;
            SelectedProductType = null;
            SelectedManufacturer = null;
            SelectedProduct = null;
            SelectedStore = null;
            purchase.SKU = null;


        }

        public void PurchaseSearch(PurchaseRateSearchModel item)
        {
            if (SelectedSupplier == null
                && item.FromInvoiceDate == null
                && item.ToInvocieDate == null
                && SelectedProductType == null
                && SelectedManufacturer == null
                && SelectedProduct == null
                && SelectedStore == null && string.IsNullOrEmpty(item.SKU))
            {
                //MessageBox.Show("Select Anything To Search!!!..");
                ShowMessage("Search cannot be empty", NotificationType.Error);
                return;
            }

            // If from date or to date is empty, throws error.
            if (item.FromInvoiceDate != null && item.ToInvocieDate == null || item.FromInvoiceDate == null && item.ToInvocieDate != null)
            {
                ShowMessage("Please enter from and to date range ", NotificationType.Error);
                return;
            }

            {
                DateTime dt1 = Convert.ToDateTime(item.FromInvoiceDate);
                DateTime dt2 = Convert.ToDateTime(item.ToInvocieDate);
                if (dt2 < dt1)
                {
                    ShowMessage("To Date can not be smaller than From Date", NotificationType.Error);
                    return;
                }
            }
            PurchaseRateSearchModel model = new PurchaseRateSearchModel()
            {
                FromInvoiceDate = item.FromInvoiceDate,
                ToInvocieDate = item.ToInvocieDate,
                SupplierId = SelectedSupplier?.SupplierId,
                SupplierName = SelectedSupplier?.Name,
                ProductType = SelectedProductType?.Name,
                Brand = SelectedManufacturer?.Name,
                ProductName = SelectedProduct?.Name,
                StoreName = SelectedStore?.Name,
                SKU = item.SKU
            };

            _eventAggregator.GetEvent<PurchaseRateSearch>().Publish(model);

            this.IsOpen = false;
        }

        private async void LoadProductName()
        {
            try
            {

                SelectedProduct = null;



                ProductDetailsList = null;

                if (SelectedManufacturer == null) return;

                await Task.Run(async () =>
                {

                    var _result = await _productService.GetProducts($"SortBy=name&ProducttypeManufacturerId={SelectedManufacturer.ProductTypeManufacturerId}&IncludeAttributes=true");

                    if (_result != null && _result.Any())
                    {
                        ProductDetailsList = new ObservableCollection<ProductViewModel>(_result);
                    }
                });

            }
            catch (Exception _ex)
            {


            }

        }

        private async void LoadProductTypes()
        {

            try
            {

                await Task.Run(async () =>
                {
                    var _result = await _productTypeService.GetProductTypes("isenabled=true");


                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (_result != null && _result.Any())
                        {
                            ProductTypes = new ObservableCollection<ProductType>(_result);
                        }
                        else
                        {
                            ProductTypes = new ObservableCollection<ProductType>();
                        }
                    });

                });
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in purchaserate loading product type", _ex);

            }
        }
        private async void LoadSuppliers()
        {

            try
            {

                await Task.Run(async () =>
                {
                    var _result = await _supplierService.GetSuppliers("isenabled=true");

                    if (_result != null && _result.Any())
                    {
                        Application.Current?.Dispatcher?.Invoke(() =>
                        {

                            Suppliers = new ObservableCollection<SuppliersViewModel>(_result);

                        });
                    }

                });

            }
            catch (Exception _ex)
            {

            }
        }

        private async void LoadStores()
        {

            try
            {
                await Task.Run(async () =>
                {
                    var _result = await _storeService.GetStores();
                    if (_result != null)
                        Stores = new ObservableCollection<Store>(_result.Where(x => x.Parent_ref == null));
                });
            }
            catch (Exception ex)
            {
                _logger?.LogError("Error in loading stores in purrateSearchFlyout", ex);
            }

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


                if (SelectedProductType == null) return;

                await Task.Run(async () =>
                {
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    var _result = await _productTypeService.GetProductTypeManufacturers(SelectedProductType.ProductTypeId.Value, _cancellationTokenSource.Token);

                    if (_result != null && _result.Any())
                    {
                        Manufacturers = new ObservableCollection<Manufacturer>(_result.OrderBy(x => x.Name).ToList());
                        //Manufacturers = new ObservableCollection<Manufacturer>(_result);

                        //SelectedManufacturer = _result.ToList().FirstOrDefault();
                    }
                }, _cancellationTokenSource.Token);

            }
            catch (Exception _Ex)
            {

            }


        }
        private ObservableCollection<ProductViewModel> _productDetailsList = new ObservableCollection<ProductViewModel>();
        public ObservableCollection<ProductViewModel> ProductDetailsList
        {
            get { return _productDetailsList; }
            set { SetProperty(ref _productDetailsList, value); }
        }

        private Manufacturer _selectedManufacturer;
        public Manufacturer SelectedManufacturer
        {
            get { return _selectedManufacturer; }
            set { SetProperty(ref _selectedManufacturer, value); }
        }

        private ProductViewModel _selectedProduct;
        public ProductViewModel SelectedProduct
        {
            get { return _selectedProduct; }
            set { SetProperty(ref _selectedProduct, value); }
        }

        private ObservableCollection<Manufacturer> _manufacturers;
        public ObservableCollection<Manufacturer> Manufacturers
        {
            get { return _manufacturers; }
            set { SetProperty(ref _manufacturers, value); }
        }

        private ObservableCollection<ProductType> _productTypes = new ObservableCollection<ProductType>();
        public ObservableCollection<ProductType> ProductTypes
        {
            get { return _productTypes; }
            set { SetProperty(ref _productTypes, value); }
        }

        private ProductType _selectedProductType;
        public ProductType SelectedProductType
        {
            get { return _selectedProductType; }
            set { SetProperty(ref _selectedProductType, value); }
        }

        private ObservableCollection<SuppliersViewModel> _suppliers;

        public ObservableCollection<SuppliersViewModel> Suppliers
        {
            get => _suppliers;
            set => SetProperty(ref _suppliers, value);
        }

        private SuppliersViewModel _selectedSupplier;

        public SuppliersViewModel SelectedSupplier
        {
            get { return _selectedSupplier; }
            set { SetProperty(ref _selectedSupplier, value); }
        }

        private ObservableCollection<Store> _stores;
        public ObservableCollection<Store> Stores
        {
            get { return _stores; }
            set { SetProperty(ref _stores, value); }
        }

        private Store _selectedStore;
        public Store SelectedStore
        {
            get { return _selectedStore; }
            set { SetProperty(ref _selectedStore, value); }
        }

        private PurchaseRateSearchModel _serachModelItem = new PurchaseRateSearchModel();
        public PurchaseRateSearchModel SearchModelItem
        {
            get
            {
                return _serachModelItem;
            }
            set
            {
                SetProperty(ref _serachModelItem, value);
            }
        }

        private void ShowMessage(string msg, NotificationType notificationType) =>
          _eventAggregator.GetEvent<NotifyMessage>().Publish(new Common.Models.ToastMessage
          {
              Message = msg,
              MessageType = notificationType
          });

    }
}
