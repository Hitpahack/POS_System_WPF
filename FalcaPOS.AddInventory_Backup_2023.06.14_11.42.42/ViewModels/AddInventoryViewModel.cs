using FalcaPOS.AddInventory.Views;
using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.AddInventory;
using FalcaPOS.Entites.Attributes;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.Indent;
using FalcaPOS.Entites.Suppliers;
using FalcaPOS.ServiceLibrary.AddInventory.Services;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Unity;
using Unity.Resolution;

namespace FalcaPOS.AddInventory.ViewModels
{

    public enum ApplyDiscountType
    {
        /// <summary>
        /// Discount is deducted from subtotal ,then GST is applied.
        /// </summary>
        BEFORE,
        /// <summary>
        ///  /// GST is added to sub total ,then discount is applied
        /// </summary>
        AFTER
    }

    public class DiscountTypeEventArgs : EventArgs
    {
        public ApplyDiscountType ApplyDiscountType { get; set; }

        public int TotalProductQtyCount { get; set; }

        public float InvoiceFlatTotal { get; set; }

        public float InvoiceDiscountPercent { get; set; }

        public string DiscountHeader { get; set; }
    }
    public class AddInventoryViewModel : BindableBase//ValidationBase
    {


        #region Fields 



        private readonly ISupplierService _supplierService;


        private readonly IUnityContainer _container;

        private readonly INotificationService _notificationService;


        private readonly StockServices _stockServices;

        private readonly IEventAggregator _eventAggregator;

        private readonly ProgressService _ProgressService;

        private readonly Logger _logger;

        private readonly IInvoiceFileService _invoiceFileService;

        public event EventHandler<DiscountTypeEventArgs> DiscountTypeDropDownChangeEventHandler;

        private readonly IIndentService _indentServices;


        #endregion

        #region DelegateCoammands 

        public DelegateCommand AddProductCardCommand { get; private set; }

        public DelegateCommand<Object> RemoveProductCardCommand { get; private set; }

        public DelegateCommand AddStockProductCommand { get; private set; }

        public DelegateCommand<object> RestStockProductCommand { get; private set; }

        public DelegateCommand<object> defectiveQtyCommand { get; private set; }

        public DelegateCommand<object> DiscountPercentCommand { get; private set; }


        public DelegateCommand<object> DiscountFlatCommand { get; private set; }

        public DelegateCommand AddFileOpenDialogCommand { get; private set; }

        public DelegateCommand<Guid?> DeleteUploadFileCommand { get; private set; }
        public DelegateCommand<bool?> DiscountbtnToggleCommand { get; private set; }

        private string None = "None";

        public DelegateCommand<object> SupplierSelectionChangeCommand { get; private set; }

        public DelegateCommand<object> IndentSelectionChangeCommand { get; private set; }


        public DelegateCommand RefreshIndentCommand { get; private set; }

        private readonly IDialogService _dialogService;

        public DelegateCommand<object> AddshippingAddressCommand { get; private set; }


        #endregion



        #region Constructor

        public AddInventoryViewModel(
            ISupplierService supplierService,
            IUnityContainer container,
            INotificationService notificationService,
            StockServices stockServices,
            IEventAggregator eventAggregator,
            ProgressService ProgressService,
            Logger logger
, IInvoiceFileService invoiceFileService, IIndentService indentService, IDialogService dialogService)
        {


            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _stockServices = stockServices ?? throw new ArgumentNullException(nameof(stockServices));

            _container = container ?? throw new ArgumentNullException(nameof(container));


            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _ProgressService = ProgressService ?? throw new ArgumentNullException(nameof(ProgressService));

            AddProductCardCommand = new DelegateCommand(ExecuteAddProductCardCommand);

            RemoveProductCardCommand = new DelegateCommand<object>(ExecuteRemoveProductCardCommand);

            ProductCards = new ObservableCollection<ProductCardViewModel>();

            ProductCards.CollectionChanged -= ProductCards_CollectionChanged;

            ProductCards.CollectionChanged += ProductCards_CollectionChanged;


            AddStockProductCommand = new DelegateCommand(async () => await AddNewStockProduct());

            RestStockProductCommand = new DelegateCommand<object>(ResetStockProduct);


            _supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _eventAggregator.GetEvent<ReloadSupplierEvent>().Subscribe(LoadSuppliers);
            defectiveQtyCommand = new DelegateCommand<object>(AddDefectiveQty);

            DiscountPercentCommand = new DelegateCommand<object>(AddDisocuntPercentToProductCard);

            DiscountFlatCommand = new DelegateCommand<object>(AddDisocuntFlatToProductCard);
            DiscountbtnToggleCommand = new DelegateCommand<bool?>(DiscountbtnToggleCommandHandler);


            // IsDcNumber = false;
            FileUploadListInfo = new ObservableCollection<FileUploadInfo>();

            AddFileOpenDialogCommand = new DelegateCommand(AddFileOpenDialog);

            DeleteUploadFileCommand = new DelegateCommand<Guid?>(DeleteUploadFile);

            _invoiceFileService = invoiceFileService ?? throw new ArgumentNullException(nameof(invoiceFileService));


            _eventAggregator.GetEvent<SignalRSupplierAddedEvent>().Subscribe(NewSupplierAdded, ThreadOption.PublisherThread);

            _eventAggregator.GetEvent<SignalRSupplierEnableDisableEvent>().Subscribe(SupplierEnableDisable, ThreadOption.PublisherThread);

            _indentServices = indentService ?? throw new ArgumentNullException(nameof(indentService));

            LoadSuppliers();

            LoadIndent();

            _eventAggregator.GetEvent<SignalRIndentStatusToInTransiteEvent>().Subscribe(RefrshIndentLoadData, ThreadOption.PublisherThread);

            SupplierSelectionChangeCommand = new DelegateCommand<object>(SupplierChange);

            IndentSelectionChangeCommand = new DelegateCommand<object>(IndentChange);

            RefreshIndentCommand = new DelegateCommand(LoadIndent);

            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            AddshippingAddressCommand = new DelegateCommand<object>(AddShippingAddress);

            _eventAggregator.GetEvent<SignalRSupplierShippingAddressAddEvent>().Subscribe(LoadSuppliers, ThreadOption.PublisherThread);



        }



        private string _selectedDiscountApplyType;

        public string SelectedDiscountApplyType
        {
            get => _selectedDiscountApplyType;
            set
            {

                _ = SetProperty(ref _selectedDiscountApplyType, value);
                if (value == null)
                {
                    return;
                }
                DiscountTypeDropDownChangeEventHandler?.Invoke(this, GetEventArgs(value));

                if (InvoiceDiscountPerecent > 0)
                {
                    CalCulateInvoiceDiscountPercent();
                }
                if (InvoiceDiscountFlat > 0)
                {
                    CalculateInvoiceFlat();
                }
            }
        }

        private DiscountTypeEventArgs GetEventArgs(string selectedTyp)
        {
            return new DiscountTypeEventArgs
            {
                ApplyDiscountType = GetDiscountApplyType(selectedTyp),
                TotalProductQtyCount = ProductCards == null ? 0 : ProductCards.Sum(x => x.ProductQty * x.ProductSubQty),
                InvoiceDiscountPercent = InvoiceDiscountPerecent,
                InvoiceFlatTotal = InvoiceDiscountFlat,
                DiscountHeader = GetInvoiceDiscountHeader()
            };
        }

        private string GetInvoiceDiscountHeader()
        {
            if (InvoiceDiscountPerecent > 0)
            {
                return "Discount(%)";
            }

            if (InvoiceDiscountFlat > 0)
            {
                return "Discount(Flat)";
            }

            return default;
        }

