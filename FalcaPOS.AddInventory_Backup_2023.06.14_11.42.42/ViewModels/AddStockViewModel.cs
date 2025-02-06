//using FalcaPOS.Common.Constants;
//using FalcaPOS.Common.Events;
//using FalcaPOS.Common.Helper;
//using FalcaPOS.Contracts;
//using FalcaPOS.Entites.Attributes;
//using FalcaPOS.Entites.Manufacturers;
//using FalcaPOS.Entites.Products;
//using FalcaPOS.Entites.ProductTypes;
//using FalcaPOS.Entites.Stock;
//using FalcaPOS.Entites.Stores;
//using FalcaPOS.Entites.Suppliers;
//using Prism.Commands;
//using Prism.Events;
//using Prism.Mvvm;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Windows.Controls;

//namespace FalcaPOS.AddInventory.ViewModels
//{
//    public class AddStockViewModel : BindableBase
//    {
//        private readonly IStoreService _storeService;
//        private readonly IEventAggregator _eventAggregator;
//        private readonly IProductTypeService _productTypeService;
//        private readonly IProductService _productService;
//        private readonly IStockService _stockService;

//        public DelegateCommand ProductTypeChange { get; private set; }

//        public DelegateCommand ManufacturerChange { get; private set; }

//        public DelegateCommand RestStockProductCommand { get; private set; }

//        public DelegateCommand<object> AddAttributeSelectionCommand { get; private set; }

//        public DelegateCommand AddStockProductCommand { get; private set; }

//        public DelegateCommand SelectedProductChangeCommand { get; private set; }

//        public AddStockViewModel(IStoreService storeService,
//            IEventAggregator eventAggregator,
//            IProductTypeService productTypeService,
//            IProductService productService,
//            IStockService stockService)
//        {
//            _storeService = storeService;

//            _eventAggregator = eventAggregator;

//            _productTypeService = productTypeService;

//            _productService = productService;

//            _stockService = stockService;

//            _eventAggregator.GetEvent<AddProductTypeEvent>().Subscribe(productType => AddProductTypeEvent(productType));

//            _eventAggregator.GetEvent<LoadManufacturesEvent>().Subscribe(obj => ReLoadBrandands(obj));

//            _eventAggregator.GetEvent<ReloadSupplierEvent>().Subscribe(LoadSuppliers);

//            ProductTypeChange = new DelegateCommand(LoadManufacturer);

//            ManufacturerChange = new DelegateCommand(LoadProductName);

//            AddAttributeSelectionCommand = new DelegateCommand<object>(AddAttributeSelection);

//            AddStockProductCommand = new DelegateCommand(AddStockProduct);

//            RestStockProductCommand = new DelegateCommand(ResetStockProduct);

//            LoadSuppliers();

//            LoadStores();

//            LoadProductTypes();

//            SelectedProductChangeCommand = new DelegateCommand(SelectedProductChanged);

//        }

//        private void SelectedProductChanged()
//        {
//            AttributesSelectedList?.Clear();
//        }

//        private void ResetStockProduct()
//        {

//            InvoiceNumber = null;
//            InvoiceDate = null;
//            SelectedSupplier = null;
//            Quantity = 0;
//            DefectiveQuantity = 0;
//            InvoiceRate = 0;
//            InvoiceDiscountPerecent = 0;
//            InvoiceDiscountFlat = 0;
//            InvoiceDiscount = 0;
//            InvoiceRoundOff = 0;
//            InvoiceTotal = 0;
//            AttributesSelectedList = null;
//            SelectedProduct = null;
//            SelectedManufacturer = null;
//            SelectedProductType = null;
//            SelectedStore = null;
//            SerialNumber = null;
//            IsQADone = false;
//            DateOfManufacture = null;
//            DateOfExpiry = null;
//            Location = null;
//            WarrantyService = null;
//            ProductRate = 0;
//            ProductDiscount = 0;
//            ProductTotal = 0;
//            ProductSellingPrice = 0;
//        }

//        private async void AddStockProduct()
//        {
//            try
//            {
//                var _product = GetProduct();

//                if (_product == null) return;

//                await Task.Run(async () =>
//                {

