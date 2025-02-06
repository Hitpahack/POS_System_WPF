using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Sku;
using FalcaPOS.Entites.Stores;
using FalcaPOS.Entites.User;
using FalcaPOS.Entites.Zone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.ServiceLibrary.Zone
{
    public class ZoneService : IZoneService
    {
        private readonly Logger _Logger;
        public ZoneService(Logger logger)
        {
            _Logger = logger;
        }
        public async Task<Response<string>> CreateTerritory(Territory territory, CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.PostAsync<Territory, Response<string>>(AppConstants.CREATE_TERRITORY, territory, AppConstants.ACCESS_TOKEN, token);
                return _result;

            }
            catch (Exception _ex)
            {
                _Logger.LogError(_ex.Message);
            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "No records found."
            };

        }

        public async Task<Response<string>> CreateTerritoryStoreMap(int storeId, int territoryId, CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.PutAsync<Response<string>>($"{AppConstants.PUT_CREATETERRITORYSTOREMAP}/{storeId}/{territoryId}", AppConstants.ACCESS_TOKEN, token);
                return _result;

            }
            catch (Exception _ex)
            {
                _Logger.LogError(_ex.Message);
            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

        public async Task<Response<string>> CreateZone(NewZone zone, CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.PostAsync<NewZone, Response<string>>(AppConstants.CREATE_ZONE, zone, AppConstants.ACCESS_TOKEN, token);
                return _result;

            }
            catch (Exception _ex)
            {
                _Logger.LogError(_ex.Message);
            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

       

        public async Task<Response<List<NewZone>>> GetZone(CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.GetAsync<Response<List<NewZone>>> (AppConstants.GET_ZONE, AppConstants.ACCESS_TOKEN, token);
                return _result;

            }
            catch (Exception _ex)
            {
                _Logger.LogError(_ex.Message);
            }

            return new Response<List<NewZone>>()
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

        public async Task<Response<List<Store>>> GetStoreMap(CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.GetAsync<Response<List<Store>>>(AppConstants.GET_STOREMAP, AppConstants.ACCESS_TOKEN, token);
                return _result;

            }
            catch (Exception _ex)
            {
                _Logger.LogError(_ex.Message);
            }

            return new Response<List<Store>>()
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

        public async Task<Response<List<Territory>>> GetTerritory(CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.GetAsync<Response<List<Territory>>>(AppConstants.GET_TERRITORY, AppConstants.ACCESS_TOKEN, token);
                return _result;

            }
            catch (Exception _ex)
            {
                _Logger.LogError(_ex.Message);
            }

            return new Response<List<Territory>>()
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

        public async Task<Response<List<ZoneTerritoryView>>> GetView(CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.GetAsync<Response<List<ZoneTerritoryView>>>(AppConstants.GET_TERRITORYVIEW, AppConstants.ACCESS_TOKEN, token);
                return _result;

            }
            catch (Exception _ex)
            {
                _Logger.LogError(_ex.Message);
            }

            return new Response<List<ZoneTerritoryView>>()
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

        public async Task<Response<List<User>>> GetRegionalMangersList(CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.GetAsync<Response<List<User>>>(AppConstants.GET_REGIONALMANGERSLIST, AppConstants.ACCESS_TOKEN, token);
                return _result;

            }
            catch (Exception _ex)
            {
                _Logger.LogError(_ex.Message);
            }

            return new Response<List<User>>()
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

        public async Task<Response<string>> AssignRegionalManagerToZone(int zoneId, int regionmanagerid, CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.PutAsync<Response<string>>($"{AppConstants.PUT_ASSIGNREGIONALMANAGERTOZONE}/{zoneId}/{regionmanagerid}", AppConstants.ACCESS_TOKEN, token);
                return _result;

            }
            catch (Exception _ex)
            {
                _Logger.LogError(_ex.Message);
            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

        public async Task<Response<string>> AssignTerritoryManagerToTerritory(int territoryId, int territorymanagerid, CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.PutAsync<Response<string>>($"{AppConstants.PUT_ASSIGNTERRITORYMANAGERTOTERRITORY}/{territoryId}/{territorymanagerid}", AppConstants.ACCESS_TOKEN, token);
                return _result;

            }
            catch (Exception _ex)
            {
                _Logger.LogError(_ex.Message);
            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

        public async Task<Response<List<User>>> GetTerritoryMangersList(CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.GetAsync<Response<List<User>>>(AppConstants.GET_TERRITORYMANAGERSLIST, AppConstants.ACCESS_TOKEN, token);
                return _result;

            }
            catch (Exception _ex)
            {
                _Logger.LogError(_ex.Message);
            }

            return new Response<List<User>>()
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

        public async Task<Response<string>> UnassignsUserFromStore(int storeId, int userId, CancellationToken cancellationToken)
        {
            try
            {

                var _result = await HttpHelper.PutAsync<Response<string>>($"{AppConstants.PUT_UNASSIGNSUSERFROMSTORE}/{storeId}/{userId}", AppConstants.ACCESS_TOKEN, cancellationToken);
                return _result;

            }
            catch (Exception _ex)
            {
                _Logger.LogError(_ex.Message);
            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

        public async Task<Response<string>> UnassignsUserFromZone(int zoneId, int userId, CancellationToken cancellationToken)
        {
            try
            {
                var _result = await HttpHelper.PutAsync<Response<string>>($"{AppConstants.PUT_UNASSIGNSUSERFROMZONE}/{zoneId}/{userId}", AppConstants.ACCESS_TOKEN, cancellationToken);
                return _result;

            }
            catch (Exception _ex)
            {
                _Logger.LogError(_ex.Message);
            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

        public async Task<Response<string>> UnassignsUserFromTerritory(int territoryId, int userId, CancellationToken cancellationToken)
        {
            try
            {

                var _result = await HttpHelper.PutAsync<Response<string>>($"{AppConstants.PUT_UNASSIGNSUSERFROMTERRITORY}/{territoryId}/{userId}", AppConstants.ACCESS_TOKEN, cancellationToken);
                return _result;

            }
            catch (Exception _ex)
            {
                _Logger.LogError(_ex.Message);
            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

        public async Task<IEnumerable<NewZone>> GetZones(int stateId, CancellationToken token = default)
        {
            try
            {
                return await HttpHelper.GetAsync<IEnumerable<NewZone>>($"{AppConstants.GET_STATE_ZONE}/{stateId}", AppConstants.ACCESS_TOKEN, token);
            }
            catch (Exception _ex)
            {
                _Logger.LogError("Error in getting all Zones", _ex);
            }

            return Enumerable.Empty<NewZone>();
        }
        public async Task<IEnumerable<Territory>> GetTerritories(int zoneId, CancellationToken token = default)
        {
            try
            {
                return await HttpHelper.GetAsync<IEnumerable<Territory>>($"{AppConstants.GET_ZONE_TERRITORY}/{zoneId}", AppConstants.ACCESS_TOKEN, token);
            }
            catch (Exception _ex)
            {

                _Logger.LogError("Error in getting all Territories", _ex);
            }

            return Enumerable.Empty<Territory>();
        }

        public async Task<IEnumerable<Store>> GetTerritoryStores(int territoryId, CancellationToken token = default)
        {
            try
            {
                return await HttpHelper.GetAsync<IEnumerable<Store>>($"{AppConstants.GET_TERRITORY_STORELIST}/{territoryId}", AppConstants.ACCESS_TOKEN, token);
            }
            catch (Exception _ex)
            {

                _Logger.LogError("Error in getting all territories store", _ex);
            }

            return Enumerable.Empty<Store>();
        }

    }
}
