using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Sku;
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

namespace FalcaPOS.Sku.ViewModels
{
    public class DailyStockReportViewModel : BindableBase
    {
        private readonly ISkuService _skuService;

        public DelegateCommand<object> SaveCommand { get; private set; }
        public DelegateCommand RestCommand { get; private set; }

        private readonly IStoreService _storeService;
        private readonly INotificationService _notificationService;
        private readonly ProgressService _progressService;

        private readonly IEventAggregator _eventAggregator;

        public DelegateCommand<object> CheckMisMatchProduct { get; private set; }

        private readonly Logger _logger;

        List<ProductTypesViewModel> TypeSearchData = new List<ProductTypesViewModel>();
        public DailyStockReportViewModel(
            ISkuService skuService,
            IStoreService storeService,
            INotificationService notificationService,
            ProgressService progressService,
            IEventAggregator eventAggregator,
            Logger logger)
        {

            _skuService = skuService ?? throw new ArgumentNullException(nameof(skuService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));

            SaveCommand = new DelegateCommand<object>((obj) => Search(obj));

            RestCommand = new DelegateCommand(() => Reset());

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _eventAggregator.GetEvent<SignalRStoreAddedEvent>().Subscribe(NewStoreAdded, ThreadOption.PublisherThread);

            LoadStores();

            CheckMisMatchProduct = new DelegateCommand<object>((obj) => CheckMisMatchProductCal(obj));

            IsCountMisMatchVisibility = false;

            DisplayEndDate = AppConstants.CurrentDate;


        }
        private void NewStoreAdded(object str)
        {
            try
            {
                _logger.LogInformation("New Store added  in daialy stock report");

                if (str is Store _str)
                {
                    Application.Current?.Dispatcher?.Invoke(delegate
                    {

                        if (Stores == null)
                        {
                            _logger.LogInformation("Stores collection is null in weekly stock report");

                            Stores = new ObservableCollection<Store> { _str };

                            return;
                        }

                        if (Stores.Any(x => x.StoreId == _str.StoreId))
                        {
                            _logger.LogInformation($"Store already added in in weekly stock report");

                            return;
                        }
                        Stores.Add(_str);
                        Stores = new ObservableCollection<Store>(Stores.OrderBy(x => x.Name));
                    });


                }
            }
            catch (Exception _ex)
            {
                _logger?.LogError("error in adding new  store to list  in daialy stock report", _ex);
            }

        }

        private async void LoadStores()
        {

            try
            {
                await Task.Run(async () =>
                {
                    var _result = await _storeService.GetStores("isenabled=true");

                    Stores = new ObservableCollection<Store>(_result.Where(x => x.Parent_ref == null));

                });
            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, Common.NotificationType.Error);
            }
        }



