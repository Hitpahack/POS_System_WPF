using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.AddInventory;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.Stores;
using FalcaPOS.Invoice.Models;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.Invoice.ViewModels
{
    public class InvoiceHomeViewModel : BindableBase
    {
        private readonly IPurchaseInvoiceService _purchaseInvoiceService;

        private CancellationTokenSource _cancellationTokenSource;

        private readonly INotificationService _notificationService;

        private readonly Logger _logger;

        public DelegateCommand<int?> AddFilesCommand { get; private set; }

        public DelegateCommand<int?> DownloadFileCommand { get; private set; }

        private readonly IInvoiceFileService _invoiceFileService;

        public DelegateCommand<Guid?> DeleteUploadFileCommand { get; private set; }

        public DelegateCommand SearchPurchaseInvoiceCommand { get; private set; }
        public DelegateCommand<object> FetchProductCommand { get; private set; }

        public DelegateCommand<int?> UploadInvoiceFilesCommand { get; private set; }

        public DelegateCommand<PurchaseInvoiceModel> EditDcNumberCommand { get; private set; }


        private readonly IStoreService _storeService;

        private readonly IDialogService _dialogService;

        private readonly ProgressService _progressService;

        public DelegateCommand RefreshCommand { get; private set; }

        private readonly IEventAggregator _eventAggregator;

        public DelegateCommand<int?> DeleteFileCommand { get; private set; }

        public DelegateCommand<object> DownloadInvoiceListCommand { get; private set; }


        public DelegateCommand<object> SelectAllInvoiceListCommand { get; private set; }


        public DelegateCommand<object> UnSelectAllInvoiceListCommand { get; private set; }


        public DelegateCommand<bool?> OnCheckedCommand { get; private set; }

        public DelegateCommand<bool?> UnOnCheckedCommand { get; private set; }



        public InvoiceHomeViewModel(IPurchaseInvoiceService purchaseInvoiceService,
                                    Logger logger,
                                    INotificationService notificationService,
                                    IInvoiceFileService invoiceFileService,
                                    IStoreService storeService,
                                    IDialogService dialogService,
                                    ProgressService progressService,
                                    IEventAggregator eventAggregator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _purchaseInvoiceService = purchaseInvoiceService ?? throw new ArgumentNullException(nameof(purchaseInvoiceService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            AddFilesCommand = new DelegateCommand<int?>(AddFilesDialog);

            DownloadFileCommand = new DelegateCommand<int?>(DownloadFile);

            _invoiceFileService = invoiceFileService ?? throw new ArgumentNullException(nameof(invoiceFileService));

            SearchPurchaseInvoiceCommand = new DelegateCommand(SearchPurchaseInvoice);

            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));

            DeleteUploadFileCommand = new DelegateCommand<Guid?>(DeleteUploadFile);

            UploadInvoiceFilesCommand = new DelegateCommand<int?>(UploadInvoiceFiles);

            EditDcNumberCommand = new DelegateCommand<PurchaseInvoiceModel>(EditDcNumber);

            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            FetchProductCommand = new DelegateCommand<object>(GetProductDetails);

            RefreshCommand = new DelegateCommand(RefreshDataGrid);

            //LoadInvoicesAsync(GetDefaultSearchDate());

            DeleteFileCommand = new DelegateCommand<int?>(DeleteFileAttachment);

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _eventAggregator.GetEvent<SignalRStoreAddedEvent>().Subscribe(NewStoreAdded, ThreadOption.PublisherThread);



            DownloadInvoiceListCommand = new DelegateCommand<object>(DownloadInvoice);

            SelectAllInvoiceListCommand = new DelegateCommand<object>(SellectAllInvocie);

            UnSelectAllInvoiceListCommand = new DelegateCommand<object>(UnSelectAllInvocie);

            OnCheckedCommand = new DelegateCommand<bool?>(CheckedDc);

            UnOnCheckedCommand = new DelegateCommand<bool?>(UnCheckedDc);

            LoadStoresAsync();

        }

        public void CheckedDc(bool? isDc)
        {
            try
            {
                FromtoDateVisibilty = true;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public void UnCheckedDc(bool? isDc)
        {
            try
            {
                FromtoDateVisibilty = false;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public void UnSelectAllInvocie(object invoicelist)
        {
            try
            {
                var Viewmodel = (InvoiceHomeViewModel)invoicelist;

                if (Viewmodel != null && Viewmodel.InvoiceList.Count > 0)
                {

                    foreach (var item in Viewmodel.InvoiceList)
                    {
                        item.IsDownloadInvoice = false;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
        public void SellectAllInvocie(object invoicelist)
        {
            try
            {
                var Viewmodel = (InvoiceHomeViewModel)invoicelist;

                if (Viewmodel != null && Viewmodel.InvoiceList.Count > 0)
                {
                    int i = 1;
                    foreach (var item in Viewmodel.InvoiceList)
                    {
                        if (i <= 30)
                        {
                            item.IsDownloadInvoice = true;
                        }
                        else
                        {
                            _notificationService.ShowMessage("Can not download more than 30 invoice pdf", NotificationType.Error);
                            return;
                        }
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public async void DownloadInvoice(object invoiceList)
        {
            try
            {
                var Viewmodel = (InvoiceHomeViewModel)invoiceList;

                if (Viewmodel != null && Viewmodel.InvoiceList.Count > 0)
                {
                    if (!Viewmodel.InvoiceList.Where(x => x.IsDownloadInvoice == true).Any())
                    {
                        _notificationService.ShowMessage("Please select invoice number or select all invoice number", Common.NotificationType.Error);
                        return;
                    }

                    List<int> FileIds = new List<int>();

                    foreach (var item in Viewmodel.InvoiceList.Where(x => x.IsDownloadInvoice == true))
                    {
                        if (item.FileAttachments != null && item.FileAttachments.Count > 0)
                        {
                            foreach (var ids in item.FileAttachments)
                            {
                                FileIds.Add(ids.FileId);
                            }
                        }

                    }

                    if (FileIds != null && FileIds.Count > 0)
                    {
                        await _progressService.StartProgressAsync();

                        PopupClose = false;

                        var _result = await _invoiceFileService.DownloadFileList(FileIds);

                        if (_result != null)
                        {
                            if (_result != null && _result.IsSuccess && _result.Data != null)
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
                                    using (MemoryStream ms = new MemoryStream())
                                    {

                                        using (ZipArchive archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
                                        {

                                            foreach (var file in _result.Data)
                                            {
                                                var fileName = Regex.Replace(file.FileName, "[^0-9A-Za-z]+", ""); ;
                                                ZipArchiveEntry entry = archive.CreateEntry(fileName + ".zip");
                                                using (Stream zipStream = entry.Open())
                                                {
                                                    zipStream.Write(file.FileStream, 0, file.FileStream.Length);
                                                }

                                            }
                                        }
                                        // Currently method returns void?
                                        // return File(ms.ToArray(), "application/zip", "sArchive.zip");
                                        // Maybe you want
                                        File.WriteAllBytes(sd.FileName, ms.ToArray());
                                    }


                                }



                            }
                            else
                            {
                                _notificationService.ShowMessage(_result?.Error ?? "An error occurred, try again", Common.NotificationType.Error);
                            }
                        }

                    }
                    await _progressService.StopProgressAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
        private void NewStoreAdded(object str)
        {
            try
            {
                _logger.LogInformation("New Store added in invoice home vm");

                if (str is Store _str)
                {
                    Application.Current?.Dispatcher?.Invoke(delegate
                    {

                        if (Stores == null)
                        {
                            _logger.LogInformation("Stores collection is null in  invoice home vm");

                            Stores = new ObservableCollection<Store> { _str };

                            return;
                        }

                        if (Stores.Any(x => x.Name == _str.Name))
                        {
                            _logger.LogInformation($"Store already added in  invoice home vm");

                            return;
                        }
                        Stores.Add(_str);
                        Stores = new ObservableCollection<Store>(Stores.OrderBy(x => x.Name));
                    });


                }
            }
            catch (Exception _ex)
            {
                _logger?.LogError("error in adding new  store to list in  invoice home vm", _ex);
            }

        }

        private async void DeleteFileAttachment(int? fileId)
        {
            try
            {
                {
                    var _confirm = await _progressService
                  .ConfirmAsync("Delete File!", $"Deleting this file will also delete all sub file this zip file contain  {Environment.NewLine} {Environment.NewLine}Do you want to procceed?");


                    if (_confirm == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative)
                    {
                        _logger.LogInformation($"User deleting file id {fileId}");


                        await _progressService.StartProgressAsync();


                        int _invoiceId = default;

                        foreach (var _invoice in InvoiceList)
                        {

                            if (_invoice.FileAttachments != null && _invoice.FileAttachments.Any(x => x.FileId == fileId.Value))
                            {
                                _invoiceId = _invoice.InvoiceId;
                                break;
                            }

                        }


                        if (_invoiceId <= 0)
                        {
                            return;
                        }

                        await Task.Run(async () =>
                        {

                            var _result = await _invoiceFileService.DeleteFileById(_invoiceId, fileId.Value);

                            Application.Current?.Dispatcher?.Invoke(async () =>
                            {

                                await _progressService.StopProgressAsync();

                                if (_result != null)
                                {

                                    if (_result.IsSuccess)
                                    {

                                        _notificationService.ShowMessage(_result.Data, NotificationType.Success);

                                        _logger.LogInformation($"File was deleted  {fileId}");

                                        //remove file entry from list .                                   
                                        foreach (var _tempFile in InvoiceList.ToList())
                                        {
                                            foreach (var _old in _tempFile.FileAttachments.ToList())
                                            {
                                                if (_old.FileId == fileId)
                                                {
                                                    _tempFile.FileAttachments.Remove(_old);
                                                    //no need to continue loop.
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _notificationService.ShowMessage(_result?.Error, NotificationType.Error);
                                    }


                                }
                                else
                                {
                                    _notificationService.ShowMessage("An Error occurred, try again", NotificationType.Error);
                                }

                            });

                        });


                    }
                }

            }


            catch (Exception _ex)
            {

                await _progressService.StopProgressAsync();

                _logger.LogError($"Error in deleting file", _ex);
            }

        }

        private int _totalCount;
        public int TotalCount
        {
            get { return _totalCount; }
            set { SetProperty(ref _totalCount, value); }
        }


        public void RefreshDataGrid()
        {
            TotalCount = 0;
            FromDate = null;
            ToDate = null;
            this.SelectedStore = null;
            InvoiceList?.Clear();
            IsDc = false;
        }
        private void EditDcNumber(PurchaseInvoiceModel invoice)
        {

            if (invoice != null)
            {

                if (invoice.DcNumber?.Trim()?.ToLower() != invoice.InvoiceNumber?.Trim()?.ToLower())
                {
                    _logger.LogWarning($"update invoice number  {invoice.InvoiceNumber} not possible for dc number {invoice.DcNumber} since they are diffrent");

                    _notificationService.ShowMessage("Invoice number is already updated", Common.NotificationType.Information);

                    return;
                }

                var _date = invoice.DcNumberDate == null ? "" : invoice.DcNumberDate.Value.ToString("dd MMM yyyy");

                _dialogService.ShowDialog("UpdateInvoiceDialog",
                    parameters: new DialogParameters { { "invoice", invoice } },

                    callback: (dialogResult) =>
                    {
                        if (dialogResult != null && dialogResult.Result == ButtonResult.OK)
                        {
                            LoadInvoicesAsync(new InvoiceSearchParams { FromDate = FromDate, ToDate = ToDate, StoreId = SelectedStore?.StoreId, IsDc = IsDc }, true);
                        }

                    });
            }

        }

        private async void UploadInvoiceFiles(int? _invoiceId)
        {
            if (_invoiceId != null && _invoiceId.Value > 0)
            {
                var _invoice = InvoiceList.FirstOrDefault(x => x.InvoiceId == _invoiceId.Value);

                if (_invoice != null)
                {
                    if (_invoice.TempFiles.Count() > 0)
                    {

                        await _progressService.StartProgressAsync();

                        var _result = await _invoiceFileService.UploadInvoiceFiles(_invoiceId.Value, _invoice.TempFiles.ToArray());

                        await _progressService.StopProgressAsync();

                        if (_result != null && _result.IsSuccess)
                        {
                            _notificationService.ShowMessage("Files uploaded", NotificationType.Success);

                            LoadInvoicesAsync(new InvoiceSearchParams { FromDate = FromDate, ToDate = ToDate, StoreId = SelectedStore?.StoreId, IsDc = IsDc }, true);

                        }
                        else
                        {
                            _notificationService.ShowMessage(_result?.Error ?? "An error occurred, try again", NotificationType.Error);
                        }
                    }
                }
            }

        }

        private void DeleteUploadFile(Guid? _fileGUID)
        {

            if (_fileGUID != null)
            {
                foreach (var _invoice in InvoiceList)
                {
                    foreach (var _tempfile in _invoice.TempFiles.ToList())
                    {

                        if (_tempfile.FileId == _fileGUID)
                        {
                            _invoice.TempFiles.Remove(_tempfile);
                        }

                    }

                }
            }
        }

        private async void LoadStoresAsync()
        {
            var _result = await _storeService.GetStores("isenabled=true");
            if (_result != null && _result.Count() > 0)
            {
                if (AppConstants.USER_ROLES.Contains("finance"))
                {
                    var addAllList = _result.ToList();
                    addAllList.Insert(0, new Store() { StoreId = -1, Name = "All" });
                    _result = addAllList;
                }
                Stores = new ObservableCollection<Store>(_result.Where(x => x.Parent_ref == null));
            }
        }

        private DateTime? _formDate;
        public DateTime? FromDate
        {
            get { return _formDate; }
            set { SetProperty(ref _formDate, value); }
        }



        private DateTime? _toDate;
        public DateTime? ToDate
        {
            get { return _toDate; }
            set { SetProperty(ref _toDate, value); }
        }

        private Store _selectedStore;
        public Store SelectedStore
        {
            get { return _selectedStore; }
            set { SetProperty(ref _selectedStore, value); }
        }

        private bool _isDc;
        public bool IsDc
        {
            get { return _isDc; }
            set { SetProperty(ref _isDc, value); }
        }

        private ObservableCollection<Store> _stores;
        public ObservableCollection<Store> Stores
        {
            get { return _stores; }
            set { SetProperty(ref _stores, value); }
        }

        private bool _fromtoDateVisibility;

        public bool FromtoDateVisibilty
        {
            get { return _fromtoDateVisibility; }
            set { SetProperty(ref _fromtoDateVisibility, value); }
        }

        private void SearchPurchaseInvoice()
        {
            if (!IsDc)
            {
                if (FromDate == null)
                {
                    _notificationService.ShowMessage("Please select From date", Common.NotificationType.Error);

                    return;
                }
                if (ToDate == null)
                {
                    _notificationService.ShowMessage("Please select To date", Common.NotificationType.Error);

                    return;
                }
                if (FromDate != null && ToDate != null)
                {
                    if (ToDate < FromDate)
                    {
                        _notificationService.ShowMessage("From date should be less than or equal to To date", NotificationType.Error);
                        return;
                    }
                }


            }
            if (AppConstants.USER_ROLES[0] == AppConstants.ROLE_FINANCE)
            {
                if (SelectedStore == null)
                {
                    _notificationService.ShowMessage("Please select store", NotificationType.Error);
                    return;
                }
            }

            var _searchParm = new InvoiceSearchParams
            {
                FromDate = FromDate,
                StoreId = SelectedStore?.StoreId,
                ToDate = ToDate,
                IsDc = IsDc,
            };
            SelectAllInvoice = false;
            LoadInvoicesAsync(_searchParm, true);
        }

        private async void DownloadFile(int? fileId)
        {
            if (fileId != null && fileId.Value > 0)
            {
                var _result = await _invoiceFileService.DownloadFile(fileId.Value);

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

        private void AddFilesDialog(int? invoiceId)
        {


            var _invoice = InvoiceList.FirstOrDefault(x => x.InvoiceId == invoiceId.Value);

            if (_invoice != null && _invoice.FileAttachments != null && _invoice.FileAttachments.Count == 5)
            {
                _notificationService.ShowMessage("Max file attachments have been reached", NotificationType.Information);

                _logger.LogWarning($"Invoice {_invoice.InvoiceNumber} max file reached");

                return;
            }

            if (_invoice != null && _invoice.TempFiles.Count >= 5)
            {
                _notificationService.ShowMessage("5 files already added. Delete old file to reselect", NotificationType.Information);

                _logger.LogWarning($"Invoice {_invoice.InvoiceNumber} has 5 files aleady");

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

            if (dialog != null && (_invoice.TempFiles?.Count + dialog.FileNames.Length) <= 5 && dialog.FileNames.Length <= 5)
            {
                if (_invoice.TempFiles == null)
                {
                    _invoice.TempFiles = new ObservableCollection<FileUploadInfo>();
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

                                continue;
                            }

                            //dont again add the same file .causes file access issue while reading file stream

                            if (_invoice.TempFiles.Any(x => x.FileName == Path.GetFileName(_fileName)))
                            {
                                _notificationService.ShowMessage($"File {Path.GetFileName(_fileName)} is already added", NotificationType.Information);

                                _logger.LogWarning($"File was already added skipping file {_fileName}");

                                continue;
                            }


                            _invoice.TempFiles?.Add(new FileUploadInfo
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

        private InvoiceSearchParams GetDefaultSearchDate()
        {
            var _fromFirstDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            return new InvoiceSearchParams
            {
                FromDate = _fromFirstDate,
                ToDate = _fromFirstDate.AddMonths(1).AddDays(-1)
            };
        }

        private ObservableCollection<PurchaseInvoiceModel> _invoiceList;
        public ObservableCollection<PurchaseInvoiceModel> InvoiceList
        {
            get => _invoiceList;
            set => _ = SetProperty(ref _invoiceList, value);
        }

        private async void LoadInvoicesAsync(InvoiceSearchParams searchParams = null, bool IsManualRefresh = false)
        {
            try
            {
                if (_cancellationTokenSource != null)
                    _cancellationTokenSource?.Cancel();

                if (IsManualRefresh)
                {
                    await _progressService.StartProgressAsync();
                }

                InvoiceList?.Clear();

                TotalCount = 0;

                _cancellationTokenSource = new CancellationTokenSource();

                await Task.Run(async () =>
                {
                    var _result = await _purchaseInvoiceService.GetPurchaseInvoices(searchParams, _cancellationTokenSource.Token);

                    Application.Current?.Dispatcher?.Invoke(async () =>
                    {
                        if (_result != null && _result.IsSuccess && _result.Data != null)
                        {
                            InvoiceList = new ObservableCollection<PurchaseInvoiceModel>(GetPurchaseInvoiceModel(_result.Data));

                            TotalCount = InvoiceList.Count;

                        }
                        else
                        {
                            _notificationService.ShowMessage(_result?.Error ?? "An error occurred, try again", Common.NotificationType.Error);
                        }
                        if (IsManualRefresh)
                        {
                            await _progressService.StopProgressAsync();
                        }
                    });
                }, _cancellationTokenSource.Token);



            }
            catch (OperationCanceledException) { }
            catch (Exception _ex)
            {
                _logger.LogError("Error in getting list of purchase invoices", _ex);
            }

        }

        private IEnumerable<PurchaseInvoiceModel> GetPurchaseInvoiceModel(IEnumerable<PurchaseInvoiceViewModel> data)
        {
            foreach (var _inv in data)
            {
                yield return new PurchaseInvoiceModel
                {
                    CreatedDate = _inv.CreatedDate,
                    DcNumber = _inv.DcNumber,
                    DcNumberDate = _inv.DcNumberDate,
                    FileAttachments = new ObservableCollection<FileAttachment>(_inv.FileAttachments ?? Enumerable.Empty<FileAttachment>()),
                    GrossTotal = _inv.GrossTotal,
                    InvoiceId = _inv.InvoiceId,
                    InvoiceDate = _inv.InvoiceDate,
                    IsDcNumber = _inv.IsDcNumber,
                    InvoiceNumber = _inv.InvoiceNumber,
                    StoreName = _inv.StoreName,
                    SupplierName = _inv.SupplierName,
                    //StockProducts = _inv.StockProducts,
                    InvoiceDays = _inv.IsDcNumber == true ? _inv.CreatedDate.ToHumanReadableString() : _inv.CreatedDate.ToHumanReadableString(),
                    IsEditButton = AppConstants.USER_ROLES[0].ToLower() == "backend" ? true : false,

                };


            }
        }

        //public string NumberOfdays(DateTime? invoicedate)
        //{
        //        var timespan = invoicedate.Value.Date - DateTime.UtcNow;
        //        return timespan.Duration().Humanize(maxUnit: TimeUnit.Year, precision: 2, minUnit: TimeUnit.Day)+" before";

        //}

        public async void GetProductDetails(object InvoiceID)
        {
            try
            {
                if (InvoiceID != null)
                {
                    var model = InvoiceID as PurchaseInvoiceModel;
                    if (_cancellationTokenSource != null)
                        _cancellationTokenSource?.Cancel();




                    _cancellationTokenSource = new CancellationTokenSource();

                    await Task.Run(async () =>
                    {
                        var _result = await _purchaseInvoiceService.GetProductDetails(model.InvoiceId, _cancellationTokenSource.Token);

                        Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            if (_result != null && _result.IsSuccess && _result.Data != null)
                            {

                                model.StockProducts = _result.Data.ToList();
                            }
                            else
                            {
                                _notificationService.ShowMessage(_result?.Error ?? "An error occurred, try again", Common.NotificationType.Error);
                            }

                        });
                    }, _cancellationTokenSource.Token

                           );

                }

            }
            catch (OperationCanceledException) { }
            catch (Exception _ex)
            {
                _logger.LogError("Error in getting list of purchase invoices", _ex);
            }
        }


        private bool _SelectAllInvoice;

        public bool SelectAllInvoice
        {
            get { return _SelectAllInvoice; }
            set { SetProperty(ref _SelectAllInvoice, value); }
        }


        private bool _popupClose;

        public bool PopupClose
        {
            get { return _popupClose; }
            set { SetProperty(ref _popupClose, value); }
        }

    }
}
