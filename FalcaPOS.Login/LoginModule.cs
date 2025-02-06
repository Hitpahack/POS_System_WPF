using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace FalcaPOS.Login
{
    public class LoginModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("LoginRegion", typeof(FalcaPOS.Login.Views.Login));
            regionManager.RegisterViewWithRegion("LoginTimeRegion", typeof(FalcaPOS.Login.Views.LoginTime));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Login.Views.Login>(typeof(FalcaPOS.Login.Views.Login).FullName);
        }
    }
}
