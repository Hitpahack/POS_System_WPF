using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.Sales;
using FalcaPOS.Sales.Views;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FalcaPOS.Sales.ViewModels
{
    public class CreditViewModel : BindableBase
    {
        private readonly Logger _logger;
        private readonly INotificationService _notificationService;
        private readonly ProgressService _progressService;
        private readonly ISalesService _salesService;
        public DelegateCommand<object> CreditViewPopUpCommand { get; private set; }

        public DelegateCommand<object> CreditsalesUpdateCommand { get; private set; }

        public DelegateCommand<object> AddChequeFileOpenDialogCommand { get; private set; }

        public DelegateCommand<Guid?> DeleteChequeUploadFileCommand { get; private set; }

        public DelegateCommand<object> RefreshCreditSalesCommand { get; private set; }
        public CreditViewModel(ISalesService salesService, INotificationService notificationService, ProgressService progressService, Logger logger)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));

            _salesService = salesService ?? throw new ArgumentNullException(nameof(salesService));

            CreditViewPopUpCommand = new DelegateCommand<object>(openPopUp);

            CreditsalesUpdateCommand = new DelegateCommand<object>(Update);

            AddChequeFileOpenDialogCommand = new DelegateCommand<object>(AddCheque);

            DeleteChequeUploadFileCommand = new DelegateCommand<Guid?>(DeleteuploadedFile);

            RefreshCreditSalesCommand = new DelegateCommand<object>(RefreshCreditSale);

            Load();


        }

        public void DeleteuploadedFile(Guid? fileId)
        {
            try
            {
                if (fileId == null) return;

                var _file = FileUploadListInfo?.FirstOrDefault(x => x.FileId == fileId);

                if (_file != null)
                {
                    _logger.LogInformation($"File Deleted {_file.FileName}");

                    FileUploadListInfo?.Remove(_file);
                }
            }
            catch (Exception _ex)
            {

                _logger.LogError("geeting error in  delete uploaded file ", _ex);
            }
        }

        public void AddCheque(object obj)
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
                                    Size = FileHelper.FormatSize(_fileInfo.Length)
                                });
                            }
                        }
                    }
                }


            }
            catch (Exception _ex)
            {
                _logger.LogError("geeting error in  adding cheque file ", _ex);
            }
        }
        public void Update(object param)
        {
            try
            {
                if (String.IsNullOrEmpty(PopupDetails.ChequeNumber))
                {
                    _notificationService.ShowMessage("Please enter cheque number", Common.NotificationType.Error);
                    return;
                }

                if (PopupDetails.ChequeNumber?.Length != 6)
                {
                    _notificationService.ShowMessage("Please enter valid cheque number", Common.NotificationType.Error);
                    return;
                }


                if (String.IsNullOrEmpty(PopupDetails.ChequeDate))
                {
                    _notificationService.ShowMessage("Please enter cheque date", Common.NotificationType.Error);
                    return;
                }

                if (FileUploadListInfo == null)
                {
                    _notificationService.ShowMessage("Please add attachment", Common.NotificationType.Error);
                    return;
                }
                if (FileUploadListInfo != null)
                {
                    if (FileUploadListInfo.Count == 0)
                    {
                        _notificationService.ShowMessage("Please add attachment", Common.NotificationType.Error);
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
                _logger.LogError("geeting error in  update popup ", _ex);

            }
        }

        public async void openPopUp(object obj)
        {
            try
            {
                var model = (obj as CreditSalesViewModel);

                if (model != null)
                {
                    CreditViewUpdatePopUp popUp = new CreditViewUpdatePopUp();
                    popUp.DataContext = this;
                    PopupDetails.InvoiceNumber = model.InvoiceNumber;
                    PopupDetails.ChequeNumber = model.ChequeNumber;
                    PopupDetails.IsOldCustomer = model.ChequeNumber != null ? false : true;
                    PopupDetails.Cheque = model.Cheque;
                    FileUploadListInfo?.Clear();
                    PopupDetails.Remarks = null;
                    PopupDetails.ChequeDate = null;


                    await DialogHost.Show(popUp, "RootDialog", ClosingEventHandler);
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError("geeting error in  update popup ", _ex);

            }
        }

        private async void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {

                var _viewmodel = (CreditViewModel)eventArgs.Parameter;
                if (_viewmodel != null)
                {
                    await _progressService.StartProgressAsync();

                    await Task.Run(async () =>
                    {
                        UpdateCreditSales update = new UpdateCreditSales()
                        {
                            InvoiceNumber = PopupDetails.InvoiceNumber,
                            ChequeNumber = PopupDetails.ChequeNumber,
                            ChequeDate = PopupDetails.ChequeDate,
                            Remarks = PopupDetails.Remarks,

                        };

                        var _result = await _salesService.UpdateCredtiSales(update);
                        Application.Current?.Dispatcher?.Invoke(async () =>
                        {

                            if (_result != null && _result.IsSuccess)
                            {
                                var _updateresult = await _salesService.UploadChequeFiles(update.InvoiceNumber, FileUploadListInfo.ToArray());
                                if (_updateresult != null && _updateresult.IsSuccess)
                                {
                                    _notificationService.ShowMessage(_result?.Data ?? "Updated Successfully", Common.NotificationType.Success);
                                    PopupDetails.ChequeDate = null;
                                    PopupDetails.Remarks = null;
                                    FileUploadListInfo.Clear();
                                    Load();

                                }
                                else
                                {
                                    _notificationService.ShowMessage(_result?.Error ?? AppConstants.CommonError, Common.NotificationType.Error);
                                }
                            }
                            else
                            {
                                _notificationService.ShowMessage(_result?.Error ?? AppConstants.CommonError, Common.NotificationType.Error);
                            }

                            await _progressService.StopProgressAsync();
                        });


                    });
                }

            }

            catch (Exception ex)
            {
                _logger?.LogError("Error in  creadit view model", ex);

                _notificationService.ShowMessage(AppConstants.CommonError, Common.NotificationType.Error);

                await _progressService.StopProgressAsync();

            }


        }
        public async void Load()
        {
            try
            {



                await Task.Run(async () =>
                {
                    var result = await _salesService.GetCreditList();

                    if (result != null && result.IsSuccess && result.Data != null)
                    {
                        CreditList = result.Data.ToList();
                        TotalCount = "Total Count " + result.Data.Count();

                    }
                    else
                    {
                        CreditList = null;
                        TotalCount = null;

                    }

                });


            }
            catch (Exception _ex)
            {
                _logger.LogError("geeting error in  loading data in credit list", _ex);

            }
        }

        private void MapUploadFilesToForm(FileUploadInfo[] fileUploads, MultipartFormDataContent formContent)
        {
            for (int i = 0; i < fileUploads.Length; i++)
            {

                if (!File.Exists(fileUploads[i].FilePath))
                    throw new PosException($"File {fileUploads[i].FileName} is not avaliable , try again");

                formContent.Add(new StreamContent(new FileStream(fileUploads[i].FilePath, FileMode.Open)), $"Files", fileUploads[i].FileName);
            }
        }

        private SalesInvoice _popupdetails = new SalesInvoice();

        public SalesInvoice PopupDetails
        {
            get { return _popupdetails; }
            set { SetProperty(ref _popupdetails, value); }
        }


        private List<CreditSalesViewModel> _creditList;
        public List<CreditSalesViewModel> CreditList
        {
            get { return _creditList; }
            set { SetProperty(ref _creditList, value); }
        }

        private string _totalcount;
        public string TotalCount
        {
            get { return _totalcount; }
            set { SetProperty(ref _totalcount, value); }
        }

        private ObservableCollection<FileUploadInfo> _fileUploadListInfo;

        public ObservableCollection<FileUploadInfo> FileUploadListInfo
        {
            get { return _fileUploadListInfo; }
            set { SetProperty(ref _fileUploadListInfo, value); }
        }


        public void RefreshCreditSale(object param)
        {
            Load();
        }

    }
}


