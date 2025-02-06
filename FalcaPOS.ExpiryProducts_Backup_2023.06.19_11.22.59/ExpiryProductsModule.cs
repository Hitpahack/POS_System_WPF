using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace FalcaPOS.ExpiryProducts
{
    public class ExpiryProductsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var _regions = containerProvider.Resolve<IRegionManager>();
            _regions.RegisterViewWithRegion("ExpiryProductsHome", typeof(ExpiryProducts.View.ExpiryProductsHome));
            _regions.RegisterViewWithRegion("CurrentMonth", typeof(ExpiryProducts.View.CurrentMonthExpiryProducts));
            _regions.RegisterViewWithRegion("NextMonth", typeof(ExpiryProducts.View.NextMonthExpiryProducts));
            _regions.RegisterViewWithRegion("Next3Month", typeof(ExpiryProducts.View.Next3MonthExpiryProducts));
            _regions.RegisterViewWithRegion("Next6Month", typeof(ExpiryProducts.View.Next6MonthExpiryProducts));
            _regions.RegisterViewWithRegion("Expired", typeof(ExpiryProducts.View.ExpiredProduct));



        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
