
using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Common.Validations;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Attributes;
using FalcaPOS.Entites.Indent;
using FalcaPOS.Entites.Manufacturers;
using FalcaPOS.Entites.Products;
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
using System.Windows.Controls;

namespace FalcaPOS.AddInventory.ViewModels
{
    public class ProductCardViewModel : ValidationBase
    {

        #region Fields
        //private readonly IProductTypeService _productTypeService;

        private ObservableCollection<SerialNumber> _serialNoList;

        //private ObservableCollection<ProductDetails> _productDetailsList = new ObservableCollection<ProductDetails>();

        private readonly Logger _logger;

        private CancellationTokenSource _cancellationTokenSource;

        private readonly IProductService _productService;

        //private readonly InvoiceGenerateService _invoiceGenerateService;

        private readonly INotificationService _notificationService;

        public DelegateCommand<object> AdddefectiveBase { get; private set; }

        public DelegateCommand<object> DeleteSingleAtributeValueCommand { get; private set; }

        public DelegateCommand<object> SelectedUnitTypeCommand { get; private set; }

        public DelegateCommand<object> SelectedSubUnitCommand { get; private set; }

        private readonly IEventAggregator _eventAggregator;

        public DelegateCommand<string> SearchTextChangedCommand { get; private set; }


        public DelegateCommand SearchProductSelectionChangedCommand { get; private set; }

        public DelegateCommand<object> defectiveQtyCommand { get; private set; }

        #endregion

        #region Constructor

        public ProductCardViewModel(IndentViewProduct Product,
          IEventAggregator eventAggregator,
          //IProductTypeService productTypeService,
          IProductService productService,
          INotificationService notificationService,
          Logger logger)
        {
            ID = Guid.NewGuid();

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            GenerateSerialCommand = new DelegateCommand<object>(ExecuteGenerateSerialCommand);

            SerialNoList = new ObservableCollection<SerialNumber>();

            // _productTypeService = productTypeService ?? throw new ArgumentNullException(nameof(productService));

            // ProductTypeChange = new DelegateCommand(LoadManufacturer);

            //ManufacturerChange = new DelegateCommand(LoadProductName);

            AdddefectiveBase = new DelegateCommand<object>(AdddefectiveBaseValue);

            DeleteSingleAtributeValueCommand = new DelegateCommand<object>(deleteDefectiveAttributeVale);

            _productService = productService ?? throw new ArgumentNullException(nameof(productService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            //SelectedProductChangeCommand = new DelegateCommand(SelectedProductChanged);

            AddAttributeSelectionCommand = new DelegateCommand<object>(AddAttributeSelection);

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            //_ = _eventAggregator.GetEvent<SingnalRProductTypeAddedEvent>().Subscribe(ProductTypeAddedEvent);

            //_ = _eventAggregator.GetEvent<SignalRProductTypeEnableDisableEvent>().Subscribe(ProductTypeEnableDisable);

            SelectedUnitTypeCommand = new DelegateCommand<object>(SelectedBaseUnit);

            SelectedSubUnitCommand = new DelegateCommand<object>(SelectedSubUnit);

            //LoadProductTypes();

            UnitTypes = GetUnitTypes();

            SubUnitTypes = GetSubUnitTypes();

            _ = eventAggregator.GetEvent<AddDefectiveQty>().Subscribe(GetDefectiveVisible);

            SearchTextChangedCommand = new DelegateCommand<string>(SearchTextChanged);

            SearchProductSelectionChangedCommand = new DelegateCommand(SearchSelectionChanged);

            ProductLocations = GetProductLocations();

            GSTslabs = GetGSTslabs();

            WarrantyServices = GetWarrantyServices();
            _ = eventAggregator.GetEvent<AddDiscountPercentToProductCard>().Subscribe(GetDiscountFromInvociePercent);


            _ = eventAggregator.GetEvent<AddDiscountFlatToProductCard>().Subscribe(GetDiscountFromInvocieFlat);

            defectiveQtyCommand = new DelegateCommand<object>(AddDefectiveQty);
            IsEditDropDown = true;
            IsAddandRemoveVisibility = true;
            if (Product != null && Product.ProductId != 0)
            {
                SearchTextChangedIndent(Product.SKU);
                ProductSearchModel product = new ProductSearchModel()
                {
                    ProductName = Product.ProductName,
                    ProductId = Product.ProductId,
                    ProductSKU = Product.SKU,
                   

                };
                SelectedProductSearch = product;
                SearchSelectionChanged();
                PoQty = Product.AvailableQty;
                PoRate = Product.EstimatedPrice;
               IsInclusiveGst= (Product.SelectedGSTslab.GstType != null&& Product.SelectedGSTslab.GstType=="Exclusive") ? false : true;
                PoGST = Product.SelectedGSTslab.GstValue;
            }
            ProductQty = 1;
            SelectedUnitType = "Load";



        }

        public void AddDefectiveQty(object obj)
        {
            Calculate();
        }
        private ObservableCollection<WarrantyService> GetWarrantyServices()
        {
            if (ApplicationSettings.WARRENTY_SERVICE != null && ApplicationSettings.WARRENTY_SERVICE.Any())
            {
                return new ObservableCollection<WarrantyService>(
                    ApplicationSettings.WARRENTY_SERVICE.Select(x => new WarrantyService { Name = x }));

            }

            return default;
        }

        private ObservableCollection<WarrantyService> _warrantyServices;

        public ObservableCollection<WarrantyService> WarrantyServices
        {
            get => _warrantyServices;
            set => SetProperty(ref _warrantyServices, value);
        }

        private WarrantyService _selectedWarrantyService;

        public WarrantyService SelectedWarrantyService
        {
            get => _selectedWarrantyService;
            set => SetProperty(ref _selectedWarrantyService, value);
        }



        private ObservableCollection<GSTslabs> GetGSTslabs()
        {
            if (ApplicationSettings.GST_VALUES != null && ApplicationSettings.GST_VALUES.Any())
            {
                return new ObservableCollection<GSTslabs>(ApplicationSettings
                    .GST_VALUES.Select(x => new GSTslabs { GstValue = x }));

            }

            return default;
        }


        private ObservableCollection<ProductLocation> GetProductLocations()
        {
            if (ApplicationSettings.PRODUCT_LOCATIONS != null && ApplicationSettings.PRODUCT_LOCATIONS.Any())
            {
                return new ObservableCollection<ProductLocation>
                (ApplicationSettings.PRODUCT_LOCATIONS.Select(x => new ProductLocation { Name = x }));
            }

            return default;
        }

        private ObservableCollection<ProductLocation> _productLocations;

        public ObservableCollection<ProductLocation> ProductLocations
        {
            get => _productLocations;
            set => SetProperty(ref _productLocations, value);
        }


        private ProductLocation _selectedLocation;

        public ProductLocation SelectedLocation
        {
            get => _selectedLocation;
            set => SetProperty(ref _selectedLocation, value);
        }

        private ObservableCollection<GSTslabs> _gstslabs;

        public ObservableCollection<GSTslabs> GSTslabs
        {
            get => _gstslabs;
            set => SetProperty(ref _gstslabs, value);
        }


        private GSTslabs _selectedGSTslab;

        public GSTslabs SelectedGSTslab
        {
            get => _selectedGSTslab;
            set
            {



                SetProperty(ref _selectedGSTslab, value);

                if (value != GSTslabs.FirstOrDefault())
                {
                    Calculate();
                }
                else
                {
                    ProductGSTPerQuantity = 0;
                    ProductGSTTotal = 0;
                    CalculateProductDiscount();
                    CaluclateProductDiscountrate();
                    calculateProductGST();
                    calculateProductRate();
                    calculateTotalProductGST();
                    CalculateProductTotal();

                }

            }
        }

        private async void SearchSelectionChanged()
        {

            try
            {
                if (SelectedProductSearch == null) return;

                await Task.Run(async () =>
                {
                    var _result = await _productService.GetSKUNumberProduct(SelectedProductSearch.ProductId,AppConstants.LoggedInStoreInfo.StoreId);

                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        if (_result != null && _result.IsSuccess && _result.Data != null)
                        {

                            //checking certificate added or not 
                            var certifcateProduct = GetCertifcateProduct();

                            if (certifcateProduct.Contains(_result.Data.Category.CategoryName.ToLower()))
                            {
                                if (!String.IsNullOrEmpty(_result.Data.Number))
                                {
                                    SelectedProduct = _result.Data;
      
                                    //ProductsSearchList?.Clear();
                                    SelectedProductChanged();
                                }
                                else
                                {

                                    _notificationService.ShowMessage("Please add product certificate for this product", Common.NotificationType.Error);

                                    return;
                                }
                            }
                            else
                            {

                                SelectedProduct = _result.Data;
                              
                                //ProductsSearchList?.Clear();
                                SelectedProductChanged();

                            }


                        }
                        else
                        {
                            _notificationService.ShowMessage(_result.Error, Common.NotificationType.Error);

                            return;
                        }
                    });

                });

            }
            catch (Exception _ex)
            {

                _logger.LogError("Error in getting product information", _ex);
            }

        }


