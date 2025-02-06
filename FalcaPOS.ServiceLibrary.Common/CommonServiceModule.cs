using FalcaPOS.Common.Helper;
using FalcaPOS.Contracts;
using FalcaPOS.ServiceLibrary.Common.Services;
using Prism.Ioc;
using Prism.Modularity;

namespace FalcaPOS.ServiceLibrary.Common
{
    public class CommonServiceModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ICommonService, CommonService>();

            containerRegistry.RegisterSingleton<INotificationService, NotificationService>();

            containerRegistry.RegisterSingleton<ProgressService>();

            containerRegistry.Register<ISignalRService, SignalRService>();

            containerRegistry.RegisterSingleton<IPrinterService, PrinterService>();

            containerRegistry.RegisterSingleton<IInvoiceFileService, InvoiceFileService>();
        }
    }
}
