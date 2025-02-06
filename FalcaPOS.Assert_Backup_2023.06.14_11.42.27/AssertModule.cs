using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace FalcaPOS.Assert
{
    public class AssertModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var _regions = containerProvider.Resolve<IRegionManager>();
            _regions.RegisterViewWithRegion("Asserts", typeof(View.Asserts));
            _regions.RegisterViewWithRegion("AddAssertCode", typeof(View.AddAssertCode));
            _regions.RegisterViewWithRegion("AddAssertType", typeof(View.AddAssertType));
            _regions.RegisterViewWithRegion("AddAssertClass", typeof(View.AddAssertClass));
            _regions.RegisterViewWithRegion("AddAssertCategory", typeof(View.AddAssertCategory));
            _regions.RegisterViewWithRegion("FlyoutRegion", typeof(View.AssertSearchFlyout));

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
