using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Login;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.ServiceLibrary.Login.Service
{
    public class LoginService : ILoginService
    {


        public async Task<Response<LoginResponse>> UserLogin(Entites.Login.Login login)
        {

            try
            {
                var _response = await HttpHelper.PostAsync<Entites.Login.Login, Response<LoginResponse>>(AppConstants.LOGIN_URL, login);

                return _response;
            }
            catch (Exception _ex)
            {

                return new Response<LoginResponse>
                {
                    IsSuccess = false,
                    Error = _ex.Message
                };
            }
        }

        public async Task<Response<IEnumerable<LoginTimeModel>>> GetLoginTime(CancellationToken token)
        {
            try
            {

                token.ThrowIfCancellationRequested();


                var _result = await HttpHelper.GetAsync<Response<IEnumerable<LoginTimeModel>>>(AppConstants.GET_LOGIN_TIME, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                return new Response<IEnumerable<LoginTimeModel>>
                {
                    IsSuccess = false,
                    Error = _ex.Message
                };
            }
        }
    }
}
