using FalcaPOS.Contracts;
using FalcaPOS.ServiceLibrary.Denomination.Services;
using Prism.Ioc;
using Prism.Modularity;

namespace FalcaPOS.ServiceLibrary.Denomination
{
    public class DenominationServiceModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IDenominationService, DenominationServices>();
        }
    }
}
