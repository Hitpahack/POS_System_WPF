using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using System;
using System.Management;
using System.Runtime.InteropServices;

namespace FalcaPOS.ServiceLibrary.Common.Services
{
    public class PrinterService : BasePrinter, IPrinterService
    {


        private readonly INotificationService _notificationService;

        private readonly Logger _logger;

        //private readonly string[] _printerTypes;

        /// <summary>
        /// Seend Commands to printer 
        /// </summary>
        /// <param name="printercommand"></param>
        /// <returns></returns>

        [DllImport("TSCLIB.dll", EntryPoint = "sendcommand")]
        public static extern int SendCommandToPrinter(string printercommand);

        /// <summary>
        /// Get the printer status,
        /// </summary>
        /// <returns></returns>
        /// 0 = standby
        /// 1 = Impact head
        /// 2 = jam
        /// 3
        /// 4 = lack of paper
        /// 10 = Pause
        /// 20 = Print
        [DllImport("TSCLIB.dll", EntryPoint = "usbportqueryprinter")]
        public static extern int USBportQueryPrinter();


        [DllImport("TSCLIB.dll", EntryPoint = "openport")]
        public static extern int OpenPort(string printername);



        [DllImport("TSCLIB.dll", EntryPoint = "closeport")]
        public static extern int ClosePort();




        [DllImport("TSCLIB.dll", EntryPoint = "clearbuffer")]
        public static extern int ClearBuffer();


        public bool IsPrinterConnected { get; set; }


        public string ConnetedPrinterType { get; set; }


        public PrinterService(INotificationService notificationService, Logger logger)
        {

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));


