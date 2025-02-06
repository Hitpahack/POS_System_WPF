using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Stock;
using FalcaPOS.Stock.Views;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FalcaPOS.Stock.ViewModels
{
    public class StockApprovalViewModel : BindableBase
    {
        private readonly IEventAggregator eventAggregator;

        private readonly Logger logger;

        private readonly ProgressService progressService;

        public DelegateCommand<StockTrnasferModel> ProductApprovalCommand { get; private set; }

        public DelegateCommand<StockTrnasferModel> DownloadStockTranferPDFCommand { get; private set; }


        private readonly INotificationService notificationService;


        private readonly IStockTransferService stockTransferService;

        public DelegateCommand<object> StockTransferConfirm { get; private set; }

        public DelegateCommand StockApprovalRefreshCommand { get; private set; }


        public StockApprovalViewModel(ProgressService progressService, IStockTransferService StockTransferService, EventAggregator EventAggregator, INotificationService NotificationService, Logger Logger)
        {
            notificationService = NotificationService ?? throw new ArgumentNullException(nameof(NotificationService));


            progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));

            stockTransferService = StockTransferService ?? throw new ArgumentNullException(nameof(StockTransferService));

            eventAggregator = EventAggregator ?? throw new ArgumentNullException(nameof(EventAggregator));

            logger = Logger ?? throw new ArgumentNullException(nameof(Logger));

            ProductApprovalCommand = new DelegateCommand<StockTrnasferModel>(ProductApprovalPopup);


            DownloadStockTranferPDFCommand = new DelegateCommand<StockTrnasferModel>(GetDownloadStockTransferPDF);

            GetStockApprovalLsit();

            IsApproval = AppConstants.USER_ROLES != null && (AppConstants.USER_ROLES[0] == AppConstants.ROLE_PURCHASE_MANAGER || AppConstants.USER_ROLES[0] == AppConstants.ROLE_DIRECTOR) ? true : false;


            StockTransferConfirm = new DelegateCommand<object>(StockApprovalUpdate);

            StockApprovalRefreshCommand = new DelegateCommand(GetStockApprovalLsit);


        }

        public void StockApprovalUpdate(object param)
        {
            try
            {


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

        public async void ProductApprovalPopup(StockTrnasferModel stockReceiverProduct)
        {
            try
            {
                StockApprovalconfirmPopup confrimPopup = new StockApprovalconfirmPopup();
                StockApproval = stockReceiverProduct;
                confrimPopup.DataContext = this;
                await DialogHost.Show(confrimPopup, "RootDialog", ApprovalupdateEventHandler);

            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
        }

        private async void ApprovalupdateEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {

                var viewModel = (StockApprovalViewModel)eventArgs.Parameter;
                if (viewModel != null)
                {

                    await progressService.StartProgressAsync();

                    await Task.Run(async () =>
                    {

                        var _result = await stockTransferService.UpdateStockTransferApproval(viewModel.StockApproval.TransferOrderNo);

                        Application.Current?.Dispatcher?.Invoke(async () =>
                        {

                            if (_result != null && _result.IsSuccess)
                            {

                            }


                            await progressService.StopProgressAsync();
                        });


                    });
                }
            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }

        }

        public async void GetStockApprovalLsit()
        {
            try
            {

                await Task.Run(async () =>
                {

                    var _result = await stockTransferService.GetStockReceiverList();

                    Application.Current?.Dispatcher?.Invoke(() =>
                    {

                        if (_result != null && _result.IsSuccess)
                        {
                            var receiverModels = _result.Data;

                            StockReceivers = new ObservableCollection<StockTrnasferModel>(receiverModels);
                        }


                    });


                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        public async void GetDownloadStockTransferPDF(StockTrnasferModel model)
        {
            try
            {
                if (model != null)
                {
                    await progressService.StartProgressAsync();

                    model.TransferOrderNo = "STR12345";

                    await Task.Run(async () =>
                    {
                        var result = await stockTransferService.GetStockTransferPdf(model.TransferOrderNo);

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
                                    logger.LogError(_ex.Message);

                                    //fall back to save manually if file creation is blocked.
                                    SaveFileManually(result.Data.FileStream);
                                }
                                catch (Exception _ex)
                                {
                                    logger.LogError(_ex.Message);
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

                    await progressService.StopProgressAsync();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
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
            catch (Exception ex)
            {
                logger.LogError(ex.Message);

            }
        }

        private ObservableCollection<StockTrnasferModel> stockReceivers;

        public ObservableCollection<StockTrnasferModel> StockReceivers
        {
            get { return stockReceivers; }
            set { SetProperty(ref stockReceivers, value); }
        }


        private StockTrnasferModel stockApproval;

        public StockTrnasferModel StockApproval
        {
            get { return stockApproval; }
            set { SetProperty(ref stockApproval, value); }
        }

        private bool isApproval;

        public bool IsApproval
        {
            get { return isApproval; }
            set { SetProperty(ref isApproval, value); }
        }
    }
}
