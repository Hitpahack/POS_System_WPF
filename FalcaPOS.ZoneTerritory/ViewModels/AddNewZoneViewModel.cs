using FalcaPOS.Common;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Indent;
using FalcaPOS.Entites.Location;
using FalcaPOS.Entites.User;
using FalcaPOS.Entites.Zone;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace FalcaPOS.ZoneTerritory.ViewModels
{
    public class AddNewZoneViewModel : BindableBase
    {

        private readonly Logger _logger;
        private readonly ProgressService _progressService;
        private readonly ICommonService _locationService;
        private readonly INotificationService _notificationService;
        private readonly IZoneService _zoneService;

        public DelegateCommand<Object> AddZoneCommand { get; private set; }
        public DelegateCommand ResetCommand { get; private set; }
        public AddNewZoneViewModel(Logger logger, ProgressService progressService, ICommonService commonService, INotificationService notificationService, IZoneService zoneService)
        {
            _logger = logger;
            _progressService = progressService;
            _locationService = commonService;
            _notificationService = notificationService;
            _zoneService = zoneService;
            AddZoneCommand = new DelegateCommand<Object>(AddZoneCommandEvent);
            ResetCommand = new DelegateCommand(ResetCommandEvent);
            LoadStatesAsync();
            LoadRegionalManagerAsync();
        }

        private async void AddZoneCommandEvent(Object obj)
        {
            try
            {
                if (String.IsNullOrEmpty(ZoneName))
                {
                    _notificationService.ShowMessage("Please enter zone", NotificationType.Error);
                    return;
                }
                if (SelectedState == null)
                {
                    _notificationService.ShowMessage("Please select state", NotificationType.Error);
                    return;
                }
                if (SelectedRM == null)
                {
                    _notificationService.ShowMessage("Please select Regional Manager", NotificationType.Error);
                    return;
                }

                if (obj != null)
                    ClosePopup(obj);
                await _progressService.StartProgressAsync();
              
                var _result = await _zoneService.CreateZone(new Entites.Zone.NewZone()
                {
                    Name = ZoneName,
                    StateName = SelectedState.Name,
                    StateId = SelectedState.StateId,
                    ZonalmanagerRef = SelectedRM.UserId
                }, new System.Threading.CancellationToken());

                if (_result.IsSuccess)
                {
                    _notificationService.ShowMessage("New zone created successfully", NotificationType.Success);
                    ResetCommandEvent();
                   
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
        private void ResetCommandEvent()
        {
            ZoneName =String.Empty;
            SelectedState = null;
            SelectedRM = null;
        }

        private void ClosePopup(object obj)
        {
            var TargetClose = ((Button)(obj));
            var dynamicCommand = TargetClose.Command;
            dynamicCommand.CanExecute(true);
            dynamicCommand.Execute(this);
        }


        private String _zonename;

        public String ZoneName
        {
            get { return _zonename; }
            set { SetProperty(ref _zonename, value); }
        }


        private State _selecctedState;

        public State SelectedState
        {
            get { return _selecctedState; }
            set { SetProperty(ref _selecctedState, value); }
        }


        private ObservableCollection<State> _states;

        public ObservableCollection<State> StatesList
        {
            get { return _states; }
            set { _ = SetProperty(ref _states, value); }
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
        private async void LoadStatesAsync()
        {
            try
            {
                _logger.LogInformation("Getting all states information");

                await Task.Run(async () =>
                {
                    var _result = await _locationService.GetStates();

                    if (_result != null && _result.Any())
                    {
                        Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            StatesList = new ObservableCollection<State>(_result);
                        });
                    }
                });
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in loading all states list view model", _ex);
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


    }
}
