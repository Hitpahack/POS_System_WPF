using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Indent;
using FalcaPOS.Entites.Location;
using FalcaPOS.Entites.Products;
using FalcaPOS.Entites.ProductTypes;
using FalcaPOS.Entites.Sku;
using FalcaPOS.Entites.Stores;
using FalcaPOS.Entites.Zone;
using MahApps.Metro.Controls;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Windows;
using Telerik.Windows.Documents.Spreadsheet.Expressions.Functions;
using Unity;
using Unity.Resolution;

namespace FalcaPOS.Store.ViewModels
{
    public class AddStoreFlyoutViewModel : BindableBase
    {
        private readonly ProgressService _progressService;
        private readonly IUnityContainer _container;

        private readonly IEventAggregator _eventAggregator;

        private readonly ICommonService _commonService;

        private readonly IZoneService _zoneService;

        private readonly IStoreService _storeService;

        public DelegateCommand StateSelectionChanged { get; private set; }

        public DelegateCommand DistrictSelectionChanged { get; private set; }
        public DelegateCommand ZoneSelectionChanged { get; private set; }
        public DelegateCommand TerritorySelectionChanged { get; private set; }

        public DelegateCommand AddStoreCommnad { get; private set; }

        public DelegateCommand<string> StoreTypeCommand { get; private set; }

        public DelegateCommand ResetStoreCommnad { get; private set; }
        
        public DelegateCommand AddStoreLicenseCommand { get; private set; }        
        public DelegateCommand<object> RemoveAddStoreLicenseCommand { get; private set; }

        private readonly INotificationService _notificationService;

        private readonly Logger _logger;



        public AddStoreFlyoutViewModel(ProgressService progressService, IUnityContainer container, IEventAggregator eventAggregator, ICommonService commonService, IZoneService zoneService, IStoreService storeService, INotificationService notificationService, Logger logger)
        {
             
            _progressService = progressService;
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));

            _zoneService = zoneService ?? throw new ArgumentNullException(nameof(zoneService));

            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _eventAggregator.GetEvent<AddStoreFlyoutOpenEvent>().Subscribe(OpenFlyout);

            AddStoreLicenseCommand = new DelegateCommand(AddStoreLicenseCard);     
            
            RemoveAddStoreLicenseCommand = new DelegateCommand<object>(RemoveLicenseCard);

            StoreLicenses = new ObservableCollection<AddStoreLicenseViewModel>();

            StoreLicenses.CollectionChanged -= StoreLicenses_CollectionChanged;

            StoreLicenses.CollectionChanged += StoreLicenses_CollectionChanged;

            AddStoreCommnad = new DelegateCommand(AddStore);

            StoreTypeCommand = new DelegateCommand<string>(SelectStoreType);

            ResetStoreCommnad = new DelegateCommand(ResetStore);

            StateSelectionChanged = new DelegateCommand(LoadDistricts);

            DistrictSelectionChanged = new DelegateCommand(() => GetStoreInvoiceFomat());
            ZoneSelectionChanged = new DelegateCommand(LoadTerritories);
          
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private void SelectStoreType(string obj)
        {
            SelectedStoreType = obj;
        }

        private CancellationTokenSource _cancellationTokenSource;


        private async void GetStoreInvoiceFomat()
        {
            try
            {
                if (SelectedDistrict == null || SelectedState == null)
                {
                    //invalid selection.
                    StoreInvoiceFormat = string.Empty;
                    StoreInvoiceSequence = 0;

                    return;
                }

                _cancellationTokenSource?.Cancel();

                _cancellationTokenSource = new CancellationTokenSource();


                await Task.Run(async () =>
                {

                    var _result = await _storeService.GetStoreInvoiceFormat(SelectedDistrict.DistrictId, _cancellationTokenSource.Token);

                    if (_result != null && _result.IsSuccess && _result.Data.IsValidString())
                    {
                        Application.Current?.Dispatcher.Invoke(() =>
                        {

                            StoreInvoiceFormat = _result.Data;
                        });


                    }
                    else
                    {
                        if (_result != null && _result.Error.IsValidString())
                        {
                            _notificationService.ShowMessage(_result.Error, NotificationType.Error);
                        }
                    }

                }, _cancellationTokenSource.Token);


            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }

        }

        private void ResetStore()
        {
            ResetForm();

        }

