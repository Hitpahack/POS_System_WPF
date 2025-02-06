using FalcaPOS.Sales.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace FalcaPOS.Sales
{
    public class SalesModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var _regions = containerProvider.Resolve<IRegionManager>();
            _regions.RegisterViewWithRegion("SalesHome", typeof(Sales.Views.SalesHome));
            _regions.RegisterViewWithRegion("SalesView", typeof(SalesView));
            //_regions.RegisterViewWithRegion("ServicesView", typeof(ServicesView));
            _regions.RegisterViewWithRegion("SalesListRegion", typeof(SalesListView));
            _regions.RegisterViewWithRegion("FlyoutRegion", typeof(SalesSearchFlyout));
            _regions.RegisterViewWithRegion("ReturnView", typeof(ExchangeView));
            _regions.RegisterViewWithRegion("CreditViewRegion", typeof(CreditView));
            //_regions.RegisterViewWithRegion("AppordersRegion", typeof(AppOrdersView));
            //_regions.RegisterViewWithRegion("AppOrderProductList", typeof(AppOrderProductList));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
