using FalcaPOS.Contracts;
using FalcaPOS.ServiceLibrary.Sales.Services;
using Prism.Ioc;
using Prism.Modularity;

namespace FalcaPOS.ServiceLibrary.Sales
{
    public class SalesServiceModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ISalesService, SalesService>();
        }
    }
}
