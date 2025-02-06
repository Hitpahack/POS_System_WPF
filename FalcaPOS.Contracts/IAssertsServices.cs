using FalcaPOS.Common.Helper;
using FalcaPOS.Entites.Asserts;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{
    public interface IAssertsServices
    {

        Task<Response<string>> AddAssertCode(string code, CancellationToken token = default(CancellationToken));

        Task<Response<string>> AddAssertType(string type, int assertClassId, CancellationToken token = default(CancellationToken));

        Task<Response<string>> AddAssertClass(string assertClass, int assertCodeId, CancellationToken token = default(CancellationToken));

        Task<Response<string>> AddAssertCategory(string category, int assertTypeId, CancellationToken token = default(CancellationToken));

        Task<Response<string>> AddAsserts(AddAssertModel assertModel, CancellationToken token = default(CancellationToken));

        Task<Response<IEnumerable<AssertsModelResponse>>> GetAsserts(CancellationToken token = default(CancellationToken));

        Task<Response<IEnumerable<AssertsModel>>> GetAssertsCode(CancellationToken token = default(CancellationToken));
        Task<Response<IEnumerable<AssertsModel>>> GetAssertsClass(int assertCodeId, CancellationToken token = default(CancellationToken));

        Task<Response<IEnumerable<AssertsModel>>> GetAssertsType(int assertClassId, CancellationToken token = default(CancellationToken));
        Task<Response<IEnumerable<AssertsModel>>> GetAssertCategory(int assertTypeId, CancellationToken token = default(CancellationToken));

        Task<Response<IEnumerable<AssertsModelResponse>>> GetAssertSearch(SearchAssertsModel assertsModel, CancellationToken token = default(CancellationToken));

        Task<Response<string>> EditAsserts(EditAssertModel assertModel, CancellationToken token = default(CancellationToken));

    }
}
