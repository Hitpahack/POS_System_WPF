using FalcaPOS.Common;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.AddInventory;
using FalcaPOS.Entites.Common;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace FalcaPOS.PurchaseReturns.ViewModel
{
    public class UpdateCreditNoteNumberViewModel : BindableBase, IDialogAware
    {
        private bool _IsDialogOpen;
        public bool IsDialogOpen
        {
            get { return _IsDialogOpen; }
            set { SetProperty(ref _IsDialogOpen, value); }
        }

        public string Title => "UpdateCreditNoteNumber";


        private IEventAggregator _eventAggregator;

        public event Action<IDialogResult> RequestClose;

        public DelegateCommand<object> AddAttachment { get; private set; }

        private readonly INotificationService _notificationService;

        private readonly Logger _logger;

        public DelegateCommand<Guid?> DeleteUploadFileCommand { get; private set; }


        public DelegateCommand<object> ApproveCommand { get; private set; }

        public UpdateCreditNoteNumberViewModel(Logger logger, INotificationService notificationService, IEventAggregator eventAggregator)
        {

            _eventAggregator = eventAggregator;

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            AddAttachment = new DelegateCommand<object>(FileOpenDialog);

            DeleteUploadFileCommand = new DelegateCommand<Guid?>(DeleteUploadFile);

            ApproveCommand = new DelegateCommand<object>(Save);

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
        public void Save(object obj)
        {
            bool _isValidData = ValidateData();

            if (!_isValidData)
            {
                return;
            }

            var StoreReturn = new StoreReturnModel()
            {
                CreditNoteNumber = CNoteNumber,
                CreditNoteDate = CreditNoteDate,
                FileUploadListInfo = FileUploadListInfo

            };

            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters { { "StoreReturn", StoreReturn } }));
        }
        private void CloseDialog()
        {
            RequestClose?.Invoke(new DialogResult());
        }
        private void OnShowDialog()
        {
            IsDialogOpen = true;
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            IsDialogOpen = true;
        }

        private bool ValidateData()
        {
            if (string.IsNullOrEmpty(CNoteNumber))
            {
                _notificationService.ShowMessage("Please enter C/N number", Common.NotificationType.Error);

                return false;
            }
            if (string.IsNullOrEmpty(CreditNoteDate))
            {
                _notificationService.ShowMessage("Please enter C/N date", Common.NotificationType.Error);

                return false;
            }
            if (FileUploadListInfo == null || FileUploadListInfo.Count == 0)
            {
                _notificationService.ShowMessage("Please add attachment", Common.NotificationType.Error);

                return false;
            }


            return true;
        }


        private ObservableCollection<FileUploadInfo> fileUploadListInfo;
        public ObservableCollection<FileUploadInfo> FileUploadListInfo
        {
            get { return fileUploadListInfo; }
            set { SetProperty(ref fileUploadListInfo, value); }
        }

        private string _cNoteNumber;

        public string CNoteNumber
        {
            get { return _cNoteNumber; }
            set { SetProperty(ref _cNoteNumber, value); }
        }


        private string _creditNoteDate;
        public string CreditNoteDate
        {
            get { return _creditNoteDate; }
            set { SetProperty(ref _creditNoteDate, value); }
        }
    }
}
