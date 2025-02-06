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
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Documents.Spreadsheet.Expressions.Functions;

namespace FalcaPOS.Stock.ViewModels
{
    public class StockApprovalViewModel : BindableBase
    {
        private readonly IEventAggregator eventAggregator;

        private readonly Logger logger;

        private readonly ProgressService _ProgressService;

        public DelegateCommand<StockTrnasferModel> ProductApprovalCommand { get; private set; }

        public DelegateCommand<StockTrnasferModel> DownloadStockTranferPDFCommand { get; private set; }


        private readonly INotificationService notificationService;


        private readonly IStockTransferService stockTransferService;

        public DelegateCommand<object> StockTransferConfirm { get; private set; }

        public DelegateCommand StockApprovalRefreshCommand { get; private set; }

        public DelegateCommand<object> StockTransferApproveCommand { get; private set; }

        public DelegateCommand<object> StockTransferRejectCommand { get; private set; }

        public DelegateCommand<object> ApprovalTransferConfirmCommand { get; private set; }
        public DelegateCommand<object> EditQtyChangeCommand { get; private set; }

        public DelegateCommand<object> EditSaveandApproveCommand { get; private set; }

        public DelegateCommand<object> RejectTransferConfirmCommand { get; set; }


        public StockApprovalViewModel(ProgressService ProgressService, IStockTransferService StockTransferService, EventAggregator EventAggregator, INotificationService NotificationService, Logger Logger)
        {
            notificationService = NotificationService ?? throw new ArgumentNullException(nameof(NotificationService));

            _ProgressService = ProgressService ?? throw new ArgumentNullException(nameof(ProgressService));

            stockTransferService = StockTransferService ?? throw new ArgumentNullException(nameof(StockTransferService));

            eventAggregator = EventAggregator ?? throw new ArgumentNullException(nameof(EventAggregator));

            logger = Logger ?? throw new ArgumentNullException(nameof(Logger));

            ProductApprovalCommand = new DelegateCommand<StockTrnasferModel>(ProductApprovalPopup);

            DownloadStockTranferPDFCommand = new DelegateCommand<StockTrnasferModel>(GetDownloadStockTransferPDF);
         
            IsApproval = AppConstants.USER_ROLES != null && (AppConstants.USER_ROLES[0] == AppConstants.ROLE_PURCHASE_MANAGER || AppConstants.USER_ROLES[0] == AppConstants.ROLE_DIRECTOR) ? true : false;

            StockTransferConfirm = new DelegateCommand<object>(StockApprovalUpdate);

            StockApprovalRefreshCommand = new DelegateCommand(GetStockApprovalLsit);

            StockTransferApproveCommand = new DelegateCommand<object>(ProductApprovalPopup);

            StockTransferRejectCommand = new DelegateCommand<object>(ProductRejectPopup);

            ApprovalTransferConfirmCommand = new DelegateCommand<object>(ApprovalUpdate);

            EditQtyChangeCommand = new DelegateCommand<object>(EditQtyChange);

            EditSaveandApproveCommand = new DelegateCommand<object>(SaveandApprove);


            RejectTransferConfirmCommand = new DelegateCommand<object>(RejectUpdate);

            GetStockApprovalLsit();


        }

