using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.AddInventory;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.Suppliers;
using FalcaPOS.PurchaseReturns.View;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FalcaPOS.PurchaseReturns.ViewModel
{
    public class StoreViewReturnViewModel : BindableBase
    {
        private readonly Logger _logger;

        private readonly IPurchaseInvoiceService _purchaseInvoiceService;

        private readonly INotificationService _notificationService;

        private readonly ProgressService _progressService;


        private CancellationTokenSource _cancellationTokenSource;

        private readonly IPurchaseReturnService _purchaseReturnService;

        public DelegateCommand<object> RefreshCommand { get; private set; }

        public DelegateCommand<int?> DownloadAttachCommand { get; private set; }

        private readonly IInvoiceFileService _invoiceFileService;

        public DelegateCommand<object> StatusCommand { get; private set; }

        public DelegateCommand<object> StatusUncheckCommand { get; private set; }


        public DelegateCommand<object> PopupCommand { get; private set; }


        private readonly IDialogService _dialogService;

        public DelegateCommand<object> EditCommand { get; private set; }

        public DelegateCommand PurhcaseReturnCommand { get; private set; }
        public DelegateCommand PurchaseResetReturnCommand { get; private set; }

        public DelegateCommand<object> RemovePurhcaseCommand { get; private set; }

        private readonly ISupplierService _supplierService;

        public DelegateCommand<object> TexChangeQtyCommand { get; private set; }

        public DelegateCommand<object> ProductChangeUpdateCommand { get; private set; }

        public DelegateCommand<object> SupplierSelectionChangeCommand { get; private set; }

        public DelegateCommand<object> UpdateApproveCommand { get; private set; }

        public DelegateCommand<object> AddAttachment { get; private set; }

        public DelegateCommand<Guid?> DeleteUploadFileCommand { get; private set; }

        public DelegateCommand<object> PurchaseViewSearchCommand { get; private set; }

        StoreReturnModel _storeReturnUpdate = new StoreReturnModel();



        public StoreViewReturnViewModel(ISupplierService supplierService, IPurchaseInvoiceService purchaseInvoiceService, IDialogService dialogService, IPurchaseReturnService purchaseReturnService, Logger logger, INotificationService notificationService, ProgressService progressService, IInvoiceFileService invoiceFileService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));

            _purchaseInvoiceService = purchaseInvoiceService ?? throw new ArgumentNullException(nameof(purchaseInvoiceService));

            _purchaseReturnService = purchaseReturnService ?? throw new ArgumentNullException(nameof(purchaseReturnService));

            _invoiceFileService = invoiceFileService ?? throw new ArgumentNullException(nameof(invoiceFileService));

            _supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));

            StatusCommand = new DelegateCommand<object>(LoadData);

            StatusUncheckCommand = new DelegateCommand<object>(UnCheckCommnand);

            DownloadAttachCommand = new DelegateCommand<int?>(ExecDownloadCommand);

            PopupCommand = new DelegateCommand<object>(ApprovePopView);

            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            EditCommand = new DelegateCommand<object>(Edit);


            PurhcaseReturnCommand = new DelegateCommand(LoadData);

            PurchaseResetReturnCommand = new DelegateCommand(ResetLoadData);

            RemovePurhcaseCommand = new DelegateCommand<object>(RemovePurhcaseProductCard);

            TexChangeQtyCommand = new DelegateCommand<object>(ReturnQtyTextChange);

            ProductChangeUpdateCommand = new DelegateCommand<object>(UpdateProduct);

            LoadSuppliers();

            SupplierSelectionChangeCommand = new DelegateCommand<object>(SelectionChange);

            UpdateApproveCommand = new DelegateCommand<object>(UpdateApprove);

            AddAttachment = new DelegateCommand<object>(FileOpenDialog);

            DeleteUploadFileCommand = new DelegateCommand<Guid?>(DeleteUploadFile);

            RefreshCommand = new DelegateCommand<object>(Refresh);

            PurchaseViewSearchCommand = new DelegateCommand<object>(GetData);
        }


        public async void LoadData(object obj)
        {
            try
            {


                var Selection = Convert.ToString(obj);


                if (Selection == "Pre-C/N")
                {

                    IsapproveChecked = false;
                    IsadjustedChecked = false;
                    IsCreatedChecked = false;
                    IsSendBackChecked = false;
                    IsPostCNChecked = false;
                    IsPartiallyAdjusted = false;
                }
                else if (Selection == "Pre-C/N-Approved")
                {
                    IsPreCNChecked = false;
                    IsadjustedChecked = false;
                    IsPostCNChecked = false;
                    IsCreatedChecked = false;
                    IsSendBackChecked = false;
                    IsPartiallyAdjusted = false;
                }
                else if (Selection == "Adjusted")
                {
                    IsPreCNChecked = false;
                    IsapproveChecked = false;
                    IsSendBackChecked = false;
                    IsCreatedChecked = false;
                    IsPostCNChecked = false;
                    IsPartiallyAdjusted = false;

                }
                else if (Selection == "Post-C/N")
                {
                    IsPreCNChecked = false;
                    IsapproveChecked = false;
                    IsCreatedChecked = false;
                    IsSendBackChecked = false;
                    IsadjustedChecked = false;
                    IsPartiallyAdjusted = false;
                }
                else if (Selection == "Created")
                {
                    IsPreCNChecked = false;
                    IsapproveChecked = false;
                    IsadjustedChecked = false;
                    IsSendBackChecked = false;
                    IsPostCNChecked = false;
                    IsPartiallyAdjusted = false;
                }
                else if (Selection == "SendBack")
                {
                    IsPreCNChecked = false;
                    IsapproveChecked = false;
                    IsPostCNChecked = false;
                    IsCreatedChecked = false;
                    IsadjustedChecked = false;
                    IsPartiallyAdjusted = false;
                }
                else if (Selection == "Partially Adjusted")
                {
                    IsPreCNChecked = false;
                    IsapproveChecked = false;
                    IsPostCNChecked = false;
                    IsCreatedChecked = false;
                    IsadjustedChecked = false;
                    IsSendBackChecked = false;
                }






            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            finally
            {

            }


        }


        private ObservableCollection<StoreReturnModel> _storereturnviews;

        public ObservableCollection<StoreReturnModel> StoreReturnViews
        {
            get { return _storereturnviews; }
            set { SetProperty(ref _storereturnviews, value); }
        }

        private string _rowcount;
        public string RowCount
        {
            get { return _rowcount; }

            set { SetProperty(ref _rowcount, value); }
        }

        private async void ExecDownloadCommand(int? fileID)
        {
            if (fileID != null && fileID.Value > 0)
            {
                var _result = await _invoiceFileService.DownloadFile(fileID.Value);

                if (_result != null)
                {
                    if (_result != null && _result.IsSuccess && _result.Data != null && _result.Data.FileStream != null)
                    {
                        SaveFileDialog sd = new SaveFileDialog
                        {
                            CreatePrompt = true,
                            OverwritePrompt = true,
                            DefaultExt = "zip",
                        };
                        var _saveFile = sd.ShowDialog();

                        if (_saveFile == true && sd.FileName.IsValidString())
                        {
                            sd.FileName = Path.ChangeExtension(sd.FileName, "zip");

                            File.WriteAllBytes(sd.FileName, _result.Data.FileStream);
                        }
                    }
                    else
                    {
                        _notificationService.ShowMessage(_result?.Error ?? "An error occurred, try again", Common.NotificationType.Error);
                    }
                }
            }
        }

        public void UnCheckCommnand(object obj)
        {
            try
            {
                string Selection = Convert.ToString(obj);
                if (Selection == "Pre-C/N")
                {
                    IsPreCNChecked = false;

                }
                else if (Selection == "Pre-C/N-Approved")
                {
                    IsapproveChecked = false;
                }
                else if (Selection == "Adjusted")
                {
                    IsadjustedChecked = false;
                }
                else if (Selection == "Post-C/N")
                {
                    IsPostCNChecked = false;
                }
                else if (Selection == "Created")
                {
                    IsCreatedChecked = false;
                }
                else if (Selection == "SendBack")
                {
                    IsSendBackChecked = false;
                }
                else if (Selection == "Partially Adjusted")
                {
                    IsPartiallyAdjusted = false;
                }

                StoreReturnViews = null;
                RowCount = null;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private bool _isPreCNChecked;

        private bool _isapproveChecked;

        private bool _isadjustedChecked;

        private bool _isCreatedChecked;

        private bool _isPosCNChecked;

        private bool _isSendBackChecked;

        private bool _isPartiallyAdjustedChecked;


        public bool IsPreCNChecked
        {
            get { return _isPreCNChecked; }
            set { SetProperty(ref _isPreCNChecked, value); }
        }
        public bool IsapproveChecked
        {
            get { return _isapproveChecked; }
            set { SetProperty(ref _isapproveChecked, value); }
        }

        public bool IsadjustedChecked
        {
            get { return _isadjustedChecked; }
            set { SetProperty(ref _isadjustedChecked, value); }
        }

        public bool IsCreatedChecked
        {
            get { return _isCreatedChecked; }
            set { SetProperty(ref _isCreatedChecked, value); }
        }
        public bool IsPostCNChecked
        {
            get { return _isPosCNChecked; }
            set { SetProperty(ref _isPosCNChecked, value); }
        }

        public bool IsSendBackChecked
        {
            get { return _isSendBackChecked; }
            set { SetProperty(ref _isSendBackChecked, value); }
        }

        public bool IsPartiallyAdjusted
        {
            get { return _isPartiallyAdjustedChecked; }
            set { SetProperty(ref _isPartiallyAdjustedChecked, value); }
        }

        public async void ApprovePopView(object obj)
        {
            try
            {
                var viewUpdate = (StoreReturnModel)obj;

                _storeReturnUpdate.Id = viewUpdate.Id;
                UpdateCreditNoteNumber updateCreditNoteNumber = new UpdateCreditNoteNumber();
                updateCreditNoteNumber.DataContext = this;
                await DialogHost.Show(updateCreditNoteNumber, "RootDialog", ClosingEventHandler);



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



        public async void Edit(object obj)
        {

            try
            {
                var _viewModel = (StoreReturnModel)obj;
                if (_viewModel != null)
                {
                    ProductSKU = null;
                    LotNumber = null;
                    IsCreditNoteVisible = true;

                    EditModel = _viewModel;

                    SelectedEditSupplier = Suppliers.FirstOrDefault(x => x.SupplierId == _viewModel.SupplierId);

                    List<EditProductModel> editProductModels = new List<EditProductModel>();

                    foreach (var item in _viewModel.PurhcaseReturnProduct)
                    {

                        EditProductModel editProductModel = new EditProductModel()
                        {
                            InvoiceNo = item.InvoiceNo,
                            //InvoiceDate = item.InvoiceDate,
                            ProductName = item.ProductName,
                            ManufactureName = item.ManufactureName,
                            ProductSKU = item.ProductSKU,
                            ProductSellingPrice = item.ProductSellingPrice,
                            ProductGST = item.ProductGST,
                            ProductRate = item.ProductRate,
                            ProductTotal = item.ProductTotal,
                            Lotnumber = item.Lotnumber,
                            ReturnQty = item.ReturnQty,
                            StockProductId = item.StockProductId,
                            //ProductId = item.ProductId,
                            ProductUniqGuid = item.ProductUniqGuid,
                            //ProductSubQty=(item.RemainingStock-item.ReturnQty),
                        };
                        editProductModels.Add(editProductModel);
                    }
                    PopPurhcaseReturnProducts = new ObservableCollection<EditProductModel>(editProductModels);
                    PopDeleteReturnProducts = new ObservableCollection<EditProductModel>(); ;
                    Amount = _viewModel.PurhcaseReturnProduct.Sum(x => x.RowTotal);
                    EditProduct editProduct = new EditProduct();
                    editProduct.DataContext = this;
                    await DialogHost.Show(editProduct, "RootDialog", UpdateCreditEventHandler);
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in getting purchase invoices", _ex);
            }
        }

        private ObservableCollection<EditProductModel> _poppurhcaseReturnProduct = new ObservableCollection<EditProductModel>();
        public ObservableCollection<EditProductModel> PopPurhcaseReturnProducts
        {
            get { return _poppurhcaseReturnProduct; }

            set { SetProperty(ref _poppurhcaseReturnProduct, value); }

        }

        private ObservableCollection<EditProductModel> _popDeleteReturnProduct = new ObservableCollection<EditProductModel>();
        public ObservableCollection<EditProductModel> PopDeleteReturnProducts
        {
            get { return _popDeleteReturnProduct; }

            set { SetProperty(ref _popDeleteReturnProduct, value); }

        }

        private StoreReturnModel _editModel;

        public StoreReturnModel EditModel
        {
            get { return _editModel; }
            set { SetProperty(ref _editModel, value); }
        }


        private async void LoadData()
        {
            try
            {

                if (_cancellationTokenSource != null)
                    _cancellationTokenSource?.Cancel();

                if (SelectedSupplier == null)
                {
                    _notificationService.ShowMessage("Please select Supplier", Common.NotificationType.Error);
                    return;
                }

                if (string.IsNullOrEmpty(ProductSKU) && string.IsNullOrEmpty(LotNumber))
                {
                    _notificationService.ShowMessage("Please enter SKU or Lot Number", Common.NotificationType.Error);
                    return;
                }

                if (!string.IsNullOrEmpty(ProductSKU) && !string.IsNullOrEmpty(LotNumber))
                {
                    _notificationService.ShowMessage("Please Remove SKU or Lot Number", Common.NotificationType.Error);
                    return;
                }

                if (ProductSKU == "")
                    ProductSKU = null;
                if (LotNumber == "")
                    LotNumber = null;

                await _progressService.StartProgressAsync();

                _cancellationTokenSource = new CancellationTokenSource();

                await Task.Run(async () =>
                {
                    var _result = await _purchaseReturnService.GetStoreReturnsSearch((int)SelectedEditSupplier.SupplierId, ProductSKU, LotNumber, _cancellationTokenSource.Token);

                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        if (_result != null && _result.IsSuccess && _result.Data != null)
                        {

                            List<EditProductModel> editProductModels = new List<EditProductModel>();
                            foreach (var item in _result.Data)
                            {
                                //this is rework magesh dont create empty obj here 
                                EditProductModel editProductModel = new EditProductModel()
                                {
                                    InvoiceNo = item.InvoiceNo,
                                    //InvoiceDate = item.InvoiceDate,
                                    ProductName = item.ProductName,
                                    ManufactureName = item.ManufactureName,
                                    ProductSKU = item.ProductSKU,
                                    ProductSellingPrice = item.ProductSellingPrice,
                                    ProductGST = item.ProductGST,
                                    ProductTotal = item.ProductTotal,
                                    Lotnumber = item.Lotnumber,
                                    ReturnQty = item.ReturnQty,
                                    ProductSubQty = item.ProductSubQty,
                                    ProductRate = item.ProductRate,
                                    //ProductId=item.ProductId,
                                    StockProductId = item.StockProductId,
                                    StroreId = item.StroreId

                                };
                                if (PopPurhcaseReturnProducts?.Count > 0)
                                {
                                    var _alreadyadded = PopPurhcaseReturnProducts.Where(x => x.ProductUniqGuid == item.ProductUniqGuid).FirstOrDefault();
                                    // group by productbyguid here 
                                    var _existingProductStockCount = PopPurhcaseReturnProducts.Where(x => x.ProductUniqGuid.Equals(item.ProductUniqGuid)).FirstOrDefault();
                                    if (_existingProductStockCount != null)
                                    {
                                        PopPurhcaseReturnProducts.Where(x => x.ProductUniqGuid.Equals(item.ProductUniqGuid)).Select(x =>
                                        {
                                            x.ProductSubQty = item.ProductSubQty; return x;
                                        }).ToList();
                                    }
                                    if (_alreadyadded != null)
                                        continue;

                                }
                                editProductModels.Add(editProductModel);

                            }
                            if (editProductModels?.Count > 0)
                            {
                                if (PopPurhcaseReturnProducts?.Count > 0)
                                    PopPurhcaseReturnProducts.AddRange(editProductModels);

                                else
                                    PopPurhcaseReturnProducts = new ObservableCollection<EditProductModel>(editProductModels);
                            }


                            IsCreditNoteVisible = PopPurhcaseReturnProducts.Count > 0 ? true : false;

                            RowCount = "Row Count " + PopPurhcaseReturnProducts.Count;


                        }
                        else
                        {
                            _notificationService.ShowMessage(_result?.Error ?? "An error occurred, try again", Common.NotificationType.Error);
                        }

                    });
                }, _cancellationTokenSource.Token);


            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in getting send bank status  edit product page load method ", _ex);

            }
            finally
            {
                await _progressService.StopProgressAsync();
            }

        }

        private ObservableCollection<SuppliersViewModel> _suppliers;

        public ObservableCollection<SuppliersViewModel> Suppliers
        {
            get => _suppliers;
            set => SetProperty(ref _suppliers, value);
        }


        private SuppliersViewModel _selectedsuppliers;

        public SuppliersViewModel SelectedSupplier
        {
            get => _selectedsuppliers;
            set => SetProperty(ref _selectedsuppliers, value);
        }

        private ObservableCollection<SuppliersViewModel> _suppliersEdit;

        public ObservableCollection<SuppliersViewModel> SuppliersEdit
        {
            get => _suppliersEdit;
            set => SetProperty(ref _suppliersEdit, value);
        }


        private SuppliersViewModel _selectedEditsuppliers;

        public SuppliersViewModel SelectedEditSupplier
        {
            get => _selectedEditsuppliers;
            set => SetProperty(ref _selectedEditsuppliers, value);
        }
        public void ReturnQtyTextChange(object obj)
        {
            try
            {
                Amount = 0;
                foreach (var item in PopPurhcaseReturnProducts)
                {

                    if (item.ReturnQty != 0)
                    {
                        Amount += item.RowTotal;
                    }

                }

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in selected product", _ex);
            }
        }

        private bool _iscreditnoteVisible;

        public bool IsCreditNoteVisible
        {
            get { return _iscreditnoteVisible; }

            set { SetProperty(ref _iscreditnoteVisible, value); }
        }



        private string _productSKU;
        public string ProductSKU
        {
            get { return _productSKU; }
            set { SetProperty(ref _productSKU, value); }
        }

        private string _lotNumber;
        public string LotNumber
        {
            get { return _lotNumber; }
            set { SetProperty(ref _lotNumber, value); }
        }


        private float _amount;

        public float Amount
        {
            get { return (float)Math.Round(_amount, MidpointRounding.AwayFromZero); }
            set { SetProperty(ref _amount, value); }
        }

        private void ResetLoadData()
        {
            try
            {
                if (PopPurhcaseReturnProducts != null && PopPurhcaseReturnProducts.Count > 0)
                {
                    PopPurhcaseReturnProducts.Clear();

                    RowCount = null;

                    ProductSKU = null;

                    LotNumber = null;


                    Amount = 0;

                    SelectedEditSupplier = null;

                    IsCreditNoteVisible = false;
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in getting purchase invoices", _ex);
            }
        }

        private void RemovePurhcaseProductCard(object obj)
        {
            try
            {
                var _viewModel = (EditProductModel)obj;

                if (_viewModel != null)
                {
                    PopPurhcaseReturnProducts.Remove(_viewModel);

                    RowCount = "Row Count " + PopPurhcaseReturnProducts.Count;

                    PopDeleteReturnProducts.Add(_viewModel);
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in getting purchase invoices", _ex);

            }
        }
        private async void LoadSuppliers()
        {

            try
            {
                _logger.LogInformation("Getting suppliers ");

                await Task.Run(async () =>
                {
                    var _result = await _supplierService.GetSuppliers("isenabled=true");

                    if (_result != null && _result.Any())
                    {
                        Application.Current?.Dispatcher?.Invoke(() =>
                        {

                            _logger.LogInformation($"Suppliers get success --count-{_result.Count()}");

                            var addAllList = _result.ToList();
                            addAllList.Insert(0, new SuppliersViewModel() { SupplierId = -1, Name = "All" });
                            _result = addAllList;

                            Suppliers = new ObservableCollection<SuppliersViewModel>(_result);
                            SuppliersEdit = new ObservableCollection<SuppliersViewModel>(_result);
                            SelectedSupplier = _result.FirstOrDefault();

                        });
                    }

                });

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in loading suppliers", _ex);
            }
        }

        public void UpdateProduct(object obj)
        {
            try
            {

                if (PopPurhcaseReturnProducts == null || PopPurhcaseReturnProducts.Count == 0)
                {
                    _notificationService.ShowMessage("Please check empty product list", Common.NotificationType.Error);
                    return;
                }
                if (PopPurhcaseReturnProducts != null)
                {
                    if (SelectedEditSupplier == null)
                    {
                        _notificationService.ShowMessage("Please select supplier", Common.NotificationType.Error);
                        return;
                    }


                    if (PopPurhcaseReturnProducts.Count > 0)
                    {
                        int i = 1;
                        foreach (var item in PopPurhcaseReturnProducts)
                        {
                            if (item.ReturnQty == 0)
                            {
                                _notificationService.ShowMessage("Please enter return qty at  row " + i + " in  SKU " + item.ProductSKU, Common.NotificationType.Error);
                                return;
                            }

                            //if (item.ReturnQty > item.ProductSubQty)
                            //{
                            //    _notificationService.ShowMessage("Return qty should not above pos stock qty  row " + i + " in  Sku " + item.ProductSKU, Common.NotificationType.Error);
                            //    return;
                            //}

                            if (!item.IsSelected)
                            {
                                _notificationService.ShowMessage("Please select row " + i + " in  SKU " + item.ProductSKU, Common.NotificationType.Error);
                                return;
                            }
                            i++;
                        }
                    }

                    var _amount = PopPurhcaseReturnProducts.Sum(x => x.RowTotal);

                    if (Math.Round(Amount, MidpointRounding.AwayFromZero) != Math.Round(_amount, MidpointRounding.AwayFromZero))
                    {
                        _notificationService.ShowMessage("Please check Total Amount and product row Total Amount should match", Common.NotificationType.Error);
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
                _logger.LogError("Error in selected product", _ex);
                _notificationService.ShowMessage(_ex.Message, Common.NotificationType.Warning);
            }
            finally
            {

            }
        }

        private async void UpdateCreditEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {
                var view = (StoreViewReturnViewModel)eventArgs.Parameter;
                if (view != null)
                {
                    EditStoreReturnModel storeReturnModel = new EditStoreReturnModel();
                    List<SendbackEditProductModel> editProductModels = new List<SendbackEditProductModel>();
                    foreach (var item in PopPurhcaseReturnProducts)
                    {

                        SendbackEditProductModel editProductModel = new SendbackEditProductModel()
                        {
                            ProductSKU = item.ProductSKU,

                            ReturnQty = item.ReturnQty,

                            StockProductId = item.StockProductId,

                            ProductRate = item.ProductRate,

                            StoreId = item.StroreId,

                        };
                        editProductModels.Add(editProductModel);
                    }
                    storeReturnModel.PurhcaseReturnProduct = new ObservableCollection<SendbackEditProductModel>(editProductModels);
                    storeReturnModel.SupplierId = (int)SelectedEditSupplier?.SupplierId;
                    storeReturnModel.Id = EditModel.Id;
                    storeReturnModel.Total = (float)PopPurhcaseReturnProducts?.Sum(x => x.RowTotal);
                    List<DeleteProductModel> DeleteProductModels = new List<DeleteProductModel>();
                    foreach (var item in PopDeleteReturnProducts)
                    {

                        DeleteProductModel deleteProductModel = new DeleteProductModel()
                        {

                            StockProductId = item.StockProductId,
                            ProductId = item.ProductId,

                        };
                        DeleteProductModels.Add(deleteProductModel);
                    }
                    storeReturnModel.DeleteReturnProduct = new ObservableCollection<DeleteProductModel>(DeleteProductModels);

                    await _progressService.StartProgressAsync();

                    await Task.Run(async () =>
                    {
                        var _result = await _purchaseReturnService.EditReturnProduct(storeReturnModel, _cancellationTokenSource.Token);

                        Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            if (_result != null && _result.IsSuccess && _result.Data != null)
                            {


                                _notificationService.ShowMessage(_result.Data, Common.NotificationType.Success);
                            }
                            else
                            {
                                _notificationService.ShowMessage(_result?.Error ?? "An error occurred, try again", Common.NotificationType.Error);
                            }

                        });
                    }, _cancellationTokenSource.Token);

                    await _progressService.StopProgressAsync();

                    LoadData("SendBack");
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

        public async void SelectionChange(object obj)
        {
            try
            {
                _cancellationTokenSource = new CancellationTokenSource();

                await _progressService.StartProgressAsync();

                //var Selection = Convert.ToString(obj);

                if (SelectedSupplier == null)
                {
                    _notificationService.ShowMessage("Please select supplier", Common.NotificationType.Error);
                    return;
                }


                if (IsPreCNChecked == false && IsapproveChecked == false && IsadjustedChecked == false && IsPostCNChecked == false && IsSendBackChecked == false && IsCreatedChecked == false && IsPartiallyAdjusted == false)
                {
                    _notificationService.ShowMessage("Please select the any one status", Common.NotificationType.Error);
                    return;
                }
                //if (Selection == "Created")
                //{

                //    IsapproveChecked = false;
                //    IsadjustedChecked = false;
                //}
                //else if (Selection == "Approved")
                //{
                //    IscreatedChecked = false;
                //    IsadjustedChecked = false;
                //}
                //else if (Selection == "Adjusted")
                //{
                //    IscreatedChecked = false;
                //    IsapproveChecked = false;
                //}

                var status = IsPreCNChecked ? "Pre-C/N" : IsapproveChecked ? "Pre-C/N-Approved" : IsPostCNChecked ? "Post-C/N" : IsCreatedChecked ? "Created" : IsSendBackChecked ? "SendBack" : IsadjustedChecked ? "Adjusted" : IsPartiallyAdjusted ? "Partially Adjusted" : "";


                int SupplierId = (int)SelectedSupplier.SupplierId;


                if (!string.IsNullOrEmpty(status))
                {
                    await Task.Run(async () =>
                    {

                        var _result = await _purchaseReturnService.GetStorePurchaseReturnsListView(status, SupplierId);

                        Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            if (_result != null && _result.IsSuccess && _result.Data != null)
                            {
                                List<StoreReturnModel> returnModels = new List<StoreReturnModel>();
                                foreach (var item in _result.Data)
                                {
                                    var istore = AppConstants.USER_ROLES[0] == AppConstants.ROLE_STORE_PERSON ? true : false;
                                    returnModels.Add(new StoreReturnModel()
                                    {

                                        CreditNoteNumber = item.CreditNoteNumber,
                                        CreditNoteDate = item.CreditNoteDate,
                                        Total = item.Total,
                                        Id = item.Id,
                                        SupplierId = item.SupplierId,
                                        SupplierName = item.SupplierName,
                                        Status = item.Status,
                                        Remark = item.Remark,
                                        CreatedDatetime = item.CreatedDatetime,
                                        PurhcaseReturnProduct = item.PurhcaseReturnProduct,
                                        StoreName = item.StoreName,
                                        FileID = item.FileID,
                                        IsEnableUpdateBtn = (istore && item.Status == "Pre-C/N-Approved") ? true : false,
                                        IsEnableProductEditbtn = (istore && item.Status == "SendBack") ? true : false,
                                        AdjustModels = (istore && item.Status == "Adjusted") || (istore && item.Status == "Partially Adjusted") ? item.AdjustModels : null
                                    });

                                }
                                StoreReturnViews = new ObservableCollection<StoreReturnModel>(returnModels);

                                RowCount = "Row Count " + StoreReturnViews.Count();

                            }
                            else
                            {
                                _notificationService.ShowMessage(_result?.Error, Common.NotificationType.Error);
                                RowCount = null;
                                StoreReturnViews = null;
                            }

                        });
                    }, _cancellationTokenSource.Token);

                }


                await _progressService.StopProgressAsync();
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


        private async void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {

            try
            {
                var _viewModel = (StoreViewReturnViewModel)eventArgs.Parameter;
                if (_viewModel != null)
                {

                    await _progressService.StartProgressAsync();

                    _storeReturnUpdate.CreditNoteNumber = _viewModel.CreditNoteNumber;
                    _storeReturnUpdate.CreditNoteDate = _viewModel.CreditNoteDate;
                    _storeReturnUpdate.FileUploadListInfo = _viewModel.FileUploadListInfo;



                    await Task.Run(async () =>
                    {
                        var _result = await _purchaseReturnService.UpdateStoreReturnWithAttachment(_storeReturnUpdate, _storeReturnUpdate.FileUploadListInfo?.ToArray());

                        Application.Current?.Dispatcher?.Invoke(async () =>
                        {
                            if (_result != null && _result.IsSuccess && _result.Data != null)
                            {
                                _notificationService.ShowMessage(_result?.Data, Common.NotificationType.Success);
                                await _progressService.StopProgressAsync();
                                LoadData("Pre-C/N-Approved");


                            }
                            else
                            {
                                _notificationService.ShowMessage(_result?.Error, Common.NotificationType.Error);
                                await _progressService.StopProgressAsync();

                            }

                        });
                    }, _cancellationTokenSource.Token);

                    await _progressService.StopProgressAsync();
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


        public void UpdateApprove(object obj)
        {
            try
            {

                if (string.IsNullOrEmpty(CreditNoteNumber))
                {

                    _notificationService.ShowMessage("Please add credit note number", Common.NotificationType.Error);
                    return;
                }

                if (string.IsNullOrEmpty(CreditNoteDate))
                {
                    _notificationService.ShowMessage("Please add credit note date", Common.NotificationType.Error);
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

        public void FileOpenDialog(object obj)
        {
            try
            {


                if (FileUploadListInfo != null && FileUploadListInfo.Count >= 1)
                {
                    _notificationService.ShowMessage("1 files already added. Delete old file", NotificationType.Information);

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


                if (dialog != null)
                {
                    if (FileUploadListInfo == null)
                    {
                        FileUploadListInfo = new ObservableCollection<FileUploadInfo>();
                    }
                    if (dialog.FileNames != null && dialog.FileNames.Count() > 1)
                    {
                        _notificationService.ShowMessage("Can't add more than 1 file", NotificationType.Information);

                        _logger.LogWarning($"Max File attachments added");

                        return;
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
                                    Size = FileHelper.FormatSize(_fileInfo.Length),
                                    FileSrc = FileSrc.local,

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
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private void DeleteUploadFile(Guid? fileId)
        {
            try
            {
                if (fileId == null) return;


                var _viewModel = FileUploadListInfo.FirstOrDefault(x => x.FileId == fileId);

                if (_viewModel != null)
                {
                    _logger.LogInformation($"File Deleted {FileUploadListInfo.FirstOrDefault().FileName}");

                    FileUploadListInfo?.Clear();
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }


        }


        public void Refresh(object obj)
        {
            try
            {

                SelectedSupplier = null;
                StoreReturnViews = null;
                IsPreCNChecked = false;
                IsPostCNChecked = false;
                IsapproveChecked = false;
                IsCreatedChecked = false;
                IsSendBackChecked = false;
                IsadjustedChecked = false;
                IsPartiallyAdjusted = false;

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public async void GetData(object obj)
        {
            try
            {

                if (SelectedSupplier == null)
                {
                    _notificationService.ShowMessage("Please select supplier", NotificationType.Error);
                    return;
                }
                if (IsPartiallyAdjusted == false && IsadjustedChecked == false && IsPreCNChecked == false && IsPostCNChecked == false && IsapproveChecked == false && IsCreatedChecked == false && IsSendBackChecked == false)
                {
                    _notificationService.ShowMessage("please select any one status", NotificationType.Error);
                    return;
                }
                _cancellationTokenSource = new CancellationTokenSource();

                await _progressService.StartProgressAsync();

                var status = IsPreCNChecked ? "Pre-C/N" : IsapproveChecked ? "Pre-C/N-Approved" : IsPostCNChecked ? "Post-C/N" : IsCreatedChecked ? "Created" : IsSendBackChecked ? "SendBack" : IsadjustedChecked ? "Adjusted" : IsPartiallyAdjusted ? "Partially Adjusted" : "";


                int SupplierId = (int)SelectedSupplier.SupplierId;

                if (!string.IsNullOrEmpty(status))
                {


                    await Task.Run(async () =>
                    {

                        var _result = await _purchaseReturnService.GetStorePurchaseReturnsListView(status, SupplierId);

                        Application.Current?.Dispatcher?.Invoke(() =>
                        {

                            if (_result != null && _result.IsSuccess && _result.Data != null && _result.Data.Count() > 0)
                            {
                                List<StoreReturnModel> returnModels = new List<StoreReturnModel>();
                                foreach (var item in _result.Data)
                                {
                                    var istore = AppConstants.USER_ROLES[0] == AppConstants.ROLE_STORE_PERSON ? true : false;
                                    returnModels.Add(new StoreReturnModel()
                                    {
                                        CreditNoteNumber = item.CreditNoteNumber,
                                        CreditNoteDate = item.CreditNoteDate,
                                        Total = item.Total,
                                        Id = item.Id,
                                        SupplierId = item.SupplierId,
                                        SupplierName = item.SupplierName,
                                        Status = item.Status,
                                        Remark = item.Remark,
                                        CreatedDatetime = item.CreatedDatetime,
                                        PurhcaseReturnProduct = item.PurhcaseReturnProduct,
                                        StoreName = item.StoreName,
                                        FileID = item.FileID,
                                        IsEnableUpdateBtn = (istore && item.Status == "Pre-C/N-Approved") ? true : false,
                                        IsEnableProductEditbtn = (istore && item.Status == "SendBack") ? true : false,
                                        AdjustModels = (istore && item.Status == "Adjusted") || (istore && item.Status == "Partially Adjusted") ? item.AdjustModels : null
                                    });

                                }
                                StoreReturnViews = new ObservableCollection<StoreReturnModel>(returnModels);

                                RowCount = "Row Count " + StoreReturnViews.Count();

                            }
                            else
                            {
                                _notificationService.ShowMessage(_result?.Error, Common.NotificationType.Error);
                                RowCount = null;
                                StoreReturnViews = null;
                            }

                        });
                    }, _cancellationTokenSource.Token);
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
        private string _creditNoteDate;
        public string CreditNoteDate
        {
            get => _creditNoteDate;

            set => SetProperty(ref _creditNoteDate, value);
        }

        private string _creditNoteNumber;
        public string CreditNoteNumber
        {
            get => _creditNoteNumber;

            set => SetProperty(ref _creditNoteNumber, value);
        }

        private ObservableCollection<FileUploadInfo> fileUploadListInfo;
        public ObservableCollection<FileUploadInfo> FileUploadListInfo
        {
            get => fileUploadListInfo;
            set => SetProperty(ref fileUploadListInfo, value);
        }


    }
}




