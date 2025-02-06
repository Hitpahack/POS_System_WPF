using FalcaPOS.Common.Constants;
using FalcaPOS.PurchaseReturns.View;
using FalcaPOS.PurchaseReturns.ViewModel;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace FalcaPOS.PurchaseReturns
{
    public class PurchaseReturnsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var _regions = containerProvider.Resolve<IRegionManager>();

            _ = _regions.RegisterViewWithRegion(AppConstants.REGION_STORE_PURCHASE_RETURNS, typeof(StoreReturns));

            _ = _regions.RegisterViewWithRegion(AppConstants.REGION_STORE_PURCHASE_RETURNS_HOME, typeof(PurchaseReturnHome));

            _ = _regions.RegisterViewWithRegion(AppConstants.REGION_STORE_PURCHASE_RETURNS_VIEW, typeof(StoreViewReturn));

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<UpdateCreditNoteNumber, UpdateCreditNoteNumberViewModel>();
        }
    }
}
