using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Sales;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.Sales.ViewModels
{
    public class SalesListViewModel : BindableBase
    {
        private readonly ISalesService _salesService;

        private readonly IEventAggregator _eventAggregators;

        private readonly INotificationService _notificationService;

        private readonly ProgressService _progressService;

        public DelegateCommand RefreshSalesCommand { get; private set; }

        public DelegateCommand<SalesInvoice> GetInvoicePDFCommand { get; private set; }

        public DelegateCommand SalesSearchCommand { get; private set; }

        public DelegateCommand<object> RowDoubleClickCommand { get; private set; }

        private readonly Logger _logger;

        public SalesListViewModel(ISalesService salesService, Logger logger, IEventAggregator eventAggregators, INotificationService notificationService, ProgressService progressService)
        {
            _salesService = salesService ?? throw new ArgumentNullException(nameof(salesService));

            _eventAggregators = eventAggregators ?? throw new ArgumentNullException(nameof(eventAggregators));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _progressService = progressService;

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            RefreshSalesCommand = new DelegateCommand(() =>
            {
                Sales = null;
                TotalCount = 0;
                CustomerNameorNumber = null;
                InvoiceNumber = null;
                InvoiceFromDate = null;
                InvoiceToDate = null;

            });

            SalesSearchCommand = new DelegateCommand(LoadSales);

            GetInvoicePDFCommand = new DelegateCommand<SalesInvoice>(GetInvoicePDF);

        }


        private async void GetInvoicePDF(SalesInvoice salesObj)
        {
            if (salesObj != null)
            {
                await _progressService.StartProgressAsync();

                await Task.Run(async () =>
                {
                    if (salesObj.Total == 0)
                    {
                        _notificationService.ShowMessage("This sales invoice has been cancelled, and the attachment cannot be downloaded.", NotificationType.Information);
                        return;
                    }
                    var result = await _salesService.GetSaleInvoicePDF(salesObj.InvoiceNumber);

                    if (result != null && result.success)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {


                            try
                            {                              
                                if (AppConstants.Printer != null)
                                {
                                    ProcessStartInfo _p = new ProcessStartInfo();
                                    if (AppConstants.USER_ROLES[0].Contains("regionalmanager") || AppConstants.USER_ROLES[0].Contains("falcadirector") || AppConstants.USER_ROLES[0].Contains("territorymanager") || AppConstants.USER_ROLES[0].Contains("controlmanager") || AppConstants.USER_ROLES[0].Contains("purchasemanager") || AppConstants.USER_ROLES[0].Contains("finance"))
                                    {
                                        _p.UseShellExecute = true;
                                        _p.FileName = result.data.sign_tiny_response._A4;

                                    }
                                    else
                                    {
                                        _p.UseShellExecute = true;
                                        _p.FileName = AppConstants.Printer.ToLower() == "a4" ? result.data.sign_tiny_response._A4 : result.data.sign_tiny_response._4inch;
                                    }
                                    Process.Start(_p);
                                }
                                else
                                {
                                    _notificationService.ShowMessage("Please add printer type",NotificationType.Error); 
                                    return;
                                }
                              
                                //string path = @"c:\PosInvoices";

                                //var _info = Directory.CreateDirectory(path);

                                //var _fileName = path + "\\" + $"{result.Data.FileName}";

                                //if (!File.Exists(_fileName))
                                //{
                                //    File.WriteAllBytes(_fileName, result.Data.FileStream);
                                //    ProcessStartInfo _p = new ProcessStartInfo
                                //    {
                                //        UseShellExecute = true,
                                //        FileName = _fileName
                                //    };

                                //    Process.Start(_p);

                                //    return;
                                //}
                                //else
                                //{
                                //    //Fall back to save  if file name already present
                                //    SaveFileManually(result.Data.FileStream);
                                //}
                            }
                            catch (UnauthorizedAccessException _ex)
                            {
                                _logger.LogError(_ex.Message);
                                //fall back to save manually if file creation is blocked.
                               // SaveFileManually(result.Data.FileStream);
                            }
                            catch (Exception _ex)
                            {
                                _logger.LogError(_ex.Message);
                                _notificationService.ShowMessage("Ann error occred while showing invoice pdf", NotificationType.Error);

                            }



                        });

                    }
                    else
                    {
                        if (result != null && !result.success && result.message.IsValidString())
                        {
                            _notificationService.ShowMessage(result.message, NotificationType.Error);
                        }
                    }

                });

                await _progressService.StopProgressAsync();
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
                _logger.LogError(ex.Message);

            }
        }

        private int _totalCount;
        public int TotalCount
        {
            get { return _totalCount; }
            set { SetProperty(ref _totalCount, value); }
        }



        private ObservableCollection<SalesInvoice> _sales;
        public ObservableCollection<SalesInvoice> Sales
        {
            get { return _sales; }
            set { SetProperty(ref _sales, value); }
        }



        private async void LoadSales()
        {
            if (InvoiceNumber == null && InvoiceFromDate == null &&
                  InvoiceToDate == null && CustomerNameorNumber == null)
            {
                _notificationService.ShowMessage("Select anything to search!!!..", NotificationType.Error);

                return;
            }
            if ((InvoiceFromDate != null && InvoiceToDate == null) || (InvoiceFromDate == null && InvoiceToDate != null))
            {
                _notificationService.ShowMessage("Please select from and to date range", NotificationType.Error);
                return;
            }
            if (InvoiceFromDate != null && InvoiceToDate != null)
            {
                if (InvoiceToDate < InvoiceFromDate)
                {
                    _notificationService.ShowMessage("To Date can not be smaller than From Date", NotificationType.Error);
                    return;
                }
            }
            if (!string.IsNullOrEmpty(CustomerNameorNumber))
            {
                if (CustomerNameorNumber.Any(char.IsDigit) && CustomerNameorNumber.Any(char.IsLetter))
                {
                    _notificationService.ShowMessage(@"Invalid data in CustomerName/Number field", NotificationType.Error);
                    return;

                }
            }
            var isPhone = false;
            var IsName = false;
            if (!string.IsNullOrEmpty(CustomerNameorNumber))
            {
                isPhone = CustomerNameorNumber.Any(char.IsDigit);
                IsName = !isPhone;
            }
            try
            {


                Sales?.Clear();
                await _progressService.StartProgressAsync();
                await Task.Run(async () =>
                {

                    var _result = await _salesService.GetSales(new SearchParams()
                    {
                        InvoiceNumber = InvoiceNumber,
                        InvoiceFromDate = InvoiceFromDate,
                        InvoiceToDate = InvoiceToDate,
                        CustomerName = IsName ? CustomerNameorNumber : null,
                        CustomerPhone = isPhone ? CustomerNameorNumber : null,
                        IsParent = true,
                    });

                    if (_result != null && _result.IsSuccess && _result.Data.Any())
                    {
                        Application.Current?.Dispatcher?.Invoke(async () =>
                        {
                            Sales = new ObservableCollection<SalesInvoice>(_result.Data);

                            TotalCount = Sales.Count;
                            await _progressService.StopProgressAsync();

                        });
                    }


                });
            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, Common.NotificationType.Error);

            }

            finally
            {
                await _progressService.StopProgressAsync();
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }


        private string _invoiceNumber;

        public string InvoiceNumber
        {
            get { return _invoiceNumber; }
            set { SetProperty(ref _invoiceNumber, value); }
        }

        private string _customerNameorNumber;

        public string CustomerNameorNumber
        {
            get { return _customerNameorNumber; }
            set { SetProperty(ref _customerNameorNumber, value); }
        }


        private DateTime? _invoiceFromDate;
        public DateTime? InvoiceFromDate
        {
            get { return _invoiceFromDate; }
            set { SetProperty(ref _invoiceFromDate, value); }
        }

        private DateTime? _invoiceToDate;
        public DateTime? InvoiceToDate
        {
            get { return _invoiceToDate; }
            set { SetProperty(ref _invoiceToDate, value); }
        }


    }
}
