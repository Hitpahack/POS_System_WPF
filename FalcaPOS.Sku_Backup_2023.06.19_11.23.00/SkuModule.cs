using FalcaPOS.Common.Constants;
using FalcaPOS.Sku.View;
using FalcaPOS.Sku.ViewModels;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace FalcaPOS.Sku
{
    public class SkuModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

            //register view with region.

            IRegionManager _region = containerProvider.Resolve<IRegionManager>();

            _ = _region.RegisterViewWithRegion(AppConstants.REGION_DAILY_STOCK_REPORT, typeof(DailyStockReport));

            _ = _region.RegisterViewWithRegion(AppConstants.REGION_ADD_STORE_DAILY_STOCK, typeof(AddStoreDailyStockReport));

            _ = _region.RegisterViewWithRegion(AppConstants.REGION_SKU_SHEET, typeof(SkuSheet));



            _ = _region.RegisterViewWithRegion(AppConstants.REGION_SKU_HOME, typeof(HomeSKU));

            _ = _region.RegisterViewWithRegion(AppConstants.REGION_SKU_CREATE, typeof(AddSKU));

            _ = _region.RegisterViewWithRegion(AppConstants.REGION_SKU_VIEW, typeof(ViewSKU));
            _ = _region.RegisterViewWithRegion(AppConstants.REGION_SKU_ALTER, typeof(AlterSKU));
            _ = _region.RegisterViewWithRegion(AppConstants.REGION_SKU_APPROVE, typeof(ApproveSKU));




        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<ConfrimationPopUp, ConfirmationViewModel>();
        }
    }
}
