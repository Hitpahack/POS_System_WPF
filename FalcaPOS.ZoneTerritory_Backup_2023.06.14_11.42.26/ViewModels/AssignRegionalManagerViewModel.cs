using FalcaPOS.Common;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.User;
using FalcaPOS.Entites.Zone;
using Prism.Commands;
using Prism.Common;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Telerik.Windows.Data;

namespace FalcaPOS.ZoneTerritory.ViewModels
{
    public class AssignRegionalManagerViewModel :BindableBase
    {

        private readonly IEventAggregator _eventAggregator;
        private readonly Logger _logger;
        private readonly ProgressService _progressService;
        private readonly ICommonService _locationService;
        private readonly INotificationService _notificationService;
        private readonly IZoneService _zoneService;

        public DelegateCommand<Object> AddRegionalManagerCommand { get; private set; }
        public DelegateCommand AddRegionalManagerResetCommand { get; private set; }
        

        public AssignRegionalManagerViewModel(IEventAggregator eventAggregator, Logger logger, ProgressService progressService, ICommonService commonService, INotificationService notificationService, IZoneService zoneService)
        {
            _eventAggregator = eventAggregator;
            _logger = logger;
            _progressService = progressService;
            _locationService = commonService;
            _notificationService = notificationService;
            _zoneService = zoneService;
            LoadRegionalManagerAsync();
            AddRegionalManagerCommand = new DelegateCommand<Object>(AddRegionalManagerCommandEvent);
            AddRegionalManagerResetCommand = new DelegateCommand(() => { SelectedRM = null; });
        }

        private async void AddRegionalManagerCommandEvent(Object payload)
        {
            try
            {
                if (SelectedRM == null)
                {
                    _notificationService.ShowMessage("Please select RM", NotificationType.Error);
                    return;
                }
                if (payload != null)
                {
                    var _payloadStoreID = (payload) as Tuple<Object,Object>;
                    if (_payloadStoreID != null)
                    {
                        var _storeId = (int)_payloadStoreID.Item2;
                        if (_storeId > 0)
                        {
                            await _progressService.StartProgressAsync();
                            var _result = await _zoneService.AssignRegionalManagerToStore(_storeId, SelectedRM.UserId, new System.Threading.CancellationToken());
                            if (_result != null && _result.IsSuccess)
                            {
                                _notificationService.ShowMessage("Regional manager assigned successfully", NotificationType.Success);
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

        private async void LoadRegionalManagerAsync()
        {
            try
            {
                var _result = await _zoneService.GetRegionalMangersList();
                if (_result != null && _result.IsSuccess)
                {
                    RegionalManagerList = new ObservableCollection<User>(_result.Data);
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }


        private User _selectedRM;

        public User SelectedRM
        {
            get { return _selectedRM; }
            set { SetProperty(ref _selectedRM, value); }
        }
        private ObservableCollection<User> _regionalManagerList;

        public ObservableCollection<User> RegionalManagerList
        {
            get { return _regionalManagerList; }
            set { SetProperty(ref _regionalManagerList, value); }
        }

    }
}
