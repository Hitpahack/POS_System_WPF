using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System.Threading.Tasks;
using System;
using System.Windows;
using System.Collections.ObjectModel;
using FalcaPOS.Entites.Location;
using Prism.Commands;
using FalcaPOS.ZoneTerritory.Views;
using MaterialDesignThemes.Wpf;
using FalcaPOS.Entites.Zone;
using FalcaPOS.Common.Events;
using Prism.Interactivity.InteractionRequest;
using FalcaPOS.Common;
using Prism.Services.Dialogs;
using MaterialDesignExtensions.Controls;

namespace FalcaPOS.ZoneTerritory.ViewModels
{
    public class ZoneTerritoryHomeViewModel:BindableBase
    {

        private readonly IEventAggregator _eventAggregator;      
        private readonly Logger _logger;
        private readonly ProgressService _progressService;
        private readonly ICommonService _locationService;
        private readonly IZoneService _zoneService;
        private readonly INotificationService _notificationService;

        public DelegateCommand AddZoneCommandPopup { get; private set; }
        public DelegateCommand AddTerritoyCommandPopup { get; private set; }
        public DelegateCommand AddStoreMapCommandPopup { get; private set; }
        public DelegateCommand<object> RefreshTerritoryViewCommand { get; private set; }
        public DelegateCommand<int?> EditRegionalManagerCommandPopup { get; private set; }
        public DelegateCommand<Object> DeleteRegionalManagerCommandPopup { get; private set; }
        public DelegateCommand<int?> EditTerritoryManagerCommandPopup { get; private set; }
        public DelegateCommand<Object> DeleteTerritoryManagerCommandPopup { get; private set; }
      

        public ZoneTerritoryHomeViewModel(IEventAggregator eventAggregator, Logger logger, ICommonService locationService, IZoneService zoneService,INotificationService notificationService, ProgressService progressService)
        {
            _eventAggregator = eventAggregator;
            _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _zoneService = zoneService ?? throw new ArgumentNullException(nameof(zoneService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));
            AddZoneCommandPopup = new DelegateCommand(AddZoneCommandPopupEvent);
            AddTerritoyCommandPopup = new DelegateCommand(AddTerritoyCommandPopupEvent);
            AddStoreMapCommandPopup = new DelegateCommand(AddStoreMapCommandPopupEvent);
            RefreshTerritoryViewCommand = new DelegateCommand<object> (RefreshZoneAndTerritory);
            EditRegionalManagerCommandPopup = new DelegateCommand<int?>(EditRegionalManagerCommandPopupEvent);
            DeleteRegionalManagerCommandPopup = new DelegateCommand<Object>(DeleteRegionalManagerCommandPopupEvent);
            EditTerritoryManagerCommandPopup = new DelegateCommand<int?>(EditTerritoryManagerCommandPopupEvent);
            DeleteTerritoryManagerCommandPopup = new DelegateCommand<Object>(DeleteTerritoryManagerCommandPopupEvent);
            
            LoadTerrioryViewAsync();
            _eventAggregator.GetEvent<RefreshTerritoryViewEvent>().Subscribe(() => LoadTerrioryViewAsync());
        }

        private void RefreshZoneAndTerritory(object obj)
        {
            if (obj != null)
            {
                ResetTelerikGridFilters.ClearTelerikGridViewFilters(obj);
            }
            LoadTerrioryViewAsync();
        }

        private async void DeleteTerritoryManagerCommandPopupEvent(Object rowDetails)
        {
            UnAssignUser(rowDetails, "Territory manager");
        }

        private void DeleteRegionalManagerCommandPopupEvent(Object rowDetails)
        {
            UnAssignUser(rowDetails, "Regional manager");
        }

