
using Prism.Ioc;
using Prism.Modularity;

namespace FalcaPOS.PurchaseReturn
{
    public class PurchaseReturnModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            // var _regionmanager = containerProvider.Resolve<IRegionManager>();
            // _regionmanager.RegisterViewWithRegion("TeamHome", typeof(FalcaPOS.Team.Views.Team));
            // _regionmanager.RegisterViewWithRegion("FlyoutRegion", typeof(AddUserFlyout));
            // _regionmanager.RegisterViewWithRegion("FlyoutRegion", typeof(EditUserFlyOut));

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
