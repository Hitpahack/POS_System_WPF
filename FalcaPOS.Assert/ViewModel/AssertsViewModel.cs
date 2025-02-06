using FalcaPOS.Assert.View;
using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Asserts;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FalcaPOS.Assert.ViewModel
{
    public class AssertsViewModel : BindableBase
    {

        private readonly INotificationService _notificationService;

        private readonly ProgressService _progressService;

        private readonly IDialogService _dialogService;

        private readonly IEventAggregator _eventAggregator;

        private readonly Logger _logger;

        private IAssertsServices _assertsServices;

        private readonly IStoreService _storeService;

        private ICommonService _commonService;
        public DelegateCommand AddAssertPopCommand { get; private set; }

        public DelegateCommand<object> AddAssertCodeCommand { get; private set; }
        public DelegateCommand<object> AddAssertClassCommand { get; private set; }
        public DelegateCommand<object> AddAssertTypeCommand { get; private set; }
        public DelegateCommand<object> AddAssertCategoryCommand { get; private set; }
        public DelegateCommand<object> SelectCgeAddcodeCommand { get; private set; }
        public DelegateCommand<object> SelectCgeAddclassCommand { get; private set; }
        public DelegateCommand<object> AddAssertCommand { get; private set; }

        public DelegateCommand<object> SelectCgeAssertcodeCommand { get; private set; }
        public DelegateCommand<object> SelectCgeAssertclassCommand { get; private set; }
        public DelegateCommand<object> SelectCgeAssertTypeCommand { get; private set; }
        public DelegateCommand<object> AssertsExportCommand { get; private set; }

        public DelegateCommand<object> SltCgeCodeCategoryAddCommand { get; private set; }
        public DelegateCommand<object> SltCgeClassCategoryAddCommand { get; private set; }
        public DelegateCommand<object> RefreshAssertPageCommand { get; private set; }
        public DelegateCommand<object> SearchAssertPageCommand { get; private set; }
        public DelegateCommand<object> EditAssertCommand { get; private set; }
        public DelegateCommand<object> EditBtnSubmitCommand { get; private set; }

        public DelegateCommand<object> RefreshAssertpopCommand { get; private set; }
        

        public AssertsViewModel(ICommonService commonService, IStoreService storeService, ProgressService progressService, Logger logger, INotificationService notificationService,
            IAssertsServices assertsServices, IEventAggregator eventAggregator)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _progressService = progressService;

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _assertsServices = assertsServices ?? throw new ArgumentNullException(nameof(assertsServices));

            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));

            AddAssertPopCommand = new DelegateCommand(AddAssertPopup);

            AddAssertCodeCommand = new DelegateCommand<object>(AddAssertcodeCommand);

            AddAssertClassCommand = new DelegateCommand<object>(AddAssertClass);

            AddAssertTypeCommand = new DelegateCommand<object>(AddAssertType);

            AddAssertCategoryCommand = new DelegateCommand<object>(AddAssertCategory);

            SelectCgeAddcodeCommand = new DelegateCommand<object>(GetAssertAddClass);

            SelectCgeAddclassCommand = new DelegateCommand<object>(GetAssertAddType);


            SelectCgeAssertcodeCommand = new DelegateCommand<object>(GetAssertClass);

            SelectCgeAssertclassCommand = new DelegateCommand<object>(GetAssertType);

            SelectCgeAssertTypeCommand = new DelegateCommand<object>(GetAssertCategory);

            SltCgeCodeCategoryAddCommand = new DelegateCommand<object>(GetAssertAddClassCtgry);

            SltCgeClassCategoryAddCommand = new DelegateCommand<object>(GetAssertAddTypeCtgery);

            AddAssertCommand = new DelegateCommand<object>(AddAssert);

            AssertsExportCommand = new DelegateCommand<object>(AssertExport);

            RefreshAssertPageCommand = new DelegateCommand<object>(Refresh);

            SearchAssertPageCommand = new DelegateCommand<object>(SearchAssertPage);

            GetAsserts();

            IsExportEnabled = false;

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _eventAggregator.GetEvent<AssertSearchEvent>().Subscribe(Search);

            EditAssertCommand = new DelegateCommand<object>(EditAsserts);

            EditBtnSubmitCommand = new DelegateCommand<object>(EditBtnSubmit);

            // Sets the visibility of Add Assets and Edit Assets to True for Auditor and Control manager.
            IsVisibility = AppConstants.USER_ROLES[0] == AppConstants.ROLE_AUDITOR || AppConstants.USER_ROLES[0] == AppConstants.ROLE_CONTROL_MANAGER ? true : false;

            RefreshAssertpopCommand = new DelegateCommand<object>(AssertPopUpClear);

        }

        public void Refresh(object obj)
        {
            try
            {
                // If RadGridView Object is not null, clears the Telerik Grid Filters.
                if (obj != null)
                {
                    ResetTelerikGridFilters.ClearTelerikGridViewFilters(obj);
                }
                GetAsserts();
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public async void AddAssertcodeCommand(object obj)
        {
            try
            {
                string assertcode = ((TextBox)obj).Text;

                if (string.IsNullOrEmpty(assertcode))
                {
                    _notificationService.ShowMessage("Please enter asset code", NotificationType.Error);
                    return;
                }
                if (!string.IsNullOrEmpty(assertcode))
                {

                    var _result = await _assertsServices.AddAssertCode(assertcode);
                    if (_result.IsSuccess)
                    {

                        _notificationService.ShowMessage(_result.Data, Common.NotificationType.Success);
                        ((TextBox)obj).Text = null;
                        PopupCloseCode = false;
                        GetAssertCode();

                    }
                    else
                        _notificationService.ShowMessage(_result.Error, Common.NotificationType.Error);
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public async void AddAssertClass(object obj)
        {
            try
            {
                string assertClass = ((TextBox)obj).Text;
                if (SelectedCodeAddClass == null)
                {
                    _notificationService.ShowMessage("Please select asset code", NotificationType.Error);
                    return;
                }
                if (string.IsNullOrEmpty(assertClass))
                {
                    _notificationService.ShowMessage("Please enter asset class", NotificationType.Error);
                    return;
                }
                if (!string.IsNullOrEmpty(assertClass))
                {
                    var _result = await _assertsServices.AddAssertClass(assertClass, SelectedCodeAddClass.Id);
                    if (_result.IsSuccess)
                    {

                        _notificationService.ShowMessage(_result.Data, Common.NotificationType.Success);
                        PopupCloseClass = false;
                        ((TextBox)obj).Text = null;
                        GetAssertClass(SelectedAssertCode);

                    }
                    else
                        _notificationService.ShowMessage(_result.Error, Common.NotificationType.Error);
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }


        public async void AddAssertType(object obj)
        {
            try
            {
                string assertType = ((TextBox)obj).Text;
                if (SelectedCodeAddType == null)
                {
                    _notificationService.ShowMessage("Please select asset code", NotificationType.Error);
                    return;
                }
                if (SelectedClassAddType == null)
                {
                    _notificationService.ShowMessage("Please select asset class", NotificationType.Error);
                    return;
                }

                if (string.IsNullOrEmpty(assertType))
                {
                    _notificationService.ShowMessage("Please enter asset type", NotificationType.Error);
                    return;
                }
                if (!string.IsNullOrEmpty(assertType))
                {
                    var _result = await _assertsServices.AddAssertType(assertType, SelectedClassAddType.Id);
                    if (_result.IsSuccess)
                    {

                        _notificationService.ShowMessage(_result.Data, Common.NotificationType.Success);
                        PopupCloseType = false;
                        ((TextBox)obj).Text = null;
                        GetAssertType(SelectedAssertClass);
                    }
                    else
                        _notificationService.ShowMessage(_result.Error, Common.NotificationType.Error);
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }


        public async void AddAssertCategory(object obj)
        {
            try
            {
                string assertCategory = ((TextBox)obj).Text;
                if (SelectedTypeAddCategory == null)
                {
                    _notificationService.ShowMessage("Please select asset type", NotificationType.Error);
                    return;
                }
                if (string.IsNullOrEmpty(SelectedTypeAddCategory.Name))
                {
                    _notificationService.ShowMessage("Please select asset type", NotificationType.Error);
                    return;
                }
                if (string.IsNullOrEmpty(assertCategory))
                {
                    _notificationService.ShowMessage("Please enter category", NotificationType.Error);
                    return;
                }
                if (!string.IsNullOrEmpty(assertCategory))
                {
                    var _result = await _assertsServices.AddAssertCategory(assertCategory, SelectedTypeAddCategory.Id);
                    if (_result.IsSuccess)
                    {

                        _notificationService.ShowMessage(_result.Data, Common.NotificationType.Success);
                        PopupCloseCategory = false;
                        ((TextBox)obj).Text = null;
                        GetAssertCategory(SelectedAssertType);
                    }
                    else
                        _notificationService.ShowMessage(_result.Error, Common.NotificationType.Error);
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }
        public async void AddAssertPopup()
        {
            AddAssertsPopup addAsserts = new AddAssertsPopup();
            addAsserts.DataContext = this;
            LoadStores();
            GetAssertCode();
            SelectedAssertCode = null;
            SelectedAssertClass = null;
            SelectedAssertType = null;
            SelectedAssertCategory = null;
            SelectedCodeAddClass = null;
            SelectedCodeAddCategory = null;
            SelectedClassAddType = null;
            SelectedClassAddCategory = null;
            SelectedCodeAddType = null;
            GoodStock = 0;
            DamageStock = 0;
            TotalStock = 0;
            Remarks = null;
            Reason = null;
            await DialogHost.Show(addAsserts, "RootDialog", AssertsEventHandler);

        }

        private async void AssertsEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {

                var _viewModel = (AssertsViewModel)eventArgs.Parameter;
                if (_viewModel != null)
                {
                    await _progressService.StartProgressAsync();

                    await Task.Run(async () =>
                    {
                        var _assertModel = new AddAssertModel()
                        {
                            CategoryId = SelectedAssertCategory.Id,
                            TotalStock = TotalStock,
                            DamageStock = DamageStock,
                            GoodStock = GoodStock,
                            StoreId = SelectedStores.StoreId,
                            Remarks = Remarks,
                            DamageReason = Reason,

                        };
                        var _result = await _assertsServices.AddAsserts(_assertModel);
                        if (_result != null && _result.IsSuccess)
                        {
                            _notificationService.ShowMessage(_result.Data, NotificationType.Success);
                            GetAsserts();
                        }
                        else { _notificationService.ShowMessage(_result.Error, NotificationType.Error); }
                    });

                    await _progressService.StopProgressAsync();
                }


            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);
            }
            finally
            {
                await _progressService.StopProgressAsync();
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

        public async void GetAssertAddClass(object obj)
        {
            try
            {
                try
                {
                    if (SelectedCodeAddType != null)
                    {
                        var _result = await _assertsServices.GetAssertsClass(SelectedCodeAddType.Id);
                        if (_result != null && _result.IsSuccess) { AssertClass = _result.Data.ToList(); }
                        else
                        {
                            _notificationService.ShowMessage(_result.Error, NotificationType.Error);
                            AssertClass = null;
                            SelectedClassAddType = null;
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

        public async void GetAssertAddType(object obj)
        {
            try
            {
                if (SelectedClassAddType != null)
                {
                    var _result = await _assertsServices.GetAssertsType(SelectedClassAddType.Id);
                    if (_result != null && _result.IsSuccess) { AssertType = _result.Data.ToList(); }
                    else { _notificationService.ShowMessage(_result.Error, NotificationType.Error); }
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public async void GetAssertAddClassCtgry(object obj)
        {
            try
            {
                try
                {
                    if (SelectedCodeAddCategory != null)
                    {
                        var _result = await _assertsServices.GetAssertsClass(SelectedCodeAddCategory.Id);
                        if (_result != null && _result.IsSuccess) { AssertClass = _result.Data.ToList(); }
                        else
                        {
                            _notificationService.ShowMessage(_result.Error, NotificationType.Error);
                            AssertClass = null;
                            SelectedClassAddCategory = null;
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

        public async void GetAssertAddTypeCtgery(object obj)
        {
            try
            {
                if (SelectedClassAddCategory != null)
                {
                    var _result = await _assertsServices.GetAssertsType(SelectedClassAddCategory.Id);
                    if (_result != null && _result.IsSuccess) { AssertType = _result.Data.ToList(); }
                    else
                    {
                        _notificationService.ShowMessage(_result.Error, NotificationType.Error);
                        AssertType = null;
                        SelectedTypeAddCategory = null;
                    }
                }

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

        private async void LoadStores()
        {
            try
            {
                Stores = null;

                await Task.Run(async () =>
                {
                    var _result = await _storeService.GetStores();

                    if (_result != null && _result.Any())
                    {
                        Application.Current?.Dispatcher.Invoke(() =>
                        {

                            Stores = new ObservableCollection<Entites.Stores.Store>(_result.Where(x => x.Parent_ref == null).ToList());
                        });
                    }

                });
            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in loading  store ", _ex);
            }

        }

        public void AddAssert(object obj)
        {
            try
            {

                if (SelectedAssertCode == null)
                {
                    _notificationService.ShowMessage("Please select asset code", NotificationType.Error);
                    return;
                }
                if (string.IsNullOrEmpty(SelectedAssertCode.Name))
                {
                    _notificationService.ShowMessage("Please select asset code", NotificationType.Error);
                    return;
                }

                if (SelectedAssertClass == null)
                {
                    _notificationService.ShowMessage("Please select asset class", NotificationType.Error);
                    return;
                }

                if (string.IsNullOrEmpty(SelectedAssertClass.Name))
                {
                    _notificationService.ShowMessage("Please select asset class", NotificationType.Error);
                    return;
                }

                if (SelectedAssertType == null)
                {
                    _notificationService.ShowMessage("Please select asset type", NotificationType.Error);
                    return;
                }

                if (string.IsNullOrEmpty(SelectedAssertType.Name))
                {
                    _notificationService.ShowMessage("Please select asset type", NotificationType.Error);
                    return;
                }

                if (SelectedAssertCategory == null)
                {
                    _notificationService.ShowMessage("Please select asset category", NotificationType.Error);
                    return;
                }

                if (string.IsNullOrEmpty(SelectedAssertCategory.Name))
                {
                    _notificationService.ShowMessage("Please select asset category", NotificationType.Error);
                    return;
                }

                if (SelectedStores == null)
                {
                    _notificationService.ShowMessage("Please select asset store", NotificationType.Error);
                    return;
                }

                if (string.IsNullOrEmpty(SelectedStores.Name))
                {
                    _notificationService.ShowMessage("Please select store", NotificationType.Error);
                    return;
                }

                if (GoodStock == 0)
                {
                    _notificationService.ShowMessage("Please enter valid good stock", NotificationType.Error);
                    return;
                }

                if (DamageStock > 0)
                {
                    if (string.IsNullOrEmpty(Reason))
                    {
                        _notificationService.ShowMessage("Please enter valid reason for damage stock", NotificationType.Error);
                        return;
                    }
                }

                if (TotalStock == 0)
                {
                    _notificationService.ShowMessage("Please enter valid total stock", NotificationType.Error);
                    return;
                }


                var TargetClose = ((Button)(obj));
                var dynamicCommand = TargetClose.Command;
                dynamicCommand.CanExecute(true);
                //pass Model as Arguments
                dynamicCommand.Execute(this);
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private async void GetAsserts()
        {
            try
            {

                await Task.Run(async () =>
                {
                    var _result = await _assertsServices.GetAsserts();

                    if (_result != null && _result.IsSuccess)
                    {
                        Asserts = new ObservableCollection<AssertsModelResponse>(_result.Data.ToList().Select(x => { x.IsVisibility = AppConstants.USER_ROLES[0] == AppConstants.ROLE_AUDITOR || AppConstants.USER_ROLES[0] == AppConstants.ROLE_CONTROL_MANAGER ? true : false; return x; }).ToList());
                        IsExportEnabled = true;
                    }
                    else
                    {
                        _notificationService.ShowMessage(_result.Error, NotificationType.Error);
                    }
                });
            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in loading  store ", _ex);
            }

        }

        public void AssertExport(object obj)
        {
            try
            {

                if (Asserts != null && Asserts.Count > 0)
                {

                    List<AssertsModelExport> AssertExport = new List<AssertsModelExport>();
                    foreach (var item in Asserts)
                    {

                        AssertExport.Add(new AssertsModelExport()
                        {
                            Category = item.Category,
                            Class = item.Class,
                            Code = item.Code,
                            Type = item.Type,
                            Store = item.Store,
                            DamageStock = item.DamageStock,
                            GoodStock = item.GoodStock,
                            TotalStock = item.TotalStock,
                            Remarks = item.Remarks,

                        });
                    }
                    bool _export = _commonService.ExportToXL(AssertExport, "AssetsReport", false, "Assets", FilePath: ApplicationSettings.ASSERTS_PATH.ToString());
                    if (_export)
                    {

                        _notificationService.ShowMessage("Assets exported successfully and file is exported to C:\\FALCAPOS\\Asserts folder", Common.NotificationType.Success);

                    }
                    else
                    {
                        _notificationService.ShowMessage("File export failed , try again", Common.NotificationType.Error);
                    }
                }
                else
                {
                    _notificationService.ShowMessage("No data found", Common.NotificationType.Error);
                    return;
                }


            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private ObservableCollection<Entites.Asserts.AssertsModelResponse> _asserts;
        public ObservableCollection<Entites.Asserts.AssertsModelResponse> Asserts
        {
            get => _asserts;
            set => SetProperty(ref _asserts, value);
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

        private AssertsModel _selectCodeAddClass;

        public AssertsModel SelectedCodeAddClass
        {
            get => _selectCodeAddClass;
            set => SetProperty(ref _selectCodeAddClass, value);
        }

        private AssertsModel _selectedCodeAddType;

        public AssertsModel SelectedCodeAddType
        {
            get => _selectedCodeAddType;
            set => SetProperty(ref _selectedCodeAddType, value);
        }

        private AssertsModel _selectCodeAddCategory;

        public AssertsModel SelectedCodeAddCategory
        {
            get => _selectCodeAddCategory;
            set => SetProperty(ref _selectCodeAddCategory, value);
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

        private AssertsModel _selectedClassAddType;

        public AssertsModel SelectedClassAddType
        {
            get => _selectedClassAddType;
            set => SetProperty(ref _selectedClassAddType, value);
        }

        private AssertsModel _selectedClassAddCategory;

        public AssertsModel SelectedClassAddCategory
        {
            get => _selectedClassAddCategory;
            set => SetProperty(ref _selectedClassAddCategory, value);
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

        private AssertsModel _selectedAddType;

        public AssertsModel SelectedAddType
        {
            get => _selectedAddType;
            set => SetProperty(ref _selectedAddType, value);
        }


        private AssertsModel _selectedTypeAddCategory;

        public AssertsModel SelectedTypeAddCategory
        {
            get => _selectedTypeAddCategory;
            set => SetProperty(ref _selectedTypeAddCategory, value);
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

        private int _goodStock;
        public int GoodStock
        {
            get => _goodStock;
            set
            {
                SetProperty(ref _goodStock, value);

                TotalStock = (_goodStock + DamageStock);
            }

        }

        private int _damageStock;

        public int DamageStock
        {
            get => _damageStock;
            set
            {
                SetProperty(ref _damageStock, value);
                TotalStock = (_damageStock + GoodStock);
            }
        }

        private int _totalStock;

        public int TotalStock
        {
            get => _totalStock;
            set => SetProperty(ref _totalStock, value);
        }

        private string _remarks;

        public string Remarks
        {
            get => _remarks;
            set => SetProperty(ref _remarks, value);
        }

        private bool _popupCloseCode;

        public bool PopupCloseCode
        {
            get => _popupCloseCode;
            set => SetProperty(ref _popupCloseCode, value);
        }


        private bool _popupCloseClass;

        public bool PopupCloseClass
        {
            get => _popupCloseClass;
            set => SetProperty(ref _popupCloseClass, value);

        }


        private bool _popupCloseType;

        public bool PopupCloseType
        {
            get => _popupCloseType;
            set => SetProperty(ref _popupCloseType, value);


        }


        private bool _popupCloseCategory;
        public bool PopupCloseCategory
        {
            get => _popupCloseCategory;
            set => SetProperty(ref _popupCloseCategory, value);


        }

        private bool _isExportEnabled;
        public bool IsExportEnabled
        {
            get => _isExportEnabled;
            set => _ = SetProperty(ref _isExportEnabled, value);
        }



        public void SearchAssertPage(object obj)
        {
            try
            {
                _eventAggregator.GetEvent<AssertSearchFlyoutOpen>().Publish(true);
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public async void Search(object obj)
        {
            try
            {
                var model = (SearchAssertsModel)obj;
                var _result = await _assertsServices.GetAssertSearch(model);
                if (_result.IsSuccess)
                {
                    Asserts = new ObservableCollection<AssertsModelResponse>(_result.Data.Select(x => { x.IsVisibility = AppConstants.USER_ROLES[0] == AppConstants.ROLE_AUDITOR || AppConstants.USER_ROLES[0] == AppConstants.ROLE_CONTROL_MANAGER ? true : false; return x; }));
                }
                else
                {
                    _notificationService.ShowMessage(_result.Error, NotificationType.Error);
                    Asserts = null;
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }



        private int _Id;
        public int Id
        {
            get => _Id;
            set => SetProperty(ref _Id, value);
        }

        private bool _IsVisibility;
        public bool IsVisibility
        {
            get => _IsVisibility;
            set => SetProperty(ref _IsVisibility, value);
        }


        public async void EditAsserts(object obj)
        {
            try
            {
                var _model = (AssertsModelResponse)obj;
                if (_model != null)
                {

                    EditAssetsPopup editAssetsPopup = new EditAssetsPopup();
                    editAssetsPopup.DataContext = this;
                    Id = _model.Id;
                    GoodStock = _model.GoodStock;
                    DamageStock = _model.DamageStock;
                    TotalStock = _model.TotalStock;
                    var Result = JsonConvert.DeserializeObject<List<ConvertToString>>(_model.History);
                    Reason = Result.LastOrDefault() != null ? Result.LastOrDefault().DamageReason : null;
                    await DialogHost.Show(editAssetsPopup, "RootDialog", EditAssertsEventHandler);
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private async void EditAssertsEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {

                var _viewModel = (AssertsViewModel)eventArgs.Parameter;
                if (_viewModel != null)
                {
                    await _progressService.StartProgressAsync();

                    await Task.Run(async () =>
                    {


                        var _result = await _assertsServices.EditAsserts(new EditAssertModel() { GoodStock = GoodStock, DamageStock = DamageStock, TotalStock = TotalStock, Id = Id, Reason = Reason });
                        if (_result != null && _result.IsSuccess)
                        {
                            _notificationService.ShowMessage(_result.Data, NotificationType.Success);
                            GetAsserts();
                        }
                        else { _notificationService.ShowMessage(_result.Error, NotificationType.Error); }
                    });

                    await _progressService.StopProgressAsync();
                }


            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);
            }
            finally
            {
                await _progressService.StopProgressAsync();
            }
        }

        public void EditBtnSubmit(object obj)
        {
            try
            {

                if (DamageStock > 0)
                {
                    if (string.IsNullOrEmpty(Reason))
                    {
                        _notificationService.ShowMessage("Please enter valid damage stock reason", NotificationType.Error);
                        return;
                    }
                }

                var TargetClose = ((Button)(obj));
                var dynamicCommand = TargetClose.Command;
                dynamicCommand.CanExecute(true);
                //pass Model as Arguments
                dynamicCommand.Execute(this);
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private string _reason;

        public string Reason
        {
            get => _reason;
            set => SetProperty(ref _reason, value);
        }

        public void AssertPopUpClear(object obj) {
            try {
                SelectedAssertCode = null;
                SelectedAssertClass = null;
                SelectedAssertType = null;
                SelectedAssertCategory = null;
                SelectedStores = null;
                GoodStock = 0;
                DamageStock = 0;
                TotalStock = 0;
                Reason = null;
                Remarks = null;
            }
            catch (Exception _ex) {
                _logger.LogError(_ex.Message);
            }
        }


    }

    public class ConvertToString
    {
        public int GoodStock { get; set; }

        public int DamageStock { get; set; }

        public int TotalStock { get; set; }

        public string DamageReason { get; set; }
    }
}



