using FalcaPOS.AddInventory.Views;
using FalcaPOS.Common.Constants;
using FalcaPOS.Indent.ViewModels;
using FalcaPOS.Indent.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace FalcaPOS.Indent
{
    public class IndentModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            IRegionManager _region = containerProvider.Resolve<IRegionManager>();

            _ = _region.RegisterViewWithRegion(AppConstants.REGION_INDENT_HOME, typeof(IndentHome));
            _ = _region.RegisterViewWithRegion(AppConstants.REGION_INDENT_CREATE, typeof(IndentCreate));
            _ = _region.RegisterViewWithRegion(AppConstants.REGION_INDENT_VIEW, typeof(IndentList));

            _ = _region.RegisterViewWithRegion(AppConstants.REGION_INDENT_APPROVAL, typeof(IndentApproval));

            _ = _region.RegisterViewWithRegion("AddSupplier", typeof(AddSupplier));

            _ = _region.RegisterViewWithRegion(AppConstants.REGION_FLYOUT, typeof(IndentListFlyout));
            _ = _region.RegisterViewWithRegion(AppConstants.REGION_FLYOUT, typeof(IndentStatusFlyout));


            _ = _region.RegisterViewWithRegion(AppConstants.REGION_INDENT_BULK_PAYMENTUPDATE, typeof(IndentBulkPaymentHome));

            _ = _region.RegisterViewWithRegion(AppConstants.REGION_INDENT_BULK_DOWNLOAD, typeof(BulkDownload));

            _ = _region.RegisterViewWithRegion(AppConstants.REGION_INDENT_BULK_UPLOAD, typeof(BulkUpload));

            //_region.RequestNavigate(AppConstants.REGION_INDENT_HOME, nameof(IndentHome));

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<IndentHome>();
            containerRegistry.RegisterForNavigation<IndentCreate>();
            containerRegistry.RegisterForNavigation<IndentList>();
            containerRegistry.RegisterForNavigation<IndentApproval>();
            containerRegistry.RegisterDialog<Confimationpopup, ConfirmationViewModel>();
        }

    }
}
