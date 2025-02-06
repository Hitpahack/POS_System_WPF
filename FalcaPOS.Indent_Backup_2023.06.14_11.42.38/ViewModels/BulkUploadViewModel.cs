using FalcaPOS.Common;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.Indent;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using File = System.IO.File;

namespace FalcaPOS.Indent.ViewModels
{
    public class BulkUploadViewModel : BindableBase
    {

        private readonly INotificationService _notificationService;

        private readonly ProgressService _ProgressService;

        public DelegateCommand<object> UploadCommad { get; private set; }

        public DelegateCommand ResetCommand { get; private set; }

        private readonly Logger _logger;

        public DelegateCommand<object> UpdateCommand { get; private set; }

        private readonly IIndentService _indentService;

        private readonly IEventAggregator _eventAggregator;

        public BulkUploadViewModel(Logger logger, IEventAggregator eventAggregator, IIndentService indentService, INotificationService notificationService, ProgressService ProgressService)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _ProgressService = ProgressService ?? throw new ArgumentNullException(nameof(ProgressService));

            _indentService = indentService ?? throw new ArgumentNullException();

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            UploadCommad = new DelegateCommand<object>(AddFileOpenDialog);

            ResetCommand = new DelegateCommand(ResetData);

            UpdateCommand = new DelegateCommand<object>(UpdateUploadedFile);

            BtnUpdateVisibility = false;
        }

        public async void UpdateUploadedFile(object obj)
        {
            try
            {
                var _dataGrid = ((BulkUploadViewModel)obj);
                if (_dataGrid != null)
                {
                    await _ProgressService.StartProgressAsync();

                    if (_dataGrid.BulkUploadView != null && _dataGrid.BulkUploadView.Count > 0)
                    {
                        List<BulkPaymentUpdateModel> bulkPaymentUpdates = new List<BulkPaymentUpdateModel>();

                        foreach (var item in _dataGrid.BulkUploadView)
                        {


                            if (string.IsNullOrEmpty(item.PaymentDate))
                            {
                                _notificationService.ShowMessage("Please check payment date empty", Common.NotificationType.Error);
                                return;
                            }

                            if (string.IsNullOrEmpty(item.UTR))
                            {
                                _notificationService.ShowMessage("Please check UTR empty", Common.NotificationType.Error);
                                return;
                            }
                            if (string.IsNullOrEmpty(item.PaymentRefNo))
                            {
                                _notificationService.ShowMessage("Please check Indent No empty", Common.NotificationType.Error);
                                return;
                            }
                            if (string.IsNullOrEmpty(item.CreditAccountNo))
                            {
                                _notificationService.ShowMessage("Please check Account No empty", Common.NotificationType.Error);
                                return;
                            }

                            if (string.IsNullOrEmpty(item.BeneficiaryBranchCode))
                            {
                                _notificationService.ShowMessage("Please check beneficiary branch code empty", Common.NotificationType.Error);
                                return;
                            }

                            bulkPaymentUpdates.Add(new BulkPaymentUpdateModel()
                            {
                                BankDetails = new Entites.Suppliers.BankDetails { AccountNo = item.CreditAccountNo, IFSC = item.BeneficiaryBranchCode },
                                PayAmount = item.Amount,
                                PaymentDate = item.PaymentDate,
                                UTR = item.UTR,
                                PoNumber = item.PaymentRefNo,


                            });
                        }

                        var _result = await _indentService.BulkPaymentUpdate(bulkPaymentUpdates);

                        if (_result != null && _result.IsSuccess)
                        {
                            _notificationService.ShowMessage(_result.Data, Common.NotificationType.Success);
                            ResetData();
                            await _ProgressService.StopProgressAsync();
                            _eventAggregator.GetEvent<BulkPaymentUpdateChangeEvent>().Publish();
                        }
                        else
                        {
                            _notificationService.ShowMessage(_result.Error, Common.NotificationType.Error);
                        }

                        await _ProgressService.StopProgressAsync();
                    }
                    else if (ICICUploadView != null && ICICUploadView.Count > 0)
                    {
                        List<BulkPaymentUpdateModel> bulkPaymentUpdates = new List<BulkPaymentUpdateModel>();

                        foreach (var item in _dataGrid.ICICUploadView)
                        {


                            if (string.IsNullOrEmpty(item.Date))
                            {
                                _notificationService.ShowMessage("Please check payment date empty", Common.NotificationType.Error);
                                return;
                            }

                            if (string.IsNullOrEmpty(item.UTR))
                            {
                                _notificationService.ShowMessage("Please check UTR empty", Common.NotificationType.Error);
                                return;
                            }
                            if (string.IsNullOrEmpty(item.Remarks))
                            {
                                _notificationService.ShowMessage("Please check Indent No empty", Common.NotificationType.Error);
                                return;
                            }
                            if (string.IsNullOrEmpty(item.BeneficiaryAccNo))
                            {
                                _notificationService.ShowMessage("Please check Account No empty", Common.NotificationType.Error);
                                return;
                            }

                            if (string.IsNullOrEmpty(item.IFSC))
                            {
                                _notificationService.ShowMessage("Please check beneficiary branch code empty", Common.NotificationType.Error);
                                return;
                            }

                            bulkPaymentUpdates.Add(new BulkPaymentUpdateModel()
                            {
                                BankDetails = new Entites.Suppliers.BankDetails { AccountNo = item.BeneficiaryAccNo, IFSC = item.IFSC },
                                PayAmount = item.Amount,
                                PaymentDate = item.Date,
                                UTR = item.UTR,
                                PoNumber = item.Remarks,


                            });
                        }

                        var _result = await _indentService.BulkPaymentUpdate(bulkPaymentUpdates);

                        if (_result != null && _result.IsSuccess)
                        {
                            _notificationService.ShowMessage(_result.Data, Common.NotificationType.Success);
                            ResetData();
                            await _ProgressService.StopProgressAsync();
                            _eventAggregator.GetEvent<BulkPaymentUpdateChangeEvent>().Publish();
                        }
                        else
                        {
                            _notificationService.ShowMessage(_result.Error, Common.NotificationType.Error);
                        }

                        await _ProgressService.StopProgressAsync();
                    }

                }


            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            finally
            {
                await _ProgressService.StopProgressAsync();
            }
        }

        public void ResetData()
        {
            try
            {
                BtnUpdateVisibility = false;
                if (FileUploadListInfo != null)
                    FileUploadListInfo.Clear();
                if (BulkUploadView != null)
                    BulkUploadView = null;
                if (ICICUploadView != null)
                    ICICUploadView = null;

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }
        public void AddFileOpenDialog(object file)
        {
            try
            {
                var viewmodel = ((BulkUploadViewModel)file);



                //if (viewmodel.FileUploadListInfo != null && viewmodel.FileUploadListInfo.Count >= 1)
                //{
                //    _notificationService.ShowMessage("1 files already added. Delete old file to reselect", NotificationType.Information);

                //    _logger.LogWarning($"Max File attachments added");

                //    return;
                //}

                OpenFileDialog dialog = new OpenFileDialog
                {
                    Title = "Please select a file",
                    Multiselect = true,
                    Filter = "Excel Documents (*.xlsx)|*.xlsx",
                };


                bool? _resultOk = dialog.ShowDialog();

                if (_resultOk == null || _resultOk != true)
                {
                    //user cancelled file selection return.
                    return;
                }


                if (dialog != null)
                {

                    viewmodel.FileUploadListInfo = new ObservableCollection<FileUploadInfo>();


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

                                if (viewmodel.FileUploadListInfo.Any(x => x.FileName == Path.GetFileName(_fileName)))
                                {
                                    _notificationService.ShowMessage($"File {Path.GetFileName(_fileName)} is already added", NotificationType.Information);

                                    _logger.LogWarning($"File was already added skipping file {_fileName}");

                                    continue;
                                }

                                _logger.LogInformation($"Adding file {_fileName}");

                                viewmodel.FileUploadListInfo?.Add(new FileUploadInfo
                                {
                                    FileId = Guid.NewGuid(),
                                    FilePath = _fileName,
                                    FileExtension = Path.GetExtension(_fileName),
                                    FileName = Path.GetFileName(_fileName),
                                    Size = FileHelper.FormatSize(_fileInfo.Length),
                                    FileSrc = FileSrc.local
                                });
                                UpdateExcelToModel(viewmodel.FileUploadListInfo.FirstOrDefault());

                            }
                        }
                    }
                }



            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }


