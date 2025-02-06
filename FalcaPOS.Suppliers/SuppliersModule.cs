using FalcaPOS.AddInventory.Views;
using FalcaPOS.Suppliers.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace FalcaPOS.Suppliers
{
    public class SuppliersModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var _regionmanager = containerProvider.Resolve<IRegionManager>();
            _regionmanager.RegisterViewWithRegion("MasterSuppliersRegion", typeof(Views.SupplierHome));
            _regionmanager.RegisterViewWithRegion("SupplierListRegion", typeof(Views.SupplierList));
            _regionmanager.RegisterViewWithRegion("SupplierDetailsRegion", typeof(Views.SupplierDetails));
            _regionmanager.RegisterViewWithRegion("AddSupplier", typeof(AddSupplier));
            _regionmanager.RegisterViewWithRegion("AddAddress", typeof(AddAddress));
            _regionmanager.RegisterViewWithRegion("AddNewBank", typeof(AddNewBank));
            _regionmanager.RegisterViewWithRegion("AddNewBankDetails", typeof(AddNewBankDetails));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
