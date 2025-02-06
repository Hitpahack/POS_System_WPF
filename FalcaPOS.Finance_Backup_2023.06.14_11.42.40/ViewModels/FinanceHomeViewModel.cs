using FalcaPOS.Common;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Finance;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;


namespace FalcaPOS.Finance.ViewModels
{
    public class FinanceHomeViewModel : BindableBase
    {

        private readonly IFinanceService _financeService;

        private readonly Logger _logger;

        private CancellationTokenSource _cancellationTokenSource;

        public DelegateCommand SearchFinanceCommand { get; private set; }
        public DelegateCommand RefreshFinanceCommand { get; private set; }

        public DelegateCommand<FinanceSale> GetInvoicePDFCommand { get; private set; }

        public DelegateCommand ExportToXLCommand { get; private set; }

        private readonly IEventAggregator _eventAggregator;

        private readonly IStoreService _storeService;

        private readonly INotificationService _notificationService;

        private readonly ICommonService _commonService;

        private readonly ProgressService _progressService;

        private readonly ISalesService _salesService;

        public FinanceHomeViewModel(IFinanceService financeService, Logger logger, IEventAggregator eventAggregator, IStoreService storeService, INotificationService notificationService, ICommonService commonService, ProgressService progressService, ISalesService salesService)
        {

            _financeService = financeService ?? throw new ArgumentNullException(nameof(financeService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));


            RefreshFinanceCommand = new DelegateCommand(RefreshFinanceSales);

            SearchFinanceCommand = new DelegateCommand(SearchFinance);

            ExportToXLCommand = new DelegateCommand(ExportToXL);

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _eventAggregator.GetEvent<SearchFinanceEvent>().Subscribe(SearchFinanceFromFlyout);

            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));

            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));

            _salesService = salesService ?? throw new ArgumentNullException(nameof(salesService));



            GetInvoicePDFCommand = new DelegateCommand<FinanceSale>(GetInvoicePDF);



            //Default load todays sales
            LoadFinanceSalesAsync(new FinanceSearch { FromDate = DateTime.Today });

        }



        private void SearchFinance()
        {
            _eventAggregator.GetEvent<SearchFinanceFlyoutEvent>().Publish();
        }



        private bool _isDownloadEnabled;
        public bool IsDownloadEnabled
        {
            get { return _isDownloadEnabled; }
            set { SetProperty(ref _isDownloadEnabled, value); }
        }


        private void SearchFinanceFromFlyout(object search)
        {
            if (search is FinanceSearch _search)
            {

                if (_search.StoreId != null && _search.StoreId == 0)
                {
                    //for all stores store id is null
                    _search.StoreId = null;
                }

                LoadFinanceSalesAsync(_search, true);

                FinSearchStoreName = _search?.StoreName;

            }
        }

        public string FinSearchStoreName { get; set; }

        private bool IsExportInProgess { get; set; }

        private void ExportToXL()
        {

            if (SalesItems == null || SalesItems.Count <= 0)
            {
                //empty collection 

                _notificationService.ShowMessage("No data to export to XL", NotificationType.Error);

                return;
            }

            if (IsExportInProgess)
            {
                _notificationService.ShowMessage("Data export already in progress.. Please wait.", NotificationType.Information);
                return;
            }
            try
            {

                IsExportInProgess = true;

                IsDownloadEnabled = false;

                bool _res = false;

                if (SalesSummery == null || SalesSummery.Count <= 0)
                {
                    //no summery
                    _res = _commonService.ExportToXL<FinanceSale>(SalesItems?.ToList(), FinSearchStoreName);
                }
                else
                {
                    //sales and summery.

                    _res = _commonService.ExportToXL(SalesItems?.ToList(), SalesSummery?.ToList(), "Sales Report", "Summary Report", FinSearchStoreName);
                }
                //bool _res = _commonService.ExportToXL<FinanceSale>(SalesItems?.ToList(), FinSearchStoreName);
                if (_res)
                {
                    _notificationService.ShowMessage($"Data exported to folder \n {ApplicationSettings.REPORTS_PATH}", NotificationType.Success);
                }
                else
                {
                    _notificationService.ShowMessage("Data export failed, try again", NotificationType.Error);
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in data export", _ex);
            }

            IsExportInProgess = false;

            if (SalesItems != null && SalesItems.Count > 0)
            {
                IsDownloadEnabled = true;
            }
        }

        private void RefreshFinanceSales()
        {

            LoadFinanceSalesAsync(new FinanceSearch { FromDate = DateTime.Today }, true);
        }



        private ObservableCollection<FinanceSale> _salesItems;
        public ObservableCollection<FinanceSale> SalesItems
        {
            get { return _salesItems; }
            set { SetProperty(ref _salesItems, value); }
        }

        public IList<FinanceSalesSummery> SalesSummery { get; set; }


        private async void LoadFinanceSalesAsync(FinanceSearch search = null, bool isManual = false)
        {
            try
            {


                SalesItems?.Clear();
                SalesSummery?.Clear();

                IsDownloadEnabled = false;

                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                }
                _cancellationTokenSource = new CancellationTokenSource();

                if (isManual)
                {
                    await _progressService.StartProgressAsync();

                }

                await Task.Run(async () =>
                {
                    var _result = await _financeService.GetFinanceInvoices(search, _cancellationTokenSource.Token);


                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        if (_result != null && _result.IsSuccess && _result.Data != null)
                        {
                            if (_result.Data.Sales != null && _result.Data.Sales.Count() > 0)
                            {
                                SalesItems = new ObservableCollection<FinanceSale>(_result.Data.Sales);
                            }
                            if (_result.Data.Summery != null && _result.Data.Summery.Count() > 0)
                            {
                                SalesSummery = _result.Data.Summery.ToList();
                            }
                            IsDownloadEnabled = true;
                        }
                        else
                        {
                            if (isManual)
                            {
                                if (_result.Data != null && _result.Data.Sales != null && _result.Data.Sales.Count() == 0)
                                {
                                    _notificationService.ShowMessage("No Records Found ", NotificationType.Information);
                                }
                            }
                        }

                    });


                }, _cancellationTokenSource.Token);


            }
            catch (OperationCanceledException)
            {
                _logger.LogError("Task was cancelled by user in get finance sales");
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in loading finance sales ", _ex);
            }
            finally
            {
                if (isManual)
                {
                    await _progressService.StopProgressAsync();
                }
                RaisePropertyChanged(nameof(SalesItems));
            }

        }

        private async void GetInvoicePDF(FinanceSale salesObj)
        {
            if (salesObj != null)
            {
                await _progressService.StartProgressAsync();

                await Task.Run(async () =>
                {
                    var result = await _salesService.GetSaleInvoicePDF(salesObj.InvNo);

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
                            catch (UnauthorizedAccessException _Ex)
                            {

                                //fall back to save manually if file creation is blocked.
                                SaveFileManually(result.Data.FileStream);
                            }
                            catch (Exception _ex)
                            {
                                _notificationService.ShowMessage("Ann error occred while showing invoice pdf", NotificationType.Error);

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
            catch (Exception)
            {


            }
        }


    }
}