        public void RejectUpdate(object param)
        {
            try
            {
                if (string.IsNullOrEmpty(RejectRemarks) || RejectRemarks.Trim().Length == 0)
                {
                    notificationService.ShowMessage("Please enter the reason", NotificationType.Error);
                    return;
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

        public void SaveandApprove(object param)
        {
            try
            {
                if (EditTransferRequest!=null)
                {
                    if (EditTransferRequest.transferProducts.Count > 0)
                    {
                        int i = 1;
                       foreach(var item in  EditTransferRequest.transferProducts)
                        {
                            if (string.IsNullOrEmpty(item.Remarks))
                            {
                                notificationService.ShowMessage("Please enter reason at row "+i , NotificationType.Error);
                                return;
                            }
                            if (item.TransferQty > item.AvailableQty)
                            {
                                notificationService.ShowMessage("Request qty should not more than available qty " + i, NotificationType.Error);
                                return;
                            }
                            if (item.TransferQty==0)
                            {
                                notificationService.ShowMessage("Request qty should not zero " + i, NotificationType.Error);
                                return;
                            }
                            i++;
                        }
                    }
                }
               
                var TargetClose = ((Button)(param));
                var dynamicCommand = TargetClose.Command;
                dynamicCommand.CanExecute(true);
                //pass Model as Arguments
                dynamicCommand.Execute(this);

            }
            catch(Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
        }
        public async void EditQtyChange(object obj)
        {
            try
            {
                var _viewModel = (TransferCompletedViewModel)obj;
                if(_viewModel != null )
                {
                    EditTransferProduct editTransferProduct = new EditTransferProduct();
                    editTransferProduct.DataContext = this;
                    EditTransferRequest = _viewModel;
                    await DialogHost.Show(editTransferProduct, "RootDialog", EditEventHandler);

                }
            }
            catch(Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
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

        public void ApprovalUpdate(object param)
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
        public async void ProductApprovalPopup(object obj)
        {
            try
            {
                var _viemodel = (TransferCompletedViewModel)obj;
                if (_viemodel != null)
                {
                    TransferApprovalConfirmationPopup confrimPopup = new TransferApprovalConfirmationPopup();
                    confrimPopup.DataContext = this;
                    Message = "Are you confirm to approve?";
                    StockTransferId = _viemodel.StockTransferId;
                    await DialogHost.Show(confrimPopup, "RootDialog", ApprovalupdateEventHandler);
                }
               

            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
        }

        public async void ProductRejectPopup(object obj)
        {
            try
            {
                var _viemodel = (TransferCompletedViewModel)obj;
                if (_viemodel != null)
                {
                    TransferRejectConfirmPopup RejectconfrimPopup = new TransferRejectConfirmPopup();
                    RejectconfrimPopup.DataContext = this;
                    Message = "Are you confirm to Reject?";
                    StockTransferId= _viemodel.StockTransferId;
                    RejectRemarks = null;
                    await DialogHost.Show(RejectconfrimPopup, "RootDialog", RejectupdateEventHandler);
                }


            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
        }

        private async void EditEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {

                var viewModel = (StockApprovalViewModel)eventArgs.Parameter;
                if (viewModel != null)
                {

                    await _ProgressService.StartProgressAsync();

                    await Task.Run(async () =>
                    {

                        var _result = await stockTransferService.UpdateStockTranferQty(EditTransferRequest);
                        if(_result!=null && _result.IsSuccess)
                        {
                            notificationService.ShowMessage(_result.Data, NotificationType.Success);
                            GetStockApprovalLsit();
                          
                        }
                        else
                        {
                            notificationService.ShowMessage(_result.Error, NotificationType.Error);
                        }



                    });
                }
            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
            finally
            {
                await _ProgressService.StopProgressAsync();
            }

        }


        private async void ApprovalupdateEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {

                var viewModel = (StockApprovalViewModel)eventArgs.Parameter;
                if (viewModel != null)
                {

                    await _ProgressService.StartProgressAsync();

                    await Task.Run(async () =>
                    {
                      
                        var _result = await stockTransferService.UpdateStockTrnasferApproveV2(StockTransferId);

                        Application.Current?.Dispatcher?.Invoke( () =>
                        {

                            if (_result != null && _result.IsSuccess)
                            {
                                notificationService.ShowMessage(_result.Data, NotificationType.Success);
                                GetStockApprovalLsit();
                            }
                            else
                            {
                                notificationService.ShowMessage(_result.Error, NotificationType.Error);
                            }


                            
                        });


                    });
                }
            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
            finally
            {
                await _ProgressService.StopProgressAsync();
            }

        }

        private async void RejectupdateEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {

                var viewModel = (StockApprovalViewModel)eventArgs.Parameter;
                if (viewModel != null)
                {

                    await _ProgressService.StartProgressAsync();

                    await Task.Run(async () =>
                    {
                        
                        var _result = await stockTransferService.UpdateStockTransferRejectV2(StockTransferId, RejectRemarks);

                        Application.Current?.Dispatcher?.Invoke(() =>
                        {

                            if (_result != null && _result.IsSuccess)
                            {
                                notificationService.ShowMessage(_result.Data, NotificationType.Success);
                                GetStockApprovalLsit();
                            }
                            else
                            {
                                notificationService.ShowMessage(_result.Error, NotificationType.Error);
                            }



                        });


                    });
                }
            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
            finally
            {
                await _ProgressService.StopProgressAsync();
            }

        }

        public async void GetStockApprovalLsit()
        {
            try
            {

                await Task.Run(async () =>
                {

                    var _result = await stockTransferService.GetStockTransferRequestList();

                    Application.Current?.Dispatcher?.Invoke(() =>
                    {

                        if (_result != null && _result.IsSuccess)
                        {
                            var _resquestList = _result.Data.ToList();
                           
                            StockTransferRequest = new ObservableCollection<TransferCompletedViewModel>(_resquestList);
                        }
                        else
                        {
                            
                            if(_result.Data==null || _result.Data.Count() == 0)
                            {
                              StockTransferRequest=new ObservableCollection<TransferCompletedViewModel>();
                            }
                            notificationService.ShowMessage(_result.Error, NotificationType.Error);



                        }


                    });


                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
            finally
            {

            }
        }

        public async void GetDownloadStockTransferPDF(StockTrnasferModel model)
        {
            try
            {
                if (model != null)
                {
                    await _ProgressService.StartProgressAsync();

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

                    await _ProgressService.StopProgressAsync();
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

        private ObservableCollection<TransferCompletedViewModel> _stockTransferRequest;

        public ObservableCollection<TransferCompletedViewModel> StockTransferRequest
        {
            get { return _stockTransferRequest; }
            set { SetProperty(ref _stockTransferRequest, value); }
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

        private string _message ;

        public string Message
        {
            get { return _message; }
            set {  SetProperty(ref _message, value); }
        }

        private int _StockTransferId;

        public int StockTransferId
        {
            get { return _StockTransferId; }
            set { SetProperty(ref _StockTransferId , value); }
        }

        private TransferCompletedViewModel _editTransferRequest;

        public TransferCompletedViewModel EditTransferRequest
        {
            get { return _editTransferRequest; }
            set { SetProperty(ref _editTransferRequest, value); }
        }

        private string  _RejectRemarks;

        public string  RejectRemarks
        {
            get { return _RejectRemarks; }
            set { SetProperty( ref _RejectRemarks, value); }
        }

        private bool _isReason;

        public bool IsReason
        {
            get { return _isReason; }
            set { SetProperty(ref _isReason, value); }
        }

    }
}
