using FalcaPOS.Common;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Indent;
using FalcaPOS.Entites.Sku;
using FalcaPOS.Sku.View;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FalcaPOS.Sku.ViewModels
{
    public class ApproveSKUFinanceViewModel:BindableBase
    {
        private readonly Logger _logger;

        private readonly ISkuService _skuService;

        private readonly ProgressService _progressService;

        private readonly INotificationService _notificationService;
        private readonly IEventAggregator _eventAggregator;
        public DelegateCommand RefreshSKUCommand { get; private set; }
        public DelegateCommand<object> ApproveSKUCommand { get; private set; }
        public DelegateCommand<object> RejectSKUCommand { get; private set; }
        public DelegateCommand<object> EditSKUCommand { get; private set; }
        public DelegateCommand<object> SaveAndApproveCommand { get; private set; }
        public DelegateCommand<object> SKUConfirmRejectCommand { get; private set; }

        public DelegateCommand<object> SKUConfirmApprovCommand { get; private set; }
        public DelegateCommand<object> MinMarginTextChangedCommand { get; private set; }
        public ApproveSKUFinanceViewModel(Logger logger, INotificationService notificationService, ISkuService skuService, ProgressService progressService,IEventAggregator eventAggregator) {

            RefreshSKUCommand = new DelegateCommand(RefreshSKUApprove);

            _skuService = skuService ?? throw new ArgumentNullException(nameof(skuService));

            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            ApproveSKUCommand = new DelegateCommand<object>(ApproveSKUproduct);

            RejectSKUCommand = new DelegateCommand<object>(RejectSKUproduct);

            EditSKUCommand = new DelegateCommand<object>(EditSKUproduct);

            SaveAndApproveCommand = new DelegateCommand<object>(SaveAndApprove);

            LoadSKU();

            SubUnitTypes = GetSubUnitTypes();

            WarrantyServices = GetWarrantyServices();

            GSTslabs = GetGSTslabs();

            NewSKUpoductList = new ObservableCollection<NewProductV2>();

            SKUConfirmRejectCommand = new DelegateCommand<object>(ConfirmReject);

            SKUConfirmApprovCommand = new DelegateCommand<object>(confirmApprove);

            MinMarginTextChangedCommand = new DelegateCommand<object>(MinmiumMarginPer);
        }

        private void SaveAndApprove(object obj)
        {
            try
            {
                if (string.IsNullOrEmpty(EditSKUProduct.ProductName) || EditSKUProduct.ProductName.Trim().Length==0)
                {
                    _notificationService.ShowMessage("Please enter the product name",NotificationType.Error);
                    return;
                }
                if (string.IsNullOrEmpty(EditSKUProduct.TechnicalName) || EditSKUProduct.TechnicalName.Trim().Length == 0)
                {
                    _notificationService.ShowMessage("Please enter the technical name" , NotificationType.Error);
                    return;
                }


                if (string.IsNullOrEmpty(EditSKUProduct.PackingSize) || EditSKUProduct.PackingSize.Trim().Length == 0)
                {
                    _notificationService.ShowMessage("Please enter the packing size", NotificationType.Error);
                    return;
                }

                if (string.IsNullOrEmpty(EditSKUProduct.UOM))
                {
                    _notificationService.ShowMessage("Please select a UOM ", NotificationType.Error);
                    return;
                }

                
                if (string.IsNullOrEmpty(EditSKUProduct.HSN) || EditSKUProduct.HSN.Trim().Length == 0)
                {
                    _notificationService.ShowMessage("Please enter the HSN code", NotificationType.Error);
                    return;

                }
                if (EditSKUProduct.HSN.Length<8)
                {
                    _notificationService.ShowMessage("HSN Code should be 8 characters", NotificationType.Error);
                    return;

                }
                if (SelectedWarrantyService == null)
                {
                    _notificationService.ShowMessage("Please select the warranty/service", NotificationType.Error);
                    return;

                }

                if (SelectedGSTslab == null)
                {
                    _notificationService.ShowMessage("Please select the GST", NotificationType.Error);
                    return;

                }
                if (EditSKUProduct.MinMargin<0)
                {
                    _notificationService.ShowMessage("Minimum margin percentage should not be negative", NotificationType.Error);
                    return;

                }

                var TargetClose = ((Button)(obj));
                var dynamicCommand = TargetClose.Command;
                dynamicCommand.CanExecute(true);
                //pass Model as Arguments
                dynamicCommand.Execute(this);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        private void RefreshSKUApprove()
        {
            try
            {
                LoadSKU();
            }
            catch(Exception ex) { _logger.LogError(ex.Message); }
        }
        public async void LoadSKU()
        {
            try
            {

                await Task.Run(async () =>
                {
                    var result = await _skuService.GetSKUApproveListV2();

                    Application.Current?.Dispatcher?.Invoke( () =>
                    {
                        if (result != null && result.IsSuccess)
                        {
                            if(result.Data!=null && result.Data.ToList().Count > 0)
                            {
                                NewSKUpoductList = new ObservableCollection<NewProductV2>(result.Data);

                            }
                            else
                            {
                                NewSKUpoductList= new ObservableCollection<NewProductV2>();
                                _notificationService.ShowMessage("No record found", Common.NotificationType.Error);
                            }


                        }
                        else
                        {
                            _notificationService.ShowMessage(result.Error, Common.NotificationType.Error);
                            return;
                        }

                    });
                });


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }


        }

        public async void ApproveSKUproduct(object obj)
        {
            try
            {
                var _viewModel = (NewProductV2)obj;
                if (_viewModel != null)
                {
                    SKUApproveConfirmationPopup sKUApprove = new SKUApproveConfirmationPopup();
                    sKUApprove.DataContext = this;
                    ProductApprove = _viewModel;
                    await DialogHost.Show(sKUApprove, "RootDialog", ApproveSKUproductEventHandler);
                }

            }
            catch(Exception _ex)
            {
                _logger.LogError(_ex.Message);  
            }
            finally
            {
                await _progressService.StopProgressAsync();
            }
        }

        public async void RejectSKUproduct(object obj)
        {
            try
            {
                var _viewModel=(NewProductV2)obj;
                if (_viewModel != null)
                {
                    SKUConfirmationPopup sKUConfirmation = new SKUConfirmationPopup();
                    sKUConfirmation.DataContext = this;
                    ProductId = _viewModel.ProductId;
                    this.Remarks = null;
                    await DialogHost.Show(sKUConfirmation, "RootDialog", RejectSKUproductEventHandler);
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
       
        public async void EditSKUproduct(object obj)
        {
            try
            {
                var _viewEdit = (NewProductV2)obj;
                if (_viewEdit != null)
                {
                    EditSKUpopup editSKUpopup = new EditSKUpopup();
                    editSKUpopup.DataContext = this;
                    EditSKUProduct = _viewEdit;
                    EditSKUProduct.Remarks = null;
                    SelectedWarrantyService = WarrantyServices.FirstOrDefault(x => x.Name == _viewEdit.Warranty);
                    SelectedGSTslab = GSTslabs.FirstOrDefault(x => x.GstValue == _viewEdit.GST);
                    await DialogHost.Show(editSKUpopup, "RootDialog", editSKUpopupEventhandler);
                }
               
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private async  void editSKUpopupEventhandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {
                var _viewModel = (ApproveSKUFinanceViewModel)eventArgs.Parameter;

                if (_viewModel != null && EditSKUProduct!=null)
                {
                    var EditChanges = new NewProductV2()
                    {
                        ProductId = EditSKUProduct.ProductId,
                        ProductName = EditSKUProduct.ProductName,
                        PackingSize = EditSKUProduct.PackingSize,
                        GST = SelectedGSTslab.GstValue,
                        HSN = EditSKUProduct.HSN,
                        TechnicalName = EditSKUProduct.TechnicalName,
                        Warranty = SelectedWarrantyService.Name,
                        UOM = EditSKUProduct.UOM,
                        MinMargin = EditSKUProduct.MinMargin,
                        ProductTypeId = EditSKUProduct.ProductTypeId,
                        Remarks = EditSKUProduct.Remarks
                       
                    };
                    await _progressService.StartProgressAsync();

                    var _result = await _skuService.ApprovalSKU(EditChanges);
                    if (_result != null && _result.IsSuccess)
                    {
                        _notificationService.ShowMessage(_result.Data, Common.NotificationType.Success);
                        LoadSKU();
                        //send event to master SKU sheet

                        _eventAggregator.GetEvent<MasterSKURefreshEvent>().Publish();


                        return;
                    }
                    else
                    {
                        _notificationService.ShowMessage(_result.Error, Common.NotificationType.Error);
                       
                    }
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

        private ObservableCollection<NewProductV2> _newSKUproductLsit;

        public ObservableCollection<NewProductV2> NewSKUpoductList
        {
            get { return _newSKUproductLsit; }
            set {  SetProperty(ref _newSKUproductLsit , value); }
        }


        private NewProductV2 _editSKUproduct;

        public NewProductV2 EditSKUProduct
        {
            get {  return _editSKUproduct; }
            set { SetProperty(ref _editSKUproduct , value); }
        }

        private ObservableCollection<WarrantyService> _warrantyServices;

        public ObservableCollection<WarrantyService> WarrantyServices
        {
            get => _warrantyServices;
            set => SetProperty(ref _warrantyServices, value);
        }

        private WarrantyService _selectedWarrantyService;

        public WarrantyService SelectedWarrantyService
        {
            get => _selectedWarrantyService;
            set => SetProperty(ref _selectedWarrantyService, value);
        }

        private ObservableCollection<GSTslabs> _gstslabs;

        public ObservableCollection<GSTslabs> GSTslabs
        {
            get => _gstslabs;
            set => SetProperty(ref _gstslabs, value);
        }


        private GSTslabs _selectedGSTslab;

        public GSTslabs SelectedGSTslab
        {
            get => _selectedGSTslab;
            set
            {
                SetProperty(ref _selectedGSTslab, value);

            }
        }

        private ObservableCollection<WarrantyService> GetWarrantyServices()
        {
            if (ApplicationSettings.WARRENTY_SERVICE != null && ApplicationSettings.WARRENTY_SERVICE.Any())
            {
                return new ObservableCollection<WarrantyService>(
                    ApplicationSettings.WARRENTY_SERVICE.Select(x => new WarrantyService { Name = x }));

            }

            return default;
        }

        private ObservableCollection<GSTslabs> GetGSTslabs()
        {
            if (ApplicationSettings.GST_VALUES != null && ApplicationSettings.GST_VALUES.Any())
            {
                return new ObservableCollection<GSTslabs>(ApplicationSettings
                    .GST_VALUES.Select(x => new GSTslabs { GstValue = x }));

            }

            return default;
        }



        private ObservableCollection<string> _subUnitTypes;
        public ObservableCollection<string> SubUnitTypes
        {
            get { return _subUnitTypes; }
            set { SetProperty(ref _subUnitTypes, value); }
        }

        private string _selectedSubUnitType;
        public string SelectedSubUnitType
        {
            get { return _selectedSubUnitType; }
            set { SetProperty(ref _selectedSubUnitType, value); }
        }
        private ObservableCollection<string> GetSubUnitTypes()
        {
            return new ObservableCollection<string>
            {
                 "KG","LT","ML","GRAMS","UNITS"
            };
        }

        private string _remarks;
        public string Remarks
        {
            get { return _remarks; }
            set {SetProperty( ref  _remarks , value); }
        }

        private int _productId;
        public int ProductId
        {
            get { return _productId; }
            set { SetProperty(ref _productId, value); }
        }

        private NewProductV2 _productApprove;

        public NewProductV2 ProductApprove
        {
            get { return _productApprove; }
            set { SetProperty( ref _productApprove , value); }
        }


        public void confirmApprove(object obj)
        {
            try
            {
                var TargetClose = ((Button)(obj));
                var dynamicCommand = TargetClose.Command;
                dynamicCommand.CanExecute(true);
                //pass Model as Arguments
                dynamicCommand.Execute(this);
            }
            catch(Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }
        public void ConfirmReject(object obj)
        {
            try
            {
                if (String.IsNullOrEmpty(this.Remarks) || this.Remarks.Trim().Length == 0)
                {
                    _notificationService.ShowMessage("Please enter the reason", NotificationType.Error);
                    return;
                }

                var TargetClose = ((Button)(obj));
                var dynamicCommand = TargetClose.Command;
                dynamicCommand.CanExecute(true);
                //pass Model as Arguments
                dynamicCommand.Execute(this);
            }
            catch(Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public async void RejectSKUproductEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {
                var _viewModel = (ApproveSKUFinanceViewModel)eventArgs.Parameter;
                if (_viewModel != null)
                {
                    
                    await _progressService.StartProgressAsync();

                    var _result = await _skuService.RemovePendingApprovalSKU(_viewModel.ProductId,_viewModel.Remarks);
                    if (_result != null && _result.IsSuccess)
                    {
                        _notificationService.ShowMessage(_result.Data, Common.NotificationType.Success);
                        this.Remarks = null;
                        LoadSKU();

                    }
                    else
                    {
                        _notificationService.ShowMessage(_result.Error, Common.NotificationType.Error);

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

        public async void ApproveSKUproductEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {
                var _viewModel = (ApproveSKUFinanceViewModel)eventArgs.Parameter; 
                if (_viewModel != null)
                {
                    await _progressService.StartProgressAsync();

                    var _result = await _skuService.ApprovalSKU(ProductApprove);
                    if (_result != null && _result.IsSuccess)
                    {
                        _notificationService.ShowMessage(_result.Data, Common.NotificationType.Success);
                        LoadSKU();

                    }
                    else
                    {
                        _notificationService.ShowMessage(_result.Error, Common.NotificationType.Error);

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

        public void MinmiumMarginPer(object obj)
      {
            try
            {
                if (EditSKUProduct.MinMargin > 100)
                {
                    EditSKUProduct.MinMargin = 0;
                }

            }
            catch(Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }
    }

}
