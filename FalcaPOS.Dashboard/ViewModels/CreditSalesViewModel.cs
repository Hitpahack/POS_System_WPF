using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Dashboard.Views;
using FalcaPOS.Entites.Sales;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FalcaPOS.Dashboard.ViewModels
{
    public class CreditSalesViewModel : BindableBase
    {
        private readonly Logger _logger;

        private readonly INotificationService _notificationService;

        private readonly ProgressService _ProgressService;

        private readonly ISalesService _salesService;
        public DelegateCommand<object> CreditFinancePopUpCommand { get; private set; }

        public DelegateCommand<object> CreditFinanceUpdateCommand { get; private set; }

        public DelegateCommand<object> DownloadChequeCommand { get; private set; }
        public DelegateCommand<object> RefreshCreditSalesViewCommand { get; private set; }

        public DelegateCommand<object> SearchCreditSalesViewCommand { get; private set; }

        private readonly IInvoiceFileService _invoiceFileService;

        private readonly ICommonService _commonService;


        public DelegateCommand ExportCreditSalesResultToCommand { get; private set; }

        public CreditSalesViewModel(ICommonService commonService, ISalesService salesService, INotificationService notificationService, ProgressService ProgressService, Logger logger, IInvoiceFileService invoiceFileService)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _ProgressService = ProgressService ?? throw new ArgumentNullException(nameof(ProgressService));

            _salesService = salesService ?? throw new ArgumentNullException(nameof(salesService));

            CreditFinancePopUpCommand = new DelegateCommand<object>(openPopUp);

            CreditFinanceUpdateCommand = new DelegateCommand<object>(Update);

            DownloadChequeCommand = new DelegateCommand<object>(DownloadCheque);

            _invoiceFileService = invoiceFileService ?? throw new ArgumentNullException(nameof(invoiceFileService));

            RefreshCreditSalesViewCommand = new DelegateCommand<object>(Refresh);

            SearchCreditSalesViewCommand = new DelegateCommand<object>(Search);

            ExportCreditSalesResultToCommand = new DelegateCommand(ExportResultToExcel);

            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));

            Load();
        }

        private async void Search(object obj)
        {
            try
            {
                var viewModel = ((CreditSalesViewModel)obj);
                if (viewModel != null)
                {
                    if (string.IsNullOrEmpty(viewModel.RealizeFromDate))
                    {
                        _notificationService.ShowMessage("Please entet realize date", Common.NotificationType.Error);
                        return;
                    }

                    if (string.IsNullOrEmpty(viewModel.RealizeToDate))
                    {
                        _notificationService.ShowMessage("Please entet realize date", Common.NotificationType.Error);
                        return;
                    }

                    if (!String.IsNullOrEmpty(viewModel.RealizeFromDate) && !string.IsNullOrEmpty(viewModel.RealizeToDate))
                    {
                        DateTime dt1 = Convert.ToDateTime(viewModel.RealizeFromDate);
                        DateTime dt2 = Convert.ToDateTime(viewModel.RealizeToDate);
                        if (dt2 < dt1)
                        {
                            _notificationService.ShowMessage("From Date should be less than or equal to To Date", Common.NotificationType.Error);
                            return;
                        }
                    }

                    await _ProgressService.StartProgressAsync();

                    await Task.Run(async () =>
                   {
                       var _result = await _salesService.SearchCreditSalesFinance(viewModel.RealizeFromDate, viewModel.RealizeToDate);

                       if (_result != null && _result.IsSuccess)
                       {
                           List<FinanceCreditSalesViewModel> salesInvoices = new List<FinanceCreditSalesViewModel>();
                           foreach (var Sales in _result.Data)
                           {
                               salesInvoices.Add(new FinanceCreditSalesViewModel()
                               {
                                   InvoiceNumber = Sales.InvoiceNumber,
                                   InvoiceDate = Sales.InvoiceDate,
                                   CustomerName = Sales.CustomerName,
                                   Phone = Sales.Phone,
                                   PayableAmount = Sales.PayableAmount,
                                   Cheque = Sales.Cheque,
                                   ChequeNumber = Sales.ChequeNumber,
                                   ChequeDate = Sales.ChequeDate,
                                   OrderTacknBy = Sales.OrderTacknBy,
                                   RealizeDate = Sales.RealizeDate,
                                   SalesId = Sales.SalesId,
                                   IsChequeupdatebtn = (String.IsNullOrEmpty(Sales.ChequeDate)) ? false : (Sales.RealizeDate == null && (AppConstants.ROLE_FINANCE == AppConstants.USER_ROLES[0].ToString())) == true ? true : false
                               });
                           }
                           CreditList = salesInvoices;
                           TotalCount = "Total Count " + _result.Data.Count();
                           IsExportEnabled = true;
                       }
                       else
                       {
                           _notificationService.ShowMessage(_result?.Error ?? AppConstants.CommonError, Common.NotificationType.Error);
                           CreditList = null;
                           TotalCount = "";
                       }


                   });
                    await _ProgressService.StopProgressAsync();

                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public void Refresh(object obj)
        {
            Load();
            RealizeFromDate = null;
            RealizeToDate = null;



        }
        public async void DownloadCheque(object param)
        {
            try
            {
                var _model = (param as CreditSalesViewModel).PopupDetails;

                if (_model != null && _model.SalesId > 0)
                {
                    var _result = await _invoiceFileService.DownloadFile(_model.SalesId);

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
            catch (Exception _ex)
            {

                _logger.LogError("Getting error in download cheque", _ex);
            }

        }

        public async void openPopUp(object obj)
        {
            try
            {
                var model = (obj as FinanceCreditSalesViewModel);

                if (model != null)
                {
                    CreditSalesUpdate popUp = new CreditSalesUpdate();
                    popUp.DataContext = this;
                    PopupDetails.InvoiceNumber = model.InvoiceNumber;
                    PopupDetails.ChequeNumber = model.ChequeNumber;
                    PopupDetails.ChequeDate = model.ChequeDate;
                    PopupDetails.RealizeDate = model.RealizeDate;
                    //reusing this propert as picture id
                    PopupDetails.SalesId = model.SalesId;
                    PopupDetails.Cheque = model.Cheque;
                    DisplayEndDate = model.ChequeDate != null ? DateTime.Parse(model.ChequeDate).AddMonths(3).ToString("dd-MM-yyyy") : null;

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

                var _viewmodel = (CreditSalesViewModel)eventArgs.Parameter;
                if (_viewmodel != null)
                {
                    await _ProgressService.StartProgressAsync();

                    await Task.Run(async () =>
                    {


                        var _result = await _salesService.UpdateCredtiSalesFinance(PopupDetails.InvoiceNumber, PopupDetails.RealizeDate);
                        Application.Current?.Dispatcher?.Invoke(async () =>
                        {

                            if (_result != null && _result.IsSuccess)
                            {

                                _notificationService.ShowMessage(_result?.Data ?? "Updated Successfully", Common.NotificationType.Success);
                                Load();

                            }
                            else
                            {
                                _notificationService.ShowMessage(_result?.Error ?? AppConstants.CommonError, Common.NotificationType.Error);
                            }

                            await _ProgressService.StopProgressAsync();
                        });


                    });
                }

            }

            catch (Exception ex)
            {
                _logger.LogError("Getting error in download cheque", ex);

                _notificationService.ShowMessage(AppConstants.CommonError, Common.NotificationType.Error);

                await _ProgressService.StopProgressAsync();

            }


        }

        public void Update(object param)
        {
            try
            {

                if (String.IsNullOrEmpty(PopupDetails.RealizeDate))
                {
                    _notificationService.ShowMessage("Please enter Realize Date", Common.NotificationType.Error);
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
                _logger.LogError("geeting error in  update popup ", _ex);

            }
        }

        public async void Load()
        {
            try
            {

                await Task.Run(async () =>
                {
                    var result = await _salesService.GetCreditListfinancePage();

                    if (result != null && result.IsSuccess && result.Data != null)
                    {
                        List<FinanceCreditSalesViewModel> salesInvoices = new List<FinanceCreditSalesViewModel>();
                        foreach (var Sales in result.Data)
                        {
                            salesInvoices.Add(new FinanceCreditSalesViewModel()
                            {
                                InvoiceNumber = Sales.InvoiceNumber,
                                InvoiceDate = Sales.InvoiceDate,
                                CustomerName = Sales.CustomerName,
                                Phone = Sales.Phone,
                                PayableAmount = Sales.PayableAmount,
                                Cheque = Sales.Cheque,
                                ChequeNumber = Sales.ChequeNumber,
                                ChequeDate = Sales.ChequeDate,
                                OrderTacknBy = Sales.OrderTacknBy,
                                RealizeDate = Sales.RealizeDate,
                                SalesId = Sales.SalesId,
                                Remarks = Sales.Remarks,
                                IsChequeupdatebtn = (String.IsNullOrEmpty(Sales.ChequeDate)) ? false : (Sales.RealizeDate == null && (AppConstants.ROLE_FINANCE == AppConstants.USER_ROLES[0].ToString())) == true ? true : false
                            });
                        }
                        CreditList = salesInvoices;
                        TotalCount = "Total Count " + result.Data.Count();
                        IsExportEnabled = true;

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



        private FinanceCreditSalesViewModel _popupdetails = new FinanceCreditSalesViewModel();

        public FinanceCreditSalesViewModel PopupDetails
        {
            get => _popupdetails;
            set => SetProperty(ref _popupdetails, value);
        }


        private List<FinanceCreditSalesViewModel> _creditList;
        public List<FinanceCreditSalesViewModel> CreditList
        {
            get => _creditList;
            set => SetProperty(ref _creditList, value);
        }

        private string _totalcount;
        public string TotalCount
        {
            get { return _totalcount; }
            set { SetProperty(ref _totalcount, value); }
        }


        private string _displayendDate;
        public string DisplayEndDate
        {
            get { return _displayendDate; }
            set { SetProperty(ref _displayendDate, value); }
        }


        private string _realizeFromDate;
        public string RealizeFromDate
        {
            get { return _realizeFromDate; }
            set { SetProperty(ref _realizeFromDate, value); }
        }

        private string _realizeToDate;
        public string RealizeToDate
        {
            get { return _realizeToDate; }
            set { SetProperty(ref _realizeToDate, value); }
        }

        public string _fileName { get; set; }

        private bool _isExportEnabled;
        public bool IsExportEnabled
        {
            get => _isExportEnabled;
            set => _ = SetProperty(ref _isExportEnabled, value);
        }




        private void ExportResultToExcel()
        {

            if (CreditList == null || !CreditList.Any())
            {
                _notificationService.ShowMessage("No records found to export", Common.NotificationType.Error);

                return;
            }

            List<CreditSalesExport> creditSalesExports = new List<CreditSalesExport>();
            foreach (var item in CreditList)
            {
                creditSalesExports.Add(new CreditSalesExport()
                {
                    InvoiceNumber = item.InvoiceNumber,
                    InvoiceDate = item.InvoiceDate.ToString("dd-MM-yyyy"),
                    CustomerName = item.CustomerName,
                    PhoneNumber = item.Phone,
                    NetPayableAmount = item.PayableAmount,
                    Cheque = (double)item.Cheque,
                    ChequeNumber = item.ChequeNumber,
                    OrderTackenBy = item.OrderTacknBy,
                    Remarks = item.Remarks,
                    ChequeDate = item.ChequeDate,
                    RealizeDate = item.RealizeDate


                });
            }

            IsExportEnabled = false;

            if (!_fileName.IsValidString())
            {
                _fileName = "Credit Sales Report";
            }
            _fileName += Guid.NewGuid().ToString().Substring(0, 3);

            bool _result = _commonService.ExportToXL(creditSalesExports, _fileName, skipfileName: true, "Credit Sales");

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
    }

    public class CreditSalesExport
    {
        public string InvoiceNumber { get; set; }

        public string InvoiceDate { get; set; }

        public string CustomerName { get; set; }

        public string PhoneNumber { get; set; }

        public float NetPayableAmount { get; set; }

        public double Cheque { get; set; }

        public string ChequeNumber { get; set; }

        public string ChequeDate { get; set; }

        public string RealizeDate { get; set; }

        public string Remarks { get; set; }

        public string OrderTackenBy { get; set; }
    }
}
