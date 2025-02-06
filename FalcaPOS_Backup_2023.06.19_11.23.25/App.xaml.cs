using AutoUpdaterDotNET;
using FalcaPos.PurchaseManager;
using FalcaPOS.AddInventory.ViewModels;
using FalcaPOS.AddInventory.Views;
using FalcaPOS.Assert;
using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.CustomRegionAdapter;
using FalcaPOS.Common.Logger;
using FalcaPOS.Dashboard;
using FalcaPOS.Denomination;
using FalcaPOS.Denomination.View;
using FalcaPOS.Denomination.ViewModel;
using FalcaPOS.Director;
using FalcaPOS.ExpiryProducts;
using FalcaPOS.Finance;
using FalcaPOS.Indent;
using FalcaPOS.Indent.ViewModels;
using FalcaPOS.Indent.Views;
using FalcaPOS.Invoice;
using FalcaPOS.Invoice.ViewModels;
using FalcaPOS.Invoice.Views;
using FalcaPOS.Notification;
using FalcaPOS.PurchaseReturns;
using FalcaPOS.PurchaseReturns.View;
using FalcaPOS.PurchaseReturns.ViewModel;
using FalcaPOS.RSP;
using FalcaPOS.RSP.View;
using FalcaPOS.RSP.ViewModel;
using FalcaPOS.ServiceLibrary.Assert;
using FalcaPOS.ServiceLibrary.Common;
using FalcaPOS.ServiceLibrary.Customer;
using FalcaPOS.ServiceLibrary.Denomination;
using FalcaPOS.ServiceLibrary.Director;
using FalcaPOS.ServiceLibrary.ExpiryProducts;
using FalcaPOS.ServiceLibrary.Finance;
using FalcaPOS.ServiceLibrary.Indent;
using FalcaPOS.ServiceLibrary.PurchaseInvoice;
using FalcaPOS.ServiceLibrary.PurchaseReturns;
using FalcaPOS.ServiceLibrary.Sales;
using FalcaPOS.ServiceLibrary.Sku;
using FalcaPOS.ServiceLibrary.Team;
using FalcaPOS.Sku;
using FalcaPOS.Stock.ViewModels;
using FalcaPOS.Stock.Views;
using FalcaPOS.Suppliers;
using MahApps.Metro.Controls;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Unity;
using Serilog;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Markup;

