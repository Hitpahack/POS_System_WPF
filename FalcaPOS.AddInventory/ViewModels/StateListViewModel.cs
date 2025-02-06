using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Location;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.AddInventory.ViewModels
{
    public class StateListViewModel : BindableBase
    {

        private readonly ICommonService _locationService;

        private readonly Logger _logger;

        public DelegateCommand<object> EditStateCommand { get; private set; }

        public DelegateCommand AddNewStateCommand { get; private set; }

        private readonly IDialogService _dialogService;

        public StateListViewModel(ICommonService locationService, Logger logger, IDialogService dialogService)
        {
            _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            EditStateCommand = new DelegateCommand<object>(EditState);

            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            AddNewStateCommand = new DelegateCommand(AddNewState);

            LoadStatesAsync();

        }

        private const string State_Dialog = "StateDialog";

        private void AddNewState()
        {
            var _params = new DialogParameters
            {
                { "title", "Add new state" },

                { "isedit", false }
            };

            _dialogService.ShowDialog(State_Dialog, _params, HandleCallBack);
        }

        private void HandleCallBack(IDialogResult res)
        {

            if (res != null && res.Result == ButtonResult.OK)
            {
                var _updatedState = res.Parameters.GetValue<State>("state");

                if (_updatedState != null)
                {
                    var _oldState = States.FirstOrDefault(x => x.StateId == _updatedState.StateId);

                    if (_oldState != null)
                    {
                        States.Remove(_oldState);
                    }
                }
                States.Add(_updatedState);
                States = new ObservableCollection<State>(States.OrderBy(x => x.Name));


            }
        }

        private void EditState(object obj)
        {
            if (obj is State _state)
            {
                _logger.LogInformation($"Editing state {_state.Name}");

                var _params = new DialogParameters
                {
                    { "title", "Edit state" },

                    { "isedit", true },

                    { "state", _state }
                };

                _dialogService.ShowDialog(State_Dialog, _params, HandleCallBack);
            }
        }

        private ObservableCollection<State> _states;

        public ObservableCollection<State> States
        {
            get { return _states; }
            set { _ = SetProperty(ref _states, value); }
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
                            States = new ObservableCollection<State>(_result);
                        });
                    }
                });
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in loading all states list view model", _ex);
            }
        }

    }
}
