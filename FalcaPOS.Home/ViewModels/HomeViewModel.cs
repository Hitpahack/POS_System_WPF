using ControlzEx.Standard;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using Prism.Events;
using Prism.Mvvm;

namespace FalcaPOS.Home.ViewModels
{
    public class HomeViewModel : BindableBase
    {

        private bool _isAdmin;
        private bool _isBackend;
        private bool _isStore;
        private bool _isSuperBackend;
        private bool _isDailySales;
        private IEventAggregator _eventAggregator;
        private bool _isDirector;
        private bool _isDailyStockReport;

        private bool _isClosingStockVisible;
        private bool _isDenomination;
        private bool _isTeam;
        private bool _isTally;
        private bool _isSKU;
        private bool _isAssert;
        private bool _isExpiry;
        private bool _isLoginTime;
        private bool _isInventoryReport;
        private bool _isSalesReport;

        /// <summary>
        /// Gets and Sets the visibility of the Closing Stock menu.
        /// </summary>
        public bool IsClosingStockVisible
        {
            get { return _isClosingStockVisible; }
            set { SetProperty(ref _isClosingStockVisible, value); }
        }

        /// <summary>
        /// Stores the boolean value of the visibility of Closing Stock menu.
        /// </summary>
        private bool _isStocksVisible;
        public bool IsStocksVisible
        {
            get { return _isStocksVisible; }
            set { SetProperty(ref _isStocksVisible, value); }
        }
        private bool _isStockTransferVisible;
        public bool IsStockTransferVisible
        {
            get { return _isStockTransferVisible; }
            set { SetProperty(ref _isStockTransferVisible, value); }
        }

        private bool _isIndent;
        public bool IsIndent
        {
            get { return _isIndent; }
            set { SetProperty(ref _isIndent, value); }
        }
        public bool IsAdmin
        {
            get { return _isAdmin; }
            set { SetProperty(ref _isAdmin, value); }
        }

        private bool _isPurchaseInvoice;
        public bool IsPurchaseInvoice
        {
            get { return _isPurchaseInvoice; }
            set { SetProperty(ref _isPurchaseInvoice, value); }
        }


        private bool _isFinance;
        public bool IsFinance
        {
            get { return _isFinance; }
            set { SetProperty(ref _isFinance, value); }
        }

        private bool _isSuppliers;

        /// <summary>
        /// Gets and sets the value of the boolean, IsSuppliers.
        /// </summary>
        public bool IsSuppliers
        {
            get { return _isSuppliers; }
            set { SetProperty(ref _isSuppliers, value); }
        }
        public bool IsBackend
        {
            get { return _isBackend; }
            set { SetProperty(ref _isBackend, value); }
        }

        public bool IsStore
        {
            get { return _isStore; }
            set { SetProperty(ref _isStore, value); }
        }
        public bool IsSuperBackend
        {
            get { return _isSuperBackend; }
            set { SetProperty(ref _isSuperBackend, value); }
        }

        private int _slectedMenuIndex;

        public int SelectedMenuIndex
        {
            get { return _slectedMenuIndex; }
            set { SetProperty(ref _slectedMenuIndex, value); isflyoutclose(); }
        }

        public bool IsDailySales
        {
            get { return _isDailySales; }
            set { SetProperty(ref _isDailySales, value); }
        }

        public bool IsDirector
        {
            get { return _isDirector; }
            set { SetProperty(ref _isDirector, value); }
        }

        public bool IsDailyStockReport
        {
            get { return _isDailyStockReport; }
            set { SetProperty(ref _isDailyStockReport, value); }
        }

        public bool IsDenomination
        {
            get { return _isDenomination; }
            set { SetProperty(ref _isDenomination, value); }
        }

        public bool IsTeam
        {
            get { return _isTeam; }
            set { SetProperty(ref _isTeam, value); }
        }