        private void ResetForm()
        {
            IsStoreChecked = true;
            SelectedStoreType="F-Shop";
            SelectedState = null;
            SelectedDistrict = null;
            SelectedZone = null;
            SelectedTerritory = null;
            Name = null;
            Phone = null;
            Alternatephone = null;
            Email = null;
            Street = null;
            Pincode = null;
            City = null;
            StoreInvoiceFormat = string.Empty;
            StoreInvoiceSequence = 0;
            StoreLicenses.Clear();            
        }

        private async void AddStore()
        {
            try
            {
                bool _isValid = ValidateData();

                if (!_isValid) return;

                var _store = GetStore();
               

                await _progressService.StartProgressAsync();

                var _result = await _storeService.AddStore(_store);             
   
                if (_result != null && _result.IsSuccess)
                {
                    _eventAggregator.GetEvent<ReloadStoresEvent>().Publish();

                    ShowAlert($"Store {_store.Name} added", NotificationType.Success);

                    ResetForm();

                    IsOpen = false;
                }

                await _progressService.StopProgressAsync();

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }
      
        private IEnumerable<StoreLicense> GetStoreLicenses()
        {
            foreach (var _licenses in StoreLicenses)
            {
                if (_licenses == null || _licenses.SelectedCategory == null) continue;

                yield return new StoreLicense
                {
                    CategoryRef = (int)_licenses.SelectedCategory.Id,
                    WholesaleLicense = _licenses.WholesaleLicense,
                    NormalLicense= _licenses.NormalLicense,                    
                };
            }

        }
        private Entites.Stores.Store GetStore()
        {
            return new Entites.Stores.Store
            {
                LastSequenceNumber = StoreInvoiceSequence,
                StoreInvoiceFormat = StoreInvoiceFormat,
                Name = Name,
                StoreType = SelectedStoreType,
                Address = new Entites.Customers.Address
                {
                    Alternatephone = Alternatephone,
                    City = City,
                    District = SelectedDistrict.Name,
                    Email = Email,
                    Phone = Phone,
                    Pincode = Pincode,
                    State = SelectedState.Name,
                    Street = Street,
                    DistrictID = SelectedDistrict.DistrictId,
                    StateID = SelectedState.StateId                 
                    
                },
                IsParent = IsParent,
                Territory_ref = SelectedTerritory.Id,
                Licenses = GetStoreLicenses(),
            };
        }


        private void ShowAlert(string msg, NotificationType notificationType)
            => _eventAggregator.GetEvent<NotifyMessage>()
                               .Publish(new Common.Models.ToastMessage { Message = msg, MessageType = notificationType });

        /// <summary>
        /// Validation for Add Store menu fields
        /// </summary>
        /// <returns>returns true if all fields are valid.if not returns false</returns>
        private bool ValidateData()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                ShowAlert("Store Name is required.", NotificationType.Error);

                return false;
            }
            else if (string.IsNullOrWhiteSpace(Phone))
            {
                ShowAlert("Phone number is required.", NotificationType.Error);

                return false;
            }
            else if (!Phone.IsValidPhone())
            {
                ShowAlert("Phone number is Invalid.", NotificationType.Error);

                return false;
            }
            else if (string.IsNullOrWhiteSpace(Email))
            {
                ShowAlert("Email is required", NotificationType.Error);
                return false;
            }
            else if (!IsValidEmail(Email))
            {
                ShowAlert("Invalid Email address", NotificationType.Error);
                return false;
            }
            else if (SelectedState == null)
            {
                ShowAlert("Select a state", NotificationType.Error);
                return false;
            }
            else if(SelectedZone==null)
            {
                ShowAlert("Select a Zone", NotificationType.Error);
                return false;
            }
           