        private async void UnAssignUser(Object rowDetails,String userType)
        {
            try
            {
                if (rowDetails != null)
                {
                    ConfirmationDialogArguments dialogArgs = new ConfirmationDialogArguments
                    {
                        Title = "Confirmation",
                        Message = $"Do you really want to unassigns the {userType} ",
                        OkButtonLabel = "YES",
                        CancelButtonLabel = "CANCEL",
                    };

                    bool result = await ConfirmationDialog.ShowDialogAsync("RootDialog", dialogArgs);
                    if (result)
                    {
                        var _currentRow = (ZoneTerritoryView)rowDetails;
                        if(userType == "Territory manager")
                        {
                            await _progressService.StartProgressAsync();
                            var _result = await _zoneService.UnassignsUserFromTerritory(_currentRow.TerritoryID, _currentRow.TerritoryManagerUserId, new System.Threading.CancellationToken());
                            if (_result != null && _result.IsSuccess)
                            {
                                _notificationService.ShowMessage($"{userType} unassigned from {_currentRow.TerritoryManager}", NotificationType.Success);
                                LoadTerrioryViewAsync();
                                return;
                            }
                            else
                            {
                                _notificationService.ShowMessage(_result.Error, NotificationType.Error);
                                return;
                            }
                        }
                        else if(userType == "Regional manager")
                        {
                            await _progressService.StartProgressAsync();
                            var _result = await _zoneService.UnassignsUserFromZone(_currentRow.ZoneID, _currentRow.RegionalManagerUserId, new System.Threading.CancellationToken());
                            if (_result != null && _result.IsSuccess)
                            {
                                _notificationService.ShowMessage($"{userType} unassigned from {_currentRow.Zone}", NotificationType.Success);
                                LoadTerrioryViewAsync();
                                return;
                            }
                            else
                            {
                                _notificationService.ShowMessage(_result.Error, NotificationType.Error);
                                return;
                            }
                        }                       
                    }
                }            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            finally
            {
                await _progressService.StopProgressAsync();
            }
        }


        private async void EditTerritoryManagerCommandPopupEvent(int? TerritoryID)
        {
            try
            {
                AssignTerritoryManager assignTerritoryManager = new AssignTerritoryManager();
                assignTerritoryManager.Tag = TerritoryID;
                assignTerritoryManager.btnclose.Click -= Btnclose_Click;
                assignTerritoryManager.btnclose.Click += Btnclose_Click;
                await DialogHost.Show(assignTerritoryManager, "RootDialog");
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private void Btnclose_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private async void EditRegionalManagerCommandPopupEvent(int? ZoneID)
        {
            try
            {
                AssignRegionalManager assignRegionalManager = new AssignRegionalManager();               
                assignRegionalManager.Tag = ZoneID;
                await DialogHost.Show(assignRegionalManager, "RootDialog");
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private async  void LoadTerrioryViewAsync()
        {
            try
            {
                var _result = await _zoneService.GetView(new System.Threading.CancellationToken());
                if (_result != null && _result.IsSuccess)
                {
                    ZoneTerritoryView = new ObservableCollection<ZoneTerritoryView>(_result.Data);
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private async void AddStoreMapCommandPopupEvent()
        {
            try
            {
                AddStoreMap addStoreMap = new AddStoreMap();
                await DialogHost.Show(addStoreMap, "RootDialog");
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private async void AddTerritoyCommandPopupEvent()
        {
            try
            {
                AddNewTerritory addNewTerritory = new AddNewTerritory();
                await DialogHost.Show(addNewTerritory, "RootDialog");
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private async void AddZoneCommandPopupEvent()
        {
            try
            {
                AddNewZone addNewZone = new AddNewZone();               
                await DialogHost.Show(addNewZone, "RootDialog",AddZoneClosingEvent);

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private void AddZoneClosingEvent(object sender, DialogClosingEventArgs eventArgs)
        {
            
        }

        private ObservableCollection<ZoneTerritoryView> _zoneTerritoryView;

        public ObservableCollection<ZoneTerritoryView> ZoneTerritoryView
        {
            get { return _zoneTerritoryView; }
            set { SetProperty(ref _zoneTerritoryView, value); }
        }
    }
}