//                    //We are not  useing this page now.
//                    var _result = await _stockService.AddStockProduct(null);

//                    if (_result)
//                    {

//                        _eventAggregator.GetEvent<NotifyMessage>().Publish(new Common.Models.ToastMessage{Message = "Stock Added.",MessageType = Common.NotificationType.Success});

//                        _eventAggregator.GetEvent<ReloadStockEvent>().Publish();

//                        ResetStockProduct();
//                    }
//                });


//            }
//            catch (Exception _Ex)
//            {

//            }
//        }

//        private AddStockProduct GetProduct()
//        {
//            try
//            {

//                var isValidData = ValidateInputData();


//                if (isValidData == null) return null;

//                if (isValidData.HasError)
//                {
//                    //Alert service pass isValidData.Error;
//                    _eventAggregator.GetEvent<NotifyMessage>().Publish(new Common.Models.ToastMessage
//                    {
//                        Message = isValidData.Error,
//                        MessageType = Common.NotificationType.Error
//                    });

//                    return null;
//                }              
//                var _stockProduct = new AddStockProduct();

//                _stockProduct.InvoiceNumber = InvoiceNumber;
//                _stockProduct.InvoiceDate = InvoiceDate.Value;
//                _stockProduct.SupplierId = SelectedSupplier.SupplierId;
//                _stockProduct.Quantity = Quantity;
//                _stockProduct.DefectiveQuantity = DefectiveQuantity;
//                _stockProduct.InvoiceRate = InvoiceRate;
//                _stockProduct.InvoiceDiscountPerecent = InvoiceDiscountPerecent;
//                _stockProduct.InvoiceDiscountFlat = InvoiceDiscountFlat;
//                _stockProduct.InvoiceDiscount = InvoiceDiscount;
//                _stockProduct.InvoiceRoundOff = InvoiceRoundOff;
//                _stockProduct.InvoiceTotal = InvoiceTotal;
//                _stockProduct.ProductTypeId = SelectedProductType.ProductTypeId.Value;
//                _stockProduct.ManufacturerId = SelectedManufacturer.ManufacturerId;
//                _stockProduct.ProductId = SelectedProduct.ProductId;
//                _stockProduct.StroreId = SelectedStore.StoreId;
//                _stockProduct.SerialNumber = SerialNumber;
//                _stockProduct.IsQADone = IsQADone;
//                _stockProduct.AttributesSelectedList = AttributesSelectedList;
//                _stockProduct.DateOfManufacture = DateOfManufacture.Value;
//                _stockProduct.DateOfExpiry = DateOfExpiry.Value;
//                _stockProduct.Location = Location;
//                _stockProduct.WarrantyService = WarrantyService;
//                _stockProduct.ProductRate = ProductRate;
//                _stockProduct.ProductDiscount = ProductDiscount;
//                _stockProduct.ProductTotal = ProductTotal;
//                _stockProduct.ProductSellingPrice = ProductSellingPrice;
//                _stockProduct.ProductDiscountMode = GetProductDiscountMode();
//                _stockProduct.InvoiceDiscountMode = GetInvoiceDiscountMode();

//                return _stockProduct;
//            }
//            catch (Exception)
//            {

//            }

//            return null;
//        }

//        private string GetInvoiceDiscountMode()
//        {
//            string _mode = "Invoice_None";

//            if (InvoiceDiscountPerecent > 0)
//            {
//                _mode = "Invoice_Percentage";
//            }else if (InvoiceDiscountFlat > 0)
//            {
//                _mode = "Invoice_Flat";
//            }                       
//            return _mode;
//        }

//        private string GetProductDiscountMode()
//        {
//            string _mode = "Product_None";


//            if (ProductDiscountPerecent > 0)
//            {
//                _mode = "Product_Percentage";
//            }
//            else if (ProductDiscountFlat > 0)
//            {
//                _mode = "Product_Flat";
//            }
//            return _mode;
//        }

//        private ValidateInputResponse ValidateInputData()
//        {
//            var _response = new ValidateInputResponse();            

//            try
//            {
//                //Validate rest and alert user.
//                //Date validate.

