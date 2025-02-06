using FalcaPOS.Contracts;
using FalcaPOS.ServiceLibrary.Sku.Services;
using Prism.Ioc;
using Prism.Modularity;

namespace FalcaPOS.ServiceLibrary.Sku
{
    public class SkuServiceModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ISkuService, SkuService>();
        }
    }
}
