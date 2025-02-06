using FalcaPOS.Common.Constants;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System.Linq;

namespace FalcaPOS.Denomination
{
    public class DenominationModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

            var _regions = containerProvider.Resolve<IRegionManager>();
            if (AppConstants.USER_ROLES != null && AppConstants.USER_ROLES.Contains(AppConstants.ROLE_STORE_PERSON))
            {
                //only store person view
                _regions.RegisterViewWithRegion("Denomination", typeof(Denomination.View.DenominationHome));
                _regions.RegisterViewWithRegion("Deposit", typeof(Denomination.View.Deposit));
                _regions.RegisterViewWithRegion("DenominationAdd", typeof(Denomination.View.Denomination));
                _regions.RegisterViewWithRegion("StoreDenominationView", typeof(Denomination.View.StoreDenominationView));
                _regions.RegisterViewWithRegion("DepositView", typeof(Denomination.View.DepositView));

            }
            else if (AppConstants.USER_ROLES != null && AppConstants.USER_ROLES.Contains(AppConstants.ROLE_FINANCE))
            {
                //only finance login view
                _regions.RegisterViewWithRegion("Denomination", typeof(Denomination.View.DenominationHome));
                _regions.RegisterViewWithRegion("FinanceDenominationView", typeof(Denomination.View.NewDenominationFinanceView));
                _regions.RegisterViewWithRegion("Deposit", typeof(Denomination.View.Deposit));
                _regions.RegisterViewWithRegion("DepositView", typeof(Denomination.View.DepositView));
            }


        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
