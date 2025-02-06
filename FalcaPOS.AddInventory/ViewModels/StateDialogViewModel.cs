using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Location;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.AddInventory.ViewModels
{
    internal class StateDialogViewModel : BindableBase, IDialogAware
    {

        public DelegateCommand CancelCommand { get; private set; }

        public DelegateCommand CreateUpdateCommand { get; private set; }
        public DelegateCommand<object> ResetCommand { get; private set; }


        private readonly ICommonService _locationService;
        private readonly INotificationService _notificationService;
        private readonly Logger _logger;

        public StateDialogViewModel(ICommonService locationService, INotificationService notificationService)
        {
            CancelCommand = new DelegateCommand(() => RequestClose?.Invoke(null));

            CreateUpdateCommand = new DelegateCommand(CreateUpdateState);
            ResetCommand = new DelegateCommand<object>(Reset);

            _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }
       
        private async void CreateUpdateState()
        {

            bool _isValid = ValidateData();

            if (!_isValid) return;


            if (IsEdit)
            {

                var _updateState = new State
                {
                    Isvisible = true,
                    Name = Name,
                    //Shortname = ShortName.ToUpper()
                };

                await Task.Run(async () =>
                {

                    var _result = await _locationService.UpdateState(_state.StateId, _updateState);

                    if (_result != null)
                    {
                        Application.Current?.Dispatcher?.Invoke(() =>
                        {

                            if (_result.IsSuccess)
                            {
                                _notificationService.ShowMessage("State updated", Common.NotificationType.Success);

                                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters
                                {
                                    { "state",_result.Data }
                                }));
                            }
                            else
                            {
                                _notificationService.ShowMessage(_result.Error ?? "An error occurred, try again", Common.NotificationType.Error);
                            }
                        });
                    }

                });
            }
            else
            {

                var _newState = new State
                {
                    Name = Name,
                    //Shortname = ShortName.ToUpper(),                   
                    Isvisible = true
                };

                await Task.Run(async () =>
                {

                    var _result = await _locationService.CreateState(_newState);

                    if (_result != null)
                    {
                        Application.Current?.Dispatcher?.Invoke(() =>
                        {

                            if (_result.IsSuccess)
                            {
                                _notificationService.ShowMessage("State created", Common.NotificationType.Success);

                                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters
                                {
                                    { "state",_result.Data }
                                }));
                            }
                            else
                            {
                                _notificationService.ShowMessage(_result.Error ?? "An error occurred, try again", Common.NotificationType.Error);
                            }
                        });
                    }

                });
            }




        }
        private void Reset(Object obj)
        {
            try
            {
                var _view = (StateDialogViewModel)obj;
                if (_view != null)
                {
                    _view.Name = null;
                    


                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }
        private bool ValidateData()
        {
            if (!Name.IsValidString())
            {
                _notificationService.ShowMessage("State name is required", Common.NotificationType.Error);

                return false;
            }

            //if (!ShortName.IsValidString())
            //{
            //    _notificationService.ShowMessage("Short code is required", Common.NotificationType.Error);

            //    return false;
            //}

            //if (ShortName.Length != 3)
            //{
            //    _notificationService.ShowMessage("Short code should be 3 character in length", Common.NotificationType.Error);

            //    return false;
            //}

            return true;
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }


        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            _state = null;
            Name = null;
            //ShortName = null;
            IsEdit = false;
            Title = null;
        }

        public bool IsEdit { get; set; }

        private State _state { get; set; }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }


        //private string _shortName;

        //public string ShortName
        //{
        //    get { return _shortName; }
        //    set { SetProperty(ref _shortName, value); }
        //}

        private string _buttonText;

        public string ButtonText
        {
            get { return _buttonText; }
            set { SetProperty(ref _buttonText, value); }
        }



        public void OnDialogOpened(IDialogParameters parameters)
        {
            Title = parameters.GetValue<string>("title");

            IsEdit = parameters.GetValue<bool>("isedit");

            if (IsEdit)
            {
                _state = parameters.GetValue<State>("state");

                Name = _state.Name;
                //ShortName = _state.Shortname;

                ButtonText = "Update";

            }
            else
            {

                ButtonText = "Create";
            }


        }
    }
}