            else if(SelectedTerritory==null)
            {
                ShowAlert("Select a Territory", NotificationType.Error);
                return false;
            }
            else if (SelectedDistrict == null)
            {
                ShowAlert("Select a district", NotificationType.Error);
                return false;
            }
            else if (string.IsNullOrWhiteSpace(City))
            {
                ShowAlert("City name is required ", NotificationType.Error);
                return false;
            }
            else if (Pincode == null)
            {
                ShowAlert("Pincode is required ", NotificationType.Error);
                return false;
            }
            else if (Pincode.ToString().Length != 6)
            {
                ShowAlert("Pincode is invalid ", NotificationType.Error);
                return false;
            }
            else if (!StoreInvoiceFormat.IsValidString())
            {
                ShowAlert("Invoice format is required", NotificationType.Error);
                return false;
            }
            else if (StoreInvoiceSequence <= 0)
            {
                ShowAlert("Invoice sequence is invalid", NotificationType.Error);
                return false;
            }
            else if (SelectedStoreType == null)
            {
                ShowAlert("Select a store type", NotificationType.Error);
                return false;
            }
            if (Alternatephone.IsValidString())
            {
                if (!Alternatephone.IsValidPhone())
                {
                    ShowAlert("Alternate phone number  is invalid", NotificationType.Error);
                    return false;
                }
            }
            if (StoreLicenses.Count > 0)
            {
                foreach (var item in StoreLicenses)
                {
                    if (item.SelectedCategory == null)
                    {
                        ShowAlert("Please select a category", NotificationType.Error);
                        return false;
                    }
                    if (String.IsNullOrEmpty(item.NormalLicense))
                    {
                        ShowAlert($"Please enter a valid retail license for {item.SelectedCategory.CategoryName}", NotificationType.Error);
                        return false;
                    }

                }
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            return new EmailAddressAttribute().IsValid(email);
        }

