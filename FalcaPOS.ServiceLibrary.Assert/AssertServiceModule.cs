using FalcaPOS.Contracts;
using FalcaPOS.ServiceLibrary.Assert.Services;
using Prism.Ioc;
using Prism.Modularity;

namespace FalcaPOS.ServiceLibrary.Assert
{
    public class AssertServiceModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

            containerRegistry.Register<IAssertsServices, AssertsServices>();
        }
    }
}
