using FalcaPOS.Common.Constants;
using FalcaPOS.Director.View;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace FalcaPOS.Director
{
    public class DirectorModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

            //register view with region.

            IRegionManager _region = containerProvider.Resolve<IRegionManager>();

            _ = _region.RegisterViewWithRegion(AppConstants.REGION_PURCHASE_RATE, typeof(PurchaseRate));

            _ = _region.RegisterViewWithRegion(AppConstants.REGION_STORE_ASSERT, typeof(StoreAssert));
            _ = _region.RegisterViewWithRegion("FlyoutRegion", typeof(SearchPurchaseRateFlyout));




        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
