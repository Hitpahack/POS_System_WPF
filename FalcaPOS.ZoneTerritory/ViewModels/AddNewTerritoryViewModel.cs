using FalcaPOS.Common;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Location;
using FalcaPOS.Entites.User;
using FalcaPOS.Entites.Zone;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace FalcaPOS.ZoneTerritory.ViewModels
{
    public class AddNewTerritoryViewModel : BindableBase
    {
        private readonly Logger _logger;
        private readonly ProgressService _progressService;
        private readonly ICommonService _locationService;
        private readonly INotificationService _notificationService;
        private readonly IZoneService _zoneService;

        public DelegateCommand<Object> AddTerritoryCommand { get; private set; }
        public DelegateCommand ResetTerritoryCommand { get; private set; }
        public AddNewTerritoryViewModel(Logger logger, ProgressService progressService, ICommonService commonService, INotificationService notificationService, IZoneService zoneService)
        {
            _logger = logger;
            _progressService = progressService;
            _locationService = commonService;
            _notificationService = notificationService;
            _zoneService = zoneService;
            AddTerritoryCommand = new DelegateCommand<Object>(AddTerritoryCommandEvent);
            ResetTerritoryCommand = new DelegateCommand(ResetTerritoryCommandEvent);
            LoadZoneAsync();
            LoadTerritoryManagerAsync();
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
        private User _selectedTM;
        public User SelectedTM
        {
            get { return _selectedTM; }
            set { SetProperty(ref _selectedTM, value); }
        }
        private ObservableCollection<NewZone> _zones;

        public ObservableCollection<NewZone> ZoneList
        {
            get { return _zones; }
            set { _ = SetProperty(ref _zones, value); }
        }
        private ObservableCollection<User> _territoryManagerList;

        public ObservableCollection<User> TerritoryManagerList
        {
            get { return _territoryManagerList; }
            set { SetProperty(ref _territoryManagerList, value); }
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
            SelectedTM = null;
        }

        private async void AddTerritoryCommandEvent(Object obj)
        {
            try
            {
                // if Territory name or Selected zone or selected TM is null it will show error
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

                if (SelectedTM == null)
                {
                    _notificationService.ShowMessage("Please select TM", NotificationType.Error);
                    return;
                }
                if (obj!=null)
                    ClosePopup(obj);
                await _progressService.StartProgressAsync();

                var _result = await _zoneService.CreateTerritory(new Entites.Zone.Territory()
                {
                    Name = TerritoryName,
                    ZoneName = SelectedZone.Name,
                    ZoneId = SelectedZone.Id,
                    TerritorymanagerRef=SelectedTM.UserId
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

        private void ClosePopup(object obj)
        {
            var TargetClose = ((Button)(obj));
            var dynamicCommand = TargetClose.Command;
            dynamicCommand.CanExecute(true);
            dynamicCommand.Execute(this);
        }

        private async void LoadTerritoryManagerAsync()
        {
            try
            {
                var _result = await _zoneService.GetTerritoryMangersList();
                if (_result != null && _result.IsSuccess)
                {
                    TerritoryManagerList = new ObservableCollection<User>(_result.Data);
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }
    }
}
