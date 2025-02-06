using FalcaPOS.Contracts;
using Prism.Ioc;
using Prism.Modularity;

namespace FalcaPOS.ServiceLibrary.Reports
{
    public class ReportsServicesModule:IModule
    {
        public void OnInitialized(IContainerProvider containerProvider) {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry) {

            _ = containerRegistry.RegisterSingleton<IReportServices, ReportServices>();
        }
    }
}
