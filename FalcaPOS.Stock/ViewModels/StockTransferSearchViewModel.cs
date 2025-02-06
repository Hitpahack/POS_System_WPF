using FalcaPOS.Common;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Stock;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.Stock.ViewModels
{
    public class StockTransferSearchViewModel : BindableBase
    {

        private readonly IEventAggregator _eventAggregator;

        private readonly INotificationService _notificationService;

        private readonly IStockTransferService _stockTransferService;

        private readonly ProgressService _ProgressService;

        public DelegateCommand SearchCommand { get; private set; }

        public DelegateCommand ResetCommand { get; private set; }

        private readonly Logger _logger;

        public DelegateCommand<object> DownloadFileCommand { get; private set; }

        private readonly IInvoiceFileService _invoiceFileService;


        public DelegateCommand<object> DownloadStockTranfer { get; private set; }
        public DelegateCommand<object> DownloadEwayBillCommand { get; private set; }
        public DelegateCommand ExportResultToExcelCommand { get; private set; }

        private readonly ICommonService _commonService;

        public StockTransferSearchViewModel(EventAggregator EventAggregator,
            Logger Logger,
              IInvoiceFileService invoiceFileService,
            INotificationService NotificationService,
            IStockTransferService StockTransferService, ProgressService ProgressService, ICommonService commonService)
        {
            _notificationService = NotificationService ?? throw new ArgumentNullException(nameof(NotificationService));


            _ProgressService = ProgressService ?? throw new ArgumentNullException(nameof(ProgressService));

            _stockTransferService = StockTransferService ?? throw new ArgumentNullException(nameof(StockTransferService));

            _eventAggregator = EventAggregator ?? throw new ArgumentNullException(nameof(EventAggregator));

            _logger = Logger ?? throw new ArgumentNullException(nameof(Logger));

            _invoiceFileService = invoiceFileService ?? throw new ArgumentNullException(nameof(invoiceFileService));

            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));

            SearchCommand = new DelegateCommand(loadData);

            ResetCommand = new DelegateCommand(ResetLoadedData);

            DownloadFileCommand = new DelegateCommand<object>(DownloadFile);

            DownloadStockTranfer = new DelegateCommand<object>(GetDownloadStockTransferPDF);
            DownloadEwayBillCommand = new DelegateCommand<object>(DownloadEwayBill);
            ExportResultToExcelCommand = new DelegateCommand(ExportResultToExcel);

            Status = new List<string>();
            Status.Add("Processing");
            Status.Add("Completed");
            Status.Add("Cancelled");
            



        }
        private void ExportResultToExcel()
        {

            if (StockTransferDetails == null || !StockTransferDetails.Any())
            {
                _notificationService.ShowMessage("No records found to export", Common.NotificationType.Error);

                return;
            }

            IsExportEnabled = false;

            if (!_fileName.IsValidString())
            {
                _fileName = "Stock Transfer Details";
            }
            _fileName += Guid.NewGuid().ToString().Substring(0, 3);

            bool _result = _commonService.ExportToXL(StockTransferDetails, _fileName, skipfileName: false);

            if (_result)
            {
                _notificationService.ShowMessage("File exported to folderC:\\FALCAPOS\\PosReports", Common.NotificationType.Success);
            }
            else
            {
                _notificationService.ShowMessage("File export failed , try again", Common.NotificationType.Error);
            }

            IsExportEnabled = true;

        }
        public void ResetLoadedData()
        {
            try
            {
                FromDate = null;
                ToDate = null;
                SelectedStatus=null;
                IsExportEnabled = false;
                if (GetStockTransferList != null && GetStockTransferList.Count > 0)
                    GetStockTransferList.Clear();
                if (StockTransferDetails != null && StockTransferDetails.Count > 0)
                    StockTransferDetails.Clear();



            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }
        public async void loadData()
        {
            try
            {
                if (string.IsNullOrEmpty(FromDate))
                {
                    _notificationService.ShowMessage("Please select from date", Common.NotificationType.Error);
                    return;
                }

                if (string.IsNullOrEmpty(ToDate))
                {
                    _notificationService.ShowMessage("Please select to date", Common.NotificationType.Error);
                    return;
                }

                if (string.IsNullOrEmpty(SelectedStatus))
                {
                    _notificationService.ShowMessage("Please select status", Common.NotificationType.Error);
                    return;
                }

                if (!String.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    DateTime dt1 = Convert.ToDateTime(FromDate);
                    DateTime dt2 = Convert.ToDateTime(ToDate);
                    if (dt2 < dt1)
                    {
                        _notificationService.ShowMessage("From Date should be less than or equal to To Date", NotificationType.Error);
                        return;
                    }
                }



                await _ProgressService.StartProgressAsync();

                await Task.Run(async () =>
                {

                    var _result = await _stockTransferService.StockTransferSearchV2(FromDate, ToDate,SelectedStatus);

                    Application.Current.Dispatcher?.Invoke(() =>
                    {
                        if (_result != null && _result.IsSuccess)
                        {
                            GetStockTransferList = new ObservableCollection<TransferCompletedViewModel>(_result.Data.ToList());

                            if (_result.Data != null && _result.Data.Count() > 0)
                            {
                                List<ExportStockTransfer> stockes = new List<ExportStockTransfer>();
                                foreach (var item in _result.Data)
                                {
                                    foreach (var product in item.transferProducts)
                                    {
                                        stockes.Add(new ExportStockTransfer()
                                        {
                                            FromStore = item.FromLocation,
                                            ToStore = item.ToLocation,
                                            SRNo = item.SRNumber,
                                            SRDate = item.SRDate,
                                            STNo = item.STNumber,
                                            STDate = item.STDate,
                                            Category = product.Category,
                                            SubCategory = product.SubCategory,
                                            Brand = product.Brand,
                                            Product = product.ProductName,
                                            SKU = product.ProductSKU,
                                           OldSKU =product.OldSKU,
                                            SellingPrice = product.SellingPrice,
                                            Rate = product.Rate,
                                            GST = product.GST,
                                            Qty = product.TransferQty,
                                            TransportCharges = item.TransportCharges,
                                            Others = item.Others,
                                            TransportChargesPayer = item.TransportChargesPayer,
                                            Status =item.Status
                                        });
                                    }

                                }

                                IsExportEnabled = true;

                                StockTransferDetails = stockes;
                            }

                        }
                        else
                        {
                            _notificationService.ShowMessage(_result?.Error, Common.NotificationType.Error);
                            GetStockTransferList?.Clear();
                        }
                    });

                    await _ProgressService.StopProgressAsync();
                });
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

        private ObservableCollection<TransferCompletedViewModel> _getStockTransferList;
        public ObservableCollection<TransferCompletedViewModel> GetStockTransferList
        {

            get { return _getStockTransferList; }
            set { SetProperty(ref _getStockTransferList, value); }
        }

        private string _fromDate;

        public string FromDate
        {
            get { return _fromDate; ; }
            set { SetProperty(ref _fromDate, value); }
        }

        private string _toDate;

        public string ToDate
        {
            get { return _toDate; ; }
            set { SetProperty(ref _toDate, value); }
        }
        private void SaveFileManually(byte[] stream)
        {
            try
            {
                SaveFileDialog _sd = new SaveFileDialog();

                //_sd.FileName = result.Data.FileName;
                _sd.AddExtension = true;
                _sd.DefaultExt = ".pdf";

                _sd.ShowDialog();

                if (_sd.FileName.IsValidString())
                {

                    File.WriteAllBytes(_sd.FileName, stream);

                    ProcessStartInfo _proccess = new ProcessStartInfo
                    {
                        UseShellExecute = true,
                        FileName = _sd.FileName
                    };
                    Process.Start(_proccess);

                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);

            }
        }

        public async void GetDownloadStockTransferPDF(object obj)
        {
            try
            {
                var ViewModel = (TransferCompletedViewModel)obj;

                if (ViewModel != null)
                {
                    await _ProgressService.StartProgressAsync();


                    await Task.Run(async () =>
                    {
                        var result = await _stockTransferService.GetStockTransferPdf(ViewModel.SRNumber);

                        if (result != null && result.IsSuccess)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {


                                try
                                {


                                    string path = @"c:\PosInvoices";

                                    var _info = Directory.CreateDirectory(path);

                                    var _fileName = path + "\\" + $"{result.Data.FileName}";

                                    if (!File.Exists(_fileName))
                                    {
                                        File.WriteAllBytes(_fileName, result.Data.FileStream);

                                        ProcessStartInfo _p = new ProcessStartInfo
                                        {
                                            UseShellExecute = true,
                                            FileName = _fileName
                                        };

                                        Process.Start(_p);

                                        return;
                                    }
                                    else
                                    {
                                        //Fall back to save  if file name already present
                                        SaveFileManually(result.Data.FileStream);
                                    }
                                }
                                catch (UnauthorizedAccessException _ex)
                                {

                                    _logger.LogError(_ex.Message);
                                    //fall back to save manually if file creation is blocked.
                                    SaveFileManually(result.Data.FileStream);
                                }
                                catch (Exception _ex)
                                {
                                    _logger.LogError(_ex.Message);
                                    _notificationService.ShowMessage("Ann error occred while showing  pdf", NotificationType.Error);

                                }



                            });

                        }
                        else
                        {
                            if (result != null && !result.IsSuccess && result.Error.IsValidString())
                            {
                                _notificationService.ShowMessage(result.Error, NotificationType.Error);
                            }
                        }

                    });

                    await _ProgressService.StopProgressAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        private async void DownloadFile(object obj)
        {
            var ViewModel = (TransferCompletedViewModel)obj;
            int? fileId = ViewModel.FileID;
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
            else
            {
                _notificationService.ShowMessage("File not found", Common.NotificationType.Error);
            }
        }
        public void DownloadEwayBill(object obj)
        {
            try
            {
                var _viewModel = (TransferCompletedViewModel)obj;
                if (_viewModel != null && _viewModel.EwayBillUrl != null)
                {
                    if (_viewModel.EwayBillUrl != null)
                    {
                        ProcessStartInfo _p = new ProcessStartInfo
                        {
                            UseShellExecute = true,
                            FileName = "http://" + _viewModel.EwayBillUrl,
                        };

                        Process.Start(_p);
                    }
                    else
                        _notificationService.ShowMessage("EWay bill not found", NotificationType.Error);
                    return;
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }
        public string _fileName { get; set; }

        private bool _isExportEnabled;
        public bool IsExportEnabled
        {
            get => _isExportEnabled;
            set => _ = SetProperty(ref _isExportEnabled, value);
        }

        private List<ExportStockTransfer> _StockTransferDetails;
        public List<ExportStockTransfer> StockTransferDetails
        {
            get => _StockTransferDetails;
            set => _ = SetProperty(ref _StockTransferDetails, value);
        }


        private List<string> _Status;

        public List<string> Status
        {
            get { return _Status; }
            set { SetProperty(ref _Status, value); }
        }

        private string _selectedStatus;
        public string SelectedStatus
        {
            get { return _selectedStatus; }
            set { SetProperty(ref _selectedStatus, value); }
        }

    }


    public class ExportStockTransfer
    {
        public string FromStore { get; set; }

        public string ToStore { get; set; }

        public string SRNo { get; set; }

        public string SRDate { get; set; }

        public string STNo { get; set; }

        public string STDate { get; set; }
        public string Category { get; set; }


        [DisplayName("Sub Category")]
        public string SubCategory { get; set; }

        public string Brand { get; set; }

        public string Product { get; set; }

        public string SKU { get; set; }

        [DisplayName("Old SKU")]
        public string OldSKU { get; set; }

        [DisplayName("Transferred Qty")]
        public int Qty { get; set; }

        [DisplayName("Purchase cost/rate")]
        public float Rate { get; set; }
        public int GST { get; set; }
        public float SellingPrice { get; set; }
        public float TransportCharges { get; set; }
        public float Others { get; set; }

        [DisplayName("Transport Charges Payer")]

        public string TransportChargesPayer { get; set; }


        public string Status { get; set; }


    }


}
