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
                _regions.RegisterViewWithRegion("AddDenominationCount", typeof(Denomination.View.AddDenominationCountView));
                _regions.RegisterViewWithRegion("DenominationAndDepositsDetailsView", typeof(View.DenominationDepositDetailsView));

            }
            else if (AppConstants.USER_ROLES != null &&( AppConstants.USER_ROLES.Contains(AppConstants.ROLE_FINANCE)|| AppConstants.USER_ROLES.Contains(AppConstants.ROLE_CONTROL_MANAGER) || AppConstants.USER_ROLES.Contains(AppConstants.ROLE_DIRECTOR)))
            {
                //only finance login view
                //_regions.RegisterViewWithRegion("DenominationAndDepositsDetailsView", typeof(View.DenominationDepositDetailsView));
                //_regions.RegisterViewWithRegion("Denomination", typeof(Denomination.View.DenominationHome));
                _regions.RegisterViewWithRegion("FinanceDenominationView", typeof(Denomination.View.NewDenominationFinanceView));
                _regions.RegisterViewWithRegion("EODCashFinanceDenominationView", typeof(Denomination.View.EODCashDeclarationFinanceView));
                _regions.RegisterViewWithRegion("Deposit", typeof(Denomination.View.Deposit));
                _regions.RegisterViewWithRegion("DepositView", typeof(Denomination.View.DepositView));
                _regions.RegisterViewWithRegion("AddDenominationCount", typeof(Denomination.View.AddDenominationCountView));

            }


        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
