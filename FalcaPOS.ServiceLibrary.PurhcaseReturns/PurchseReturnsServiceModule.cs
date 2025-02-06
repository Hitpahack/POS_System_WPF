using FalcaPOS.Contracts;
using FalcaPOS.ServiceLibrary.PurhcaseReturns.Services;
using Prism.Ioc;
using Prism.Modularity;

namespace FalcaPOS.ServiceLibrary.PurchaseReturns
{
    public class PurchseReturnsServiceModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            _ = containerRegistry.RegisterSingleton<IPurchaseReturnService, PurhaseReturnsService>();
        }
    }
}
