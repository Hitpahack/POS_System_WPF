using FalcaPOS.Common.Helper;
using FalcaPOS.Entites.Location;
using FalcaPOS.Entites.Zone;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{
    public interface ICommonService
    {
        Task<IEnumerable<State>> GetStates(string querry = null, CancellationToken token = default(CancellationToken));

        Task<Response<State>> CreateState(State state, CancellationToken token = default(CancellationToken));

        Task<Response<State>> UpdateState(int stateId, State state, CancellationToken token = default(CancellationToken));

        Task<Response<District>> CreateDistrict(District district, CancellationToken token = default(CancellationToken));

        Task<Response<District>> UpdateDistrict(int districtId, District district, CancellationToken token = default(CancellationToken));

        Task<IEnumerable<District>> GetDistricts(int stateId, CancellationToken token = default(CancellationToken));

        Task<IEnumerable<District>> GetAllDistricts(CancellationToken token = default(CancellationToken));

        bool ExportToXL<T>(List<T> data, string fileName = null, bool skipfileName = false, string tablename = "Falca POS Reports", string FilePath = null);

        bool ExportToXL<T1, T2>(IEnumerable<T1> T1Data, IEnumerable<T2> T2Data, string T1Title, string T2Titile, string fileName = null, bool skipfile = false, string FilePath = null);

        bool ExportObjectToXL<T>(List<T> data, List<string> columns, Dictionary<string,String> propDisplayNames, string fileName = null, bool skipfileName = false, string tablename = "Falca POS Reports", string FilePath = null);
    }
}
