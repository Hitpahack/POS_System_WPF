using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Asserts;
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

namespace FalcaPOS.Assert.ViewModel
{
    public class AssertSearchFlyoutViewModel : BindableBase
    {

        private bool isOpen;
        public bool IsOpen
        {
            get => this.isOpen;
            set => SetProperty(ref this.isOpen, value);
        }

        private Position position;
        public Position Position
        {
            get => this.position;
            set => SetProperty(ref this.position, value);

        }

        private int _width;
        public int Width
        {
            get => _width;
            set => SetProperty(ref _width, value);

        }
        private int _height;

        public int Height
        {
            get => _height;
            set => SetProperty(ref _height, value);

        }

        private IEventAggregator _eventAggregator;

        private readonly IStoreService _storeService;

        private readonly Logger _logger;

        private readonly ProgressService _progressService;

        private IAssertsServices _assertsServices;

        private readonly INotificationService _notificationService;
        public DelegateCommand<object> SelectCgeAssertcodeCommand { get; private set; }
        public DelegateCommand<object> SelectCgeAssertclassCommand { get; private set; }
        public DelegateCommand<object> SelectCgeAssertTypeCommand { get; private set; }

        public DelegateCommand<object> FlyOutSearchAssertCommand { get; private set; }

        public DelegateCommand<object> FlyoutResetAssertCommand { get; private set; }


        public AssertSearchFlyoutViewModel(IEventAggregator eventAggregator, IStoreService storeService, Logger logger, ProgressService progressService, IAssertsServices assertsServices, INotificationService notificationService)
        {
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));


            _assertsServices = assertsServices ?? throw new ArgumentNullException(nameof(assertsServices));


            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            SelectCgeAssertcodeCommand = new DelegateCommand<object>(GetAssertClass);

            SelectCgeAssertclassCommand = new DelegateCommand<object>(GetAssertType);

            SelectCgeAssertTypeCommand = new DelegateCommand<object>(GetAssertCategory);

            FlyOutSearchAssertCommand = new DelegateCommand<object>(SearchFly);

            FlyoutResetAssertCommand = new DelegateCommand<object>(ResetFly);

            this.Width = 1600;

            this.Height = 200;

            this.Position = MahApps.Metro.Controls.Position.Top;

            _eventAggregator.GetEvent<AssertSearchFlyoutOpen>().Subscribe((is_open) =>
            {
                this.IsOpen = is_open;
                GetAssertCode();
                LoadStores();
                SelectedAssertCategory = null;
                SelectedAssertClass = null;
                SelectedAssertCode = null;
                SelectedAssertType = null;
                SelectedStores = null;
                AssertCategory = null;
                AssertClass = null;
                AssertType = null;
                
            });

