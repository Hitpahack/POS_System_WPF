using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Validations;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Location;
using FalcaPOS.Entites.Suppliers;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.AddInventory.ViewModels
{
    public class AddSupplierViewModel : ValidationBase
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly ISupplierService _supplierService;

        private readonly ICommonService _commonService;

        public DelegateCommand StateSelectionChanged { get; private set; }

        public DelegateCommand AddSupplierCommnad { get; private set; }

        public DelegateCommand ResetSupplierCommnad { get; private set; }

        public AddSupplierViewModel(IEventAggregator eventAggregator, ISupplierService supplierService, ICommonService commonService)
        {
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));

            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));

            AddSupplierCommnad = new DelegateCommand(AddSupplierdetails)
                                    .ObservesCanExecute(() => IsValid);

            LoadStates();

            ResetSupplierCommnad = new DelegateCommand(ResetForm);

            StateSelectionChanged = new DelegateCommand(LoadDistricts);

            _eventAggregator.GetEvent<SignalRStateAddedEvent>().Subscribe(StateAddedEventHandler);

            SupplierType = ApplicationSettings.SUPPLIER_TYPE.ToList();

        }

        private void StateAddedEventHandler(object obj)
        {
            if (obj is State _state)
            {

                if (States == null)
                {
                    States = new ObservableCollection<State>
                    {
                        _state
                    };

                    return;
                }

                var _existingState = States.FirstOrDefault(x => x.StateId == _state.StateId);

                if (_existingState != null)
                {
                    States.Remove(_existingState);
                }

                States.Add(_state);

                States = new ObservableCollection<State>(States.OrderBy(x => x.Name));
            }

        }

        private async void LoadStates()
        {

            var _result = await _commonService.GetStates("?isenabled = true");

            if (_result != null)
                States = new ObservableCollection<State>(_result);

        }

        private async void LoadDistricts()
        {
            Districts?.Clear();

            SelectedDistrict = null;

            if (SelectedState == null || SelectedState.StateId <= 0) return;

            try
            {


                var _result = await _commonService.GetDistricts(SelectedState.StateId);
                if (_result != null)
                    Districts = new ObservableCollection<District>(_result);

            }
            catch (Exception)
            {

            }
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
        [Required(ErrorMessage = "Select a State")]
        public State SelectedState
        {
            get { return _selectedState; }
            set
            {
                SetProperty(ref _selectedState, value);
                if (value != null)
                    ValidateProperty(value);

            }
        }

        private District _selectedDistrict;

        [Required(ErrorMessage = "Select a District")]
        public District SelectedDistrict
        {
            get { return _selectedDistrict; }
            set
            {
                SetProperty(ref _selectedDistrict, value);
                if (value != null)
                    ValidateProperty(value);
            }
        }


        public async void AddSupplierdetails()
        {
            bool _isValid = ValidateData();

            if (!_isValid) return;

            var _supplier = GetSupplier();


            await Task.Run(async () =>
            {

                var _result = await _supplierService.AddSuppliers(_supplier);

                if (_result.IsSuccess)
                {
                    _eventAggregator.GetEvent<ReloadSupplierEvent>().Publish();

                    Application.Current?.Dispatcher?.Invoke(() =>
                    {


                        ShowAlert("Supplier created ", NotificationType.Success);
                        PopupClose = false;
                        ResetForm();
                        if (AppConstants.ROLE_FINANCE == AppConstants.USER_ROLES[0].ToString())
                        {
                            //_supplier.SupplierId = Convert.ToInt32(_result.Data);
                            SuppliersDetailsViewModel createSupplierModel = new SuppliersDetailsViewModel()
                            {
                                SupplierId = Convert.ToInt32(_result.Data)
                            };
                            _eventAggregator.GetEvent<SupplierNewTabCreateEvent>().Publish(createSupplierModel);
                        }


                    });


                }


            });


        }

        private void ResetForm()
        {
            Districts?.Clear();
            Name = null;
            GstNumber = null;
            Phone = null;
            Email = null;
            Street = null;
            City = null;
            Pincode = null;
            SelectedDistrict = null;
            SelectedState = null;
            SelectedSuppliertype = null;
            ClearErrors();
            TallyCode = null;

        }

        private bool ValidateData()
        {

            if (!Name.IsValidString())
            {
                ShowAlert("Name is required", NotificationType.Error);
                return false;
            }
            else if (!GstNumber.IsValidString())
            {
                ShowAlert("GST number is required", NotificationType.Error);

                return false;
            }
            else if (!SelectedSuppliertype.IsValidString())
            {
                ShowAlert("supplier type is required", NotificationType.Error);

                return false;
            }
            else if (!Phone.IsValidString())
            {
                ShowAlert("Phone number is required", NotificationType.Error);

                return false;
            }
            else if (!Email.IsValidString())
            {
                ShowAlert("Email is required", NotificationType.Error);

                return false;
            }
            else if (!City.IsValidString())
            {
                ShowAlert("City is required", NotificationType.Error);

                return false;
            }
            else if (!Street.IsValidString())
            {
                ShowAlert("Street is required", NotificationType.Error);

                return false;
            }
            else if (SelectedState == null)
            {
                ShowAlert("Select a state", NotificationType.Error);

                return false;
            }
            else if (SelectedDistrict == null)
            {
                ShowAlert("Select a district", NotificationType.Error);

                return false;
            }
            else if (Pincode == null || Pincode?.ToString().Length != 6)
            {
                ShowAlert("Invalid Pin Code ", NotificationType.Error);
                return false;
            }
            else if (string.IsNullOrEmpty(TallyCode))
            {

                ShowAlert("Please Enter Tally Code", NotificationType.Error);
                return false;
            }
            return true;
        }

        private void ShowAlert(string msg, NotificationType notificationType) =>
            _eventAggregator.GetEvent<NotifyMessage>().Publish(new Common.Models.ToastMessage
            {
                Message = msg,
                MessageType = notificationType
            });


        private CreateSupplierModel GetSupplier()
        {
            var _supplier = new CreateSupplierModel();

            _supplier.Name = Name;
            _supplier.GstNumber = GstNumber;
            _supplier.Address = GetAddress();
            _supplier.Suppliertype = SelectedSuppliertype;
            _supplier.TallyCode = TallyCode;
            return _supplier;
        }

        private AddressModel GetAddress()
        {
            var _address = new AddressModel();
            _address.Alternatephone = Alternatephone;
            _address.Phone = Phone;
            _address.Pincode = Pincode;
            _address.City = City;
            _address.Email = Email;
            _address.Street = Street;
            _address.StateID = SelectedState.StateId;
            _address.DistrictID = SelectedDistrict.DistrictId;

            return _address;
        }


        #region Props                        

        private string _name;

        [Required(ErrorMessage = "Name is required")]
        public string Name
        {
            get { return _name; }
            set
            {
                SetProperty(ref _name, value);
                if (value != null)
                    ValidateProperty(value);
            }
        }

        private string _gstnumber;

        [Required(ErrorMessage = "GST number is required")]
        [StringLength(maximumLength: 15, ErrorMessage = "Invalid GST Number")]
        // [RegularExpression("^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$",ErrorMessage ="Invalid GST number",MatchTimeoutInMilliseconds =5000)]
        [RegularExpression(pattern: "^([0-9]{2}[a-zA-Z]{4}([a-zA-Z]{1}|[0-9]{1})[0-9]{4}[a-zA-Z]{1}([a-zA-Z]|[0-9]){3}){0,15}$", ErrorMessage = "Invalid GST number", MatchTimeoutInMilliseconds = 5000)]
        public string GstNumber
        {
            get { return _gstnumber; }
            set
            {
                SetProperty(ref _gstnumber, value);
                if (value != null) ValidateProperty(value);
            }
        }
        private string _phone;

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(maximumLength: 10, ErrorMessage = "Invalid Phone Number")]
        [RegularExpression(pattern: "[6-9][0-9]{9}", ErrorMessage = "Invalid Phone number", MatchTimeoutInMilliseconds = 5000)]

        public string Phone
        {
            get { return _phone; }
            set { SetProperty(ref _phone, value); if (value != null) ValidateProperty(value); }
        }
        private string _alternatephone;
        public string Alternatephone
        {
            get { return _alternatephone; }
            set { SetProperty(ref _alternatephone, value); }
        }
        private string _email;


        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); if (value != null) ValidateProperty(value); }
        }
        private string _street;

        public string Street
        {
            get { return _street; }
            set { SetProperty(ref _street, value); }
        }

        private int? _pincode;
        [Required(ErrorMessage = "Pin code is required")]

        [RegularExpression(pattern: "^[1-9][0-9]*$", ErrorMessage = "Invalid Pin Code", MatchTimeoutInMilliseconds = 5000)]
        public int? Pincode
        {
            get { return _pincode; }
            set
            {
                SetProperty(ref _pincode, value);
                if (value != null)
                {
                    ValidateProperty(value);
                }
            }
        }

        private string _city;

        [Required(ErrorMessage = "City is required")]
        public string City
        {
            get { return _city; }
            set { SetProperty(ref _city, value); if (value != null) ValidateProperty(value); }
        }
        //private string _state;
        //public string State
        //{
        //    get { return _state; }
        //    set { SetProperty(ref _state, value); }
        //}
        //private string _district;
        //public string District
        //{
        //    get { return _district; }
        //    set { SetProperty(ref _district, value); }
        //}


        private bool _popupClose;

        public bool PopupClose
        {
            get { return _popupClose; }
            set { SetProperty(ref _popupClose, value); }
        }


        private List<string> _suppliertype;
        public List<string> SupplierType
        {
            get { return _suppliertype; }
            set { SetProperty(ref _suppliertype, value); }
        }


        private string _selectedsuppliertype;

        [Required(ErrorMessage = "City is required")]
        public string SelectedSuppliertype
        {
            get { return _selectedsuppliertype; }
            set
            {
                SetProperty(ref _selectedsuppliertype, value);
                if (value != null) ValidateProperty(value);
            }
        }

        private string _tallyCode;

        public string TallyCode
        {
            get => _tallyCode;
            set => SetProperty(ref _tallyCode, value);
        }


        #endregion
    }
}
