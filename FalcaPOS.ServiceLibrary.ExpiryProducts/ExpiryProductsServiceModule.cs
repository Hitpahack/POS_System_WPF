using FalcaPOS.Contracts;
using FalcaPOS.ServiceLibrary.ExpiryProducts.Services;
using Prism.Ioc;
using Prism.Modularity;

namespace FalcaPOS.ServiceLibrary.ExpiryProducts
{
    public class ExpiryProductsServiceModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

            containerRegistry.Register<IExpiryProductsService, ExpiryProductService>();
        }
    }
}
