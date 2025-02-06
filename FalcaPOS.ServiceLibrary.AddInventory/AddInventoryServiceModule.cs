using FalcaPOS.Contracts;
using FalcaPOS.ServiceLibrary.AddInventory.Services;
using Prism.Ioc;
using Prism.Modularity;

namespace FalcaPOS.ServiceLibrary.AddInventory
{
    public class AddInventoryServiceModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ISupplierService, SupplierService>();

            containerRegistry.Register<IAttributeTypeService, AttributeTypeService>();

            containerRegistry.Register<IProductTypeService, ProductTypeService>();

            containerRegistry.Register<IStoreService, StoreService>();


            containerRegistry.Register<IProductService, ProductService>();

            containerRegistry.Register<IStockService, StockServices>();

            containerRegistry.Register<IInvoiceService, InvoiceService>();

            containerRegistry.Register<IBrandService, BrandService>();

            //containerRegistry.Register<IInvoiceGenerateService, InvoiceGenerateService>();



        }
    }
}
