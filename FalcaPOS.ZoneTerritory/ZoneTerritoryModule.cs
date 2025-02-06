
using FalcaPOS.ZoneTerritory.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace FalcaPOS.ZoneTerritory
{
    public class ZoneTerritoryModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var _region = containerProvider.Resolve<IRegionManager>();
            _region.RegisterViewWithRegion("ZoneTerritoryHome", typeof(ZoneTerritoryHome));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
