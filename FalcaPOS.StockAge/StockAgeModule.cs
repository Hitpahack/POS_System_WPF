using FalcaPOS.StockAge.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace FalcaPOS.StockAge
{
    public class StockAgeModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var _region = containerProvider.Resolve<IRegionManager>();
            _region.RegisterViewWithRegion("StockAgeing", typeof(StockAgeing));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
