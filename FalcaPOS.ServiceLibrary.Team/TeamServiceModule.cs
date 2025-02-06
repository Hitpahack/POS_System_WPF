using FalcaPOS.Contracts;
using FalcaPOS.ServiceLibrary.Team.Services;
using Prism.Ioc;
using Prism.Modularity;

namespace FalcaPOS.ServiceLibrary.Team
{
    public class TeamServiceModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ITeamService, TeamService>();
        }
    }
}
