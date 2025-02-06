using FalcaPOS.Store.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace FalcaPOS.Store
{
    public class StoreModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var _regionmanager = containerProvider.Resolve<IRegionManager>();
            _regionmanager.RegisterViewWithRegion("StoreHome", typeof(Views.Store));
            _regionmanager.RegisterViewWithRegion("FlyoutRegion", typeof(AddStoreFlyout));
            _regionmanager.RegisterViewWithRegion("FlyoutRegion", typeof(EditStoreFlyout));
            _regionmanager.RegisterViewWithRegion("FlyoutRegion", typeof(AddStoreLicense));
            _regionmanager.RegisterViewWithRegion("FlyoutRegion", typeof(EditStoreLicense));

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
