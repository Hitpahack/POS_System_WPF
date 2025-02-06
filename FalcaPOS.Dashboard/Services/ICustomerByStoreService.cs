using FalcaPOS.Common.Helper;
using FalcaPOS.Entites.Dashboard;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Dashboard.Services
{
    public interface ICustomerByStoreService
    {
        Task<Response<DashboardCustomer>> GetCustomerByStore(int districtId = 0, CancellationToken token = default(CancellationToken));
    }
}
