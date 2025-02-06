using FalcaPOS.Common.Helper;
using FalcaPOS.Entites.Login;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{
    public interface ILoginService
    {
        //login method
        Task<Response<LoginResponse>> UserLogin(Login login);

        Task<Response<IEnumerable<LoginTimeModel>>> GetLoginTime(CancellationToken token = default(CancellationToken));
    }
}
