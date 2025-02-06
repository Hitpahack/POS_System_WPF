using FalcaPOS.Common;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Location;
using FalcaPOS.Entites.Zone;
using MahApps.Metro.Controls;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Xml.Linq;
using FalcaPOS.Entites.Stores;
using FalcaPOS.Store.Views;
using System.Web.Configuration;
using FalcaPOS.Entites.Suppliers;
using FalcaPOS.Entites.Indent;
using Unity.Resolution;
using Unity;
using System.ComponentModel;
using Telerik.Windows.Documents.Spreadsheet.Expressions.Functions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using System.Collections.Generic;

namespace FalcaPOS.Store.ViewModels
{
    public class EditStoreFlyoutViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly ICommonService _commonService;

        private readonly IZoneService _zoneService;

        private readonly IStoreService _storeService;

        //public DelegateCommand StateSelectionChanged { get; private set; }
        private readonly IUnityContainer _container;
        public DelegateCommand CancelCommnad { get; private set; }

        public DelegateCommand UpdateStoreCommand { get; private set; }

        public DelegateCommand<object> ZoneSelectionChanged { get; private set; }

        private readonly ProgressService _progressService;
        public DelegateCommand AddEditStoreLicenseCommand { get; private set; }

        private readonly Logger _logger;

        public DelegateCommand<object> RemoveEditStoreLicenseCommand { get; private set; }


