using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Attributes;
using FalcaPOS.Entites.Manufacturers;
using FalcaPOS.Entites.Products;
using FalcaPOS.Entites.ProductTypes;
using FalcaPOS.Entites.Stock;
using FalcaPOS.Entites.Suppliers;
using MahApps.Metro.Controls;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.AddInventory.ViewModels
{
    public class StockSearchFlyoutViewModel : BindableBase
    {
        private readonly IProductTypeService _productTypeService;
        private readonly IStoreService _storeService;

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
        private StockModelRequest _stockModelItem = new StockModelRequest();
        public StockModelRequest StockModelItem
        {
            get
            {
                return _stockModelItem;
            }
            set
            {
                SetProperty(ref _stockModelItem, value);
            }
        }

        private List<string> _reference;
        public List<string> Reference
        {
            get { return _reference; }
            set { SetProperty(ref _reference, value); }
        }

        private List<string> _supplier;
        public List<string> supplier
        {
            get { return _supplier; }
            set { SetProperty(ref _supplier, value); }
        }

        private ObservableCollection<Supplier> _suppliers;
        public ObservableCollection<Supplier> Suppliers
        {
            get { return _suppliers; }
            set { SetProperty(ref _suppliers, value); }
        }
        private Supplier _selectedSupplier;
        public Supplier SelectedSupplier
        {
            get { return _selectedSupplier; }
            set { SetProperty(ref _selectedSupplier, value); }
        }

        private ProductType _selectedProductType;
        public ProductType SelectedProductType
        {
            get { return _selectedProductType; }
            set { SetProperty(ref _selectedProductType, value); }
        }
        private readonly ISupplierService _supplierService;

        public DelegateCommand ProductTypeChange { get; private set; }

        public DelegateCommand<object> SupplierNameChanged { get; private set; }

        public DelegateCommand ManufacturerChange { get; private set; }

        public DelegateCommand<object> ProductNameChange { get; private set; }

        public DelegateCommand<StockModelRequest> StockSearch { get; private set; }
        public DelegateCommand<StockModelRequest> Reset { get; private set; }

        private readonly IProductService _productService;

        private readonly IEventAggregator _eventAggregator;

        private readonly Logger _logger;

        public StockSearchFlyoutViewModel(IEventAggregator eventAggregator, IProductService productService, IProductTypeService productTypeService, ISupplierService supplierService, IStoreService storeService, Logger logger)
        {
            _logger = logger ?? throw new ArgumentException(nameof(logger));

            _storeService = storeService;

            _productTypeService = productTypeService;

            _productService = productService;

            _eventAggregator = eventAggregator;

            _supplierService = supplierService;

            this.Position = MahApps.Metro.Controls.Position.Top;
            this.Width = 1600;
            this.Height = 250;
            LoadReference();
            LoadSuppliers();

            LoadProductTypes();
            _eventAggregator.GetEvent<SearchFlyout>().Subscribe((isopen) =>
            {
                this.IsOpen = isopen;
                LoadSuppliers();

                LoadProductTypes();
                LoadStores();
            });


            ProductTypeChange = new DelegateCommand(LoadManufacturer);

            ManufacturerChange = new DelegateCommand(LoadProductName);
            StockSearch = new DelegateCommand<StockModelRequest>(GetStockSearch);
            Reset = new DelegateCommand<StockModelRequest>(ResetStockSerarchInput);
            _eventAggregator.GetEvent<SignalRProductTypeAddedEvent>().Subscribe(NewProducttypeAdded, ThreadOption.PublisherThread);

            _eventAggregator.GetEvent<SignalRProductTypeEnableDisableEvent>().Subscribe(ProducttypeEnabledDisabled, ThreadOption.PublisherThread);

            _eventAggregator.GetEvent<SignalRSupplierAddedEvent>().Subscribe(NewSupplierAdded, ThreadOption.PublisherThread);

            _eventAggregator.GetEvent<SignalRSupplierEnableDisableEvent>().Subscribe(SupplierEnableDisable, ThreadOption.PublisherThread);
            _eventAggregator.GetEvent<SignalRProductEnableDisableAddEvent>().Subscribe(LoadProductName);


        }



        private void SupplierEnableDisable(object obj)
        {
            if (obj is Supplier _splr)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {


                        if (Suppliers == null)
                        {
                            Suppliers = new ObservableCollection<Supplier>();
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

                            Suppliers = new ObservableCollection<Supplier>(Suppliers.OrderBy(x => x.Name));

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
            if (obj is Supplier _suplr)
            {
                try
                {
                    Application.Current.Dispatcher?.Invoke(() =>
                    {

                        if (Suppliers == null)
                        {
                            Suppliers = new ObservableCollection<Supplier> { _suplr };

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

                        Suppliers = new ObservableCollection<Supplier>(Suppliers.OrderBy(x => x.Name));


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

        public void ResetStockSerarchInput(StockModelRequest item)
        {
            item.SerialNo = string.Empty;
            item.InvoiceToDate = string.Empty;
            item.InvoiceFromDate = string.Empty;

            item.InvoiceNo = string.Empty;
            item.Location = string.Empty;
            item.Status = string.Empty;
            item.Brand = string.Empty;
            item.ProductName = string.Empty;
            item.ProductType = string.Empty;
            item.SupplierName = string.Empty;
        }
        public void GetStockSearch(StockModelRequest item)
        {
            if (string.IsNullOrEmpty(item.SerialNo) && string.IsNullOrEmpty(item.SupplierName) && string.IsNullOrEmpty(item.ProductType) && string.IsNullOrEmpty(item.ProductName) && string.IsNullOrEmpty(item.Brand) && string.IsNullOrEmpty(item.Location) && string.IsNullOrEmpty(item.InvoiceNo)
                && string.IsNullOrEmpty(item.Status) && string.IsNullOrEmpty(item.InvoiceToDate) && string.IsNullOrEmpty(item.InvoiceFromDate))
            {
                MessageBox.Show("Select Anything To Search!!!..");
                return;
            }

            StockModelRequest modelRequest = new StockModelRequest()
            {
                SerialNo = string.IsNullOrEmpty(item.SerialNo) ? null : item.SerialNo,
                SupplierName = string.IsNullOrEmpty(item.SupplierName) ? null : item.SupplierName,
                ProductType = string.IsNullOrEmpty(item.ProductType) ? null : item.ProductType,
                ProductName = string.IsNullOrEmpty(item.ProductName) ? null : item.ProductName,
                Brand = string.IsNullOrEmpty(item.Brand) ? null : item.Brand,
                Location = string.IsNullOrEmpty(item.Location) ? null : item.Location,
                Status = string.IsNullOrEmpty(item.Status) ? null : item.Status,
                InvoiceNo = string.IsNullOrEmpty(item.InvoiceNo) ? null : item.InvoiceNo,
                InvoiceFromDate = string.IsNullOrEmpty(item.InvoiceFromDate) ? null : item.InvoiceFromDate,
                InvoiceToDate = string.IsNullOrEmpty(item.InvoiceToDate) ? null : item.InvoiceToDate,

            };

            _eventAggregator.GetEvent<StockSearch>().Publish(modelRequest);
            this.IsOpen = false;
        }

        public string GetCommaSeparatedString(List<string> splitstring)
        {
            StringBuilder sp = new StringBuilder();
            foreach (var item in splitstring)
            {
                sp.Append(item + ",");
            }
            return sp.ToString();
        }
        private async void LoadSuppliers()
        {

            try
            {
                await Task.Run(async () =>
                {
                    var _result = await HttpHelper.GetAsync<IEnumerable<Supplier>>($"{AppConstants.SUPPLIERS_GETALL}?isenabled=true", AppConstants.ACCESS_TOKEN);

                    if (_result != null && _result.Any())
                    {
                        Suppliers = new ObservableCollection<Supplier>(_result);
                        supplier = new List<string>(_result.Select(x => x.Name).ToList());
                        SelectedSupplier = Suppliers[0];
                    }

                });

            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in stock search view model", _ex);
            }
        }
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

                    var _result = await _productService.GetenabledProducts($"SortBy=name&ProducttypeManufacturerId={SelectedManufacturer.ProductTypeManufacturerId}&IncludeAttributes=true");

                    if (_result != null && _result.Any())
                    {
                        ProductDetailsList = new ObservableCollection<ProductDetails>(_result);
                        SelectedProduct = ProductDetailsList[0];
                    }
                });

            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in stock search view model", _ex);

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
                    var _result = await _productTypeService.GetProductTypes("isenabled=true");

                    if (_result != null && _result.Any())
                    {
                        ProductTypes = new ObservableCollection<ProductType>(_result);
                        SelectedProductType = ProductTypes[0];
                    }
                    else
                        ProductTypes = new ObservableCollection<ProductType>();
                });
            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in stock search view model", _ex);
            }
        }

        private ObservableCollection<ProductDetails> _productDetailsList = new ObservableCollection<ProductDetails>();
        public ObservableCollection<ProductDetails> ProductDetailsList
        {
            get { return _productDetailsList; }
            set { SetProperty(ref _productDetailsList, value); }
        }


        private ObservableCollection<ProductType> _productTypes = new ObservableCollection<ProductType>();
        public ObservableCollection<ProductType> ProductTypes
        {
            get { return _productTypes; }
            set { SetProperty(ref _productTypes, value); }
        }
        private Manufacturer _selectedManufacturer;
        public Manufacturer SelectedManufacturer
        {
            get { return _selectedManufacturer; }
            set { SetProperty(ref _selectedManufacturer, value); }
        }

        private ProductDetails _selectedProduct;
        public ProductDetails SelectedProduct
        {
            get { return _selectedProduct; }
            set { SetProperty(ref _selectedProduct, value); }
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

                    var _result = await _productTypeService.GetProductTypeManufacturers(SelectedProductType.ProductTypeId.Value, _cancellationTokenSource.Token);

                    if (_result != null && _result.Any())
                    {
                        Manufacturers = new ObservableCollection<Manufacturer>(_result);

                        SelectedManufacturer = _result.ToList().FirstOrDefault();
                    }
                }, _cancellationTokenSource.Token);

            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in stock search view model", _ex);
            }


        }
        private ObservableCollection<Manufacturer> _manufacturers;
        public ObservableCollection<Manufacturer> Manufacturers
        {
            get { return _manufacturers; }
            set { SetProperty(ref _manufacturers, value); }
        }

        private List<string> _stores;
        public List<string> Stores
        {
            get { return _stores; }
            set { SetProperty(ref _stores, value); }
        }


        private async void LoadStores()
        {
            if (!AppConstants.IsLoggedIn) return;
            try
            {
                await Task.Run(async () =>
                {
                    var _result = await _storeService.GetStores("isenabled=true");
                    if (_result != null && _result.ToList().Count > 0)
                        Stores = new List<string>(_result.Where(x => x.Parent_ref == null).Select(x => x.Name));

                });
            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in stock search view model", _ex);
            }
        }

    }
}
