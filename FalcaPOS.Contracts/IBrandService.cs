using FalcaPOS.Common.Helper;
using FalcaPOS.Entites.Manufacturers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{
    public interface IBrandService
    {
        Task<IEnumerable<Manufacturer>> GetAllBrands(CancellationToken token = default(CancellationToken));

        Task<Response<string>> EnableDisableBrand(int brandId, bool isenable, CancellationToken token = default(CancellationToken));
    }
}
