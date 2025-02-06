using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Director;
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

namespace FalcaPOS.Director.ViewModel
{
    public class StoreAsserViewModel : BindableBase
    {
        public DelegateCommand SerachFlyout { get; private set; }
        public DelegateCommand RefreshDataGrid { get; private set; }
        private readonly IDirectorService _directorService;

        private readonly INotificationService _notificationService;
        private readonly IStoreService _storeService;
        private readonly ProgressService _ProgressService;
        private readonly IEventAggregator _eventAggregator;

        private readonly Logger _logger;
        public StoreAsserViewModel(IDirectorService directorService, IStoreService storeService, INotificationService notificationService, ProgressService ProgressService, Logger logger, IEventAggregator eventAggregator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _directorService = directorService ?? throw new ArgumentNullException(nameof(directorService));

            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _ProgressService = ProgressService;

            SerachFlyout = new DelegateCommand(LoadData);

            RefreshDataGrid = new DelegateCommand(RefreshGrid);

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _eventAggregator.GetEvent<SignalRStoreAddedEvent>().Subscribe(NewStoreAdded, ThreadOption.PublisherThread);

            LoadStores();
        }
        private void NewStoreAdded(object str)
        {
            try
            {
                _logger.LogInformation("New Store added  from store assert");

                if (str is Store _str)
                {
                    Application.Current?.Dispatcher?.Invoke(delegate
                    {

                        if (Stores == null)
                        {
                            _logger.LogInformation("Stores collection is null in store assert");

                            Stores = new ObservableCollection<Store> { _str };

                            return;
                        }

                        if (Stores.Any(x => x.Name == _str.Name))
                        {
                            _logger.LogInformation($"Store already added in store assert");

                            return;
                        }
                        Stores.Add(_str);
                        Stores = new ObservableCollection<Store>(Stores.OrderBy(x => x.Name));
                    });


                }
            }
            catch (Exception _ex)
            {
                _logger?.LogError("error in adding new  store to list in store assert", _ex);
            }

        }

        public void RefreshGrid()
        {
            Model = null;
            assertTypeModels1 = null;
            assertTypeModels2 = null;
            assertTypeModels3 = null;
            SelectedStore = null;
        }

        private async void LoadStores()
        {

            try
            {
                await Task.Run(async () =>
                {
                    var _result = await _storeService.GetStores();
                    if (_result != null)
                        Stores = new ObservableCollection<Store>(_result.Where(x => x.Parent_ref == null));
                });
            }
            catch (Exception ex)
            {

            }

        }
        public async void LoadData()
        {

            try
            {

                if (SelectedStore == null)
                {
                    _notificationService.ShowMessage("Please Select Store", Common.NotificationType.Error);

                    return;
                }

                PurchaseRateSearchModel vm = new PurchaseRateSearchModel();
                vm.StoreName = SelectedStore.Name;
                vm.StoreId = SelectedStore.StoreId;

                await _ProgressService.StartProgressAsync();

                await Task.Run(async () =>
                {

                    var _result = await _directorService.GetStoreAssert(vm);

                    if (_result != null && _result.IsSuccess)
                    {
                        Application.Current?.Dispatcher?.Invoke(async () =>
                        {

                            if (_result != null && _result.IsSuccess && _result.Data != null)
                            {
                                Model = _result.Data;
                                int totalcount = Model.assertTypeModels.Count;
                                int firsthalfcount = totalcount / 3;
                                var productlist1 = Model.assertTypeModels.Take(firsthalfcount).ToList();
                                var productlist = Model.assertTypeModels.Skip(firsthalfcount).ToList();
                                int secondhalfcount = productlist.Count / 2;
                                var productlist2 = productlist.Take(secondhalfcount).ToList();
                                var productlist3 = productlist.Skip(secondhalfcount).ToList();
                                assertTypeModels1 = productlist3;
                                assertTypeModels2 = productlist2;
                                assertTypeModels3 = productlist1;

                                Model.Net = ConvertNumbertoWords(Convert.ToInt64(_result.Data.NetWorth));
                                await _ProgressService.StopProgressAsync();
                                IsVisibleRank = Model.NetWorth == 0 ? false : true;

                            }
                            else
                            {
                                await _ProgressService.StopProgressAsync();
                                _notificationService.ShowMessage(_result?.Error ?? "No records found", Common.NotificationType.Error);
                            }


                        });
                    }
                    else

                    {

                        _notificationService.ShowMessage(_result?.Error ?? "No records found", Common.NotificationType.Error);
                        await _ProgressService.StopProgressAsync();

                    }


                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in load data", ex);
            }

        }
        public string ConvertNumbertoWords(long number)
        {
            double val = Math.Abs(number);
            string word = "";
            if (val >= 10000000)
            {

                val = Convert.ToDouble(String.Format("{0:0.00}", ((double)val / (double)10000000)));

                word = Convert.ToString(val) + " Cr";
            }
            else if (val >= 100000)
            {

                val = Convert.ToDouble(String.Format("{0:0.00}", ((double)val / (double)100000)));

                word = Convert.ToString(val) + " Lac";
            }
            else if (val >= 10000)
            {

                val = Convert.ToDouble(String.Format("{0:0.00}", ((double)val / (double)100000)));

                word = Convert.ToString(val) + " Lac";
            }
            else
            {
                word = Convert.ToString(val);
            }

            return word;
        }

        private StoreAssertModel _model;
        public StoreAssertModel Model
        {
            get { return _model; }
            set { SetProperty(ref _model, value); }
        }

        private List<AssertTypeModel> _assertTypeModel1;
        public List<AssertTypeModel> assertTypeModels1
        {
            get { return _assertTypeModel1; }
            set { SetProperty(ref _assertTypeModel1, value); }
        }


        private List<AssertTypeModel> _assertTypeModel2;
        public List<AssertTypeModel> assertTypeModels2
        {
            get { return _assertTypeModel2; }
            set { SetProperty(ref _assertTypeModel2, value); }
        }
        private List<AssertTypeModel> _assertTypeModel3;
        public List<AssertTypeModel> assertTypeModels3
        {
            get { return _assertTypeModel3; }
            set { SetProperty(ref _assertTypeModel3, value); }
        }


        private ObservableCollection<Store> _stores;
        public ObservableCollection<Store> Stores
        {
            get { return _stores; }
            set { SetProperty(ref _stores, value); }
        }

        private Store _selectedStore;
        public Store SelectedStore
        {
            get { return _selectedStore; }
            set { SetProperty(ref _selectedStore, value); }
        }

        private bool isVisibleRank;

        public bool IsVisibleRank
        {
            get { return isVisibleRank; }
            set { SetProperty(ref isVisibleRank, value); }
        }


    }


}