namespace FalcaPOS.Shell
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<Views.Shell>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

            _ = containerRegistry.RegisterSingleton<Notify>();

            containerRegistry.RegisterSerilog();

            _ = containerRegistry.Register<Logger>();

            _ = Container.Resolve<Notify>();

        }

        protected override void OnExit(ExitEventArgs e)
        {

            Log.Information("----------Closing app----------");

            Log.CloseAndFlush();

            base.OnExit(e);
        }

        protected override void InitializeModules()
        {
            base.InitializeModules();

            IRegionManager _regionmanager = Container.Resolve<IRegionManager>();

            _ = _regionmanager.RegisterViewWithRegion("FlyoutRegion", typeof(FalcaPOS.Shell.Views.ReleseNotes));
            _ = _regionmanager.RegisterViewWithRegion("FlyoutRegion", typeof(FalcaPOS.Shell.Views.GSTCalculator));
            _ = _regionmanager.RegisterViewWithRegion("FlyoutRegion", typeof(FalcaPOS.Shell.Views.NotificationView));

            RegionManager.GetRegionManager(Application.Current.MainWindow).RequestNavigate("LoginRegion", "FalcaPOS.Login.Views.Login");
        }

        protected override void OnStartup(StartupEventArgs e)
        {

            CultureInfo _cultureInfo = new CultureInfo("en-IN");

            Thread.CurrentThread.CurrentCulture = _cultureInfo;
            Thread.CurrentThread.CurrentUICulture = _cultureInfo;
            CultureInfo.DefaultThreadCurrentCulture = _cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = _cultureInfo;

            FrameworkElement
                .LanguageProperty
                    .OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.Name)));

            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Information()
                            .WriteTo.File(path: @"c:\POSLOGS\POSAppLogs.txt", encoding: Encoding.UTF8, rollingInterval: RollingInterval.Day, fileSizeLimitBytes: (1024 * 1024), rollOnFileSizeLimit: true)
                            .CreateLogger();


            Log.Information("----------Starting app----------");

            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            base.OnStartup(e);

            string _updateURL = string.Empty;

            if (ApplicationSettings.APP_ENVIRONMENT == AppConstants.ENV_PRODUCTION)
            {
                _updateURL = $"{ApplicationSettings.APP_UPDATE_URL}/production/posupdate.xml";
            }
            else
            {
                _updateURL = $"{ApplicationSettings.APP_UPDATE_URL}/testing/posupdate.xml";
            }


            AutoUpdater.AppTitle = "Falca POS Update";

            //check for updates sync
            AutoUpdater.Synchronous = true;

            //force check update for every launch
            AutoUpdater.Mandatory = true;


            //force update app ,not allowing to skip versions(might result in data loss)
            AutoUpdater.UpdateMode = Mode.Forced;

            AutoUpdater.Start(_updateURL, Assembly.GetExecutingAssembly());


            //Theme change
            //try
            //{

            //    var theme = ThemeManager.Current.AddLibraryTheme(
            //   new LibraryTheme(
            //       new Uri("pack://application:,,,/FalcaPOS.Resources;component/Resources/FalcaTheme.xaml"),
            //       MahAppsLibraryThemeProvider.DefaultInstance
            //       )
            //   );

            //    ThemeManager.Current.ChangeTheme(this, theme);
            //}
            //catch (Exception ex)
            //{

            //}
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Error(e?.Exception, "App exception");

            e.Handled = true;
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            ViewModelLocationProvider.Register<ViewStock, AddInventory.ViewModels.StockViewModel>();
            //ViewModelLocationProvider.Register<StockSearchFlyout, StockSearchFlyoutViewModel>();
            ViewModelLocationProvider.Register<AddSupplier, AddSupplierViewModel>();
            ViewModelLocationProvider.Register<Stock.Views.Stock, Stock.ViewModels.StockViewModel>();
            ViewModelLocationProvider.Register<Dashboard.Views.BusinessView, Dashboard.ViewModels.BusinessViewModel>();
            ViewModelLocationProvider.Register<InWardDamageView, InwardDamageViewModel>();
            ViewModelLocationProvider.Register<SalesDefectiveView, SalesDefectiveViewModel>();
            ViewModelLocationProvider.Register<Sku.View.SkuSheet, Sku.ViewModels.SkuSheetViewModel>();
            ViewModelLocationProvider.Register<Sku.View.AddStoreDailyStockReport, Sku.ViewModels.AddStoreDailyStockReportViewModel>();
            ViewModelLocationProvider.Register<Sku.View.DailyStockReport, Sku.ViewModels.DailyStockReportViewModel>();
            ViewModelLocationProvider.Register<Sku.View.ConfrimationPopUp, Sku.ViewModels.ConfirmationViewModel>();
            ViewModelLocationProvider.Register<Director.View.StoreAssert, Director.ViewModel.StoreAsserViewModel>();
            ViewModelLocationProvider.Register<Director.View.PurchaseRate, Director.ViewModel.PurchaseRateViewModel>();
            ViewModelLocationProvider.Register<Director.View.SearchPurchaseRateFlyout, Director.ViewModel.PurchaseRateSearchFlyoutViewModel>();
            ViewModelLocationProvider.Register<ExpiryProducts.View.ExpiryProductsHome, ExpiryProducts.ViewModel.ExpiryProductsViewModel>();
            ViewModelLocationProvider.Register<ExpiryProducts.View.CurrentMonthExpiryProducts, ExpiryProducts.ViewModel.CurrentMonthViewModel>();
            ViewModelLocationProvider.Register<ExpiryProducts.View.Next3MonthExpiryProducts, ExpiryProducts.ViewModel.Next3MonthViewModel>();
            ViewModelLocationProvider.Register<ExpiryProducts.View.Next6MonthExpiryProducts, ExpiryProducts.ViewModel.Next6MonthViewModel>();
            ViewModelLocationProvider.Register<ExpiryProducts.View.NextMonthExpiryProducts, ExpiryProducts.ViewModel.NextMonthViewModel>();
            ViewModelLocationProvider.Register<ExpiryProducts.View.ExpiredProduct, ExpiryProducts.ViewModel.ExpriedProductViewModel>();
            ViewModelLocationProvider.Register<Indent.Views.IntentPopupStatusChange, Indent.ViewModels.IndentApprovalViewModel>();
            ViewModelLocationProvider.Register<Denomination.View.Denomination, Denomination.ViewModel.DenominationViewModel>();
            ViewModelLocationProvider.Register<Denomination.View.NewDenominationFinanceView, Denomination.ViewModel.NewDenominationFinanceViewModel>();
            ViewModelLocationProvider.Register<Indent.Views.Confimationpopup, Indent.ViewModels.ConfirmationViewModel>();
            ViewModelLocationProvider.Register<Finance.Views.TallyExport, Finance.ViewModels.TallyExportViewModel>();
            ViewModelLocationProvider.Register<Suppliers.Views.SupplierHome, Suppliers.ViewModels.SupplierHomeViewModel>();
            ViewModelLocationProvider.Register<Sales.Views.AppOrdersView, Sales.ViewModels.AppOrderViewModel>();
            ViewModelLocationProvider.Register<Sales.Views.AppOrderProductList, Sales.ViewModels.OrderedProductspopupViewModel>();
            ViewModelLocationProvider.Register<Stock.Views.StockApproval, Stock.ViewModels.StockApprovalViewModel>();
            ViewModelLocationProvider.Register<Stock.Views.StockTransferHome, Stock.ViewModels.StockTransferHome>();
            ViewModelLocationProvider.Register<Stock.Views.Stockcompleted, Stock.ViewModels.StockCompletedViewModel>();
            ViewModelLocationProvider.Register<Stock.Views.StockHome, Stock.ViewModels.StockHome>();
            ViewModelLocationProvider.Register<Stock.Views.StockTransferSearch, Stock.ViewModels.StockTransferSearchViewModel>();
            ViewModelLocationProvider.Register<DownloadInvoiceListpopUp, DownloadInvoicePopupViewModel>();
            ViewModelLocationProvider.Register<Denomination.View.StoreDenominationView, Denomination.ViewModel.StoreDenominationViewModel>();
            ViewModelLocationProvider.Register<Sku.View.AddSKU, Sku.ViewModels.AddSKUViewModel>();
            ViewModelLocationProvider.Register<Sku.View.ViewSKU, Sku.ViewModels.ViewSKUViewModel>();
            ViewModelLocationProvider.Register<Sku.View.AlterSKU, Sku.ViewModels.AlterSKUViewModel>();
            ViewModelLocationProvider.Register<Sku.View.ApproveSKU, Sku.ViewModels.ApproveSKUViewModel>();
            ViewModelLocationProvider.Register<Sku.View.HomeSKU, Sku.ViewModels.HomeSKUViewModel>();
            ViewModelLocationProvider.Register<Invoice.Views.PurchaseReturns, Invoice.ViewModels.PurchaseReturnsViewModel>();
            ViewModelLocationProvider.Register<StoreReturns, StoreReturnsViewModel>();
            ViewModelLocationProvider.Register<PurchaseReturnsView, PurhcaseReturnsViewViewModel>();
            ViewModelLocationProvider.Register<StoreViewReturn, StoreViewReturnViewModel>();
            ViewModelLocationProvider.Register<UpdateCreditNoteNumber, UpdateCreditNoteNumberViewModel>();
            ViewModelLocationProvider.Register<BulkDownload, BulkDownloadViewModel>();
            ViewModelLocationProvider.Register<BulkUpload, BulkUploadViewModel>();
            ViewModelLocationProvider.Register<CreditnoteSummary, CreditNoteSummaryViewModel>();
            ViewModelLocationProvider.Register<RspStores, RspStoresViewModel>();
            ViewModelLocationProvider.Register<RSP.View.Sales, RspSalesViewModel>();
            ViewModelLocationProvider.Register<RSP.View.Stock, RspStockViewModel>();
            ViewModelLocationProvider.Register<RSP.View.Summary, RspSummaryViewModel>();
            ViewModelLocationProvider.Register<InvoiceTab, InvoiceHomeTabViewModel>();
            ViewModelLocationProvider.Register<DepositView, DepositViewModel>();
            ViewModelLocationProvider.Register<SellingPriceUpdate, SellingPriceUpdateViewModel>();
            ViewModelLocationProvider.Register<Assert.View.Asserts, Assert.ViewModel.AssertsViewModel>();
            ViewModelLocationProvider.Register<Assert.View.AssertSearchFlyout, Assert.ViewModel.AssertSearchFlyoutViewModel>();
            ViewModelLocationProvider.Register<Login.Views.LoginTime, Login.ViewModels.LoginTimeViewModel>();
            ViewModelLocationProvider.Register<PurchaseReturns.View.PurchaseReturnHome, PurchaseReturns.ViewModel.PurchaseReturnHomeViewModel>();


        }


        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
        {
            base.ConfigureRegionAdapterMappings(regionAdapterMappings);

            regionAdapterMappings.RegisterMapping(typeof(FlyoutsControl), Container.Resolve<FlyoutsControlRegionAdapter>());

        }
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {

            base.ConfigureModuleCatalog(moduleCatalog);

            _ = moduleCatalog.AddModule<CommonServiceModule>();

            _ = moduleCatalog.AddModule<ServiceLibrary.Login.LoginServiceModule>();
            _ = moduleCatalog.AddModule<ServiceLibrary.AddInventory.AddInventoryServiceModule>();
            _ = moduleCatalog.AddModule<SalesServiceModule>();
            _ = moduleCatalog.AddModule<TeamServiceModule>();

            _ = moduleCatalog.AddModule<CustomerServiceModule>();
            _ = moduleCatalog.AddModule<ServiceLibrary.Stock.StockServiceModule>();
            _ = moduleCatalog.AddModule<ServiceLibrary.StockAge.StockAgeServiceModule>();

            _ = moduleCatalog.AddModule<Login.LoginModule>();

            //Register Finance Service Module
            _ = moduleCatalog.AddModule<FinanceServiceModule>();

            //Register Indent Service module
            _ = moduleCatalog.AddModule<IndentServiceModule>();

            //Purchase invoice service module
            _ = moduleCatalog.AddModule<PurchaseInvoiceServiceModule>();

            //sku service module
            _ = moduleCatalog.AddModule<SkuServiceModule>();

            //director service module
            _ = moduleCatalog.AddModule<DirectorServiceModule>();

            _ = moduleCatalog.AddModule<ExpiryProductsServiceModule>();

            _ = moduleCatalog.AddModule<DenominationServiceModule>();

            _ = moduleCatalog.AddModule<PurchseReturnsServiceModule>();

            _ = moduleCatalog.AddModule<AssertServiceModule>();

            _ = moduleCatalog.AddModule<Home.HomeModule>("HomeModule", InitializationMode.OnDemand);
            _ = moduleCatalog.AddModule<Sales.SalesModule>("SalesModule", InitializationMode.OnDemand);
            _ = moduleCatalog.AddModule<Customer.CustomerModule>("CustomerModule", InitializationMode.OnDemand);
            _ = moduleCatalog.AddModule<Stock.StockModule>("StockModule", InitializationMode.OnDemand);
            _ = moduleCatalog.AddModule<AddInventory.InventoryModule>("InventoryModule", InitializationMode.OnDemand);
            _ = moduleCatalog.AddModule<Team.TeamModule>("TeamModule", InitializationMode.OnDemand);
            _ = moduleCatalog.AddModule<Store.StoreModule>("StoreModule", InitializationMode.OnDemand);
            _ = moduleCatalog.AddModule<DashboardModule>(nameof(DashboardModule), InitializationMode.OnDemand);

            //Register Finance Module FinanceModule
            _ = moduleCatalog.AddModule<FinanceModule>(nameof(FinanceModule), InitializationMode.OnDemand);
            _ = moduleCatalog.AddModule<InvoiceModule>(nameof(InvoiceModule), InitializationMode.OnDemand);
            _ = moduleCatalog.AddModule<SuppliersModule>(nameof(SuppliersModule), InitializationMode.OnDemand);
            _ = moduleCatalog.AddModule<StockAge.StockAgeModule>("StockAgeModule", InitializationMode.OnDemand);


            //Register SKU Module 
            _ = moduleCatalog.AddModule<SkuModule>(nameof(SkuModule), InitializationMode.OnDemand);

            //Director Module

            _ = moduleCatalog.AddModule<DirectorModule>(nameof(DirectorModule), InitializationMode.OnDemand);

            //manager module

            _ = moduleCatalog.AddModule<PurchaseManagerModule>(nameof(PurchaseManagerModule), InitializationMode.OnDemand);

            _ = moduleCatalog.AddModule<IndentModule>(nameof(IndentModule), InitializationMode.OnDemand);

            _ = moduleCatalog.AddModule<ExpiryProductsModule>(nameof(ExpiryProductsModule), InitializationMode.OnDemand);

            _ = moduleCatalog.AddModule<DenominationModule>(nameof(DenominationModule), InitializationMode.OnDemand);

            _ = moduleCatalog.AddModule<PurchaseReturnsModule>(nameof(PurchaseReturnsModule), InitializationMode.OnDemand);

            _ = moduleCatalog.AddModule<RSPModule>(nameof(RSPModule), InitializationMode.OnDemand);

            _ = moduleCatalog.AddModule<AssertModule>(nameof(AssertModule), InitializationMode.OnDemand);


            //moduleCatalog.AddModule<Home.HomeModule>();
            //moduleCatalog.AddModule<Sales.SalesModule>();
            //moduleCatalog.AddModule<Customer.CustomerModule>();
            //moduleCatalog.AddModule<Stock.StockModule>();
            //moduleCatalog.AddModule<AddInventory.InventoryModule>();


        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            //_regionmanager.RegisterViewWithRegion("ViewStock", typeof(AddInventory.Views.ViewStock));
        }


    }
}