        private bool _isDropDownOpen;
        public bool IsDropDownOpen
        {
            get { return _isDropDownOpen; }
            set { SetProperty(ref _isDropDownOpen, value); }
        }

        private bool _isEditDropDown;
        public bool IsEditDropDown
        {
            get { return _isEditDropDown; }
            set { SetProperty(ref _isEditDropDown, value); }
        }

        private bool _isAddandRemoveVisibility;
        public bool IsAddandRemoveVisibility
        {
            get { return _isAddandRemoveVisibility; }
            set { SetProperty(ref _isAddandRemoveVisibility, value); }
        }


        private ProductSearchModel _selectedProductSearch;
        public ProductSearchModel SelectedProductSearch
        {
            get { return _selectedProductSearch; }
            set { SetProperty(ref _selectedProductSearch, value); }
        }

        private ObservableCollection<ProductSearchModel> _productsSearchList;
        public ObservableCollection<ProductSearchModel> ProductsSearchList
        {
            get { return _productsSearchList; }
            set { SetProperty(ref _productsSearchList, value); }
        }


        private async void SearchTextChanged(string _searchText)
        {
            try
            {
                SelectedProduct = null;

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
                            IsEditDropDown = true;
                            GstHeaderPer = State == AppConstants.STATE ? AppConstants.GSTHEADERPER : AppConstants.IGSTHEADERPER;
                            GstHeaderQty = State == AppConstants.STATE ? AppConstants.GSTHEADERQTY : AppConstants.IGSTHEADERQTY;
                            GstHeaderTotal = State == AppConstants.STATE ? AppConstants.GSTHEADER : AppConstants.IGSTHEADER;

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
                _logger.LogWarning($"Task was cancelled in product search text {_searchText}");
            }
            catch (Exception _ex)
            {
                _logger.LogError($"Error in searching product by text {_searchText}", _ex);
            }
        }

        public void SelectedBaseUnit(object obj)
        {
            string headerBase = obj as string;
            DefectiveHeader = "Defective Base Unit(" + headerBase + ")";
            if (DefectiveList != null)
            {
                if (DefectiveList.Count > 0)
                {
                    DefectiveList.Clear();
                }
            }
        }

        public void SelectedSubUnit(object obj)
        {
            string headerBase = obj as string;
            DefectiveSubHeader = "Defective Sub Unit(" + headerBase + ")";
            if (DefectiveList != null)
            {
                if (DefectiveList.Count > 0)
                {
                    DefectiveList.Clear();
                }
            }

        }
        public void deleteDefectiveAttributeVale(object obj)
        {

            var _attribute = (obj as Dictionary<AttributeMap, AttributeMap>).Keys.FirstOrDefault();

            if (_attribute != null)
            {

                foreach (var item in DefectiveList)
                {
                    foreach (var i in item)
                    {
                        if (i.Key.AttributeValueName == _attribute.AttributeValueName && i.Key.AttributeValueId == _attribute.AttributeValueId)
                        {
                            DefectiveList.Remove(item);
                            return;
                        }
                    }
                }

            }
        }

        public void AdddefectiveBaseValue(object obj)
        {
            var _tuple = obj as Tuple<object, object>;
            var defectiveqty = SelectedProduct != null ? SelectedProduct.DefectiveQty : 0;
            var productqty = defectiveqty + ProductQty;
            if (productqty <= 0)
            {

                _notificationService.ShowMessage("Enter valid product quantity", Common.NotificationType.Error);
                DefectiveBase = null;
                DefectiveSubQty = null;
                return;
            }
            if (ProductSubQty <= 0)
            {

                _notificationService.ShowMessage("Enter valid product sub quantity", Common.NotificationType.Error);
                DefectiveBase = null;
                DefectiveSubQty = null;
                return;
            }
            if (string.IsNullOrEmpty(SelectedUnitType) || (string.IsNullOrEmpty(SelectedSubUnitType)))
            {
                _notificationService.ShowMessage("Please Select Unit Type and SubUnitType", Common.NotificationType.Error);
                DefectiveBase = null;
                DefectiveSubQty = null;
                return;
            }




            if (_tuple != null)
            {
                if (Convert.ToString(_tuple.Item1) == "" || Convert.ToString(_tuple.Item2) == "") return;
                var _attribute = Convert.ToInt32(_tuple.Item1);
                var _value = Convert.ToInt32(_tuple.Item2);

                if (_attribute == 0 || _value == 0) return;
                if (_attribute > ProductQty)
                {
                    _notificationService.ShowMessage("Defective base unit can not add more then unit qty ", Common.NotificationType.Error);
                    DefectiveBase = null;
                    DefectiveSubQty = null;
                    return;
                }
                if (_value > ProductSubQty)
                {
                    _notificationService.ShowMessage("Defective sub qty can not add more then sub qty ", Common.NotificationType.Error);
                    DefectiveBase = null;
                    DefectiveSubQty = null;
                    return;
                }
                Dictionary<AttributeMap, AttributeMap> keyValues = new Dictionary<AttributeMap, AttributeMap>();
                AttributeMap attributeMapkey = new AttributeMap();
                attributeMapkey.AttributeValueName = SelectedUnitType;
                attributeMapkey.AttributeValueId = _attribute;
                AttributeMap attributeMapkeyValue = new AttributeMap();
                attributeMapkeyValue.AttributeValueName = SelectedSubUnitType;
                attributeMapkeyValue.AttributeValueId = _value;
                keyValues.Add(attributeMapkey, attributeMapkeyValue);
                bool _isvaluePresent = false;
                DefectiveList.ToList().ForEach(x =>
                {
                    var _keys = x.Keys;

                    var _attributeName = _keys.FirstOrDefault();

                    if (_attributeName == null) return;



                    //check if same value is getting added.

                    x.Keys.ToList().ForEach(v =>
                   {
                       var _result = v.AttributeValueName == attributeMapkey.AttributeValueName && v.AttributeValueId == attributeMapkey.AttributeValueId;
                       if (_result)
                       {
                           _notificationService.ShowMessage("Defective Type is already added", NotificationType.Error);
                           _isvaluePresent = true;
                           return;
                       }
                   });


                });

                if (!_isvaluePresent)
                {
                    DefectiveList.Add(keyValues);
                    DefectiveBase = null;
                    DefectiveSubQty = null;

                }


            }


        }

