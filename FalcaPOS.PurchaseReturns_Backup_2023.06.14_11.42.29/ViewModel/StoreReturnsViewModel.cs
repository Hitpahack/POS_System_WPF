using FalcaPOS.Common;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.AddInventory;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.Suppliers;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.PurchaseReturns.ViewModel
{
    public class StoreReturnsViewModel : BindableBase
    {

        private readonly IPurchaseReturnService _purchaseReturnService;

        private CancellationTokenSource _cancellationTokenSource;


        private readonly Logger _logger;

        private readonly INotificationService _notificationService;

        private readonly IStoreService _storeService;

        private readonly ProgressService _progressService;
        public DelegateCommand PurhcaseReturnCommand { get; private set; }
        public DelegateCommand PurchaseResetReturnCommand { get; private set; }

        public DelegateCommand<object> RemovePurhcaseCommand { get; private set; }

        public DelegateCommand<object> PurchaseReturnSubmitCommand { get; private set; }


        private readonly IInvoiceFileService _invoiceFileService;


        public DelegateCommand<object> AddFileAttachement { get; private set; }

        public DelegateCommand<Guid?> DeleteUploadFileCommand { get; private set; }


        private readonly ISupplierService _supplierService;


        public DelegateCommand<object> ProductSelectCommand { get; private set; }

        public DelegateCommand<object> TexChangeQtyCommand { get; private set; }

        public StoreReturnsViewModel(ISupplierService supplierService, IInvoiceFileService InvoiceFileService, IPurchaseReturnService purchaseReturnService, Logger logger, IStoreService storeService, INotificationService notificationService, ProgressService progressService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _purchaseReturnService = purchaseReturnService ?? throw new ArgumentNullException(nameof(purchaseReturnService));

            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _progressService = progressService;

            _invoiceFileService = InvoiceFileService ?? throw new ArgumentNullException(nameof(InvoiceFileService));

            _supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));

            PurhcaseReturnCommand = new DelegateCommand(LoadData);

            PurchaseResetReturnCommand = new DelegateCommand(ResetLoadData);

            RemovePurhcaseCommand = new DelegateCommand<object>(RemovePurchaseProductCard);

            PurchaseReturnSubmitCommand = new DelegateCommand<object>(SubmitPurchaseReturn);

            AddFileAttachement = new DelegateCommand<object>(FileOpenDialog);

            DeleteUploadFileCommand = new DelegateCommand<Guid?>(DeleteUploadFile);

            ProductSelectCommand = new DelegateCommand<object>(SelectedProduct);

            TexChangeQtyCommand = new DelegateCommand<object>(ReturnQtyTextChange);

            LoadSuppliers();

        }
        private void ResetLoadData()
        {
            try
            {
                if (PurhcaseReturnProducts != null && PurhcaseReturnProducts.Count > 0)
                {
                    PurhcaseReturnProducts.Clear();


                    IsCreditNoteVisible = false;
                }

                RowCount = null;

                ProductSKU = null;

                LotNumber = null;


                Amount = 0;

                SelectedSupplier = null;

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in getting purchase invoices", _ex);
            }
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
                    _notificationService.ShowMessage("Please remove SKU or Lot Number", Common.NotificationType.Error);
                    return;
                }

                //check same supplier product only in cart
                if (PurhcaseReturnProducts?.Count > 0)
                {
                    var _existingSupplier = PurhcaseReturnProducts?.GroupBy(x => x.SupplierName).Select(x => x).ToList();
                    if (_existingSupplier?.Count == 1)
                    {
                        if (_existingSupplier.FirstOrDefault().Key != SelectedSupplier.Name)
                        {
                            _notificationService.ShowMessage("Supplier is different.Please add same supplier product", Common.NotificationType.Error);
                            return;
                        }
                    }
                    else if (_existingSupplier?.Count > 1)
                    {
                        _notificationService.ShowMessage("Multiple suppliers record added.Please remove all product and try again", Common.NotificationType.Error);
                        return;
                    }
                }

                if (ProductSKU == "")
                    ProductSKU = null;
                if (LotNumber == "")
                    LotNumber = null;

                await _progressService.StartProgressAsync();

                _cancellationTokenSource = new CancellationTokenSource();

                await Task.Run(async () =>
                {
                    var _result = await _purchaseReturnService.GetStoreReturnsSearch((int)SelectedSupplier.SupplierId, ProductSKU, LotNumber, _cancellationTokenSource.Token);

                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        if (_result != null && _result.IsSuccess && _result.Data != null)
                        {

                            foreach (var item in _result.Data)
                            {
                                if (PurhcaseReturnProducts != null && PurhcaseReturnProducts.Count > 0)
                                {
                                    var _alreadyAdded = PurhcaseReturnProducts.Where(x => x.StockProductId == item.StockProductId).Select(s => { s.ProductSubQty = item.ProductSubQty; return s; }).FirstOrDefault();

                                    if (_alreadyAdded == null)
                                        PurhcaseReturnProducts.Add(item);
                                    else
                                        _notificationService.ShowMessage("Record is already added", Common.NotificationType.Error);
                                }
                                else
                                    PurhcaseReturnProducts.Add(item);
                            }

                            IsCreditNoteVisible = PurhcaseReturnProducts.Count > 0 ? true : false;

                            RowCount = "Row Count " + PurhcaseReturnProducts.Count;


                        }
                        else
                        {
                            _notificationService.ShowMessage(_result?.Error ?? "An error occurred, try again", Common.NotificationType.Error);
                        }

                    });
                }, _cancellationTokenSource.Token);

                await _progressService.StopProgressAsync();
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in getting purchase invoices", _ex);

            }
            finally
            {
                await _progressService.StopProgressAsync();
            }

        }

        private void RemovePurchaseProductCard(object obj)
        {
            try
            {
                var _viewModel = (PurchaseReturnProductViewModel)obj;

                if (_viewModel != null)
                {
                    PurhcaseReturnProducts.Remove(_viewModel);

                    RowCount = "Row Count " + PurhcaseReturnProducts.Count;

                    if (PurhcaseReturnProducts.Count == 0)
                    {
                        IsCreditNoteVisible = false;
                    }
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in getting purchase invoices", _ex);

            }
        }


        public async void SubmitPurchaseReturn(object obj)
        {
            try
            {
                var _view = (StoreReturnsViewModel)obj;

                if (_view != null)
                {
                    if (_view.SelectedSupplier == null)
                    {
                        _notificationService.ShowMessage("Please select supplier", Common.NotificationType.Error);
                        return;
                    }


                    if (_view.PurhcaseReturnProducts.Count > 0)
                    {
                        int i = 1;
                        foreach (var item in _view.PurhcaseReturnProducts)
                        {
                            if (item.ReturnQty == 0)
                            {
                                _notificationService.ShowMessage("Please enter return qty at  row " + i + " in  SKU " + item.ProductSKU, Common.NotificationType.Error);
                                return;
                            }

                            if (item.ReturnQty > item.ProductSubQty)
                            {
                                _notificationService.ShowMessage("Return qty should not above pos stock qty  row " + i + " in  SKU " + item.ProductSKU, Common.NotificationType.Error);
                                return;
                            }

                            if (!item.IsSelected)
                            {
                                _notificationService.ShowMessage("Please select row " + i + " in  SKU " + item.ProductSKU, Common.NotificationType.Error);
                                return;
                            }
                            i++;
                        }
                    }

                    var _amount = Math.Round(_view.PurhcaseReturnProducts.Sum(x => x.RowTotal), MidpointRounding.AwayFromZero);

                    if (Math.Round(Amount, MidpointRounding.AwayFromZero) != _amount)
                    {
                        _notificationService.ShowMessage("Please check Total Amount and product row Total Amount should match", Common.NotificationType.Error);
                        return;
                    }

                    List<CreateReturnProductModel> createReturns = new List<CreateReturnProductModel>();
                    foreach (var item in PurhcaseReturnProducts)
                    {
                        createReturns.Add(new CreateReturnProductModel()
                        {
                            StockProductId = item.StockProductId,
                            StroreId = item.StroreId,
                            ProductSKU = item.ProductSKU,
                            ReturnQty = item.ReturnQty
                        });
                    }

                    StoreReturnModels storeReturnModels = new StoreReturnModels();
                    storeReturnModels.SupplierId = (int)SelectedSupplier?.SupplierId;
                    storeReturnModels.Total = (float)Math.Round(Amount, MidpointRounding.AwayFromZero);

                    storeReturnModels.PurhcaseReturnProduct = new ObservableCollection<CreateReturnProductModel>(createReturns);


                    if (_cancellationTokenSource != null)
                        _cancellationTokenSource?.Cancel();

                    await _progressService.StartProgressAsync();

                    _cancellationTokenSource = new CancellationTokenSource();

                    await Task.Run(async () =>
                    {
                        var _result = await _purchaseReturnService.UpdateStorePurchaseReturns(storeReturnModels, _cancellationTokenSource.Token);

                        Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            if (_result != null && _result.IsSuccess && _result.Data != null)
                            {
                                _notificationService.ShowMessage(_result.Data, Common.NotificationType.Success);

                                ResetLoadData();
                            }
                            else
                            {
                                _notificationService.ShowMessage(_result?.Error ?? "An error occurred, try again", Common.NotificationType.Error);
                            }

                        });
                    }, _cancellationTokenSource.Token);

                    await _progressService.StopProgressAsync();
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in getting submit  purchase returns", _ex);


            }
            finally
            {
                await _progressService.StopProgressAsync();
            }
        }

        public void FileOpenDialog(object bank)
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



        private ObservableCollection<PurchaseReturnProductViewModel> _purhcaseReturnProduct = new ObservableCollection<PurchaseReturnProductViewModel>();
        public ObservableCollection<PurchaseReturnProductViewModel> PurhcaseReturnProducts
        {
            get => _purhcaseReturnProduct;
            set => SetProperty(ref _purhcaseReturnProduct, value);
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

        private string _rowcount;
        public string RowCount
        {
            get { return _rowcount; }

            set { SetProperty(ref _rowcount, value); }
        }

        private ObservableCollection<FileUploadInfo> fileUploadListInfo;
        public ObservableCollection<FileUploadInfo> FileUploadListInfo
        {
            get { return fileUploadListInfo; }
            set { SetProperty(ref fileUploadListInfo, value); }
        }


        private string _creditNoteDate;
        public string CreditNoteDate
        {
            get { return _creditNoteDate; }
            set { SetProperty(ref _creditNoteDate, value); }
        }


        private string _creditNoteNumber;

        public string CreditNoteNumber
        {
            get { return _creditNoteNumber; }
            set { SetProperty(ref _creditNoteNumber, value); }
        }

        private float _amount;

        public float Amount
        {
            get { return _amount; }
            set { SetProperty(ref _amount, value); }
        }

        private string _remark;

        public string Remark
        {
            get { return _remark; }
            set { SetProperty(ref _remark, value); }
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

                            Suppliers = new ObservableCollection<SuppliersViewModel>(_result);


                        });
                    }

                });

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in loading suppliers", _ex);
            }
        }

        private ObservableCollection<SuppliersViewModel> _suppliers;

        public ObservableCollection<SuppliersViewModel> Suppliers
        {
            get => _suppliers;
            set => SetProperty(ref _suppliers, value);
        }


        private SuppliersViewModel _selectedSuppliers;

        public SuppliersViewModel SelectedSupplier
        {
            get => _selectedSuppliers;
            set => SetProperty(ref _selectedSuppliers, value);
        }

        public void SelectedProduct(object obj)
        {
            try
            {
                var _productModel = (StockProductViewModel)obj;
                if (_productModel != null && _productModel.IsSelected)
                {
                    Amount += _productModel.RowTotal;
                }
                else
                {
                    var total = _productModel.RowTotal;
                    Amount = (Amount - total);
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in selected product", _ex);
            }
        }

        public void ReturnQtyTextChange(object obj)
        {
            try
            {
                Amount = 0;
                foreach (var item in PurhcaseReturnProducts)
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

    }


}
