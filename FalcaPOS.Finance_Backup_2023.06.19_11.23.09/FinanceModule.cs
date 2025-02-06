using FalcaPOS.Common.Constants;
using FalcaPOS.Finance.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System.Linq;

namespace FalcaPOS.Finance
{
    public class FinanceModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

            //register view with region.

            IRegionManager _region = containerProvider.Resolve<IRegionManager>();

            _ = _region.RegisterViewWithRegion(AppConstants.REGION_FINANCE_HOME, typeof(FinanceHome));

            _ = _region.RegisterViewWithRegion(AppConstants.REGION_FLYOUT, typeof(FinanceSearchFlyout));

            _ = _region.RegisterViewWithRegion(AppConstants.REGION_CLOSING_STOCK, typeof(ClosingStock));
            if (AppConstants.USER_ROLES != null && AppConstants.USER_ROLES.Contains(AppConstants.ROLE_FINANCE))
            {
                _ = _region.RegisterViewWithRegion(AppConstants.REGION_TALLY_EXPORT, typeof(TallyExport));
            }



        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
