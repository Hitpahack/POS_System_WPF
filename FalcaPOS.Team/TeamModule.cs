using FalcaPOS.Team.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace FalcaPOS.Team
{
    public class TeamModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var _regionmanager = containerProvider.Resolve<IRegionManager>();
            _regionmanager.RegisterViewWithRegion("TeamHome", typeof(FalcaPOS.Team.Views.Team));
            _regionmanager.RegisterViewWithRegion("FlyoutRegion", typeof(AddUserFlyout));
            _regionmanager.RegisterViewWithRegion("FlyoutRegion", typeof(EditUserFlyOut));

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
