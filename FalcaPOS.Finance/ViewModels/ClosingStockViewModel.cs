using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Stock;
using FalcaPOS.Entites.Stores;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.Finance.ViewModels
{
    public class ClosingStockViewModel : BindableBase
    {
        private readonly IClosingStockReportService _closingStockReportService;

        private readonly IStoreService _storeService;

        private readonly Logger _logger;

        private readonly INotificationService _notificationService;

        public DelegateCommand SearchClosingStockCommand { get; private set; }

        public DelegateCommand ExportResultToExcelCommand { get; private set; }

        private readonly ProgressService _progressService;

        private readonly ICommonService _commonService;

        private readonly IEventAggregator _eventAggregator;


        public DelegateCommand RefreshCommand { get; private set; }
        public ClosingStockViewModel(IClosingStockReportService closingStockReportService, IStoreService storeService, Logger logger, INotificationService notificationService, ProgressService progressService, ICommonService commonService, IEventAggregator eventAggregator)
        {
            _closingStockReportService = closingStockReportService ?? throw new ArgumentNullException(nameof(closingStockReportService));

            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            SearchClosingStockCommand = new DelegateCommand(SearchClosingStock);

            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));

            ExportResultToExcelCommand = new DelegateCommand(ExportResultToExcel);

            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _eventAggregator.GetEvent<SignalRStoreAddedEvent>().Subscribe(NewStoreAdded, ThreadOption.PublisherThread);

            LoadStoresAsync();

            RefreshCommand = new DelegateCommand(RefreshGrid);
        }

        private void NewStoreAdded(object str)
        {
            try
            {
                _logger.LogInformation("New Store added in closing stock model");

                if (str is Store _str)
                {
                    Application.Current?.Dispatcher?.Invoke(delegate
                    {

                        if (Stores == null)
                        {
                            _logger.LogInformation("Stores collection is null in closing stock model");

                            Stores = new ObservableCollection<Store> { _str };

                            return;
                        }

                        if (Stores.Any(x => x.Name == _str.Name))
                        {
                            _logger.LogInformation($"Store already added in closing stock model");

                            return;
                        }
                        Stores.Add(_str);
                        Stores = new ObservableCollection<Store>(Stores.OrderBy(x => x.Name));
                    });


                }
            }
            catch (Exception _ex)
            {
                _logger?.LogError("error in adding new  store to list in closing stock model", _ex);
            }

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

            if (ClosingStockDetails == null || !ClosingStockDetails.Any())
            {
                _notificationService.ShowMessage("No records found to export", Common.NotificationType.Error);

                return;
            }

            IsExportEnabled = false;

            if (!_fileName.IsValidString())
            {
                _fileName = "Closing Stock Report";
            }
            _fileName += Guid.NewGuid().ToString().Substring(0, 3);

            bool _result = _commonService.ExportToXL(ClosingStockDetails, _fileName + "Stock", skipfileName: false);

            if (_result)
            {
                _notificationService.ShowMessage("File exported to folder C:\\FALCAPOS\\PosReports", Common.NotificationType.Success);
            }
            else
            {
                _notificationService.ShowMessage("File export failed , try again", Common.NotificationType.Error);
            }

            IsExportEnabled = true;

        }

        private async void SearchClosingStock()
        {
            try
            {
                bool _isValid = ValidateSearch();

                if (!_isValid)
                {
                    return;
                }

                IsExportEnabled = false;

                ClosingStockDetails = null;

                _fileName = GetFileName(FromDate, ToDate, SelectedStore);

                await _progressService.StartProgressAsync();

                await Task.Run(async () =>
                {
                    Response<List<ClosingStockDetails>> _result = await _closingStockReportService.ClosingStocks(new ClosingStockSearch
                    {
                        FromData = FromDate.GetValueOrDefault(),
                        ToDate = ToDate.GetValueOrDefault(),
                        StoreId = SelectedStore.StoreId
                    });
                    Application.Current?.Dispatcher?.Invoke(async () =>
                     {
                         await _progressService.StopProgressAsync();

                         if (_result != null && _result.IsSuccess && _result.Data != null)
                         {
                             ClosingStockDetails = _result.Data;

                             IsExportEnabled = true;

                         }
                         else
                         {
                             _notificationService.ShowMessage(_result?.Error ?? "No records found", Common.NotificationType.Error);
                         }
                     });
                });
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in searching closing stocks", _ex);
            }
            finally {
                await _progressService.StopProgressAsync();
            }

        }

        private string GetFileName(DateTime? fromDate, DateTime? toDate, Store selectedStore)
        {
            string _file = string.Empty;

            if (fromDate != null && toDate != null)
            {
                _file = fromDate?.ToString("dd-MMM-yyyy");

                _file += "-";

                _file += toDate?.ToString("dd-MMM-yyyy");
            }

            if (_selectedStore != null)
            {
                _file += _selectedStore?.Name;
            }

            return _file;

        }

        private bool ValidateSearch()
        {
            if (FromDate == null)
            {
                _notificationService.ShowMessage("Please select from Date", Common.NotificationType.Error);

                return false;

            }
            if (ToDate == null)
            {
                _notificationService.ShowMessage("Please select to Date", Common.NotificationType.Error);

                return false;

            }
            if (FromDate != null && ToDate != null)
            {
                if (ToDate < FromDate)
                {
                    _notificationService.ShowMessage("From date should be less than or equal to To date", Common.NotificationType.Error);
                    return false;
                }
            }
            if (SelectedStore == null)
            {
                _notificationService.ShowMessage("Please select a Store", Common.NotificationType.Error);

                return false;

            }

            return true;
        }

        private List<ClosingStockDetails> _closingStockDetails;
        public List<ClosingStockDetails> ClosingStockDetails
        {
            get => _closingStockDetails;
            set => _ = SetProperty(ref _closingStockDetails, value);
        }
        private async void LoadStoresAsync()
        {
            try
            {
                await Task.Run(async () =>
                {
                    var _result = await _storeService.GetStores("isenabled=true");

                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        if (_result != null && _result.Any())
                        {
                            Stores = new ObservableCollection<Store>(_result.Where(x => x.Parent_ref == null));
                            Stores.Insert(0, new Store { StoreId = 0, Name = "All" });
                        }
                        else
                        {
                            _logger.LogError("Error in getting store in closing stocks or stores are empty");
                        }
                    });

                });
            }
            catch (OperationCanceledException) { }
            catch (Exception _ex)
            {
                _logger.LogError("Error in getting stores in closing stocks", _ex);
            }

        }


        private DateTime? _fromDate;
        public DateTime? FromDate
        {
            get => _fromDate;
            set => _ = SetProperty(ref _fromDate, value);
        }

        private DateTime? _toDate;
        public DateTime? ToDate
        {
            get => _toDate;
            set => _ = SetProperty(ref _toDate, value);
        }


        private Store _selectedStore;
        public Store SelectedStore
        {
            get => _selectedStore;
            set => _ = SetProperty(ref _selectedStore, value);
        }

        private ObservableCollection<Store> _stores;
        public ObservableCollection<Store> Stores
        {
            get => _stores;
            set => _ = SetProperty(ref _stores, value);
        }


        public void RefreshGrid()
        {
            ClosingStockDetails = null;
            IsExportEnabled = false;
            FromDate = null;
            ToDate = null;
            SelectedStore = null;
        }
    }
}
