using FalcaPOS.Common;
using FalcaPOS.Common.Cache;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Common.Models;
using FalcaPOS.Common.Validations;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.User;
using Prism.Commands;
using Prism.Events;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Unity;

namespace FalcaPOS.Login.ViewModels
{
    public class LoginViewModel : ValidationBase
    {

        private readonly IRegionManager _regionManager;

        private readonly IEventAggregator _eventAggregator;

        private readonly ILoginService _loginService;

        private readonly Logger _logger;

        private readonly IUnityContainer _container;

        private readonly ProgressService _progressService;

        private readonly ISignalRService _signalRService;

        private readonly INotificationService _notificationService;

        public DelegateCommand LoginCommand { get; private set; }



        public LoginViewModel(IRegionManager regionManager,
            ProgressService progressService,
            IUnityContainer container,
            IEventAggregator eventAggregator,
            ILoginService loginService,
            Logger logger, ISignalRService signalRService, INotificationService notificationService)
        {

            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));

            LoginCommand = new DelegateCommand(Login);

            _container = container ?? throw new ArgumentNullException(nameof(container));

            _regionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _signalRService = signalRService ?? throw new ArgumentNullException(nameof(signalRService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));


            IsLoginEnabled = true;

            _logger.LogInformation("Starting login screen");

            IsQABuild = ApplicationSettings.APP_ENVIRONMENT == "TESTING";
            IsPreProdBuild = ApplicationSettings.APP_ENVIRONMENT == "PREPRODUCTION";

