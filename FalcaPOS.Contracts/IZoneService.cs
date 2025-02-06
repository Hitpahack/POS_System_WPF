using FalcaPOS.Common.Helper;
using FalcaPOS.Entites.Stores;
using FalcaPOS.Entites.User;
using FalcaPOS.Entites.Zone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{
    public interface IZoneService
    {
        Task<Response<String>> CreateZone(NewZone zone, CancellationToken token = default);
        Task<Response<String>> CreateTerritory(Territory territory, CancellationToken token = default);
        Task<Response<String>> CreateTerritoryStoreMap(int storeId, int territoryId, CancellationToken token = default);
        Task<Response<List<NewZone>>> GetZone(CancellationToken token = default);
        Task<Response<List<Territory>>> GetTerritory(CancellationToken token = default);
        Task<Response<List<Store>>> GetStoreMap(CancellationToken token = default);
        Task<Response<List<ZoneTerritoryView>>> GetView(CancellationToken token = default);
        Task<Response<String>> AssignRegionalManagerToZone(int zoneId, int regionmanagerid, CancellationToken token = default);
        Task<Response<String>> AssignTerritoryManagerToTerritory(int territoryId, int territorymanagerid, CancellationToken token = default);
        Task<Response<List<User>>> GetRegionalMangersList(CancellationToken token = default);

        Task<Response<List<User>>> GetTerritoryMangersList(CancellationToken token = default);

        Task<Response<String>> UnassignsUserFromStore(int storeId, int userId, CancellationToken cancellationToken);
        Task<Response<string>> UnassignsUserFromZone(int zoneId, int userId, CancellationToken cancellationToken);
        Task<Response<string>> UnassignsUserFromTerritory(int territoryId, int userId, CancellationToken cancellationToken);
        Task<IEnumerable<NewZone>> GetZones(int stateId, CancellationToken token = default(CancellationToken));

        Task<IEnumerable<Territory>> GetTerritories(int zoneId, CancellationToken token = default(CancellationToken));
        Task<IEnumerable<Store>> GetTerritoryStores(int territoryId, CancellationToken token = default);
    }

}
