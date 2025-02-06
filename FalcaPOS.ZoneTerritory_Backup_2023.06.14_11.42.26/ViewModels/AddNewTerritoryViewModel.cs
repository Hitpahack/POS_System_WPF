using FalcaPOS.Common;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Location;
using FalcaPOS.Entites.Zone;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;

namespace FalcaPOS.ZoneTerritory.ViewModels
{
    public class AddNewTerritoryViewModel : BindableBase
    {
        private readonly Logger _logger;
        private readonly ProgressService _progressService;
        private readonly ICommonService _locationService;
        private readonly INotificationService _notificationService;
        private readonly IZoneService _zoneService;

        public DelegateCommand AddTerritoryCommand { get; private set; }
        public DelegateCommand ResetTerritoryCommand { get; private set; }
        public AddNewTerritoryViewModel(Logger logger, ProgressService progressService, ICommonService commonService, INotificationService notificationService, IZoneService zoneService)
        {
            _logger = logger;
            _progressService = progressService;
            _locationService = commonService;
            _notificationService = notificationService;
            _zoneService = zoneService;
            AddTerritoryCommand = new DelegateCommand(AddTerritoryCommandEvent);
            ResetTerritoryCommand = new DelegateCommand(ResetTerritoryCommandEvent);

            LoadZoneAsync();
        }

        private String _territoryName;

        public String TerritoryName
        {
            get { return _territoryName; }
            set { SetProperty(ref _territoryName, value); }
        }


        private NewZone _selecctedZone;

        public NewZone SelectedZone
        {
            get { return _selecctedZone; }
            set { SetProperty(ref _selecctedZone, value); }
        }

        private ObservableCollection<NewZone> _zones;

        public ObservableCollection<NewZone> ZoneList
        {
            get { return _zones; }
            set { _ = SetProperty(ref _zones, value); }
        }

        private async void LoadZoneAsync()
        {
            try
            {
                var _result = await _zoneService.GetZone();
                if (_result != null && _result.IsSuccess)
                {
                    ZoneList = new ObservableCollection<NewZone>(_result.Data);

                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private void ResetTerritoryCommandEvent()
        {
            TerritoryName = String.Empty;
            SelectedZone = null;
        }

        private async void AddTerritoryCommandEvent()
        {
            try
            {
                if (String.IsNullOrEmpty(TerritoryName))
                {
                    _notificationService.ShowMessage("Please enter territory", NotificationType.Error);
                    return;
                }
                if (SelectedZone == null)
                {
                    _notificationService.ShowMessage("Please select zone", NotificationType.Error);
                    return;
                }

                await _progressService.StartProgressAsync();

                var _result = await _zoneService.CreateTerritory(new Entites.Zone.Territory()
                {
                    Name = TerritoryName,
                    ZoneName = SelectedZone.Name,
                    ZoneId = SelectedZone.Id

                }, new System.Threading.CancellationToken());

                if (_result.IsSuccess)
                {
                    _notificationService.ShowMessage("New territory created successfully", NotificationType.Success);
                    ResetTerritoryCommandEvent();
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
