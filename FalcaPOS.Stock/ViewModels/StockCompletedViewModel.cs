using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace FalcaPOS.Stock.ViewModels
{
    public class StockCompletedViewModel : BindableBase
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

        public DelegateCommand<object> DownloadEwayBillCommand { get; private set; }
        public DelegateCommand<object> DownloadStockTranfer { get; private set; }

        public StockCompletedViewModel(IEventAggregator EventAggregator,
            Logger Logger,
              IInvoiceFileService invoiceFileService,
            INotificationService NotificationService,
            IStockTransferService StockTransferService, ProgressService ProgressService)
        {
            _notificationService = NotificationService ?? throw new ArgumentNullException(nameof(NotificationService));


            _ProgressService = ProgressService ?? throw new ArgumentNullException(nameof(ProgressService));

            _stockTransferService = StockTransferService ?? throw new ArgumentNullException(nameof(StockTransferService));

            _eventAggregator = EventAggregator ?? throw new ArgumentNullException(nameof(EventAggregator));

            _logger = Logger ?? throw new ArgumentNullException(nameof(Logger));

            _invoiceFileService = invoiceFileService ?? throw new ArgumentNullException(nameof(invoiceFileService));

            SearchCommand = new DelegateCommand(loadData);

            ResetCommand = new DelegateCommand(ResetLoadedData);

            DownloadFileCommand = new DelegateCommand<object>(DownloadFile);
            DownloadEwayBillCommand = new DelegateCommand<object>(DownloadEwayBill);
            DownloadStockTranfer = new DelegateCommand<object>(GetDownloadStockTransferPDF);

            Status = new List<string> {"Processing", "Approved", "Transferred", "Completed", "Rejected","SahayaPending", "Cancelled" };

        }


        public void ResetLoadedData()
        {
            try
            {
                FromDate = null;
                ToDate = null;
                SelectedStatus = null;
                if (GetStockTransferCompletedList != null && GetStockTransferCompletedList.Count > 0)
                {
                    GetStockTransferCompletedList.Clear();
                }

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


                await _ProgressService.StartProgressAsync();

                await Task.Run(async () =>
                {

                    if (AppConstants.USER_ROLES != null && AppConstants.USER_ROLES.Contains(AppConstants.ROLE_TERRITORY_MANAGER))
                    {
                        var _result = await _stockTransferService.StockTransferSearchV2(FromDate, ToDate, SelectedStatus);

                        Application.Current.Dispatcher?.Invoke(() =>
                        {
                            if (_result != null && _result.IsSuccess)
                            {
                                GetStockTransferCompletedList = new ObservableCollection<TransferCompletedViewModel>(_result.Data.ToList());
                            }
                            else
                            {
                                _notificationService.ShowMessage(_result?.Error, Common.NotificationType.Error);
                                GetStockTransferCompletedList?.Clear();
                            }
                        });

                    }
                    else
                    {
                        var _result = await _stockTransferService.StoreStockTransferSearchV2(FromDate, ToDate, SelectedStatus);

                        Application.Current.Dispatcher?.Invoke(() =>
                        {
                            if (_result != null && _result.IsSuccess)
                            {
                                GetStockTransferCompletedList = new ObservableCollection<TransferCompletedViewModel>(_result.Data.ToList());
                            }
                            else
                            {
                                _notificationService.ShowMessage(_result?.Error, Common.NotificationType.Error);
                                GetStockTransferCompletedList?.Clear();
                            }
                        });
                    }

                    await _ProgressService.StopProgressAsync();
                });
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            finally {
                await _ProgressService.StopProgressAsync();
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


        private ObservableCollection<TransferCompletedViewModel> _getStockTransferCompletedList;
        public ObservableCollection<TransferCompletedViewModel> GetStockTransferCompletedList
        {

            get { return _getStockTransferCompletedList; }
            set { SetProperty(ref _getStockTransferCompletedList, value); }
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
        public void DownloadEwayBill(object obj)
        {
            try
            {
                var _viewModel = (TransferCompletedViewModel)obj;
                if (_viewModel != null)
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
                    {
                        string Datetime = null;
                        if(_viewModel.EWayBillModel.DocumentDate!=null)
                        {
                            Datetime = _viewModel.EWayBillModel.DocumentDate.Value.ToString("dd-MM-yyyy");
                        }

                        var temp = new StockTransferModelForEWayModel()
                        {
                            SRNumber = _viewModel.SRNumber,
                            TransferId = _viewModel.StockTransferId,
                            FromLocation = _viewModel.FromLocation,
                            ToLocation = _viewModel.ToLocation,
                            StockTransferProducts = new ObservableCollection<StockTransferProducts>(),
                            DocumentDate = Datetime,
                            DocumentNo = _viewModel.EWayBillModel.DocumentNo,
                            vehicleNo = _viewModel.EWayBillModel.VehicleNo,
                            VehicleType = _viewModel.EWayBillModel.VehicleType,
                            DocumentName = _viewModel.EWayBillModel.DocumentName,
                            Id = (int)_viewModel.EWayBillModel.Id,
                            IsTransportAdded = true,


                           
                        };

                        foreach (var c in _viewModel.transferProducts)
                        {
                            var product = new StockTransferProducts()
                            {
                                ProductName = c.ProductName,
                                TransferQty = c.TransferQty,
                                SellingPrice = c.SellingPrice,
                                Gst = c.GST,
                                HsnCode = c.HSNCode==null?"00000000":c.HSNCode
                            };

                            temp.StockTransferProducts.Add(product);
                        }

                        _eventAggregator.GetEvent<RegenerateEwayBill>().Publish(temp);

                        return;
                    }
                }
            }

            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
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
            catch (Exception)
            {


            }
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
}
