using FalcaPOS.Common;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.User;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.Team.ViewModels
{
    public class TeamViewModel : BindableBase
    {
        private readonly ITeamService _teamService;

        private readonly IEventAggregator _eventAggregator;

        public DelegateCommand<object> DeleteUserCommand { get; private set; }

        public DelegateCommand<object> EnableUserCommand { get; private set; }

        public DelegateCommand CreateUserCommand { get; private set; }

        public DelegateCommand RefreshTeamCommand { get; private set; }

        public DelegateCommand<object> EditUserCommand { get; private set; }

        private readonly ProgressService _ProgressService;

        private readonly Logger _logger;

        public TeamViewModel(ITeamService TeamService, IEventAggregator EventAggregator, ProgressService ProgressService, Logger Logger)
        {
            _teamService = TeamService;

            _eventAggregator = EventAggregator;

            _eventAggregator.GetEvent<ReloadUsersEvent>().Subscribe(LoadUsers);

            DeleteUserCommand = new DelegateCommand<object>(DeleteUser);

            CreateUserCommand = new DelegateCommand(CreateUser);

            EnableUserCommand = new DelegateCommand<object>(EnableUser);

            EditUserCommand = new DelegateCommand<object>(EditUser);

            RefreshTeamCommand = new DelegateCommand(LoadUsers);

            _eventAggregator.GetEvent<UserAddedEvent>().Subscribe(NewUserAdded);

            _ProgressService = ProgressService ?? throw new ArgumentNullException(nameof(ProgressService));

            _logger = Logger ?? throw new ArgumentNullException(nameof(Logger));

            LoadUsers();

        }

        private void NewUserAdded(object obj)
        {
            _logger.LogInformation("New user was added");

            if (obj is User _user)
            {
                try
                {

                    //check if it already contatains user. 

                    if (Users != null && Users.Any(x => x.UserId == _user.UserId))
                    {
                        //user already loadded 
                        _logger.LogInformation($"New user {_user?.Name} is already added ");

                        return;
                    }


                    if (Users == null)
                    {

                        Users = new ObservableCollection<User>();
                    }

                    Application.Current?.Dispatcher.Invoke(() =>
                    {
                        Users.Add(_user);
                    });

                    _logger.LogInformation($"added new user {_user.Name} to team");
                }
                catch (Exception _ex)
                {
                    _logger.LogError("Error in adding new user to list ", _ex);

                }


            }
        }

        private void EditUser(object user)
        {
            if (user != null && (user is User))
                _eventAggregator.GetEvent<EditUserFlyoutOpenEvent>().Publish(user);
        }

        private async void EnableUser(object obj)
        {

            bool _result = false;


            var _user = obj as User;

            if (_user == null || _user.UserId <= 0)
            {
                ShowAlert("Invalid user ", NotificationType.Error);
                return;
            }

            if (_user.IsAlive != null && _user.IsAlive.Value)
            {
                ShowAlert("User is already enabled.", NotificationType.Information);
                return;
            }

            await Task.Run(async () => { _result = await _teamService.EnableDisableUser(_user.UserId, true); });

            if (_result)
            {
                ShowAlert("User Enabled.", NotificationType.Success);
                LoadUsers();
            }

        }

        private void CreateUser() => _eventAggregator.GetEvent<AddUserFlyoutOpenEvent>().Publish(true);

        private void ShowAlert(string message, NotificationType notification)
        {
            _eventAggregator.GetEvent<NotifyMessage>().Publish(new Common.Models.ToastMessage
            {
                Message = message,
                MessageType = notification
            });
        }


        private async void DeleteUser(object obj)
        {

            bool _result = false;


            var _user = obj as User;

            if (_user == null || _user.UserId <= 0)
            {
                ShowAlert("Invalid user ", NotificationType.Error);
                return;
            }

            if (_user.IsAlive != null && !_user.IsAlive.Value)
            {
                ShowAlert("User is already disabled.", NotificationType.Information);
                return;
            }

            var _confirm = await _ProgressService.ConfirmAsync("Disable User!",
                $"Do you want to disable user {_user.Name} ? user will be forced to logout if logged in.");

            if (_confirm == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Negative)
            {
                return;
            }

            await Task.Run(async () => { _result = await _teamService.EnableDisableUser(_user.UserId, false); });

            if (_result)
            {
                ShowAlert("User disabled.", NotificationType.Success);
                LoadUsers();
            }

        }

        private async void LoadUsers()
        {
            Users = null;



            await Task.Run(async () =>
            {
                var _result = await _teamService.GetUsers();

                if (_result != null && _result.Any())
                {
                    Application.Current?.Dispatcher.Invoke(() =>
                    {
                        Users = new ObservableCollection<User>(_result);
                    });
                }

            });



        }

        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get { return _users; }
            set { SetProperty(ref _users, value); }
        }
    }
}