        public async void Search(object obj)
        {
            try
            {
                if (string.IsNullOrEmpty(SelectedDate))
                {
                    _notificationService.ShowMessage("Please select date", Common.NotificationType.Error);

                    return;
                }

                if (string.IsNullOrEmpty(SelectedStore?.Name))
                {
                    _notificationService.ShowMessage("Please select store name", Common.NotificationType.Error);

                    return;
                }

                DailyStockSearch dailyStockSearch = new DailyStockSearch();
                dailyStockSearch.SelectedDate = SelectedDate;
                dailyStockSearch.StoreId = SelectedStore.StoreId;
                await _progressService.StartProgressAsync();

                await Task.Run(async () =>
                {

                    var _result = await _skuService.GetDailyStockReportServices(dailyStockSearch);

                    if (_result != null && _result.IsSuccess && _result.Data.Any())
                    {
                        Application.Current?.Dispatcher?.Invoke( () =>
                        {
                            
                            List<ProductTypesViewModel> Result = new List<ProductTypesViewModel>();
                            foreach (var item in _result.Data)
                            {
                                ProductTypesViewModel productTypesView = new ProductTypesViewModel();
                                int _totalCount = item.Products.Count;
                                int _firstHalfCount = _totalCount / 2;
                                productTypesView.ProductType = item.ProductType;
                                productTypesView.ProductTypeWithDeptcode = item.DeptCode != null ? item.ProductType.ToUpper() + " (" + item.DeptCode + ")" : item.ProductType.ToUpper();
                                var _firstColumnProducts = item.Products.Take(_firstHalfCount).ToList();
                                var _secondColumnProducts = item.Products.Skip(_firstHalfCount).ToList();
                                
                                productTypesView.FirstColumnProducts = _firstColumnProducts.OrderBy(x => x.Sku).ToList();
                                productTypesView.SecondColumnProducts = _secondColumnProducts.OrderBy(x => x.Sku).ToList();
                                Result.Add(productTypesView);
                            }

                            Type = new ObservableCollection<ProductTypesViewModel>(Result);
                            TypeSearchData = Result;
                            SelectedIndex = 0;
                            IsCountMisMatchVisibility = true;

                        });
                    }
                    else

                    {
                       
                        _notificationService.ShowMessage(_result.Error, Common.NotificationType.Error);
                        if (Type != null && Type.Count > 0)
                        {
                            List<ProductTypesViewModel> Result = new List<ProductTypesViewModel>();
                            Type = new ObservableCollection<ProductTypesViewModel>(Result);
                            IsCountMisMatchVisibility = false;
                        }


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

        public void Reset()
        {
            SelectedStore = null;
            SelectedDate = null;
            Type = null;
            CountMisMatch = false;
            IsCountMisMatchVisibility = false;
        }

        public void CheckMisMatchProductCal(object obj)
        {
            try
            {

                var mismatch = ((DailyStockReportViewModel)obj).CountMisMatch;
                if (mismatch)
                {
                    List<ProductTypesViewModel> MismatchList = new List<ProductTypesViewModel>();
                    if (Type != null && Type.Count > 0)
                        foreach (var item in Type)
                        {
                            item.FirstColumnProducts = item.FirstColumnProducts.Where(x => x.Count != x.ServerCount).ToList();
                            item.SecondColumnProducts = item.SecondColumnProducts.Where(x => x.Count != x.ServerCount).ToList();
                           
                            if (item.FirstColumnProducts == null && item.FirstColumnProducts.Count == 0 && item.SecondColumnProducts == null && item.SecondColumnProducts.Count == 0) { }
                                
                            else
                                MismatchList.Add(item);

                        }
                    Type = new ObservableCollection<ProductTypesViewModel>(MismatchList);
                    IsCountMisMatchVisibility = true;
                }
                else
                {
                    Search(obj);
                }

            }
            catch (Exception)
            {

            }
        }

        private ObservableCollection<ProductTypesViewModel> _type = new ObservableCollection<ProductTypesViewModel>();

        public ObservableCollection<ProductTypesViewModel> Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }


        private int _slectedIndex;

        public int SelectedIndex
        {
            get => _slectedIndex;
            set => SetProperty(ref _slectedIndex, value);
        }

        private ObservableCollection<Store> _stores = new ObservableCollection<Store>();

        public ObservableCollection<Store> Stores
        {
            get => _stores;
            set => SetProperty(ref _stores, value);
        }

        private Store _selectedStore;

        public Store SelectedStore
        {
            get => _selectedStore;
            set => SetProperty(ref _selectedStore, value);
        }

        private string _selectedDate;

        public string SelectedDate
        {
            get => _selectedDate;
            set => SetProperty(ref _selectedDate, value);
        }

        private bool _countMisMatch;

        public bool CountMisMatch
        {
            get => _countMisMatch;
            set => SetProperty(ref _countMisMatch, value);
        }

        private bool isCountMisMatchVisibility;

        public bool IsCountMisMatchVisibility
        {
            get => isCountMisMatchVisibility;
            set => SetProperty(ref isCountMisMatchVisibility, value);
        }

        private string _displayEndDate;

        public string DisplayEndDate
        {
            get => _displayEndDate;
            set => SetProperty(ref _displayEndDate, value);
        }

    }
}
