using FalcaPOS.Contracts;
using FalcaPOS.ServiceLibrary.Stock.Services;
using Prism.Ioc;
using Prism.Modularity;

namespace FalcaPOS.ServiceLibrary.Stock
{
    public class StockServiceModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            _ = containerRegistry.Register<IStoreStockService, StoreStockService>();
            _ = containerRegistry.Register<IBarCodeService, BarCodeService>();
            _ = containerRegistry.Register<IClosingStockReportService, ClosingStockReportService>();
            _ = containerRegistry.Register<IStockTransferService, StockTransferService>();


        }
    }
}
