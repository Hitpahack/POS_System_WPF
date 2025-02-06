using FalcaPOS.Common;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Common.Validations;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Location;
using FalcaPOS.Entites.Suppliers;
using MahApps.Metro.Controls;
using Prism.Commands;
using Prism.Events;
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
    public class SupplierEditFlyoutViewModel : ValidationBase
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly Logger _logger;

        private readonly ISupplierService _supplierService;

        private CancellationTokenSource _cancellationTokenSource;

        private readonly ICommonService _commonService;

        private readonly INotificationService _notificationService;

        private readonly ProgressService _ProgressService;


        public DelegateCommand UpdateSupplierCommand { get; private set; }

        // public event RoutedEventHandler ClosingFinished;
        public DelegateCommand StateSelectionChanged { get; private set; }

        public DelegateCommand CancelSupplierCommnad { get; private set; }


        public SupplierEditFlyoutViewModel(IEventAggregator eventAggregator,
            Logger logger,
            ISupplierService supplierService,
            ICommonService commonService,
            INotificationService notificationService,
            ProgressService ProgressService)
        {
            Width = 400;

            Height = GridLength.Auto;

            Position = Position.Left;

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));

            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _ProgressService = ProgressService ?? throw new ArgumentNullException(nameof(ProductCardViewModel));

            _eventAggregator.GetEvent<EditSupplerEvent>().Subscribe(supplier => EditSuplier(supplier));

            UpdateSupplierCommand = new DelegateCommand(UpdateSupplier).ObservesCanExecute(() => IsValid);

            StateSelectionChanged = new DelegateCommand(LoadDistricts);

            CancelSupplierCommnad = new DelegateCommand(CancelSupplier);

            LoadStatesAsync();


            SupplierType = ApplicationSettings.SUPPLIER_TYPE.ToList();
        }


        private void CancelSupplier()
        {
            IsOpen = false;
        }

        private async void LoadStatesAsync()
        {
            await Task.Run(async () =>
            {
                var _result = await _commonService.GetStates("?isenabled = true");

                if (_result != null)
                {
                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        States = new ObservableCollection<State>(_result);
                    });
                }

            });



        }

        private bool _isLoading;
        public bool Isloading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        private async void LoadDistricts()
        {
            Districts?.Clear();

            SelectedDistrict = null;

            if (SelectedState == null || SelectedState.StateId <= 0) return;

            try
            {
                await Task.Run(async () =>
                {

                    var _result = await _commonService.GetDistricts(SelectedState.StateId);

                    if (_result != null && _result.Any())
                    {
                        Application.Current.Dispatcher?.Invoke(() =>
                        {
                            Districts = new ObservableCollection<District>(_result);

                            if (DistrictId > 0)
                            {
                                SelectedDistrict = Districts.Where(x => x.DistrictId == DistrictId).FirstOrDefault();

                                DistrictId = 0;
                            }
                        });

                    }
                });




            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in stock  view model", _ex);
            }
        }

        private async void UpdateSupplier()
        {
            try
            {
                bool _isValid = ValidateSupplierInfo();

                if (!_isValid) return;

                CreateSupplierModel _supplier = GetSupplierInfo();

                if (_supplier != null && SupplierId > 0)
                {
                    await _ProgressService.StartProgressAsync();


                    await Task.Run(async () =>
                   {

                       var _result = await _supplierService.UpadateSupplier(SupplierId, _supplier);

                       if (_result != null)
                       {
                           await Application.Current?.Dispatcher?.InvokeAsync(async () =>
                           {


                               await _ProgressService.StopProgressAsync();

                               if (_result.IsSuccess)
                               {
                                   IsOpen = false;

                                   _notificationService.ShowMessage("Supplier Updated .", Common.NotificationType.Success);

                                   _eventAggregator.GetEvent<ReloadSupplierEvent>().Publish();
                               }
                               else
                               {
                                   _notificationService.ShowMessage(_result.Error ?? "An error occurred,try again", Common.NotificationType.Error);
                               }
                           });
                       }
                   });
                }
            }
            catch (Exception _ex)
            {

                _logger.LogError("Error in update supplier", _ex);

                await _ProgressService.StopProgressAsync();
            }

        }

        private CreateSupplierModel GetSupplierInfo()
        {
            var _supplier = new CreateSupplierModel();

            _supplier.Name = Name;
            _supplier.GstNumber = GstNumber;
            _supplier.Address = GetSupplierAddress();
            _supplier.Suppliertype = SelectedSuppliertype;
            _supplier.TallyCode = TallyCode;
            return _supplier;
        }

        private AddressModel GetSupplierAddress()
        {
            var _address = new AddressModel();
            _address.Alternatephone = Alternatephone;
            _address.City = City;
            _address.Phone = Phone;
            _address.Email = Email;
            _address.Pincode = Pincode;
            _address.Street = Street;
            if (SelectedState != null)
                _address.StateID = SelectedState.StateId;
            if (SelectedDistrict != null)
                _address.DistrictID = SelectedDistrict.DistrictId;


            return _address;
        }

        private bool ValidateSupplierInfo()
        {

            if (!Name.IsValidString())
            {
                _notificationService.ShowMessage("Name is required", NotificationType.Error);
                return false;
            }
            else if (!GstNumber.IsValidString())
            {
                _notificationService.ShowMessage("GST number is required", NotificationType.Error);

                return false;
            }
            else if (!SelectedSuppliertype.IsValidString())
            {
                _notificationService.ShowMessage("supplier type is required", NotificationType.Error);

                return false;
            }
            else if (!Phone.IsValidString())
            {
                _notificationService.ShowMessage("Phone number is required", NotificationType.Error);

                return false;
            }
            else if (!Email.IsValidString())
            {
                _notificationService.ShowMessage("Email is required", NotificationType.Error);

                return false;
            }
            else if (!TallyCode.IsValidString())
            {
                _notificationService.ShowMessage("TallyCode is required", NotificationType.Error);

                return false;
            }
            else if (!City.IsValidString())
            {
                _notificationService.ShowMessage("City is required", NotificationType.Error);

                return false;
            }
            else if (!Street.IsValidString())
            {
                _notificationService.ShowMessage("Street is required", NotificationType.Error);

                return false;
            }
            else if (SelectedState == null)
            {
                _notificationService.ShowMessage("Select a state", NotificationType.Error);

                return false;
            }
            else if (SelectedDistrict == null)
            {
                _notificationService.ShowMessage("Select a district", NotificationType.Error);

                return false;
            }
            else if (Pincode == null || Pincode?.ToString().Length != 6)
            {
                _notificationService.ShowMessage("Invalid Pin Code ", NotificationType.Error);
                return false;
            }
            return true;
        }

        private async void EditSuplier(SuppliersViewModel supplier)
        {
            ClearValues();

            if (supplier != null)
            {
                Isloading = true;

                if (_cancellationTokenSource != null)
                    _cancellationTokenSource.Cancel();

                _cancellationTokenSource = new CancellationTokenSource();
                Header = $"Edit supplier {supplier.Name}";
                IsOpen = true;

                try
                {
                    _logger.LogInformation($"Getting supplier edit details {supplier.Name}");

                    await Task.Run(async () =>
                    {
                        var _result = await _supplierService
                        .GetSupplierDetails((int)supplier.SupplierId, _cancellationTokenSource.Token);

                        if (_result != null && _result.IsSuccess)
                        {
                            Application.Current?.Dispatcher.Invoke(() =>
                            {
                                BindValues(_result.Data);
                            });
                        }
                        else
                        {
                            if (_result != null && !_result.IsSuccess)
                            {
                                IsOpen = false;

                                _notificationService.ShowMessage(_result.Error.IsValidString() ? _result.Error : "An error occurred , try again", Common.NotificationType.Error);
                                return;
                            }
                        }

                    }, _cancellationTokenSource.Token);

                    Isloading = false;
                }
                catch (OperationCanceledException _ex)
                {
                    _logger.LogError(_ex.Message);
                }
                catch (Exception _ex)
                {
                    _logger.LogError("Error in edit supplier vm", _ex);

                    IsOpen = false;
                    _notificationService.ShowMessage("An error occurred , try again", Common.NotificationType.Error);
                }
            }
        }

        private void ClearValues()
        {
            Districts?.Clear();
            Name = null;
            GstNumber = null;
            Phone = null;
            Email = null;
            Street = null;
            City = null;
            Pincode = null;
            DistrictId = 0;
            SelectedDistrict = null;
            SelectedState = null;
            ClearErrors();
            SelectedSuppliertype = null;
            TallyCode = null;

        }

        private void BindValues(SuppliersDetailsViewModel supplier)
        {
            Name = supplier.Name;
            GstNumber = supplier.GstNumber;
            SupplierId = supplier.SupplierId;
            Phone = supplier.Address.Phone;
            Alternatephone = supplier.Address.Alternatephone;
            Email = supplier.Address.Email;
            Street = supplier.Address.Street;
            Pincode = supplier.Address.Pincode;
            SelectedState = States.Where(x => x.StateId == supplier.Address.StateID).FirstOrDefault();
            DistrictId = supplier.Address.DistrictID;
            City = supplier.Address.City;
            SelectedSuppliertype = supplier.Suppliertype;
            TallyCode = supplier.TallyCode;
        }

        public int SupplierId { get; set; }

        public int DistrictId { get; set; }

        private string _header;
        public string Header
        {
            get { return _header; }
            set { SetProperty(ref _header, value); }
        }

        private bool _isModal;
        public bool IsModal
        {
            get { return _isModal; }
            set { SetProperty(ref _isModal, value); }
        }

        private bool _isOpen;
        public bool IsOpen
        {
            get { return _isOpen; }
            set { SetProperty(ref _isOpen, value); }
        }
        private Position position;
        public Position Position
        {
            get { return this.position; }
            set { SetProperty(ref this.position, value); }

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
                // if (value != null)
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
                // if (value != null)
                ValidateProperty(value);
            }
        }


        private string _name;

        [Required(ErrorMessage = "Name is required")]
        public string Name
        {
            get { return _name; }
            set
            {
                SetProperty(ref _name, value);
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
                ValidateProperty(value);
            }
        }
        private string _phone;

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(maximumLength: 10, ErrorMessage = "Invalid Phone Number")]
        [RegularExpression(pattern: "[6-9][0-9]{9}", ErrorMessage = "Invalid Phone number", MatchTimeoutInMilliseconds = 5000)]

        public string Phone
        {
            get { return _phone; }
            set { SetProperty(ref _phone, value); ValidateProperty(value); }
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
            set { SetProperty(ref _email, value); ValidateProperty(value); }
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

                ValidateProperty(value);

            }
        }
        private string _city;

        [Required(ErrorMessage = "City is required")]
        public string City
        {
            get { return _city; }
            set { SetProperty(ref _city, value); ValidateProperty(value); }
        }


        private List<string> _suppliertype;
        public List<string> SupplierType { get { return _suppliertype; } set { SetProperty(ref _suppliertype, value); } }


        private string _selectedsuppliertype;

        [Required(ErrorMessage = "City is required")]
        public string SelectedSuppliertype
        {
            get { return _selectedsuppliertype; }
            set { SetProperty(ref _selectedsuppliertype, value); if (value != null) ValidateProperty(value); }
        }

        private string _tallyCode;

        public string TallyCode
        {
            get => _tallyCode;
            set => SetProperty(ref _tallyCode, value);
        }


    }
}
