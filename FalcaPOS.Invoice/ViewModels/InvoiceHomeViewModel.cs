using ControlzEx.Standard;
using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.AddInventory;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.Denomination;
using FalcaPOS.Entites.Stock;
using FalcaPOS.Entites.Stores;
using FalcaPOS.Invoice.Models;
using FalcaPOS.Invoice.Views;
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
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Documents.Spreadsheet.Expressions.Functions;

namespace FalcaPOS.Invoice.ViewModels
{
    /// <summary>
    /// The class holds the view model logic for Invoice Home.
    /// </summary>
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

        public DelegateCommand<object> SearchPurchaseInvoiceCommand { get; private set; }
        public DelegateCommand<object> FetchProductCommand { get; private set; }

        public DelegateCommand<int?> UploadInvoiceFilesCommand { get; private set; }

        public DelegateCommand<PurchaseInvoiceModel> EditDcNumberCommand { get; private set; }

        /// <summary>
        /// The command for Downloading the attachments of an invoice row.
        /// </summary>
        public DelegateCommand<object> DownloadAttachmentsCommand { get; private set; }

        /// <summary>
        /// The command to edit/delete the file attachments
        /// </summary>
        public DelegateCommand<object> EditFileAttachmentsCommand { get; private set; }

        public DelegateCommand<object> UpdateFileAttachmentsCommand { get; private set; }


        private readonly IStoreService _storeService;

        private readonly IDialogService _dialogService;

        private readonly ProgressService _progressService;

        public DelegateCommand RefreshCommand { get; private set; }

        private readonly IEventAggregator _eventAggregator;

        public DelegateCommand<int?> DeleteFileCommand { get; private set; }

        public DelegateCommand<object> DeleteFileOkCommand { get; private set; }
        public DelegateCommand<object> DownloadInvoiceListCommand { get; private set; }


        public DelegateCommand<object> SelectAllInvoiceListCommand { get; private set; }


        public DelegateCommand<object> UnSelectAllInvoiceListCommand { get; private set; }


        public DelegateCommand<bool?> OnCheckedCommand { get; private set; }

        public DelegateCommand<bool?> UnOnCheckedCommand { get; private set; }

        public DelegateCommand<object> HandleRowDetailsVisibilityChangedCommand { get; private set; }


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

