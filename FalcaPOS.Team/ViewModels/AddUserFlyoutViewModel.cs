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
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.Team.ViewModels
{
    public class AddUserFlyoutViewModel : BindableBase //ValidationBase
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly ITeamService _teamService;

        private readonly IStoreService _storeService;

        public DelegateCommand CreateUserCommand { get; private set; }

        public DelegateCommand CancelCommand { get; private set; }

        private readonly ProgressService _ProgressService;

        private readonly Logger _logger;

        public DelegateCommand<object> StoreComboSelectionChangedCmd { get; private set; }

        public AddUserFlyoutViewModel(IEventAggregator EventAggregator, ProgressService ProgressService, ITeamService TeamService, IStoreService StoreService, Logger Logger)
        {
            _ProgressService = ProgressService ?? throw new ArgumentNullException(nameof(ProgressService));

            _eventAggregator = EventAggregator ?? throw new ArgumentNullException(nameof(EventAggregator));

            _teamService = TeamService ?? throw new ArgumentNullException(nameof(TeamService));

            _storeService = StoreService ?? throw new ArgumentNullException(nameof(StoreService));

            _eventAggregator.GetEvent<AddUserFlyoutOpenEvent>().Subscribe(AddUserFlyoutEvent);

            CreateUserCommand = new DelegateCommand(CreateUserAsync);

            CancelCommand = new DelegateCommand(CloseFlyout);

            _logger = Logger ?? throw new ArgumentNullException(nameof(Logger));

            StoreComboSelectionChangedCmd = new DelegateCommand<object>(SelectionStoreChange);

        }

        private void ShowAlert(string msg, NotificationType msgType)
        {

            _eventAggregator.GetEvent<NotifyMessage>().Publish(
                new ToastMessage
                {
                    Message = msg,
                    MessageType = msgType
                });
        }

        private void CloseFlyout()
        {
            ResetUser();
        }


        private ObservableCollection<Role> GetRoles()
        {
            return new ObservableCollection<Role>
            {
                new Role
                {
                    Name="admin",
                    Description="Admin",
                    AutoMationId="addAdminRoleId"
                },
                new Role
                {
                    Name="storeperson",
                    Description="Store Person",
                    AutoMationId="addStorePersonRoleId"
                },
                new Role
                {
                    Name="superbackend",
                    Description="Super Backend",
                    AutoMationId="addSuperBackendRoleId"
                },
                new Role
                {
                    Name="backend",
                    Description="Backend",
                    AutoMationId="addBackendRoleId"
                },
                new Role
                {
                    Name="finance",
                    Description="Finance",
                    AutoMationId="addFinanceRoleId"
                },
                  new Role
                {
                    Name="falcadirector",
                    Description="Falca Director",
                    AutoMationId="addfalcadirectorRoleId"
                },
                  new Role
                {
                    Name="purchasemanager",
                    Description="Purchase Manager",
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
                    AutoMationId="addTerritoryManagerId"
                },
                  new Role
                {
                    Name="regionalmanager",
                    Description="Regional Manager",
                    AutoMationId="addRegionalRoleId"
                },
                 new Role
                {
                    Name="controlmanager",
                    Description="Control Manager",
                    AutoMationId="addControlRoleId"
                }

            };
        }

        private ObservableCollection<Role> _roles;


        public ObservableCollection<Role> Roles
        {
            get { return _roles; }
            set { SetProperty(ref _roles, value); }
        }




        private async void CreateUserAsync()
        {

            bool isValidData = ValidateData();

            if (!isValidData) return;

            User _user = GetUser();

            try
            {

                await _ProgressService.StartProgressAsync();


                await Task.Run(async () =>
                {
                    var _result = await _teamService.CreateUser(_user);

                    if (_result != null && _result.IsSuccess)
                    {
                        ShowAlert("User created", NotificationType.Success);

                        _eventAggregator.GetEvent<ReloadUsersEvent>().Publish();

                        ResetUser();
                    }
                    else
                    {
                        if (_result != null && !_result.IsSuccess)
                        {
                            ShowAlert(_result.Error, NotificationType.Error);
                        }
                        else
                        {
                            ShowAlert("Unknown error", NotificationType.Error);
                        }

                    }

                });
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }

            await _ProgressService.StopProgressAsync();
        }

        private void ResetUser()
        {
            try
            {
                //ClearErrors();
                Name = null;
                UserName = null;
                Password = null;
                City = null;
                Roles = null;
                Stores = null;
                SelectedStore = null;
                Email = null;
                Phone = null;
            }
            catch (Exception)
            {
            }
            IsOpen = false;

        }

        private User GetUser()
        {
            var _user = new User
            {
                Name = Name,
                UserName = UserName,
                Password = Password,
                IsAlive = true,
                Roles = Roles.Where(x => x.IsSelected).Select(x => x.Name).ToArray(),
            };


            if (SelectedStore != null)
            {
                _user.StoreId = SelectedStore.StoreId;
            }

            _user.Address = GetUserAddres();


            return _user;

        }

        private Address GetUserAddres()
        {
            return new Address
            {
                Phone = Phone,
                Email = Email,
                City = City,
            };
        }

        private bool ValidateData()
        {


            if (!Name.IsValidString())
            {
                ShowAlert("Name is required.", NotificationType.Error);

                return false;

            }
            if (!UserName.IsValidString())
            {
                ShowAlert("Username is required.", NotificationType.Error);
                return false;
            }

            if (!Password.IsValidString())
            {
                ShowAlert("Password is required.", NotificationType.Error);

                return false;
            }
            if (!ComparePassword.IsValidString())
            {
                ShowAlert("Confirm Password is required.", NotificationType.Error);

                return false;
            }
            if (!Password.Equals(ComparePassword))
            {
                ShowAlert("Password and Confirm Password do not match.", NotificationType.Error);

                return false;
            }
            if (!Phone.IsValidString())
            {
                ShowAlert("Phone number is required.", NotificationType.Error);

                return false;
            }
            if (!Phone.IsValidPhone())
            {
                ShowAlert("Phone number is invalid.", NotificationType.Error);

                return false;
            }

            if (!Email.IsValidString())
            {
                ShowAlert("Email is required.", NotificationType.Error);

                return false;
            }

            if (!Email.IsValidEmail())
            {
                ShowAlert("Email is invalid.", NotificationType.Error);

                return false;
            }
            if (!City.IsValidString())
            {
                ShowAlert("City is required.", NotificationType.Error);
                return false;
            }
            if (SelectedStore == null)
            {
                ShowAlert("Select a store.", NotificationType.Error);

                return false;
            }

            if (!Roles.Any(x => x.IsSelected))
            {
                ShowAlert("Select role.", NotificationType.Error);

                return false;
            }

            if (SelectedStore != null)
            {
                var _selectedRole = Roles.Where(x => x.IsSelected).FirstOrDefault();

                if (_selectedRole != null)
                {
                    if (SelectedStore.Name == "ALL STORE" && (_selectedRole.Name == AppConstants.ROLE_STORE_PERSON || _selectedRole.Name == AppConstants.ROLE_BACKEND || _selectedRole.Name == AppConstants.ROLE_SUPER_BACKEND))
                    {
                        ShowAlert("Please check selected store or selected role is wrong .", NotificationType.Error);

                        return false;
                    }
                    if (SelectedStore.Name != "ALL STORE" && (_selectedRole.Name == AppConstants.ROLE_ADMIN || _selectedRole.Name == AppConstants.ROLE_FINANCE || _selectedRole.Name == AppConstants.ROLE_DIRECTOR || _selectedRole.Name == AppConstants.ROLE_PURCHASE_MANAGER ||_selectedRole.Name==AppConstants.ROLE_CONTROL_MANAGER))
                    {
                        ShowAlert("Please check selected store or selected role is wrong .", NotificationType.Error);

                        return false;
                    }
                    if (SelectedStore.Parent_ref != null && (_selectedRole.Name != AppConstants.ROLE_STORE_PERSON))
                    {
                        ShowAlert("Please check selected store or selected role is wrong .", NotificationType.Error);

                        return false;
                    }
                }
            }

            return true;
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


        private ObservableCollection<Store> _stores;
        public ObservableCollection<Store> Stores
        {
            get { return _stores; }
            set { SetProperty(ref _stores, value); }
        }


        private async void AddUserFlyoutEvent(Boolean isopen)
        {
            //open flyout
            Header = "Add User";
            Width = 350;
            Height = GridLength.Auto;
            Position = Position.Right;
            IsOpen = isopen;
            if (isopen)
            {
                Roles = GetRoles();
                SelectedStore = null;
                Stores = null;
                try
                {
                    var _stores = await _storeService.GetStores();

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
                catch (Exception _ex)
                {
                    _logger.LogError(_ex.Message);
                }
            }
        }

        private string _name;

        //[Required(ErrorMessage = "Name is required")]
        public string Name
        {
            get { return _name; }
            set
            {
                SetProperty(ref _name, value);
                //if (value != null)
                //{
                //    ValidateProperty(value);
                //}

            }
        }

        private string _userName;

        //[Required(ErrorMessage = "User Name is required")]
        public string UserName
        {
            get { return _userName; }
            set
            {
                SetProperty(ref _userName, value);
                //{
                //    ValidateProperty(value);
                //}
            }
        }

        private string _password;
        //[Required(ErrorMessage = "Password is required")]
        public string Password
        {
            get { return _password; }
            set
            {
                SetProperty(ref _password, value);
                //if (value != null)
                //{
                //    ValidateProperty(value);
                //}

                ComparePassword = null;
            }
        }

        private string _comparePassword;
        // [Required(ErrorMessage = "Confirm password is required")]
        //[Compare(nameof(Password), ErrorMessage = "Password and confirm password do not match")]
        public string ComparePassword
        {
            get { return _comparePassword; }
            set
            {
                SetProperty(ref _comparePassword, value);
                //if (value != null)
                //{
                //    ValidateProperty(value);
                //}
            }
        }

        private string _phone;

        // [Required(ErrorMessage = "Phone number is required")]
        // [StringLength(maximumLength: 10, ErrorMessage = "Invalid Phone Number")]
        //[RegularExpression(pattern: "[6-9][0-9]{9}", ErrorMessage = "Invalid Phone number", MatchTimeoutInMilliseconds = 5000)]

        public string Phone
        {
            get { return _phone; }
            set
            {
                SetProperty(ref _phone, value);
                //{
                //    ValidateProperty(value);
                //}
            }
        }

        private string _email;

        //[Required(ErrorMessage = "Email is required")]
        //[EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email
        {
            get { return _email; }
            set
            {
                SetProperty(ref _email, value);
                //{
                //    ValidateProperty(value);
                //}
            }
        }
        private Store _selectedStore;
        //[Required(ErrorMessage = "Select a Store")]
        public Store SelectedStore
        {
            get { return _selectedStore; }
            set
            {
                SetProperty(ref _selectedStore, value);
                //{
                //    ValidateProperty(value);
                //}
            }
        }

        private string _city;
        // [Required(ErrorMessage = "City is required")]
        public string City
        {
            get { return _city; }
            set
            {
                SetProperty(ref _city, value);
                //{
                //    ValidateProperty(value);
                //}
            }
        }


        public void SelectionStoreChange(object obj)
        {
            try
            {
                var _storeView = (Store)obj;
                if (_storeView != null)
                {
                    if (_storeView.StoreId == 1)
                    {
                        foreach (var item in Roles)
                        {
                            if ((item.Name == AppConstants.ROLE_FINANCE) || (item.Name == AppConstants.ROLE_ADMIN) || (item.Name == AppConstants.ROLE_DIRECTOR) || (item.Name == AppConstants.ROLE_PURCHASE_MANAGER) || item.Name == AppConstants.ROLE_AUDITOR || item.Name == AppConstants.ROLE_TERRITORY_MANAGER || item.Name == AppConstants.ROLE_REGIONAL_MANAGER ||item.Name==AppConstants.ROLE_CONTROL_MANAGER)
                                item.IsEnableRole = true;
                            else
                                item.IsEnableRole = false;
                        }
                    }
                    if (_storeView.StoreId != 1 && _storeView.Parent_ref != null)
                    {
                        foreach (var item in Roles)
                        {
                            if ((item.Name == AppConstants.ROLE_FINANCE) || (item.Name == AppConstants.ROLE_ADMIN) || (item.Name == AppConstants.ROLE_DIRECTOR) || (item.Name == AppConstants.ROLE_PURCHASE_MANAGER) || (item.Name == AppConstants.ROLE_SUPER_BACKEND) || (item.Name == AppConstants.ROLE_BACKEND)||item.Name==AppConstants.ROLE_CONTROL_MANAGER)
                                item.IsEnableRole = false;
                            else
                                item.IsEnableRole = true;
                        }
                    }
                    if (_storeView.StoreId != 1 && _storeView.Parent_ref == null)
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
