using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.Stock;
using FalcaPOS.Stock.Views;
using MaterialDesignThemes.Wpf;
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
using System.Windows.Controls;

namespace FalcaPOS.Stock.ViewModels
{
    public class StockReceiverViewModel : BindableBase
    {
        private readonly IEventAggregator eventAggregator;

        private readonly Logger logger;

        private readonly ProgressService _ProgressService;

        public DelegateCommand<ReceiverViewModel> ProductReceiveCommand { get; private set; }

        public DelegateCommand<ReceiverViewModel> DownloadStockTranferPDFCommand { get; private set; }

        public DelegateCommand<object> TransportFileAttachement { get; private set; }

        private readonly INotificationService notificationService;

        public DelegateCommand<Guid?> DeleteUploadTransFileCommand { get; private set; }

        private readonly IStockTransferService stockTransferService;

        public DelegateCommand StockReceiverRefreshCommand { get; set; }

        public DelegateCommand<object> StockReceiverConfirm { get; private set; }

        public StockReceiverViewModel(ProgressService ProgressService, IStockTransferService StockTransferService, EventAggregator EventAggregator, INotificationService NotificationService, Logger Logger)
        {
            notificationService = NotificationService ?? throw new ArgumentNullException(nameof(NotificationService));


            _ProgressService = ProgressService ?? throw new ArgumentNullException(nameof(ProgressService));

            stockTransferService = StockTransferService ?? throw new ArgumentNullException(nameof(StockTransferService));

            eventAggregator = EventAggregator ?? throw new ArgumentNullException(nameof(EventAggregator));

            logger = Logger ?? throw new ArgumentNullException(nameof(Logger));

            ProductReceiveCommand = new DelegateCommand<ReceiverViewModel>(ProductReceivePopup);

            TransportFileAttachement = new DelegateCommand<object>(AddFileOpenDialog);

            DeleteUploadTransFileCommand = new DelegateCommand<Guid?>(DeleteUploadFile);

            DownloadStockTranferPDFCommand = new DelegateCommand<ReceiverViewModel>(GetDownloadStockTransferPDF);

            StockReceiverRefreshCommand = new DelegateCommand(GetStockReceiverList);

            StockReceiverConfirm = new DelegateCommand<object>(StockReceiverUpdate);

            _ = EventAggregator.GetEvent<StockTransferEvent>().Subscribe(() => GetStockReceiverList(), ThreadOption.PublisherThread);



        }