//                if (string.IsNullOrWhiteSpace(InvoiceNumber))
//                {
//                    _response.HasError = true;
//                    _response.Error = "Invoice Number is  required.";
//                    return _response;
//                }
//                else if (InvoiceDate == null || InvoiceDate == default(DateTime))
//                {
//                    _response.HasError = true;
//                    _response.Error = "Invoice Date is required.";
//                    return _response;
//                }
//                else if (SelectedSupplier == null || SelectedSupplier.SupplierId <= 0)
//                {
//                    _response.HasError = true;
//                    _response.Error = "Supplier is required";
//                    return _response;
//                }
//                else if (string.IsNullOrWhiteSpace(SerialNumber))
//                {
//                    _response.HasError = true;
//                    _response.Error = "Serial number is requied";
//                    return _response;
//                }
//                else if (Quantity < 0 || DefectiveQuantity < 0)
//                {
//                    _response.HasError = true;
//                    _response.Error = "Invalid Quanity or Defective quantity.";
//                    return _response;
//                }
//                else if (InvoiceDiscountPerecent < 0 || InvoiceDiscountFlat < 0)
//                {
//                    _response.HasError = true;
//                    _response.Error = "Invalid Inovice Discount percent or Discount flat.";
//                    return _response;

//                }
//                else if (InvoiceDiscount < 0 || InvoiceRoundOff < 0 || InvoiceTotal < 0)
//                {
//                    _response.HasError = true;
//                    _response.Error = "Invalid Disount or Round off or Invoice Total.";
//                    return _response;
//                }
//                else if (SelectedProductType == null || SelectedProductType.ProductTypeId <= 0)
//                {
//                    _response.HasError = true;
//                    _response.Error = "Product type is required";
//                    return _response;
//                }
//                else if (SelectedManufacturer == null || SelectedManufacturer.ManufacturerId <= 0)
//                {
//                    _response.HasError = true;
//                    _response.Error = "Brand is required";
//                    return _response;
//                }
//                else if (SelectedProduct == null || SelectedProduct.ProductId <= 0)
//                {
//                    _response.HasError = true;
//                    _response.Error = "Product is required";
//                    return _response;
//                }
//                else if (SelectedStore == null || SelectedStore.StoreId <= 0)
//                {
//                    _response.HasError = true;
//                    _response.Error = "Store is required";
//                    return _response;
//                }               
//                else if (DateOfManufacture == null || DateOfManufacture == default(DateTime))
//                {
//                    _response.HasError = true;
//                    _response.Error = "Date of manufacture is required.";
//                    return _response;
//                }
//                else if (DateOfExpiry == null || DateOfExpiry == default(DateTime))
//                {

//                    _response.HasError = true;
//                    _response.Error = "Date of expiry is required.";                    
//                    return _response;
//                }
//                else if (string.IsNullOrWhiteSpace(Location))
//                {
//                    _response.HasError = true;
//                    _response.Error = "Loaction is required";
//                    return _response;
//                }
//                else if (string.IsNullOrWhiteSpace(WarrantyService))
//                {
//                    _response.HasError = true;
//                    _response.Error = "Warranty or service is required.";
//                    return _response;
//                }
//                else if (ProductRate < 0 || ProductDiscount < 0 || ProductTotal < 0 || ProductSellingPrice < 0)
//                {
//                    _response.HasError = true;
//                    _response.Error = "Invliad Product rate or discount or total or selling price.";
//                    return _response;
//                }

//            }
//            catch (Exception _ex)
//            {                
//            }

//            return _response;
//        }

//        private void AddAttributeSelection(object obj)
//        {
//            try
//            {

//                if (obj is SelectionChangedEventArgs) return;

//                if (SelectedProduct == null) return;


//                var _values = obj as Tuple<object, object>;

//                if (_values == null || _values.Item1 == null || _values.Item2 == null) return;


//                var _productAttribute = _values.Item1 as ProductAttribute;

//                var _attribute = _values.Item2 as AttributeMap;

//                if (_productAttribute == null || _attribute == null) return;


//                if (AttributesSelectedList == null)
//                    AttributesSelectedList = new List<ProductAttributeMapping>();


//                if (AttributesSelectedList.Any(x => x.ProductAttribute.AttributeId == _productAttribute.AttributeId))
//                {
//                    //Already there 

