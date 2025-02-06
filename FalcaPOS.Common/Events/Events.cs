using FalcaPOS.Common.Models;
using FalcaPOS.Entites.Asserts;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.Customers;
using FalcaPOS.Entites.Denomination;
using FalcaPOS.Entites.Director;
using FalcaPOS.Entites.Indent;
using FalcaPOS.Entites.Notification;
using FalcaPOS.Entites.Products;
using FalcaPOS.Entites.ProductTypes;
using FalcaPOS.Entites.Sales;
using FalcaPOS.Entites.Stock;
using FalcaPOS.Entites.Stores;
using FalcaPOS.Entites.Suppliers;
using Prism.Events;
using System.Collections.ObjectModel;

namespace FalcaPOS.Common.Events
{
    public class Login : PubSubEvent<bool> { }

    public class ShowReleseNote : PubSubEvent<bool> { }

    public class AddProductTypeEvent : PubSubEvent<object> { }

    public class AddAttributeEvent : PubSubEvent<object> { }

    public class LoadManufacturesEvent : PubSubEvent<object> { }

    public class SearchFlyout : PubSubEvent<bool> { }

    public class StoreSearchFlyout : PubSubEvent<StockFlyout> { }

    public class StockSearch : PubSubEvent<StockModelRequest> { }

    public class StoreStockSearch : PubSubEvent<StockModelRequest> { }

    public class CustomerSearchFlyout : PubSubEvent<CustomerSearchModel> { }

    public class CustomerSearchFlyoutOpen : PubSubEvent<bool> { }

    public class NotifyMessage : PubSubEvent<ToastMessage> { }

    public class LoginedRole : PubSubEvent<string> { }

    public class AddUserFlyoutOpenEvent : PubSubEvent<bool> { };

    public class ReloadUsersEvent : PubSubEvent { };

    public class EditUserFlyoutOpenEvent : PubSubEvent<object> { };

    public class ReloadSupplierEvent : PubSubEvent { };

    public class AddStoreFlyoutOpenEvent : PubSubEvent<StoreFlyout> { };

    public class EditStoreFlyoutOpenEvent : PubSubEvent<object> { };

    public class ReloadStoresEvent : PubSubEvent { };

    public class ReloadStockEvent : PubSubEvent { };

    public class LoggedInUserEvent : PubSubEvent<object> { };

    public class SearchSalesEvent : PubSubEvent<SearchParams> { };

    public class SalesSearchFlyoutOpenEvent : PubSubEvent<SalesFlyout> { };

    public class EditSupplerEvent : PubSubEvent<SuppliersViewModel> { };

    public class RefreshProductEvent : PubSubEvent { };

    public class RefreshInvoices : PubSubEvent { };

    // public class ProgressBarOpenEvent : PubSubEvent{ };

    // public class ProgressBarCloseEvent : PubSubEvent { };

    public class AddDefectiveQty : PubSubEvent<bool> { };

    public class AddDiscountPercentToProductCard : PubSubEvent<string> { };

    public class AddDiscountFlatToProductCard : PubSubEvent<string> { };


    public class SearchFinanceFlyoutEvent : PubSubEvent { }

    public class SearchFinanceEvent : PubSubEvent<object> { }

    #region SignalR Triggered Events

    /// <summary>
    /// Event to handle new product type received via Hub
    /// </summary>
    public class SignalRProductTypeAddedEvent : PubSubEvent<object> { };

    /// <summary>
    /// Event to handle new product type received via Hub
    /// </summary>
    public class SignalRCategoryAddedEvent : PubSubEvent<object> { };

    /// <summary>
    /// Event to handle user diabled received via Hub ,username is param 
    /// </summary>
    public class SignalRUserDisabledEvent : PubSubEvent<string> { };


    /// <summary>
    /// Event to handle product type is disabled/enabled
    /// </summary>
    public class SignalRProductTypeEnableDisableEvent : PubSubEvent<object> { }

    /// <summary>
    /// Event to handle product type is disabled/enabled
    /// </summary>
    public class SignalRBrandEnableDisableEvent : PubSubEvent<object> { }

    /// <summary>
    /// Event to handle when new user is added in server
    /// </summary>
    public class UserAddedEvent : PubSubEvent<object> { }

    /// <summary>
    /// Event to handle when new store was created.
    /// </summary>
    public class SignalRStoreAddedEvent : PubSubEvent<object> { }

    /// <summary>
    /// Event to handle when new supplier was created
    /// </summary>
    public class SignalRSupplierAddedEvent : PubSubEvent<object> { }

    /// <summary>
    /// Event to handle when supplier is enabled or disabled. or updated/edited
    /// </summary>
    public class SignalRSupplierEnableDisableEvent : PubSubEvent<object> { }

    /// <summary>
    /// Event to handle when state is created/updated.
    /// </summary>
    public class SignalRStateAddedEvent : PubSubEvent<object> { }

    /// <summary>
    /// Event to handle when distrcit is created/updated.
    /// </summary>
    public class SignalRDistrictAddedEvent : PubSubEvent<object> { }

    #endregion

    public class ConfirmationPopUpViewEvent : PubSubEvent<bool> { };

