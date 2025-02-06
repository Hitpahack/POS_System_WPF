using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace FalcaPOS.Customer
{
    public class CustomerModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var _regions = containerProvider.Resolve<IRegionManager>();
            _regions.RegisterViewWithRegion("CustomerHome", typeof(FalcaPOS.Customer.Views.Customer));
            _regions.RegisterViewWithRegion("FlyoutRegion", typeof(FalcaPOS.Customer.Views.CustomerSearchFlyout));

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
