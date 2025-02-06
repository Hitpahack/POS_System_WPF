using FalcaPOS.Common.Helper;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{
    public interface IBarCodeService
    {
        Task<Response<string>> GetBarCode(int productId, CancellationToken token = default(CancellationToken));

    }
}
