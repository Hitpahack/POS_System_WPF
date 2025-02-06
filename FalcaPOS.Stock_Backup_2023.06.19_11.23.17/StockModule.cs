using FalcaPOS.Common.Constants;
using FalcaPOS.Stock.ViewModels;
using FalcaPOS.Stock.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System.Linq;

namespace FalcaPOS.Stock
{
    public class StockModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var _regions = containerProvider.Resolve<IRegionManager>();
            _regions.RegisterViewWithRegion("StockHome", typeof(Views.StockHome));
            _regions.RegisterViewWithRegion("Stockv2Home", typeof(StockDetails));
            _regions.RegisterViewWithRegion("FlyoutRegion", typeof(StoreStockSearchFlyout));
            _regions.RegisterViewWithRegion("StockRegion", typeof(Views.Stock));
            _regions.RegisterViewWithRegion("StockTransferSearch", typeof(Views.StockTransferSearch));
            _regions.RegisterViewWithRegion("StockTransferHomeV2", typeof(Views.StockTransferHomeV2));

            if (AppConstants.USER_ROLES != null && (AppConstants.USER_ROLES.Contains(AppConstants.ROLE_BACKEND) || AppConstants.USER_ROLES.Contains(AppConstants.ROLE_PURCHASE_MANAGER)))
            {
                _regions.RegisterViewWithRegion("InvoicesList", typeof(InvoicesList));
                _regions.RegisterViewWithRegion("StockTransferHome", typeof(Views.StockTransferHome));
                _regions.RegisterViewWithRegion("StockTransferRegion", typeof(Views.StockTransfer));
                _regions.RegisterViewWithRegion("StockReceiverRegion", typeof(Views.StockReceiver));
                _regions.RegisterViewWithRegion("StockcompletedRegion", typeof(Views.Stockcompleted));
                _regions.RegisterViewWithRegion("StockApprovalRegion", typeof(Views.StockApproval));
                _regions.RegisterViewWithRegion("StockReuestRegion", typeof(Views.StockRequest));
                _regions.RegisterViewWithRegion("RSPRequestRegion", typeof(Views.RspRequest));
                _regions.RegisterViewWithRegion("SellingPriceUpdateRegion", typeof(Views.SellingPriceUpdate));

            }
            //if (AppConstants.USER_ROLES != null && (AppConstants.USER_ROLES.Contains(AppConstants.ROLE_DIRECTOR) || AppConstants.USER_ROLES.Contains(AppConstants.ROLE_PURCHASE_MANAGER)))
            //{
            //    _regions.RegisterViewWithRegion("StockcompletedRegion", typeof(Views.Stockcompleted));
            //    _regions.RegisterViewWithRegion("StockApprovalRegion", typeof(Views.StockApproval));
            //}


        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<UpdateInvoiceDialog, UpdateInvoiceDialogViewModel>();
        }
    }
}
