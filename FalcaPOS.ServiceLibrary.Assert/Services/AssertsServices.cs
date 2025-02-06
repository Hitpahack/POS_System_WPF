using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Asserts;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.ServiceLibrary.Assert.Services
{
    public class AssertsServices : IAssertsServices
    {


        private readonly Logger _logger;

        public AssertsServices(Logger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Response<string>> AddAssertCategory(string category, int assertTypeId, CancellationToken token = default)
        {
            try
            {
                var _result = await HttpHelper
                     .PutAsync<Response<string>>($"{AppConstants.ASSERT_CATEGORY}/{category}/{assertTypeId}", AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            return new Response<string> { IsSuccess = false, Error = "An error occurred, try again" };

        }

        public async Task<Response<string>> AddAssertClass(string assertClass, int assertCodeId, CancellationToken token = default)
        {
            try
            {
                var _result = await HttpHelper
                     .PutAsync<Response<string>>($"{AppConstants.ASSERT_CLASS}/{assertClass}/{assertCodeId}", AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            return new Response<string> { IsSuccess = false, Error = "An error occurred, try again" };

        }

        public async Task<Response<string>> AddAssertCode(string code, CancellationToken token = default)
        {
            try
            {
                var _result = await HttpHelper
                     .PutAsync<Response<string>>($"{AppConstants.ASSERTS_CODE}/{code}", AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            return new Response<string> { IsSuccess = false, Error = "An error occurred, try again" };

        }

        public async Task<Response<string>> AddAsserts(AddAssertModel assertModel, CancellationToken token = default)
        {
            try
            {
                var _result = await HttpHelper
                     .PostAsync<AddAssertModel, Response<string>>($"{AppConstants.ADD_ASSERTS}", assertModel, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            return new Response<string> { IsSuccess = false, Error = "An error occurred, try again" };

        }

        public async Task<Response<string>> AddAssertType(string Type, int assertClassId, CancellationToken token = default)
        {
            try
            {
                var _result = await HttpHelper
                     .PutAsync<Response<string>>($"{AppConstants.ASSERT_TYPE}/{Type}/{assertClassId}", AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            return new Response<string> { IsSuccess = false, Error = "An error occurred, try again" };

        }

        public async Task<Response<IEnumerable<AssertsModel>>> GetAssertCategory(int assertTypeId, CancellationToken token = default)
        {
            try
            {
                var _result = await HttpHelper
                     .GetAsync<Response<IEnumerable<AssertsModel>>>($"{AppConstants.GET_ASSERT_CATEGORY}/{assertTypeId}", AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            return new Response<IEnumerable<AssertsModel>> { IsSuccess = false, Error = "An error occurred, try again" };
        }

        public async Task<Response<IEnumerable<AssertsModelResponse>>> GetAsserts(CancellationToken token = default)
        {
            try
            {
                var _result = await HttpHelper
                     .GetAsync<Response<IEnumerable<AssertsModelResponse>>>($"{AppConstants.GET_ASSERTS}", AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            return new Response<IEnumerable<AssertsModelResponse>> { IsSuccess = false, Error = "An error occurred, try again" };
        }

        public async Task<Response<IEnumerable<AssertsModel>>> GetAssertsClass(int assertCodeId, CancellationToken token = default)
        {
            try
            {
                var _result = await HttpHelper
                     .GetAsync<Response<IEnumerable<AssertsModel>>>($"{AppConstants.GET_ASSERT_CLASS}/{assertCodeId}", AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            return new Response<IEnumerable<AssertsModel>> { IsSuccess = false, Error = "An error occurred, try again" };
        }

        public async Task<Response<IEnumerable<AssertsModel>>> GetAssertsCode(CancellationToken token = default)
        {
            try
            {
                var _result = await HttpHelper
                     .GetAsync<Response<IEnumerable<AssertsModel>>>($"{AppConstants.GET_ASSERT_CODE}", AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            return new Response<IEnumerable<AssertsModel>> { IsSuccess = false, Error = "An error occurred, try again" };

        }

        public async Task<Response<IEnumerable<AssertsModelResponse>>> GetAssertSearch(SearchAssertsModel assertsModel, CancellationToken token = default)
        {
            try
            {
                var _result = await HttpHelper
                     .PostAsync<SearchAssertsModel, Response<IEnumerable<AssertsModelResponse>>>($"{AppConstants.GET_ASSERTS_SEARCH}", assertsModel, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            return new Response<IEnumerable<AssertsModelResponse>> { IsSuccess = false, Error = "An error occurred, try again" };
        }

        public async Task<Response<IEnumerable<AssertsModel>>> GetAssertsType(int assertClassId, CancellationToken token = default)
        {
            try
            {
                var _result = await HttpHelper
                     .GetAsync<Response<IEnumerable<AssertsModel>>>($"{AppConstants.GET_ASSERT_TYPE}/{assertClassId}", AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            return new Response<IEnumerable<AssertsModel>> { IsSuccess = false, Error = "An error occurred, try again" };
        }

        public async Task<Response<string>> EditAsserts(EditAssertModel editAssert, CancellationToken token = default)
        {
            try
            {
                var _result = await HttpHelper
                     .PostAsync<EditAssertModel, Response<string>>($"{AppConstants.EDIT_ASSERTS}", editAssert, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            return new Response<string> { IsSuccess = false, Error = "An error occurred, try again" };
        }
    }
}
