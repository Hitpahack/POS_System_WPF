using FalcaPOS.Common;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Common.Validations;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Location;
using FalcaPOS.Entites.Suppliers;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace FalcaPOS.Suppliers.ViewModels
{
    public class AddAddressViewModel : ValidationBase
    {

        private readonly IEventAggregator _eventAggregator;

        private readonly ISupplierService _supplierService;

        private readonly ICommonService _commonService;

        public DelegateCommand AddAddressCommnad { get; private set; }

        public DelegateCommand ResetAddressCommnad { get; private set; }

        public DelegateCommand StateSelectionChanged { get; private set; }

        private readonly Logger _logger;


        public AddAddressViewModel(IEventAggregator EventAggregator, ISupplierService SupplierService, ICommonService CommonService, Logger Logger)
        {
            _eventAggregator = EventAggregator ?? throw new ArgumentNullException(nameof(EventAggregator));

            _supplierService = SupplierService ?? throw new ArgumentNullException(nameof(SupplierService));

            _commonService = CommonService ?? throw new ArgumentNullException(nameof(CommonService));

            _logger = Logger ?? throw new ArgumentNullException(nameof(Logger));

            AddAddressCommnad = new DelegateCommand(AddAdressdetails)
                                  .ObservesCanExecute(() => IsValid);

            LoadStates();

            ResetAddressCommnad = new DelegateCommand(ResetForm);

            StateSelectionChanged = new DelegateCommand(LoadDistricts);

            _ = _eventAggregator.GetEvent<ShippingAddressEvent>().Subscribe(ShippingAddressclose);

        }

        public void ShippingAddressclose(bool obj)
        {
            PopupClose = false;
            ResetForm();
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
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public void AddAdressdetails()
        {
            bool _isValid = ValidateData();

            if (!_isValid) return;

            var _supplier = GetSupplier();


            _eventAggregator.GetEvent<SupplierIDCreateEvent>().Publish(_supplier);


        }

        private ShippingAddress GetSupplier()
        {

            var supplierModel = new ShippingAddress();

            supplierModel.Phone = Phone;
            supplierModel.Email = Email;
            supplierModel.StateID = SelectedState.StateId;
            supplierModel.DistrictID = SelectedDistrict.DistrictId;
            supplierModel.City = City;
            supplierModel.Pincode = Pincode;
            supplierModel.State = SelectedState.Name;
            supplierModel.District = SelectedDistrict.Name;

            return supplierModel;
        }

        private async void LoadStates()
        {

            var _result = await _commonService.GetStates("?isenabled = true");

            if (_result != null)
                States = new ObservableCollection<State>(_result);

        }

        private void ResetForm()
        {
            Districts?.Clear();
            City = null;
            Pincode = null;
            SelectedDistrict = null;
            SelectedState = null;
            Phone = null;
            Email = null;
            ClearErrors();

        }


        private AddressViewModel GetAddress()
        {
            var addressModel = new AddressViewModel();

            addressModel.Pincode = Pincode;
            addressModel.State = SelectedState.Name;
            addressModel.City = City;
            addressModel.District = SelectedDistrict.Name;
            addressModel.StateID = SelectedState.StateId;
            addressModel.DistrictID = SelectedDistrict.DistrictId;
            addressModel.Phone = String.Empty;

            return addressModel;
        }

        private bool ValidateData()
        {

            if (!Phone.IsValidPhone())
            {
                ShowAlert("Phone number is required", NotificationType.Error);

                return false;
            }
            if (!Email.IsValidEmail())
            {
                ShowAlert("Email is required", NotificationType.Error);

                return false;
            }

            if (!City.IsValidString())
            {
                ShowAlert("City is required", NotificationType.Error);

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
            return true;
        }

        private void ShowAlert(string msg, NotificationType notificationType) =>
          _eventAggregator.GetEvent<NotifyMessage>().Publish(new Common.Models.ToastMessage
          {
              Message = msg,
              MessageType = notificationType
          });


        private int? pinCode;
        [Required(ErrorMessage = "Pin code is required")]

        [RegularExpression(pattern: "^[1-9][0-9]*$", ErrorMessage = "Invalid Pin Code", MatchTimeoutInMilliseconds = 5000)]
        public int? Pincode
        {
            get { return pinCode; }
            set
            {
                SetProperty(ref pinCode, value);
                if (value != null)
                {
                    ValidateProperty(value);
                }
            }
        }

        private string city;

        [Required(ErrorMessage = "City is required")]
        public string City
        {
            get { return city; }
            set { SetProperty(ref city, value); if (value != null) ValidateProperty(value); }
        }

        private ObservableCollection<State> states;
        public ObservableCollection<State> States
        {
            get { return states; }
            set { SetProperty(ref states, value); }
        }
        private ObservableCollection<District> districts;
        public ObservableCollection<District> Districts
        {
            get { return districts; }
            set { SetProperty(ref districts, value); }
        }


        private State selectedState;
        [Required(ErrorMessage = "Select a State")]
        public State SelectedState
        {
            get { return selectedState; }
            set
            {
                SetProperty(ref selectedState, value);
                if (value != null)
                    ValidateProperty(value);

            }
        }

        private District selectedDistrict;

        [Required(ErrorMessage = "Select a District")]
        public District SelectedDistrict
        {
            get { return selectedDistrict; }
            set
            {
                SetProperty(ref selectedDistrict, value);
                if (value != null)
                    ValidateProperty(value);
            }
        }

        private bool popupClose;

        public bool PopupClose
        {
            get { return popupClose; }
            set { SetProperty(ref popupClose, value); }
        }

        private Supplier supplierDetails;
        public Supplier SuppliersDetails { get { return supplierDetails; } set { SetProperty(ref supplierDetails, value); } }

        private string phone;
        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(maximumLength: 10, ErrorMessage = "Invalid Phone Number")]
        [RegularExpression(pattern: "[6-9][0-9]{9}", ErrorMessage = "Invalid Phone number", MatchTimeoutInMilliseconds = 5000)]

        public string Phone { get { return phone; } set { SetProperty(ref phone, value); if (value != null) ValidateProperty(value); } }

        private string email;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get { return email; } set { SetProperty(ref email, value); if (value != null) ValidateProperty(value); } }

    }
}