        public bool IsTally
        {
            get { return _isTally; }
            set { SetProperty(ref _isTally, value); }
        }

        public bool IsSKU
        {
            get { return _isSKU; }
            set { SetProperty(ref _isSKU, value); }
        }

        public bool IsAssert
        {
            get { return _isAssert; }
            set { SetProperty(ref _isAssert, value); }
        }

        public bool IsExpiry
        {
            get { return _isExpiry; }
            set { SetProperty(ref _isExpiry, value); }
        }

        public bool IsLoginTime
        {
            get => _isLoginTime;
            set => SetProperty(ref _isLoginTime, value);
        }

        public bool IsInventoryReport {
            get => _isInventoryReport;
            set=> SetProperty(ref _isInventoryReport, value);
        }

        public bool IsSalesReport {
            get => _isSalesReport;
            set => SetProperty(ref _isSalesReport, value);
        }

        public HomeViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            IsAdmin = IsBackend = IsStore = IsFinance = IsIndent = IsSKU = IsInventoryReport= IsSalesReport= false;
            _eventAggregator.GetEvent<FalcaPOS.Common.Events.LoginedRole>().Subscribe((s) =>
            {
                IsAdmin = IsBackend = IsStore = IsIndent = false;

                if (!string.IsNullOrEmpty(s))
                    switch (s)
                    {
                        case AppConstants.ROLE_ADMIN:
                            IsAdmin = true;
                            SelectedMenuIndex = (int)StartMenu.DashBoard;
                            IsStocksVisible = true;
                            IsFinance = false;
                            IsDailySales = true;
                            IsIndent = true;
                            IsClosingStockVisible = true;
                            IsPurchaseInvoice = true;
                            IsDailyStockReport = true;
                            IsTeam = true;
                            IsSKU = true;
                            IsExpiry = true;
                            IsSalesReport = false;
                            IsSuppliers = false;
                            break;
                        case AppConstants.ROLE_BACKEND:
                            IsBackend = true;
                            IsStocksVisible = true;
                            SelectedMenuIndex = (int)StartMenu.Inventory;
                            IsIndent = true;
                            IsPurchaseInvoice = true;
                            IsTeam = true;
                            IsExpiry = true;
                            IsStockTransferVisible = true;
                            IsSuppliers = false;
                            IsInventoryReport = true;
                            break;

                        case AppConstants.ROLE_STORE_PERSON:
                            IsStore = true;
                            IsStocksVisible = true;
                            SelectedMenuIndex = (int)StartMenu.Sales;
                            IsIndent = true;
                            IsDenomination = true;
                            IsTeam = true;
                            IsSKU = true;
                            IsExpiry = true;
                            IsFinance = false;
                            IsInventoryReport = true;
                            IsSuppliers = false;
                            IsAssert = true;
                            break;

                        case AppConstants.ROLE_SUPER_BACKEND:
                            IsStore = false;
                            IsSuperBackend = true;
                            IsStocksVisible = true;
                            SelectedMenuIndex = 1;
                            IsIndent = true;
                            IsPurchaseInvoice = true;
                            IsTeam = true;
                            IsExpiry = true;
                            IsSuppliers = false;
                            break;
                        case AppConstants.ROLE_CONTROL_MANAGER:
                            IsInventoryReport = IsStockTransferVisible = IsExpiry = IsAssert = IsIndent = IsDenomination = IsDailyStockReport = IsPurchaseInvoice = IsClosingStockVisible = IsDailySales = IsStocksVisible = IsFinance = IsSalesReport= true;
                            SelectedMenuIndex = (int)StartMenu.FinanceSales;
                            IsTeam = IsBackend = IsSuperBackend = IsAdmin = IsStore = IsTally= IsSuppliers =  false;
                            break;
                        case AppConstants.ROLE_FINANCE:
                           IsStockTransferVisible= IsExpiry = IsAssert  =IsIndent = IsDenomination = IsDailyStockReport =IsPurchaseInvoice = IsClosingStockVisible = IsDailySales =IsStocksVisible = IsFinance = IsSalesReport= IsInventoryReport=IsSuppliers= IsSKU= true;
                            SelectedMenuIndex = (int)StartMenu.FinanceSales;
                            IsTeam= IsBackend = IsSuperBackend = IsAdmin = IsStore = false;
                            break;

                        case AppConstants.ROLE_DIRECTOR:
                            IsInventoryReport= IsExpiry = IsTeam = IsIndent = IsDailyStockReport = IsDirector = IsClosingStockVisible = IsFinance = IsSalesReport= true;
                            IsPurchaseInvoice= IsDailySales = IsStocksVisible = IsBackend = IsSuperBackend = IsAdmin = IsDailySales = IsPurchaseInvoice = IsStore = IsSuppliers = false;
                            break;
                        case AppConstants.ROLE_PURCHASE_MANAGER:
                            IsFinance = IsStore= IsAdmin= IsSuperBackend= IsBackend= IsDailySales= IsDirector=IsClosingStockVisible= IsDailyStockReport= IsSalesReport= IsSuppliers = false;
                            IsPurchaseInvoice= IsIndent = IsTeam= IsSKU= IsExpiry= IsLoginTime=IsInventoryReport= IsStocksVisible=true;                           
                            break;
                        case AppConstants.ROLE_AUDITOR:
                            IsFinance = false;
                            IsStore = false;
                            IsAdmin = false;
                            IsSuperBackend = false;
                            IsBackend = false;
                            IsStocksVisible = false;
                            IsDailySales = false;
                            IsClosingStockVisible = true;
                            IsPurchaseInvoice = false;
                            IsDirector = false;
                            IsDailyStockReport = false;
                            IsIndent = false;
                            IsTeam = true;
                            IsSKU = false;
                            IsTally = false;
                            IsAssert = true;
                            IsExpiry = false;
                            IsSuppliers = false;
                            IsInventoryReport = true;
                            break;
                        case AppConstants.ROLE_TERRITORY_MANAGER:
                            IsSKU = IsTally = IsAssert = IsDailyStockReport = IsDirector = IsPurchaseInvoice = IsClosingStockVisible = IsDailySales = IsStocksVisible =IsBackend = IsSuperBackend = IsAdmin = IsStore  = false; // SUGI 1970 - Hide Stock Transfer for TM as it's not a part of RIO sprint.
                           IsInventoryReport = IsTeam= IsIndent = IsSalesReport= IsStockTransferVisible = true;
                            IsExpiry = true;
                            IsAssert = true;
                            break;
                        case AppConstants.ROLE_REGIONAL_MANAGER:
                             IsAssert = IsTally = IsSKU = IsDailyStockReport = IsDirector = IsPurchaseInvoice = IsClosingStockVisible  = IsStore =  IsAdmin =  IsSuperBackend = IsBackend =  IsStocksVisible = IsDailySales = false;
                           IsInventoryReport= IsTeam= IsIndent = IsSalesReport = true;
                            IsExpiry = true;
                            IsAssert = true;
                            break;
                        default:
                            break;
                    }

            });

        }

        public void isflyoutclose()
        {
            _eventAggregator.GetEvent<AddUserFlyoutOpenEvent>().Publish(false);
            _eventAggregator.GetEvent<AddStoreFlyoutOpenEvent>().Publish(new Entites.Stores.StoreFlyout() { IsOpen = false, IsParent = true });
            _eventAggregator.GetEvent<EditStoreFlyoutOpenEvent>().Publish(false);
            _eventAggregator.GetEvent<EditUserFlyoutOpenEvent>().Publish(false);
        }
    }


    public enum StartMenu
    {
        Invoice = 0,
        DashBoard = 1,
        Inventory = 2,
        Sales = 4,
        FinanceSales = 1,
    }
}
