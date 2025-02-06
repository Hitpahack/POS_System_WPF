using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Login;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FalcaPOS.Login.ViewModels
{
    public class LoginTimeViewModel : BindableBase
    {

        private readonly ILoginService _loginService;

        private readonly Logger _logger;

        private readonly ProgressService _progressService;

        public DelegateCommand RefreshLoginTimeCommand { get; private set; }
        public LoginTimeViewModel(ILoginService loginService, Logger logger, ProgressService progressService)
        {

            _loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));

            LoadData();

            RefreshLoginTimeCommand = new DelegateCommand(LoadData);
        }


        public async void LoadData()
        {
            try
            {

                var _result = await _loginService.GetLoginTime();

                if (_result != null && _result.IsSuccess)
                {
                    List<LoginTimeModel> models = new List<LoginTimeModel>();

                    foreach (LoginTimeModel model in _result.Data)
                    {
                        models.Add(new LoginTimeModel()
                        {
                            Date = model.Date,
                            LoginTime = model.LoginTime,
                            StoreName = model.StoreName,
                            Time = model.CreateDateTime.ToString("hh:mm:ss tt")
                        });
                    }

                    LoginTimes = new ObservableCollection<LoginTimeModel>(models);
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private ObservableCollection<LoginTimeModel> _loginTimes;

        public ObservableCollection<LoginTimeModel> LoginTimes
        {
            get => _loginTimes;
            set => SetProperty(ref _loginTimes, value);
        }

    }


}
