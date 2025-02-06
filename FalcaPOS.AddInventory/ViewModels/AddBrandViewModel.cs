using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Manufacturers;
using FalcaPOS.Entites.ProductTypes;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.AddInventory.ViewModels
{
    public class AddBrandViewModel : BindableBase//: ValidationBase
    {

        public DelegateCommand AddManufacturerCommand { get; private set; }

        private readonly Logger _logger;

        private readonly INotificationService _notificationService;



        public AddBrandViewModel(IProductTypeService productTypeService,
            IEventAggregator eventAggregator,
            Logger logger,
            INotificationService notificationService)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _productTypeService = productTypeService ?? throw new ArgumentNullException(nameof(productTypeService));

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _eventAggregator.GetEvent<AddProductTypeEvent>().Subscribe(type => AddNewType(type));


            _eventAggregator.GetEvent<SignalRProductTypeAddedEvent>().Subscribe(NewProductTypeAdded, ThreadOption.PublisherThread);

            _eventAggregator.GetEvent<SignalRProductTypeEnableDisableEvent>().Subscribe(ProducttypeEnabledDisabled, ThreadOption.PublisherThread);

            AddManufacturerCommand = new DelegateCommand(AddManufacturer);

            LoadProductTypes();

            LoadBrandsAsync();

            Title = "Add New Brand";

            // ProductTypes = new ObservableCollection<ProductType>();
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

        private void NewProductTypeAdded(object obj)
        {

            _logger?.LogInformation("New Product type was added . Eevent from hub");
            try
            {
                if (obj is ProductType _prodType)
                {
                    Application.Current.Dispatcher?.Invoke(() =>
                    {

                        _logger?.LogInformation($"Recieved product type {_prodType.Name}");

                        if (ProductTypes == null)
                        {
                            ProductTypes = new ObservableCollection<ProductType>();
                        }

                        if (!ProductTypes.Any(x => x.ProductTypeId == _prodType.ProductTypeId))
                        {
                            ProductTypes.Add(_prodType);

                            ProductTypes = new ObservableCollection<ProductType>(ProductTypes.OrderBy(x => x.Name));
                        }

                    });
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in adding new product type", _ex);
            }

        }

        private bool _mapToProductType;
        public bool MapToProductType
        {
            get { return _mapToProductType; }
            set
            {
                SetProperty(ref _mapToProductType, value);
                Title = value ? "Map Brand" : "Add New Brand";
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private ObservableCollection<Manufacturer> _manufacturers;
        public ObservableCollection<Manufacturer> Manufacturers
        {
            get { return _manufacturers; }
            set { SetProperty(ref _manufacturers, value); }
        }

        private Manufacturer _selectedManufacturer;
        public Manufacturer SelectedManufacturer
        {
            get { return _selectedManufacturer; }
            set { SetProperty(ref _selectedManufacturer, value); }
        }

        private async void LoadBrandsAsync()
        {
            try
            {

                await Task.Run(async () =>
                {
                    var _result = await _productTypeService.GetAllEnabledManufacturers();

                    if (_result != null && _result.IsSuccess && _result.Data != null && _result.Data.Count() > 0)
                    {
                        Application.Current?.Dispatcher?.Invoke(() =>
                        {

                            Manufacturers = new ObservableCollection<Manufacturer>(_result.Data.OrderBy(x => x.Name).ToList());

                        });

                    }

                });


            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Task was cancelled in load brands");
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in loading brands", _ex);

            }

        }

        private void AddNewType(object type)
        {
            var _type = type as ProductType;
            if (_type != null)
            {
                if (ProductTypes == null)
                    ProductTypes = new ObservableCollection<ProductType>();

                if (!ProductTypes.Any(x => x.ProductTypeId == _type.ProductTypeId))
                {
                    ProductTypes.Add(_type);

                    ProductTypes = new ObservableCollection<ProductType>(ProductTypes.OrderBy(x => x.Name));
                }

            }
        }

        private async void AddManufacturer()
        {
            try
            {



                if (SelectedProductType == null)
                {
                    _notificationService.ShowMessage("Select sub category", Common.NotificationType.Error);

                    return;
                }

                if (MapToProductType)
                {
                    if (SelectedManufacturer == null)
                    {
                        _notificationService.ShowMessage("Select a Brand", Common.NotificationType.Error);
                        return;
                    }

                }
                else
                {
                    if (!ManufacturerName.IsValidString())
                    {
                        _notificationService.ShowMessage("Brand name is required.", Common.NotificationType.Error);
                        return;
                    }

                }


                var _productTypeManufacturer = new CreateManufactureModel
                {

                    Name = ManufacturerName,
                    productTypeId = SelectedProductType.ProductTypeId.Value,
                    BrandId = MapToProductType ? SelectedManufacturer?.ManufacturerId ?? 0 : 0,
                    MapToProductType = MapToProductType,
                };

                _logger.LogObject($"adding new product type manufactures,type : {_productTypeManufacturer?.GetType()} ", _productTypeManufacturer);

                await Task.Run(async () =>
                {

                    var _result = await _productTypeService.CreateProductTypeManufacturerAsync(_productTypeManufacturer); //await _productTypeService.CreateProductTypeManufacturer(_productTypeManufacturer);

                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        if (_result != null && _result.IsSuccess)
                        {

                            // ClearErrors();

                            _logger.LogInformation($"new product type manufacuturer {_productTypeManufacturer.Name} added");

                            ManufacturerName = null;

                            SelectedManufacturer = null;

                            // ShortCode = null;

                            _eventAggregator.GetEvent<LoadManufacturesEvent>().Publish(SelectedProductType);

                            string _message = MapToProductType ? "Brand Mapped" : "Brand Created";

                            _notificationService.ShowMessage(_message, Common.NotificationType.Success);

                            SelectedProductType = null;

                            MapToProductType = false;

                            PopupClose = false;

                            LoadBrandsAsync();
                        }
                        else
                        {
                            _notificationService.ShowMessage(_result?.Error ?? "An error occurred, try again", Common.NotificationType.Error);
                        }

                    });

                });

            }
            catch (Exception _ex)
            {
                _logger.LogError($"Error in {MethodBase.GetCurrentMethod()?.Name}", _ex);
            }

        }
        private string _manufacturerName;


        public string ManufacturerName
        {
            get { return _manufacturerName; }
            set
            {
                SetProperty(ref _manufacturerName, value);

            }
        }

        private ObservableCollection<ProductType> _productTypes;

        private readonly IProductTypeService _productTypeService;
        private readonly IEventAggregator _eventAggregator;

        public ObservableCollection<ProductType> ProductTypes
        {
            get { return _productTypes; }
            set { SetProperty(ref _productTypes, value); }
        }

        private ProductType _selectedProductType;

        public ProductType SelectedProductType
        {
            get { return _selectedProductType; }
            set
            {
                SetProperty(ref _selectedProductType, value);

            }
        }

        public void ResetData()
        { 
            SelectedProductType = null;
            MapToProductType = false;
            SelectedManufacturer=null;
            ManufacturerName = null;

        }

        private bool _popupClose;

        public bool PopupClose
        {
             
            get { return _popupClose; }
            set
            {
                if (_popupClose != false && SelectedProductType != null || MapToProductType != false || SelectedManufacturer!=null || ManufacturerName!=null)
                {
                    ResetData();
                }
                SetProperty(ref _popupClose, value);
            }
        }

        private async void LoadProductTypes()
        {
            try
            {
                _logger.LogInformation("getting product types");

                await Task.Run(async () =>
                {
                    var _result = await _productTypeService.GetProductTypes("isenabled=true");

                    if (_result != null && _result.Any())
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ProductTypes = new ObservableCollection<ProductType>(_result);
                        });


                    }


                });

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in add brand view model", _ex);
            }
        }

    }
}