//                    var _existingAttribute = AttributesSelectedList.FirstOrDefault(x => x.ProductAttribute.AttributeId == _productAttribute.AttributeId);

//                    if (_existingAttribute != null)
//                    {
//                        _existingAttribute.AttributesList?.Clear();
//                        _existingAttribute.AttributesList = new List<AttributeMap>
//                        {
//                            new AttributeMap
//                            {
//                                AttributeValueId=_attribute.AttributeValueId,
//                                AttributeValueName=_attribute.AttributeValueName
//                            }
//                        };
//                    }
//                    return;

//                }

//                var _productMapping = new ProductAttributeMapping
//                {
//                    ProductAttribute = _productAttribute
//                };
//                _productMapping.AttributesList.Add(new AttributeMap
//                {
//                    AttributeValueId = _attribute.AttributeValueId,
//                    AttributeValueName = _attribute.AttributeValueName
//                });

//                AttributesSelectedList?.Add(_productMapping);


//            }
//            catch (Exception _ex)
//            {

//            }

//        }

//        private ProductType _selectedProductType;
//        public ProductType SelectedProductType
//        {
//            get { return _selectedProductType; }
//            set
//            {
//                SetProperty(ref _selectedProductType, value);

//            }
//        }

//        private void ReLoadBrandands(object obj)
//        {
//            if (obj != null)
//            {
//                var _type = obj as ProductType;

//                if (SelectedProductType != null && _type != null && _type.ProductTypeId == SelectedProductType.ProductTypeId)
//                {
//                    LoadManufacturer();
//                }
//            }

//        }

//        private void AddProductTypeEvent(object productType)
//        {
//            var _type = productType as ProductType;

//            if (_type != null)
//            {
//                ProductTypes.Add(_type);
//            }
//        }

//        private ObservableCollection<Manufacturer> _manufacturers = new ObservableCollection<Manufacturer>();
//        public ObservableCollection<Manufacturer> Manufacturers
//        {
//            get { return _manufacturers; }
//            set { SetProperty(ref _manufacturers, value); }
//        }


//        //private void SetSupplierNameChanged(object obj)
//        //{

//        //}

//        private void SetProductNameChange(object obj)
//        {

//        }

//        private ObservableCollection<Store> _stores = new ObservableCollection<Store>();
//        public ObservableCollection<Store> Stores
//        {
//            get { return _stores; }
//            set { SetProperty(ref _stores, value); }
//        }

//        private Store _selectedStore;
//        public Store SelectedStore
//        {
//            get { return _selectedStore; }
//            set { SetProperty(ref _selectedStore, value); }
//        }


//        private async void LoadStores()
//        {
//            if (!AppConstants.IsLoggedIn) return;
//            try
//            {
//                await Task.Run(async () =>
//                {
//                    var _result = await _storeService.GetStores("isenabled=true");

//                    Stores = new ObservableCollection<Store>(_result);

//                });
//            }
//            catch (Exception _ex)
//            {

//            }
//        }


//        private ObservableCollection<ProductDetails> _productDetailsList = new ObservableCollection<ProductDetails>();
//        public ObservableCollection<ProductDetails> ProductDetailsList
//        {
//            get { return _productDetailsList; }
//            set { SetProperty(ref _productDetailsList, value); }
//        }

//        private ProductDetails _selectedProduct;
//        public ProductDetails SelectedProduct
//        {
//            get { return _selectedProduct; }
//            set { SetProperty(ref _selectedProduct, value); }
//        }

//        private async void LoadProductName()
//        {
//            try
//            {

//                if (_cancellationTokenSource != null)
//                    _cancellationTokenSource.Cancel();

//                _cancellationTokenSource = new CancellationTokenSource();

//                SelectedProduct = null;

//                AttributesSelectedList?.Clear();

//                ProductDetailsList = null;

//                if (SelectedManufacturer == null) return;

//                await Task.Run(async () =>
//                {

//                    var _result = await _productService.GetProducts($"SortBy=name&ProducttypeManufacturerId={SelectedManufacturer.ProductTypeManufacturerId}&IncludeAttributes=true", _cancellationTokenSource.Token);