            SearchPurchaseInvoiceCommand = new DelegateCommand<object>((obj) => SearchPurchaseInvoice(obj));

            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));

            DeleteUploadFileCommand = new DelegateCommand<Guid?>(DeleteUploadFile);

            UploadInvoiceFilesCommand = new DelegateCommand<int?>(UploadInvoiceFiles);

            EditDcNumberCommand = new DelegateCommand<PurchaseInvoiceModel>(EditDcNumber);

            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

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

            // Command to download attachments of invoices.
            DownloadAttachmentsCommand = new DelegateCommand<object>(DownloadAllAttachments);

            HandleRowDetailsVisibilityChangedCommand = new DelegateCommand<object>(RowDetailsVisibilityEvent);

            LoadStoresAsync();
            
            // Command to edit/delete the file attachments
            EditFileAttachmentsCommand = new DelegateCommand<object>(EditAllAttachments);

            UpdateFileAttachmentsCommand = new DelegateCommand<object>(UpdateFileAttachments);

        }

        /// <summary>
        /// This method is called when the user clicks on Upload Invoices button in the Edit File Attachments pop up.
        /// </summary>
        /// <param name="obj"></param>
        private void UpdateFileAttachments(object obj)
        {
            // Closes the Edit File Attachments Pop up.
            var TargetClose = ((Button)(obj));
            var dynamicCommand = TargetClose.Command;
            dynamicCommand.CanExecute(true);
            dynamicCommand.Execute(this);
        }

        /// <summary>
        /// This method opens the Edit/Delete File Attachments popup to delete or modify any attachments.
        /// </summary>
        /// <param name="obj">object.</param>
        private async void EditAllAttachments(object obj)
        {
            try
            {
                var purchaseInvoiceRowDetails = (PurchaseInvoiceModel)obj;

                // Creates an observable collection for storing all the File attachments that are present for a selected invoice.
                AllFilesToEdit = new ObservableCollection<FileAttachment>();
                foreach (var fil in purchaseInvoiceRowDetails.FileAttachments)
                {
                    AllFilesToEdit.Add(fil);
                }

                // Stores the Id of the Invoice that is under edit/delete pop up.
                InvoiceIdToEdit = purchaseInvoiceRowDetails.InvoiceId;

                // Opens the Edit File Attachments pop up.
                EditFileAttachmentsPopup editFileAttachmentsPopup = new EditFileAttachmentsPopup();
                editFileAttachmentsPopup.DataContext = this;
                await DialogHost.Show(editFileAttachmentsPopup, "RootDialog", ButtonCloseEventHandler);

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        /// <summary>
        /// This method handles the button close event handler for Edit All Attachments Pop up.
        /// </summary>
        /// <param name="sender">object.</param>
        /// <param name="eventArgs">DialogClosingEventArgs.</param>
        private async void ButtonCloseEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            // Stores the details of the InvoiceHomeViewModel on the button close event.
            var _viewModel = (InvoiceHomeViewModel)eventArgs.Parameter;

            // If the _viewModel is not null
            if (_viewModel != null)
            {
                // If the InvoiceId to edit is not null and value is greater than zero.
                if (InvoiceIdToEdit != null && InvoiceIdToEdit.Value > 0)
                {
                    // Fetches the invoice data for the InvoiceId from the InvoiceList.
                    var _invoice = InvoiceList.FirstOrDefault(x => x.InvoiceId == InvoiceIdToEdit.Value);

                    // If the invoice data is not null
                    if (_invoice != null)
                    {
                        // Checks if there are any new files to upload. (New Files are Stored in _invoice.TempFiles).
                        if (_invoice.TempFiles.Count() > 0)
                        { 
                            await _progressService.StartProgressAsync();

                            // Uploads the new file list.
                            var _result = await _invoiceFileService.UploadInvoiceFiles(InvoiceIdToEdit.Value, _invoice.TempFiles.ToArray());

                            await _progressService.StopProgressAsync();


                            // Checks if the result is not null and IsSuccess.
                            if (_result != null && _result.IsSuccess)
                            {
                                // Clears the Temp Files after a successful upload.
                                AllTempFiles?.Clear();
                                _notificationService.ShowMessage("Files uploaded", NotificationType.Success);

                                // Reloads the Invoices in the UI.
                                LoadInvoicesAsync(new InvoiceSearchParams { FromDate = FromDate, ToDate = ToDate, StoreId = SelectedStore?.StoreId, IsDc = IsDc }, true);
                            }
                            else
                            {
                                // Error is notified to the user.
                                _notificationService.ShowMessage(_result?.Error ?? "An error occurred, try again", NotificationType.Error);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This event is called when the Toggle button in the Telerik Grid rows are expanded or collapsed.
        /// </summary>
        /// <param name="obj"></param>
        private void RowDetailsVisibilityEvent(object obj)
        {
            try
            {
                // Fetches the visibility state of the row details.
                bool visibility = (obj as GridViewRowDetailsEventArgs).Visibility == Visibility.Visible ? true : false;

                // If the visibility is true
                if (visibility)
                {
                    var rowDetails = (obj as GridViewRowDetailsEventArgs).DetailsElement.DataContext as PurchaseInvoiceModel;

                    // Fetches the product details of that particular row which is expanded.
                    GetProductDetails(rowDetails);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            
        }

        /// <summary>
        /// This method download all the attachments of the selected invoice row as a zip file.
        /// </summary>
        /// <param name="obj"></param>
        private async void DownloadAllAttachments(object obj)
        {
            try
            {
                var Viewmodel = (PurchaseInvoiceModel)obj;

                List<int> FileIds = new List<int>();
                if (Viewmodel.FileAttachments != null && Viewmodel.FileAttachments.Count > 0)
                {
                    foreach (var ids in Viewmodel.FileAttachments)
                    {
                        FileIds.Add(ids.FileId);
                    }
                }

                if (FileIds != null && FileIds.Count > 0)
                {
                    await _progressService.StartProgressAsync();

                    var _result = await _invoiceFileService.DownloadFileList(FileIds);

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
                    await _progressService.StopProgressAsync();

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
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
            MessageBoxResult result = MessageBox.Show("Delete File!\nDeleting this file will also delete all sub file this zip file contains.Do you want to procceed?", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    ObservableCollection<FileAttachment> fileAttachmentsAfterDelete = new ObservableCollection<FileAttachment>(AllFilesToEdit);
                    FileAttachment temp = fileAttachmentsAfterDelete.FirstOrDefault(item => item.FileId == fileId);
                    if (temp != null)
                    {
                        fileAttachmentsAfterDelete.Remove(temp);
                    }
                    if (fileAttachmentsAfterDelete.Count < 1)
                    {
                        _notificationService.ShowMessage("Error in deleting the attachment.\n.Atleast one attachment should be present.", NotificationType.Error);
                    }
                    else
                    {
                        _logger.LogInformation($"User deleting file id {fileId}");
                        await _progressService.StartProgressAsync();
                        int _invoiceId = InvoiceIdToEdit.Value;

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

                                        FileAttachment itemToRemove = AllFilesToEdit.FirstOrDefault(item => item.FileId == fileId);
                                        if (itemToRemove != null)
                                        {
                                            AllFilesToEdit.Remove(itemToRemove);
                                        }
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
                            });

                        });
                    }   
                }
                catch (Exception _ex)
                {

                    await _progressService.StopProgressAsync();

                    _logger.LogError($"Error in deleting file", _ex);
                }

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
            InvoiceList = null;
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
                            AllTempFiles?.Clear();
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
                            AllTempFiles.Remove(_tempfile);
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
                if (AppConstants.USER_ROLES.Contains("finance")||AppConstants.USER_ROLES.Contains("controlmanager") || AppConstants.USER_ROLES.Contains("purchasemanager") || AppConstants.USER_ROLES.Contains("admin")) 
                {
                    var addAllList = _result.ToList();
                    addAllList.Insert(0, new Store() { StoreId = -1, Name = "All" });
                    addAllList.Insert(1, new Store() { StoreId = -2, Name = "All F-Shop" });
                    addAllList.Insert(2, new Store() { StoreId = -3, Name = "All RSP" });
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

        private void SearchPurchaseInvoice(object obj)
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
            if (AppConstants.USER_ROLES[0] == AppConstants.ROLE_FINANCE || AppConstants.USER_ROLES[0] == AppConstants.ROLE_PURCHASE_MANAGER || AppConstants.USER_ROLES[0] == AppConstants.ROLE_CONTROL_MANAGER || AppConstants.USER_ROLES[0] == AppConstants.ROLE_ADMIN)
            {
                if (SelectedStore == null)
                {
                    _notificationService.ShowMessage("Please select store", NotificationType.Error);
                    return;
                }
            }

            if (obj != null)
            {
                ResetTelerikGridFilters.ClearTelerikGridViewFilters(obj);
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

            if (_invoice != null)
            {
                if (_invoice.FileAttachments != null && _invoice.FileAttachments.Count >= 5)
                {
                    _notificationService.ShowMessage("Max file attachments have been reached", NotificationType.Information);

                    _logger.LogWarning($"Invoice {_invoice.InvoiceNumber} max file reached");

                    return;
                }
                else if (_invoice.FileAttachments != null && (_invoice.FileAttachments.Count + _invoice.TempFiles.Count) >= 5)
                {
                    _notificationService.ShowMessage("5 files already added. Delete old file to reselect", NotificationType.Information);

                    _logger.LogWarning($"Invoice {_invoice.InvoiceNumber} has 5 files aleady");

                    return;
                }
                else if (_invoice.TempFiles.Count >= 5)
                {
                    _notificationService.ShowMessage("5 files already added. Delete old file to reselect", NotificationType.Information);

                    _logger.LogWarning($"Invoice {_invoice.InvoiceNumber} has 5 files aleady");

                    return;
                }
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
                        if (System.IO.File.Exists(_fileName))
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
                            AllTempFiles = new ObservableCollection<FileUploadInfo>(_invoice.TempFiles);
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
                InvoiceList = null;

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

                    // Sets DCNumberDate to InvoiceDate if the record is DC Record, if not, sets the Invoice Date itself.
                    InvoiceDate = (DateTime)(_inv.IsDcNumber? _inv.DcNumberDate: _inv.InvoiceDate),
                    IsDcNumber = _inv.IsDcNumber,
                    InvoiceNumber = _inv.InvoiceNumber,
                    StoreName = _inv.StoreName,
                    SupplierName = _inv.SupplierName,
                    
                    //StockProducts = _inv.StockProducts,
                    InvoiceDays = _inv.IsDcNumber == true ? _inv.CreatedDate.ToHumanReadableString() : _inv.CreatedDate.ToHumanReadableString(),
                    IsEditButton = (AppConstants.USER_ROLES[0].ToLower() == "backend") && _inv.IsDcNumber == true ? true : false,
                    IsFileEditable =( AppConstants.USER_ROLES[0].ToLower() == AppConstants.ROLE_FINANCE || AppConstants.USER_ROLES[0].ToLower() == AppConstants.ROLE_PURCHASE_MANAGER) == true ? true : false,

                };


            }
        }

        //public string NumberOfdays(DateTime? invoicedate)
        //{
        //        var timespan = invoicedate.Value.Date - DateTime.UtcNow;
        //        return timespan.Duration().Humanize(maxUnit: TimeUnit.Year, precision: 2, minUnit: TimeUnit.Day)+" before";

        //}

        public async void GetProductDetails(PurchaseInvoiceModel purchaseInvoiceModel)
        {
            try
            {
                var model = purchaseInvoiceModel;
                if (_cancellationTokenSource != null)
                    _cancellationTokenSource?.Cancel();
                _cancellationTokenSource = new CancellationTokenSource();

                await Task.Run(async () =>
                {
                    // Fetches the product details of the selected Purchase Invoice Model.
                    var _result = await _purchaseInvoiceService.GetProductDetails(model.InvoiceId, _cancellationTokenSource.Token);

                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        if (_result != null && _result.IsSuccess && _result.Data != null)
                        {
                            // Updates the Stock Products list with the result.
                            model.StockProducts = _result.Data.ToList();
                        }
                        else
                        {
                            _notificationService.ShowMessage(_result?.Error ?? "An error occurred, try again", NotificationType.Error);
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

        /// <summary>
        /// Stores the list of attachments of the invoice that is in edit state. 
        /// </summary>
        private ObservableCollection<FileAttachment> _allFilesToEdit;
        /// <summary>
        /// Gets and sets the list of attachments of the invoice that is in edit state. 
        /// </summary>
        public ObservableCollection<FileAttachment> AllFilesToEdit
        {
            get { return _allFilesToEdit; }
            set { SetProperty(ref _allFilesToEdit, value); }
        }

        /// <summary>
        /// Stores the invoice id of the invoice row that is in edit state.
        /// </summary>
        private int? _invoiceIdToEdit;

        /// <summary>
        /// Gets and sets the invoice id of the invoice row that is in edit state.
        /// </summary>
        public int? InvoiceIdToEdit
        {
            get { return _invoiceIdToEdit; }
            set { SetProperty(ref _invoiceIdToEdit, value); }
        }

        /// <summary>
        /// Stores the list of files that are added in the Edit File Attachment pop up.
        /// </summary>
        private ObservableCollection<FileUploadInfo> _allTempFiles;

        /// <summary>
        /// Gets and sets the list of files that are added in the Edit File Attachment pop up.
        /// </summary>
        public ObservableCollection<FileUploadInfo> AllTempFiles
        {
            get { return _allTempFiles; }
            set { SetProperty(ref _allTempFiles, value); }
        }
    }
}
