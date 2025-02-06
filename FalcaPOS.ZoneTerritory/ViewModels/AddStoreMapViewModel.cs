using FalcaPOS.Common;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Stores;
using FalcaPOS.Entites.Zone;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FalcaPOS.ZoneTerritory.ViewModels
{
    internal class AddStoreMapViewModel : BindableBase
    {
        private readonly Logger _logger;
        private readonly ProgressService _progressService;
        private readonly ICommonService _locationService;
        private readonly INotificationService _notificationService;
        private readonly IZoneService _zoneService;

        public DelegateCommand AddStoreMapCommand { get; private set; }
        public DelegateCommand StoreMapResetCommand { get; private set; }

        public AddStoreMapViewModel(Logger logger, ProgressService progressService, ICommonService commonService, INotificationService notificationService, IZoneService zoneService)
        {
            _logger = logger;
            _progressService = progressService;
            _locationService = commonService;
            _notificationService = notificationService;
            _zoneService = zoneService;
            AddStoreMapCommand = new DelegateCommand(AddStoreMapCommandEvent);
            StoreMapResetCommand = new DelegateCommand(StoreMapResetCommandEvent);

            LoadTerritoryMissingStoreAsync();
            LoadTerriotyZoneAsync();
        }

        private async void LoadTerritoryMissingStoreAsync()
        {
            try
            {
                var _result = await _zoneService.GetStoreMap();
                if (_result != null && _result.IsSuccess)
                {
                    TerritoryMissingStoreList = new ObservableCollection<Store>(_result.Data);

                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private async void LoadTerriotyZoneAsync()
        {
            try
            {
                var _result = await _zoneService.GetTerritory();
                if (_result != null && _result.IsSuccess)
                {
                    ZoneList = new ObservableCollection<Territory>(_result.Data);

                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }    


        private Store _selecctedStore;
        public Store SelecctedStore
        {
            get { return _selecctedStore; }
            set { SetProperty(ref _selecctedStore, value); }
        }

        private ObservableCollection<Store> _stores;

        public ObservableCollection<Store> TerritoryMissingStoreList
        {
            get { return _stores; }
            set { _ = SetProperty(ref _stores, value); }
        }



        private Territory _selecctedZone;
        public Territory SelectedZone
        {
            get { return _selecctedZone; }
            set { SetProperty(ref _selecctedZone, value); }
        }

        private ObservableCollection<Territory> _zones;

        public ObservableCollection<Territory> ZoneList
        {
            get { return _zones; }
            set { _ = SetProperty(ref _zones, value); }
        }


        private void StoreMapResetCommandEvent()
        {
            SelecctedStore = null;
            SelectedZone = null;
        }

        private async void AddStoreMapCommandEvent()
        {
            try
            {

                if (SelecctedStore == null)
                {
                    _notificationService.ShowMessage("Please select store", NotificationType.Error);
                    return;
                }
                if (SelectedZone == null)
                {
                    _notificationService.ShowMessage("Please select zone", NotificationType.Error);
                    return;
                }

                await _progressService.StartProgressAsync();
                var _result = await _zoneService.CreateTerritoryStoreMap(SelecctedStore.StoreId, SelectedZone.Id, new System.Threading.CancellationToken());
                if (_result != null && _result.IsSuccess)
                {
                    _notificationService.ShowMessage("Store successfully mapped to territory", NotificationType.Success);
                    StoreMapResetCommandEvent();
                    return;
                }
                else
                {
                    _notificationService.ShowMessage(_result.Error, NotificationType.Error);
                    return;
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            finally
            {

                await _progressService.StopProgressAsync();
            }
        }
    }
}
