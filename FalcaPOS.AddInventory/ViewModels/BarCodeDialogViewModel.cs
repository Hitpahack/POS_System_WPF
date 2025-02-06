using FalcaPOS.Common;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.AddInventory.ViewModels
{
    public class BarCodeDialogViewModel : BindableBase, IDialogAware
    {

        private readonly IBarCodeService _barCodeService;

        private readonly Logger _logger;

        private readonly IPrinterService _printerService;
        private readonly INotificationService _notificationService;
        public DelegateCommand PrintBarCodeCommand { get; private set; }

        public DelegateCommand CancelCommand { get; private set; }

        public DelegateCommand RefreshPrinterCommand { get; private set; }
        public BarCodeDialogViewModel(IBarCodeService barCodeService,
            Logger logger,
            IPrinterService printerService,
            INotificationService notificationService

            )
        {
            _barCodeService = barCodeService ?? throw new ArgumentNullException(nameof(barCodeService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _printerService = printerService ?? throw new ArgumentNullException(nameof(printerService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            RefreshPrinterCommand = new DelegateCommand(RefreshPrinters);
            CancelCommand = new DelegateCommand(() => RequestClose?.Invoke(null));

            PrintBarCodeCommand = new DelegateCommand(PrintBarCodes);

        }

        private void PrintBarCodes()
        {
            try
            {
                if (ApplicationSettings.APP_ENVIRONMENT != "PRODUCTION")
                {
                    //no need check print in test
                    _notificationService.ShowMessage("Barcode print is available only in production", NotificationType.Information);

                    return;
                }

                if (!BarCode.IsValidString())
                {
                    _notificationService.ShowMessage("Invalid barcode to print", NotificationType.Error);
                    return;
                }
                if (PrintCount <= 0)
                {
                    _notificationService.ShowMessage("Invalid print count", NotificationType.Error);
                    return;
                }

                if (!_printerService.IsPrinterAvailable)
                {
                    _notificationService.ShowMessage("Printer not available, try again", NotificationType.Error);
                    return;

                }

                _printerService.PrintBarcode(BarCode, PrintCount, Price);

                //for (int i = 0; i < PrintCount; i++)
                //{
                //    _printerService.PrintBarcode(BarCode);
                //}
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in printing barcode command ", _ex);
            }
        }

        private void RefreshPrinters()
        {


            if (ApplicationSettings.APP_ENVIRONMENT != "PRODUCTION")
            {
                //non prod env

                _notificationService.ShowMessage("This option is available in production", NotificationType.Information);
                return;
            }
            _printerService.RefreshPrinter(true);

            //IsPrinterDisConnected = !_printerService.IsPrinterAvailable;

            if (_printerService.IsPrinterAvailable)
            {
                _notificationService.ShowMessage($"Printer {_printerService.GetPrinterName()} connected", Common.NotificationType.Success);
            }
            else
            {
                _notificationService.ShowMessage("No Printers connected,try again", Common.NotificationType.Error);
            }

        }

        public string Title => "Bar Code";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            BarCode = "";
            Price = 0;
        }
        public double Price { get; set; }

        private int _printCount;
        public int PrintCount
        {
            get { return _printCount; }
            set { SetProperty(ref _printCount, value); }
        }

        private string _barcode;

        public string BarCode
        {
            get => _barcode;
            set => SetProperty(ref _barcode, value);
        }



        //private bool _isPrinterDisConnected;
        //public bool IsPrinterDisConnected
        //{
        //    get
        //    {
        //        if (ApplicationSettings.APP_ENVIRONMENT == "PRODUCTION")
        //        {
        //            return !_printerService.IsPrinterAvailable;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    set
        //    {
        //        SetProperty(ref _isPrinterDisConnected, value);
        //    }
        //}



        public async void OnDialogOpened(IDialogParameters parameters)
        {

            var _productCode = parameters.GetValue<int>("productId");

            if (_productCode > 0)
            {
                if (ApplicationSettings.APP_ENVIRONMENT == "PRODUCTION")
                {
                    _printerService.RefreshPrinter();

                    //IsPrinterDisConnected = !_printerService.IsPrinterAvailable;

                    if (!_printerService.IsPrinterAvailable)
                    {
                        _notificationService.ShowMessage("Please connect printer,and try again", Common.NotificationType.Error);

                        BarCode = "";
                        Price = 0;


                    }

                    await Task.Run(async () =>
                    {
                        var _result = await _barCodeService.GetBarCode(_productCode);

                        if (_result != null && _result.IsSuccess && _result.Data.IsValidString())
                        {
                            Application.Current?.Dispatcher?.Invoke(() =>
                            {
                                BarCode = _result.Data;

                                Price = parameters.GetValue<double>("price");
                                //default
                                PrintCount = 1;
                                //_printerService.PrintBarcode(_result.Data);



                            });
                        }

                    });
                }
                else
                {
                    await Task.Run(async () =>
                    {
                        var _result = await _barCodeService.GetBarCode(_productCode);

                        if (_result != null && _result.IsSuccess && _result.Data.IsValidString())
                        {
                            Application.Current?.Dispatcher?.Invoke(() =>
                            {
                                BarCode = _result.Data;
                                PrintCount = 1;
                                Price = parameters.GetValue<double>("price");
                            });
                        }

                    });
                }

            }

        }
    }
}
