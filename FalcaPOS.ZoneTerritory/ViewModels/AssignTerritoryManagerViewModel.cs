using FalcaPOS.Common;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.User;
using FalcaPOS.Entites.Zone;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalcaPOS.ZoneTerritory.ViewModels
{
    public class AssignTerritoryManagerViewModel : BindableBase
    {

        private readonly IEventAggregator _eventAggregator;
        private readonly Logger _logger;
        private readonly ProgressService _progressService;
        private readonly ICommonService _locationService;
        private readonly INotificationService _notificationService;
        private readonly IZoneService _zoneService;

        public DelegateCommand<Object> AddTerritoryManagerCommand { get; private set; }
        public DelegateCommand AddTerritoryManagerResetCommand { get; private set; }


        public AssignTerritoryManagerViewModel(IEventAggregator eventAggregator, Logger logger, ProgressService progressService, ICommonService commonService, INotificationService notificationService, IZoneService zoneService)
        {
            _eventAggregator = eventAggregator;
            _logger = logger;
            _progressService = progressService;
            _locationService = commonService;
            _notificationService = notificationService;
            _zoneService = zoneService;
            LoadTerritoryManagerAsync();
            AddTerritoryManagerCommand = new DelegateCommand<Object>(AddTerritoryManagerCommandEvent);
            AddTerritoryManagerResetCommand = new DelegateCommand(() => { SelectedTM = null; });
        }

        private async void AddTerritoryManagerCommandEvent(Object payload)
        {
            try
            {
                if (_selectedTM == null)
                {
                    _notificationService.ShowMessage("Please select TM", NotificationType.Error);
                    return;
                }
                if (payload != null)
                {
                   // obj is User _user)
                    var _payloadStoreID = (payload) as Tuple<Object, Object>;
                    if (_payloadStoreID != null)
                    {
                        var _territoryId = (int)_payloadStoreID.Item2;
                        if (_territoryId > 0)
                        {
                            await _progressService.StartProgressAsync();
                            var _result = await _zoneService.AssignTerritoryManagerToTerritory(_territoryId, _selectedTM.UserId, new System.Threading.CancellationToken());
                            if (_result != null && _result.IsSuccess)
                            {
                                _notificationService.ShowMessage("Territory manager assigned successfully", NotificationType.Success);
                                _eventAggregator.GetEvent<RefreshTerritoryViewEvent>().Publish();
                                return;
                            }
                            else
                            {
                                _notificationService.ShowMessage(_result.Error, NotificationType.Error);
                                return;
                            }
                        }
                    }

                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
                return;
            }
            finally
            {
                await _progressService.StopProgressAsync();
            }

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


        private User _selectedTM;

        public User SelectedTM
        {
            get { return _selectedTM; }
            set { SetProperty(ref _selectedTM, value); }
        }
        private ObservableCollection<User> _territoryManagerList;

        public ObservableCollection<User> TerritoryManagerList
        {
            get { return _territoryManagerList; }
            set { SetProperty(ref _territoryManagerList, value); }
        }
    }

}
