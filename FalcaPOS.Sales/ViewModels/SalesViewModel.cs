
using FalcaPOS.Common;
using FalcaPOS.Common.Cache.Events;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Customers;
using FalcaPOS.Entites.Denomination;
using FalcaPOS.Entites.Location;
using FalcaPOS.Entites.Sales;
using FalcaPOS.Sales.Models;
using FalcaPOS.Sales.Views;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Compilation;
using System.Windows;
using System.Windows.Controls;

namespace FalcaPOS.Sales.ViewModels
{
    public class SalesViewModel : BindableBase
    {
        private readonly ISalesService _salesService;

        private readonly ICustomerService _customerService;

        private readonly Logger _logger;

        private readonly IEventAggregator _eventAggregator;

        public DelegateCommand GetProductCommand { get; private set; }

        public DelegateCommand SaveSalesDetailsCommand { get; private set; }

        public DelegateCommand ResetSalesCommand { get; private set; }

        public DelegateCommand<object> RemoveProductCommand { get; private set; }

        public DelegateCommand GetCustomerCommand { get; private set; }

        public DelegateCommand ShowSchemeCouponCommand { get; private set; }

        public DelegateCommand<object> SchemeCouponAcknowlegementCommand { get; private set; }

        public DelegateCommand<object> SchemeCouponCancelCommand { get; private set; }

        public DelegateCommand<Object> GetPinCodeDetailsCommand { get; private set; }

        public DelegateCommand StateSelectionCommand { get; private set; }

        //public DelegateCommand PriceCalulationCommand { get; private set; }

        //     public DelegateCommand TotalCalulationCommand { get; private set; }

        private readonly INotificationService _notificationService;

        private readonly ProgressService _progressService;

        public DelegateCommand<object> ProductAddCommand { get; private set; }


        public DelegateCommand GetProductSKUCommand { get; private set; }

        public DelegateCommand AppOrderCustomerCommand { get; private set; }

        private readonly IDenominationService _denominationService;

        public DelegateCommand<object> SaveNewCustomerCommand { get; private set; }
        public DelegateCommand<object> EditCustomerCommand { get; private set; }
        public DelegateCommand<object> EditCustomerPopUpCommand { get; private set; }
        public DelegateCommand<object> ApplyCouponCodeCommand { get; private set; }
        public DelegateCommand RemoveCouponCodeCommand { get; private set; }
        public DelegateCommand<object> LandTypeCommand { get; private set; }
        public DelegateCommand<object> AddCropCommand { get; private set; }
        public DelegateCommand<object> RemoveCropCommand { get; private set; }

        public DelegateCommand<object> OTPCommand { get; private set; }
        public IEnumerable<District> DistrictCache { get; set; } = new ObservableCollection<District>();
        public SalesViewModel(ISalesService salesService,
            ICustomerService customerService,
            INotificationService notificationService,
            ProgressService progressService,
            Logger logger,
            IEventAggregator eventAggregator, IDenominationService denominationService)
        {
            _salesService = salesService ?? throw new ArgumentNullException(nameof(salesService));

            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));

            _denominationService = denominationService ?? throw new ArgumentNullException(nameof(denominationService));

            GetProductCommand = new DelegateCommand(GetProduct);

            GetCustomerCommand = new DelegateCommand(GetCustomerInfo);

            GetPinCodeDetailsCommand = new DelegateCommand<Object>(GetPinCodeDetails);

            RemoveProductCommand = new DelegateCommand<object>(RemoveProduct);

            //SaveSalesDetailsCommand = new DelegateCommand(SaveSalesDetails);
            SaveSalesDetailsCommand = new DelegateCommand(SaveSalesDetails);

            //SaveSalesDetailsCommand = new DelegateCommand(ShowSchemesCoupons);
            ResetSalesCommand = new DelegateCommand(ResetSales);

            StateSelectionCommand = new DelegateCommand(LoadDistricts);

            // PriceCalulationCommand = new DelegateCommand(PriceCalculation);

            //TotalCalulationCommand = new DelegateCommand(TotalCalculation);

            InvoiceDate = DateTime.Now;

            _eventAggregator.GetEvent<SignalRStateAddedEvent>().Subscribe(StateAddedEventHandler);



