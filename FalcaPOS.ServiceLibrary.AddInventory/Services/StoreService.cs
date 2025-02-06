using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Stores;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.ServiceLibrary.AddInventory.Services
{
    public class StoreService : IStoreService
    {
        private readonly IEventAggregator _eventAggregator;

        public StoreService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public async Task<Response<string>> AddStore(Store store, CancellationToken token = default(CancellationToken))
        {
            try
            {
                var _result = await HttpHelper.PostAsync<Store, Response<string>>(AppConstants.ADD_STORE, store, AppConstants.ACCESS_TOKEN);

                if (_result != null)
                {
                    if (!_result.IsSuccess)
                        ShowAlert(_result.Error, NotificationType.Error);
                    else
                        return _result;

                }

            }
            catch (Exception)
            {

            }

            return default(Response<string>);
        }

        public async Task<Response<string>> GetStoreInvoiceFormat(int districtId, CancellationToken token = default(CancellationToken))
        {
            try
            {
                if (districtId <= 0) return new Response<string> { IsSuccess = false, Error = "Invalid district code" };

                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.GetAsync<Response<string>>($"{AppConstants.GET_STORE_INVOICE_FORMAT}/{districtId}", AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex) when (_ex is OperationCanceledException)
            {
                return new Response<string>
                {
                    IsSuccess = false
                };
            }
            catch (Exception _ex)
            {
                //log error

            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "An error occurred , try again"
            };

        }

        public async Task<IEnumerable<Entites.Stores.Store>> GetStores(string query)
        {
            try
            {

                string _url = AppConstants.STORES_GETALL;

                if (!string.IsNullOrEmpty(query))
                {
                    _url = string.Format("{0}?{1}", _url, query);
                }
                var _result = await HttpHelper.GetAsync<IEnumerable<Entites.Stores.Store>>(_url, AppConstants.ACCESS_TOKEN);

                return _result;

            }
            catch (Exception _Ex)
            {

            }
            return null;
        }
        public async Task<IEnumerable<Entites.Stores.StoreLicense>> GetStoreLicense(int StoreId, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                var _result = await HttpHelper.GetAsync<IEnumerable<Entites.Stores.StoreLicense>>($"{AppConstants.GET_STORE_LICENSE}/{StoreId}", AppConstants.ACCESS_TOKEN, token);
                return _result;
            }
            catch (Exception _Ex)
            {

            }
            return null;
        }
        public async Task<IEnumerable<Entites.Stores.Store>> GetStoreDetailsbyuser(int userid, string role, CancellationToken token = default)
        {
            try
            {

                token.ThrowIfCancellationRequested();
                var _result = await HttpHelper.GetAsync<IEnumerable<Entites.Stores.Store>>($"{AppConstants.STORES_GET_BYUSER}/{userid}/{role}", AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _Ex)
            {

            }
            return null;
        }

        public async Task<Response<string>> UpdateStore(Store store, CancellationToken token = default(CancellationToken))
        {
            try
            {
                var _result = await HttpHelper.PutAsync<Store, Response<string>>($"{AppConstants.UPDATE_STORE}/{store.StoreId}", store, AppConstants.ACCESS_TOKEN);

                if (_result != null && _result.IsSuccess)
                {
                    return _result;
                }
                else
                {
                    if (_result != null && !_result.IsSuccess)
                        ShowAlert(_result.Error, NotificationType.Error);
                }

            }
            catch (Exception)
            {
            }

            return default(Response<string>);
        }

        private void ShowAlert(string msg, NotificationType notificationType)
            => _eventAggregator.GetEvent<NotifyMessage>()
              .Publish(new Common.Models.ToastMessage { Message = msg, MessageType = notificationType });

    }
}