//                    if (_result != null && _result.Any())
//                    {
//                        ProductDetailsList = new ObservableCollection<ProductDetails>(_result);
//                        SelectedProduct = ProductDetailsList[0];
//                    }
//                }, _cancellationTokenSource.Token);

//            }
//            catch (Exception _ex)
//            {


//            }

//        }

//        private Manufacturer _selectedManufacturer;
//        public Manufacturer SelectedManufacturer
//        {
//            get { return _selectedManufacturer; }
//            set { SetProperty(ref _selectedManufacturer, value); }
//        }


//        private CancellationTokenSource _cancellationTokenSource;

//        private async void LoadManufacturer()
//        {
//            try
//            {
//                if (_cancellationTokenSource != null)
//                    _cancellationTokenSource.Cancel();

//                _cancellationTokenSource = new CancellationTokenSource();

//                AttributesSelectedList?.Clear();

//                SelectedManufacturer = null;

//                SelectedProduct = null;

//                Manufacturers = null;


//                if (SelectedProductType == null) return;

//                await Task.Run(async () =>
//                 {
//                     _cancellationTokenSource.Token.ThrowIfCancellationRequested();

//                     var _result = await _productTypeService.GetProductTypeManufacturers(SelectedProductType.ProductTypeId.Value, _cancellationTokenSource.Token);

//                     if (_result != null && _result.Any())
//                     {
//                         Manufacturers = new ObservableCollection<Manufacturer>(_result);

//                         SelectedManufacturer = _result.ToList().FirstOrDefault();
//                     }
//                 }, _cancellationTokenSource.Token);

//            }
//            catch (Exception _Ex)
//            {

//            }


//        }

//        private async void LoadProductTypes()
//        {

//            try
//            {

//                await Task.Run(async () =>
//                  {
//                      var _result = await _productTypeService.GetProductTypes("isenabled=true");

//                      if (_result != null && _result.Any())
//                      {
//                          ProductTypes = new ObservableCollection<ProductType>(_result);
//                          SelectedProductType = ProductTypes[0];
//                      }
//                      else
//                          ProductTypes = new ObservableCollection<ProductType>();
//                  });
//            }
//            catch (Exception _ex)
//            {

//            }

//        }

//        private async void LoadSuppliers()
//        {

//            try
//            {
//                await Task.Run(async () =>
//                {
//                    var _result = await HttpHelper.GetAsync<IEnumerable<Supplier>>($"{AppConstants.SUPPLIERS_GETALL}?isenabled=true", AppConstants.TOKEN);

//                    if (_result != null && _result.Any())
//                    {
//                        Suppliers = new ObservableCollection<Supplier>(_result);

//                        SelectedSupplier = Suppliers[0];
//                    }

//                });

//            }
//            catch (Exception __ex)
//            {

//            }
//        }


//        private ObservableCollection<Supplier> _suppliers;
//        public ObservableCollection<Supplier> Suppliers
//        {
//            get { return _suppliers; }
//            set { SetProperty(ref _suppliers, value); }
//        }

//        private Supplier _selectedSupplier;
//        public Supplier SelectedSupplier
//        {
//            get { return _selectedSupplier; }
//            set { SetProperty(ref _selectedSupplier, value); }
//        }



//        private ObservableCollection<ProductType> _productTypes = new ObservableCollection<ProductType>();
//        public ObservableCollection<ProductType> ProductTypes
//        {
//            get { return _productTypes; }
//            set { SetProperty(ref _productTypes, value); }
//        }


//        public List<ProductAttributeMapping> AttributesSelectedList { get; set; } = new List<ProductAttributeMapping>();


//        #region Props

//        private string _invoiceNumber;
//        public string InvoiceNumber
//        {
//            get { return _invoiceNumber; }
//            set { SetProperty(ref _invoiceNumber, value); }
//        }


//        private DateTime? _invoiceDate;
//        public DateTime? InvoiceDate
//        {
//            get { return _invoiceDate; }
//            set { SetProperty(ref _invoiceDate, value); }
//        }

//        private int _quantity;
//        public int Quantity
//        {
//            get { return _quantity; }
//            set { SetProperty(ref _quantity, value); }
//        }

