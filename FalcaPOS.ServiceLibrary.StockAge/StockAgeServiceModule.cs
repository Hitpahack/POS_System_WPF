using FalcaPOS.Contracts;
using FalcaPOS.ServiceLibrary.StockAge.Services;
using Prism.Ioc;
using Prism.Modularity;

namespace FalcaPOS.ServiceLibrary.StockAge
{
    public class StockAgeServiceModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            _ = containerRegistry.Register<IStockAgeService, StockAgeService>();
        }
    }
}
