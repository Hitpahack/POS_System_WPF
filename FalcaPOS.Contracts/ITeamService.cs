using FalcaPOS.Common.Helper;
using FalcaPOS.Entites.Team;
using FalcaPOS.Entites.User;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{
    public interface ITeamService
    {
        Task<IEnumerable<User>> GetUsers(CancellationToken token = default(CancellationToken));

        Task<bool> EnableDisableUser(int userId, bool enable);

        Task<Response<User>> UpdateUser(int userId, UpdateUserModel user);

        Task<RegisterResponse> CreateUser(User user);
    }
}
