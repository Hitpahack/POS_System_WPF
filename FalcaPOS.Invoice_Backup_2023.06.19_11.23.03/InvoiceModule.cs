using FalcaPOS.Common.Constants;
using FalcaPOS.Invoice.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace FalcaPOS.Invoice
{
    public class InvoiceModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            IRegionManager _region = containerProvider.Resolve<IRegionManager>();

            _region.RegisterViewWithRegion("Download", typeof(DownloadInvoiceListpopUp));
            _ = _region.RegisterViewWithRegion(AppConstants.REGION_INVOICE_HOMETAB, typeof(InvoiceTab));
            _ = _region.RegisterViewWithRegion(AppConstants.REGION_PURCHASE_INVOICE, typeof(InvoiceHome));
            _ = _region.RegisterViewWithRegion(AppConstants.REGION_PURCHASE_RETURNS, typeof(PurchaseReturns));
            _ = _region.RegisterViewWithRegion(AppConstants.REGION_PURCHASE_RETURNS_VIEW, typeof(PurchaseReturnsView));
            _ = _region.RegisterViewWithRegion(AppConstants.REGION_CREDITNOTE_SUMMARY, typeof(CreditnoteSummary));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<InvoiceTab>();
            containerRegistry.RegisterForNavigation<InvoiceHome>();
            containerRegistry.RegisterForNavigation<InvoiceHome>();

        }
    }
}
