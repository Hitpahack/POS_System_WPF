using FalcaPOS.Common;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Common.Validations;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Manufacturers;
using FalcaPOS.Entites.Products;
using FalcaPOS.Entites.ProductTypes;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.AddInventory.ViewModels
{
    public class AddProductViewModel : ValidationBase
    {
        private readonly IAttributeTypeService _attributeTypeService;
        private readonly IProductTypeService _productTypeService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IProductService _productService;
        private readonly INotificationService _notificationService;
        private readonly Logger _logger;

        public DelegateCommand ProductTypeChangedCommand { get; private set; }

        public DelegateCommand AddNewProductCommand { get; private set; }



        public AddProductViewModel(INotificationService notificationService,
            IAttributeTypeService attributeTypeService,
            IProductTypeService productTypeService,
            IEventAggregator eventAggregator,
            IProductService productService,
            Logger logger

            )
        {



            ProductTypeChangedCommand = new DelegateCommand(ProductTypeChanged);


            AddNewProductCommand = new DelegateCommand(AddNewProduct).ObservesCanExecute(() => IsValid);

            _attributeTypeService = attributeTypeService ?? throw new ArgumentNullException(nameof(attributeTypeService));

            _productTypeService = productTypeService ?? throw new ArgumentNullException(nameof(productTypeService));

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _productService = productService ?? throw new ArgumentNullException(nameof(productService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));



            _eventAggregator.GetEvent<AddProductTypeEvent>().Subscribe(ptype => AddNewProducctType(ptype));

            _eventAggregator.GetEvent<SignalRProductTypeAddedEvent>().Subscribe(NewProducttypeAdded, ThreadOption.PublisherThread);

            _eventAggregator.GetEvent<SignalRProductTypeEnableDisableEvent>().Subscribe(ProducttypeEnabledDisabled, ThreadOption.PublisherThread);



            //InventoryTrackMode = GetBarCodeTypes();

            InvTrackModes = GetInventoryTrackMode();

            LoadProductTypes();

            SubUnitTypes = GetSubUnitTypes();

        }

        private void ProducttypeEnabledDisabled(object obj)
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
                    //Never get called
                    //if(SelectedProductType!=null && SelectedProductType.ProductTypeId == _pType.ProductTypeId && !_pType.Isenabled)
                    //{
                    //    _notificationService.ShowMessage("Selected Product type is disabled, select different product type", NotificationType.Information);
                    //}
                });

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

        private ObservableCollection<InventoryTrackMode> GetInventoryTrackMode()
        {
            return new ObservableCollection<InventoryTrackMode>
            {
                new InventoryTrackMode{Name="individual",Description="Individual"},
                new InventoryTrackMode{Name="group",Description="Group"},
                //new InventoryTrackMode{Name="none",Description="None"}
            };
        }

        private ObservableCollection<InventoryTrackMode> _invTrackModes;

        public ObservableCollection<InventoryTrackMode> InvTrackModes
        {
            get { return _invTrackModes; }
            set { SetProperty(ref _invTrackModes, value); }
        }

        private ObservableCollection<ProductType> _productTypes = new ObservableCollection<ProductType>();

        public ObservableCollection<ProductType> ProductTypes
        {
            get { return _productTypes; }
            set { SetProperty(ref _productTypes, value); }
        }

        private ObservableCollection<Manufacturer> _manufacturers;

        public ObservableCollection<Manufacturer> Manufacturers
        {
            get { return _manufacturers; }
            set { SetProperty(ref _manufacturers, value); }
        }




        private async void AddNewProduct()
        {
            try
            {


                var _product = GetProduct();

                if (_product == null)
                {
                    return;
                    //return false;
                }
                //return false;            


                await Task.Run(async () =>
                {
                    var _result = await _productService.CreateProduct(_product);

                    if (_result != null && _result.IsSuccess)
                    {
                        Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            _notificationService.ShowMessage($"{_result.Data ?? "Product Created"}", NotificationType.Success);

                            ResetAddProduct();

                            PopupClose = false;

                            _eventAggregator.GetEvent<RefreshProductEvent>().Publish();

                        });
                    }
                    else
                    {
                        _notificationService.ShowMessage(_result?.Error ?? "An error occurred, try again", NotificationType.Error);
                    }
                });



                //ResetFormData();
            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in new product type added", _ex);
            }
        }

        private void ResetAddProduct()
        {
            try
            {
                ClearErrors();
                //BarCodeTypes = GetBarCodeTypes();
                SelectedManufacturer = null;
                SelectedProductType = null;
                //ProductShortCode = null;

                ProductName = null;
                InvTrackModes = GetInventoryTrackMode();
                ProductSKU = null;
                LastkSKU = null;
                SelectedSubUnitType = null;
                Description = null;
            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in reset product", _ex);
            }

        }


        private ProductDetails GetProduct()
        {
            //Alert user for required fields.

            if (SelectedProductType == null)
            {
                ShowWarning("Sub Category is required.", NotificationType.Error);
                return null;
            }
            else if (SelectedManufacturer == null)
            {
                ShowWarning("Brand is required.", NotificationType.Error);

                return null;
            }
            else if (string.IsNullOrEmpty(ProductName))
            {
                ShowWarning("Product name is required.", NotificationType.Error);
                return null;
            }
            else if (string.IsNullOrEmpty(Description))
            {
                ShowWarning("Description is required.", NotificationType.Error);
                return null;
            }
            else if (string.IsNullOrEmpty(SelectedSubUnitType))
            {
                ShowWarning("Please select SubUnitType.", NotificationType.Error);
                return null;
            }
            //else if (!InvTrackModes.Any(x => x.IsSelected))
            //{
            //    ShowWarning("Inventory track mode type is required", NotificationType.Error);
            //    return null;
            //}
            //else if (!ProductShortCode.IsValidString())
            //{
            //    _notificationService.ShowMessage("Product short code is required.", NotificationType.Error);
            //    return null;
            //}

            //validate  attribute 

            if (!ProductSKU.IsValidString())
            {
                ShowWarning("Product SKU  is required", NotificationType.Error);
                return null;
            }


            //produt sku should be 6 digit in lentgh

            if (ProductSKU.Length != 6)
            {
                ShowWarning("Product SKU should be 6 characters", NotificationType.Error);
                return null;

            }





            try
            {
                var _product = new ProductDetails
                {
                    Description = Description,
                    Name = ProductName,
                    Isenabled = true,
                    ProductTypeManufacturerId = SelectedManufacturer.ProductTypeManufacturerId,
                    //BarCodeType=BarCodeTypes.FirstOrDefault(x=>x.IsSelected)?.Name
                    // ProductShortCode=ProductShortCode
                    InventoryTrackMode = "group",
                    ProductSKU = ProductSKU,
                    SubUnitType = SelectedSubUnitType
                };

                return _product;

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in add product ", _ex);
            }

            return null;


        }

        private void ShowWarning(string msg, NotificationType notificationType)
        {
            _eventAggregator.GetEvent<NotifyMessage>().Publish(new Common.Models.ToastMessage
            {
                Message = msg,
                MessageType = notificationType
            });
        }

        public Dictionary<int, List<string>> Propertier { get; set; }

        //private ProductDetails MapSelectedAttributes(ProductDetails product)
        //{
        //    //TODO More Validation.. attr
        //    if (AttributeTypesKeyValuePairs == null) return product;

        //    var _attributesCollection = AttributeTypesKeyValuePairs.ToList();

        //    if (!_attributesCollection.Any()) return product;

        //    try
        //    {

        //        foreach (var item in _attributesCollection)
        //        {
        //            var _attributeItem = new AttributesCollection();

        //            var _keys = item.Keys;

        //            var _values = item.Values;

        //            foreach (var _key in _keys)
        //            {

        //                foreach (var _value in _values)
        //                {

        //                    foreach (var _subValue in _value)
        //                    {

        //                        if (_key.AttributeId == _subValue.AttributeType.AttributeId)
        //                        {

        //                            var _existingAttributeCollection = product.AttributesCollection.FirstOrDefault(x => x.ProductAttribute.AttributeId == _key.AttributeId);

        //                            if (_existingAttributeCollection != null)
        //                            {
        //                                _existingAttributeCollection.ProductAttributeMapping.AttributesList.Add(new AttributeMap { AttributeValueName = _subValue.AttributeName });

        //                                continue;
        //                            }

        //                            _attributeItem.ProductAttribute = new ProductAttribute
        //                            {
        //                                AttributeId = _key.AttributeId,
        //                                AttributeName = _key.Name,
        //                            };


        //                            _attributeItem.ProductAttributeMapping = new ProductAttributeMapping
        //                            {
        //                                ProductAttribute = new ProductAttribute
        //                                {
        //                                    AttributeId = _key.AttributeId,
        //                                    AttributeName = _key.Name
        //                                }
        //                            };
        //                            _attributeItem.ProductAttributeMapping.AttributesList.Add(new AttributeMap { AttributeValueName = _subValue.AttributeName });


        //                            product.AttributesCollection.Add(_attributeItem);

        //                        }
        //                    }
        //                }


        //            }
        //        }
        //    }
        //    catch (Exception _ex)
        //    {
        //        _logger?.LogError("Error in new product type added signalr event", _ex);
        //        return null;
        //    }


        //    return product;

        //}



        //private void DeleteSingleAtributeValue(AttributeMapping _attributeMapping)
        //{
        //    try
        //    {

        //        if (_attributeMapping == null) return;

        //        AttributeTypesKeyValuePairs.ToList().ForEach(x =>
        //        {
        //            var _keys = x.Keys;

        //            var _attributeName = _keys.FirstOrDefault(y => y.AttributeId == _attributeMapping.AttributeType.AttributeId);

        //            if (_attributeName == null) return;

        //            x.Values.ToList().ForEach(y =>
        //            {

        //                var _attribute = y.FirstOrDefault(z => z.Id == _attributeMapping.Id);

        //                if (_attribute == null) return;

        //                y.Remove(_attribute);
        //            });

        //        });
        //        AttributeTypesKeyValuePairs = new ObservableCollection<Dictionary<AttributeType, List<AttributeMapping>>>(AttributeTypesKeyValuePairs);




        //    }
        //    catch (Exception _ex)
        //    {
        //        _logger?.LogError("Error in new product type added signalr event", _ex);
        //    }
        //}

        private void AddNewProducctType(object ptype)
        {
            try
            {
                Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        var _type = ptype as ProductType;
                        if (_type != null)
                        {
                            if (ProductTypes == null)
                                ProductTypes = new ObservableCollection<ProductType>();

                            ProductTypes.Add(_type);

                            ProductTypes = new ObservableCollection<ProductType>(ProductTypes.OrderBy(x => x.Name));
                        }
                    });
            }
            catch (Exception _ex)
            {
                _logger.LogError("error in new product type add", _ex);
            }
        }

        //private void AddNewAttribute(object attibuteType)
        //{
        //    var _attributeType = attibuteType as AttributeType;

        //    if (_attributeType != null)
        //        AttributeTypes.Add(_attributeType);

        //}

        private CancellationTokenSource _cancellationTokenSource;

        private async void ProductTypeChanged()
        {
            try
            {

                Manufacturers = new ObservableCollection<Manufacturer>();
                SelectedManufacturer = null;
                // LastkSKU = null;

                if (SelectedProductType != null)
                {

                    if (_cancellationTokenSource != null)
                        _cancellationTokenSource.Cancel();

                    _cancellationTokenSource = new CancellationTokenSource();

                    var _productType = SelectedProductType;



                    await Task.Run(async () =>
                    {
                        var _result = await _productTypeService
                                .GetProductTypeManufacturers(_productType.ProductTypeId.Value, _cancellationTokenSource.Token);

                        Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            if (_result != null && _result.Any())
                            {
                                Manufacturers = new ObservableCollection<Manufacturer>(_result.OrderBy(x => x.Name)
                                                                                                   .ToList());
                                // Manufacturers = new ObservableCollection<Manufacturer>(_result);
                                // SelectedManufacturer = Manufacturers[0];
                            }
                        }, System.Windows.Threading.DispatcherPriority.Normal, _cancellationTokenSource.Token);

                    }, _cancellationTokenSource.Token);

                    await Task.Run(async () =>
                    {

                        var _lastSKU = await _productService
                        .ProductCurrentSKU(SelectedProductType.ProductTypeId.GetValueOrDefault(), _cancellationTokenSource.Token);

                        Application.Current?.Dispatcher?.Invoke(() =>
                        {

                            if (_lastSKU != null && _lastSKU.IsSuccess)
                            {
                                LastkSKU = _lastSKU.Data;
                            }

                        }, System.Windows.Threading.DispatcherPriority.Normal, _cancellationTokenSource.Token);

                    }, _cancellationTokenSource.Token);

                }


            }
            catch (OperationCanceledException) { }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in new product type added signalr event", _ex);
            }
        }

        private string _lastSKU;
        public string LastkSKU
        {
            get { return _lastSKU; }
            set { SetProperty(ref _lastSKU, value); }
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
                        if (_result != null)
                            ProductTypes = new ObservableCollection<ProductType>(_result);
                        else
                            ProductTypes = new ObservableCollection<ProductType>();
                    });

                });
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in loading product types in add new product", _ex);
            }
        }

        //private void DeleteAttributeValue(object obj)
        //{

        //    var _type = obj as Dictionary<AttributeType, List<AttributeMapping>>;

        //    if (_type != null && _type.Count > 0)
        //    {

        //        var _attributType = _type.Keys.FirstOrDefault() as AttributeType;

        //        Dictionary<AttributeType, List<AttributeMapping>> _item = null;

        //        if (_attributType != null)
        //        {
        //            AttributeTypesKeyValuePairs.ToList().ForEach(x =>
        //            {

        //                var _keys = x.Keys;

        //                var _attType = _keys.FirstOrDefault(y => y.AttributeId == _attributType.AttributeId);

        //                if (_attType != null)
        //                {
        //                    _item = x;
        //                }

        //            });
        //            if (_item != null)
        //            {
        //                AttributeTypesKeyValuePairs.Remove(_item);
        //            }

        //        }

        //    }



        //}

        //private void AddAttributeValue(object obj)
        //{
        //    try
        //    {

        //        var _tuple = obj as Tuple<object, object>;

        //        if (_tuple != null)
        //        {
        //            var _attribute = _tuple.Item1 as string;
        //            var _value = _tuple.Item2 as string;

        //            if (string.IsNullOrEmpty(_attribute) || string.IsNullOrEmpty(_value)) return;

        //            //Find attribute from collection ;

        //            AttributeTypesKeyValuePairs.ToList().ForEach(x =>
        //            {
        //                var _keys = x.Keys;

        //                var _attributeName = _keys.FirstOrDefault(y => y.Name == _attribute);

        //                if (_attributeName == null) return;

        //                bool _isvaluePresent = false;

        //                //check if same value is getting added.

        //                x.Values.ToList().ForEach(v =>
        //                    {
        //                        var _result = v.Where(c => c.AttributeName == _value && c.AttributeType.AttributeId == _attributeName.AttributeId).FirstOrDefault();
        //                        if (_result != null)
        //                        {
        //                            _notificationService.ShowMessage("Attribute value is already added", NotificationType.Error);
        //                            _isvaluePresent = true;
        //                            return;
        //                        }
        //                    });

        //                if (!_isvaluePresent)
        //                {
        //                    x.Values.ToList().ForEach(y =>
        //                    {
        //                        y.Add(new AttributeMapping
        //                        {

        //                            AttributeName = _value,
        //                            AttributeType = _attributeName,
        //                        });
        //                    });
        //                }
        //            });


        //            AttributeTypesKeyValuePairs = new ObservableCollection<Dictionary<AttributeType, List<AttributeMapping>>>(AttributeTypesKeyValuePairs);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        _notificationService.ShowMessage("An error occurred ,try again", NotificationType.Error);
        //    }

        //}

        //private ObservableCollection<Dictionary<AttributeType, List<AttributeMapping>>> _attributeTypesKeyValuePairs = new ObservableCollection<Dictionary<AttributeType, List<AttributeMapping>>>();
        //public ObservableCollection<Dictionary<AttributeType, List<AttributeMapping>>> AttributeTypesKeyValuePairs
        //{
        //    get { return _attributeTypesKeyValuePairs; }
        //    set { SetProperty(ref _attributeTypesKeyValuePairs, value); }
        //}

        //private void AddAttributeType(object obj)
        //{

        //    var _attributeType = obj as AttributeType;

        //    bool _contains = false;
        //    if (AttributeTypesKeyValuePairs == null)
        //    {
        //        AttributeTypesKeyValuePairs = new ObservableCollection<Dictionary<AttributeType, List<AttributeMapping>>>();
        //    }

        //    AttributeTypesKeyValuePairs.ToList().ForEach(x =>
        //    {

        //        var _keys = x.Keys;

        //        _keys.ToList().ForEach(y =>
        //        {
        //            if (y.AttributeId == _attributeType.AttributeId)
        //            {
        //                _contains = true;
        //                return;
        //            }
        //        });

        //    });

        //    if (!_contains)
        //    {
        //        var _pairs = new Dictionary<AttributeType, List<AttributeMapping>>();
        //        var _values = new List<AttributeMapping>();
        //        _pairs.Add(_attributeType, _values);
        //        AttributeTypesKeyValuePairs.Add(_pairs);
        //    }


        //}

        //private async void LoadAttributes()
        //{

        //    await Task.Run(async () =>
        //    {
        //        var _result = await _attributeTypeService.GetAttributeTypes("isenabled=true");

        //        Application.Current.Dispatcher.Invoke(() =>
        //        {
        //            if (_result != null)
        //            {
        //                AttributeTypes = new ObservableCollection<AttributeType>(_result);
        //            }
        //        });

        //    });

        //}

        //private ObservableCollection<AttributeType> _attributeTypes = new ObservableCollection<AttributeType>();


        //public ObservableCollection<AttributeType> AttributeTypes
        //{
        //    get { return _attributeTypes; }
        //    set { SetProperty(ref _attributeTypes, value); }
        //}

        private ProductType _selectedProductType;

        [Required(ErrorMessage = "Sub Category is required")]
        public ProductType SelectedProductType
        {
            get { return _selectedProductType; }
            set
            {
                SetProperty(ref _selectedProductType, value);
                if (value != null)
                    ValidateProperty(value);
            }
        }


        private string _productSKU;

        [Required(ErrorMessage = "Product SKU is required")]
        [MaxLength(200, ErrorMessage = "Only 200 characters allowed")]
        public string ProductSKU
        {
            get { return _productSKU; }
            set
            {
                SetProperty(ref _productSKU, value);
                if (value != null)
                    ValidateProperty(value);
            }
        }



        private Manufacturer _selectedManufacturer;

        [Required(ErrorMessage = "Brand is required")]
        public Manufacturer SelectedManufacturer
        {
            get { return _selectedManufacturer; }
            set
            {
                SetProperty(ref _selectedManufacturer, value);
                if (value != null)
                    ValidateProperty(value);
            }
        }

        private string _productName;

        [Required(ErrorMessage = "Product Name is required")]
        public string ProductName
        {
            get { return _productName; }
            set
            {
                SetProperty(ref _productName, value);
                if (value != null)
                    ValidateProperty(value);
            }
        }


        private string _description;

        [Required(ErrorMessage = "Description is required")]
        public string Description
        {
            get { return _description; }
            set
            {
                SetProperty(ref _description, value);
                if (value != null)
                    ValidateProperty(value);
            }
        }
        //private string _productShortCode;

        //[Required]
        //[StringLength(3, MinimumLength = 3, ErrorMessage = "Product short code should be three characters")]

        //public string ProductShortCode
        //{
        //    get { return _productShortCode; }
        //    set { SetProperty(ref _productShortCode, value?.ToUpper());

        //        if (value != null)
        //            ValidateProperty(value);
        //    }
        //}
        private bool _popupClose;

        public bool PopupClose
        {
            get { return _popupClose; }
            set { SetProperty(ref _popupClose, value); }
        }
        private ObservableCollection<string> GetSubUnitTypes()
        {
            return new ObservableCollection<string>
            {
                 "Packet","Bottle","Piece","Bucket","Bag","Tin"
            };
        }

        private ObservableCollection<string> _subUnitTypes;
        public ObservableCollection<string> SubUnitTypes
        {
            get { return _subUnitTypes; }
            set { SetProperty(ref _subUnitTypes, value); }
        }

        private string _selectedSubUnitType;
        public string SelectedSubUnitType
        {
            get { return _selectedSubUnitType; }
            set { SetProperty(ref _selectedSubUnitType, value); }
        }

    }

    public class InventoryTrackMode : BindableBase
    {
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }
    }
}
