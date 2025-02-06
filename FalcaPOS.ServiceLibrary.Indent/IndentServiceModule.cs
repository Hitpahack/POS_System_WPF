using FalcaPOS.Contracts;
using FalcaPOS.ServiceLibrary.Indent.Service;
using Prism.Ioc;
using Prism.Modularity;

namespace FalcaPOS.ServiceLibrary.Indent
{
    public class IndentServiceModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IIndentService, IndentService>();
        }
    }
}