            DetectPrinterConnected();

        }

        private void DetectPrinterConnected(bool _ismanualRefresh = false)
        {

            IsPrinterConnected = false;
            ConnetedPrinterType = null;


            if (ApplicationSettings.APP_ENVIRONMENT != "PRODUCTION")
            {
                //dont check for test env
                return;
            }


            //check for zenpert
            var _zenpertResult = PrinterTypeConnected(AppConstants.ZENPERT_PRINTER);//_printerTypes[0]);

            if (_zenpertResult.Item1)
            {
                //zenpert is connected
                IsPrinterConnected = true;
                ConnetedPrinterType = AppConstants.ZENPERT_PRINTER;// _printerTypes[0];
                _logger.LogInformation($"Printer {AppConstants.ZENPERT_PRINTER} connected");
                if (_ismanualRefresh)
                {
                    _notificationService.ShowMessage($"Barcode printer connected", FalcaPOS.Common.NotificationType.Success);

                    return;
                }


            }
            if (_ismanualRefresh)
            {
                _notificationService.ShowMessage($"No barcode printer connected", FalcaPOS.Common.NotificationType.Error);
            }



        }

        // public IPrinter SelectedPrinter { get; set; }

        public bool IsPrinterAvailable
        {
            get => IsPrinterConnected;  //SelectedPrinter != null;
        }
        //List<ILabelObject> _labelObjects;

        //public List<ILabelObject> LabelObjects
        //{
        //    get
        //    {
        //        if (_labelObjects == null)
        //            _labelObjects = new List<ILabelObject>();
        //        return _labelObjects;
        //    }
        //    set
        //    {
        //        _labelObjects = value;
        //    }
        //}


        //public void GetPrinter()
        //{
        //    SelectedPrinter = DymoPrinter.Instance.GetPrinters().Where(x => x.IsConnected).FirstOrDefault();

        //    //return SelectedPrinter;

        //}




        public void PrintBarcode(string barocode, int barCodeCount, double price)
        {

            if (barocode.IsValidString())
            {
                _logger.LogInformation($"Printer bar code {barocode}");

                if (IsPrinterAvailable)
                {

                    if (ConnetedPrinterType.IsValidString() && ConnetedPrinterType == AppConstants.ZENPERT_PRINTER)
                    {
                        CheckZenpertStatusPrint(barocode, barCodeCount, price);
                    }
                    else
                    {
                        //unknown printer;
                        _notificationService.ShowMessage("Not a valid barcode label printer", NotificationType.Error);

                    }
                }
                else
                {
                    _notificationService.ShowMessage("Printer not available", FalcaPOS.Common.NotificationType.Error);
                }
            }
            else
            {
                _notificationService.ShowMessage("Invalid barcode", FalcaPOS.Common.NotificationType.Error);
            }
        }

        private void CheckZenpertStatusPrint(string barocode, int barCodeCount, double price)
        {
            try
            {
                int _printerStatus = GenZenPertPrinterStatus();
                switch (_printerStatus)
                {
                    case 0:
                        PrintZenpertBarCode(barocode, barCodeCount, price);
                        break;
                    case 1:
                        _notificationService.ShowMessage("Printer head is impacted", NotificationType.Error);
                        break;

                    case 2:
                        _notificationService.ShowMessage("Printer  is jammed", NotificationType.Error);
                        break;
                    case 4:
                        _notificationService.ShowMessage("Lack of labels in printer", NotificationType.Error);
                        break;
                    case -1:
                        _notificationService.ShowMessage("Please connect the printer and try again", NotificationType.Error);
                        break;
                    default:
                        _notificationService.ShowMessage("Printer is out of order, try again", NotificationType.Error);
                        break;
                }
            }
            catch (Exception _Ex)
            {
                _logger.LogError("Error in checking barcode printer status", _Ex);
            }
        }

        private void PrintZenpertBarCode(string _barcode, int barCodeCount, double price)
        {
            try
            {
                //fall back label to Falca if no location
                string title = "FALCA";

                if (AppConstants.UserStoreLocation.IsValidString())
                {
                    title = $"FALCA-{AppConstants.UserStoreLocation.ToUpper()}";
                }

                var _currency = $"Rs.{price}";



                OpenPort(AppConstants.ZENPERT_PRINTER_NAME);
                ClearBuffer();
                SendCommand("SET PEEL OFF");
                SendCommand("SET CUTTER OFF");
                SendCommand("SET PARTIAL_CUTTER OFF");
                SendCommand("SET TEAR ON");
                SendCommand("DIRECTION 1");
                SendCommand("SIZE 100.00 mm,25.00 mm");
                SendCommand("GAP 3 mm,0 mm");
                SendCommand("OFFSET 0.00 mm");
                SendCommand("REFERENCE 0,0");
                SendCommand("CLS");


                //include Falca Label on top
                #region FalcaLabel
                //SendCommand($"TEXT 140,30,\"3\",0,1,1,\"{title}\"");
                //SendCommand($"BARCODE 45,58,\"128M\",72,0,0,2,6,\"{_barcode}\"");
                //SendCommand($"TEXT 97,138,\"3\",0,1,1,\"{_barcode}\"");



                //SendCommand($"TEXT 545,30,\"3\",0,1,1,\"{title}\"");
                //SendCommand($"BARCODE 450,58,\"128M\",72,0,0,2,6,\"{_barcode}\"");
                //SendCommand($"TEXT 502,138,\"3\",0,1,1,\"{_barcode}\""); 
                #endregion


                //include Falca label and store location 

                #region FalcaLabel & Store Location

                //this is two split barcode 
                //first half

                //Falca-Store name title 
                SendCommand($"TEXT 100,20,\"3\",0,1,1,\"{title}\"");

                //barcode BW lines
                SendCommand($"BARCODE 45,48,\"128M\",72,0,0,2,6,\"{_barcode}\"");

                //barcode text value
                SendCommand($"TEXT 97,128,\"3\",0,1,1,\"{_barcode}\"");

                //Product Rs format
                SendCommand($"TEXT 127,158,\"3\",0,1,1,\"{_currency}\"");


                //2nd Half of the barcode
                //Falca-store title
                SendCommand($"TEXT 505,20,\"3\",0,1,1,\"{title}\"");

                //barcode value BW line
                SendCommand($"BARCODE 450,48,\"128M\",72,0,0,2,6,\"{_barcode}\"");

                //barcode text value
                SendCommand($"TEXT 502,128,\"3\",0,1,1,\"{_barcode}\"");
                //product price
                SendCommand($"TEXT 532,158,\"3\",0,1,1,\"{_currency}\"");



                #endregion


                #region Old Code Before Includeing Falca Label on top

                //SendCommand($"BARCODE 45,58,\"128M\",72,0,0,2,6,\"{_barcode}\"");
                //SendCommand($"TEXT 97,138,\"3\",0,1,1,\"{_barcode}\"");
                //SendCommand($"BARCODE 450,58,\"128M\",72,0,0,2,6,\"{_barcode}\"");
                //SendCommand($"TEXT 502,138,\"3\",0,1,1,\"{_barcode}\"");

                #endregion

                SendCommand($"PRINT 1,{barCodeCount}");

                ClosePort();

                _notificationService.ShowMessage("Barcode printed", NotificationType.Success);
            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in barcode printing", _ex);
            }
        }
        public int SendCommand(string command)
        {
            try
            {
                return SendCommandToPrinter(command);
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in sending command to printer", _ex);
            }
            return -1;
        }

        public void RefreshPrinter(bool _isManual = false)
        {
            DetectPrinterConnected(_isManual);
        }

        public string GetPrinterName()
        {
            return ConnetedPrinterType;  //SelectedPrinter?.Name;
        }

        private int GenZenPertPrinterStatus()
        {
            try
            {
                OpenPort(AppConstants.ZENPERT_PRINTER_NAME);
                int _status = USBportQueryPrinter();
                ClosePort();
                return _status;
            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in getting barcode printer status ", _ex);
            }

            return -2;
        }
    }
}

public abstract class BasePrinter
{
    //public abstract string PrinterType { get; set; }
    protected virtual Tuple<bool, string> PrinterTypeConnected(string PrinterType)
    {
        try
        {
            // Set management scope
            ManagementScope scope = new ManagementScope(@"\root\cimv2");
            scope.Connect();

            // Select Printers from WMI Object Collections
            ManagementObjectSearcher searcher = new
             ManagementObjectSearcher("SELECT * FROM Win32_Printer");

            string printerName = "";
            foreach (ManagementObject printer in searcher.Get())
            {

                printerName = printer["Name"]?.ToString()?.ToLower();
                if (printerName.IsValidString() && printerName.Contains(PrinterType))
                {
                    if (printer["WorkOffline"].ToString().ToLower().Equals("true"))
                    {
                    }
                    else
                    {
                        // printer is online                 

                        return new Tuple<bool, string>(true, PrinterType);
                    }
                    break;
                }
            }

        }
        catch (Exception _ex)
        {
        }

        return new Tuple<bool, string>(false, string.Empty);

    }

}