            if (ApplicationSettings.CacheEnabled)
            {

                _eventAggregator.GetEvent<ResponseCacheEvent>().Subscribe((x) =>
                {
                    if (x != null)
                    {
                        if (!String.IsNullOrEmpty(x.Payload.ToString()))
                        {
                            switch (x.Id)
                            {
                                case CacheKeyEnum.States:
                                    States = new ObservableCollection<State>(JsonConvert.DeserializeObject<IEnumerable<State>>(x.Payload.FirstOrDefault()));
                                    break;
                                case CacheKeyEnum.Districts:
                                    {
                                        if (SelectState?.StateId != 0)
                                        {
                                            var _districts = JsonConvert.DeserializeObject<IEnumerable<District>>(x.Payload.FirstOrDefault());
                                            if (_districts != null)
                                            {
                                                if (DistrictCache.Count() == 0)
                                                    DistrictCache = _districts;
                                                Districts = new ObservableCollection<District>(_districts.Where(xs => xs.StateId == SelectState.StateId));
                                            }
                                        }
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }

                    }
                });


            }
            else
            {
                LoadStates();
            }

            LoadInvoiceNumber();

            double userHeightScreen = System.Windows.SystemParameters.PrimaryScreenHeight;

            MaxHeight = userHeightScreen / 2;


            GetProductSKUCommand = new DelegateCommand(GetProductSKUSearch);

            ProductAddCommand = new DelegateCommand<object>(updateProduct);

            _eventAggregator.GetEvent<AppOrderMoveToSalePageEvent>().Subscribe(AppOrderCustomer);

            AppOrderCustomerCommand = new DelegateCommand(AppOrderCustomer);

            IsAppOrderEnable = false;
            _eventAggregator.GetEvent<ResponseDictionaryCacheEvent>().Publish(new Common.Cache.RequestCacheModel
            {
                Id = CacheKeyEnum.States,
            });

            _eventAggregator.GetEvent<DenominationVerifyEvent>().Subscribe(GetDenominationVerify);

            _eventAggregator.GetEvent<SalesPageRefreshEvent>().Subscribe((x) => SalePageRefresh(x));

            _eventAggregator.GetEvent<EditCustomerPopupEvent>().Subscribe((x) => SaveEditCustomer(x));

            CustomerTypes = ApplicationSettings.CUSTOMER_TYPE.ToList();
            SaveNewCustomerCommand = new DelegateCommand<object>(SaveCustomer);
            EditCustomerCommand = new DelegateCommand<object>(EditCustomer);

            ShowSchemeCouponCommand = new DelegateCommand(ShowSchemesCoupons);

            SchemeCouponAcknowlegementCommand = new DelegateCommand<object>(OkSchemeCoupon);
            EditCustomerPopUpCommand = new DelegateCommand<object>(EditValidateCustomer);
            ProductCount = "0";
            ApplyCouponCodeCommand = new DelegateCommand<object>(ApplyCode);
            RemoveCouponCodeCommand = new DelegateCommand(RemoveCode);
            LandTypeCommand = new DelegateCommand<object>(LandsType);
            LandType = new List<string>();
            Crops = new ObservableCollection<string>();
            AddCropCommand = new DelegateCommand<object>(AddCrop);
            RemoveCropCommand = new DelegateCommand<object>(RemoveCrop);
            OTPCommand = new DelegateCommand<object>(OTPValidation);
            IsSaveBtnEnable = true;

        }

        private async void ShowSchemesCoupons()
        {
            try
            {
                if (IsDiscountApplied)
                {
                    CouponSchemeDataViewPopup couponSchemeDataViewPopup = new CouponSchemeDataViewPopup();
                    couponSchemeDataViewPopup.DataContext = this;
                    await DialogHost.Show(couponSchemeDataViewPopup, "RootDialog", CouponSchemeDataViewPopupClosingEventHandler);
                }
                else
                {
                    IsSaveBtnEnable = false;
                    AddSales();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        private void OkSchemeCoupon(object obj)
        {
            var TargetClose = ((Button)(obj));
            var dynamicCommand = TargetClose.Command;
            dynamicCommand.CanExecute(true);
            dynamicCommand.Execute(this);
        }

        private void CouponSchemeDataViewPopupClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            var _viewModel = (SalesViewModel)eventArgs.Parameter;
            if (_viewModel != null)
            {
                IsSaveBtnEnable = false;
                AddSales();
            }
            else
            {
                IsSaveBtnEnable = true;
            }
        }

        private void RemoveCode()
        {
            try
            {
                IsDiscountApplied = false;
                CouponCode = null;
                PayableAmount = Convert.ToDouble(this.SalesProducts?.Sum(x => x.SellingQty * x.AcutalSellingPrice));
                TotalCouponAmount = 0;
                if (SalesProducts != null && SalesProducts.Count > 0)
                {
                    foreach (var it in SalesProducts)
                    {
                            it.ProductManualDiscount = 0;
                    }
                }
                CalculateTotal();
                StoreOTP = 0;
                OTP = 0;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public async void SaveEditCustomer(Object model)
        {
            try
            {
                if (model != null)
                {
                        var _SourceModel = (EditCustomerPopupViewModel)model;
                        if (_SourceModel != null)
                        {
                            var editCustomer = new CustomerModel()
                            {
                                Name = _SourceModel.CustomerName,
                                Phone = _SourceModel.Phone,
                                GSTNumber = _SourceModel.GSTIN,
                                PinCode = (int)_SourceModel.PinCode,
                                CustomerType = _SourceModel.SelectCustomerType,
                                LandType = _SourceModel.LandType,
                                Acreage=_SourceModel.Acreage,
                                Crops=_SourceModel.Crops.ToList(),
                                Address = new SalesAddressModel()
                                {
                                    VillageId = _SourceModel.SelectedVillages.Id,
                                    Address=_SourceModel.Address
                                }

                            };

                            await _progressService.StartProgressAsync();
                            var _result = await _salesService.EditCustomer(editCustomer);
                            if (_result != null && _result.IsSuccess)
                            {
                                _notificationService.ShowMessage("Customer details updated successfully", NotificationType.Success);
                                //Object closeObj = Objdata[1];
                                //if (closeObj != null)
                                //    EditValidateCustomer(closeObj);
                                OldCustomerPhone = _SourceModel.Phone;
                                GetCustomerInfo();
                            }
                            else
                            {
                                _notificationService.ShowMessage(_result.Error, NotificationType.Error);
                                return;
                            }
                        }
                    
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            finally
            {
                await _progressService.StopProgressAsync();

            }
        }

        private void EditValidateCustomer(object obj)
        {
            var TargetClose = ((Button)(obj));
            var dynamicCommand = TargetClose.Command;
            dynamicCommand.CanExecute(true);            
        }

        private async void EditCustomer(object obj)
        {
            try
            {
                EditCustomerPopup editCustomerPopup = new EditCustomerPopup();
                var _vm = (EditCustomerPopupViewModel)editCustomerPopup.DataContext;
                if (_vm != null)
                {
                    _vm.Phone = FarmerPhone;
                    _vm.CustomerName = FarmerName;
                    _vm.CustomerTypes = CustomerTypes;
                    _vm.SelectCustomerType = CustomerType;
                    _vm.GSTIN = GSTNumber;
                    _vm.Address = FarmerAddress;
                    _vm.Acreage = Acreage;
                    _vm.Crops = Crops;
                    _vm.LandType = LandType;
                    if(LandType!=null && LandType.Count > 0)
                    {
                        foreach (var c in LandType)
                        {
                            if (c.ToLower() == "irrigation")
                                _vm.Irrigation = true;
                            if (c.ToLower() == "dry")
                                _vm.Dry = true;
                        }

                    }

                    if (PincodeDetailsInfo != null)
                    {
                        _vm.PinCode = Convert.ToInt32(PincodeDetailsInfo.Pincode);
                        _vm.States = new ObservableCollection<State>() { PincodeDetailsInfo.State };
                        _vm.Districts = new ObservableCollection<District>() { PincodeDetailsInfo.District };
                        _vm.Villages = new ObservableCollection<Village>(PincodeDetailsInfo.Villages);
                        _vm.SelectState = _vm.States.FirstOrDefault();
                        _vm.SelectDistrict = _vm.Districts.FirstOrDefault();
                        var _villageId = PincodeDetailsInfo.Villages.Where(x => x.Name == CustomerVillage).FirstOrDefault();
                        if (_villageId != null)
                        {
                            _vm.SelectedVillages = _villageId;
                        }
                        else
                            _vm.SelectedVillages = null;
                    }
                    
                    await DialogHost.Show(editCustomerPopup, "RootDialog", EditCustomerPopupEventClosing);

                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        private void EditCustomerPopupEventClosing(object sender, DialogClosingEventArgs eventArgs)
        {
        }

        private async void GetPinCodeDetails(Object PinCode)
        {
            try
            {
                if (PinCode != null)
                {
                    var _pinCode = Convert.ToInt32(PinCode);
                    if (_pinCode.ToString().Length == 6)
                    {

                        await Task.Run(async () =>
                        {
                            var _result = await _salesService.GetPincodeDetailsNew((int)PinCode);
                            if (_result != null && _result.IsSuccess && _result.Data != null)
                            {
                                States = new ObservableCollection<State>(new List<State>() { _result.Data.State });
                                SState = States.FirstOrDefault();

                                Districts = new ObservableCollection<District>(new List<District>() { _result.Data.District });
                                SDistrict = Districts.FirstOrDefault();

                                Villages = new ObservableCollection<Village>(_result.Data.Villages);
                            }

                        });
                    }
                    else
                    {
                        _notificationService.ShowMessage("Invalid pin code number", NotificationType.Error);
                        return;
                    }
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError("Getting error in download cheque", _ex);

            }
        }

        public void AppOrderCustomer(AppOrderModel model)
        {
            try
            {
                IsAppOrderCustomer = true;
                IsAppOrderEnable = true;
                AppOrderModel = model;
                OldCustomerPhone = model.Phone;
                SalesProducts.Clear();
                GetCustomerInfo();

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }

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

        private async void LoadInvoiceNumber()
        {

            SalesInvoiceNumber = null;

            var _result = await _salesService.GetSalesInvoiceNumber();

            if (_result != null && _result.IsSuccess)
            {
                SalesInvoiceNumber = _result.Data[0].Split(':').Last();
            }

        }

        private async void LoadStates()
        {
            try
            {
                await Task.Run(async () =>
                {
                    var _result = await _salesService.GetStates();

                    States = new ObservableCollection<State>(_result);
                });
            }
            catch (Exception _ex)
            {
                _logger.LogError("Getting error in download cheque", _ex);

            }
        }

        private async void LoadDistricts()
        {
            Districts = null;

            SDistrict = null;

            if (SState == null || SState.StateId <= 0) return;

            if (ApplicationSettings.CacheEnabled)
            {
                if (DistrictCache.Count() == 0)
                {
                    _eventAggregator.GetEvent<ResponseDictionaryCacheEvent>().Publish(new Common.Cache.RequestCacheModel
                    {
                        Id = CacheKeyEnum.Districts,
                    });
                }
                else
                {
                    Districts = new ObservableCollection<District>(DistrictCache.Where(xs => xs.StateId == SState.StateId));
                }
            }
            else
            {
                var _result = await _salesService.GetDistricts(SState.StateId);
                Districts = new ObservableCollection<District>(_result);
            }
        }

        //public bool UpdateCustomerAddress { get; set; }


        private bool _updateCustomerAddress;
        public bool UpdateCustomerAddress
        {
            get { return _updateCustomerAddress; }
            set { SetProperty(ref _updateCustomerAddress, value); }
        }

        private string _salesInvoiceNumber;
        public string SalesInvoiceNumber
        {
            get => _salesInvoiceNumber;
            set => SetProperty(ref _salesInvoiceNumber, value);
        }



        private string _remarks;
        public string Remarks
        {
            get => _remarks;
            set => SetProperty(ref _remarks, value);
        }


        private string _gstNumber;
        public string GSTNumber
        {
            get { return _gstNumber; }
            set { SetProperty(ref _gstNumber, value); }
        }

        private bool _isSpecialDiscount;
        public bool IsSpecialDiscount
        {
            get => _isSpecialDiscount;
            set
            {
                SetProperty(ref _isSpecialDiscount, value);
                if (!IsSpecialDiscount)
                {
                    SpecialDiscountAmount = 0;
                    Remarks = string.Empty;
                }
            }
        }


        private float _specialDiscountAmount;
        public float SpecialDiscountAmount
        {
            get => _specialDiscountAmount;
            set
            {
                SetProperty(ref _specialDiscountAmount, value);
                GrossTotal = GetProductsGrossTotal(SalesProducts); // SalesProducts.Sum(x => x.ProductSellingPrice);
                if (SpecialDiscountAmount > 0)
                    GrossTotal -= (IsSpecialDiscount ? SpecialDiscountAmount : 0);
            }

        }

        //private void CalculateSpecialDiscount()
        //{
        //    CalculateTotal();
        //    if (SpecialDiscountAmount > 0)
        //        NetTotal = NetTotal - SpecialDiscountAmount;
        //}

        private District _selectDistrict;
        public District SelectDistrict
        {
            get { return _selectDistrict; }
            set { SetProperty(ref _selectDistrict, value); }
        }

        private ObservableCollection<District> _districts;
        public ObservableCollection<District> Districts
        {
            get { return _districts; }
            set { SetProperty(ref _districts, value); }
        }


        private State _selectState;
        public State SelectState
        {
            get { return _selectState; }
            set { SetProperty(ref _selectState, value); }
        }

        private ObservableCollection<State> _states;
        public ObservableCollection<State> States
        {
            get { return _states; }
            set { SetProperty(ref _states, value); }
        }


        private String _customerVillage;

        public String CustomerVillage
        {
            get { return _customerVillage; }
            set { SetProperty(ref _customerVillage, value); }
        }



        private CancellationTokenSource _cancellationTokenSource;

        private async void GetCustomerInfo()
        {
            try
            {
                if (!OldCustomerPhone.IsValidPhone())
                {

                    _notificationService.ShowMessage("Phone number is invalid.", NotificationType.Error);
                    return;
                }

                if (_cancellationTokenSource != null)
                    _cancellationTokenSource.Cancel();

                _cancellationTokenSource = new CancellationTokenSource();

                _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                var _result = await _customerService.GetCustomerByPhone(OldCustomerPhone, _cancellationTokenSource.Token);

                if (_result != null)
                {
                    //apporder customer checking exists or not 
                    //if (IsAppOrderCustomer)
                    //{
                    //    IsOldCustomer = true;
                    //    OldCustomerPhone = AppOrderModel.Phone;

                    //}
                    IsOldCustomer = true;
                    FarmerName = _result.Name;
                    FarmerPhone = _result.Phone;
                    FarmerAddress = _result.Address.RAddress;
                    PinCode = _result.PinCode;
                    OldCustomerId = _result.CustomerId;
                    FarmerPhoneAlternate = _result.AlternatePhone;
                    City = _result.Address.City;
                    GSTNumber = _result.GSTNumber;
                    CustomerType = _result.CustomerType != null ? _result.CustomerType : "Store";
                    CustomerVillage = ((_result.Address.Village != null) ? _result.Address.Village.Name : (_result.Address.City));
                    TotalBillingAmount = _result.TotalBillingAmount;
                    try
                    {
                        SelectState = States.FirstOrDefault(x => x.StateId == _result.Address.StateID);

                        if (SelectState != null)
                        {
                            var _districts = await _salesService.GetDistricts(SelectState.StateId);

                            if (_districts != null && _districts.Any())
                            {

                                Districts = new ObservableCollection<District>(_districts);

                                SelectDistrict = Districts.FirstOrDefault(x => x.DistrictId == _result.Address.DistrictID);


                            }
                        }

                        Acreage = _result.Acreage;
                        Crops = _result.Crops!=null? new ObservableCollection<string>( _result.Crops.Split(',').Select(sValue => sValue.Trim()).ToList()):new ObservableCollection<string>();
                        LandType = _result.LandType!=null?(_result.LandType.Split(',').Select(sValue => sValue.Trim()).ToList()):new List<string>();
                        RemoveCode();
                    }
                    catch (Exception _ex)
                    {
                        _logger.LogError(_ex.Message);
                    }

                    PincodeDetailsInfo = _result.PincodeDetailsInfo;
                }
                else
                {

                    ResetAddCustomerDetails();
                    FPhone = OldCustomerPhone;
                    IsOldCustomer = false;
                    NewCustomerPopup newCustomerPopup = new NewCustomerPopup();
                    newCustomerPopup.DataContext = this;
                    await DialogHost.Show(newCustomerPopup, "RootDialog", NewCustomerPopupEventClosing);
                }
            }
            catch (TaskCanceledException _Ex)
            {
                _logger.LogError(_Ex.Message);
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);

            }
        }

        private void ResetSales()
        {
            SalesProducts?.Clear();
            InvoiceDate = DateTime.Now;
            IsOldCustomer = false;
            IsAppOrderCustomer = false;
            IsAppOrderEnable = false;
            IsSpecialDiscount = false;
            SpecialDiscountAmount = 0;
            ResetFormerDetails();
            Remarks = null;
            //GrossTotal = 0;
            //Discount = 0;
            //SGST = 0;
            //CGST = 0;
            //NetTotal = 0;
            //Cash = 0;
            //Card = 0;
            //CardNumber = string.Empty;
            //UPI = 0;
            //UPINumber = string.Empty;
            ResetPrice();
            LoadInvoiceNumber();
            ProductCount = "0";
            FarmerName = null;
            CouponCode = null;
            IsDiscountApplied = false;
            CouponId = null;
            BillingId = null;
            OTP = 0;
            StoreOTP = 0;
            IsCouponApplied = false;
            IsSaveBtnEnable = true;
        }

        private void ResetFormerDetails()
        {
            InvoiceDate = DateTime.Now;
            SelectDistrict = null;
            SelectState = null;
            FarmerName = null;
            GSTNumber = string.Empty;
            FarmerPhone = string.Empty;
            //FarmerAlternatePhone = string.Empty;
            City = string.Empty;
            InvoiceNumber = string.Empty;
            SelectedState = string.Empty;
            SelectedDistrict = string.Empty;
            PinCode = 0;
            OldCustomerId = 0;
            OldCustomerPhone = string.Empty;
            //Check.
            //IsOldCustomer = false;
            UpdateCustomerAddress = false;
            SelectedVillages = null;
            CustomerType = null;

        }

        private async void SaveSalesDetails()
        {   
            bool _isValidAppOrder = ValidateAppOrder();

            if (!_isValidAppOrder) return;

            bool _isValid = ValidateData();

            if (!_isValid) return;

            ShowSchemesCoupons();
        }

        private async void AddSales()
        {
            AddSalesInput = GetSalesDetails();


            if (AddSalesInput == null)
            {
                _notificationService.ShowMessage("An Error occurred, try again", NotificationType.Error);
                IsSaveBtnEnable = true;
                return;
            }

            if (OTP != 0 && (OTP != StoreOTP))
            {
                //RemoveCode();
                _notificationService.ShowMessage("Incorrect OTP entered", NotificationType.Error);
                IsSaveBtnEnable = true;
                IsEnableOTP = true;
                return;
            }
            await _progressService.StartProgressAsync();

            await Task.Run(async () =>
            {
                var result = await _salesService.AddSales(AddSalesInput);

                 if (result != null && result.success && result.data!=null)
                {


                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        _notificationService.ShowMessage("New Sales Added", NotificationType.Success);

                        _eventAggregator.GetEvent<DenominationPageRefreshEvent>().Publish();

                        if (IsAppOrderCustomer)
                        {
                            _eventAggregator.GetEvent<RefreshAppOrderPage>().Publish();
                        }


                        try
                        {
                            try
                            {
                                ResetSales();
                                if (AppConstants.Printer != null)
                                {
                                    ProcessStartInfo _p = new ProcessStartInfo();
                                    if (AppConstants.USER_ROLES[0].Contains("regionalmanager") || AppConstants.USER_ROLES[0].Contains("falcadirector") || AppConstants.USER_ROLES[0].Contains("territorymanager") || AppConstants.USER_ROLES[0].Contains("controlmanager") || AppConstants.USER_ROLES[0].Contains("purchasemanager") || AppConstants.USER_ROLES[0].Contains("finance"))
                                    {
                                        _p.UseShellExecute = true;
                                        _p.FileName = result.data.sign_tiny_response._A4;

                                    }
                                    else
                                    {
                                        _p.UseShellExecute = true;
                                        _p.FileName = AppConstants.Printer.ToLower() == "a4" ? result.data.sign_tiny_response._A4 : result.data.sign_tiny_response._4inch;
                                    }

                                    Process.Start(_p);
                                }
                                else
                                {
                                    _notificationService.ShowMessage("Please add printer type", NotificationType.Error);
                                    return;
                                }

                                //string path = @"c:\PosInvoices";

                                //var _info = Directory.CreateDirectory(path);

                                //var _fileName = path + "\\" + $"{result.Data.FileName}";


                                //if (!File.Exists(_fileName))
                                //{
                                //    File.WriteAllBytes(_fileName, result.Data.FileStream);
                                //    ProcessStartInfo _p = new ProcessStartInfo
                                //    {
                                //        UseShellExecute = true,
                                //        FileName = _fileName
                                //    };

                                //    Process.Start(_p);

                                //    return;
                                //}
                                //else
                                //{
                                //    //Fall back to save  if file name already present
                                //    SaveFileManually(result.Data.FileStream);
                                //}


                            }
                            catch (UnauthorizedAccessException _Ex)
                            {
                                // _progressService.StopProgressAsync();

                                //fall back to save manually if file creation is blocked.
                                _logger.LogError(_Ex.Message);
                                //SaveFileManually(result.Data.FileStream);
                            }
                            catch (Exception _ex)
                            {
                                // _progressService.StopProgressAsync();
                                _logger.LogError(_ex.Message);
                                IsSaveBtnEnable = true;
                                _notificationService.ShowMessage("Ann error occurred while showing invoice pdf", NotificationType.Error);

                            }

                        }
                        catch (Exception _ex)
                        {
                            _logger.LogError(_ex.Message);
                            IsSaveBtnEnable = true;
                            _notificationService.ShowMessage("Ann error occurred while showing invoice pdf", NotificationType.Error);

                        }


                        ResetSales();
                    });
                 }

            });

            await _progressService.StopProgressAsync();
        }

        private void SaveFileManually(byte[] stream)
        {
            try
            {
                SaveFileDialog _sd = new SaveFileDialog();

                //_sd.FileName = result.Data.FileName;
                _sd.AddExtension = true;
                _sd.DefaultExt = ".pdf";

                _sd.ShowDialog();

                if (_sd.FileName.IsValidString())
                {

                    File.WriteAllBytes(_sd.FileName, stream);

                    ProcessStartInfo _process = new ProcessStartInfo
                    {
                        UseShellExecute = true,
                        FileName = _sd.FileName
                    };
                    Process.Start(_process);

                }
            }
            catch (Exception _ex)
            {

                _logger.LogError(_ex.Message);
            }
        }

        private bool ValidateData()
        {
            //if (string.IsNullOrWhiteSpace(InvoiceNumber))
            //{
            //    _notificationService.ShowMessage("Enter invoice number", NotificationType.Error);
            //    return false;
            //}

            //Invoice date is selectable to today only
            //if (InvoiceDate == null)
            //{
            //    _notificationService.ShowMessage("Enter invoice Date", NotificationType.Error);
            //    return false;
            //}

            if (!FarmerPhone.IsValidPhone())
            {
                _notificationService.ShowMessage("Enter valid phone number", NotificationType.Error);
                IsSaveBtnEnable = true;
                return false;

            }

            if (!FarmerName.IsValidString())
            {
                _notificationService.ShowMessage("Enter valid customer name", NotificationType.Error);
                IsSaveBtnEnable = true;
                return false;

            }
            if (FarmerName.IsDigitsOnly())
            {
                _notificationService.ShowMessage("Numbers are not allowed", NotificationType.Error);
                IsSaveBtnEnable = true;
                return false;

            }



            if (GSTNumber.IsValidString() && !GSTNumber.IsValidGST())
            {
                _notificationService.ShowMessage("GST number is invalid", NotificationType.Error);
                IsSaveBtnEnable = true;
                return false;
            }

            //if (FarmerAlternatePhone.IsValidString())
            //{
            //    if (!FarmerAlternatePhone.IsValidPhone())
            //    {
            //        _notificationService.ShowMessage("Alternate phone number is invalid", NotificationType.Error);

            //        return false;
            //    }
            //}


            // as per 2.0 UI design no need to validate old customer details
            if (!IsOldCustomer)
            {
                if (SelectedVillages == null)
                {
                    _notificationService.ShowMessage("Please select village", NotificationType.Error);
                    IsSaveBtnEnable = true;
                    return false;

                }

                if (SelectState == null)
                {
                    _notificationService.ShowMessage("Select a state ", NotificationType.Error);
                    IsSaveBtnEnable = true;
                    return false;
                }

                if (SelectDistrict == null)
                {
                    _notificationService.ShowMessage("Select a district ", NotificationType.Error);
                    IsSaveBtnEnable = true;
                    return false;
                }

                if (PinCode == null)
                {
                    _notificationService.ShowMessage("Please enter  PinCode", NotificationType.Error);
                    IsSaveBtnEnable = true;
                    return false;

                }
                if (PinCode.ToString().Length != 6)
                {
                    _notificationService.ShowMessage("Please enter valid PinCode", NotificationType.Error);
                    IsSaveBtnEnable = true;
                    return false;

                }

            }

            if (SalesProducts == null || !SalesProducts.Any())
            {
                _notificationService.ShowMessage("Sales should have At least one product", NotificationType.Error);
                IsSaveBtnEnable = true;
                return false;
            }


            if (GrossTotal <= 0)
            {
                _notificationService.ShowMessage("Invalid gross total", NotificationType.Error);
                IsSaveBtnEnable = true;
                return false;

            }

            if (UPI <= 0 && Cash <= 0 && Card <= 0 && Cheque <= 0)
            {
                _notificationService.ShowMessage("Enter valid amount for UPI/Cash/Card", NotificationType.Warning);
                IsSaveBtnEnable = true;
                return false;
            }

            if (Cheque > 0 && (Card > 0 || Cash > 0 || UPI > 0))
            {

                _notificationService.ShowMessage("If this sales is  a cheque payment then cash/card/upi should be zero", NotificationType.Error);
                IsSaveBtnEnable = true;
                return false;

            }

            if (Cash >= 200000)
            {

                _notificationService.ShowMessage("Cash payment greater than or equal to 2Lakh per day/per invoice is not allowed in POS for single customers", NotificationType.Error);
                IsSaveBtnEnable = true;
                return false;

            }

            if (UPI > 0 && !UPINumber.IsValidString())
            {
                _notificationService.ShowMessage("Enter valid UPI number", NotificationType.Error);
                IsSaveBtnEnable = true;
                return false;

            }

            if (Cheque > 0 && !ChecqueNumber.IsValidString() && ISChequeFuture == false)
            {

                _notificationService.ShowMessage("Enter valid Cheque number", NotificationType.Error);
                IsSaveBtnEnable = true;
                return false;

            }

            if (Cheque > 0 && ChecqueNumber?.Length != 6 && ISChequeFuture == false)
            {

                _notificationService.ShowMessage("Enter valid Cheque number", NotificationType.Error);
                IsSaveBtnEnable = true;
                return false;

            }

            // card number is not the actual card number but the trasaction number for the card swiped /used.
            if (Card > 0 && !CardNumber.IsValidString())
            {
                _notificationService.ShowMessage("Enter valid card transaction number", NotificationType.Error);
                IsSaveBtnEnable = true;
                return false;

            }


            if (Cheque > 0)
            {
                if (!String.IsNullOrEmpty(CardNumber) || !String.IsNullOrEmpty(UPINumber))
                {
                    _notificationService.ShowMessage("If this sales is  a cheque payment then card number / upi number Should be empty", NotificationType.Error);
                    IsSaveBtnEnable = true;
                    return false;
                }


            }




            var _sum = Math.Round((float)(Card + Cash + UPI + Cheque), 2, MidpointRounding.AwayFromZero);


            if (_sum <= 0 || _sum != PayableAmount)
            {

                _notificationService.ShowMessage($"Invalid Total, Payable Amount is {PayableAmount} ", NotificationType.Error);
                IsSaveBtnEnable = true;
                return false;

            }

            if (IsSpecialDiscount && SpecialDiscountAmount <= 0)
            {
                _notificationService.ShowMessage("Invalid Special discount amount ", NotificationType.Error);
                IsSaveBtnEnable = true;
                return false;

            }


            return true;
        }

        private SalesModel GetSalesDetails()
        {
            var _adSales = new SalesModel();

            try
            {
                //_adSales.InvoiceNumber = InvoiceNumber;
                //Date maybe older at the time of sale from previous sale
                _adSales.GrossTotal = (GrossTotal-(float)TotalCouponAmount);
                _adSales.Discount = (float)TotalCouponAmount;
                _adSales.GST = GST;
                _adSales.SalesPayment = GetSalesPayment();
                _adSales.SalesProducts = GetSalesProducts(SalesProducts);
                //Check
                _adSales.DiscountType = !string.IsNullOrEmpty(CouponCode) ? "Flat": CouponSchemeData != null ? CouponSchemeData.All(coupon => coupon.Type == "FutureOffer") ? "FutureOffer" : "Flat": "Flat";
                _adSales.CouponCode = CouponCode;
                _adSales.CouponAmount = (float)TotalCouponAmount;
                _adSales.SalesType = "Sales";
                _adSales.Total = NetTotal;
                _adSales.CouponId = CouponId;
                _adSales.BillingId = BillingId;
                _adSales.AppliedCoupons = AppliedCoupon;
                _adSales.IsFutureOfferAlsoCreated = CouponSchemeData != null ? ((CouponSchemeData.Any(x=>x.Type.Contains("FutureOffer")) && ! IsCouponEnteredFromUI && ! CouponSchemeData.All(x=>x.Type.Contains("FutureOffer"))) ? true: false) :false;

                if (!IsOldCustomer)
                {
                    _adSales.IsOldCustomer = false;
                    _adSales.CustomerId = 0;
                    _adSales.CustomerDetails = GetCustomerDetails();
                }
                else
                {
                    _adSales.IsOldCustomer = true;
                    _adSales.CustomerId = OldCustomerId;
                    _adSales.UpdateCustomerAddress = UpdateCustomerAddress;
                    if (UpdateCustomerAddress)
                    {
                        _adSales.CustomerDetails = GetCustomerDetails();
                    }
                }

                // _adSales.Remarks = Remarks;


                if (IsSpecialDiscount)
                {
                    _adSales.IsSepcialDiscount = IsSpecialDiscount;
                    _adSales.SpecialDiscountAmount = SpecialDiscountAmount;
                }

                _adSales.TotalServiceCharges = TotalServiceCharge;
                if (IsAppOrderCustomer)
                {
                    _adSales.IsAppOrderCustomer = true;
                    _adSales.AppOrderId = AppOrderModel.AppOrderId;

                }
                else
                {
                    _adSales.IsAppOrderCustomer = false;
                    _adSales.AppOrderId = 0;
                }


            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in creating sales", _ex);

                return null;
            }



            return _adSales;
        }

        private List<SalesProductDTO> GetSalesProducts(ObservableCollection<SalesProductModel> _salesProducts)
        {
            var _Products = new List<SalesProductDTO>();
            var _falcaproductcount = _salesProducts.Where(x => x.Manufacturer.Name.ToLower() == "falca").Count();
            var _netCartTotal = _salesProducts.Where(x=>x.Manufacturer.Name.ToLower()=="falca").Sum(x => x.SellingQty * x.AcutalSellingPrice);
            foreach (var _prod in _salesProducts.ToList())
            {

                var _product = new SalesProductDTO
                {

                    Discount = _prod.Discount,
                    DiscountMode = _prod.DiscountMode,
                    BrandName = _prod.Manufacturer?.Name,
                    ProductGST = _prod.ProductGST,
                    ProductName = _prod.ProductName,
                    ProductSellingPrice = (_prod.ProductSellingPrice-_prod.ProductManualDiscount),
                    ProductTotal = _prod.ProductTotal,
                    ProductType = _prod.ProductType?.Name,
                    StockProductId = _prod.StockProductId,
                    WarrentyDate = _prod.WarrentyDate,
                    IsGroupTrackMode = _prod.IsGroupTrackMode,
                    SellingQty = _prod.SellingQty,
                    //Extra Discount
                    ExtraDiscount = _prod.ProductManualDiscount,
                    DiscountDetails = _prod.DiscountDetails,

                    //service charge
                    ServiceChargeAmount = (float)_prod.ServiceChargeAmount,
                    IsServiceChargeApplicable = _prod.IsServiceChargeApplicable,
                    SelectedbySKUProductId = _prod.SelectedbySKUProductId,
                    SKU = _prod.SKU,
                    BarCode = _prod.BarCode,

                };

                //if (IsDiscountApplied &&  _prod.Manufacturer?.Name.ToLower() == "falca")
                //{
                    
                //    if (_falcaproductcount == 1)
                //    {
                //        var _totalsellingpriceand_qty = _salesProducts.Select(x => x.SellingQty * x.ProductSellingPrice).Sum();
                //        var totalDiscount = (float)Math.Ceiling(_totalsellingpriceand_qty * 0.10);
                //        _product.ExtraDiscount = (((totalDiscount < 100 ? totalDiscount : 100)) / _prod.SellingQty);
                //        _product.ProductSellingPrice = (_product.ProductSellingPrice - _product.ExtraDiscount);
                //    }
                //    else
                //    {
                //        _product.ExtraDiscount = (((_prod.SellingQty * _prod.ProductSellingPrice) * 100) / _netCartTotal);
                //        _product.ProductSellingPrice = (_product.ProductSellingPrice - _product.ExtraDiscount);
                //    }
                // }

                _Products.Add(_product);
            }

            return _Products;
        }

        private SalesPaymentModel GetSalesPayment()
        {
            var _salesPayment = new SalesPaymentModel();

            _salesPayment.Card = Card;
            _salesPayment.CardNumber = CardNumber;
            _salesPayment.Cash = (float)Cash;
            _salesPayment.UPI = UPI;
            _salesPayment.UPINumber = UPINumber;
            _salesPayment.Cheque = Cheque;
            _salesPayment.ChequeNumber = ChecqueNumber;
            return _salesPayment;
        }

        private CustomerModel GetCustomerDetails()
        {
            try
            {
                var _customer = new CustomerModel
                {
                    //AlternatePhone = FarmerAlternatePhone,
                    Name = FarmerName,
                    PinCode = PinCode.Value,
                    Phone = FarmerPhone,
                    AlternatePhone = FarmerPhoneAlternate,
                    Address = GetCustomerAddress(),
                    GSTNumber = GSTNumber?.IsValidString() == false ? null : GSTNumber,
                };

                return _customer;


            }
            catch (Exception _ex)
            {

                _logger.LogError(_ex.Message);
            }

            return null;
        }

        private SalesAddressModel GetCustomerAddress()
        {
            var _address = new SalesAddressModel
            {

                //Check
                Alternatephone = FarmerPhoneAlternate,
                City = City,// "City",
                Email = null,
                Phone = FarmerPhone,
                District = SelectDistrict.Name,
                State = SelectState.Name,
                Street = null,
                DistrictID = SelectDistrict.DistrictId,
                StateID = SelectState.StateId,
                Pincode = PinCode,
                VillageId = SelectedVillages.Id,
            };

            return _address;
        }

        private void RemoveProduct(object obj)
        {
            var _product = obj as SalesProductModel;

            if (_product == null) return;

            SalesProducts.Remove(_product);

            //PriceCalculation();

            CalculateCouponDisount();

            CalculateTotal();

            ProductCount = (SalesProducts != null && SalesProducts.Count() > 0) ? (SalesProducts.Count() > 0 && SalesProducts.Count() < 10) ? ("0" + SalesProducts.Count()) : Convert.ToString(SalesProducts.Count()) : "0";

            IsDiscountApplied = false;

            OTP = 0;

            IsCouponApplied = false;


        }

        private void CalculateCouponDisount()
        {
            try
            {
                DiscountPayableAmount = Convert.ToDouble(this.SalesProducts?.Select(x => x.SellingQty * x.ProductSellingPrice).Sum());
                TotalCouponAmount = 0;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private async void GetProduct()
        {
            if (string.IsNullOrEmpty(ProductCode))
            {
                _notificationService.ShowMessage("Product code is required", NotificationType.Error);

                return;
            };


            //bool _res = int.TryParse(ProductCode, out int _productId);

            //if (!_res)
            //{
            //    _notificationService.ShowMessage("Enter valid product code", NotificationType.Error);

            //    return;
            //}
            SalesProduct _product = null;

            //if (_res) 
            { 
                //  if (_productId > 0)
                {
                    if (SalesProducts.Any(x => x.BarCode.Trim().ToLower() == ProductCode.Trim().ToLower()))
                    {
                        _notificationService.ShowMessage("Product is already added.", NotificationType.Error);

                        ProductCode = string.Empty;

                        return;
                    }

                    await Task.Run(async () =>
                    {
                        _product = await _salesService.GetStockProduct(ProductCode);

                    });

                    if (_product != null)
                    {
                        if (IsAppOrderCustomer)
                        {

                            if (AppOrderModel.Products.Where(x => x.SKU == _product.SKU).FirstOrDefault() == null)
                            {
                                _notificationService.ShowMessage("App Order SKU and Search Product SKU not matching.", NotificationType.Error);

                                return;
                            }
                            if (AppOrderModel.Products.Where(x => x.SKU == _product.SKU && x.SellingPrice == _product.ProductSellingPrice).FirstOrDefault() == null)
                            {
                                _notificationService.ShowMessage("App Order product selling price and search product selling price not matching.", NotificationType.Error);

                                return;
                            }
                        }


                        SalesProductModel model = GetSalesModel(_product);

                        model.PriceCalculateEvent += Model_PriceCalculateEvent;


                        SalesProducts.Add(model);

                        ProductCode = string.Empty;

                        // PriceCalculation();

                        CalculateTotal();

                        IsDiscountApplied = false;

                        IsCouponApplied = false;
                    }
                    ProductCode = string.Empty;

                    ProductCount = (SalesProducts != null && SalesProducts.Count() > 0) ? (SalesProducts.Count() > 0 && SalesProducts.Count() < 10) ? ("0" + SalesProducts.Count()) :Convert.ToString(SalesProducts.Count()) : "0";

                }

            }
        }

        private void Model_PriceCalculateEvent(object sender, Events.PriceCalculateEventArgs e)
        {

            CalculateTotal();
        }

        private void CalculateTotal()
        {
            try
            {

                if (SalesProducts != null && SalesProducts.Any())
                {

                    GrossTotal = GetProductsGrossTotal(SalesProducts);

                    Discount = GetProductsDiscountTotal(SalesProducts);

                    Cash = 0;
                    Card = 0;
                    UPI = 0;

                    GST = GetProductTotalGST(SalesProducts);


                    NetTotal = GrossTotal - GST + Discount;

                    //special discount
                    if (IsSpecialDiscount && SpecialDiscountAmount > 0)
                    {
                        GrossTotal = GrossTotal - SpecialDiscountAmount;
                    }

                    if (NetTotal < 0)
                    {

                        NetTotal = 0;

                    }
                    
                    //calculate total service charges.
                    TotalServiceCharge = Math.Round(SalesProducts.Sum(x => (decimal)x.ServiceChargeAmount * x.SellingQty), 2, MidpointRounding.AwayFromZero);

                    if (IsDiscountApplied)
                    {
                        DiscountPayableAmount = 0;
                        IsDiscountApplied = false;
                        OTP = 0;
                        StoreOTP = 0;
                        CouponCode = null;
                    }
                    else
                        DiscountPayableAmount = 0;
                    PayableAmount = Math.Round((((GrossTotal) + (float)TotalServiceCharge) -DiscountPayableAmount), 2, MidpointRounding.AwayFromZero);

                }
                else
                {
                    ResetPrice();
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private float GetProductTotalGST(ObservableCollection<SalesProductModel> salesProducts)
        {
            float _tGst = 0.0F;


            foreach (var _prod in salesProducts)
            {
                if (_prod == null) continue;

                if (!_prod.IsGroupTrackMode)
                {
                    //Math.Round(_chargers, 2, MidpointRounding.AwayFromZero);
                    var _withOutGSt = (float)Math.Round(_prod.ProductSellingPrice * 100 / (100 + _prod.ProductGST), 2, MidpointRounding.AwayFromZero);

                    var _gstAmount = (float)Math.Round(_prod.ProductSellingPrice - _withOutGSt, 2, MidpointRounding.AwayFromZero);  //(x.ProductSellingPrice-x.Discount) * (x.ProductGST / 100);

                    if (_gstAmount < 0)
                    {
                        _gstAmount = 0;
                    }

                    _tGst += _gstAmount;
                }
                else
                {
                    var _withOutGSt = (float)Math.Round(_prod.ProductSellingPrice * 100 / (100 + _prod.ProductGST), 2, MidpointRounding.AwayFromZero);

                    var _gstAmount = (float)Math.Round((_prod.ProductSellingPrice - _withOutGSt) * _prod.SellingQty, 2, MidpointRounding.AwayFromZero);  //(x.ProductSellingPrice-x.Discount) * (x.ProductGST / 100);



                    if (_gstAmount < 0)
                    {
                        _gstAmount = 0;
                    }

                    _tGst += _gstAmount;

                }


            }


            return _tGst;
        }

        private float GetProductsDiscountTotal(ObservableCollection<SalesProductModel> salesProducts)
        {
            float _discountTotal = 0.0F;

            foreach (var prod in salesProducts)
            {

                if (prod == null) continue;

                if (!prod.IsGroupTrackMode)
                {
                    //single product
                    _discountTotal += (prod.Discount + prod.ProductManualDiscount);
                }
                else
                {
                    //group product 

                    _discountTotal += ((prod.Discount + prod.ProductManualDiscount) * prod.SellingQty);
                }

            }

            return _discountTotal;
        }

        private float GetProductsGrossTotal(ObservableCollection<SalesProductModel> salesProducts)
        {

            float _gTotal = 0.0F;

            foreach (var prod in salesProducts)
            {

                if (prod == null) continue;

                if (!prod.IsGroupTrackMode)
                {
                    //single product
                    _gTotal += prod.ProductSellingPrice;
                }
                else
                {
                    //group product 

                    _gTotal += (prod.ProductSellingPrice * prod.SellingQty);
                }

            }

            return _gTotal;
        }

        private void ResetPrice()
        {
            GST = 0;
            NetTotal = 0;
            GrossTotal = 0;
            Discount = 0;
            Card = 0;
            Cash = 0;
            UPI = 0;
            CardNumber = string.Empty;
            UPINumber = string.Empty;
            IsSpecialDiscount = false;
            SpecialDiscountAmount = 0;
            TotalServiceCharge = 0;
            PayableAmount = 0;
            TotalCouponAmount = 0;
            ChecqueNumber = null;
            ISChequeFuture = false;
            Cheque = 0;
            FarmerPhoneAlternate = null;
        }

        private SalesProductModel GetSalesModel(SalesProduct product)
        {

            if (product == null) return null;

            var _salesModel = new SalesProductModel(_notificationService);

            //_salesModel.Discount = product.Discount;
            _salesModel.Manufacturer = product.Manufacturer;
            _salesModel.ProductType = product.ProductType;
            _salesModel.ProductType.CategoryName = product.ProductType.CategoryName;
            _salesModel.WarrentyDate = product.WarrentyDate;
            _salesModel.ProductName = product.ProductName;
            _salesModel.ProductSpecifications = product.ProductSpecifications;
            _salesModel.DiscountMode = product.DiscountMode;
            _salesModel.ProductSellingPrice = product.ProductSellingPrice;
            _salesModel.ProductTotal = product.ProductTotal;
            _salesModel.StockProductId = product.StockProductId;
            _salesModel.ProductGST = product.ProductGST;
            //_salesModel.ProductDiscountPercent = product.ProductDiscountPercent;
            //_salesModel.ProductDiscountFlat = product.ProductDiscountFlat;
            _salesModel.BarCode = product.BarCode;
            _salesModel.ProductRate = product.ProductRate;
            _salesModel.IsGroupTrackMode = product.IsGroupTrackMode;
            _salesModel.AvailableQuantity = product.AvailableQuantity;
            _salesModel.ExpiryProductCount = product.ExpiryProductCount;
            _salesModel.ProductId = product.ProductId;
            if (IsAppOrderCustomer)
            {
                _salesModel.IsSellingQty = false;
                _salesModel.SellingQty = AppOrderModel.Products.Where(x => x.SKU == product.SKU).FirstOrDefault().OrderQty;
            }
            else
            {
                _salesModel.IsSellingQty = true;
                _salesModel.SellingQty = 1;
            }


            //set base selling price
            _salesModel.AcutalSellingPrice = product.ProductSellingPrice;

            //service charge applicable ?
            _salesModel.IsServiceChargeApplicable = product.IsServiceChargeApplicable;
            _salesModel.SKU = product.SKU;
            _salesModel.SelectedbySKUProductId = product.SelectedProduct == true ? product.StockProductId : 0;

            return _salesModel;

        }


        //public void TotalCalculation()
        //{
        //    try
        //    {

        //        var Total = GrossTotal - Discount;
        //        //GST = SGST + CGST;
        //        //var sgstAmount = SgstCalculation(Total, SGST);
        //        //var cgstAmount = CgstCalculation(Total, CGST);
        //        //var GstAmount = Convert.ToInt32(sgstAmount) + Convert.ToInt32(cgstAmount);
        //        //NetTotal = Total + GstAmount;
        //        //var Balance = NetTotal - (Cash + Card + UPI);

        //    }
        //    catch (Exception)
        //    {


        //    }
        //}

        //private Decimal SgstCalculation(float total, float sgst)
        //{
        //    var SgstAmount = total * sgst / 100;
        //    return Convert.ToDecimal(Math.Round(SgstAmount, MidpointRounding.AwayFromZero));
        //}

        //private Decimal CgstCalculation(float total, float cgst)
        //{
        //    var CgstAmount = total * cgst / 100;
        //    return Convert.ToDecimal(Math.Round(CgstAmount, MidpointRounding.AwayFromZero));
        //}

        //public void PriceCalculation()
        //{
        //    try
        //    {
        //        if (SalesProducts != null && SalesProducts.Count > 0)
        //        {
        //            GrossTotal = 0;
        //            NetTotal = 0;
        //            foreach (var _product in SalesProducts)
        //            {

        //                GrossTotal += _product.ProductTotal;




        //                //decimal discountAmount = sales.ProductDiscountFlat != 0 ? Convert.ToDecimal(sales.ProductDiscountFlat) : sales.ProductDiscountPercent != 0 ? GetDiscountAmountFromPercentage(sales.ProductSellingPrice, sales.ProductDiscountPercent) : 0;
        //                //var total = sales.ProductSellingPrice - Convert.ToInt32(discountAmount);
        //                //decimal GSTAmount = sales.ProductGST != 0 ? GetGSTCalculations(total, sales.ProductGST) : 0;
        //                ////sales.ProductTotal = total + Convert.ToInt32(GSTAmount);
        //                //GrossTotal += sales.ProductTotal;
        //                //NetTotal += sales.ProductTotal;
        //            }
        //        }
        //        else
        //        {
        //            GrossTotal = 0;
        //            NetTotal = 0;
        //            UPI = 0;
        //            Cash = 0;
        //            Card = 0;
        //            GST = 0;
        //            CGST = 0;
        //            SGST = 0;
        //            Discount = 0;


        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }


        //}

        private decimal GetGSTCalculations(float total, float gst)
        {
            try
            {
                var GstAmount = total * gst / 100;
                return Convert.ToDecimal(Math.Round(GstAmount, MidpointRounding.AwayFromZero));
            }
            catch (Exception)
            {

                //throw;
            }

            return default(decimal);
        }
        private decimal GetDiscountAmountFromPercentage(float sellingPrice, float percent)
        {
            try
            {

                var total = sellingPrice * percent / 100;
                return Convert.ToDecimal(Math.Round(total, MidpointRounding.AwayFromZero));
            }
            catch (Exception)
            {
                return 0;

            }
        }


        #region Props

        private int _oldCustomerId;
        public int OldCustomerId
        {
            get => _oldCustomerId;
            set => SetProperty(ref _oldCustomerId, value);
        }


        private bool _isOldCustomer;
        public bool IsOldCustomer
        {
            get { return _isOldCustomer; }
            set
            {
                SetProperty(ref _isOldCustomer, value);
                ResetFormerDetails();
            }
        }

        private string _oldCustomerPhone;
        public string OldCustomerPhone
        {
            get => _oldCustomerPhone;
            set => SetProperty(ref _oldCustomerPhone, value);
        }

        private string _productCode;
        public string ProductCode
        {
            get => _productCode;
            set => SetProperty(ref _productCode, value);
        }


        private string _productSKU;
        public string ProductSKU
        {
            get => _productSKU;
            set => SetProperty(ref _productSKU, value);
        }


        private ObservableCollection<SalesProductModel> _salesProducts = new ObservableCollection<SalesProductModel>();
        public ObservableCollection<SalesProductModel> SalesProducts
        {
            get => _salesProducts;
            set => SetProperty(ref _salesProducts, value);

        }

        private string _invoiceNumber;
        public string InvoiceNumber
        {
            get => _invoiceNumber;
            set => SetProperty(ref _invoiceNumber, value);
        }


        private string _farmerName;
        public string FarmerName
        {
            get => _farmerName;
            set => SetProperty(ref _farmerName, value);
        }

        private string _farmerPhone;
        public string FarmerPhone
        {
            get { return _farmerPhone; }
            set
            {
                if (value != _farmerPhone)
                {
                    SetProperty(ref _farmerPhone, value);

                    if (!IsOldCustomer)
                    {
                        ValidatePhoneNumber(value);
                    }

                }
            }
        }

        private string _farmerAddress;

        public string FarmerAddress
        {
            get { return _farmerAddress; }
            set => SetProperty(ref _farmerAddress, value);
        }


        private string _farmerPhoneAlternate;
        public string FarmerPhoneAlternate
        {
            get { return _farmerPhoneAlternate; }
            set
            {
                if (value != _farmerPhoneAlternate)
                {
                    SetProperty(ref _farmerPhoneAlternate, value);

                }


            }
        }




        private async void ValidatePhoneNumber(string phone)
        {

            try
            {

                //invalid phone number
                if (!phone.IsValidPhone()) return;


                if (_cancellationTokenSource != null)
                    _cancellationTokenSource.Cancel();


                _cancellationTokenSource = new CancellationTokenSource();

                await Task.Run(async () =>
                {

                    var _result = await _salesService.GetCustomerByPhone(phone, _cancellationTokenSource.Token);

                    if (_result != null && !_result.IsSuccess)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _notificationService.ShowMessage(_result.Error, NotificationType.Error);
                        });
                    }

                }, _cancellationTokenSource.Token);
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in checking customer phone number", _ex);
            }
        }



        private async void GetProductSKUSearch()
        {
            if (string.IsNullOrEmpty(ProductSKU))
            {
                _notificationService.ShowMessage("Product SKU is required", NotificationType.Error);

                return;
            };


            IEnumerable<SalesProduct> _product = null;


            if (SalesProducts.Any(x => x.SKU.Trim().ToLower() == ProductSKU.Trim().ToLower()))
            {
                _notificationService.ShowMessage("Product is already added.", NotificationType.Error);

                ProductSKU = string.Empty;

                return;
            }

            await Task.Run(async () =>
            {
                _product = await _salesService.GetStockProductSKUSearch(ProductSKU);

            });

            if (_product != null && _product.Count() > 0)
            {

                ProductSKUSearchPopup productSKUSearch = new ProductSKUSearchPopup();
                productSKUSearch.DataContext = this;

                var groupBySellingProducts = (from list in _product
                                              group list by list.ProductSellingPrice into g
                                              select new { sellingprice = g.Key, availabeqty = g.Sum(x => x.AvailableQuantity), productlist = g.FirstOrDefault() });

                List<SalesProduct> products = new List<SalesProduct>();
                if (groupBySellingProducts != null && groupBySellingProducts.Count() > 0)
                {
                    foreach (var item in groupBySellingProducts)
                    {
                        products.Add(new SalesProduct()
                        {
                            SKU = item.productlist.SKU,
                            ProductName = item.productlist.ProductName,
                            Manufacturer = item.productlist.Manufacturer,
                            ProductType = item.productlist.ProductType,
                            AvailableQuantity = item.availabeqty,
                            ProductSellingPrice = item.productlist.ProductSellingPrice,
                            Discount = item.productlist.Discount,
                            WarrentyDate = item.productlist.WarrentyDate,
                            ProductSpecifications = item.productlist.ProductSpecifications,
                            DiscountMode = item.productlist.DiscountMode,
                            ProductTotal = item.productlist.ProductTotal,
                            StockProductId = item.productlist.StockProductId,
                            ProductGST = item.productlist.ProductGST,
                            ProductDiscountPercent = item.productlist.ProductDiscountPercent,
                            ProductDiscountFlat = item.productlist.ProductDiscountFlat,
                            BarCode = item.productlist.BarCode,
                            IsGroupTrackMode = item.productlist.IsGroupTrackMode,
                            SellingQty = 1,
                            IsServiceChargeApplicable = item.productlist.IsServiceChargeApplicable,


                        });

                    }

                }



                SkuSearchProduct = products;

                await DialogHost.Show(productSKUSearch, "RootDialog", PopupClosingEventHandler);

            }




        }



        private void PopupClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {
                var viewModel = (SalesViewModel)eventArgs.Parameter;
                if (viewModel != null)
                {
                    var _selectedProduct = SkuSearchProduct.FirstOrDefault(x => x.SelectedProduct == true);

                    if (_selectedProduct != null)
                    {
                        SalesProductModel model = GetSalesModel(_selectedProduct);

                        model.PriceCalculateEvent += Model_PriceCalculateEvent;


                        SalesProducts.Add(model);

                        ProductSKU = string.Empty;

                        // PriceCalculation();

                        CalculateTotal();
                    }
                }




            }
            catch (Exception ex)
            {
                _logger.LogError("update closing event getting error", ex);
            }
        }


        public void updateProduct(object obj)
        {
            try
            {
                var _selectSKU = SkuSearchProduct.FirstOrDefault(x => x.SelectedProduct == true);
                if (_selectSKU == null)
                {
                    _notificationService.ShowMessage("Please select any one product", NotificationType.Error);

                    return;
                }


                var TargetClose = ((Button)(obj));
                var dynamicCommand = TargetClose.Command;
                dynamicCommand.CanExecute(true);
                //pass Model as Arguments
                dynamicCommand.Execute(this);
            }
            catch (Exception ex)
            {
                _logger.LogError("update product closing event getting error", ex);
            }
        }

        private string _city;
        public string City
        {
            get => _city;
            set => SetProperty(ref _city, value);
        }

        //private string _farmerAlternatePhone;
        //public string FarmerAlternatePhone
        //{
        //    get { return _farmerAlternatePhone; }
        //    set { SetProperty(ref _farmerAlternatePhone, value); }
        //}

        private DateTime? _invoiceDate;
        public DateTime? InvoiceDate
        {
            get => _invoiceDate = DateTime.Now;
            set => SetProperty(ref _invoiceDate, value);
        }

        private string _selectedState;
        public string SelectedState
        {
            get => _selectedState;
            set => SetProperty(ref _selectedState, value);
        }

        private string _selectedDistrict;
        public string SelectedDistrict
        {
            get => _selectedDistrict;
            set => SetProperty(ref _selectedDistrict, value);
        }

        private int? _pinCode;
        public int? PinCode
        {
            get => _pinCode;
            set => SetProperty(ref _pinCode, value);
        }


        private float _grossTotal;
        public float GrossTotal
        {
            get => _grossTotal;
            set => SetProperty(ref _grossTotal, value);

        }

        private float _discount;
        public float Discount
        {
            get => _discount;
            set => SetProperty(ref _discount, value);
        }


        //private float _sGST;
        //public float SGST
        //{
        //    get { return _sGST; }
        //    set { SetProperty(ref _sGST, value); }
        //}

        //private float _cGST;
        //public float CGST
        //{
        //    get { return _cGST; }
        //    set { SetProperty(ref _cGST, value); }
        //}


        private float _gSt;
        public float GST
        {
            get { return _gSt; }
            set { SetProperty(ref _gSt, value); }
        }

        private float _netTotal;
        public float NetTotal
        {
            get { return _netTotal; }
            set { SetProperty(ref _netTotal, value); }
        }


        private double _cash;
        public double Cash
        {
            get { return _cash; }
            set { SetProperty(ref _cash, value); }
        }


        private float _card;
        public float Card
        {
            get { return _card; }
            set { SetProperty(ref _card, value); }
        }


        private float _upi;
        public float UPI
        {
            get { return _upi; }
            set { SetProperty(ref _upi, value); }
        }

        private float _cheque;
        public float Cheque
        {
            get { return _cheque; }
            set { SetProperty(ref _cheque, value); }
        }


        private string _cardNumber;
        public string CardNumber
        {
            get { return _cardNumber; }
            set { SetProperty(ref _cardNumber, value); }
        }

        private string _upiNumber;
        public string UPINumber
        {
            get { return _upiNumber; }
            set { SetProperty(ref _upiNumber, value); }
        }

        private string _chequeNumber;
        public string ChecqueNumber
        {
            get { return _chequeNumber; }
            set { SetProperty(ref _chequeNumber, value); }
        }


        private decimal _totalServiceCharge;
        public decimal TotalServiceCharge
        {
            get { return _totalServiceCharge; }
            set
            {
                if (value < 0)
                {
                    //block -ve values
                    value = 0;
                }
                SetProperty(ref _totalServiceCharge, value);


            }
        }

        private double _payableAmount;
        public double PayableAmount
        {
            get { return _payableAmount; }
            set { SetProperty(ref _payableAmount, value); }
        }

        private double _disountpayableAmount;
        public double DiscountPayableAmount
        {
            get { return _disountpayableAmount; }
            set { SetProperty(ref _disountpayableAmount, value); }
        }

        #endregion

        private double _maxHeight;

        public double MaxHeight
        {
            get { return _maxHeight; }
            set { SetProperty(ref _maxHeight, value); }
        }

        private bool _ischequefuture;
        public bool ISChequeFuture
        {
            get { return _ischequefuture; }
            set
            {


                if (value == true)
                {
                    ChecqueNumber = null;
                }
                SetProperty(ref _ischequefuture, value);
            }
        }

        private List<SalesProduct> _SkuSearchProduct;

        public List<SalesProduct> SkuSearchProduct
        {
            get => _SkuSearchProduct;
            set => SetProperty(ref _SkuSearchProduct, value);
        }

        private bool _isAppOrderCustomer;
        public bool IsAppOrderCustomer
        {
            get => _isAppOrderCustomer;
            set => SetProperty(ref _isAppOrderCustomer, value);

        }

        private AppOrderModel _appOrderModel;
        public AppOrderModel AppOrderModel
        {
            get => _appOrderModel;
            set => SetProperty(ref _appOrderModel, value);

        }

        private bool _isAppOrderEnable;
        public bool IsAppOrderEnable
        {
            get => _isAppOrderEnable;
            set => SetProperty(ref _isAppOrderEnable, value);

        }

        public void ResetProductDetails()
        {
            SalesProducts.Clear();
            ResetSales();
            ResetPrice();
            IsOldCustomer = false;
        }

        public bool ValidateAppOrder()
        {
            if (IsAppOrderCustomer)
            {
                if (SalesProducts.Count != AppOrderModel?.Products.Count)
                {
                    _notificationService.ShowMessage("App Order product count and Sales product count not matching", NotificationType.Error);
                    IsSaveBtnEnable = true;
                    return false;
                }
                foreach (var app in AppOrderModel.Products)
                {

                    var sales = SalesProducts.Where(x => x.SKU == app.SKU).FirstOrDefault();
                    if (sales != null)
                    {
                        if (app.OrderQty != sales.SellingQty)
                        {
                            _notificationService.ShowMessage(sales.SKU + " App Order qty and Sales product qty not matching", NotificationType.Error);
                            IsSaveBtnEnable = true;
                            return false;
                        }
                        if (app.SellingPrice != sales.AcutalSellingPrice)
                        {
                            _notificationService.ShowMessage(sales.SKU + " App Order SellingPrice and Sales product SellingPrice not matching", NotificationType.Error);
                            IsSaveBtnEnable = true;
                            return false;
                        }
                    }


                }
            }

            return true;
        }

        public void AppOrderCustomer()
        {
            if (!IsAppOrderCustomer)
                IsAppOrderEnable = false;
            ResetProductDetails();
        }

        public async void GetDenominationVerify()
        {
            try
            {

                var _result = await _denominationService.GetDenominationVerify();
                if (_result != null && _result.IsSuccess)
                {
                    _logger.LogInformation(_result.Data.Data);
                    IsDenominationSuccess = true;
                    DenominationErrorMsg = null;

                }
                else
                {
                    _logger.LogInformation(_result?.Data?.Error);
                    _notificationService.ShowMessage(_result?.Data?.Error, NotificationType.Error);
                    IsDenominationSuccess = false;
                    DenominationErrorMsg = _result?.Data?.Error;
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);

            }
        }

        public void SalePageRefresh(DenominationVerifyModel denominationVerify)
        {
            try
            {
                IsDenominationSuccess = denominationVerify != null ? denominationVerify.IsSuccess : false;
                DenominationErrorMsg = denominationVerify?.Error;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private bool _isDenominationSuccess;

        public bool IsDenominationSuccess
        {
            get => _isDenominationSuccess;
            set => SetProperty(ref _isDenominationSuccess, value);
        }

        private string _denominationErrorMsg;

        public string DenominationErrorMsg
        {
            get => _denominationErrorMsg;
            set => SetProperty(ref _denominationErrorMsg, value);
        }



        private ObservableCollection<Village> _villages;

        public ObservableCollection<Village> Villages
        {
            get => _villages;
            set => SetProperty(ref _villages, value);
        }

        private Village _selectedVillages;

        public Village SelectedVillages
        {
            get => _selectedVillages;
            set => SetProperty(ref _selectedVillages, value);
        }

        //public async void GetVillage() {
        //    try {


        //        var _result = await _salesService.GetVillage();

        //        if (_result != null && _result.IsSuccess) {

        //            Villages = _result.Data.ToList();
        //        }

        //    }
        //    catch (Exception _ex) {
        //        _logger.LogError(_ex.Message);
        //    }
        //}

        private string _productCount;

        public string ProductCount
        {
            get => _productCount;
            set => SetProperty(ref _productCount, value);
        }

        private async void NewCustomerPopupEventClosing(object sender, DialogClosingEventArgs eventArg)
        {
            try
            {
                var _viewModel = (SalesViewModel)eventArg.Parameter;
                //var _view = (((Grid)(((Border)((((StackPanel)((NewCustomerPopup)((((DialogHost)eventArg.OriginalSource)).DialogContent)).Content).Children)[0])).Child)).Children)[2];
                if (_viewModel != null)
                {

                    var _customer = new CustomerModel()
                    {
                        Phone = FPhone,
                        Name = FName,
                        GSTNumber = GNumber,
                        CustomerType = SCustomerType,
                        Acreage = Acreage,
                        Crops = Crops.ToList(),
                        LandType=LandType,
                        Address = new SalesAddressModel()
                        {
                            State = SState.Name,
                            District = SDistrict.Name,
                            DistrictID = SDistrict.DistrictId,
                            StateID = SState.StateId,
                            Phone = FPhone,
                            VillageId = SVillages.Id,
                            Pincode = PCode,
                            Address=CAddress

                        }
                    };
                    await _progressService.StartProgressAsync();

                    await Task.Run(async () =>
                    {
                        var result = await _salesService.AddCustomer(_customer);

                        if (result != null && result.IsSuccess)
                        {
                            ResetAddCustomerDetails();
                            _notificationService.ShowMessage(result.Data, NotificationType.Success);  
                            return;
                        }
                        else
                        {
                            _notificationService.ShowMessage(result.Error, NotificationType.Error);
                            return;
                        }

                    });
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

        public void SaveCustomer(object obj)
        {
            try
            {                 
                if (string.IsNullOrEmpty(FPhone))
                {
                    _notificationService.ShowMessage("Please enter farmer Phone number", NotificationType.Error);
                    return;
                }
                if (!FPhone.IsValidPhone())
                {
                    _notificationService.ShowMessage("Phone number is invalid.", NotificationType.Error);
                    return;
                }

                if (string.IsNullOrEmpty(FName))
                {
                    _notificationService.ShowMessage("Please enter customer name", NotificationType.Error);
                    return;
                }
                if (string.IsNullOrEmpty(SCustomerType))
                {
                    _notificationService.ShowMessage("Please select custselect customer type", NotificationType.Error);
                    return;
                }
                if (PCode == 0) {
                    _notificationService.ShowMessage("Please enter PinCode", NotificationType.Error);
                    return;
                }
                if (SState == null)
                {
                    _notificationService.ShowMessage("Please select state", NotificationType.Error);
                    return;
                }
                if (SDistrict == null)
                {
                    _notificationService.ShowMessage("Please select district", NotificationType.Error);
                    return;
                }

                if (SVillages == null)
                {
                    _notificationService.ShowMessage("Please select village", NotificationType.Error);
                    return;
                }
                if (SCustomerType != null)
                {
                    if (SCustomerType == "FPO")
                    {
                        if (string.IsNullOrEmpty(GNumber))
                        {
                            _notificationService.ShowMessage("Please enter GST number", NotificationType.Error);
                            return;
                        }
                        else
                        {
                            if (GNumber.IsValidString() && !GNumber.IsValidGST())
                            {
                                _notificationService.ShowMessage("GST number is invalid", NotificationType.Error);

                                return;
                            }

                        }
                    }

                    if (SCustomerType.ToLower() == "store" || SCustomerType.ToLower() == "b2b" || SCustomerType.ToLower() == "rsp")
                    {
                        if (!string.IsNullOrEmpty(GNumber))
                        {
                            if (GNumber.IsValidString() && !GNumber.IsValidGST())
                            {
                                _notificationService.ShowMessage("GST number is invalid", NotificationType.Error);
                                return;
                            }
                        }
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


        private List<string> _customerTypes;

        public List<string> CustomerTypes
        {
            get => _customerTypes;
            set => SetProperty(ref _customerTypes, value);
        }

        private string _customerType;

        public string CustomerType
        {
            get => _customerType;
            set => SetProperty(ref _customerType, value);
        }


        private string _fName;
        public string FName
        {
            get => _fName;
            set => SetProperty(ref _fName, value);
        }

        private string _fPhone;
        public string FPhone
        {
            get { return _fPhone; }
            set
            {
                if (value != _fPhone)
                {
                    SetProperty(ref _fPhone, value);

                    if (!IsOldCustomer)
                    {
                        ValidatePhoneNumber(value);
                    }

                }


            }
        }

        private Village _sVillages;

        public Village SVillages
        {
            get => _sVillages;
            set => SetProperty(ref _sVillages, value);
        }


        private string _cAddress;
        public string CAddress
        {
            get => _cAddress;
            set => SetProperty(ref _cAddress, value);
        }


        private State _sState;
        public State SState
        {
            get { return _sState; }
            set { SetProperty(ref _sState, value); }
        }

        private string _sCustomerType;

        public string SCustomerType
        {
            get => _sCustomerType;
            set => SetProperty(ref _sCustomerType, value);
        }


        private District _sDistrict;
        public District SDistrict
        {
            get { return _sDistrict; }
            set { SetProperty(ref _sDistrict, value); }
        }

        private string _gNumber;
        public string GNumber
        {
            get { return _gNumber; }
            set { SetProperty(ref _gNumber, value); }
        }

        private int? _pCode;
        public int? PCode
        {
            get => _pCode;
            set => SetProperty(ref _pCode, value);
        }

        private PincodeDetails _pincodedetails;

        public PincodeDetails PincodeDetailsInfo
        {
            get { return _pincodedetails; }
            set { SetProperty(ref _pincodedetails, value); }
        }

        private void ResetAddCustomerDetails()
        {
            try
            {

                FName = string.Empty;
                GSTNumber = string.Empty;
                FPhone = string.Empty;
                //FarmerAlternatePhone = string.Empty;
                CAddress = string.Empty; 
                SState = null;
                SDistrict = null;
                PCode = 0;
                SVillages = null;
                SCustomerType = null;
                GNumber = null;
                Acreage = 0;
                Crops = new ObservableCollection<string>();
                LandType= new List<string>();
                Crop = null;
                TotalBillingAmount = 0;
                StoreOTP = 0;
                RemoveCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }


        }
        private String _couponcode;
        public String CouponCode
        {
            get { return _couponcode; }
            set { SetProperty(ref _couponcode, value); }
        }


        private bool _iscouponapplied;

        public bool IsCouponApplied
        {
            get { return _iscouponapplied; }
            set { SetProperty(ref _iscouponapplied, value); }
        }

        private double _totalcouponamount;

        public double TotalCouponAmount
        {
            get { return _totalcouponamount; }
            set
            {
                SetProperty(ref _totalcouponamount, value);
                //DiscountPayableAmount = value != 0 ? SalesProducts.Select(x => x.SellingQty * x.ProductSellingPrice).Sum() : 0;

            }
        }


        private bool _isDiscountApplied;

        public bool IsDiscountApplied
        {
            get { return _isDiscountApplied; }
            set { SetProperty(ref _isDiscountApplied, value);
                if (!value)
                {
                    DiscountPayableAmount = 0;
                }
            }
        }



        public  async void ApplyCode(object obj) {
            try {

                CouponSchemeData = new List<CouponsSchemes>();
                IsOnlyFutureOffersCreated = false;

                StoreOTP = 0;
                if (this.SalesProducts?.Count == 0)
                {
                    _notificationService.ShowMessage("Please add the products", NotificationType.Error);
                    return;
                }

                if (String.IsNullOrEmpty(FarmerName) && String.IsNullOrEmpty(FarmerPhone))
                {
                    _notificationService.ShowMessage("Please add a customer", NotificationType.Error);
                    return;
                }
              
                var ProductList = new List<Products>();
                foreach (var item in SalesProducts)
                {
                    var product = new Products()
                    {
                        Id = Convert.ToInt32(item.SKU),
                        Name = item.ProductName,
                        Price = ((item.ProductSellingPrice / (100 + item.ProductGST))) * 100,
                        PriceInclusiveGST = item.ProductSellingPrice,
                        Quantity = item.SellingQty,
                        Brand = item.Manufacturer.Name,
                        Category = item.ProductType.CategoryName,
                        Subcategory = item.ProductType.Name,
                        GST = item.ProductGST,
                        Type ="product"
                    };
                    ProductList.Add(product);

                    if (item.IsServiceChargeApplicable)
                    {
                        var serviceChargeProduct = new Products()
                        {
                            Id = Convert.ToInt32(item.SKU),
                            Name = item.ProductName,
                            PriceInclusiveGST = item.ServiceChargeAmount,
                            Quantity = item.SellingQty,
                            Brand = item.Manufacturer.Name,
                            Category = item.ProductType.CategoryName,
                            Subcategory = item.ProductType.Name,
                            GST = 18,
                            Type = "service"
                        };
                        ProductList.Add(serviceChargeProduct);
                    }
                }

                var SchemeRequest = new SchemeDTO()
                {
                    Demography = new Demography() { Store_name = AppConstants.LoggedInStoreInfo.Name },
                    Coupon_code = CouponCode,
                    Customer = new Customer()
                    {
                        Phone = FarmerPhone,
                        Total_billing_amount_milestone = TotalBillingAmount,
                        GST = string.Empty,
                    },
                    Products = ProductList,
                };

                IsCouponEnteredFromUI = string.IsNullOrEmpty(SchemeRequest.Coupon_code) ? false : true;

                var _result = await _salesService.FetchCoupon(SchemeRequest);
                if (_result.IsSuccess)
                {
                    if (_result.Data.Products != null && _result.Data.Products.Count > 0)
                    {
                        foreach (var item in _result.Data.Products)
                        {
                            if (SalesProducts != null && SalesProducts.Count > 0)
                            {
                                foreach (var it in SalesProducts)
                                {
                                    if (Convert.ToInt32(it.SKU) == item.Id && item.Discount!="")
                                    {
                                        it.ProductManualDiscount += item.DiscountPerProduct;
                                        it.DiscountDetails = item.Discount;
                                    }
                                }
                            }
                        }
                    }
                    IsDiscountApplied = true;
                    DiscountPayableAmount = _result.Data.TotalPayableAmount;
                    TotalCouponAmount =Math.Round( _result.Data.TotalDiscount,2,MidpointRounding.AwayFromZero);
                    PayableAmount = Math.Round(DiscountPayableAmount, 2, MidpointRounding.AwayFromZero);
                    NetTotal = _result.Data.NetTotal;
                    BillingId = _result.Data.BillingId;
                    CouponId = _result.Data.CouponId;
                    OTP = _result.Data.OTP;
                    GST = _result.Data.TotalGST;
                    IsEnableOTP = OTP > 0 ? true : false;

                    var cc = _result.Data.CouponCode.Split(',').ToList();
                    var ct = _result.Data.CouponType.Split(',').ToList();
                    var cn = _result.Data.CouponName.Split(',').ToList();

                    var len = _result.Data.CouponCode.Split(',').Length;

                    for (int i = 0; i < len; i++)
                    {
                        var couponScheme = new CouponsSchemes
                        {
                            Code = cc[i],
                            Name = cn[i],
                            Type = ct[i],
                            SNo = i + 1
                        };
                        CouponSchemeData.Add(couponScheme);
                    }

                    var sb = new StringBuilder();

                    foreach (var coupon in CouponSchemeData)
                    {
                        sb.AppendLine($"Code: {coupon.Code}, Name: {coupon.Name}, Type: {coupon.Type}");
                    }

                    string couponSchemeDataString = sb.ToString().TrimEnd();

                    AppliedCoupon = couponSchemeDataString;
                    CouponCode = (CouponSchemeData != null && !string.IsNullOrEmpty(SchemeRequest.Coupon_code))
             ? _result.Data.CouponCode
             : (CouponSchemeData.All(coupon => coupon.Type == "FutureOffer") ? null : _result.Data.CouponCode);
                    
                    if (string.IsNullOrEmpty(SchemeRequest.Coupon_code) && CouponSchemeData.All(coupon => coupon.Type == "FutureOffer") && IsDiscountApplied)
                    {
                        IsOnlyFutureOffersCreated = true;
                    }
                    if (CouponCode != null && string.IsNullOrEmpty(SchemeRequest.Coupon_code))
                    {
                        CouponCode = string.Join(",",CouponSchemeData.Where(x => x.Type != "FutureOffer").Select(x => x.Code)).TrimEnd();
                    }
                }
                else
                {
                    _notificationService.ShowMessage(_result.Error, NotificationType.Error);
                    return;
                }

            }
            catch (Exception _ex) {
                _logger.LogError(_ex.Message);
            }
        }

        private float _acreage;

        public float Acreage
        {
            get => _acreage;
            set => SetProperty(ref _acreage,value);
        }

        private string _crop;

        public string Crop
        {
            get => _crop;
            set => SetProperty(ref _crop,value); 
        }

        private List<string> _landType;

        public List<string> LandType
        {
            get => _landType; 
            set => SetProperty(ref _landType,value); 
        }

        private ObservableCollection<string> _crops;

        public ObservableCollection<string> Crops
        {
            get => _crops;
            set => SetProperty(ref _crops, value);
        }

        public void LandsType(object Param)
        {
            try
            {
                var values = (object[])Param;
                string name = (string)values[0];
                bool check = (bool)values[1];
                if (check)
                {
                    LandType.Add(name); 
                }
                else
                {
                    LandType.Remove(name);
                }
            }
            catch(Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public void AddCrop(object Param)
        {
            try
            {
                if (string.IsNullOrEmpty(Crop) || string.IsNullOrWhiteSpace(Crop)|| Crop.All(char.IsDigit))
                {
                    _notificationService.ShowMessage("Please enter the valid crop name", NotificationType.Error);
                    return;
                }

                foreach (var item in Crops)
                {
                    if (item.ToLower()==Crop.ToLower())
                    {
                        _notificationService.ShowMessage("Crop name already added", NotificationType.Error);
                        return;
                    }
                }
                
                
                Crops.Add(Crop);
                Crop = null;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public void RemoveCrop(object obj)
        {
            try
            {
                var crop = (string)obj;
                Crops.Remove(crop);
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public  void OTPValidation(object obj)
        {
            try
            {
                if (OTP != StoreOTP)
                {
                    //RemoveCode();
                    _notificationService.ShowMessage("Incorrect OTP Entered", NotificationType.Error);
                    IsEnableOTP = true;
                    return;
                }
                else
                {
                    _notificationService.ShowMessage("OTP verified successfully", NotificationType.Success);
                    IsEnableOTP = false;
                    return;
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private float _totalBillingAmount;

        public float TotalBillingAmount
        {
            get => _totalBillingAmount;
            set => SetProperty(ref _totalBillingAmount , value);
        }


        private int _otp;

        public int OTP
        {
            get => _otp;
            set => SetProperty(ref _otp,value); 
        }

        private string _appliedCoupon;

        public string AppliedCoupon
        {
            get => _appliedCoupon;
            set => SetProperty(ref _appliedCoupon,value);
        }


        private string _couponId;

        public string CouponId
        {
            get => _couponId;
            set => SetProperty(ref _couponId, value);
        }

        private string _billingId;

        public string BillingId
        {
            get => _billingId;
            set => SetProperty(ref _billingId, value);
        }

        private int _Storeotp;

        public int StoreOTP
        {
            get => _Storeotp;
            set => SetProperty(ref _Storeotp, value);
        }

        private bool _isEnableOTP;

        public bool IsEnableOTP
        {
            get => _isEnableOTP;
            set => SetProperty(ref _isEnableOTP, value);
        }

        private bool _isOnlyFutureOffersCreated;

        public bool IsOnlyFutureOffersCreated
        {
            get => _isOnlyFutureOffersCreated;
            set => SetProperty(ref _isOnlyFutureOffersCreated, value);
        }

        private List<CouponsSchemes> _couponSchemeData;

        public List<CouponsSchemes> CouponSchemeData
        {
            get => _couponSchemeData;
            set => SetProperty(ref _couponSchemeData, value);
        }

        private SalesModel _addSalesInput;

        public SalesModel AddSalesInput
        {
            get => _addSalesInput;
            set => SetProperty(ref _addSalesInput, value);
        }

        private bool _isCouponEnteredFromUI;

        public bool IsCouponEnteredFromUI
        {
            get => _isCouponEnteredFromUI;
            set => SetProperty(ref _isCouponEnteredFromUI, value);
        }


        private bool _IsSaveBtnEnable;

        public bool IsSaveBtnEnable
        {
            get { return _IsSaveBtnEnable; }
            set { SetProperty( ref _IsSaveBtnEnable, value); }
        }

    }



}



