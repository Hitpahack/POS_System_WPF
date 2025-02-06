using FalcaPOS.Contracts;
using FalcaPOS.ServiceLibrary.Finance.Services;
using Prism.Ioc;
using Prism.Modularity;

namespace FalcaPOS.ServiceLibrary.Finance
{
    public class FinanceServiceModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

            containerRegistry.Register<IFinanceService, FinanceService>();
        }
    }
}
