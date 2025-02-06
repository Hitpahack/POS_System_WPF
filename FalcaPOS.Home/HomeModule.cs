using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Unity;

namespace FalcaPOS.Home
{
    public class HomeModule : IModule
    {

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var _regionmanager = containerProvider.Resolve<IRegionManager>();
            _regionmanager.RegisterViewWithRegion("LoginRegion", typeof(FalcaPOS.Home.Views.Home));

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Home.Views.Home>(typeof(FalcaPOS.Home.Views.Home).FullName);
        }
    }
}