            //UserName = "gshambu";
            //Password = "admin";

        }

        private bool _isQABuild;
        public bool IsQABuild
        {
            get { return _isQABuild; }
            set
            {
                SetProperty(ref _isQABuild, value);
            }
        }
        
        private bool _ispreprodBuild;
        public bool IsPreProdBuild
        {
            get { return _ispreprodBuild; }
            set
            {
                SetProperty(ref _ispreprodBuild, value);
            }
        }

        private string _userName;


        private bool _isLoginEnabled;
        public bool IsLoginEnabled
        {
            get { return _isLoginEnabled; }
            set { SetProperty(ref _isLoginEnabled, value); }
        }


        [MaxLength(30, ErrorMessage = "Maximum allowed character length is 30.")]
        [Required(ErrorMessage = "User name is required")]
        public string UserName
        {
            get { return _userName; }
            set
            {
                SetProperty(ref _userName, value);
                ValidateProperty(value);

            }
        }

        private string _password;


        [Required(ErrorMessage = "Password is required.")]
        [MaxLength(30, ErrorMessage = "Maximum allowed character length is 30.")]
        public string Password
        {
            get { return _password; }
            set
            {
                SetProperty(ref _password, value);
                ValidateProperty(value);
            }
        }



        private async void Login()
        {
            if (string.IsNullOrEmpty(UserName))
            {
                _notificationService.ShowMessage("Please enter user name",NotificationType.Error);
                return;
            }
            if (string.IsNullOrEmpty(Password))
            {
                _notificationService.ShowMessage("Please enter password", NotificationType.Error);
                return;
            }


                await _progressService.StartProgressAsync();


                _logger.LogInformation($"user login start for user name {UserName}");

                var _result = await _loginService.UserLogin(new Entites.Login.Login() { UserName = UserName, Password = _password });

                if (_result != null && _result.IsSuccess && _result.Data != null && _result.Data.roles.Length > 0)
                {
                    if (Directory.Exists(ApplicationSettings.CachePath))
                    {
                        _container.Resolve<CacheHub>();
                    }
                    //else copy all file to cache location
                    _logger.LogInformation($"login success for user {_result.Data.username}");

                    AppConstants.ACCESS_TOKEN = _result.Data.token;

                    AppConstants.UserName = _result.Data.username;

                    AppConstants.UserStoreLocation = _result.Data.StoreCityLocation;

                    AppConstants.StoreName = _result.Data.StoreInfo.Name;

                    AppConstants.LoggedInStoreInfo = _result.Data.StoreInfo;

                    AppConstants.STATE = _result.Data.State;

                    AppConstants.IsLoggedIn = true;

                AppConstants.UserId = _result.Data.Id;

                    if (_result.Data.roles != null && _result.Data.roles.Any())
                    {
                        AppConstants.USER_ROLES = _result.Data.roles;
                    }

                    UserName = string.Empty;

                    Password = "";

                    string role = _result.Data.roles.FirstOrDefault();

                    AppConstants.CurrentDate = _result.Data.CurrentDate;

                    AppConstants.FalcaGSTIN = _result.Data.FalcaGSTIN;

                AppConstants.Printer = _result.Data.Printer;

                    //if (_result.Data.roles.Contains("admin"))
                    //{
                    //    role = "admin";
                    //}
                    //else if (_result.Data.roles.Contains("superbackend"))
                    //{
                    //    role = "superbackend";
                    //}
                    //else if (_result.Data.roles.Contains("storeperson"))
                    //{

                    //    role = "storeperson";
                    //}
                    //else if (_result.Data.roles.Contains(AppConstants.ROLE_FINANCE))
                    //{
                    //    role = AppConstants.ROLE_FINANCE;
                    //}
                    //else
                    //{
                    //    role = "backend";
                    //}


                    //check which module to show to users.
                    _container.Resolve<IModuleManager>().LoadModule("HomeModule");
                    _container.Resolve<IModuleManager>().LoadModule("StoreModule");
                    _container.Resolve<IModuleManager>().LoadModule("TeamModule");
                    _container.Resolve<IModuleManager>().LoadModule("SkuModule");
                    _container.Resolve<IModuleManager>().LoadModule("ExpiryProductsModule");
                    _container.Resolve<IModuleManager>().LoadModule("RSPModule");
                    _container.Resolve<IModuleManager>().LoadModule("StockAgeModule");
                    _container.Resolve<IModuleManager>().LoadModule("ReportsModule");
                switch (role)
                    {
                        case AppConstants.ROLE_ADMIN:
                            _container.Resolve<IModuleManager>().LoadModule("StockModule");
                            _container.Resolve<IModuleManager>().LoadModule("CustomerModule");
                            _container.Resolve<IModuleManager>().LoadModule("DashboardModule");
                            _container.Resolve<IModuleManager>().LoadModule("FinanceModule");
                            _container.Resolve<IModuleManager>().LoadModule("IndentModule");
                            _container.Resolve<IModuleManager>().LoadModule("InvoiceModule");
                            break;

                        case AppConstants.ROLE_BACKEND:
                            _container.Resolve<IModuleManager>().LoadModule("StockModule");
                            _container.Resolve<IModuleManager>().LoadModule("InventoryModule");
                            _container.Resolve<IModuleManager>().LoadModule("IndentModule");
                            _container.Resolve<IModuleManager>().LoadModule("InvoiceModule");
                            break;

                        case AppConstants.ROLE_STORE_PERSON:
                            _container.Resolve<IModuleManager>().LoadModule("StockModule");
                            _container.Resolve<IModuleManager>().LoadModule("SalesModule");
                            _container.Resolve<IModuleManager>().LoadModule("CustomerModule");
                            _container.Resolve<IModuleManager>().LoadModule("IndentModule");
                            _container.Resolve<IModuleManager>().LoadModule("DenominationModule");
                            _container.Resolve<IModuleManager>().LoadModule("PurchaseReturnsModule");
                            _container.Resolve<IModuleManager>().LoadModule("AssertModule");
                           
                        break;

                        case AppConstants.ROLE_SUPER_BACKEND:
                            _container.Resolve<IModuleManager>().LoadModule("StockModule");
                            //_container.Resolve<IModuleManager>().LoadModule("InventoryModule");
                            _container.Resolve<IModuleManager>().LoadModule("InvoiceModule");
                            _container.Resolve<IModuleManager>().LoadModule("IndentModule");
                            break;
                        case AppConstants.ROLE_CONTROL_MANAGER:
                        case AppConstants.ROLE_FINANCE:
                            _container.Resolve<IModuleManager>().LoadModule("DashboardModule");
                            _container.Resolve<IModuleManager>().LoadModule("CustomerModule");
                            _container.Resolve<IModuleManager>().LoadModule("FinanceModule");
                            _container.Resolve<IModuleManager>().LoadModule("InvoiceModule");
                            _container.Resolve<IModuleManager>().LoadModule("StockModule");
                            _container.Resolve<IModuleManager>().LoadModule("DenominationModule");
                            _container.Resolve<IModuleManager>().LoadModule("SuppliersModule");
                            _container.Resolve<IModuleManager>().LoadModule("StockAgeModule");
                            _container.Resolve<IModuleManager>().LoadModule("IndentModule");
                            _container.Resolve<IModuleManager>().LoadModule("SalesModule");
                            _container.Resolve<IModuleManager>().LoadModule("AssertModule");
                            break;
                        case AppConstants.ROLE_DIRECTOR:
                            _container.Resolve<IModuleManager>().LoadModule("DirectorModule");
                        _container.Resolve<IModuleManager>().LoadModule("DenominationModule");
                        _container.Resolve<IModuleManager>().LoadModule("CustomerModule");
                            _container.Resolve<IModuleManager>().LoadModule("StockModule");
                            _container.Resolve<IModuleManager>().LoadModule("IndentModule");
                            _container.Resolve<IModuleManager>().LoadModule("StockAgeModule");
                            _container.Resolve<IModuleManager>().LoadModule("FinanceModule");
                        _container.Resolve<IModuleManager>().LoadModule("SalesModule");
                            _container.Resolve<IModuleManager>().LoadModule("ZoneTerritoryModule");

                        break;
                        case AppConstants.ROLE_PURCHASE_MANAGER:
                            _container.Resolve<IModuleManager>().LoadModule("PurchaseManagerModule");
                            _container.Resolve<IModuleManager>().LoadModule("IndentModule");
                            _container.Resolve<IModuleManager>().LoadModule("StockModule");
                            _container.Resolve<IModuleManager>().LoadModule("InventoryModule");
                            _container.Resolve<IModuleManager>().LoadModule("InvoiceModule");
                            _container.Resolve<IModuleManager>().LoadModule("StockAgeModule");
                            _container.Resolve<IModuleManager>().LoadModule("SalesModule");
                            _container.Resolve<IModuleManager>().LoadModule("ZoneTerritoryModule");
                        
 
                            break;
                        case AppConstants.ROLE_AUDITOR:
                            _container.Resolve<IModuleManager>().LoadModule("AssertModule");
                            _container.Resolve<IModuleManager>().LoadModule("FinanceModule");

                            break;
                        case AppConstants.ROLE_TERRITORY_MANAGER:
                        _container.Resolve<IModuleManager>().LoadModule("StockModule");
                        _container.Resolve<IModuleManager>().LoadModule("IndentModule");
                        _container.Resolve<IModuleManager>().LoadModule("FinanceModule");
                        _container.Resolve<IModuleManager>().LoadModule("AssertModule");

                        break;
                        case AppConstants.ROLE_REGIONAL_MANAGER:
                        _container.Resolve<IModuleManager>().LoadModule("IndentModule");
                        _container.Resolve<IModuleManager>().LoadModule("FinanceModule");
                        _container.Resolve<IModuleManager>().LoadModule("AssertModule");

                        break;
                        default:
                            break;
                    }


                    var _region = _regionManager.Regions["LoginRegion"];
                    _region.Deactivate(_region.Views.FirstOrDefault());
                    _region.Activate(_region.Views.LastOrDefault());
                    _eventAggregator.GetEvent<FalcaPOS.Common.Events.Login>().Publish(true);
                    _eventAggregator.GetEvent<FalcaPOS.Common.Events.LoginedRole>().Publish(role);
                    _eventAggregator.GetEvent<LoggedInUserEvent>().Publish(new User
                    {
                        Name = _result.Data.username,
                    });
                    _eventAggregator.GetEvent<NotifyMessage>().Publish(new ToastMessage { Message = $"Welcome {_result.Data.username}", MessageType = NotificationType.Success });
                    _eventAggregator.GetEvent<DenominationVerifyEvent>().Publish();
    

                //No Use case yet disabled
                await _signalRService.ConnectToHubAsync($"{ApplicationSettings.ServerUrl}poshub");
                }
                else
                {

                    _logger.LogError($"User login failed {_result?.Error}");

                    if (_result != null && !_result.IsSuccess)
                    {
                        if (!string.IsNullOrEmpty(_result.Error))
                        {
                            _eventAggregator
                                .GetEvent<NotifyMessage>()
                                .Publish(new ToastMessage
                                { Message = _result.Error, MessageType = NotificationType.Error });
                        }


                    }

                    IsLoginEnabled = true;
                }

                await _progressService.StopProgressAsync();
            }


        


    }
}
