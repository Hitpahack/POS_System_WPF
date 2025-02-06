using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Team;
using FalcaPOS.Entites.User;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.ServiceLibrary.Team.Services
{
    public class TeamService : ITeamService
    {
        private readonly IEventAggregator _eventAggregator;

        public TeamService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }


        public async Task<RegisterResponse> CreateUser(User user)
        {

            try
            {
                RegisterResponse _result = await HttpHelper.PostAsync<User, RegisterResponse>(AppConstants.CREATE_USER, user, AppConstants.ACCESS_TOKEN);

                return _result;
            }
            catch (Exception _ex)
            {
            }

            return default(RegisterResponse);
        }



        public async Task<bool> EnableDisableUser(int userId, bool enable)
        {
            if (userId <= 0) return false;

            try
            {
                return await HttpHelper.PutAsync($"{AppConstants.ENABLE_DISABLE_USER}/{userId}/{enable}", AppConstants.ACCESS_TOKEN);

            }
            catch (Exception _ex)
            {

            }


            return false;

        }

        public async Task<IEnumerable<User>> GetUsers(CancellationToken token = default(CancellationToken))
        {
            try
            {

                var _result = await HttpHelper.GetAsync<IEnumerable<User>>(AppConstants.GET_USERS, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
            }

            return Enumerable.Empty<User>();
        }

        public async Task<Response<User>> UpdateUser(int userId, UpdateUserModel user)
        {


            try
            {
                if (userId <= 0)
                {
                    ShowAlert("Invalid User Id", NotificationType.Error);
                }
                return await HttpHelper.PutAsync<UpdateUserModel, Response<User>>($"{AppConstants.UPDATE_USER}/{userId}", user, AppConstants.ACCESS_TOKEN);

            }
            catch (Exception _ex)
            {
                ShowAlert(_ex.Message, NotificationType.Error);

            }

            return default(Response<User>);
        }

        void ShowAlert(string msg, NotificationType notificationType)
        {
            _eventAggregator.GetEvent<NotifyMessage>().Publish(new Common.Models.ToastMessage
            {
                Message = msg,
                MessageType = notificationType
            });
        }
    }
}