        public void GetDiscountFromInvociePercent(string Discountpercent)
        {
            try
            {
                bool _IsDiscount = Discountpercent != "0" ? true : false;
                DiscountHeader = "Discount(%)";
                IsDiscount = _IsDiscount;
                //Discount = (float)Convert.ToDouble(Discountpercent);
                CalculationProductDiscountPercent();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public void GetDiscountFromInvocieFlat(string DiscountFalt)
        {
            try
            {
                bool _IsDiscount = DiscountFalt != "0" ? true : false;
                DiscountHeader = "Discount(Flat)";
                IsDiscount = _IsDiscount;
                Discount = (float)Convert.ToDouble(DiscountFalt);

                CalculationProductDiscountFlat();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public void CalculationProductDiscountFlat()
        {

            //if (IsGroupTrackMode)
            //{               
            //    var disocuntperamount = Discount / (ProductSubQty * ProductQty);
            //    DiscountedRate = ProductRate - disocuntperamount;
            //}
            //else
            //{
            //    var defectiveqty = SelectedProduct != null ? SelectedProduct.DefectiveQty : 0;
            //    var qty = ProductQty + defectiveqty;
            //    var disocuntperamount = Discount / qty;
            //    DiscountedRate = ProductRate - disocuntperamount;
            //}

            Discount = (float)Math.Round(GetTotalProductFlatAmount(), 2, MidpointRounding.AwayFromZero);

            Calculate();
        }


        public void CalculationProductDiscountPercent()
        {
            //    var discountamount = (Discount * ProductRate) / 100;
            //    DiscountedRate = ProductRate - discountamount;
            Calculate();
        }

        public void GetDefectiveVisible(bool isDefective)
        {
            if (SelectedProduct != null)
            {
                if (IsGroupTrackMode)
                {
                    SelectedProduct.IsDefectiveQty = false;
                    IsGroupDefectiveQty = isDefective;
                    InvoiceDefectiveQty = isDefective == true ? 1 : 0;
                    DefectiveHeader = SelectedUnitType != null ? "Defective Base Unit(" + SelectedUnitType + ")" : "Defective Base Unit Type";
                    DefectiveSubHeader = SelectedSubUnitType != null ? "Defective Sub Unit(" + SelectedSubUnitType + ")" : "Defective Sub Unit type";
                    if (!isDefective)
                    {
                        if (DefectiveList.Count > 0)
                        {
                            DefectiveList.Clear();
                        }

                    }
                }
                else
                {
                    SelectedProduct.IsDefectiveQty = isDefective;
                    InvoiceDefectiveQty = isDefective == true ? 1 : 0;
                    IsGroupDefectiveQty = false;

                    if (!isDefective)
                    {
                        SelectedProduct.DefectiveQty = 0;
                    }
                }

            }
        }



        private ObservableCollection<Dictionary<AttributeMap, AttributeMap>> _defectiveList = new ObservableCollection<Dictionary<AttributeMap, AttributeMap>>();
        public ObservableCollection<Dictionary<AttributeMap, AttributeMap>> DefectiveList
        {
            get { return _defectiveList; }
            set { SetProperty(ref _defectiveList, value); }
        }



        private ObservableCollection<string> GetSubUnitTypes()
        {
            return new ObservableCollection<string>
            {
                 "Packet","Bottle","Piece","Bucket","Bag","Tin"
            };
        }

        private ObservableCollection<string> GetUnitTypes()
        {
            return new ObservableCollection<string> {

                "Bag","Box","Bundle","Drum","Load"
            };
        }


        private string _selectedUnitType;
        public string SelectedUnitType
        {
            get { return _selectedUnitType; }
            set { SetProperty(ref _selectedUnitType, value); }
        }


        private int _productQty;
        public int ProductQty
        {
            get { return _productQty; }
            set
            {

                if (value <= 0)
                {
                    value = 1;
                }
                SetProperty(ref _productQty, value);

                ProductQtyChangeEvent?.Invoke(this, null);

                Calculate();


            }
        }

        private string _selectedSubUnitType;
        public string SelectedSubUnitType
        {
            get { return _selectedSubUnitType; }
            set { SetProperty(ref _selectedSubUnitType, value); }
        }

        private int _poQty;
        public int PoQty {
            get { return _poQty; }
            set { SetProperty(ref _poQty, value); }
        }

        private float _poRate;
        public float PoRate {
            get { return _poRate; }
            set { SetProperty(ref _poRate, value); }
        }
        private bool _isInclusiveGst;

        public bool IsInclusiveGst {
            get { return _isInclusiveGst; }
            set { SetProperty(ref _isInclusiveGst, value); }
        }

        private float _poGST;
        public float PoGST {
            get { return _poGST; }
            set { SetProperty(ref _poGST, value); }
        }


        private int _defectiveQty;
        public int  DefectiveQty {
            get { return _defectiveQty; }
            set { SetProperty(ref _defectiveQty, value); }
        }

        // public DelegateCommand ProductTypeChange { get; private set; }

        //public DelegateCommand ManufacturerChange { get; private set; }

        // public DelegateCommand SelectedProductChangeCommand { get; private set; }

        public DelegateCommand<object> AddAttributeSelectionCommand { get; private set; }


        public DelegateCommand<object> GenerateSerialCommand { get; private set; }



        private Guid _id;

        public Guid ID
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private DateTime? _dateOfManufacture;
        public DateTime? DateOfManufacture
        {
            get { return _dateOfManufacture; }
            set
            {
                SetProperty(ref _dateOfManufacture, value);

                //reset if is less then manufaturere data
                if (DateOfExpiry != null && DateOfExpiry < DateOfManufacture)
                    DateOfExpiry = null;

            }
        }

        //public ObservableCollection<ProductDetails> ProductDetailsList
        //{
        //    get { return _productDetailsList; }
        //    set { SetProperty(ref _productDetailsList, value); }
        //}
        //private ObservableCollection<ProductType> _productTypes = new ObservableCollection<ProductType>();
        //public ObservableCollection<ProductType> ProductTypes
        //{
        //    get { return _productTypes; }
        //    set { SetProperty(ref _productTypes, value); }
        //}
        //private ProductType _selectedProductType;
        //public ProductType SelectedProductType
        //{
        //    get { return _selectedProductType; }
        //    set
        //    {
        //        SetProperty(ref _selectedProductType, value);

        //    }
        //}

        public ObservableCollection<SerialNumber> SerialNoList
        {
            get { return _serialNoList; }
            set { SetProperty(ref _serialNoList, value); }
        }

        //private string _location;
        //public string Location
        //{
        //    get { return _location; }
        //    set { SetProperty(ref _location, value); }
        //}


        private bool _isSerialNumberManual;
        public bool IsSerialNumberManual
        {
            get { return _isSerialNumberManual; }
            set { SetProperty(ref _isSerialNumberManual, value); }
        }

        public List<ProductAttributeMapping> AttributesSelectedList { get; set; } = new List<ProductAttributeMapping>();

        private Manufacturer _selectedManufacturer;
        public Manufacturer SelectedManufacturer
        {
            get { return _selectedManufacturer; }
            set { SetProperty(ref _selectedManufacturer, value); }

        }


        private InventaryProductViewModel _selectedProduct;
        public InventaryProductViewModel SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);

        }



        //private ObservableCollection<Manufacturer> _manufacturers = new ObservableCollection<Manufacturer>();
        //public ObservableCollection<Manufacturer> Manufacturers
        //{
        //    get { return _manufacturers; }
        //    set { SetProperty(ref _manufacturers, value); }
        //}


        private DateTime? _dateOfExpiry;
        public DateTime? DateOfExpiry
        {
            get { return _dateOfExpiry; }
            set
            {

                //Block setting up of previous dates.

                if (value != null && value < DateTime.Now)
                {
                    value = null;
                    _notificationService.ShowMessage("Expiry date cannot be older than today", Common.NotificationType.Error);
                }
                SetProperty(ref _dateOfExpiry, value);
            }
        }


        //private string _warrantyService;
        //public string WarrantyService
        //{
        //    get { return _warrantyService; }
        //    set { SetProperty(ref _warrantyService, value); }
        //}


        private int _ProductSubQty;
        public int ProductSubQty
        {
            get { return _ProductSubQty; }
            set
            {
                SetProperty(ref _ProductSubQty, value);

                ProductQtyChangeEvent?.Invoke(this, null);
                Calculate();


            }
        }

        private string _defectiveheader;
        public string DefectiveHeader
        {
            get { return _defectiveheader; }
            set { SetProperty(ref _defectiveheader, value); }
        }

        private string _defectivesubheader;
        public string DefectiveSubHeader
        {
            get { return _defectivesubheader; }
            set { SetProperty(ref _defectivesubheader, value); }
        }
        private string _defectiveBase;
        public string DefectiveBase
        {
            get { return _defectiveBase; }
            set { SetProperty(ref _defectiveBase, value); }
        }

        private string _defectiveSubQty;
        public string DefectiveSubQty
        {
            get { return _defectiveSubQty; }
            set { SetProperty(ref _defectiveSubQty, value); }
        }

        private float _productRate;
        public float ProductRate
        {
            get { return _productRate; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }

                SetProperty(ref _productRate, value);
                if (value > 0)
                {
                    if (IsDiscount)
                    {
                        if (DiscountHeader == "Discount(%)")
                        {
                            CalculationProductDiscountPercent();
                        }
                        if (DiscountHeader == "Discount(Flat)")
                        {
                            CalculationProductDiscountFlat();
                        }
                    }

                    Calculate();
                }
                else
                {
                    ResetPrice();
                }


            }
        }

        private void calculateProductRate()
        {

            if (IsGroupTrackMode)
            {

                if (SelectedGSTslab != null)
                {
                    if (ApplyDiscountType == ApplyDiscountType.AFTER)
                    {
                        //discount % ;
                        if (DiscountHeader == PercentDiscountHeader)
                        {

                            //TODO: Need to check this case .Bug

                            float _amount = ProductRate + (ProductRate * SelectedGSTslab.GstValue / 100);

                            float _discountAmountRate = _amount * InvoiceLevelDiscountPercent / 100;// GetDiscountAmountPercent();

                            float _totalProdQtyrate = (_amount - _discountAmountRate) * ProductQty * ProductSubQty;

                            ProductTTotalQuantityRate = (float)Math.Round((ProductRate * ProductQty * ProductSubQty), 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {

                            //Discount flat amount.

                            if (GetTotalProductFlatAmount() > 0 || InvoiceLevelDiscountPercent > 0)
                            {
                                float _amount = ProductRate + ProductRate * SelectedGSTslab.GstValue / 100;

                                float _discountAmountRate = GetTotalProductFlatAmount();

                                float _totalProdQtyrate = (_amount - _discountAmountRate) * ProductQty * ProductSubQty;

                                ProductTTotalQuantityRate = (float)Math.Round((ProductRate * ProductQty * ProductSubQty), 2, MidpointRounding.AwayFromZero);
                            }
                            else
                            {
                                //float _amount = ProductRate + ProductRate * SelectedGSTslab.GstValue / 100;

                                //float _discountAmountRate = GetTotalProductFlatAmount();

                                float _totalProdQtyrate = ProductRate * ProductQty * ProductSubQty;

                                ProductTTotalQuantityRate = (float)Math.Round(_totalProdQtyrate, 2, MidpointRounding.AwayFromZero);
                            }


                        }
                    }
                    else
                    {
                        //percent discount
                        if (DiscountHeader == PercentDiscountHeader)
                        {
                            float _discountAmount = GetDiscountAmountPercent();

                            float _sub = ProductRate - _discountAmount;

                            //float _gstApply = _sub * (SelectedGSTslab.GstValue / 100);

                            ProductTTotalQuantityRate = (float)Math.Round((ProductRate * ProductQty * ProductSubQty), 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            //flat discount.

                            float _discountAmount = GetTotalProductFlatAmount();

                            float _sub = ProductRate - _discountAmount;

                            //float _gstApply = _sub * (SelectedGSTslab.GstValue / 100);

                            ProductTTotalQuantityRate = (float)Math.Round((ProductRate * ProductQty * ProductSubQty), 2, MidpointRounding.AwayFromZero);
                        }
                    }
                }
                else
                {
                    //no GST dropdown selected default calculate.
                    if (ApplyDiscountType == ApplyDiscountType.AFTER)
                    {

                        if (DiscountHeader == PercentDiscountHeader)
                        {

                            float _amount = ProductRate;

                            float _discountAmountRate = GetDiscountAmountPercent();

                            float _totalProdQtyrate = (_amount - _discountAmountRate) * ProductQty * ProductSubQty;

                            ProductTTotalQuantityRate = (float)Math.Round((ProductRate * ProductQty * ProductSubQty), 2, MidpointRounding.AwayFromZero);

                        }
                        else
                        {
                            float _amount = ProductRate;

                            float _discountAmountRate = GetTotalProductFlatAmount();

                            float _totalProdQtyrate = (_amount - _discountAmountRate) * ProductQty * ProductSubQty;

                            ProductTTotalQuantityRate = (float)Math.Round((ProductRate * ProductQty * ProductSubQty), 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {

                        if (DiscountHeader == PercentDiscountHeader)
                        {

                        }
                        else
                        {

                            //no gst  // flat. //apply type before..

                            float _rate = ProductRate;
                            float _discountAmount = GetTotalProductFlatAmount();
                            float _totalRAteAmount = (_rate - _discountAmount) * ProductQty * ProductSubQty;

                            ProductTTotalQuantityRate = (float)Math.Round((ProductRate * ProductQty * ProductSubQty), 2, MidpointRounding.AwayFromZero);
                        }

                    }
                }
            }
            else
            {
                var defectiveqty = SelectedProduct != null ? SelectedProduct.DefectiveQty : 0;
                var qty = ProductQty + defectiveqty;

                if (SelectedGSTslab != null)
                {
                    if (ApplyDiscountType == ApplyDiscountType.AFTER)
                    {
                        //discount %
                        if (DiscountHeader == "Discount(%)")
                        {





                        }
                        else
                        {

                            //Discount flat amount.
                            float _amount = ProductRate + ProductRate * SelectedGSTslab.GstValue / 100;

                            float _discountAmountRate = GetTotalProductFlatAmount();

                            float _totalProdQtyrate = (_amount - _discountAmountRate) * qty;

                            ProductTTotalQuantityRate = (float)Math.Round((ProductRate * ProductQty * ProductSubQty), 2, MidpointRounding.AwayFromZero);

                        }
                    }
                    else
                    {

                    }
                }
                else
                {
                    //no GST dropdown selected default calculate.
                    if (ApplyDiscountType == ApplyDiscountType.AFTER)
                    {

                        if (DiscountHeader == "Discount(%)")
                        {

                        }
                        else
                        {
                            float _amount = ProductRate;

                            float _discountAmountRate = GetTotalProductFlatAmount();

                            float _totalProdQtyrate = (_amount - _discountAmountRate) * qty;

                            ProductTTotalQuantityRate = (float)Math.Round((ProductRate * ProductQty * ProductSubQty), 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {

                    }
                }
            }

            #region OldCode-InvoiceDiscountType change



            //if (IsGroupTrackMode)
            //{
            //    if (Discount > 0)
            //    {
            //        //invoice level disocuntrate
            //        ProductTTotalQuantityRate = ProductSubQty * DiscountedRate * ProductQty;
            //    }
            //    else
            //    {
            //        ProductTTotalQuantityRate = ProductSubQty * ProductRate * ProductQty;
            //    }

            //}
            //else
            //{
            //    var defectiveqty = SelectedProduct != null ? SelectedProduct.DefectiveQty : 0;
            //    var qty = ProductQty + defectiveqty;
            //    if (Discount > 0)
            //    {
            //        //invoice level disocuntrate
            //        ProductTTotalQuantityRate = qty * DiscountedRate;
            //    }
            //    else
            //    {
            //        ProductTTotalQuantityRate = qty * ProductRate;
            //    }


            //}

            #endregion

        }

        /// <summary>
        /// Returns the Total Invoice level discount amount divided by / total product qty.
        /// </summary>
        /// <returns></returns>
        private float GetTotalProductFlatAmount()
        {
            if (TotalProductQtyCount == 0) return 0;
            try
            {
                return InvoiceLevelDiscountFlatAmount / TotalProductQtyCount;
            }
            catch (Exception)
            {
            }

            return 0;
        }

        private ObservableCollection<string> _unitTypes;
        public ObservableCollection<string> UnitTypes
        {
            get { return _unitTypes; }
            set { SetProperty(ref _unitTypes, value); }
        }


        private ObservableCollection<string> _subUnitTypes;
        public ObservableCollection<string> SubUnitTypes
        {
            get { return _subUnitTypes; }
            set { SetProperty(ref _subUnitTypes, value); }
        }


        private bool _isGroupDefectvieQty;

        public bool IsGroupDefectiveQty
        {
            get { return _isGroupDefectvieQty; }
            set { SetProperty(ref _isGroupDefectvieQty, value); }
        }

        private float _productTTotalQuantityRate;

        public float ProductTTotalQuantityRate
        {
            get => _productTTotalQuantityRate;
            set
            {
                SetProperty(ref _productTTotalQuantityRate, value);

            }
        }

        private float _productGSTPerQuantity;

        public float ProductGSTPerQuantity
        {
            get => _productGSTPerQuantity;
            set
            {
                SetProperty(ref _productGSTPerQuantity, value);

            }
        }


        private float _productMRP;
        public float ProductMRP
        {
            get => _productMRP;
            set
            {
                SetProperty(ref _productMRP, value);
            }
        }

        private float _productTotal;

        public float ProductTotal
        {
            get => _productTotal;
            set
            {

                SetProperty(ref _productTotal, value);
            }
        }

        private void CalculateProductTotal()
        {
            if (SelectedGSTslab != null)
            {
                if (IsGroupTrackMode)
                {
                    ProductTotal = (float)Math.Round(GetProductTotalWhenGST(), 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    //TODO check for def and single case.
                    var defectiveqty = SelectedProduct != null ? SelectedProduct.DefectiveQty : 0;
                    var qty = (ProductQty*ProductSubQty) + defectiveqty;
                    if (Discount > 0)
                    {
                        //invoice level discount
                        //ProductTotal = (qty * DiscountedRate * SelectedGSTslab.GstValue) / 100 + DiscountedRate * qty;
                        ProductTotal= (float)Math.Round(GetProductTotalWhenGST(), 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        ProductTotal = (qty * ProductRate * SelectedGSTslab.GstValue) / 100 + ProductRate * qty;
                    }

                }
            }
            else
            {

                if (IsGroupTrackMode)
                {
                    ProductTotal = GetProductTotalWhenNoGST();
                }
                else
                {
                    //TODO.single product type grouping

                }

            }
            #region Old Code Invoice discount type

            //if (SelectedGSTslab != null && SelectedGSTslab.GstValue > 0)
            //{

            //    if (IsGroupTrackMode)
            //    {

            //        if (Discount > 0)
            //        {
            //            //invoice level discount calculation
            //            ProductTotal = (ProductSubQty * DiscountedRate * SelectedGSTslab.GstValue * ProductQty) / 100 + DiscountedRate * ProductSubQty * ProductQty;

            //        }
            //        else
            //        {
            //            ProductTotal = (ProductSubQty * ProductRate * SelectedGSTslab.GstValue * ProductQty) / 100 + ProductRate * ProductSubQty * ProductQty;
            //        }

            //    }
            //    else
            //    {
            //        var defectiveqty = SelectedProduct != null ? SelectedProduct.DefectiveQty : 0;
            //        var qty = ProductQty + defectiveqty;
            //        if (Discount > 0)
            //        {
            //            //invoice level discount
            //            ProductTotal = (qty * DiscountedRate * SelectedGSTslab.GstValue) / 100 + DiscountedRate * qty;
            //        }
            //        else
            //        {
            //            ProductTotal = (qty * ProductRate * SelectedGSTslab.GstValue) / 100 + ProductRate * qty;
            //        }

            //    }


            //}
            //else
            //{
            //    if (IsGroupTrackMode)
            //    {
            //        if (Discount > 0)
            //        {
            //            //invoice level discount 
            //            ProductTotal = ProductSubQty * DiscountedRate * ProductQty;
            //        }
            //        else
            //        {
            //            ProductTotal = ProductSubQty * ProductRate * ProductQty;
            //        }

            //    }
            //    else
            //    {
            //        var defectiveqty = SelectedProduct != null ? SelectedProduct.DefectiveQty : 0;
            //        var qty = ProductQty + defectiveqty;
            //        if (Discount > 0)
            //        {
            //            //invoice level discount 
            //            ProductTotal = qty * DiscountedRate;
            //        }
            //        else
            //        {
            //            ProductTotal = qty * ProductRate;
            //        }
            //    }


            //}
            #endregion

        }

        private float GetProductTotalWhenNoGST()
        {
            try
            {
                if (ApplyDiscountType == ApplyDiscountType.AFTER)
                {
                    //discount percent
                    if (DiscountHeader == PercentDiscountHeader)
                    {

                        float _discountAmountFlat = GetDiscountAmountPercent();

                        float _amountGST = ProductRate;

                        float _prodT = _amountGST - _discountAmountFlat;

                        return _prodT * ProductQty * ProductSubQty;

                    }
                    else
                    {
                        //discount flat.

                        float _discountAmountFlat = GetTotalProductFlatAmount();

                        float _amountGST = ProductRate;

                        float _prodT = _amountGST - _discountAmountFlat;

                        return _prodT * ProductQty * ProductSubQty;

                    }
                }
                else
                {
                    if (DiscountHeader == PercentDiscountHeader)
                    {

                        float _discountAmount = ProductRate * InvoiceLevelDiscountPercent / 100;


                        float _discountedRate = ProductRate - _discountAmount;

                        return (float)Math.Round(_discountedRate * ProductQty * ProductSubQty, 2, MidpointRounding.AwayFromZero);


                    }
                    else
                    {
                        //discount flat.

                        //float _discountAmountFlat = GetTotalProductFlatAmount();

                        //float _amountGST = ProductRate;

                        //float _prodT = _amountGST - _discountAmountFlat;

                        return ProductRate * ProductQty * ProductSubQty + ProductGSTTotal - InvoiceLevelDiscountFlatAmount;





                    }

                }
            }
            catch (Exception)
            {

            }

            return default;
        }

        /// <summary>
        /// Calculates the product total (SelectedGSTslab value should not be null.)
        /// </summary>
        /// <returns>Prouct Total amount or default</returns>
        private float GetProductTotalWhenGST()
        {
            try
            {

                if (ApplyDiscountType == ApplyDiscountType.AFTER)
                {
                    //discount percent
                    if (DiscountHeader == PercentDiscountHeader)
                    {
                        //float _discountAmountFlat = GetDiscountAmountPercent();

                        //float _amountGST = ProductRate + ProductRate * (SelectedGSTslab.GstValue / 100);

                        //float _prodT = _amountGST - _discountAmountFlat;

                        //return _prodT * ProductQty * ProductSubQty;

                        float _gstAmount = ProductRate * SelectedGSTslab.GstValue / 100;

                        float _rateWithGSt = _gstAmount + ProductRate;


                        float _discountPercentApplied = (_gstAmount + ProductRate) * InvoiceLevelDiscountPercent / 100;


                        return (float)Math.Round((_rateWithGSt - _discountPercentApplied) * ProductSubQty * ProductQty, 2, MidpointRounding.AwayFromZero);




                    }
                    else
                    {
                        //discount flat.

                        float _discountAmountFlat = GetTotalProductFlatAmount();

                        float _amountGST = ProductRate + ProductRate * (SelectedGSTslab.GstValue / 100);

                        float _prodT = _amountGST - _discountAmountFlat;

                        return _prodT * ProductQty * ProductSubQty;

                    }
                }
                else
                {

                    //discount percent
                    if (DiscountHeader == PercentDiscountHeader)
                    {

                        float _prodRateTotal = ProductRate * ProductSubQty * ProductQty;

                        float _producttotalGst = ProductGSTTotal;

                        //TODO check Discount or flat /qty amount??
                        return _prodRateTotal + _productGSTTotal - (GetDiscountAmountPercent() * ProductSubQty * ProductQty);//GetTotalProductFlatAmount();
                    }
                    else
                    {

                        //flat discount amount.

                        float _prodRateTotal = (ProductRate - GetTotalProductFlatAmount()) * ProductSubQty * ProductQty;

                        float _producttotalGst = ProductGSTTotal;

                        //TODO check Discount or flat /qty amount??
                        return _prodRateTotal + _productGSTTotal;// - GetTotalProductFlatAmount();


                    }
                }
            }
            catch (Exception)
            {

            }

            return default;

        }

        private float _discount;

        public float Discount
        {
            get { return _discount; }
            set { SetProperty(ref _discount, value); }
        }

        private float _discountedRate;

        public float DiscountedRate
        {
            get { return _discountedRate; }
            set { SetProperty(ref _discountedRate, value); }
        }

        private string _discountheader;
        public string DiscountHeader
        {
            get { return _discountheader; }
            set { SetProperty(ref _discountheader, value); }
        }

        private bool _isDiscount;
        public bool IsDiscount
        {
            get { return _isDiscount; }
            set { SetProperty(ref _isDiscount, value); }
        }

        private void Calculate()
        {

            //check for zero qty 
            //reset price if zero or less.    
            int defectiveqty = SelectedProduct != null ? SelectedProduct.DefectiveQty : 0;
            int productQty = ProductQty + defectiveqty;
            if (productQty <= 0)
            {
                ProductRate = 0;
                ResetPrice();
            }

            if (ProductSubQty > 0 && ProductRate > 0 || (productQty > 0 && !IsGroupTrackMode))
            {
                CalculateProductDiscount();
                CaluclateProductDiscountrate();
                calculateProductGST();
                calculateProductRate();
                calculateTotalProductGST();
                CalculateProductTotal();
            }
            else
            {
                ProductRate = 0;
                ResetPrice();
            }

        }

        private void CalculateProductDiscount()
        {

            if (InvoiceLevelDiscountFlatAmount == 0 && InvoiceLevelDiscountPercent == 0)
            {
                Discount = 0;
            }

            if (InvoiceLevelDiscountFlatAmount != 0)
            {

                if (DiscountHeader == PercentDiscountHeader)
                {

                }
                else
                {
                    Discount = (float)Math.Round(GetTotalProductFlatAmount(), 2, MidpointRounding.AwayFromZero);
                }



            }
            else
            {
                //discount amount is in percent.
                if (DiscountHeader == PercentDiscountHeader)
                {
                    if (ApplyDiscountType == ApplyDiscountType.AFTER)
                    {

                        //gst is selected
                        if (SelectedGSTslab != null)
                        {
                            float _sumRate = ProductRate + ProductRate * SelectedGSTslab.GstValue / 100;

                            Discount = (float)Math.Round(_sumRate * InvoiceLevelDiscountPercent / 100, 2, MidpointRounding.AwayFromZero);


                        }
                        else
                        {
                            //no gst is selected.
                            Discount = (float)Math.Round(ProductRate * InvoiceLevelDiscountPercent / 100, 2, MidpointRounding.AwayFromZero);

                        }

                    }
                    else
                    {
                        Discount = (float)Math.Round(GetDiscountAmountPercent(), 2, MidpointRounding.AwayFromZero);
                    }



                }

            }
            IsDiscount = IsInvoiceDiscountAvailable();
        }

        //private const string PercentDiscountHeader = "Discount(%)";

        //private void CaluclateProductDiscountrate()
        //{
        //    //GST has a value.
        //    if (SelectedGSTslab != null)
        //    {
        //        if (IsGroupTrackMode)
        //        {

        //            if (ApplyDiscountType == ApplyDiscountType.AFTER)
        //            {

        //                if (DiscountHeader == PercentDiscountHeader)
        //                {

        //                }
        //                else
        //                {
        //                    //flat discount

        //                    float _discountAmountTotal = GetTotalProductFlatAmount();

        //                    float _gstAmount = ProductRate + ProductRate * SelectedGSTslab.GstValue / 100;

        //                    DiscountedRate =(float)Math.Round( _gstAmount - _discountAmountTotal,2,MidpointRounding.AwayFromZero);

        //                }                    
        //            }
        //            else
        //            {
        //                //discount before.
        //                //vendor 2 case . discount type before GST("Here Discount is deducted from the Sub Total and then GST is added")

        //                // percent discount
        //                if (DiscountHeader == PercentDiscountHeader)
        //                {

        //                }
        //                else
        //                {

        //                    //flat discount.

        //                    float _discountFlatamt = GetTotalProductFlatAmount();

        //                    float _rateSub = ProductRate - _discountFlatamt;

        //                    //no negative values.
        //                    if (_rateSub < 0)
        //                    {
        //                        _rateSub = 0;
        //                    }
        //                    DiscountedRate = (float)Math.Round(_rateSub, 2, MidpointRounding.AwayFromZero);
        //                }
        //            }


        //        }
        //        else
        //        {
        //            //single product.
        //        }
        //    }
        //    else
        //    {


        //        if (DiscountHeader == PercentDiscountHeader)
        //        {

        //        }
        //        else
        //        { 
        //            //no gst available.
        //            float _noGST = ProductRate;

        //            float _discountAmount = GetTotalProductFlatAmount();

        //            DiscountedRate = (float)Math.Round(_noGST - _discountAmount, 2, MidpointRounding.AwayFromZero);

        //            IsDiscount = _discountAmount > 0;

        //            Discount =(float)Math.Round(GetTotalProductFlatAmount(),2,MidpointRounding.AwayFromZero);

        //        }



        //    }

        //    //if (IsGroupTrackMode)
        //    //{               
        //    //    var disocuntperamount = Discount / (ProductSubQty * ProductQty);
        //    //    DiscountedRate = ProductRate - disocuntperamount;
        //    //}
        //    //else
        //    //{
        //    //    var defectiveqty = SelectedProduct != null ? SelectedProduct.DefectiveQty : 0;
        //    //    var qty = ProductQty + defectiveqty;
        //    //    var disocuntperamount = Discount / qty;
        //    //    DiscountedRate = ProductRate - disocuntperamount;
        //    //}
        //}

        private const string PercentDiscountHeader = "Discount(%)";

        private void CaluclateProductDiscountrate()
        {
            //GST has a value.
            if (SelectedGSTslab != null)
            {
                if (IsGroupTrackMode)
                {

                    if (ApplyDiscountType == ApplyDiscountType.AFTER)
                    {

                        if (DiscountHeader == PercentDiscountHeader)
                        {

                            float _gstAmount = ProductRate * SelectedGSTslab.GstValue / 100;

                            float _rateWithGSt = _gstAmount + ProductRate;


                            float _discountPercentApplied = (_gstAmount + ProductRate) * InvoiceLevelDiscountPercent / 100;


                            DiscountedRate = (float)Math.Round(_rateWithGSt - _discountPercentApplied, 2, MidpointRounding.AwayFromZero);

                            //float _discountAmountTotal = GetDiscountAmountPercent();

                            //float _gstAmount = ProductRate + ProductRate * SelectedGSTslab.GstValue / 100;

                            //DiscountedRate = (float)Math.Round(_gstAmount - _discountAmountTotal, 2, MidpointRounding.AwayFromZero);

                        }
                        else
                        {
                            //flat discount

                            float _discountAmountTotal = GetTotalProductFlatAmount();

                            float _gstAmount = ProductRate + ProductRate * SelectedGSTslab.GstValue / 100;

                            DiscountedRate = (float)Math.Round(_gstAmount - _discountAmountTotal, 2, MidpointRounding.AwayFromZero);

                        }
                    }
                    else
                    {
                        //discount before.
                        //vendor 2 case . discount type before GST("Here Discount is deducted from the Sub Total and then GST is added")

                        // percent discount
                        if (DiscountHeader == PercentDiscountHeader)
                        {


                            float _discountPercentAmount = GetDiscountAmountPercent();

                            float _rateSub = ProductRate - _discountPercentAmount;

                            if (_rateSub < 0)
                            {
                                _rateSub = 0;
                            }
                            DiscountedRate = (float)Math.Round(_rateSub, 2, MidpointRounding.AwayFromZero);


                        }
                        else
                        {

                            //flat discount.

                            float _discountFlatamt = GetTotalProductFlatAmount();

                            float _rateSub = ProductRate - _discountFlatamt;

                            //no negative values.
                            if (_rateSub < 0)
                            {
                                _rateSub = 0;
                            }
                            DiscountedRate = (float)Math.Round(_rateSub, 2, MidpointRounding.AwayFromZero);
                        }
                    }


                }
                else
                {
                    //single product.
                    if (ApplyDiscountType == ApplyDiscountType.AFTER)
                    {

                        if (DiscountHeader == PercentDiscountHeader)
                        {

                            float _gstAmount = ProductRate * SelectedGSTslab.GstValue / 100;

                            float _rateWithGSt = _gstAmount + ProductRate;


                            float _discountPercentApplied = (_gstAmount + ProductRate) * InvoiceLevelDiscountPercent / 100;


                            DiscountedRate = (float)Math.Round(_rateWithGSt - _discountPercentApplied, 2, MidpointRounding.AwayFromZero);

                            //float _discountAmountTotal = GetDiscountAmountPercent();

                            //float _gstAmount = ProductRate + ProductRate * SelectedGSTslab.GstValue / 100;

                            //DiscountedRate = (float)Math.Round(_gstAmount - _discountAmountTotal, 2, MidpointRounding.AwayFromZero);

                        }
                        else
                        {
                            //flat discount

                            float _discountAmountTotal = GetTotalProductFlatAmount();

                            float _gstAmount = ProductRate + ProductRate * SelectedGSTslab.GstValue / 100;

                            DiscountedRate = (float)Math.Round(_gstAmount - _discountAmountTotal, 2, MidpointRounding.AwayFromZero);

                        }
                    }
                    else
                    {
                        //discount before.
                        //vendor 2 case . discount type before GST("Here Discount is deducted from the Sub Total and then GST is added")

                        // percent discount
                        if (DiscountHeader == PercentDiscountHeader)
                        {


                            float _discountPercentAmount = GetDiscountAmountPercent();

                            float _rateSub = ProductRate - _discountPercentAmount;

                            if (_rateSub < 0)
                            {
                                _rateSub = 0;
                            }
                            DiscountedRate = (float)Math.Round(_rateSub, 2, MidpointRounding.AwayFromZero);


                        }
                        else
                        {

                            //flat discount.

                            float _discountFlatamt = GetTotalProductFlatAmount();

                            float _rateSub = ProductRate - _discountFlatamt;

                            //no negative values.
                            if (_rateSub < 0)
                            {
                                _rateSub = 0;
                            }
                            DiscountedRate = (float)Math.Round(_rateSub, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                }
            }
            else
            {


                if (DiscountHeader == PercentDiscountHeader)
                {
                    //percent discount.

                    if (ApplyDiscountType == ApplyDiscountType.AFTER)
                    {
                        float _noGstRate = ProductRate;

                        float _discountAmount = GetDiscountAmountPercent();

                        DiscountedRate = (float)Math.Round(_noGstRate - _discountAmount, 2, MidpointRounding.AwayFromZero);

                        IsDiscount = _discountAmount > 0;

                        Discount = (float)Math.Round(_discountAmount, 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        float _noGstRate = ProductRate;

                        float _discountAmount = GetDiscountAmountPercent();

                        DiscountedRate = (float)Math.Round(_noGstRate - _discountAmount, 2, MidpointRounding.AwayFromZero);

                        IsDiscount = _discountAmount > 0;

                        Discount = (float)Math.Round(_discountAmount, 2, MidpointRounding.AwayFromZero);
                    }


                }
                else
                {
                    //no gst available.
                    float _noGST = ProductRate;

                    float _discountAmount = GetTotalProductFlatAmount();

                    DiscountedRate = (float)Math.Round(_noGST - _discountAmount, 2, MidpointRounding.AwayFromZero);

                    IsDiscount = _discountAmount > 0;

                    Discount = (float)Math.Round(GetTotalProductFlatAmount(), 2, MidpointRounding.AwayFromZero);

                }



            }

            //if (IsGroupTrackMode)
            //{               
            //    var disocuntperamount = Discount / (ProductSubQty * ProductQty);
            //    DiscountedRate = ProductRate - disocuntperamount;
            //}
            //else
            //{
            //    var defectiveqty = SelectedProduct != null ? SelectedProduct.DefectiveQty : 0;
            //    var qty = ProductQty + defectiveqty;
            //    var disocuntperamount = Discount / qty;
            //    DiscountedRate = ProductRate - disocuntperamount;
            //}
        }
        /// <summary>
        /// Gets the discount amount before GST  ProductRate * InvoiceLevelDiscountPercent / 100;
        /// </summary>
        /// <returns></returns>
        private float GetDiscountAmountPercent()
        {
            return ProductRate * InvoiceLevelDiscountPercent / 100;
        }

        private void ResetPrice()
        {

            ProductTTotalQuantityRate = 0;
            //SelectedGSTslab = GSTslabs.FirstOrDefault();
            ProductGSTPerQuantity = 0;
            ProductGSTTotal = 0;
            ProductMRP = 0;
            ProductTotal = 0;
            DiscountedRate = 0;
            Discount = 0;
            IsDiscount = false;

        }

        private void calculateTotalProductGST()
        {

            if (SelectedGSTslab != null)
            {
                if (ApplyDiscountType == ApplyDiscountType.AFTER)
                {
                    ProductGSTTotal = ProductRate * (SelectedGSTslab.GstValue / 100) * ProductQty * ProductSubQty;
                }
                else
                {
                    //Discount %
                    if (DiscountHeader == PercentDiscountHeader)
                    {

                        float _totalProductsDiscount = GetDiscountAmountPercent();

                        var _afterDisount = ProductRate - _totalProductsDiscount;

                        ProductGSTTotal = (float)Math.Round(_afterDisount * ProductQty * ProductSubQty * (SelectedGSTslab.GstValue / 100), 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        //flat 

                        float _totalProductsDiscount = GetTotalProductFlatAmount();

                        var _afterDisount = ProductRate - _totalProductsDiscount;

                        ProductGSTTotal = (float)Math.Round(_afterDisount * ProductQty * ProductSubQty * (SelectedGSTslab.GstValue / 100), 2, MidpointRounding.AwayFromZero);
                    }


                }
            }
            else
            {
                ProductGSTTotal = 0;
            }


            ProductGSTTotal = (float)Math.Round(ProductGSTTotal, 2, MidpointRounding.AwayFromZero);












            #region Old Code Invoice Discountype changes

            //if (SelectedGSTslab != null && SelectedGSTslab.GstValue > 0)
            //{
            //    if (IsGroupTrackMode)
            //    {
            //        if (Discount > 0)
            //        {
            //            //invocie level discountrate
            //            ProductGSTTotal = (ProductSubQty * DiscountedRate * SelectedGSTslab.GstValue * ProductQty) / 100;
            //        }
            //        else
            //        {
            //            ProductGSTTotal = (ProductSubQty * ProductRate * SelectedGSTslab.GstValue * ProductQty) / 100;
            //        }
            //    }
            //    else
            //    {
            //        var defectiveqty = SelectedProduct != null ? SelectedProduct.DefectiveQty : 0;
            //        var qty = ProductQty + defectiveqty;
            //        if (Discount > 0)
            //        {
            //            //invocie level discountrate
            //            ProductGSTTotal = (qty * DiscountedRate * SelectedGSTslab.GstValue) / 100;
            //        }
            //        else
            //        {
            //            ProductGSTTotal = (qty * ProductRate * SelectedGSTslab.GstValue) / 100;
            //        }

            //    }


            //}
            //else
            //{
            //    ProductGSTTotal = 0;
            //}

            #endregion


        }

        //private int _selectedGSTIndex;
        //public int SelectedGSTIndex
        //{
        //    get => _selectedGSTIndex;
        //    set => SetProperty(ref _selectedGSTIndex, value);
        //}

        private float _productGSTTotal;
        public float ProductGSTTotal
        {
            get => _productGSTTotal;
            set => SetProperty(ref _productGSTTotal, value);
        }

        public float InvoiceLevelDiscountFlatAmount { get; set; }

        private void calculateProductGST()
        {
            //have gst selected and Product rate is non zero
            if (SelectedGSTslab != null && ProductRate > 0)
            {

                if (ApplyDiscountType == ApplyDiscountType.AFTER)
                {
                    //apply gst to product rate.
                    ProductGSTPerQuantity = (float)Math.Round(ProductRate * SelectedGSTslab.GstValue / 100, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    //subtract discount before applying discount.
                    //discount is in %
                    if (DiscountHeader == PercentDiscountHeader)
                    {

                        float _totalProductsDiscount = GetDiscountAmountPercent();

                        var _afterDisount = ProductRate - _totalProductsDiscount;

                        ProductGSTPerQuantity = (float)Math.Round(SelectedGSTslab.GstValue * _afterDisount / 100, 2, MidpointRounding.AwayFromZero);

                    }
                    else
                    {
                        //discount amount is in flat.

                        float _totalProductsDiscount = GetTotalProductFlatAmount();

                        var _afterDisount = ProductRate - _totalProductsDiscount;

                        ProductGSTPerQuantity = (float)Math.Round(SelectedGSTslab.GstValue * _afterDisount / 100, 2, MidpointRounding.AwayFromZero);

                    }

                }

                //if (Discount > 0)
                //{
                //    //invoice level discountrate
                //    ProductGSTPerQuantity = DiscountedRate * SelectedGSTslab.GstValue / 100;
                //}
                //else
                //{
                //    ProductGSTPerQuantity = ProductRate * SelectedGSTslab.GstValue / 100;
                //}

            }
            else
            {
                ProductGSTPerQuantity = 0;
            }

        }

        private string _productUniqGuid;
        public string ProductUniqGuid
        {
            get { return _productUniqGuid; }
            set { SetProperty(ref _productUniqGuid, value); }
        }

        private string _lotnumber;
        public string Lotnumber
        {
            get { return _lotnumber; }
            set { SetProperty(ref _lotnumber, value); }
        }


        private string _hsn;
        public string HSN
        {
            get { return _hsn; }
            set
            {
                SetProperty(ref _hsn, value);
            }
        }



        #endregion



        #region Methods 




        private void ExecuteGenerateSerialCommand(object obj)
        {

            var defectiveqty = SelectedProduct != null ? SelectedProduct.DefectiveQty : 0;
            var productqty = defectiveqty + ProductQty;
            if (productqty <= 0)
            {

                SerialNoList?.Clear();

                _notificationService.ShowMessage("Enter valid product quantity", Common.NotificationType.Error);

                IsSerialNumberManual = false;

                return;
            }

            if (obj is bool _res)
            {
                if (_res)
                {
                    SerialNoList?.Clear();
                    if (IsGroupTrackMode)
                    {
                        SerialNoList.AddRange(Enumerable.Range(0, ProductQty).Select(_ => new SerialNumber()));

                    }
                    else
                    {

                        SerialNoList.AddRange(Enumerable.Range(0, productqty).Select(_ => new SerialNumber()));

                    }


                }
                else
                {
                    SerialNoList.Clear();
                }
            }
        }




        private bool _isGroupTrackMode;
        public bool IsGroupTrackMode
        {
            get { return _isGroupTrackMode; }
            set { SetProperty(ref _isGroupTrackMode, value); }
        }




        private int _invoicedefectiveqty;
        public int InvoiceDefectiveQty
        {
            get { return _invoicedefectiveqty; }
            set { SetProperty(ref _invoicedefectiveqty, value); }
        }

        private bool _invoicediscount;
        public bool Invoicediscount
        {
            get { return _invoicediscount; }
            set { SetProperty(ref _invoicediscount, value); }
        }

        private float _invoicediscountPercent;
        public float InvoicediscountPercent
        {
            get { return _invoicediscountPercent; }
            set { SetProperty(ref _invoicediscountPercent, value); }
        }

        private float _invoicediscountFlat;
        public float InvoicediscountFlat
        {
            get { return _invoicediscountFlat; }
            set { SetProperty(ref _invoicediscountFlat, value); }
        }

        private string _state;
        public string State
        {
            get { return _state; }
            set { SetProperty(ref _state, value); }
        }

        private void SelectedProductChanged()
        {
            AttributesSelectedList?.Clear();

            IsGroupTrackMode = SelectedProduct?.InventoryTrackMode == "group";

            SelectedSubUnitType = SelectedProduct?.SubUnitType;

            SelectedUnitType = null;



            if (GSTslabs != null && GSTslabs.Any())
            {
                SelectedGSTslab = GSTslabs.FirstOrDefault();
            }

            IsSerialNumberManual = false;

            SerialNoList?.Clear();

            ProductSubQty = 0;
            ProductQty = 0;
            DefectiveList?.Clear();
            ResetPrice();
            if (SelectedProduct != null)
            {
                if (IsGroupTrackMode)
                {
                    SelectedProduct.IsDefectiveQty = false;
                    IsGroupDefectiveQty = InvoiceDefectiveQty > 0 ? true : false;
                    DefectiveHeader = "Defective Base Unit Type";
                    DefectiveSubHeader = SelectedSubUnitType != null ? "Defective Sub Unit(" + SelectedSubUnitType + ")" : "Defective Sub Unit type";
                }
                else
                {
                    SelectedProduct.IsDefectiveQty = InvoiceDefectiveQty > 0 ? true : false;
                    IsGroupDefectiveQty = false;
                }

                if (Invoicediscount)
                {
                    IsDiscount = true;
                    if (InvoicediscountFlat > 0)
                    {
                        GetDiscountFromInvocieFlat(Convert.ToString(InvoicediscountFlat));
                    }
                    if (InvoicediscountPercent > 0)
                    {
                        GetDiscountFromInvociePercent(Convert.ToString(InvoicediscountPercent));
                    }
                }
                else
                {
                    IsDiscount = false;
                }
            }



        }
        private void AddAttributeSelection(object obj)
        {
            try
            {

                if (obj is SelectionChangedEventArgs) return;

                if (SelectedProduct == null) return;


                var _values = obj as Tuple<object, object>;

                if (_values == null || _values.Item1 == null || _values.Item2 == null) return;


                var _productAttribute = _values.Item1 as ProductAttribute;

                var _attribute = _values.Item2 as AttributeMap;

                if (_productAttribute == null || _attribute == null) return;


                if (AttributesSelectedList == null)
                    AttributesSelectedList = new List<ProductAttributeMapping>();


                if (AttributesSelectedList.Any(x => x.ProductAttribute.AttributeId == _productAttribute.AttributeId))
                {
                    //Already there 

                    var _existingAttribute = AttributesSelectedList.FirstOrDefault(x => x.ProductAttribute.AttributeId == _productAttribute.AttributeId);

                    if (_existingAttribute != null)
                    {
                        _existingAttribute.AttributesList?.Clear();
                        _existingAttribute.AttributesList = new List<AttributeMap>
                        {
                            new AttributeMap
                            {
                                AttributeValueId=_attribute.AttributeValueId,
                                AttributeValueName=_attribute.AttributeValueName
                            }
                        };
                    }
                    return;

                }

                var _productMapping = new ProductAttributeMapping
                {
                    ProductAttribute = _productAttribute
                };
                _productMapping.AttributesList.Add(new AttributeMap
                {
                    AttributeValueId = _attribute.AttributeValueId,
                    AttributeValueName = _attribute.AttributeValueName
                });

                AttributesSelectedList?.Add(_productMapping);


            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in inward damage view model", _ex);
            }

        }

        public event EventHandler ProductQtyChangeEvent;

        public int TotalProductQtyCount { get; set; }

        public float InvoiceLevelDiscountPercent { get; set; }

        internal void DiscountTypeDropDownChangeEventHandler(object sender, DiscountTypeEventArgs e)
        {
            ApplyDiscountType = e.ApplyDiscountType;

            TotalProductQtyCount = e.TotalProductQtyCount;

            InvoiceLevelDiscountFlatAmount = e.InvoiceFlatTotal;

            InvoiceLevelDiscountPercent = e.InvoiceDiscountPercent;

            DiscountHeader = e.DiscountHeader;

            //flat invoice discount amount

            if (InvoiceLevelDiscountFlatAmount == 0 && InvoiceLevelDiscountPercent == 0)
            {
                Discount = 0;
            }

            if (InvoiceLevelDiscountFlatAmount != 0)
            {

                if (DiscountHeader == PercentDiscountHeader)
                {

                }
                else
                {
                    Discount = (float)Math.Round(GetTotalProductFlatAmount(), 2, MidpointRounding.AwayFromZero);
                }



            }
            else
            {
                //discount amount is in percent.
                if (DiscountHeader == PercentDiscountHeader)
                {
                    if (ApplyDiscountType == ApplyDiscountType.AFTER)
                    {

                        //gst is selected
                        if (SelectedGSTslab != null)
                        {
                            float _sumRate = ProductRate + ProductRate * SelectedGSTslab.GstValue / 100;

                            Discount = (float)Math.Round(_sumRate * InvoiceLevelDiscountPercent / 100, 2, MidpointRounding.AwayFromZero);


                        }
                        else
                        {
                            //no gst is selected.
                            Discount = (float)Math.Round(ProductRate * InvoiceLevelDiscountPercent / 100, 2, MidpointRounding.AwayFromZero);

                        }

                    }
                    else
                    {
                        Discount = (float)Math.Round(GetDiscountAmountPercent(), 2, MidpointRounding.AwayFromZero);
                    }



                }

            }
            IsDiscount = IsInvoiceDiscountAvailable();

            Calculate();
        }

        private bool IsInvoiceDiscountAvailable()
        {
            return Discount > 0;
        }

        private ApplyDiscountType _applyDiscountType;


        public ApplyDiscountType ApplyDiscountType
        {
            get => _applyDiscountType;
            set
            {
                _applyDiscountType = value;
            }
        }

        #endregion


        private string _gstHeaderper;

        public string GstHeaderPer
        {
            get { return _gstHeaderper; }
            set { SetProperty(ref _gstHeaderper, value); }
        }

        private string _gstHeaderQty;

        public string GstHeaderQty
        {
            get { return _gstHeaderQty; }
            set { SetProperty(ref _gstHeaderQty, value); }
        }

        private string _gstHeaderTotal;

        public string GstHeaderTotal
        {
            get { return _gstHeaderTotal; }
            set { SetProperty(ref _gstHeaderTotal, value); }
        }




        private List<string> GetCertifcateProduct()
        {
            if (ApplicationSettings.CategoryCertificate != null && ApplicationSettings.CategoryCertificate.Any())
            {
                return new List<string>(ApplicationSettings.CategoryCertificate);

            }
            return null;

        }


        private async void SearchTextChangedIndent(string _searchText)
        {
            try
            {
                SelectedProduct = null;

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
                            SelectedProductSearch = ProductsSearchList.FirstOrDefault();
                            IsDropDownOpen = false;
                            IsEditDropDown = false;
                            GstHeaderPer = State == AppConstants.STATE ? AppConstants.GSTHEADERPER : AppConstants.IGSTHEADERPER;
                            GstHeaderQty = State == AppConstants.STATE ? AppConstants.GSTHEADERQTY : AppConstants.IGSTHEADERQTY;
                            GstHeaderTotal = State == AppConstants.STATE ? AppConstants.GSTHEADER : AppConstants.IGSTHEADER;
                            IsAddandRemoveVisibility = false;
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
                _logger.LogWarning($"Task was cancelled in product search text {_searchText}");
            }
            catch (Exception _ex)
            {
                _logger.LogError($"Error in searching product by text {_searchText}", _ex);
            }
        }

      
    }



    public class SerialNumber : BindableBase
    {
        private string _value;
        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }



    }

    public class GSTslabs : BindableBase
    {
        private string _name;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                SetProperty(ref _name, value);
            }
        }

        private float _gstValue;

        public float GstValue
        {
            get => _gstValue;
            set
            {
                SetProperty(ref _gstValue, value);
                Name = $"{value}%";
            }
        }
    }

    public class ProductLocation
    {
        public string Name { get; set; }
    }

    public class WarrantyService
    {
        public string Name { get; set; }
    }




}


