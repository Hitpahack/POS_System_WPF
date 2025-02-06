using FalcaPOS.Common;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.Denomination;
using FalcaPOS.Entites.Stock;
using FalcaPOS.Invoice.Models;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.Stock.ViewModels
{
    public class UpdateInvoiceDialogViewModel : BindableBase, IDialogAware
    {
        public string Title => "Update Invoice Number";

        private readonly INotificationService notificationService;

        private readonly IStockService stockService;

        public DelegateCommand CancelCommand { get; private set; }

        public DelegateCommand<object> ResetCommand { get; private set; }

        public DelegateCommand<int?> AddFilesCommand { get; private set; }



        private readonly Logger _logger;

        private readonly IInvoiceFileService _invoiceFileService;

        public DelegateCommand<Guid?> DeleteUploadFileCommand { get; private set; }

        public UpdateInvoiceDialogViewModel(IInvoiceFileService invoiceFileService, INotificationService NotificationService, IStockService StockService, Logger Logger)
        {
            UpdateInvoiceNumberCommand = new DelegateCommand(UpdteInvoiceNumber);

            CancelCommand = new DelegateCommand(CloseDialog);

            ResetCommand = new DelegateCommand<object>(Reset);

            notificationService = NotificationService ?? throw new ArgumentNullException(nameof(NotificationService));

            stockService = StockService ?? throw new ArgumentNullException(nameof(StockService));

            _logger = Logger ?? throw new ArgumentNullException(nameof(Logger));

            AddFilesCommand = new DelegateCommand<int?>(AddFilesDialog);

            _invoiceFileService = invoiceFileService ?? throw new ArgumentNullException(nameof(invoiceFileService));

            DeleteUploadFileCommand = new DelegateCommand<Guid?>(DeleteUploadFile);
        }

        /// <summary>
        /// Reset Button Method for Update Invoice Number pop up
        /// </summary>
        /// <param name="obj"></param>
        private void Reset(Object obj)
        {
            try
            {
                var _view = (UpdateInvoiceDialogViewModel)obj;
                if (_view != null)
                {
                    _view.InvoiceNumber = null;
                    _view.InvoiceDate=null;
                    _view.TempFiles= null;
                    
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }
          
        private void CloseDialog()
        {
            RequestClose?.Invoke(new DialogResult());
        }
        private async void UpdteInvoiceNumber()
        {
            bool _isValid = VailidateInvoice();

            if (!_isValid) return;

            var _vm = GetInvoiceInfo();

            await Task.Run(async () =>
            {
                var _result = await stockService.UpdateInvoiceNumber(_vm);

                if (_result != null && _result.IsSuccess)
                {
                    Application.Current?.Dispatcher?.Invoke(async () =>
                    {

                        var _resultfile = await _invoiceFileService.UploadInvoiceFiles(InvoiceList.InvoiceId, TempFiles.ToArray());



                        if (_resultfile != null && _resultfile.IsSuccess)
                        {
                            notificationService.ShowMessage("Files uploaded", NotificationType.Success);


                        }
                        else
                        {
                            notificationService.ShowMessage(_result?.Error ?? "An error occurred, try again", NotificationType.Error);
                        }

                        //logger.LogInformation($"Invoice number updated against dc number {DCNumber}");

                        notificationService.ShowMessage("Invoice number is updated", Common.NotificationType.Success);

                        RequestClose?.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters($"invoiceId={InvoiceId}")));
                    });
                }
                else
                {
                    _logger.LogError($"error in update invoice number {_result?.Error}");

                    notificationService.ShowMessage($"{_result?.Error ?? "An error occurred try again"}", Common.NotificationType.Error);
                }

            });
        }

        private UpdateDCNumberViewModel GetInvoiceInfo()
        {
            return new UpdateDCNumberViewModel
            {
                InvoiceId = InvoiceId,
                InvoiceNumber = InvoiceNumber,
                InvoiceDate = InvoiceDate.Value,
                DcNumber = DCNumber
            };
        }

        private bool VailidateInvoice()
        {
            if (!InvoiceNumber.IsValidString())
            {
                notificationService.ShowMessage("Invoice number is required", Common.NotificationType.Error);
                return false;
            }

            if (InvoiceDate == null)
            {
                notificationService.ShowMessage("Invoice date is required", Common.NotificationType.Error);

                return false;
            }

            //check same as dc number  

            if (DCNumber.IsValidString() && (InvoiceNumber.Trim().ToLower() == DCNumber.Trim().ToLower()))
            {
                notificationService.ShowMessage("New Invoice number cannot be same as dc number", Common.NotificationType.Error);

                return false;
            }

            if (TempFiles.Count() == 0)
            {
                notificationService.ShowMessage("Please add attachment", Common.NotificationType.Error);

                return false;

            }

            return true;
        }

        private void AddFilesDialog(int? invoiceId)
        {


            var _invoice = InvoiceList;

            if (_invoice != null && _invoice.FileAttachments != null && _invoice.FileAttachments.Count == 5)
            {
                notificationService.ShowMessage("Max file attachments have been reached", NotificationType.Information);

                _logger.LogWarning($"Invoice {_invoice.InvoiceNumber} max file reached");

                return;
            }

            if (TempFiles != null && TempFiles.Count >= 5)
            {
                notificationService.ShowMessage("5 files already added. Delete old file to reselect", NotificationType.Information);

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

            if (dialog != null && (TempFiles?.Count + dialog.FileNames.Length) <= 5 && dialog.FileNames.Length <= 5)
            {
                if (TempFiles == null)
                {
                    TempFiles = new ObservableCollection<FileUploadInfo>();
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
                                notificationService.ShowMessage($"File {Path.GetFileName(_fileName)} size should be less than 10 mb in size", NotificationType.Error);

                                continue;
                            }

                            //dont again add the same file .causes file access issue while reading file stream

                            if (TempFiles.Any(x => x.FileName == Path.GetFileName(_fileName)))
                            {
                                notificationService.ShowMessage($"File {Path.GetFileName(_fileName)} is already added", NotificationType.Information);

                                _logger.LogWarning($"File was already added skipping file {_fileName}");

                                continue;
                            }



                            TempFiles?.Add(new FileUploadInfo
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
                notificationService.ShowMessage("Maximum 5 files allowed", NotificationType.Error);
            }


        }

        private void DeleteUploadFile(Guid? _fileGUID)
        {

            if (_fileGUID != null)
            {

                foreach (var _tempfile in TempFiles.ToList())
                {

                    if (_tempfile.FileId == _fileGUID)
                    {
                        TempFiles.Remove(_tempfile);
                    }

                }


            }
        }

        public DelegateCommand UpdateInvoiceNumberCommand { get; private set; }

        public event Action<IDialogResult> RequestClose;

        public int InvoiceId { get; set; }
        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        private string invoiceNumber;
        public string InvoiceNumber
        {
            get { return invoiceNumber; }
            set { SetProperty(ref invoiceNumber, value); }
        }


        private DateTime? invoiceDate;
        public DateTime? InvoiceDate
        {
            get { return invoiceDate; }
            set { SetProperty(ref invoiceDate, value); }
        }

        private string dcNumber;
        public string DCNumber
        {
            get { return dcNumber; }
            set { SetProperty(ref dcNumber, value); }
        }

        private string supplierName;

        public string SupplierName
        {
            get { return supplierName; }
            set { SetProperty(ref supplierName, value); }
        }


        private string storeName;
        public string StoreName
        {
            get { return storeName; }
            set { SetProperty(ref storeName, value); }
        }

        private string dcDate;
        public string DcDate
        {
            get { return dcDate; }
            set { SetProperty(ref dcDate, value); }
        }


        private ObservableCollection<FileUploadInfo> _tempFiles = new ObservableCollection<FileUploadInfo>();
        public ObservableCollection<FileUploadInfo> TempFiles
        {
            get => _tempFiles;
            set => _ = SetProperty(ref _tempFiles, value);
        }


        private PurchaseInvoiceModel _invoiceList;
        public PurchaseInvoiceModel InvoiceList
        {
            get => _invoiceList;
            set => _ = SetProperty(ref _invoiceList, value);
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            try
            {
                //InvoiceId = parameters.GetValue<int>("invoiceId");
                //DCNumber = parameters.GetValue<string>("dcnumber");
                //SupplierName = parameters.GetValue<string>("supplierName");
                //StoreName = parameters.GetValue<string>("storeName");
                //DcDate = parameters.GetValue<string>("dcDate");
                InvoiceList = parameters.GetValue<PurchaseInvoiceModel>("invoice");
                DcDate = Convert.ToString(InvoiceList.DcNumberDate).Substring(0,10);
                InvoiceId = InvoiceList.InvoiceId;
                DCNumber = InvoiceList.DcNumber;
                SupplierName = InvoiceList.SupplierName;
                StoreName = InvoiceList.StoreName;

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

    }
}