    public class PurchaseRateSearchFlyOut : PubSubEvent<bool> { };

    public class PurchaseRateSearch : PubSubEvent<PurchaseRateSearchModel> { }

    public class IntentNewTabCreateEvent : PubSubEvent<IndentViewModel> { }

    public class IntentStatuschangeRefreshEvent : PubSubEvent<int> { };
    public class StepSelectedIndexChangeEvent : PubSubEvent<int> { };

    public class DenominationPageRefreshEvent : PubSubEvent { };

    public class SignalRIndentAddedEvent : PubSubEvent<IndentViewModel> { }

    public class SignalRIndentStatusEvent : PubSubEvent<IndentViewModel> { }

    public class SignalRPOCancelledSahaya : PubSubEvent { }

    public class SignalRIndentStatusToInTransiteEvent : PubSubEvent<IndentViewModel> { }

    public class IndentStatusChangeToSahayaPendingEvent : PubSubEvent { }

    public class IndentStatusChangeFromSahayaInFinance : PubSubEvent{ }

    public class SignalRReloadStockTransferListForSahayaEvent : PubSubEvent<object> { };

    public class RegenerateEwayBill : PubSubEvent<StockTransferModelForEWayModel> { };

    public class ReloadStockReceiverListForSahayaEvent : PubSubEvent { };

    public class IntentNewTabRemoveEvent : PubSubEvent { }

    public class SignalRNewProductAddEvent : PubSubEvent<ProductDetails> { }

    public class ShowNotificationEvent : PubSubEvent { }
    public class NotificationCountEvent : PubSubEvent<int> { }
    public class NotificationEvent : PubSubEvent<POSNotification> { }

    public class SignalRNewBrandAddEvent : PubSubEvent<ProductTypeManufacturer> { }

    public class SignalRProductEnableDisableAddEvent : PubSubEvent { }

    public class SupplierNewTabCreateEvent : PubSubEvent<SuppliersDetailsViewModel> { }

    public class SupplierIDCreateEvent : PubSubEvent<ShippingAddress> { }

    public class ShippingAddressEvent : PubSubEvent<bool> { }


    public class AddNewBankEvent : PubSubEvent { }


    public class SignalRSupplierShippingAddressAddEvent : PubSubEvent { }

    public class SupplierTabRemoveEvent : PubSubEvent { }

    public class AppOrderMoveToSalePageEvent : PubSubEvent<AppOrderModel> { }

    public class StepSelectedIndexChangeAppOrderEvent : PubSubEvent<int> { };

    public class RefreshAppOrderPage : PubSubEvent { };

    public class StockRequestEvent : PubSubEvent { };

    public class StockTransferEvent : PubSubEvent { };


    public class MasterSKURefreshEvent : PubSubEvent { };
    public class ParentDataEvent : PubSubEvent<object> { };


    public class AddSKUFlyoutOpenEvent : PubSubEvent<bool> { };

    public class AddSKUOpenEvent : PubSubEvent<ObservableCollection<FileUploadInfo>> { };


    public class IndentListFlyoutOpenEvent : PubSubEvent<bool> { };
    public class IndentListFlyoutSearchDataEvent : PubSubEvent<IndentListSearch> { };
    public class IndentStatusFlyoutEvent : PubSubEvent<bool> { };


    public class ShowGSTCalculator : PubSubEvent<bool> { }

    public class StepSelectedPopIndexChangeEvent : PubSubEvent<int> { };

    public class BulkPaymentUpdateChangeEvent : PubSubEvent { };

    public class DenominationVerifyEvent : PubSubEvent { };

    public class SalesPageRefreshEvent : PubSubEvent<DenominationVerifyModel> { };
    public class EditCustomerPopupEvent : PubSubEvent<object> { };

    public class AssertSearchFlyoutOpen : PubSubEvent<bool> { }
    public class AssertSearchEvent : PubSubEvent<SearchAssertsModel> { }

    public class ExpiredEvent : PubSubEvent<bool> { }
    public class CurrentExpiryEvent : PubSubEvent<bool> { }
    public class NextExpiryEvent : PubSubEvent<bool> { }

    public class ThreeExpiryEvent : PubSubEvent<bool> { }
    public class SixExpiryEvent : PubSubEvent<bool> { }
    public class ExpiryExportEvent : PubSubEvent<bool> { }

    public class ExpiryCurrentEvent : PubSubEvent<bool> { }

    public class ExpiryNextEvent : PubSubEvent<bool> { }

    public class ExpiryThreeEvent : PubSubEvent<bool> { }
    public class ExpirySixEvent : PubSubEvent<bool> { }

    public class AddCategoryEvent : PubSubEvent<bool> { }
    public class RefreshTerritoryViewEvent : PubSubEvent { }

    public class MapStore : PubSubEvent<bool> { }



    public class ProductCertificateRefreshEvent : PubSubEvent { }
    public class PreviewEvent : PubSubEvent<string> { }
    /// <summary>
    /// Holds the event to load the barcode for the selected product and store.
    /// </summary>
    public class LoadBarcodesEvent : PubSubEvent { }
}