//        private int _defectiveQuantity;
//        public int DefectiveQuantity
//        {
//            get { return _defectiveQuantity; }
//            set { SetProperty(ref _defectiveQuantity, value); }
//        }

//        private float _invoiceRate;
//        public float InvoiceRate
//        {
//            get { return _invoiceRate; }
//            set
//            {
//                SetProperty(ref _invoiceRate, value);
//                ResetInvoiceDiscount();
//            }

//        }

//        private void ResetInvoiceDiscount()
//        {
//            InvoiceDiscount = 0;
//            InvoiceDiscountPerecent = 0;
//            InvoiceDiscountFlat = 0;
//            InvoiceRoundOff = 0;
//            InvoiceTotal = InvoiceRate;
//        }

//        private float _invoiceDiscountPercent;
//        public float InvoiceDiscountPerecent
//        {
//            get { return _invoiceDiscountPercent; }
//            set
//            {
//                //    if (InvoiceRate <= 0)
//                //        return;

//                if (InvoiceRate == 0)
//                {
//                    value = 0;
//                }

//                if (value < 0 || value > 100)
//                {
//                    value = 0;
//                }

//                SetProperty(ref _invoiceDiscountPercent, value);
//                if(value > 0)
//                {
//                    CalCulateInvoiceDiscountPercent();
//                }
//                if (value == 0 && InvoiceDiscountFlat == 0)
//                {
//                    InvoiceDiscount = 0;
//                    CalulateInvoiceRoundOff();
//                }
//            }
//        }

//        private void CalCulateInvoiceDiscountPercent()
//        {
//            InvoiceDiscountFlat = 0;

//            InvoiceDiscount = (InvoiceDiscountPerecent * InvoiceRate) / 100;

//            InvoiceTotal = (InvoiceRate - InvoiceDiscount)+InvoiceRoundOff;


//        }

//        private float _invoiceDiscountFlat;
//        public float InvoiceDiscountFlat
//        {
//            get { return _invoiceDiscountFlat; }
//            set {

//                //if (InvoiceRate <= 0)
//                //{                    
//                //    return;
//                //}

//                if (InvoiceRate == 0)
//                {
//                    value = 0;
//                }

//                if (value > InvoiceRate)
//                {
//                    value = 0;
//                }

//                SetProperty(ref _invoiceDiscountFlat, value);

//                if (value > 0)
//                {
//                    CalculateInvoiceFlat();
//                }

//                if(value==0 && InvoiceDiscountPerecent == 0)
//                {
//                    InvoiceDiscount = 0;
//                    CalulateInvoiceRoundOff();
//                }
//            }
//        }

//        private void CalculateInvoiceFlat()
//        {
//            InvoiceDiscountPerecent = 0;

//            InvoiceDiscount =  InvoiceDiscountFlat;

//            InvoiceTotal = (InvoiceRate - InvoiceDiscount)+InvoiceRoundOff;
//        }

//        private float _invoiceDiscount;
//        public float InvoiceDiscount
//        {
//            get { return _invoiceDiscount; }
//            set { SetProperty(ref _invoiceDiscount, value); }
//        }


//        private float _invoiceRoundOff;
//        public float InvoiceRoundOff
//        {
//            get { return _invoiceRoundOff; }
//            set
//            {
//                if (value < 0)
//                {
//                    value = 0;
//                }
//                SetProperty(ref _invoiceRoundOff, value);                        
//                    CalulateInvoiceRoundOff();                   
//            }
//        }

//        private void CalulateInvoiceRoundOff()
//        {
//            InvoiceTotal = (InvoiceRate - InvoiceDiscount) + InvoiceRoundOff;
//        }

//        private float _invoiceTotal;
//        public float InvoiceTotal
//        {
//            get { return _invoiceTotal; }
//            set
//            {
//                SetProperty(ref
//              _invoiceTotal, value);
//            }
//        }


//        private string _serialNumber;
//        public string SerialNumber
//        {
//            get { return _serialNumber; }
//            set { SetProperty(ref _serialNumber, value); }
//        }


//        private bool _isQADone;
//        public bool IsQADone
//        {
//            get { return _isQADone; }
//            set { SetProperty(ref _isQADone, value); }
//        }


