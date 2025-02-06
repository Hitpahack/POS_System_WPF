using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Common.Models;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Customers;
using FalcaPOS.Entites.Stores;
using FalcaPOS.Entites.User;
using MahApps.Metro.Controls;
using Prism.Commands;
using Prism.Events;
using Prism.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;

namespace FalcaPOS.Team.ViewModels
{
    public class EditUserFlyOutViewModel : ValidatableBindableBase
    {

        #region Fields
        private readonly IEventAggregator _eventAggregator;
        private readonly ProgressService _ProgressService;
        private readonly ITeamService _teamService;
        private readonly IStoreService _storeService;
        #endregion        

        public DelegateCommand UpdateUserCommand { get; private set; }

        public DelegateCommand CancelCommand { get; private set; }

        private readonly Logger _logger;

        public DelegateCommand<object> StoreComboSelectionChangedCmdEdit { get; private set; }


        #region Constructor
        public EditUserFlyOutViewModel(IEventAggregator EventAggregator, ProgressService ProgressService, ITeamService TeamService, IStoreService StoreService, Logger Logger)
        {
            _eventAggregator = EventAggregator;

            _ProgressService = ProgressService;

            _teamService = TeamService;

            _storeService = StoreService;

            _eventAggregator.GetEvent<EditUserFlyoutOpenEvent>().Subscribe(EditUser);

            UpdateUserCommand = new DelegateCommand(UpdateUser).ObservesCanExecute(() => HasAnyErrors);

            CancelCommand = new DelegateCommand(CloseFlyout);

            PropertyChanged += EditUserFlyOutViewModel_PropertyChanged;

            _logger = Logger;

            StoreComboSelectionChangedCmdEdit = new DelegateCommand<object>(SelectionStoreChange);


        }
        #endregion


        #region Methods

        private void CloseFlyout()
        {
            Clear();
        }

