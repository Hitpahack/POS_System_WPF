using FalcaPOS.Contracts;
using FalcaPOS.ServiceLibrary.Login.Service;
using Prism.Ioc;
using Prism.Modularity;

namespace FalcaPOS.ServiceLibrary.Login
{
    public class LoginServiceModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ILoginService, LoginService>();
        }
    }
}
