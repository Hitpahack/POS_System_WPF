using FalcaPOS.AddInventory.ViewModels;
using FalcaPOS.AddInventory.Views;
using FalcaPOS.Common.Constants;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System.Linq;

namespace FalcaPOS.AddInventory
{
    public class InventoryModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var _regions = containerProvider.Resolve<IRegionManager>();
            _regions.RegisterViewWithRegion("InventoryHome", typeof(Views.AddInventory));
            _regions.RegisterViewWithRegion("AddStock", typeof(AddStock));
            _regions.RegisterViewWithRegion("ViewStock", typeof(ViewStock));
            _regions.RegisterViewWithRegion("AddProductType", typeof(AddProducttype));
            _regions.RegisterViewWithRegion("AddBrand", typeof(AddBrand));
            _regions.RegisterViewWithRegion("AddProduct", typeof(AddProduct));
            _regions.RegisterViewWithRegion("AddAttribute", typeof(AddAttribute));
            _regions.RegisterViewWithRegion("FlyoutRegion", typeof(StockSearchFlyout));
            _regions.RegisterViewWithRegion("FlyoutRegion", typeof(SupplierEditFlyout));
            _regions.RegisterViewWithRegion("AddSupplier", typeof(AddSupplier));
            _regions.RegisterViewWithRegion("Manage", typeof(Manage));
            _regions.RegisterViewWithRegion("SuppliersList", typeof(SuppliersList));
            _regions.RegisterViewWithRegion("ProductTypeList", typeof(ProductTypeList));
            _regions.RegisterViewWithRegion("ManufacturersList", typeof(ManufacturersList));
            _regions.RegisterViewWithRegion("ProductsList", typeof(ProductsList));
            _regions.RegisterViewWithRegion("Returns", typeof(ReturnStocks));
            _regions.RegisterViewWithRegion("InwardDamageView", typeof(InWardDamageView));
            _regions.RegisterViewWithRegion("ReturnToSupplierView", typeof(ReturnToSupplierView));
            _regions.RegisterViewWithRegion("SalesDefectView", typeof(SalesDefectiveView));
            _regions.RegisterViewWithRegion("CategoryList", typeof(CategoryList));
            _regions.RegisterViewWithRegion("AddCategory", typeof(AddCategory));

            _ = _regions.RegisterViewWithRegion(AppConstants.STATE_LIST_REGIONS, typeof(StateList));
            _ = _regions.RegisterViewWithRegion(AppConstants.DISTRICT_LIST_REGION, typeof(DistrictList));

            if (AppConstants.USER_ROLES != null && AppConstants.USER_ROLES.Contains(AppConstants.ROLE_PURCHASE_MANAGER))
            {
                _regions.RegisterViewWithRegion("InvoiceHome", typeof(Invoice));
            }


        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<BarCodeDialog, BarCodeDialogViewModel>();

            containerRegistry.RegisterDialog<StateDialog, StateDialogViewModel>();

            containerRegistry.RegisterDialog<DistrictDialog, DistrictDialogViewModel>();
            containerRegistry.RegisterDialog<GSTConfirmationPopUp, GSTConfirmationViewModel>();
        }
    }
}
