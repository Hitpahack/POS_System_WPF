using FalcaPOS.Common;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Logger;
using FalcaPOS.Common.Validations;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Indent;
using FalcaPOS.Entites.ProductTypes;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace FalcaPOS.Sku.ViewModels
{
    public class SKUProductCardViewModel : ValidationBase
    {
        private readonly IProductService _productService;

        private readonly ISupplierService _supplierService;

        private readonly INotificationService _notificationService;

        private CancellationTokenSource _cancellationTokenSource;

        private readonly Logger _logger;

        public Guid SKUProductGUIDId { get; set; }

        private readonly IEventAggregator _eventAggregator;



        private readonly IProductTypeService _productTypeService;

        public DelegateCommand ProductTypeChangedCommand { get; private set; }


        private bool _validuptxtBoxVisibility;
        public bool ValidtxtBoxVisibility
        {
            get { return _validuptxtBoxVisibility; }
            set { SetProperty(ref _validuptxtBoxVisibility, value); }
        }


        private bool _lifetimeCheckBoxVisibility;
        public bool LifeTimeBoxVisibility
        {
            get { return _lifetimeCheckBoxVisibility; }
            set { SetProperty(ref _lifetimeCheckBoxVisibility, value); }
        }

        public DelegateCommand<object> MinMarginTextChangedCommand { get; private set; }

        public SKUProductCardViewModel(CategoryModel category, IProductService productService, IProductTypeService productTypeService, ISupplierService supplierService, INotificationService notificationService, Logger logger, IEventAggregator eventAggregator)
        {
            SKUProductGUIDId = Guid.NewGuid();

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _productService = productService ?? throw new ArgumentNullException(nameof(productService));

            _supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));

            _productTypeService = productTypeService ?? throw new ArgumentNullException(nameof(productTypeService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            SubUnitTypes = GetSubUnitTypes();
         
            if (category != null)
                headerChange(category);

            WarrantyServices = GetWarrantyServices();

            GSTslabs = GetGSTslabs();

            MinMarginTextChangedCommand = new DelegateCommand<object>(MinimumMarginPercentage);

        }


        private string _productName;
        public string ProductName
        {
            get { return _productName; }
            set
            {
                SetProperty(ref _productName, value);
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                SetProperty(ref _description, value);
            }
        }



        private string _validupto;
        public string ValidupTo
        {
            get { return _validupto; }
            set
            {
                SetProperty(ref _validupto, value);
                if (!String.IsNullOrEmpty(value) && LifeTime)
                    LifeTime = false;
            }
        }

        private bool _lifetime;
        public bool LifeTime
        {
            get { return _lifetime; }
            set
            {
                SetProperty(ref _lifetime, value);
                if (value && !String.IsNullOrEmpty(ValidupTo))
                    ValidupTo = null;
            }
        }

        private string _license;
        public string License
        {
            get { return _license; }
            set
            {
                SetProperty(ref _license, value);
            }
        }

        private void headerChange(CategoryModel category)
        {
            ValidtxtBoxVisibility = false;
            LifeTimeBoxVisibility = false;
            
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
        private ObservableCollection<string> GetSubUnitTypes()
        {
            return new ObservableCollection<string>
            {
                 "Nos","KG","LT","ML","GRAMS","UNITS"
            };
        }


        private int? _serialNumber;

        public int? SerialNumber
        {
            get { return _serialNumber; }
            set { SetProperty(ref _serialNumber, value); }
        }

        private string _technicalName;
        public string TechnicalName {
            get => _technicalName;
            set => SetProperty(ref _technicalName , value);
        }

        private string _packingSize;
        public string PackingSize {
            get => _packingSize;
            set => SetProperty(ref _packingSize, value);
        }

        private string _uom;
        public string UOM {
            get => _uom;
            set => SetProperty( ref _uom , value );
        }

        private string _type;
        public string Type {
            get => _type;
            set => SetProperty(ref _type , value);
        }

        private string _productSKU;

        public string ProductSKU {
            get => _productSKU;
            set => SetProperty(ref _productSKU, value);
        }



        private string _hsnCode;
        public string HsnCode {
            get =>  _hsnCode; 
            set => SetProperty(ref _hsnCode, value);
        }

       
        private ObservableCollection<WarrantyService> GetWarrantyServices() {
            if (ApplicationSettings.WARRENTY_SERVICE != null && ApplicationSettings.WARRENTY_SERVICE.Any()) {
                return new ObservableCollection<WarrantyService>(
                    ApplicationSettings.WARRENTY_SERVICE.Select(x => new WarrantyService { Name = x }));

            }

            return default;
        }

        private ObservableCollection<WarrantyService> _warrantyServices;

        public ObservableCollection<WarrantyService> WarrantyServices {
            get => _warrantyServices;
            set => SetProperty(ref _warrantyServices, value);
        }

        private WarrantyService _selectedWarrantyService;

        public WarrantyService SelectedWarrantyService {
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

            }
        }

        private decimal _MinMargin;

        public decimal MinMargin
        {
            get { return _MinMargin; }
            set { 
                if(value < 0 || value>100)
                {
                    value = 0;
                }
                SetProperty(ref _MinMargin , value); 
            }
        }

        public void MinimumMarginPercentage(object obj)
        {
            try
            {
                var _minMargin = (SKUProductCardViewModel)obj;

                if (MinMargin > 100)
                {
                    MinMargin = 0;
                }
            }
            catch(Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }
    }


    public class WarrantyService {
        public string Name { get; set; }
    }
}
