using FalcaPOS.Common.Logger;
using FalcaPOS.Common.Validations;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.ProductTypes;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
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


        public SKUProductCardViewModel(CategoryModel category, IProductService productService, IProductTypeService productTypeService, ISupplierService supplierService, INotificationService notificationService, Logger logger)
        {
            SKUProductGUIDId = Guid.NewGuid();

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _productService = productService ?? throw new ArgumentNullException(nameof(productService));

            _supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));

            _productTypeService = productTypeService ?? throw new ArgumentNullException(nameof(productTypeService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            SubUnitTypes = GetSubUnitTypes();

            if (category != null)
                headerChange(category);


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
            switch (category.CategoryName.ToLower())
            {
                case "crop protection chemicals":
                    ValidtxtBoxVisibility = true;
                    LifeTimeBoxVisibility = true;
                    break;
                case "fertilizers":
                case "speciality nutrients":
                case "seeds":
                    ValidtxtBoxVisibility = true;
                    break;
                default:
                    ValidtxtBoxVisibility = false;
                    LifeTimeBoxVisibility = false;
                    break;
            }

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
                 "Packet","Bottle","Piece","Bucket","Bag","Tin"
            };
        }


        private int? _serailNumber;

        public int? SerailNumber
        {
            get { return _serailNumber; }
            set { SetProperty(ref _serailNumber, value); }
        }

    }
}