        private async void LoadDistricts()
        {
            SelectedDistrict = null;
            SelectedZone = null;

            Districts?.Clear();
            Zones?.Clear();

            if (SelectedState == null || SelectedState.StateId <= 0) return;

            try
            {
                var _result = await _commonService.GetDistricts(SelectedState.StateId);
                if (_result != null)
                    Districts = new ObservableCollection<District>(_result);

                var _Zoneresult = await _zoneService.GetZones(SelectedState.StateId);
                if (_Zoneresult != null)
                    Zones = new ObservableCollection<NewZone>(_Zoneresult);
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }

        }
        private async void LoadTerritories()
        {
            SelectedTerritory = null;

            Territories?.Clear();

            if (SelectedZone == null || SelectedZone.Id <= 0) return;

            try
            {
                var _result = await _zoneService.GetTerritories(SelectedZone.Id);

                if (_result != null)
                    Territories = new ObservableCollection<Territory>(_result);
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }

        }
        private async void OpenFlyout(StoreFlyout isopen)
        {

            try
            {
                Header = isopen.IsParent == true ? "Add Store" : "Add RSP";
                IsParent = isopen.IsParent;
                Width = 670;
                Height = GridLength.Auto;
                Position = Position.Right;
                IsOpen = isopen.IsOpen;
                if (isopen.IsOpen)
                {
                    ClearDefaults();

                    var _result = await _commonService.GetStates("?isenabled = true");

                    if (_result != null)
                        States = new ObservableCollection<State>(_result);
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private void ClearDefaults()
        {
            ResetForm();
            States?.Clear();
            Districts?.Clear();            
        }
        //private void AddNewStoreLicense()
        //{
        //    try
        //    { 
        //        var _SLicense = _container.Resolve<AddStoreLicenseViewModel>();
        //        _SLicense.SlNo = SlNo;
        //        StoreLicenses.Add(_SLicense);
        //        SlNo++;
        //    }
        //    catch (Exception _ex)
        //    {
        //        _logger.LogError(_ex.Message);
        //    }
        //}
        private void AddStoreLicenseCard()
        {
            try
            {
                var licenseCodes = _container.Resolve<AddStoreLicenseViewModel>(new ParameterOverride("StoreLicense", null));
                StoreLicenses.Add(licenseCodes);
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private void RemoveLicenseCard(object obj)
        {
            if (obj is Guid _productGUIDId)
            {
                var _productRemove = StoreLicenses?.FirstOrDefault(x => x.StoreLicenseGUIDId == _productGUIDId);

                if (_productRemove != null)
                {
                    StoreLicenses.Remove(_productRemove);
                }
            }
        }
        private void StoreLicenses_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("StoreLicenses");
        }
        private ObservableCollection<AddStoreLicenseViewModel> _storeLicenses;
        public ObservableCollection<AddStoreLicenseViewModel> StoreLicenses
        {
            get { return _storeLicenses; }
            set { SetProperty(ref _storeLicenses, value); }
        }

        #region Props
        private ObservableCollection<Category> _stores;
        public ObservableCollection<Category> Stores
        {
            get => _stores;
            set => SetProperty(ref _stores, value);
        }
        private string _storeInvoiceFormat;
        public string StoreInvoiceFormat
        {
            get => _storeInvoiceFormat;
            set => SetProperty(ref _storeInvoiceFormat, value);
        }
        private ObservableCollection<Category> _categoryList = new ObservableCollection<Category>();

        public ObservableCollection<Category> CategoryList
        {
            get => _categoryList;
            set => SetProperty(ref _categoryList, value);
        }
        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                SetProperty(ref _selectedCategory, value);
            }
        }

        private int _storeInvoiceSequence;
        public int StoreInvoiceSequence
        {
            get => _storeInvoiceSequence;
            set => SetProperty(ref _storeInvoiceSequence, value);
        }


        private bool _isOpen;
        public bool IsOpen
        {
            get { return _isOpen; }
            set { SetProperty(ref _isOpen, value); }
        }

        private int _width;
        public int Width
        {
            get { return _width; }
            set { SetProperty(ref _width, value); }
        }

        private GridLength _height;
        public GridLength Height
        {
            get { return _height; }
            set { SetProperty(ref _height, value); }
        }

        private Position _position;
        public Position Position
        {
            get { return _position; }
            set { SetProperty(ref _position, value); }
        }

        private string _header;
        public string Header
        {
            get { return _header; }
            set { SetProperty(ref _header, value); }
        }

        private ObservableCollection<State> _states;
        public ObservableCollection<State> States
        {
            get { return _states; }
            set { SetProperty(ref _states, value); }
        }

        private ObservableCollection<District> _districts;
        public ObservableCollection<District> Districts
        {
            get { return _districts; }
            set { SetProperty(ref _districts, value); }
        }

        private State _selectedState;
        public State SelectedState
        {
            get { return _selectedState; }
            set { SetProperty(ref _selectedState, value); }
        }

        private string _selectedStoreType;
        public string SelectedStoreType
        {
            get { return _selectedStoreType; }
            set { SetProperty(ref _selectedStoreType, value); }
        }

        private bool _isStoreChecked;

        public bool IsStoreChecked
        {
            get { return _isStoreChecked; }
            set { SetProperty(ref _isStoreChecked, value); }
        }



        private District _selectedDistrict;
        public District SelectedDistrict
        {
            get { return _selectedDistrict; }
            set { SetProperty(ref _selectedDistrict, value); }
        }
        private ObservableCollection<NewZone> _Zones;
        public ObservableCollection<NewZone> Zones
        {
            get { return _Zones; }
            set { SetProperty(ref _Zones, value); }
        }
        private NewZone _selectedZone;
        public NewZone SelectedZone
        {
            get { return _selectedZone; }
            set { SetProperty(ref _selectedZone, value); }
        }
        private ObservableCollection<Territory> _territories;
        public ObservableCollection<Territory> Territories
        {
            get { return _territories; }
            set { SetProperty(ref _territories, value); }
        }
      
        private Territory _selectedTerritory;
        public Territory SelectedTerritory
        {
            get { return _selectedTerritory; }
            set { SetProperty(ref _selectedTerritory, value); }
        }

        private string _name;


        public string Name
        {
            get { return _name; }
            set
            {
                SetProperty(ref _name, value);
            }
        }

        private string _gstnumber;

        public string GstNumber
        {
            get { return _gstnumber; }
            set { SetProperty(ref _gstnumber, value); }
        }
        private string _phone;

        public string Phone
        {
            get { return _phone; }
            set { SetProperty(ref _phone, value); }
        }
        private string _alternatephone;
        public string Alternatephone
        {
            get { return _alternatephone; }
            set { SetProperty(ref _alternatephone, value); }
        }
        private string _email;
        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }
        private string _street;

        public string Street
        {
            get { return _street; }
            set { SetProperty(ref _street, value); }
        }
        private int? _pincode;
        public int? Pincode
        {
            get { return _pincode; }
            set { SetProperty(ref _pincode, value); }
        }
        private string _city;
        public string City
        {
            get => _city;
            set => SetProperty(ref _city, value);
        }

        private bool _isparent;
        public bool IsParent
        {
            get => _isparent;
            set => SetProperty(ref _isparent, value);
        }
        private int _slNo;

        public int SlNo
        {
            get => _slNo;
            set => SetProperty(ref _slNo, value);
        }
        #endregion
       

    }
}