        private void Clear()
        {
            try
            {

                //Name = null;
                //UserName = null;
                //City = null;
                //Roles = null;
                //Stores = null;
                //SelectedStore = null;
                //Email = null;
                //Phone = null;
                //UserId = 0;
                //ChangePassword = false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            IsOpen = false;
        }

        private async void UpdateUser()
        {

            bool _isValid = ValidateData();

            if (!_isValid)
            {
                return;
            }

            var _user = GetUser();

            if (_user == null) return;


            await _ProgressService.StartProgressAsync();

            var _result = await _teamService.UpdateUser(_user.UserId, _user);

            if (_result != null && _result.IsSuccess)
            {
                ShowAlert("User updated", NotificationType.Success);

                Clear();

                _eventAggregator.GetEvent<ReloadUsersEvent>().Publish();
            }
            else
            {
                ShowAlert(_result.Error ?? "An error occurred,try again", NotificationType.Error);
            }

            await _ProgressService.StopProgressAsync();

        }

        private UpdateUserModel GetUser()
        {
            try
            {
                var _user = new UpdateUserModel();

                _user.Address = GetAddress();
                _user.UserName = UserName;
                _user.Name = Name;
                _user.UserId = UserId;
                _user.Roles = Roles.Where(x => x.IsSelected).Select(x => x.Name).ToArray();
                _user.StoreId = SelectedStore != null ? selectedStore.StoreId : -1;

                if (ChangePassword)
                {
                    if (Password.Equals(ComparePassword))
                    {
                        _user.Password = Password;
                        _user.Changepassword = ChangePassword;
                    }
                }

                return _user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return default(UpdateUserModel);
        }

        private Address GetAddress()
        {
            try
            {
                var _address = new Address();

                _address.City = City;
                _address.Phone = Phone;
                _address.Email = Email;
                return _address;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return default(Address);
        }

        private bool ValidateData()
        {
            bool _isValid = true;

            if (string.IsNullOrWhiteSpace(Name))
            {
                ShowAlert("Name is required.", NotificationType.Error);

                return _isValid = false;

            }
            else if (string.IsNullOrWhiteSpace(UserName))
            {
                ShowAlert("Username is required.", NotificationType.Error);
                return _isValid = false;
            }
            else if (!Phone.IsValidPhone())
            {
                ShowAlert("Phone number is Invalid.", NotificationType.Error);
                return _isValid = false;
            }
            else if (string.IsNullOrWhiteSpace(Email))
            {
                ShowAlert("Email is required.", NotificationType.Error);
                return _isValid = false;
            }
            else if (string.IsNullOrWhiteSpace(City))
            {
                ShowAlert("City is required.", NotificationType.Error);
                return _isValid = false;
            }
            else if (SelectedStore == null)
            {
                ShowAlert("Select a store.", NotificationType.Error);
                return _isValid = false;
            }
            else if (!Roles.Any(x => x.IsSelected))
            {
                ShowAlert("Select role.", NotificationType.Error);
                return _isValid = false;
            }

            if (ChangePassword)
            {
                if (string.IsNullOrWhiteSpace(Password))
                {
                    ShowAlert("Password is required.", NotificationType.Error);
                    return _isValid = false;
                }
                else if (string.IsNullOrWhiteSpace(ComparePassword) || !Password.Equals(ComparePassword))
                {
                    ShowAlert("Password and Confirm password do not match", NotificationType.Error);
                    return _isValid = false;
                }
            }

            return _isValid;
        }

        private void EditUserFlyOutViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            HasAnyErrors = HasErrors;

        }


        private async void EditUser(object obj)
        {
            Width = 350;
            Height = GridLength.Auto;
            Position = Position.Right;
            var Isopen = Convert.ToString(obj);
            IsOpen = Isopen == "False" ? false : true;
            if (IsOpen)
            {
                var _user = obj as User;

                if (obj == null)
                {
                    ShowAlert("Invalid User", NotificationType.Error);
                    return;
                }

                try
                {

                    var _stores = await _storeService.GetStores("isenabled=true");
                    if (_stores != null)
                    {
                        List<Store> storesList = new List<Store>();

                        foreach (var store in _stores)
                        {
                            if (store.IsParent == true && store.Parent_ref == null)
                                storesList.Add(store);
                            else
                            {
                                Store childstore = new Store();
                                childstore.StoreId = store.StoreId;
                                childstore.Address = store.Address;
                                childstore.Parent_ref = store.Parent_ref;
                                childstore.LastSequenceNumber = store.LastSequenceNumber;
                                childstore.StoreInvoiceFormat = store.StoreInvoiceFormat;
                                childstore.Name = _stores.Where(x => x.StoreId == store.Parent_ref).FirstOrDefault()?.Name + "-->" + store.Name;
                                storesList.Add(childstore);

                            }
                        }

                        var addAllList = storesList.ToList();
                        addAllList.Insert(0, new Store() { StoreId = 1, Name = "ALL STORE" });
                        storesList = addAllList;

                        Stores = new ObservableCollection<Store>(storesList);
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
                Header = $"Edit User {_user.Name}";
                BindValues(_user);

            }


        }

        private ObservableCollection<Role> GetRoles()
        {
            return new ObservableCollection<Role>
            {
                new Role
                {
                    Name="admin",
                    Description="Admin",
                    AutoMationId="editAdminRoleId"
                },
                new Role
                {
                    Name="storeperson",
                    Description="Store Person",
                    AutoMationId="editStorePersonRoleId"
                },
                new Role
                {
                    Name="superbackend",
                    Description="Super Backend",
                    AutoMationId="editSuperBackendRoleId"
                },
                new Role
                {
                    Name="backend",
                    Description="Backend",
                    AutoMationId="editBackendRoleId"
                },
                new Role
                {
                    Name="finance",
                    Description="Finance",
                    AutoMationId="editFinanceRoleId"
                }, new Role
                {
                    Name="falcadirector",
                    Description="Falca Director",
                    AutoMationId="addfalcadirectorRoleId"
                },
                  new Role
                {
                    Name="purchasemanager",
                    Description="Purcahse Manager",
                    AutoMationId="addPurchaseManagerRoleId"
                },
                  new Role
                {
                    Name="auditor",
                    Description="Auditor",
                    AutoMationId="addAuditorRoleId"
                },
                  new Role
                {
                    Name="territorymanager",
                    Description="Territory Manager",
                    AutoMationId="addFalcaFactoryManagerId"
                },
                  new Role
                {
                    Name="regionalmanager",
                    Description="Regional Manager",
                    AutoMationId="addRegionalManager"
                },
                   new Role
                {
                    Name="controlmanager",
                    Description="Control Manager",
                    AutoMationId="addControlRoleId"
                }
            };
        }

        private void BindValues(User user)
        {

            Name = user.Name;
            UserName = user.UserName;
            Phone = user.Address?.Phone ?? string.Empty;
            City = user.Address?.City ?? string.Empty;
            Email = user.Address?.Email ?? string.Empty;
            UserId = user.UserId;
            ChangePassword = false;
            if (Stores != null && Stores.Any())
            {
                SelectedStore = Stores.FirstOrDefault(x => x.StoreId == user.Store?.StoreId);
            }
            Roles = GetRoles();

            if (user.Roles != null && user.Roles.Any())
            {
                foreach (var item in Roles)
                {
                    if (user.Roles.Contains(item.Name))
                    {
                        item.IsSelected = true;
                    }
                }

            }

        }

        void ShowAlert(string msg, NotificationType type) => _eventAggregator.GetEvent<NotifyMessage>().Publish(new ToastMessage
        {
            Message = msg,
            MessageType = type
        });
        #endregion


        #region Props

        private ObservableCollection<Store> _stores;
        public ObservableCollection<Store> Stores
        {
            get { return _stores; }
            set { SetProperty(ref _stores, value); }
        }

        private ObservableCollection<Role> _roles;


        public ObservableCollection<Role> Roles
        {
            get { return _roles; }
            set { SetProperty(ref _roles, value); }
        }




        private bool _isOpen;
        public bool IsOpen
        {
            get { return _isOpen; }
            set { SetProperty(ref _isOpen, value); }
        }

        private int width;
        public int Width
        {
            get { return width; }
            set { SetProperty(ref width, value); }
        }

        private GridLength height;
        public GridLength Height
        {
            get { return height; }
            set { SetProperty(ref height, value); }
        }

        private Position position;
        public Position Position
        {
            get { return position; }
            set { SetProperty(ref position, value); }
        }

        private string header;
        public string Header
        {
            get { return header; }
            set { SetProperty(ref header, value); }
        }

        private string name;

        [Required(ErrorMessage = "Name is required")]
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private string userName = string.Empty;

        [Required(ErrorMessage = "Username is required")]
        public string UserName
        {
            get { return userName; }
            set { SetProperty(ref userName, value); }
        }



        private bool changePassword;
        public bool ChangePassword
        {
            get { return changePassword; }
            set
            {
                SetProperty(ref changePassword, value);
                Password = null;
            }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                SetProperty(ref password, value);
                ComparePassword = null;
            }
        }

        private string comparePassword;
        public string ComparePassword
        {
            get { return comparePassword; }
            set { SetProperty(ref comparePassword, value); }
        }


        public int UserId { get; set; }

        private string phone;

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(maximumLength: 10, ErrorMessage = "Invalid Phone Number")]
        [RegularExpression(pattern: "[6-9][0-9]{9}", ErrorMessage = "Invalid Phone number", MatchTimeoutInMilliseconds = 5000)]

        public string Phone
        {
            get { return phone; }
            set { SetProperty(ref phone, value); }
        }

        private string email;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email
        {
            get { return email; }
            set { SetProperty(ref email, value); }
        }
        private Store selectedStore;
        [Required(ErrorMessage = "Select a Store")]
        public Store SelectedStore
        {
            get { return selectedStore; }
            set { SetProperty(ref selectedStore, value); }
        }

        private string city;
        [Required(ErrorMessage = "City is required")]
        public string City
        {
            get { return city; }
            set { SetProperty(ref city, value); }
        }

        private bool hasAnyErrors;
        public bool HasAnyErrors
        {
            get { return !HasErrors; }
            set { SetProperty(ref hasAnyErrors, value); }
        }


        #endregion



        public void SelectionStoreChange(object obj)
        {
            try
            {
                var _storeEditView = (Store)obj;
                if (_storeEditView != null)
                {
                    if (_storeEditView.StoreId == 1)
                    {
                        foreach (var item in Roles)
                        {
                            if ((item.Name == AppConstants.ROLE_FINANCE) || (item.Name == AppConstants.ROLE_ADMIN) || (item.Name == AppConstants.ROLE_DIRECTOR) || (item.Name == AppConstants.ROLE_PURCHASE_MANAGER) || (item.Name == AppConstants.ROLE_AUDITOR) || (item.Name == AppConstants.ROLE_TERRITORY_MANAGER) || (item.Name == AppConstants.ROLE_REGIONAL_MANAGER) || item.Name == AppConstants.ROLE_CONTROL_MANAGER)
                                item.IsEnableRole = true;
                            else
                                item.IsEnableRole = false;
                        }
                    }
                    if (_storeEditView.StoreId != 1 && _storeEditView.Parent_ref != null)
                    {
                        foreach (var item in Roles)
                        {
                            if ((item.Name == AppConstants.ROLE_FINANCE) || (item.Name == AppConstants.ROLE_ADMIN) || (item.Name == AppConstants.ROLE_DIRECTOR) || (item.Name == AppConstants.ROLE_PURCHASE_MANAGER) || (item.Name == AppConstants.ROLE_SUPER_BACKEND) || (item.Name == AppConstants.ROLE_BACKEND) || item.Name == AppConstants.ROLE_CONTROL_MANAGER)
                                item.IsEnableRole = false;
                            else
                                item.IsEnableRole = true;
                        }
                    }
                    if (_storeEditView.StoreId != 1 && _storeEditView.Parent_ref == null)
                    {
                        foreach (var item in Roles)
                        {
                            if ((item.Name == AppConstants.ROLE_FINANCE) || (item.Name == AppConstants.ROLE_ADMIN) || (item.Name == AppConstants.ROLE_DIRECTOR) || (item.Name == AppConstants.ROLE_PURCHASE_MANAGER) || (item.Name == AppConstants.ROLE_AUDITOR) || (item.Name == AppConstants.ROLE_TERRITORY_MANAGER) || (item.Name == AppConstants.ROLE_REGIONAL_MANAGER) || item.Name == AppConstants.ROLE_CONTROL_MANAGER)
                                item.IsEnableRole = false;
                            else
                                item.IsEnableRole = true;
                        }
                    }
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }
    }
}