        public void StockReceiverUpdate(object param)
        {
            try
            {
                if (UpdateReceivedProduct.TransportCharges > 0 || UpdateReceivedProduct.Others > 0)
                {
                    if (TransportFileUploadInfo == null || TransportFileUploadInfo.Count() == 0)
                    {
                        notificationService.ShowMessage("Please add attachment", NotificationType.Error);
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
                logger.LogError(_ex.Message);
            }
        }


        public async void GetDownloadStockTransferPDF(ReceiverViewModel model)
        {
            try
            {
                if (model != null)
                {
                    await _ProgressService.StartProgressAsync();


                    await Task.Run(async () =>
                    {
                        var result = await stockTransferService.GetStockTransferPdf(model.SRNumber);

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
                                    logger.LogError("Getting error in stock receive cheque", _ex);
                                    //fall back to save manually if file creation is blocked.
                                    SaveFileManually(result.Data.FileStream);
                                }
                                catch (Exception _ex)
                                {
                                    logger.LogError("Getting error in stock receive cheque", _ex);
                                    notificationService.ShowMessage("Ann error occred while showing  pdf", NotificationType.Error);

                                }



                            });

                        }
                        else
                        {
                            if (result != null && !result.IsSuccess && result.Error.IsValidString())
                            {
                                notificationService.ShowMessage(result.Error, NotificationType.Error);
                            }
                        }

                    });

                    await _ProgressService.StopProgressAsync();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        public async void GetStockReceiverList()
        {
            try
            {

                await Task.Run(async () =>
                {

                    var _result = await stockTransferService.GetStockReceiver(AppConstants.LoggedInStoreInfo.StoreId);

                    Application.Current?.Dispatcher?.Invoke(() =>
                    {

                        if (_result != null && _result.IsSuccess)
                        {
                            var receiverModels = _result.Data;

                            StockReceivers = new ObservableCollection<ReceiverViewModel>(receiverModels);
                        }
                        else
                        {
                            StockReceivers = new ObservableCollection<ReceiverViewModel>();
                        }


                    });


                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        public async void ProductReceivePopup(ReceiverViewModel stockReceiverProduct)
        {
            try
            {
                if (transportfileUploadInfo != null)
                {
                    transportfileUploadInfo.Clear();
                }

                if (stockReceiverProduct != null)
                {
                    if (stockReceiverProduct.StockTransferList != null && stockReceiverProduct.StockTransferList.Count > 0)
                    {
                        int i = 1;
                        foreach (var item in stockReceiverProduct.StockTransferList)
                        {
                            if (item.NewSellingPrice == 0)
                            {
                                notificationService.ShowMessage("Please enter new selling price at " + item.ProductSKU + " Row " + i, NotificationType.Error);
                                return;
                            }

                            if (item.NewSellingPrice < item.SellingPrice)
                            {
                                notificationService.ShowMessage("New selling price should not be less than old selling price " + item.ProductSKU + " Row " + i, NotificationType.Error);
                                return;
                            }

                            if (item.CategoryName.ToLower() == "fertilizers")
                            {
                                if (item.NewSellingPrice > item.Mrp)
                                {
                                    notificationService.ShowMessage("New selling should be less than or equal to MRP price " + item.Mrp + " sku " + item.ProductSKU + " Row " + i, NotificationType.Error);
                                    return;
                                }
                            }

                            i++;


                        }

                        UpdateReceivedProduct = stockReceiverProduct;
                        StockReceiverConfrimPopup confrimPopup = new StockReceiverConfrimPopup();
                        confrimPopup.DataContext = this;
                        await DialogHost.Show(confrimPopup, "RootDialog", updateEventHandler);
                    }
                }



            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
        }

        private async void updateEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {
                var _viewmodel = (StockReceiverViewModel)eventArgs.Parameter;
                if (_viewmodel != null)
                {
                    List<ReceiverUpdateProduct> updateModel = new List<ReceiverUpdateProduct>();
                    if (_viewmodel.UpdateReceivedProduct.StockTransferList != null && _viewmodel.UpdateReceivedProduct.StockTransferList.Count > 0)
                    {
                        foreach (var item in _viewmodel.UpdateReceivedProduct.StockTransferList)
                        {
                            updateModel.Add(new ReceiverUpdateProduct()
                            {
                                ProductId = item.ProductId,
                                NewSellingPrice = item.NewSellingPrice,
                                StockProuductId = item.StockProuductId,
                            });
                        }
                    }
                    ReceiverUpdateModel receiverUpdateModel = new ReceiverUpdateModel()
                    {
                        TransferId = _viewmodel.UpdateReceivedProduct.TransferId,
                        Others = _viewmodel.UpdateReceivedProduct.Others,
                        TransportCharges = _viewmodel.UpdateReceivedProduct.TransportCharges,
                        ReceiverUpdateProduct = updateModel,
                    };

                    await _ProgressService.StartProgressAsync();

                    await Task.Run(async () =>
                    {

                        var _result = await stockTransferService.UpdateStockReceiver(receiverUpdateModel);

                        Application.Current?.Dispatcher?.Invoke(async () =>
                        {
                            if (TransportFileUploadInfo != null && transportfileUploadInfo.Count > 0)
                            {
                                if (_result != null && _result.IsSuccess)
                                {
                                    var _updateresult = await stockTransferService.UploadFiles(Convert.ToString(UpdateReceivedProduct.SRNumber), TransportFileUploadInfo.ToArray());
                                    if (_updateresult != null && _updateresult.IsSuccess)
                                    {
                                        notificationService.ShowMessage(_result.Data, NotificationType.Success);

                                        transportfileUploadInfo.Clear();

                                    }

                                }
                            }

                            notificationService.ShowMessage(_result.Data, NotificationType.Success);

                            GetStockReceiverList();

                            await _ProgressService.StopProgressAsync();
                        });


                    });
                }

            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
            finally {
                await _ProgressService.StopProgressAsync();
            }
        }

        public void AddFileOpenDialog(object bank)
        {
            try
            {

                if (TransportFileUploadInfo != null && TransportFileUploadInfo.Count >= 1)
                {
                    notificationService.ShowMessage("1 files already added. Delete old file", NotificationType.Information);

                    logger.LogWarning($"Max File attachments added");

                    return;
                }

                if (UpdateReceivedProduct.TransportCharges == 0 && UpdateReceivedProduct.Others == 0)
                {

                    notificationService.ShowMessage("Please Enter Transport or Other charges ", NotificationType.Error);
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
                    if (TransportFileUploadInfo == null)
                    {
                        TransportFileUploadInfo = new ObservableCollection<FileUploadInfo>();
                    }
                    if (dialog.FileNames != null && dialog.FileNames.Count() > 1)
                    {
                        notificationService.ShowMessage("Can't add more than 1 file", NotificationType.Information);

                        logger.LogWarning($"Max File attachments added");

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
                                    notificationService.ShowMessage($"File {Path.GetFileName(_fileName)} size should be less than 10 mb in size", NotificationType.Error);

                                    logger.LogWarning($"Filename {_fileName} has size of {_fileInfo.Length / (1024 * 1024)} mb");

                                    continue;
                                }

                                //dont again add the same file .causes file access issue while reading file stream

                                if (TransportFileUploadInfo.Any(x => x.FileName == Path.GetFileName(_fileName)))
                                {
                                    notificationService.ShowMessage($"File {Path.GetFileName(_fileName)} is already added", NotificationType.Information);

                                    logger.LogWarning($"File was already added skipping file {_fileName}");

                                    continue;
                                }

                                logger.LogInformation($"Adding file {_fileName}");

                                TransportFileUploadInfo?.Add(new FileUploadInfo
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
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
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

        private void DeleteUploadFile(Guid? fileId)
        {
            if (fileId == null) return;

            var _file = TransportFileUploadInfo?.FirstOrDefault(x => x.FileId == fileId);

            if (_file != null)
            {
                logger.LogInformation($"File Deleted {_file.FileName}");

                TransportFileUploadInfo?.Remove(_file);
            }

        }
        private ObservableCollection<FileUploadInfo> transportfileUploadInfo;
        public ObservableCollection<FileUploadInfo> TransportFileUploadInfo
        {
            get { return transportfileUploadInfo; }
            set { SetProperty(ref transportfileUploadInfo, value); }
        }

        private ObservableCollection<ReceiverViewModel> stockReceivers;

        public ObservableCollection<ReceiverViewModel> StockReceivers
        {
            get { return stockReceivers; }
            set { SetProperty(ref stockReceivers, value); }
        }

        private ReceiverViewModel _updateReceivedProduct;

        public ReceiverViewModel UpdateReceivedProduct
        {
            get { return _updateReceivedProduct; }
            set { SetProperty(ref _updateReceivedProduct, value); }
        }


    }



}
