using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Location;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FalcaPOS.AddInventory.ViewModels
{
    internal class DistrictDialogViewModel : BindableBase, IDialogAware
    {
        //public string Title { get; set; }

        private string _title;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }


        private readonly ICommonService _locationService;

        private readonly Logger _logger;

        public DelegateCommand CancelCommand { get; private set; }
        public DelegateCommand<object> ResetCommand { get; private set; }

        public DelegateCommand CreateUpdateCommand { get; private set; }

        public INotificationService _notificationService;

        public DistrictDialogViewModel(ICommonService locationService, Logger logger, INotificationService notificationService)
        {
            _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            CancelCommand = new DelegateCommand(() => RequestClose?.Invoke(null));

            ResetCommand = new DelegateCommand<object>(Reset);

            CreateUpdateCommand = new DelegateCommand(CreateUpdateDistrict);
        }

        /// <summary>
        /// This method is called when user clicks on reset button in District Dialog.
        /// </summary>
        /// <param name="obj"></param>
        private void Reset(Object obj)
        {
            try
            {
                var _view = (DistrictDialogViewModel)obj;
                if (_view != null)
                {
                    // Clears the Name of the district and SelectedState.
                    _view.Name = null;
                    _view.SelectedState = null;
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }




        private async void CreateUpdateDistrict()
        {
            try
            {
                bool _isValidData = ValidateData();

                if (!_isValidData)
                {
                    return;
                }

                if (IsEdit)
                {

                    //edit.

                    var _district = new District
                    {
                        Name = Name,
                        //Shortname = ShortName.ToUpper(),
                        StateId = SelectedState.StateId
                    };

                    var _result = await _locationService.UpdateDistrict(districtId, _district);

                    if (_result != null && _result.IsSuccess)
                    {
                        _notificationService.ShowMessage($"District Updated", Common.NotificationType.Success);

                        RequestClose?.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters { { "district", _result.Data } }));
                    }
                    else
                    {
                        _notificationService.ShowMessage(_result?.Error ?? "An error occurred, try again", Common.NotificationType.Error);
                    }

                }
                else
                {

                    //create.
                    var _distrit = new District
                    {
                        Isvisible = true,
                        Name = Name,
                        //Shortname = ShortName.ToUpper(),
                        StateId = SelectedState.StateId
                    };


                    var _result = await _locationService.CreateDistrict(_distrit);

                    if (_result != null && _result.IsSuccess)
                    {
                        _notificationService.ShowMessage($"District created", Common.NotificationType.Success);

                        RequestClose?.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters { { "district", _result.Data } }));
                    }
                    else
                    {
                        _notificationService.ShowMessage(_result?.Error ?? "An error occurred, try again", Common.NotificationType.Error);
                    }


                }





            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in district dialog view model", _ex);
            }

        }

        private int districtId { get; set; }

        private bool ValidateData()
        {
            if (SelectedState == null)
            {
                _notificationService.ShowMessage("Select a state", Common.NotificationType.Error);

                return false;
            }
            if (!Name.IsValidString())
            {
                _notificationService.ShowMessage("District name is required", Common.NotificationType.Error);

                return false;
            }
            //if (!ShortName.IsValidString())
            //{
            //    _notificationService.ShowMessage("Enter valid short name", Common.NotificationType.Error);

            //    return false;
            //}
            //if (ShortName.Length != 3)
            //{
            //    _notificationService.ShowMessage("Short code should be 3 character in length", Common.NotificationType.Error);

            //    return false;
            //}

            return true;
        }

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            IsEdit = false;
            SelectedState = null;
            Name = null;
            //ShortName = null;
            Title = null;
        }

        private bool IsEdit { get; set; }

        public async void OnDialogOpened(IDialogParameters parameters)
        {

            var _stateResult = await LoadStates();

            if (_stateResult != null)
            {
                States = new ObservableCollection<State>(_stateResult.OrderBy(x => x.Name));
            }

            IsEdit = parameters.GetValue<bool>("isedit");

            Title = parameters.GetValue<string>("title");

            if (IsEdit)
            {
                ButtonText = "Update";
                SelectedState = States.FirstOrDefault(x => x.StateId == parameters.GetValue<int>("stateId"));
                Name = parameters.GetValue<string>("name");
                //ShortName = parameters.GetValue<string>("shortname");
                districtId = parameters.GetValue<int>("districtId");

            }
            else
            {
                ButtonText = "Create";
                SelectedState = null;

            }

        }


        private State _selectedState;

        public State SelectedState
        {
            get { return _selectedState; }
            set { _ = SetProperty(ref _selectedState, value); }
        }


        private ObservableCollection<State> _states;

        public ObservableCollection<State> States
        {
            get { return _states; }
            set { _ = SetProperty(ref _states, value); }
        }


        private string _name;

        public string Name
        {
            get { return _name; }
            set { _ = SetProperty(ref _name, value); }
        }


        //private string _shortName;

        //public string ShortName
        //{
        //    get { return _shortName; }
        //    set { _ = SetProperty(ref _shortName, value); }
        //}


        private string _buttonText;
       
        public string ButtonText
        {
            get { return _buttonText; }
            set { _ = SetProperty(ref _buttonText, value); }
        }


        private async Task<IEnumerable<State>> LoadStates() => await Task.Run(async () => { return await _locationService.GetStates(); });
    }
}