        public async void UpdateExcelToModel(FileUploadInfo fileUploadInfo)
        {
            Excel.Application xlApp = new Excel.Application();

            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(fileUploadInfo.FilePath);
            try
            {
                BulkUploadView = null;
                ICICUploadView = null;
                await _ProgressService.StartProgressAsync();

                List<BulkDownloadExport> listData = new List<BulkDownloadExport>();

                Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
                Excel.Range xlRange = xlWorksheet.UsedRange;
                int rowCount = xlRange.Rows.Count;
                int colCount = xlRange.Columns.Count;

                if (xlWorksheet.Name.ToLower() == "falca pos sbi bulk payment")
                {

                    for (int i = 2; i <= rowCount; i++)
                    {

                        var CustomerCode = xlRange.Cells[i, 2].Value2.ToString();
                        var CustomerName = xlRange.Cells[i, 3].Value2.ToString();
                        var DebitAccountNo = xlRange.Cells[i, 4].Value2.ToString();
                        var ProductCode = xlRange.Cells[i, 5].Value2.ToString();
                        var Amount = xlRange.Cells[i, 6].Value2;
                        var Phone = xlRange.Cells[i, 12].Value2.ToString();
                        var StoreName = xlRange.Cells[i, 11].Value2.ToString();

                        var PaymentRefNo = xlRange.Cells[i, 13].Value2.ToString();
                        var UTR = xlRange.Cells[i, 14].Value2.ToString();
                        var PaymentDate = xlRange.Cells[i, 15].Value2.ToString();
                        double date = double.Parse(PaymentDate);

                        var dateTime = DateTime.FromOADate(date).ToString("dd-MM-yyyy");
                        var BeneficiaryName = xlRange.Cells[i, 7].Value2.ToString();
                        var BeneficiaryBranchCode = xlRange.Cells[i, 8].Value2.ToString();
                        var CreditAccountNo = xlRange.Cells[i, 9].Value2.ToString();
                        var EmailId = xlRange.Cells[i, 10].Value2.ToString();
                        string InvoiceNo = null;
                        if (xlRange.Cells[i, 16].Value2 != null)
                        {
                            InvoiceNo = xlRange.Cells[i, 16].Value2.ToString();

                        }




                        listData.Add(new BulkDownloadExport()
                        {
                            CustomerCode = CustomerCode,
                            CustomerName = CustomerName,
                            DebitAccountNo = DebitAccountNo,
                            ProductCode = ProductCode,
                            Amount = (float)Amount,
                            BeneficiaryName = BeneficiaryName,
                            BeneficiaryBranchCode = BeneficiaryBranchCode,
                            CreditAccountNo = CreditAccountNo,
                            EmailId = EmailId,
                            Phone = Phone,
                            StoreName = StoreName,
                            PaymentRefNo = PaymentRefNo,
                            UTR = UTR,
                            PaymentDate = dateTime,
                            InvoiceNo = InvoiceNo,

                        });
                        BulkUploadView = new ObservableCollection<BulkDownloadExport>(listData);
                    }

                    await _ProgressService.StopProgressAsync();

                    BtnUpdateVisibility = (BulkUploadView != null && BulkUploadView.Count > 0) ? true : false;
                }
                else if (xlWorksheet.Name.ToLower() == "falca pos icici bulk payment")
                {

                    List<ICICExport> ExportData = new List<ICICExport>();

                    for (int i = 2; i <= rowCount; i++)
                    {

                        var DebitAcNo = xlRange.Cells[i, 2].Value2.ToString();
                        var BeneficiaryAcNo = xlRange.Cells[i, 3].Value2.ToString();
                        var BeneficiaryName = xlRange.Cells[i, 4].Value2.ToString();
                        var Amount = xlRange.Cells[i, 5].Value2;
                        var PaymentMode = xlRange.Cells[i, 6].Value2;
                        var Date = xlRange.Cells[i, 7].Value2.ToString();
                        var IFSC = xlRange.Cells[i, 8].Value2.ToString();

                        var BeneficiaryMobile = xlRange.Cells[i, 11].Value2.ToString();


                        var EmailId = xlRange.Cells[i, 12].Value2.ToString();

                        var PoNo = xlRange.Cells[i, 22].Value2.ToString();

                        var UTR = xlRange.Cells[i, 23].Value2.ToString();
                        //var PaymentDate = xlRange.Cells[i, 24].Value2.ToString();
                        double date = double.Parse(Date);

                        var dateTime = DateTime.FromOADate(date).ToString("dd-MM-yyyy");




                        ExportData.Add(new ICICExport()
                        {
                            DebitAccNo = DebitAcNo,
                            Amount = (float)Amount,
                            BeneficiaryName = BeneficiaryName,
                            BeneficiaryAccNo = BeneficiaryAcNo,
                            BeneMobileNo = BeneficiaryAcNo,
                            BeneEmailId = EmailId,
                            Remarks = PoNo,
                            UTR = UTR,
                            Date = dateTime,
                            IFSC = IFSC,
                            PaymentMode = PaymentMode,

                        });

                    }

                    ICICUploadView = new ObservableCollection<ICICExport>(ExportData);

                    await _ProgressService.StopProgressAsync();

                    BtnUpdateVisibility = (ICICUploadView != null && ICICUploadView.Count > 0) ? true : false;
                }


            }
            catch (Exception _ex)
            {

                _logger.LogError(_ex.Message);
                _notificationService.ShowMessage("Payment Date and UTR Should not empty all rows", NotificationType.Error);

            }
            finally
            {
                xlApp?.Workbooks?.Close();
                await _ProgressService.StopProgressAsync();
            }

        }
        private ObservableCollection<BulkDownloadExport> _bulkUploadView;
        public ObservableCollection<BulkDownloadExport> BulkUploadView
        {
            get { return _bulkUploadView; }
            set { SetProperty(ref _bulkUploadView, value); }


        }

        private ObservableCollection<ICICExport> _iCICUploadView;
        public ObservableCollection<ICICExport> ICICUploadView
        {
            get => _iCICUploadView;
            set => SetProperty(ref _iCICUploadView, value);


        }

        private ObservableCollection<FileUploadInfo> fileUploadListInfo;
        public ObservableCollection<FileUploadInfo> FileUploadListInfo
        {
            get { return fileUploadListInfo; }
            set { SetProperty(ref fileUploadListInfo, value); }


        }

        private bool _btnUpdateVisibility;
        public bool BtnUpdateVisibility
        {
            get => _btnUpdateVisibility;
            set => SetProperty(ref _btnUpdateVisibility, value);
        }


    }

}