//        private DateTime? _dateOfManufacture;
//        public DateTime? DateOfManufacture
//        {
//            get { return _dateOfManufacture; }
//            set { SetProperty(ref _dateOfManufacture, value); }
//        }


//        private DateTime? _dateOfExpiry;
//        public DateTime? DateOfExpiry
//        {
//            get { return _dateOfExpiry; }
//            set {

//                //Block setting up of previous dates.

//                if (value < DateTime.Now)
//                {
//                    value = null;                    
//                }
//                SetProperty(ref _dateOfExpiry, value);
//            }
//        }


//        private string _location;
//        public string Location
//        {
//            get { return _location; }
//            set { SetProperty(ref _location, value); }
//        }



//        private string _warrantyService;
//        public string WarrantyService
//        {
//            get { return _warrantyService; }
//            set { SetProperty(ref _warrantyService, value); }
//        }

//        private float _productRate;
//        public float ProductRate
//        {
//            get { return _productRate; }
//            set {
//                SetProperty(ref _productRate, value);
//                ResetProductDiscount();

//            }
//        }

//        private void ResetProductDiscount()
//        {
//            ProductDiscount = 0;
//            ProductDiscountPerecent = 0;
//            ProductDiscountFlat = 0;
//            ProductTotal = ProductRate;

//        }

//        private float _productDiscount;
//        public float ProductDiscount
//        {
//            get { return _productDiscount; }
//            set { SetProperty(ref _productDiscount, value); }
//        }



//        private float _productTotal;
//        public float ProductTotal
//        {
//            get { return _productTotal; }
//            set { SetProperty(ref _productTotal, value); }
//        }

//        private float _productSellingPrice;
//        public float ProductSellingPrice
//        {
//            get { return _productSellingPrice; }
//            set { SetProperty(ref _productSellingPrice, value); }
//        }

//        private string _productDiscountMode;
//        public string ProductDiscountMode
//        {
//            get { return _productDiscountMode; }
//            set { SetProperty(ref _productDiscountMode, value); }
//        }

//        private string _invoiceDiscountMode;
//        public string InvoiceDiscountMode
//        {
//            get { return _invoiceDiscountMode; }
//            set { SetProperty(ref _invoiceDiscountMode, value); }
//        }


//        private float _productDiscountPerecent;
//        public float ProductDiscountPerecent
//        {
//            get { return _productDiscountPerecent; }
//            set {
//                if (ProductRate == 0)
//                {
//                    value = 0;
//                }

//                if (value < 0 || value > 100)
//                {
//                    value = 0;
//                }
//                SetProperty(ref _productDiscountPerecent, value);
//                if (value > 0)
//                {
//                    CalculateProductDiscountPercent();                 
//                }
//                if (value == 0 && ProductDiscountFlat == 0)
//                {
//                    ProductDiscount = 0;
//                    CalculateProductTotal();
//                }




//            }
//        }

//        private void CalculateProductTotal()
//        {
//            ProductTotal = ProductRate - ProductDiscount;

//        }

//        private void CalculateProductDiscountPercent()
//        {
//            ProductDiscountFlat = 0;

//            ProductDiscount = (ProductDiscountPerecent * ProductRate) / 100;

//            ProductTotal = (ProductRate - ProductDiscount);
//        }

//        private float _productDiscountFlat;
//        public float ProductDiscountFlat
//        {
//            get { return _productDiscountFlat; }
//            set {

//                if (ProductRate == 0)
//                {
//                    value = 0;
//                }

//                if (value > ProductRate)
//                {
//                    value = 0;
//                }

//                SetProperty(ref _productDiscountFlat, value);

//                if (value > 0)
//                {
//                    CalculateProductFlat();
//                }

//                if (value == 0 && ProductDiscountPerecent == 0)
//                {
//                    ProductDiscount = 0;
//                    CalculateProductTotal();
//                }


//            }
//        }

//        private void CalculateProductFlat()
//        {
//            ProductDiscountPerecent = 0;

//            ProductDiscount = ProductDiscountFlat;

//            ProductTotal = ProductRate - ProductDiscount;

//        }

//        #endregion



//    }
//}





