using FalcaPOS.Contracts;
using FalcaPOS.ServiceLibrary.PurchaseInvoice.Services;
using Prism.Ioc;
using Prism.Modularity;

namespace FalcaPOS.ServiceLibrary.PurchaseInvoice
{
    public class PurchaseInvoiceServiceModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            _ = containerRegistry.RegisterSingleton<IPurchaseInvoiceService, PurchaseInvoiceService>();
        }
    }
}