            IsStoreVisibility = (AppConstants.USER_ROLES[0]==AppConstants.ROLE_STORE_PERSON)?true:false;
        }

        public void SearchFly(object obj)
        {
            try
            {
                if (SelectedAssertCode == null && SelectedAssertType == null && SelectedAssertCategory == null && SelectedAssertClass == null && SelectedStores == null)
                {
                    _notificationService.ShowMessage("Please select any one field", NotificationType.Error);
                    return;
                }

                var _search = new SearchAssertsModel()
                {
                    StoreId = (AppConstants.USER_ROLES[0]==AppConstants.ROLE_STORE_PERSON)?AppConstants.LoggedInStoreInfo.StoreId: SelectedStores != null ? SelectedStores.StoreId : 0,
                    CategoryId = SelectedAssertCategory != null ? SelectedAssertCategory.Id : 0,
                    ClassId = SelectedAssertClass != null ? SelectedAssertClass.Id : 0,
                    CodeId = SelectedAssertCode != null ? SelectedAssertCode.Id : 0,
                    TypeId = SelectedAssertType != null ? SelectedAssertType.Id : 0,
                };

                _eventAggregator.GetEvent<AssertSearchEvent>().Publish(_search);

                IsOpen = false;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public void ResetFly(object obj)
        {
            try
            {

                SelectedAssertCategory = null;
                SelectedAssertClass = null;
                SelectedAssertCode = null;
                SelectedAssertType = null;
                SelectedStores = null;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public async void GetAssertClass(object obj)
        {
            try
            {
                try
                {
                    if (SelectedAssertCode != null)
                    {
                        var _result = await _assertsServices.GetAssertsClass(SelectedAssertCode.Id);
                        if (_result != null && _result.IsSuccess) { AssertClass = _result.Data.ToList(); }
                        else
                        {
                            _notificationService.ShowMessage(_result.Error, NotificationType.Error);
                            SelectedAssertClass = null;
                            AssertClass = null;
                        }
                    }

                }
                catch (Exception _ex)
                {
                    _logger.LogError(_ex.Message);
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }


        public async void GetAssertType(object obj)
        {
            try
            {
                if (SelectedAssertClass != null)
                {
                    var _result = await _assertsServices.GetAssertsType(SelectedAssertClass.Id);
                    if (_result != null && _result.IsSuccess)
                    {
                        AssertType = _result.Data.ToList();
                    }
                    else
                    {
                        _notificationService.ShowMessage(_result.Error, NotificationType.Error);
                        SelectedAssertType = null;
                        AssertType = null;
                    }
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public async void GetAssertCategory(object obj)
        {
            try
            {
                if (SelectedAssertType != null)
                {
                    var _result = await _assertsServices.GetAssertCategory(SelectedAssertType.Id);
                    if (_result != null && _result.IsSuccess)
                    {
                        AssertCategory = _result.Data.ToList();
                    }
                    else
                    {
                        _notificationService.ShowMessage(_result.Error, NotificationType.Error);
                        SelectedAssertCategory = null;
                        AssertCategory = null;
                    }
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public async void GetAssertCode()
        {
            try
            {

                var _result = await _assertsServices.GetAssertsCode();
                if (_result != null && _result.IsSuccess) { AssertCode = _result.Data.ToList(); }
                else { _notificationService.ShowMessage(_result.Error, NotificationType.Error); }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private async void LoadStores()
        {
            try
            {
                Stores = null;
               
                //await Task.Run(async () =>
                //{
                //    var _result = await _storeService.GetStores();

                //    if (_result != null && _result.Any())
                //    {
                //        Application.Current?.Dispatcher.Invoke(() =>
                //        {

                //            Stores = new ObservableCollection<Entites.Stores.Store>(_result.Where(x => x.Parent_ref == null).ToList());
                //        });
                //    }

                //});
                await Task.Run(async () =>
                {
                    var _result = await _storeService.GetStoreDetailsbyuser(AppConstants.UserId, AppConstants.USER_ROLES[0]);

                    if (_result != null && _result.Any())
                    {
                        Application.Current?.Dispatcher.Invoke(() =>
                        {

                            Stores = new ObservableCollection<Entites.Stores.Store>(_result.OrderBy(x => x.Name).ToList());
                        });
                    }

                });
            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in loading  store ", _ex);
            }

        }

        private ObservableCollection<Entites.Stores.Store> _stores;
        public ObservableCollection<Entites.Stores.Store> Stores
        {
            get => _stores;
            set => SetProperty(ref _stores, value);
        }

        private Entites.Stores.Store _selectedStores;
        public Entites.Stores.Store SelectedStores
        {
            get => _selectedStores;
            set => SetProperty(ref _selectedStores, value);
        }

        private List<AssertsModel> _assertCode;

        public List<AssertsModel> AssertCode
        {
            get => _assertCode;
            set => SetProperty(ref _assertCode, value);
        }

        private AssertsModel _selectedassertCode;

        public AssertsModel SelectedAssertCode
        {
            get => _selectedassertCode;
            set => SetProperty(ref _selectedassertCode, value);
        }

        private List<AssertsModel> _assertClass;

        public List<AssertsModel> AssertClass
        {
            get => _assertClass;
            set => SetProperty(ref _assertClass, value);
        }

        private AssertsModel _selectedassertClass;

        public AssertsModel SelectedAssertClass
        {
            get => _selectedassertClass;
            set => SetProperty(ref _selectedassertClass, value);
        }

        private List<AssertsModel> _assertType;

        public List<AssertsModel> AssertType
        {
            get => _assertType;
            set => SetProperty(ref _assertType, value);
        }

        private AssertsModel _selectedassertType;

        public AssertsModel SelectedAssertType
        {
            get => _selectedassertType;
            set => SetProperty(ref _selectedassertType, value);
        }

        private List<AssertsModel> _assertCategory;

        public List<AssertsModel> AssertCategory
        {
            get => _assertCategory;
            set => SetProperty(ref _assertCategory, value);
        }

        private AssertsModel _selectedassertCategory;

        public AssertsModel SelectedAssertCategory
        {
            get => _selectedassertCategory;
            set => SetProperty(ref _selectedassertCategory, value);
        }

        private bool _isStoreVisibility;

        public bool IsStoreVisibility
        {
            get => _isStoreVisibility; 
            set => SetProperty(ref _isStoreVisibility , value); 
        }

    }
}
