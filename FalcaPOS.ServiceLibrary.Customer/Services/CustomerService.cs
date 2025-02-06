using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Customers;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.ServiceLibrary.Customer.Services
{
    public class CustomerService : ICustomerService
    {
        public async Task<CustomerDetails> GetCustomerByPhone(string phoneNumber, CancellationToken token = default(CancellationToken))
        {

            try
            {

                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper
                    .GetAsync<CustomerDetails>($"{AppConstants.GET_CUSTOMER_BY_PHONE}/{phoneNumber}", AppConstants.ACCESS_TOKEN, token);

                token.ThrowIfCancellationRequested();

                return _result;
            }
            catch (Exception _ex)
            {


            }


            return null;
        }


        public async Task<DataSet> GetCustomerSearch(CustomerSearchModel customerModelRequest)
        {
            try
            {

                var _result = await HttpHelper.PostAsyncDataSet<CustomerSearchModel, DataSet>(AppConstants.GET_CUSTOMER_SEARCH, customerModelRequest, AppConstants.ACCESS_TOKEN);

                return _result;
            }
            catch (Exception _ex)
            {

            }
            return null;
        }
    }
}
