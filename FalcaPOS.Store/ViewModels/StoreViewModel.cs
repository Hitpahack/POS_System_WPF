using FalcaPOS.Common;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Location;
using FalcaPOS.Entites.Stores;
using FalcaPOS.Store.Views;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.Store.ViewModels
{
    public class StoreViewModel : BindableBase
    {

        private ObservableCollection<Entites.Stores.Store> _stores;
        private ObservableCollection<Entites.Stores.StoreLicense> _storelicenses;

        private readonly IStoreService _storeService;

        private readonly ICommonService _commonService;

        private readonly IEventAggregator _eventAggregator;

        private readonly INotificationService _notificationService;

        private readonly Logger _logger;
        private CancellationTokenSource _cancellationTokenSource;
        public DelegateCommand AddStoreCommand { get; private set; }
        public DelegateCommand SearchStoreCommand { get; private set; }
        public DelegateCommand RefreshStoreCommand { get; private set; }
        public DelegateCommand StateSelectionChanged { get; private set; }

        public DelegateCommand<object> EditStoreCommand { get; private set; }

        public StoreViewModel(IStoreService storeService, ICommonService commonService, IEventAggregator eventAggregator, Logger logger,INotificationService notificationService)
        {
            _notificationService= notificationService ?? throw new ArgumentException(nameof(notificationService));
            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _eventAggregator.GetEvent<ReloadStoresEvent>().Subscribe(LoadStores);

            _eventAggregator.GetEvent<SignalRStoreAddedEvent>().Subscribe(NewStoreAdded, ThreadOption.PublisherThread);
            
            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));

            EditStoreCommand = new DelegateCommand<object>((store) => _eventAggregator.GetEvent<EditStoreFlyoutOpenEvent>().Publish(store));

            AddStoreCommand = new DelegateCommand(executeMethod: () => { _eventAggregator.GetEvent<AddStoreFlyoutOpenEvent>().Publish(new Entites.Stores.StoreFlyout() { IsOpen = true, IsParent = true }); });
           
            SearchStoreCommand = new DelegateCommand(SearchStores);            
            
            LoadStates();

            RefreshStoreCommand = new DelegateCommand(LoadStores);

            StateSelectionChanged = new DelegateCommand(LoadDistricts);

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            LoadStores();            

        }

        private void NewStoreAdded(object store)
        {
            try
            {
                if (store is Entites.Stores.Store _str)
                {

                    Application.Current?.Dispatcher?.Invoke(() =>
                    {

                        if (Stores == null)
                        {
                            _logger?.LogInformation("Stores collection is null");
                            Stores = new ObservableCollection<Entites.Stores.Store> { _str };
                            _logger.LogInformation("Store  addded");

                            return;
                        }

                        if (Stores.Any(x => x.Name == _str.Name))
                        {
                            _logger.LogInformation("Store already added");

                            return;
                        }

                        //Stores.Add(_str);

                       // Stores = new ObservableCollection<Entites.Stores.Store>(Stores.OrderBy(x => x.Name));
                       LoadStores();
                    });
                }
            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in adding new store ", _ex);
            }
        }

        public ObservableCollection<Entites.Stores.Store> Stores
        {
            get { return _stores; }
            set { SetProperty(ref _stores, value); }
        }
        public ObservableCollection<Entites.Stores.StoreLicense> StoreLicenses
        {
            get { return _storelicenses; }
            set { SetProperty(ref _storelicenses, value); }
        }
        private async void LoadStores()
        {
            try
            {
                Stores = null;
                SelectedDistrict = null;
                SelectedState = null;
                await Task.Run(async () =>
                {
                    var _result = await _storeService.GetStores();                   

                    if (_result != null && _result.Any())
                    {
                        Application.Current?.Dispatcher.Invoke(() =>
                        {
                            var _stores = new ObservableCollection<Entites.Stores.Store>(_result);

                            List<Entites.Stores.Store> storesList = new List<Entites.Stores.Store>();
                            var _randStoreImages = new Random();
                            foreach (var store in _stores)
                            {
                                if (store.IsParent == true && store.Parent_ref == null)
                                {
                                    store.Name = String.Concat(store.Name.Replace("FALCA RAITHA UNNATI KENDRA", String.Empty), ", ", store.Address.State);
                                    store.ImageName = $"Store{_randStoreImages.Next(1, 8)}";
                                    storesList.Add(store);

                                }
                                else
                                {
                                    Entites.Stores.Store childStore = new Entites.Stores.Store();
                                    childStore.StoreId = store.StoreId;
                                    childStore.Address = store.Address;
                                    childStore.Parent_ref = store.Parent_ref;
                                    childStore.ImageName = $"Store{_randStoreImages.Next(1, 8)}";
                                    childStore.LastSequenceNumber = store.LastSequenceNumber;
                                    childStore.StoreInvoiceFormat = store.StoreInvoiceFormat;
                                    childStore.Name = _stores.Where(x => x.StoreId == store.Parent_ref).FirstOrDefault()?.Name + "-->" + store.Name;
                                    storesList.Add(childStore);

                                }
                            }

                            Stores = new ObservableCollection<Entites.Stores.Store>(storesList);
                        });
                    }                  

                });
            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in loading  store ", _ex);
            }

        }
        private async void LoadStates()
        {
            try
            { 
            var _result = await _commonService.GetStates("?isenabled = true");

            if (_result != null && _result.Any())
                States = new ObservableCollection<State>(_result);
            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in loading state ", _ex);
            }

        }
        private async void LoadDistricts()
        {
            Districts?.Clear();

            SelectedDistrict = null;

            if (SelectedState == null || SelectedState.StateId <= 0) return;

            try
            {

                var _result = await _commonService.GetDistricts(SelectedState.StateId);
                if (_result != null && _result.Any())
                    Districts = new ObservableCollection<District>(_result);

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private void SearchStores()
        {           
            if (SelectedState == null)
            {
            _notificationService.ShowMessage("Please select state", NotificationType.Error);
                return;
            }
            if (SelectedDistrict == null)
            {
                _notificationService.ShowMessage("Please select district", NotificationType.Error);
                return;
            }

            var _searchParm = new StoreSearchParams
            {                
                StateId = SelectedState?.StateId,
                DistrictId=SelectedDistrict?.DistrictId
            };            
            LoadStoresAsync(_searchParm, true);
        }
        private async void LoadStoresAsync(StoreSearchParams searchParams = null, bool IsManualRefresh = false)
        {

            try
            {

                await Task.Run(async () =>
                {
                    var _result = await _storeService.GetStores();

                    if (_result != null && _result.Any())
                    {
                        Application.Current?.Dispatcher.Invoke(() =>
                        {
                            var _randStoreImages = new Random();
                            var _stores = new ObservableCollection<Entites.Stores.Store>(_result);
                            var _results = _stores.Where(s => s.Address.StateID == searchParams.StateId && s.Address.DistrictID == searchParams.DistrictId)
                                         .OrderBy(x => x.Name)
                                         .ToList();
                            
                            List<Entites.Stores.Store> storesList = new List<Entites.Stores.Store>();
                            if (_results.Any())
                            {
                                foreach (var store in _results)
                                {
                                    store.ImageName = $"Store{_randStoreImages.Next(1, 8)}";
                                    storesList.Add(store);
                                }
                            }
                            else
                            {
                                _notificationService.ShowMessage("No records found", NotificationType.Error);
                            }
                            Stores = new ObservableCollection<Entites.Stores.Store>(storesList);
                        });
                    }   
                    });
                }
            catch (OperationCanceledException) { }
            catch (Exception _ex)
            {
                _logger.LogError("Error in getting list of Stores", _ex);
            }

        }
        private ObservableCollection<State> _states;
        public ObservableCollection<State> States
        {
            get { return _states; }
            set { SetProperty(ref _states, value); }

        }
        private ObservableCollection<District> _districts;
        public ObservableCollection<District> Districts
        {
            get { return _districts; }
            set { SetProperty(ref _districts, value); }
        }


        private State _selectedState;
        [Required(ErrorMessage = "Select a State")]
        public State SelectedState
        {
            get { return _selectedState; }
            set
            {
                SetProperty(ref _selectedState, value);             

            }
        }
        private District _selectedDistrict;

        [Required(ErrorMessage = "Select a District")]
        public District SelectedDistrict
        {
            get { return _selectedDistrict; }
            set
            {
                SetProperty(ref _selectedDistrict, value);                
            }
        }    
    }
}