        public EditStoreFlyoutViewModel(ProgressService progressService, IEventAggregator eventAggregator, ICommonService commonService, IStoreService storeService, IZoneService zoneService, Logger logger, IUnityContainer container)
        {
            _progressService = progressService;

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));

            _zoneService = zoneService ?? throw new ArgumentNullException(nameof(zoneService));

            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            _eventAggregator.GetEvent<EditStoreFlyoutOpenEvent>().Subscribe(EditStore);
            //EditStoreLicenseCommand = new DelegateCommand<object>(LoadEditStoreLicenses);
            //StateSelectionChanged = new DelegateCommand(LoadDistricts);

            CancelCommnad = new DelegateCommand(CloseFlyout);

            UpdateStoreCommand = new DelegateCommand(UpdateStore);

            ZoneSelectionChanged = new DelegateCommand<object>(LoadTerritoriesOnZoneChanged);

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _container = container ?? throw new ArgumentNullException(nameof(container));

            StoreLicenseCard = new ObservableCollection<EditStoreLicenseViewModel>();

            StoreLicenseCard.CollectionChanged -= StoreLicenseCard_CollectionChanged;

            StoreLicenseCard.CollectionChanged += StoreLicenseCard_CollectionChanged;

            AddEditStoreLicenseCommand = new DelegateCommand(AddStoreLicenseCard);

            RemoveEditStoreLicenseCommand = new DelegateCommand<object>(RemoveLicenseCard);

        }
        //private async void BindValuesLicense(Entites.Stores.Store store)
        //{
        //    var _resultl = await _storeService.GetStoreLicense(store.StoreId);
        //    if (_resultl != null || _resultl.Count() != 0)
        //    {
        //        foreach (var _sl in _resultl)
        //        {

        //            //Sel = _sl.CategoryRef,
        //            //WholesaleLicense = _sl.WholesaleLicense,
        //            //NormalLicense = _sl.NormalLicense,


        //        }
        //    }
        //}
        public async void LoadEditStoreLicenses(int StoreId)
        {
            try
            {
               

               // var store = ((EditStoreFlyoutViewModel)(obj));
                if (StoreId != null)
                {
                    StoreLicenseCard.Clear();   
                    var _resultl = await _storeService.GetStoreLicense(StoreId);
                    if (_resultl != null || _resultl.Count() != 0)
                    {
                        foreach (var item in _resultl)
                        {
                            var licenseCodes = _container.Resolve<EditStoreLicenseViewModel>(new ParameterOverride("StoreLicense", item));

                            StoreLicenseCard.Add(licenseCodes);

                        }
                    }
                }             

            }

            catch (Exception _ex)
            {
                _logger.LogError("getting error in selection change indent", _ex);

            }
        }
       
        private async void UpdateStore()
        {
            try
            {
                bool _isValid = ValidateData();

                if (!_isValid) return;

                var _store = GetStore();

                await _progressService.StartProgressAsync();

                await Task.Run(async () =>
                {
                    var _result = await _storeService.UpdateStore(_store);

                    if (_result != null && _result.IsSuccess)
                    {
                        _eventAggregator.GetEvent<ReloadStoresEvent>().Publish();

                        ShowAlert($"Store {_store.Name} Update", NotificationType.Success);

                        ResetForm();

                        IsOpen = false;

                    }
                });



            }
            catch (Exception)
            {
            }

            await _progressService.StopProgressAsync();
        }
        private Entites.Stores.Store GetStore()
        {
            var _license = new List<StoreLicense>();
            if(StoreLicenseCard!=null && StoreLicenseCard.Count > 0) {
                foreach (var item in StoreLicenseCard) {
                    _license.Add(new StoreLicense() {
                        CategoryRef = item.SelectedCategory.Id,
                        WholesaleLicense = item.WholesaleLicense,
                        NormalLicense = item.NormalLicense,
                    });
                }
            }
            
            return new Entites.Stores.Store
            {
                Name = Name,
                StoreId = StoreId,

                Territory_ref = SelectedTerritory.Id,
                Address = new Entites.Customers.Address
                {
                    Alternatephone = Alternatephone,
                    City = City,
                    // District = SelectedDistrict.Name,
                    Email = Email,
                    Phone = Phone,
                    Pincode = Pincode,
                    //State = SelectedState.Name,
                    Street = Street,
                },
               
                Licenses=_license
            };
        }

        private bool ValidateData()
        {


            if (string.IsNullOrWhiteSpace(Name))
            {
                ShowAlert("Store Name is required.", NotificationType.Error);

                return false;
            }
            else if (!Phone.IsValidPhone())
            {
                ShowAlert("Phone number is invalid.", NotificationType.Error);

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
            else if (SelectedZone == null)
            {
                ShowAlert("Select a Zone", NotificationType.Error);
                return false;

            }
            else if (SelectedTerritory == null)
            {
                ShowAlert("Select a Territory", NotificationType.Error);
                return false;
            }
            else if (string.IsNullOrWhiteSpace(City))
            {
                ShowAlert("City name is required ", NotificationType.Error);
                return false;
            }
            else if (Pincode == null || Pincode?.ToString().Length != 6)
            {
                ShowAlert("Pincode is invalid", NotificationType.Error);
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
            //else if (!StoreInvoiceFormat.IsValidString())
            //{
            //    ShowAlert("Store format is required ", NotificationType.Error);
            //    return false;
            //}

            if (StoreLicenseCard.Count > 0)
            {
                foreach (var item in StoreLicenseCard)
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
        private bool IsValidEmail(string email) => new EmailAddressAttribute().IsValid(email);

        private void ShowAlert(string msg, NotificationType notificationType)
           => _eventAggregator.GetEvent<NotifyMessage>()
                              .Publish(new Common.Models.ToastMessage { Message = msg, MessageType = notificationType });

        private void CloseFlyout()
        {
            ClearDefaults();

            IsOpen = false;
        }

        //private async void LoadDistricts()
        //{
        //    SelectedDistrict = null;

        //    Districts?.Clear();

        //    if (SelectedState == null || SelectedState.StateId <= 0) return;

        //    try
        //    {
        //        var _result = await _commonService.GetDistricts(SelectedState.StateId);

        //        if (_result != null)
        //            Districts = new ObservableCollection<District>(_result);
        //    }
        //    catch (Exception _ex)
        //    {

        //    }

        //}

        private async void EditStore(object obj)
        {

            var Isopen = System.Convert.ToString(obj) == "False" ? false : true;
            Width = 670;
            Height = GridLength.Auto;
            Position = Position.Right;
            IsOpen = Isopen;
            if (Isopen)
            {
                if (obj is Entites.Stores.Store _store)
                {
                    try
                    {         
                        // Checks the first occurence of the ',' in the store name. Ex: HAVERI SUGGI, KARNATAKA
                        int index = _store.Name.IndexOf(',');
                        
                        // if index is  -1, it means that there's no comma in the string.
                        if (index != -1)
                        {
                            // Removes the state name from the store name.
                            _store.Name = _store.Name.Remove(index, _store.Name.Length - index);
                        }
                        
                        ClearDefaults();
                        // Loads the zone and territory of the store.
                        LoadStateZoneAndTerritory(_store);
                      
                        BindValues(_store);                        

                        Header = $"Edit Store {_store.Name}";

                    }
                    catch (Exception _ex)
                    {
                        _logger.LogError(_ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Loads the territories based on the zone selected.
        /// </summary>
        /// <param name="obj">object.</param>
        private async void LoadTerritoriesOnZoneChanged(object obj)
        {
            Territories?.Clear();

            if (SelectedZone == null || SelectedZone.Id <= 0)
            {
                SelectedTerritory = null;
                Territories?.Clear();
                return;
            }

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
        private CancellationTokenSource _cancellationTokenSource;
        private async void LoadStateZoneAndTerritory(Entites.Stores.Store store)
        {
            SelectedZone = null;
            SelectedTerritory = null;
            Zones?.Clear();
            Territories?.Clear();
            _cancellationTokenSource?.Cancel();

            _cancellationTokenSource = new CancellationTokenSource();
            try
            {
                await Task.Run(async () =>
                {
                    var _Zoneresult = await _zoneService.GetZones(store.Address.StateID);
                    if (_Zoneresult != null)
                    {

                        Application.Current?.Dispatcher.InvokeAsync(async () =>
                        {
                            Zones = new ObservableCollection<NewZone>(_Zoneresult);
                            SelectedZone = new NewZone();
                            SelectedZone = Zones.FirstOrDefault(x => x.Name == store.Zone);
                            if (SelectedZone == null || SelectedZone.Id <= 0)
                            {
                                SelectedTerritory = null;
                                Territories?.Clear();
                                return;
                            }
                            var _result = await _zoneService.GetTerritories(SelectedZone.Id);
                            if (_result != null)
                            {
                                Application.Current?.Dispatcher.InvokeAsync(() =>
                                {
                                    Territories = new ObservableCollection<Territory>(_result);
                                    SelectedTerritory = new Territory();
                                    SelectedTerritory = Territories.FirstOrDefault(x => x.Name == store.TerritoryName);
                                });
                              
                            }
                            

                        });
                        
                        
                    }
                    
                }, _cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        

        
private void ClearDefaults()
        {
            ResetForm();
            //States?.Clear();
            //Districts?.Clear();

        }
        private void ResetForm()
        {
            //SelectedState = null;
            //SelectedDistrict = null;
            Name = null;
            Phone = null;
            Alternatephone = null;
            Email = null;
            Street = null;
            Pincode = null;
            City = null;
            StoreId = 0;
            State = null;
            StoreType = null;
            District = null;
            SelectedZone = null;
            SelectedTerritory = null;
            IsFshop = false;
            IsRsp=false;
        }



        private async void BindValues(Entites.Stores.Store store)
        {
            Thread.Sleep(1000);
            Name = store.Name;
            StoreId = store.StoreId;
            StoreInvoiceFormat = store.StoreInvoiceFormat;
            LastSequenceNumber = store.LastSequenceNumber;
            StoreType = store.StoreType;
            Territory_ref = store.Territory_ref.GetValueOrDefault();
            if (store.Address != null)
            {
                Email = store.Address.Email;
                Phone = store.Address.Phone;
                Alternatephone = store.Address.Alternatephone;
                Pincode = store.Address.Pincode;
                City = store.Address.City;
                State = store.Address.State;
                District = store.Address.District;
                Street = store.Address.Street;
                Zone = store.Zone;
                Territory = store.TerritoryName;               
            }
            LoadEditStoreLicenses(store.StoreId);
            if(StoreType=="RSP") 
                IsRsp = true;
            else
                IsFshop = true;
        }

        

        public int StoreId { get; set; }

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

        private ObservableCollection<Territory> _territories;
        public ObservableCollection<Territory> Territories
        {
            get { return _territories; }
            set { SetProperty(ref _territories, value); }
        }

        //private ObservableCollection<State> _states;
        //public ObservableCollection<State> States
        //{
        //    get { return _states; }
        //    set { SetProperty(ref _states, value); }
        //}

        //private ObservableCollection<District> _districts;
        //public ObservableCollection<District> Districts
        //{
        //    get { return _districts; }
        //    set { SetProperty(ref _districts, value); }
        //}

        //private State _selectedState;
        //public State SelectedState
        //{
        //    get { return _selectedState; }
        //    set { SetProperty(ref _selectedState, value); }
        //}

        //private District _selectedDistrict;
        //public District SelectedDistrict
        //{
        //    get { return _selectedDistrict; }
        //    set { SetProperty(ref _selectedDistrict, value); }
        //}


        private NewZone _selectedZone;
        public NewZone SelectedZone
        {
            get { return _selectedZone; }
            set { SetProperty(ref _selectedZone, value); }
        }
       
        private ObservableCollection<ZoneTerritoryView> _zoneTerritoryView;
        public ObservableCollection<ZoneTerritoryView> ZoneTerritoryView
        {
            get { return _zoneTerritoryView; }
            set { SetProperty(ref _zoneTerritoryView, value); }
        }
        private ZoneTerritoryView _selectedZoneTerritory;
        public ZoneTerritoryView SelectedZoneTerritory
        {
            get { return _selectedZoneTerritory; }
            set { SetProperty(ref _selectedZoneTerritory, value); }
        }
        private Territory _selectedTerritory;
        public Territory SelectedTerritory
        {
            get { return _selectedTerritory; }
            set
            {
                SetProperty(ref _selectedTerritory, value);
            }
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

        private string _storeInvoiceFomart;

        public string StoreInvoiceFormat
        {
            get => _storeInvoiceFomart;
            set => SetProperty(ref _storeInvoiceFomart, value);
        }


        private int? _lastSequenceNumber;

        public int? LastSequenceNumber
        {
            get => _lastSequenceNumber;
            set => SetProperty(ref _lastSequenceNumber, value);
        }

        private string _storeType;

        public string StoreType
        {
            get => _storeType;
            set => SetProperty(ref _storeType, value);
        }

        private bool _isRsp;

        public bool IsRsp
        {
            get => _isRsp;
            set => SetProperty(ref _isRsp, value);
        }

        private bool _isFshop;

        public bool IsFshop
        {
            get => _isFshop;
            set => SetProperty(ref _isFshop, value);
        }
        private string _state;

        public string State
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }
        private string _district;

        public string District
        {
            get => _district;
            set => SetProperty(ref _district, value);
        }

        private ObservableCollection<NewZone> _Zones;
        public ObservableCollection<NewZone> Zones
        {
            get { return _Zones; }
            set { SetProperty(ref _Zones, value); }
        }


        private int _Territoryref;
        public int Territory_ref
    {
            get { return _Territoryref; }
            set { SetProperty(ref _Territoryref, value); }
        }
       
        
        private string _zone;
        public string Zone
        {
            get => _zone;
            set => SetProperty(ref _zone, value);
        }
        private string _territory;
        public string Territory
        {
            get => _territory;
            set => SetProperty(ref _territory, value);
        }

        private void StoreLicenseCard_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            this.RaisePropertyChanged("StoreLicenseCard");
        }


        private ObservableCollection<EditStoreLicenseViewModel> _storeLicenseCard;

        public ObservableCollection<EditStoreLicenseViewModel> StoreLicenseCard {
            get { return _storeLicenseCard; }
            set { SetProperty(ref _storeLicenseCard, value); }
        }
        private void AddStoreLicenseCard() {
            try {

                var licenseCodes = _container.Resolve<EditStoreLicenseViewModel>(new ParameterOverride("StoreLicense", null));

                StoreLicenseCard.Add(licenseCodes);


            }
            catch (Exception _ex) {
                _logger.LogError(_ex.Message);
            }


        }

        private void RemoveLicenseCard(object obj) {
            if (obj is Guid _productGUIDId) {
                var _productRemove = StoreLicenseCard?.FirstOrDefault(x => x.StoreLicenseGUIDId == _productGUIDId);
              
                if (_productRemove != null) {
                    
                    StoreLicenseCard.Remove(_productRemove);
                 
                }

            }
        }
    }
}
