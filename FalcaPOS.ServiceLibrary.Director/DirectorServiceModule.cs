using FalcaPOS.Contracts;
using FalcaPOS.ServiceLibrary.Director.Services;
using Prism.Ioc;
using Prism.Modularity;

namespace FalcaPOS.ServiceLibrary.Director
{
    public class DirectorServiceModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

            containerRegistry.Register<IDirectorService, DirectorService>();
        }
    }
}
