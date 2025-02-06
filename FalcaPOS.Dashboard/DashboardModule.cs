using FalcaPOS.Dashboard.Services;
using FalcaPOS.Dashboard.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace FalcaPOS.Dashboard
{
    public class DashboardModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            //regionManager.RegisterViewWithRegion("DashBoardHome", typeof(DashboardView));
            //regionManager.RegisterViewWithRegion("SalesByRegionChart", typeof(SalesByRegionView));
            //regionManager.RegisterViewWithRegion("TotalSalesChart", typeof(TotalSalesView));
            //regionManager.RegisterViewWithRegion("PieChartRegion", typeof(PieChartView));
            //regionManager.RegisterViewWithRegion("DoughnutChartRegion", typeof(DoughnutChartView));
            regionManager.RegisterViewWithRegion("Mis", typeof(BusinessView));
            //regionManager.RegisterViewWithRegion("CustomerByStoreRegion", typeof(CustomerByStoreView));
            //regionManager.RegisterViewWithRegion("SupplierByStore", typeof(SupplierByStoreView));
            //regionManager.RegisterViewWithRegion("SalesByMonth", typeof(SalesByMonth));
            regionManager.RegisterViewWithRegion("CreditSalesView", typeof(CreditSalesView));
            regionManager.RegisterViewWithRegion("BusinessHome", typeof(BusinessHome));


        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IChartDataService, ChartDataService>();
            containerRegistry.Register<ICustomerByStoreService, CustomerByStoreService>();
        }
    }
}
