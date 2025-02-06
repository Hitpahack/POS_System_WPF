using FalcaPOS.Entites.Customers;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{
    public interface ICustomerService
    {
        Task<CustomerDetails> GetCustomerByPhone(string phoneNumber, CancellationToken token = default(CancellationToken));

        Task<DataSet> GetCustomerSearch(CustomerSearchModel customerModelRequest);

    }
}