        private ApplyDiscountType GetDiscountApplyType(string selectedGSTApplyType)
        {
            switch (selectedGSTApplyType)
            {
                case AppConstants.APPLY_DISCOUNT_BEFORE_GST:
                    return ApplyDiscountType.BEFORE;
                case AppConstants.APPLY_DISCOUNT_AFTER_GST:
                    return ApplyDiscountType.AFTER;
                default:
                    return ApplyDiscountType.AFTER;
            }
        }

        private ObservableCollection<string> _discountApplyTypes = new ObservableCollection<string> { AppConstants.APPLY_DISCOUNT_AFTER_GST, AppConstants.APPLY_DISCOUNT_BEFORE_GST };

        public ObservableCollection<string> DiscountApplyTypes
        {
            get
            {
                return _discountApplyTypes;
            }
            set
            {
                SetProperty(ref _discountApplyTypes, value);
            }
        }






        private void SupplierEnableDisable(object obj)
        {
            if (obj is SuppliersViewModel _splr)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {


                        if (Suppliers == null)
                        {
                            Suppliers = new ObservableCollection<SuppliersViewModel>();
                        }

                        //remove from list of suppliers.
                        var _exSupplier = Suppliers.FirstOrDefault(x => x.SupplierId == _splr.SupplierId);

                        if (_exSupplier != null)
                        {
                            Suppliers.Remove(_exSupplier);
                        }

                        if (!_splr.Isenabled)
                        {
                            return;
                        }
                        else
                        {
                            Suppliers.Add(_splr);

                            Suppliers = new ObservableCollection<SuppliersViewModel>(Suppliers.OrderBy(x => x.Name));

                        }
                    });

                }
                catch (Exception _ex)
                {
                    _logger?.LogError("Error in supplier enable disable", _ex);
                }
            }
        }

        private void NewSupplierAdded(object obj)
        {
            if (obj is SuppliersViewModel _suplr)
            {
                try
                {
                    Application.Current.Dispatcher?.Invoke(() =>
                    {

                        if (Suppliers == null)
                        {
                            Suppliers = new ObservableCollection<SuppliersViewModel> { _suplr };

                            return;
                        }

                        if (Suppliers.Any(x => x.SupplierId == _suplr.SupplierId))
                        {
                            var _exstSupplier = Suppliers.FirstOrDefault(x => x.SupplierId == _suplr.SupplierId);

                            if (_exstSupplier != null)
                            {
                                Suppliers.Remove(_exstSupplier);
                            }
                        }

                        Suppliers.Add(_suplr);

                        Suppliers = new ObservableCollection<SuppliersViewModel>(Suppliers.OrderBy(x => x.Name));


                    });
                }
                catch (Exception _ex)
                {
                    _logger.LogError("Error in adding supplier ", _ex);
                }
            }

        }

        private void DeleteUploadFile(Guid? fileId)
        {
            if (fileId == null) return;

            var _file = FileUploadListInfo?.FirstOrDefault(x => x.FileId == fileId);

            if (_file != null)
            {
                _logger.LogInformation($"File Deleted {_file.FileName}");

                FileUploadListInfo?.Remove(_file);
            }

        }

        private void AddFileOpenDialog()
        {

            //if (!InvoiceNumber.IsValidString() && !IsDcNumber)
            //{
            //    _logger.LogWarning("User adding file for invalid invoice number");

            //    _notificationService.ShowMessage("Enter valid invoice number", NotificationType.Error);

            //    return;

            //}


            //if (IsDcNumber && !DcNumber.IsValidString())
            //{
            //    _logger.LogWarning("User adding file for invalid dc number");

            //    _notificationService.ShowMessage("Enter valid DC number", NotificationType.Error);

            //    return;
            //}


            if (FileUploadListInfo != null && FileUploadListInfo.Count >= 5)
            {
                _notificationService.ShowMessage("5 files already added. Delete old file to reselect", NotificationType.Information);

                _logger.LogWarning($"Max File attachments added");

                return;
            }


            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Please select a file",
                Multiselect = true,
                Filter = ApplicationSettings.Invoice_File_Extension_Filter,
            };


            bool? _resultOk = dialog.ShowDialog();

            if (_resultOk == null || _resultOk != true)
            {
                //user cancelled file selection return.
                return;
            }


            if (dialog != null && (FileUploadListInfo?.Count + dialog.FileNames.Length) <= 5 && dialog.FileNames.Length <= 5)
            {
                if (FileUploadListInfo == null)
                {
                    FileUploadListInfo = new ObservableCollection<FileUploadInfo>();
                }
                foreach (string _fileName in dialog.FileNames)
                {
                    if (_fileName.IsValidString())
                    {
                        if (File.Exists(_fileName))
                        {
                            //Check for each fil size ...
                            FileInfo _fileInfo = new FileInfo(_fileName);

                            if ((_fileInfo.Length / (1024 * 1024)) > 10)
                            {
                                _notificationService.ShowMessage($"File {Path.GetFileName(_fileName)} size should be less than 10 mb in size", NotificationType.Error);

                                _logger.LogWarning($"Filename {_fileName} has size of {_fileInfo.Length / (1024 * 1024)} mb");

                                continue;
                            }

                            //dont again add the same file .causes file access issue while reading file stream

                            if (FileUploadListInfo.Any(x => x.FileName == Path.GetFileName(_fileName)))
                            {
                                _notificationService.ShowMessage($"File {Path.GetFileName(_fileName)} is already added", NotificationType.Information);

                                _logger.LogWarning($"File was already added skipping file {_fileName}");

                                continue;
                            }

                            _logger.LogInformation($"Adding file {_fileName}");

                            FileUploadListInfo?.Add(new FileUploadInfo
                            {
                                FileId = Guid.NewGuid(),
                                FilePath = _fileName,
                                FileExtension = Path.GetExtension(_fileName),
                                FileName = Path.GetFileName(_fileName),
                                Size = FileHelper.FormatSize(_fileInfo.Length)
                            });
                        }
                    }
                }
            }
            else
            {
                _notificationService.ShowMessage("Maximum 5 files allowed", NotificationType.Error);
            }
        }

        private ObservableCollection<FileUploadInfo> _fileUploadListInfo;
        public ObservableCollection<FileUploadInfo> FileUploadListInfo
        {
            get { return _fileUploadListInfo; }
            set { SetProperty(ref _fileUploadListInfo, value); }
        }

        public void AddDisocuntPercentToProductCard(object obj)
        {
            var discount = ((TextBox)obj).Text;

            //no need to trigger if the count is zero.
            if (ProductCards == null || ProductCards.Count() == 0) return;
            //bool IsDiscount = discount != "0" ? true : false;
            _eventAggregator.GetEvent<AddDiscountPercentToProductCard>().Publish(discount);
        }

        public void AddDisocuntFlatToProductCard(object obj)
        {
            var discount = ((TextBox)obj).Text;

            //bool IsDiscount = discount != "0" ? true : false;
            if (ProductCards == null || ProductCards.Count() == 0) return;
            _eventAggregator.GetEvent<AddDiscountFlatToProductCard>().Publish(discount);
        }

        public void AddDefectiveQty(object obj)
        {
            var Qty = ((TextBox)obj).Text;
            int defective = Convert.ToInt32(Qty);

            if (defective > Quantity)
            {
                DefectiveQuantity = 0;
                _notificationService.ShowMessage("defective qty can not add more then qty", NotificationType.Error);
                return;
            }
            bool IsDefective = Qty != "0" ? true : false;
            _eventAggregator.GetEvent<AddDefectiveQty>().Publish(IsDefective);
        }

        private void ProductCards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.RaisePropertyChanged("ProductCards");
        }

        #endregion 

        #region Props

        private ObservableCollection<ProductCardViewModel> _productcards;

        public ObservableCollection<ProductCardViewModel> ProductCards
        {
            get { return _productcards; }
            set { SetProperty(ref _productcards, value); }
        }

        private ObservableCollection<SuppliersViewModel> _suppliers;

        public ObservableCollection<SuppliersViewModel> Suppliers
        {
            get => _suppliers;
            set => SetProperty(ref _suppliers, value);
        }

        //public bool IsDcNumber { get; set; }

        //public string DcNumber { get; set; }

        //public DateTime? DcNumberDate { get; set; }

        private bool _isDcNumber;
        public bool IsDcNumber
        {
            get { return _isDcNumber; }
            set
            {
                SetProperty(ref _isDcNumber, value);
                DcNumber = null;
                DcNumberDate = null;
                InvoiceDate = null;
                InvoiceNumber = null;

            }
        }
        private string _dcNumber;
        public string DcNumber
        {
            get { return _dcNumber; }
            set { SetProperty(ref _dcNumber, value); }
        }
        private DateTime? _dcNumberDate;
        public DateTime? DcNumberDate
        {
            get { return _dcNumberDate; }
            set { SetProperty(ref _dcNumberDate, value); }
        }

        private string _invoiceNumber;

        public string InvoiceNumber
        {
            get { return _invoiceNumber; }
            set { SetProperty(ref _invoiceNumber, value); }
        }
        private DateTime? _invoiceDate;

        public DateTime? InvoiceDate
        {
            get { return _invoiceDate; }
            set { SetProperty(ref _invoiceDate, value); }
        }

        private float _totalGST;
        public float TotalGST
        {
            get => _totalGST;
            set
            {
                if (value < 0)
                    value = 0;

                value = (float)Math.Round(value, 2, MidpointRounding.AwayFromZero);

                SetProperty(ref _totalGST, value);
                //CalulateInvoiceRoundOff();
                CalCulateInvoiceDiscountPercent();
                CalculateInvoiceFlat();
            }
        }




        private SuppliersViewModel _selectedSupplier;

        public SuppliersViewModel SelectedSupplier
        {
            get => _selectedSupplier;
            set => SetProperty(ref _selectedSupplier, value);


        }

        private int _quantity;

        public int Quantity
        {
            get { return _quantity; }
            set { SetProperty(ref _quantity, value); }
        }
        private int _defectiveQuantity;

        public int DefectiveQuantity
        {
            get { return _defectiveQuantity; }
            set { SetProperty(ref _defectiveQuantity, value); }
        }
        private float _InvoiceTotal;

        public float InvoiceTotal
        {
            get { return _InvoiceTotal; }
            set
            {
                if (value < 0)
                    value = 0;
                SetProperty(ref _InvoiceTotal, value);
                ResetInvoiceDiscount();
            }

        }

        private float _additionalCharges;
        public float AdditionalCharges
        {
            get => _additionalCharges;
            set => SetProperty(ref _additionalCharges, value);
        }

        private float _invoiceDiscountPercent;

        public float InvoiceDiscountPerecent
        {
            get { return _invoiceDiscountPercent; }
            set
            {
                //    if (InvoiceTotal <= 0)
                //        return;


                if (InvoiceTotal == 0)
                {
                    value = 0;
                }

                if (value < 0 || value > 100)
                {
                    value = 0;
                }

                SetProperty(ref _invoiceDiscountPercent, value);
                if (value > 0)
                {
                    CalCulateInvoiceDiscountPercent();
                }
                if (value == 0 && InvoiceDiscountFlat == 0)
                {
                    InvoiceDiscount = 0;
                    CalulateInvoiceRoundOff();
                    TextBox textBox = new TextBox();
                    textBox.Text = Convert.ToString(_invoiceDiscountPercent);
                    AddDisocuntPercentToProductCard(textBox);
                }
                DiscountTypeDropDownChangeEventHandler?.Invoke(this, GetEventArgs(SelectedDiscountApplyType));
            }
        }
        private float _invoiceDiscountFlat;

        public float InvoiceDiscountFlat
        {
            get { return _invoiceDiscountFlat; }
            set
            {

                //if (InvoiceTotal <= 0)
                //{                    
                //    return;
                //}


                //if (InvoiceTotal == 0)
                //{
                //    value = 0;
                //}

                if (value > InvoiceTotal)
                {
                    value = 0;
                }
                if (value < 0)
                {
                    value = 0;
                }

                SetProperty(ref _invoiceDiscountFlat, value);

                if (value > 0)
                {
                    CalculateInvoiceFlat();
                }

                if (value == 0 && InvoiceDiscountPerecent == 0)
                {
                    InvoiceDiscount = 0;
                    CalulateInvoiceRoundOff();
                    TextBox textBox = new TextBox();
                    textBox.Text = Convert.ToString(_invoiceDiscountFlat);
                    AddDisocuntFlatToProductCard(textBox);
                }
                DiscountTypeDropDownChangeEventHandler?.Invoke(this, GetEventArgs(SelectedDiscountApplyType));
            }
        }



        private float _invoiceDiscount;

        public float InvoiceDiscount
        {
            get { return _invoiceDiscount; }
            set { SetProperty(ref _invoiceDiscount, value); }
        }


        private float _invoiceRoundOff;

        public float InvoiceRoundOff
        {
            get { return _invoiceRoundOff; }
            set
            {
                //if (value < 0)
                //{
                //    value = 0;
                //}
                SetProperty(ref _invoiceRoundOff, value);
                CalulateInvoiceRoundOff();
            }
        }
        private float _others;

        public float InvoiceOthers
        {
            get { return _others; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                SetProperty(ref _others, value);
                CalulateInvoiceRoundOff();
            }
        }

        private float _InvoiceRate;

        public float InvoiceRate
        {
            get { return _InvoiceRate; }
            set
            {
                SetProperty(ref
              _InvoiceRate, value);
            }
        }
        private bool _isQADone;

        public bool IsQADone
        {
            get { return _isQADone; }
            set { SetProperty(ref _isQADone, value); }
        }


        #endregion

        #region Methods

        private async void LoadSuppliers()
        {

            try
            {
                _logger.LogInformation("Gettig suppliers ");

                await Task.Run(async () =>
                {
                    var _result = await _supplierService.GetSuppliers("isenabled=true");

                    if (_result != null && _result.Any())
                    {
                        Application.Current?.Dispatcher?.Invoke(() =>
                        {

                            _logger.LogInformation($"Suppliers get success --count-{_result.Count()}");

                            Suppliers = new ObservableCollection<SuppliersViewModel>(_result);

                            //let user select the supplier
                            //if (SelectedSupplier == null)
                            //{
                            //    SelectedSupplier = Suppliers[0];
                            //}
                        });
                    }

                });

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in loding suplliers", _ex);
            }
        }

        private float _transportCharges;
        public float TransportCharges
        {
            get { return _transportCharges; }
            set
            {
                if (value < 0)
                    value = 0;
                SetProperty(ref _transportCharges, value);
                CalulateInvoiceRoundOff();
            }
        }

        private void CalculateInvoiceFlat()
        {
            InvoiceDiscountPerecent = 0;

            ////default case is after gst
            //if (SelectedDiscountApplyType == AppConstants.APPLY_DISCOUNT_BEFORE_GST)
            //{
            //    //InvoiceDiscount =
            //    InvoiceDiscount = (float)Math.Round(InvoiceTotal- InvoiceDiscountFlat, 2, MidpointRounding.AwayFromZero);
            //}
            //else
            //{
            //    InvoiceDiscount = (float)Math.Round(InvoiceTotal + TotalGST -InvoiceDiscountFlat, 2, MidpointRounding.AwayFromZero);
            //}

            InvoiceDiscount = InvoiceDiscountFlat;

            CalulateInvoiceRoundOff();
        }
        private void CalCulateInvoiceDiscountPercent()
        {
            InvoiceDiscountFlat = 0;

            //default case is after gst
            if (SelectedDiscountApplyType == AppConstants.APPLY_DISCOUNT_BEFORE_GST)
            {

                InvoiceDiscount = (float)Math.Round(InvoiceTotal * InvoiceDiscountPerecent / 100, 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                InvoiceDiscount = (float)Math.Round((InvoiceTotal + TotalGST) * InvoiceDiscountPerecent / 100, 2, MidpointRounding.AwayFromZero);

                //InvoiceDiscount = (InvoiceDiscountPerecent * InvoiceTotal) / 100;
            }

            //InvoiceDiscount = (InvoiceDiscountPerecent * InvoiceTotal) / 100;

            CalulateInvoiceRoundOff();

        }

        private void CalulateInvoiceRoundOff()
        {
            InvoiceRate = (float)Math.Round(((InvoiceTotal + TotalGST) - InvoiceDiscount) + InvoiceRoundOff + InvoiceOthers + TransportCharges, 2, MidpointRounding.AwayFromZero);
        }

        private void ResetInvoiceDiscount()
        {
            InvoiceDiscount = 0;
            InvoiceDiscountPerecent = 0;
            InvoiceDiscountFlat = 0;
            InvoiceRoundOff = 0;
            InvoiceOthers = 0;
            TotalGST = 0;
            TransportCharges = 0;
            InvoiceRate = InvoiceTotal;
            TextBox text = new TextBox();
            text.Text = "0";
            AddDisocuntPercentToProductCard(text);
            AddDisocuntFlatToProductCard(text);
        }
        private void ResetStockProduct(object obj)
        {

            //Application.Current?.Dispatcher?.Invoke(() =>
            //{
            ResetStockProduct();
            ProductCards?.Clear();
            //});

        }

        private async Task AddNewStockProduct()
        {
            try
            {

                _logger.LogInformation("Adding new stockproducts");


                bool _isProductValid = ValidateProducts();

                if (!_isProductValid)
                {
                    _logger.LogWarning("Invalid product validation");

                    return;
                }
                bool _isValid = ValidateData();


                if (_isValid)
                {
                    //confirm  if hsn are empty

                    bool _confirm = await ConfirmHSN();
                    if (!_confirm)
                    {
                        //whish to enter the hsn

                        return;
                    }
                    //proceed to add 
                    //Get the invoice vm 
                    _dialogService.ShowDialog("GSTConfirmationPopUp",
                               parameters: new DialogParameters
                               (""),
                               callback: async (dialogResult) =>
                               {
                                   if (dialogResult != null && dialogResult.Result == ButtonResult.OK)
                                   {
                                       var _vm = GetInvoiceVM();

                                       if (_vm != null)
                                       {
                                           await _ProgressService.StartProgressAsync();

                                           await Task.Run(async () =>
                                           {
                                               Response<string> _result = await _stockServices.AddStockProduct(_vm);

                                               if (_result != null && _result.IsSuccess)
                                               {
                                                   Application.Current?.Dispatcher?.Invoke(async () =>
                                                   {

                                                       if (FileUploadListInfo != null && FileUploadListInfo.Count > 0)
                                                       {
                                                           // _ProgressService.UpdateProgressMessage("Uploading Invoice Files, Please wait");

                                                           if (!int.TryParse(_result.Data, out int _invoiceId))
                                                           {
                                                               _notificationService.ShowMessage("Invoice details added,file upload failed. Please re-upload from my invoices", NotificationType.Error);
                                                           }

                                                           var _uploadResult = await UploadInvoiceFiles(_invoiceId);

                                                           if (_uploadResult != null && _uploadResult.IsSuccess)
                                                           {
                                                               _notificationService.ShowMessage("Invoice details added ", NotificationType.Success);
                                                           }
                                                           else
                                                           {
                                                               _notificationService.ShowMessage(_uploadResult?.Error ?? "Invoice details are added , please re-upload from myinvoice", NotificationType.Error);
                                                           }

                                                       }
                                                       else
                                                       {
                                                           _notificationService.ShowMessage("Invoice details added ", NotificationType.Success);
                                                       }

                                                       //refresh invoice
                                                       _eventAggregator.GetEvent<RefreshInvoices>().Publish();

                                                       //reset view.
                                                       ResetStockProduct(null);

                                                   });
                                               }
                                               else
                                               {
                                                   if (_result != null && !_result.IsSuccess)
                                                   {
                                                       _notificationService.ShowMessage(_result.Error ?? "An error occurred ,try again", NotificationType.Error);
                                                   }
                                               }

                                           });

                                           await _ProgressService.StopProgressAsync();
                                       }
                                   }
                                   else
                                   {
                                       return;
                                   }

                               });





                }
            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in new product type added", _ex);
                _notificationService.ShowMessage("An error occurred ,try again", NotificationType.Error);

                await _ProgressService.StopProgressAsync();

            }

        }

        private async Task<Response<string>> UploadInvoiceFiles(int InvoiceId)
        {
            try
            {
                Response<string> _result = await _invoiceFileService.UploadInvoiceFiles(InvoiceId, FileUploadListInfo.ToArray());

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in uploading invoices ", _ex);

            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "Error in uploading invoice, Please reupload from my invoices"
            };
        }

        private async ValueTask<bool> ConfirmHSN()
        {
            if (ProductCards != null && ProductCards.ToList().Count > 0 && ProductCards.ToList().Any(x => x.HSN == null || x.HSN.Trim() == string.Empty))
            {
                var _result = await _ProgressService.ConfirmAsync("HSN Code Missing !!", "HSN Code is not entered, do you wish to proceed?");

                return _result == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative;
            }
            return true;
        }

        private AddStockProductModel GetInvoiceVM()
        {
            try
            {
                var _vm = new AddStockProductModel();
                _vm.GrossTotal = InvoiceRate;
                //_vm.InvoiceDate = InvoiceDate.Value;
                _vm.InvoiceDiscount = InvoiceDiscount;
                _vm.InvoiceDiscountFlat = InvoiceDiscountFlat;
                _vm.InvoiceDiscountPerecent = InvoiceDiscountPerecent;
                _vm.InvoiceNetTotal = InvoiceTotal;
                //_vm.InvoiceNumber = InvoiceNumber;
                _vm.InvoiceRoundOff = InvoiceRoundOff;
                _vm.IsQADone = IsQADone;
                _vm.Quantity = Quantity;
                _vm.DefectiveQuantity = DefectiveQuantity;
                _vm.StoreID = AppConstants.LoggedInStoreInfo.StoreId;
                _vm.SupplierId = (int)SelectedSupplier.SupplierId;
                _vm.InvoiceOthers = InvoiceOthers;
                _vm.StockProducts = GetStockProducts();
                _vm.Transportcharges = TransportCharges;
                _vm.PoId = SelectedIndent.PoNumber != None ? SelectedIndent.Id : (int?)null;
                _vm.ShippingAddressId = _selectedAddress != null ? SelectedAddress.AddressId : null;

                if (InvoiceDiscountFlat > 0)
                {
                    _vm.InvoiceDiscountMode = "Flat";

                }
                else if (InvoiceDiscountPerecent > 0)
                {
                    _vm.InvoiceDiscountMode = "Percent";
                }



                if (IsDcNumber)
                {
                    //dc number
                    _vm.DcNumber = DcNumber;
                    _vm.DcNumberDate = DcNumberDate;
                    _vm.IsDcNumber = true;
                }
                else
                {
                    //invoice

                    _vm.InvoiceDate = InvoiceDate.Value;
                    _vm.InvoiceNumber = InvoiceNumber;
                }


                //apply discount type
                if (SelectedDiscountApplyType != null)
                {


                    _vm.ApplyDiscountType = SelectedDiscountApplyType;
                }
                else
                {
                    //default will be after GST.
                    _vm.ApplyDiscountType = AppConstants.APPLY_DISCOUNT_AFTER_GST;
                }


                return _vm;



            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in getinvocievm", _ex);
                throw;
            }

        }

        private List<StockProductModel> GetStockProducts()
        {
            try
            {

                var _productCollection = ProductCards;


                if (_productCollection != null && _productCollection.Any())
                {
                    var _productList = new List<StockProductModel>();


                    foreach (var _product in _productCollection)
                    {
                        StockProductModel _p = GetProduct(_product);
                        if (_p != null)
                        {
                            _productList.Add(_p);
                        }

                    }

                    return _productList;
                }

                return null;


            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in getstockproduct method", _ex);
                throw;
            }
        }

        private StockProductModel GetProduct(ProductCardViewModel product)
        {
            try
            {

                return new StockProductModel
                {
                    DateOfExpiry = product.DateOfExpiry,
                    DateOfManufacture = product.DateOfManufacture.Value,
                    //DefectiveQuantity=product.d
                    IsSerialNumberManual = product.IsSerialNumberManual,
                    Location = product.SelectedLocation?.Name,
                    ManufacturerId = (int)product.SelectedProduct.Manufacturer.ManufacturerId,
                    ProductId = (int)product.SelectedProduct.ProductId,
                    ProductRate = product.ProductRate,
                    ProductTypeId = product.SelectedProduct.ProductType.ProductTypeId.GetValueOrDefault(),
                    SerialNumbers = product.SerialNoList.Select(x => x.Value).ToList(),
                    WarrantyService = product.SelectedWarrantyService?.Name,
                    //AttributesSelectedList = product.AttributesSelectedList,
                    ProductGST = (product.SelectedGSTslab?.GstValue ?? default),
                    //ProductUniqGuid = product.ProductUniqGuid,
                    Lotnumber = product.Lotnumber,
                    ProductMRP = product.ProductMRP,

                    //additional fields
                    Quantity = product.ProductQty,//changed to base qty
                    ProductSubQty = product.ProductSubQty,//this is sub qty
                    //SubunitType = product.SelectedSubUnitType,
                    BaseunitType = product.SelectedUnitType,
                    IsGroupTrackMode = product.IsGroupTrackMode,
                    DefectiveQuantity = product.SelectedProduct.DefectiveQty,
                    DefectiveGroup = product.IsGroupTrackMode ? product.DefectiveList.Count > 0 ? attributeMaps(product.DefectiveList.ToList()) : null : null,
                    StroreId = AppConstants.LoggedInStoreInfo.StoreId,
                    //hsn code at stock product
                    HSN = product.HSN,

                };

            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<AttributeMap> attributeMaps(List<Dictionary<AttributeMap, AttributeMap>> keyValuePairs)
        {
            List<AttributeMap> attributeMaps = new List<AttributeMap>();

            foreach (var item in keyValuePairs)
            {
                foreach (var i in item)
                {
                    attributeMaps.Add(new AttributeMap() { AttributeValueName = Convert.ToString(i.Key.AttributeValueId), AttributeValueId = i.Value.AttributeValueId });
                }
            }
            return attributeMaps;
        }

        private bool ValidateProducts()
        {


            if (ProductCards != null && ProductCards.Any())
            {
                bool _isvalid = false;

                foreach (var _product in ProductCards.Select((x, i) => new { _product = x, index = i }))
                {

                    _isvalid = ValidateProductInfo(_product._product, _product.index);

                    if (!_isvalid)
                    {
                        //Dont validate others unless one product is valid.

                        break;
                    }


                }

                //Include null check product maybe null .
                int totalDefectiveIndividul = ProductCards.Where(x => x != null).Select(x => x.SelectedProduct).Sum(x => x == null ? 0 : x.DefectiveQty);
                int totalDefectiveGroup = ProductCards.Where(x => x != null).Select(x => x.DefectiveList).Sum(x => x == null ? 0 : x.Count);
                int totalDefective = totalDefectiveIndividul + totalDefectiveGroup;
                if (totalDefective != DefectiveQuantity)
                {
                    _notificationService.ShowMessage($"Please check  defective quantity not Matching", NotificationType.Error);

                    return false;

                }

                return _isvalid;

            }
            else
            {
                _notificationService.ShowMessage("Add at least one product", NotificationType.Error);
                return false;
            }




        }

        private bool ValidateProductInfo(ProductCardViewModel product, int index = 0)
        {

            if (product != null)
            {
                if (product.IsGroupTrackMode)
                {
                    //verify other fields                   


                    if (!product.SelectedUnitType.IsValidString())
                    {
                        _notificationService.ShowMessage($"Select a Unit type for product {product.SelectedProduct.Name}" +
                            $"at row {index + 1}", NotificationType.Error);
                        return false;
                    }

                    if (product.ProductQty <= 0)
                    {
                        _notificationService.ShowMessage($"Enter valid  unit qty product {product.SelectedProduct.Name}" +
                            $"at row {index + 1}", NotificationType.Error);
                        return false;
                    }

                    if (product.ProductSubQty <= 0)
                    {
                        _notificationService.ShowMessage($"Enter valid sub unit qty for product {product.SelectedProduct.Name}" +
                            $"at row {index + 1}", NotificationType.Error);
                        return false;

                    }
                    //if (!product.SelectedSubUnitType.IsValidString())
                    //{
                    //    _notificationService.ShowMessage($"Select sub unit type for product {product.SelectedProduct.Name}" +
                    //           $"at row {index + 1}", NotificationType.Error);
                    //    return false;

                    //}


                    if (!product.SelectedSubUnitType.IsValidString())
                    {
                        _notificationService.ShowMessage($"Select a Sub Unit type for product {product.SelectedProduct.Name}" +
                            $"at row {index + 1}", NotificationType.Error);
                        return false;
                    }
                }

                if (product.SelectedProduct == null)
                {
                    _notificationService.ShowMessage($"Select a Product  at row {index + 1}", NotificationType.Error);

                    return false;
                }


                if (product.SelectedProduct.ProductType == null)
                {
                    _notificationService.ShowMessage($"Select a sub category at row {index + 1}", NotificationType.Error);

                    return false;
                }

                if (product.SelectedProduct.Manufacturer == null)
                {
                    _notificationService.ShowMessage($"Select a Brand at row {index + 1}", NotificationType.Error);

                    return false;
                }

                //if (product.SelectedProductType == null)
                //{

                //    _notificationService.ShowMessage($"Select a Product type at row {index + 1}", NotificationType.Error);

                //    return false;
                //}
                //else if (product.SelectedManufacturer == null)
                //{
                //    _notificationService.ShowMessage($"Select a Brand at row {index + 1}", NotificationType.Error);

                //    return false;
                //}
                //else if (product.SelectedProduct == null)
                //{
                //    _notificationService.ShowMessage($"Select a Product at row {index + 1}", NotificationType.Error);

                //    return false;

                //}
                if (product.DateOfManufacture == null)
                {
                    _notificationService.ShowMessage($"Select a Date of Manufacture at row {index + 1}", NotificationType.Error);

                    return false;
                }

                if (product.SelectedLocation == null)
                {
                    _notificationService.ShowMessage($"Select a product location at row {index + 1}", NotificationType.Error);

                    return false;
                }
                // if (!product.WarrantyService.IsValidString())
                if (product.SelectedWarrantyService == null)
                {
                    _notificationService.ShowMessage($"Select a warranty or service at row {index + 1}", NotificationType.Error);

                    return false;
                }

                int defective = product.SelectedProduct != null ? product.SelectedProduct.DefectiveQty : 0;
                int productqty = product.ProductQty + defective;
                if (productqty <= 0)
                {
                    _notificationService.ShowMessage($"Enter valid product quantity at row {index + 1}", NotificationType.Error);

                    return false;
                }
                if (product.ProductRate <= 0)
                {
                    _notificationService.ShowMessage($"Enter valid product rate for {product.SelectedProduct.Name} at row {index + 1}", NotificationType.Error);

                    return false;
                }

                if (product.IsSerialNumberManual)
                {
                    //check each serial nummbers are filled.

                    if (product.SerialNoList != null && product.SerialNoList.Any())
                    {
                        foreach (var _sln in product.SerialNoList)
                        {
                            if (!_sln.Value.IsValidString())
                            {
                                _notificationService.ShowMessage($"Manual serial number cannot be empty for {product.SelectedProduct.Name} at  row {index + 1}", NotificationType.Error);

                                return false;
                            }
                        }
                    }

                }


                //expiry date is optional for fetrlizers and implements

                if (product.DateOfExpiry == null &&
                    AppConstants.ExcludeDateOfExpiry != null &&
                    !AppConstants.ExcludeDateOfExpiry.Any(x => x.ToLower().Contains(product.SelectedProduct.Category.CategoryName.ToLower())))
                {
                    _notificationService.ShowMessage($"Select a Date of Expiry at row {index + 1}", NotificationType.Error);

                    return false;
                }

                //Force check for seed product type.
                if (product.SelectedProduct.Category.CategoryName.Trim().ToLower().Contains("seeds"))
                {
                    if (!product.Lotnumber.IsValidString())
                    {
                        _notificationService.ShowMessage($"Lot number is required at row  {index + 1}", NotificationType.Error);
                        return false;
                    }
                }


                //check for MRP is required for fertilizers

                if (product.SelectedProduct.Category.CategoryName.Trim().ToLower().Contains("fertilizers"))
                {
                    if (product.ProductMRP <= 0)
                    {
                        _notificationService.ShowMessage($"Please add MRP for fertilizers at row {index + 1}", NotificationType.Error);

                        return false;

                    }
                }

                //hsn code is  required
                if (!product.HSN.IsValidString())
                {
                    _notificationService.ShowMessage($"Enter valid HSN Code " +
                        $"at row {index + 1}", NotificationType.Error);
                    return false;

                }

                //product sku is required


                if (!product.SelectedProduct.ProductSKU.IsValidString())
                {
                    _notificationService.ShowMessage($"Product SKU is required at row {index + 1}", NotificationType.Error);
                    return false;
                }


                //else
                //{
                //    //Add additional checks if required above.
                //    //finally return true after valid cases.

                //    return true;
                //}
                return true;

            }

            return false;
        }

        private bool ValidateData()
        {

            //dc number changes
            if (IsDcNumber)
            {

                if (!DcNumber.IsValidString())
                {
                    _notificationService.ShowMessage("Enter valid DC number", NotificationType.Error);
                    return false;
                }

                if (DcNumberDate == null)
                {
                    _notificationService.ShowMessage("Select a valid DC number date", NotificationType.Error);
                    return false;
                }

            }
            else
            {
                //invoice
                if (string.IsNullOrEmpty(InvoiceNumber))
                {
                    _notificationService.ShowMessage("Invoice Number is required", NotificationType.Error);
                    return false;
                }
                if (InvoiceDate == null)
                {
                    _notificationService.ShowMessage("Invoice Date is required", NotificationType.Error);
                    return false;
                }
            }



            if (SelectedSupplier == null)
            {
                _notificationService.ShowMessage("Supplier is required", NotificationType.Error);
                return false;
            }
            else if (Quantity <= 0)
            {
                _notificationService.ShowMessage("Enter a valid quantity", NotificationType.Error);
                return false;
            }
            //DO Addtional validations..
            else if (InvoiceTotal <= 0)
            {
                _notificationService.ShowMessage("Enter a valid Net Total", NotificationType.Error);
                return false;
            }
            //else if (SelectedStore == null)
            //{
            //    _notificationService.ShowMessage("Store  is required", NotificationType.Error);
            //    return false;

            //}

            int totalQuantity = ProductCards.Sum(x => x.ProductQty) + ProductCards.Select(x => x.SelectedProduct).Sum(x => x.DefectiveQty);

            if (totalQuantity != Quantity)
            {
                _notificationService.ShowMessage("Invoice quantity should match sum of all product quantities", NotificationType.Error);

                return false;
            }


            //validate total GST
            var _gstTotal = ProductCards.Sum(x => x.ProductGSTTotal);


            var _gstToatlRoundoff = _gstTotal > 0 ? Math.Floor(_gstTotal) : 0.0;

            var _totalGSTRoundoff = TotalGST > 0 ? Math.Floor(TotalGST) : 0.0;





            if (_gstToatlRoundoff != _totalGSTRoundoff)
            {
                _notificationService.ShowMessage("Total GST does not match", NotificationType.Error);

                return false;
            }

            var invocietotal = InvoiceRate - (InvoiceOthers + TransportCharges + InvoiceRoundOff);
            if (Math.Floor(invocietotal) != Math.Floor(ProductCards.Sum(x => x.ProductTotal)))
            {
                _notificationService.ShowMessage("Net total does not match with product sum total", NotificationType.Error);

                return false;
            }


            bool _areFilesValid = ValidateFiles();


            //invalid files
            if (!_areFilesValid)
            {
                return false;
            }


            return true;
        }

        private bool ValidateFiles()
        {
            if (FileUploadListInfo == null || FileUploadListInfo.Count == 0)
            {
                _notificationService.ShowMessage("Invoice should have at least one file as attachment", NotificationType.Error);

                return false;
            }

            foreach (FileUploadInfo _files in FileUploadListInfo.ToList())
            {
                if (File.Exists(_files.FilePath))
                {
                    FileInfo _fileInfo = new FileInfo(_files.FilePath);

                    if ((_fileInfo.Length / (1024 * 1024)) > 10)
                    {
                        _notificationService.ShowMessage($"File {Path.GetFileName(_files.FileName)} size should be less than 10 mb in size", NotificationType.Error);

                        return false;
                    }
                }
                else
                {
                    _notificationService.ShowMessage($"File {_files.FileName} is not available", NotificationType.Error);

                    return false;

                }
            }
            return true;
        }

        //private Object _SelectedCard;

        //public Object SelectedCard
        //{
        //    get { return _SelectedCard; }
        //    set { SetProperty(ref _SelectedCard, value); }
        //}

        private void ExecuteAddProductCardCommand()
        {

            if (SelectedSupplier == null)
            {
                _notificationService.ShowMessage("Please select a supplier", NotificationType.Error);

                return;
            }

            if (SelectedIndent == null)
            {
                _notificationService.ShowMessage("Please select a indent", NotificationType.Error);

                return;
            }
            if (SelectedIndent.PoNumber != None)
            {
                if (SelectedSupplier.SupplierId != SelectedIndent.SupplierId)
                {
                    _notificationService.ShowMessage("selected supplier and selected indent supplier not matching", NotificationType.Error);

                    return;
                }
            }

            if (Quantity <= 0)
            {
                _notificationService.ShowMessage("Enter valid product quantity", NotificationType.Error);

                return;
            }

            if (InvoiceRate <= 0)
            {
                _notificationService.ShowMessage("Enter valid net total", NotificationType.Error);
                return;
            }

            var productCodes = _container.Resolve<ProductCardViewModel>();


            DiscountTypeDropDownChangeEventHandler += productCodes.DiscountTypeDropDownChangeEventHandler;

            productCodes.ProductQtyChangeEvent += ProductCodes_ProductQtyChangeEvent;

            productCodes.ApplyDiscountType = GetDiscountApplyType(SelectedDiscountApplyType);

            if (DefectiveQuantity > 0)
            {
                productCodes.InvoiceDefectiveQty = DefectiveQuantity;
                if (InvoiceDiscountPerecent > 0 || InvoiceDiscountFlat > 0)
                {
                    productCodes.Invoicediscount = true;
                    productCodes.InvoicediscountPercent = InvoiceDiscountPerecent;
                    productCodes.InvoicediscountFlat = InvoiceDiscountFlat;
                }
                productCodes.State = SelectedSupplier.Address?.State;
                ProductCards.Add(productCodes);
            }
            else
            {
                if (InvoiceDiscountPerecent > 0 || InvoiceDiscountFlat > 0)
                {
                    productCodes.Invoicediscount = true;
                    productCodes.InvoicediscountPercent = InvoiceDiscountPerecent;
                    productCodes.InvoicediscountFlat = InvoiceDiscountFlat;
                }
                productCodes.State = SelectedSupplier.Address?.State;
                ProductCards.Add(productCodes);
            }



        }



        private void ProductCodes_ProductQtyChangeEvent(object sender, EventArgs e)
        {
            //calculate only if discount percent or discount flat value precent
            if (InvoiceDiscountFlat != 0 || InvoiceDiscountPerecent != 0)
            {
                DiscountTypeDropDownChangeEventHandler?.Invoke(this, GetEventArgs(SelectedDiscountApplyType));
            }

        }

        private void ExecuteRemoveProductCardCommand(Object RemoveCard)
        {
            if (ProductCards != null)
            {
                var _removeCard = ProductCards.Where(x => x.ID.Equals(RemoveCard)).FirstOrDefault();
                if (_removeCard != null)
                {
                    //remove event handlers.
                    DiscountTypeDropDownChangeEventHandler -= _removeCard.DiscountTypeDropDownChangeEventHandler;
                    _removeCard.ProductQtyChangeEvent -= ProductCodes_ProductQtyChangeEvent;

                    ProductCards.Remove(_removeCard);


                    if (ProductCards != null && ProductCards.Count() > 0)
                    {
                        DiscountTypeDropDownChangeEventHandler?.Invoke(this, GetEventArgs(SelectedDiscountApplyType));
                    }
                }

                if (SelectedIndent != null && SelectedIndent.PoNumber != None)
                {
                    if (ProductCards.Count == 0)
                        SelectedIndent = null;
                }


            }

        }



        private void ResetStockProduct()
        {

            //InvoiceNumber = null;
            //InvoiceDate = null;
            IsDcNumber = false;
            TotalGST = 0;
            SelectedSupplier = null;
            Quantity = 0;
            DefectiveQuantity = 0;
            InvoiceTotal = 0;
            InvoiceDiscountPerecent = 0;
            InvoiceDiscountFlat = 0;
            InvoiceDiscount = 0;
            InvoiceRoundOff = 0;
            InvoiceOthers = 0;
            InvoiceRate = 0;
            TransportCharges = 0;
            //AttributesSelectedList = null;
            // SelectedProduct = null;
            // SelectedManufacturer = null;
            //SelectedProductType = null;
            //SelectedStore = null;
            //SerialNumber = null;
            IsQADone = false;
            // DateOfManufacture = null;
            // DateOfExpiry = null;
            //Location = null;
            // WarrantyService = null;
            // ProductRate = 0;
            // ProductDiscount = 0;
            // ProductTotal = 0;
            //ProductSellingPrice = 0;



            SelectedDiscountApplyType = null;



            FileUploadListInfo?.Clear();

            SelectedIndent = null;
            IsInvoiceNotoggle = false;
        }


        private void DiscountbtnToggleCommandHandler(bool? isChecked)
        {
            if (isChecked != null && !(bool)isChecked)
            {
                InvoiceDiscountPerecent = 0;
                InvoiceDiscountFlat = 0;
            }
        }

        #endregion


        public async void LoadIndent()
        {
            try
            {
                await Task.Run(async () =>
                {
                    var _result = await _indentServices.GetIndentInTransitList();

                    if (_result != null && _result.Data != null)
                    {
                        Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            var viewList = new ObservableCollection<IndentViewModel>(_result.Data.ToList());
                            if (viewList != null && viewList.Count > 0)
                            {
                                IndentviewList?.Clear();

                                foreach (IndentViewModel indent in viewList)
                                {
                                    var supplierdetails = indent.SupplierDetails;
                                    IndentviewList.Add(new IndentViewModel()
                                    {
                                        //reusing this propertyes
                                        PoNumber = indent.PoNumber + "-->" + indent.Date + "-->" + indent.SupplierName,
                                        Id = indent.Id,
                                        SupplierId = supplierdetails.Status == "INPROCESSING" ? supplierdetails.SupplierId : 0,
                                        SupplierName = supplierdetails.Status == "INPROCESSING" ? supplierdetails.SupplierName : null,

                                    });
                                }
                                IndentviewList.Add(new IndentViewModel() { PoNumber = None });

                            }
                        });
                    }
                    else
                        IndentviewList.Add(new IndentViewModel() { PoNumber = None });

                });
            }
            catch (Exception _ex)
            {

                _logger.LogError("getting error in loading indent details", _ex);
            }
        }

        public void RefrshIndentLoadData(IndentViewModel indentView)
        {
            try
            {
                LoadIndent();
            }
            catch (Exception _ex)
            {
                _logger.LogError("getting error in loading indent details", _ex);

            }
        }


        private ObservableCollection<IndentViewModel> _indentViewList = new ObservableCollection<IndentViewModel>();

        public ObservableCollection<IndentViewModel> IndentviewList
        {
            get { return _indentViewList; }
            set { SetProperty(ref _indentViewList, value); }
        }



        private IndentViewModel _selectedindent;

        public IndentViewModel SelectedIndent
        {
            get { return _selectedindent; }
            set
            {


                SetProperty(ref _selectedindent, value);

            }
        }

        public async void SupplierChange(object obj)
        {
            try
            {
                var value = ((AddInventoryViewModel)(obj)).SelectedSupplier;

                if (SelectedIndent != null && SelectedIndent?.PoNumber != None && value != null)
                {
                    if (SelectedIndent.SupplierId != value.SupplierId)
                    {
                        _notificationService.ShowMessage("Selected supplier and selected indent supplier not matching", NotificationType.Error);
                        SelectedSupplier = null;
                        return;
                    }
                }

                if (value != null)
                {
                    GstHeader = value.Address?.State == AppConstants.STATE ? AppConstants.GSTHEADER : AppConstants.IGSTHEADER;
                    await Task.Run(async () =>
                    {
                        var _result = await _supplierService.GetShippingAddress((int)value.SupplierId);


                        if (_result != null && _result.IsSuccess)
                        {

                            Application.Current?.Dispatcher.Invoke(async () =>
                            {

                                if (_result.Data.Count() > 0)
                                {
                                    List<AddressViewModel> View = new List<AddressViewModel>();
                                    foreach (var item in _result.Data)
                                    {
                                        View.Add(new AddressViewModel()
                                        {
                                            supplierName = value.Name,
                                            GST = value.GstNumber,
                                            Phone = item.Phone,
                                            Email = item.Email,
                                            City = item.City,
                                            District = item.District,
                                            State = item.State,
                                            Pincode = item.Pincode,
                                            AddressId = item.AddressId,


                                        });
                                    }

                                    ShippingAddress = View;
                                    IshippingAddress = true;
                                    NoShippingAddress = null;
                                    if (ShippingAddress.Count > 1)
                                    {
                                        ShippingAddresspopup address = new ShippingAddresspopup();
                                        address.DataContext = this;
                                        await DialogHost.Show(address, "RootDialog", ShippingaddressClosingEventHandler);

                                    }
                                    else
                                    {
                                        SelectedAddress = ShippingAddress.FirstOrDefault();
                                    }




                                }
                                else
                                {
                                    SelectedAddress = null;
                                    IshippingAddress = false;
                                    NoShippingAddress = "None";
                                }
                            });

                        }
                        else
                        {
                            SelectedAddress = null;
                            IshippingAddress = false;
                            NoShippingAddress = "None";
                        }


                    });

                }
                else
                {
                    SelectedAddress = null;
                    IshippingAddress = false;
                    GstHeader = "Total GST";
                    NoShippingAddress = "None";
                    SelectedIndent = null;
                }




            }
            catch (Exception _ex)
            {
                _logger.LogError("getting error in selection change supplier", _ex);

            }
        }

        public async void IndentChange(object obj)
        {
            try
            {
                ProductCards?.Clear();
                var value = ((AddInventoryViewModel)(obj)).SelectedIndent;
                if (SelectedSupplier != null && value != null && value?.PoNumber != None)
                {
                    if (SelectedSupplier.SupplierId != value?.SupplierId)
                    {
                        _notificationService.ShowMessage("Selected supplier and selected indent supplier not matching", NotificationType.Error);
                        SelectedIndent = null;
                        return;
                    }


                }
                if (value != null && value?.PoNumber != None)
                {
                    var result = await _indentServices.ViewDetailIndent(value.Id);
                    if (result != null && result.IsSuccess)
                    {
                        if (result.Data != null && result.Data.Products.Count() > 0)
                        {

                            foreach (var item in result.Data.Products)
                            {
                                if(item.AvailableQty>0)
                                AddIndentProduct(item, result.Data.SelecedSupplierAddress?.State);
                            }


                            SelectedSupplier = Suppliers.FirstOrDefault(x => x.SupplierId == value?.SupplierId);
                            selectedIndentChangeSupplier();


                        }
                    }
                }
                else
                {
                    SelectedSupplier = null;
                }


            }

            catch (Exception _ex)
            {
                _logger.LogError("getting error in selection change indent", _ex);

            }
        }

        private void ShippingaddressClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {
                var _viewmodel = (AddInventoryViewModel)eventArgs.Parameter;
                if (_viewmodel != null)
                {
                    SelectedAddress = _viewmodel.ShippingAddress.Where(x => x.IsSelectedAddress).FirstOrDefault();
                }
                else
                {
                    SelectedSupplier = null;
                }


            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in getting shipping address", _ex);

            }
        }


        private List<AddressViewModel> _shippingAddress;

        public List<AddressViewModel> ShippingAddress
        {
            get { return _shippingAddress; }

            set { SetProperty(ref _shippingAddress, value); }
        }

        private AddressViewModel _selectedAddress;

        public AddressViewModel SelectedAddress
        {
            get { return _selectedAddress; }
            set { SetProperty(ref _selectedAddress, value); }
        }

        private bool _IshippingAddress;

        public bool IshippingAddress
        {
            get { return _IshippingAddress; }
            set { SetProperty(ref _IshippingAddress, value); }
        }

        private string _noshippingAddress;

        public string NoShippingAddress
        {
            get { return _noshippingAddress; }
            set { SetProperty(ref _noshippingAddress, value); }
        }

        private string _gstHeader;

        public string GstHeader
        {
            get { return _gstHeader; }
            set { SetProperty(ref _gstHeader, value); }
        }

        private bool _isInvocieNotoggle;

        public bool IsInvoiceNotoggle
        {
            get { return _isInvocieNotoggle; }
            set { SetProperty(ref _isInvocieNotoggle, value); }
        }


        public void AddShippingAddress(object param)
        {
            try
            {
                if (ShippingAddress != null)
                {
                    var shipping = ShippingAddress.Where(x => x.IsSelectedAddress == true).FirstOrDefault();
                    if (shipping == null)
                    {
                        _notificationService.ShowMessage("Please select one shipping address", NotificationType.Error);

                        return;
                    }
                }


                var TargetClose = ((Button)(param));
                var dynamicCommand = TargetClose.Command;
                dynamicCommand.CanExecute(true);
                //pass Model as Arguments
                dynamicCommand.Execute(this);
            }
            catch (Exception _ex)
            {

                _logger.LogError("Error in getting shipping", _ex);
            }
        }


        private void AddIndentProduct(IndentViewProduct product, string state)
        {
            try
            {
                var productCodes = _container.Resolve<ProductCardViewModel>(new ParameterOverride("Product", product));


                DiscountTypeDropDownChangeEventHandler += productCodes.DiscountTypeDropDownChangeEventHandler;

                productCodes.ProductQtyChangeEvent += ProductCodes_ProductQtyChangeEvent;

                productCodes.ApplyDiscountType = GetDiscountApplyType(SelectedDiscountApplyType);

                if (DefectiveQuantity > 0)
                {
                    productCodes.InvoiceDefectiveQty = DefectiveQuantity;
                    if (InvoiceDiscountPerecent > 0 || InvoiceDiscountFlat > 0)
                    {
                        productCodes.Invoicediscount = true;
                        productCodes.InvoicediscountPercent = InvoiceDiscountPerecent;
                        productCodes.InvoicediscountFlat = InvoiceDiscountFlat;
                    }
                    productCodes.State = state;
                    ProductCards.Add(productCodes);
                }
                else
                {
                    if (InvoiceDiscountPerecent > 0 || InvoiceDiscountFlat > 0)
                    {
                        productCodes.Invoicediscount = true;
                        productCodes.InvoicediscountPercent = InvoiceDiscountPerecent;
                        productCodes.InvoicediscountFlat = InvoiceDiscountFlat;
                    }
                    productCodes.State = state;
                    ProductCards.Add(productCodes);
                }


            }
            catch (Exception _ex)
            {
                _logger.LogError("getting error in added intent product", _ex);

            }
        }

        public async void selectedIndentChangeSupplier()
        {
            try
            {
                var value = SelectedSupplier;

                if (SelectedIndent != null && SelectedIndent?.PoNumber != None && value != null)
                {
                    if (SelectedIndent.SupplierId != value.SupplierId)
                    {
                        _notificationService.ShowMessage("Selected supplier and selected indent supplier not matching", NotificationType.Error);
                        SelectedSupplier = null;
                        return;
                    }
                }

                if (value != null)
                {
                    GstHeader = value.Address?.State == AppConstants.STATE ? AppConstants.GSTHEADER : AppConstants.IGSTHEADER;
                    await Task.Run(async () =>
                    {
                        var _result = await _supplierService.GetShippingAddress((int)value.SupplierId);


                        if (_result != null && _result.IsSuccess)
                        {

                            Application.Current?.Dispatcher.Invoke(async () =>
                            {

                                if (_result.Data.Count() > 0)
                                {
                                    List<AddressViewModel> View = new List<AddressViewModel>();
                                    foreach (var item in _result.Data)
                                    {
                                        View.Add(new AddressViewModel()
                                        {
                                            supplierName = value.Name,
                                            GST = value.GstNumber,
                                            Phone = item.Phone,
                                            Email = item.Email,
                                            City = item.City,
                                            District = item.District,
                                            State = item.State,
                                            Pincode = item.Pincode,
                                            AddressId = item.AddressId,


                                        });
                                    }

                                    ShippingAddress = View;
                                    IshippingAddress = true;
                                    NoShippingAddress = null;
                                    if (ShippingAddress.Count > 1)
                                    {
                                        ShippingAddresspopup address = new ShippingAddresspopup();
                                        address.DataContext = this;
                                        await DialogHost.Show(address, "RootDialog", ShippingaddressClosingEventHandler);

                                    }
                                    else
                                    {
                                        SelectedAddress = ShippingAddress.FirstOrDefault();
                                    }




                                }
                                else
                                {
                                    SelectedAddress = null;
                                    IshippingAddress = false;
                                    NoShippingAddress = "None";
                                }
                            });

                        }
                        else
                        {
                            SelectedAddress = null;
                            IshippingAddress = false;
                            NoShippingAddress = "None";
                        }


                    });

                }
                else
                {
                    SelectedAddress = null;
                    IshippingAddress = false;
                    GstHeader = "Total GST";
                    NoShippingAddress = "None";
                }

                SelectedSupplier = SelectedSupplier;


            }
            catch (Exception _ex)
            {
                _logger.LogError("getting error in selection change supplier", _ex);

            }
        }
    }





}
