using FalcaPOS.Entites.Stores;
using System;
using System.Collections.Generic;

namespace FalcaPOS.Common.Constants
{
    public class AppConstants
    {
        public const string LOGIN_URL = "api/account/login";

        public const string SUPPLIERS_GETALL = "api/Supplier/GetAllSuppliers";

        public const string GET_SUPPLIER_TDS_DETAILS = "api/Supplier/GetSupplierTDSDetails";

        public const string SUPPLIERS_CREATE = "api/Supplier/CreateSupplier";

        public const string ADD_SHIPPING_ADDRESS = "api/Supplier/ShippingAddress";

        public const string UPDATE_SUPPLIER_DETAILS = "api/Supplier/UpdateSupplierDetails";

        public const string SUPPLIERS_DETAILS = "api/Supplier/GetSupplierDetails";

        public const string SUPPLIERS_DETAILS_UPDATE = "api/Supplier/UpdateSupplier";

        public const string GET_SHIPPING_ADDRESS = "api/Supplier/GetShippingAddress";

        public const string GET_BANK_LIST = "api/Supplier/GetBankList";

        public const string ADD_NEW_BANK = "api/Supplier/addNewBank";

        public const string ADD_NEW_BANK_DETAILS = "api/Supplier/addNewBankDetails";

        public const string SUPPLIER_ENABLE_DISABLE = "api/Supplier/EnableDisableSupplier";

        public const string STORES_GETALL = "api/Store/GetAllStores";

        public const string STORES_GET_BYUSER = "api/Store/GetStoreDetailsbyuser";

        public const string PRODUCT_TYPES_GETALL = "api/ProductType/GetAllProductTypes";

        public const string PRODUCT_TYPES_CREATE = "api/ProductType/CreateProductType";

        public const string PRODUCT_TYPE_MANUFACTURERS = "api/ProductType/GetProductTypeManufacturers";

        public const string PRODUCT_TYPE_MANUFACTURER_CREATE = "api/ProductType/CreateProductTypeManufacterer";

        public const string PRODUCT_TYPE_ENABLE_DISABLE = "api/ProductType/EnableDisableProductType";

        public const string ATTRIBUTETYPE_CREATE = "api/Attribute/CreateAttribute";

        public const string ATTRIBUTETYPE_GETALL = "api/Attribute/GetAllAttributes";

        public const string MANUFACTURERS_GETALL = "api/Manufacturer/GetAllManufacturers";

        public const string MANUFACTURERS_ENABLE_DISABLE = "api/Manufacturer/EnableDisableManufacturer";


        public const string PRODUCT_CREATE = "api/Product/CreateProduct";

        public const string PRODUCT_GET_ALL = "api/Product/GetAllProducts";

        public const string PRODUCT_ENABLE_DISABLE = "api/Product/EnableDisableProduct";

        public const string GET_SKU_STOCK = "api/Product/GetStockbySKU";



        public const string ADD_STOCK_PRODUCT = "api/Stock/AddStockProduct";

        public const string GET_BACKEND_SEARCH = "api/Stock/GetBackendStockSearch";

        public const string GET_SALES_PRODUCT = "api/Sales/GetSalesProduct";

        public const string ADD_SALES = "api/Sales/AddSales";

        public const string GET_CUSTOMER_BY_PHONE = "api/Customer/GetCustomerByPhone";

        public const string GET_STORE_SEARCH = "api/Stock/GetStoreStockSearch";

        public const string GET_STATES = "api/Location/GetAllStates";
        

        public const string GET_PINCODEDETAILS = "api/Location/GetPincodeDetails";

        public const string CREATE_NEW_STATE = "api/Location/createstate";

        public const string UPDATE_STATE = "api/Location/UpdateState";

        public const string GET_STATE_DISTRICTS = "api/Location/GetStateDistricts";

        public const string GET_STATE_ZONE = "api/Zone/GetStateZones";

        public const string GET_ZONE_TERRITORY = "api/Zone/GetZoneTerritories";

        public const string GET_TERRITORY_STORELIST = "api/Zone/GetTerritoryStores";

        public const string GET_ALl_DISTRICTS = "api/location/GetAllDistricts";

        public const string GET_USERS = "api/Users/GetUsers";

        public const string ENABLE_DISABLE_USER = "api/Users/EnableDisableUser";

        public const string UPDATE_USER = "api/Users/UpdateUser";

        public const string CREATE_USER = "api/Account/Register";

        public const string GET_CUSTOMER_SEARCH = "api/Customer/GetCustomerSearch";

        public const string ADD_STORE = "api/Store/CreateStore";

        public const string UPDATE_STORE = "api/Store/UpdateStore";
        public const string GET_STORE_LICENSE = "api/Store/GetAllStoresWithLicense";

        public const string GET_INVOICE_SERVICE_PRODUCTS = "api/Sales/GetServiceProducts";

        public const string UPDATE_INVOICE_DETAILS = "api/Stock/UpdateInvoiceDetails";

        public const string GET_SALES_PRODUCT_SKUSEARCH = "api/Sales/GetSalesProductSKUSearch";



        //public const string GENERATE_INVOICE_NUMBERS = "api/Stock/GenerateInvoiceNumbers";


        public const string GET_INVOICE_LIST = "api/Stock/GetInvoices";

        public const string GET_BACKEND_INVOICE_LIST = "api/stock/BackendUserInvoices";

        public const string GET_INVOICE_DETAILS = "api/Stock/GetInvoiceDetails";

        public const string GET_PRODUCT_DETAILS = "api/Sales/GetSalesProduct";


        public const string GET_SALES_LIST = "api/Sales/GetSales";


        public const string GET_INVOICE_PDF = "api/sales/GetInvoicePDF";


        public const string GET_CLOSING_STOCK_REPORT = "api/stock/ClosingStockReport";

        public const string GET_STOCK_PRODUCT_INFORMATION = "api/stock/getproductinformation";

        public const string GET_STORE_INVOICE_FORMAT = "api/store/GetStroreCode";

        public const string GET_PRODUCT_BARCODE = "api/stock/GetProductBarCode";

        public const string GET_STORE_BARCODELIST = "api/stock/GetProductStoreBarCode";

        public const string PUT_STOCK_TRANSFER = "api/stock/StockTransfer";

        public const string PUT_RSP_STOCK_TRANSFER = "api/stock/RspStockTransfer";

        public const string PUT_STOCK_REQUEST = "api/stock/StockRequest";

        public const string GET_STOCK_RECEIVER = "api/stock/GetStockReceiver";

        public const string GET_STOCK_RECEIVER_LIST = "api/stock/GetStockReceiverList";

        public const string GET_STOCK_TRANSFER_LIST = "api/stock/GetStockTransferList";

        public const string GET_STOCK_TRANSFERPDF = "api/stock/GetStockTransferPDF";

        public const string GET_STOCK_TRANSFER_COMPLETED = "api/stock/GetStockTransferCompleted";

        public const string GET_STOCK_TRANSFER_SEARCHLIST = "api/stock/GetStockTransferSearchList";

        public const string UDATE_STOCK_TRANSFER_APPROVAL = "api/stock/UpdateStockTransferApproval";

        public const string UDATE_STOCK_RECEIVER = "api/stock/UpdateStockReceiver";

        public const string GET_CURRENT_STREQUEST_NUMBER = "api/stock/StockTransferOrderSequence";

        public const string GET_CURRENT_STRECEIPT_NUMBER = "api/stock/StockTransferReceiptOrderSequence";

        public const string UPLOAD_TRANSPORT_FILES = "api/stock/UploadFile";

        public const string GET_SALES_INVOICE_NUMBER = "api/sales/GetSalesInvoiceNumber";

        public const string GET_SALES_SLOTWISEINVOICEPRODUCTDETAILS = "api/stock/GetSlotWiseInvoiceProductDetails";

        public const string GET_MIS = "api/Dashboard/GetMIS";

        public const string GET_MIS_IN_DETAILS = "api/Dashboard/GetMISinDetail";


        public const string UPDATE_BACKEND_INVOICE_NUMBER = "api/stock/UpdateInvoiceNumber";

        public const string UPDATE_SELLING_PRICE = "api/stock/UpdateSellingPrice";
        public const string UPDATE_SELLING_PRICEV2 = "api/stock/UpdateSellingPriceV2";

        public const string GET_STORE_STOCK_TRANSFER_SEARCH = "api/stock/GetStoreStockTransferSearchV2";

        public const string GET_STOCK_TRANSFER_SEARCHV2 = "api/stock/GetStockTransferSearchV2";
       
        public const string GET_EWAY_BILL_STOCKTRANSFER = "api/EWayBill/GenerateEWayBill";

        public const string GET_EWAY_ACCESS_TOKEN = "api/EWayBill/GenerateAccessToken";

        public const string GET_STOCK_TRANSFER_RQUEST_LIST = "api/stock/GetStockTransferRequestList";

        public const string UPDATE_STOCK_TRANSFER_APPROVE = "api/stock/UpdateTransferRequestApprove";

        public const string UPDATE_STOCK_TRANSFER_REJECT = "api/stock/UpdateTransferRequestReject";

        public const string UPDATE_STOCK_TRANSFER_QTY = "api/stock/UpdateStockTranferQty";

        public static string EwayBillAccesstoken { get; set; }

        //admin dashboard API's 


        public const string GET_CUSTOMERS_BY_STORE = "/api/Dashboard/CustomersByStore";

        public const string GET_SALES_BY_STORE = "/api/Dashboard/GetSalesByStore";

        public const string GET_SUPPLIER_BY_STORE = "/api/Dashboard/GetSupplierByStore";
        public const string GET_SALES_BY_MONTH = "/api/Dashboard/GetSalesByMonth";
        public const string GET_SALES_BY_BRAND = "/api/Dashboard/GetMostNumberOfSalesBrnad";
        public const string GET_SALES_BY_PRODUCT = "/api/Dashboard/GetMostNumberOfSalesProduct";
        //Return api
        public const string GET_EXCAHNGE_PRODUCT = "api/sales/GetExhangeProduct";
        public const string FARMER_RETURN_PRODUCT = "api/sales/FarmerReturnProduct";

        public const string GET_DEFECTIVE_LIST = "api/Stock/GetDefectiveList";

        public const string GET_SALES_DEFECTIVE_LIST = "api/Stock/GetSalesDefectiveList";

        public const string GET_DEFECTIVEPRODUCT_DETAILS = "api/Stock/GetDefectiveProductList";


        //Finance API version 1.0

        public const string POST_FINANCE_SALES = "api/Finance/FinanceSales";

        public const string GET_TALLY_EXPORT = "api/Finance/GetTallyExport";


        //Product type dept code

        public const string PRODUCT_TYPE_DEPT_CODE = "api/producttype/ProductTypeDeptNumber";

        public const string PRODUCT_CURRENT_SKU_NUMBER = "api/product/ProductCurrentSku";


        /// <summary>
        /// Get the product for SKU number, 
        /// <br/>
        /// Pass skunumber as request body param.
        /// </summary>
        public const string GET_SKU_PRODUCT = "api/product/getskuproduct";

        public const string GET_SKU_TRANSFER_PRODUCT = "api/product/GetSKUSelectTransferProduct";

        public const string GET_BRANDCATEGORYSUBCATEGORYSKUBYID = "api/product/GetBrandCategorySubcategorySKUbyId";

        public const string SEARCH_PRODUCT = "api/product/searchproduct";

        public const string GET_ALL_PRODUCT_TYPE = "api/Sku/GetAllProductType";

        public const string VERIFY_PRODUCT_COUNT = "api/Sku/VerifyProductCount";

        public const string GET_DAILY_STOCK = "api/Sku/DailyStockReport";

        public const string GET_ALL_SKU_SHEET = "api/Sku/GetAllSkuSheet";

        public const string GET_ALL_SKU_SHEETV2 = "api/Sku/GetAllSkuSheetV2";

        public const string GET_SKU_APPROVE_LIST = "api/Sku/GetSKUApproveListV2";

        public const string REMOVE_SKU_APPROVAL_PENDING = "api/Sku/RejectSKU";

        public const string SKU_APPROVE = "api/Sku/ApproveSKU";

        public const string GET_PURCHASE_RATE_LIST = "api/Director/GetPurchaseRateList";

        public const string GET_STORE_ASSERT = "api/Director/GetStoreAssert";

        public const string CLEAR_EXPIRY_CACHE_ON_REFRESH = "api/ExpiryProducts/ClearExpiryProductsCacheOnRefresh";

        public const string GET_CURRENT_MONTH = "api/ExpiryProducts/GetCurrentMonth";

        public const string GET_NEXT_MONTH = "api/ExpiryProducts/GetNextMonth";

        public const string GET_NEXT3_MONTH = "api/ExpiryProducts/GetNext3Month";

        public const string GET_NEXT6_MONTH = "api/ExpiryProducts/GetNext6Month";

        public const string GET_EXPIRED_PRODUCT = "api/ExpiryProducts/GetExpired";

        public const string UPDATE_EXPIRY_DATE = "api/ExpiryProducts/UpdateExpiryDate";

        public const string PRODUCT_GET_ALL_ENABLED = "api/Product/GetAllEnabledProducts";

        public static string ACCESS_TOKEN { get; set; }

        public static string UserName { get; set; }
        public static string StoreName { get; set; }

        public const string ShortStoreNameReplace = "FALCA RAITHA UNNATI KENDRA";

        public const string ShortStoreName = "UK";

        public static Store LoggedInStoreInfo { get; set; }

        public static bool IsLoggedIn { get; set; } = true;

        public static string[] USER_ROLES { get; set; }

        public static string UserStoreLocation { get; set; }

        public static int UserId { get; set; }

        public static string STATE { get; set; }
        public static string FalcaGSTIN { get; set; }
        public static string Printer { get; set; }

        public const string DOWNLOAD_FILE_INVOICE = "api/Resource/GetById";

        public const string DOWNLOAD_FILE_MSME = "api/Resource/GetByIdMSME";


        public const string DOWNLOAD_FILE_INVOICE_LIST = "api/Resource/GetByIdList";

        /// <summary>
        /// Delete file by id endpoint        
        /// </summary>
        public const string DELETE_FILE_INVOICE = "api/Resource/DeleteFile";


        //Roles

        public const string ROLE_SUPER_BACKEND = "superbackend";

        public const string ROLE_ADMIN = "admin";

        public const string ROLE_BACKEND = "backend";

        public const string ROLE_STORE_PERSON = "storeperson";

        /// <summary>
        /// Finance user role.
        /// </summary>
        public const string ROLE_FINANCE = "finance";

        public const string ROLE_DIRECTOR = "falcadirector";

        public const string ROLE_PURCHASE_MANAGER = "purchasemanager";

        public const string ROLE_AUDITOR = "auditor";

        public const string ROLE_TERRITORY_MANAGER = "territorymanager";

        public const string ROLE_REGIONAL_MANAGER = "regionalmanager";

        public const string ROLE_CONTROL_MANAGER = "controlmanager";

        public const string FALCA_SITE_URL = "http://www.falcasolutions.com/";

        public static string CurrentDate { get; set; }

        #region SignalR Hub Methods and events

        public const string PRODUCTTYPE_ADDED = "producttypeadded";

        public const string CATEGORY_ADDED = "categoryadded";

        public const string USER_DISABLED = "userdisabled";

        /// <summary>
        /// SingnalR Method name
        /// </summary>
        public const string PRODUCTTYPE_ENABLE_DISABLE = "producttypeenabledisable";

        /// <summary>
        /// New User added by admins.
        /// </summary>
        public const string USER_ADDED = "user_added";

        /// <summary>
        /// Hub method name for new store created.
        /// </summary>
        public const string STORE_ADDED = "storeadded";

        /// <summary>
        /// Hub method name for new supplier added or edited/updated.
        /// </summary>
        public const string SUPPLIER_ADDED = "supplieradded";

        /// <summary>
        /// Hub method name for supplier.
        /// </summary>
        public const string SUPPLIER_ENABLE_DISABLE_EVENT = "supplierenabledisable";


        /// <summary>
        /// Event to notify clients when new state created.
        /// </summary>
        public const string SIGNALR_NEW_STATE_ADDED_EVENT = "stateadded";


        /// <summary>
        /// Event to notify clients when new district created.
        /// </summary>
        //public const string SIGNALR_NEW_DISTRICT_ADDED_EVENT = "districtadded";



        public const string INDENT_CREATED = "indentcreated";

        public const string INVENTORY_ADDED = "InventoryAdded";

        public const string INDENT_STATUS_CHANGE = "indentstatuschange";

        public const string INDENT_STATUS_INTRANSITE = "indentstatusintransite";

        public const string INDENT_STATUS_CHANGE_SAHAYA = "indentstatuschangefromsahaya";

        public const string STOCK_TRANSFER_STATUS_CHANGE_SAHAYA = "stocktransferstatuschangefromsahaya"; 

        public const string PRODUCT_ADDED = "productadded";

        public const string BRAND_ADDED = "brandadded";

        public const string BRAND_ENABLED_DISABLED = "brandenabledisable";

        public const string PRODUCT_TYPE_ADDED = "producttypeadded";

        public const string PRODUCT_ENABLED_DISABLED = "productenableddisabled";

        public const string SUPPLIER_BRANCH_ADDED = "supplierbranchadded";

        public const string SUPPLIER_BANK_ADDED = "supplierbankadded";


        public const string STOCK_TRANSFER_REQUEST_SENT = "stocktransferrequestsent";

        public const string STOCK_TRANSFER = "stocktransfer";

        public const string STOCK_TRANSFER_RECEIVER = "stocktransferreceiver";

        public const string UPDATED_SELLINGPRICE = "UpdatedSellingPrice";

        public const string CREATE_SKU = "createsku";

        public const string SKU_REQUEST_APPROVE = "skurequestapproved";

        public const string MAPSTORE = "mapstore";




        #endregion


        #region Printer Types


        // public const string DYMO_PRINTER = "dymo";

        public const string ZENPERT_PRINTER = "zenpert";

        public const string ZENPERT_PRINTER_NAME = "ZENPERT 4T520";

        #endregion




        #region Region Names


        public const string REGION_FINANCE_HOME = "FinanceHome";

        public const string REGION_CLOSING_STOCK = "ClosingStockHome";

        public const string REGION_FLYOUT = "FlyoutRegion";

        public const string REGION_INDENT_HOME = "IndentHomeRegion";

        public const string REGION_INDENT_VIEW = "IndentViewRegion";

        public const string REGION_INDENT_CREATE = "IndentCreateRegion";

        public const string REGION_PURCHASE_INVOICE = "PurchaseInvoiceListHome";

        public const string REGION_PURCHASE_RETURNS = "PurchaseReturns";

        public const string REGION_INVOICE_HOMETAB = "InvoiceTab";

        public const string REGION_PURCHASE_RETURNS_VIEW = "PurchaseReturnsView";

        public const string REGION_DAILY_STOCK_REPORT = "DailyStockReport";

        public const string REGION_ADD_STORE_DAILY_STOCK = "AddStoreDailyStock";

        public const string REGION_SKU_SHEET = "SkuSheet";
        //director
        public const string REGION_STORE_ASSERT = "StoreAssert";

        public const string REGION_PURCHASE_RATE = "PurchaseRate";


        public const string REGION_INDENT_APPROVAL = "IndentApprovalRegion";

        public const string REGION_INDENT_BULK_PAYMENTUPDATE = "IndentBulkPaymnetUpdate";

        public const string REGION_INDENT_BULK_DOWNLOAD = "IndentBulkDownload";

        public const string REGION_INDENT_BULK_UPLOAD = "IndentBulkUpload";

        public const string REGION_TALLY_EXPORT = "TallyExportRegion";

        public const string REGION_SKU_HOME = "HomeSKURegion";

        public const string REGION_SKU_CREATE = "HomeSKUCreate";

        public const string REGION_SKU_VIEW = "HomeSKUView";

        public const string REGION_SKU_ALTER = "HomeSKUAlter";

        public const string REGION_SKU_APPROVE = "HomeSKUAPPROVE";


        public const string REGION_STORE_PURCHASE_RETURNS = "StoreReturns";

        public const string REGION_STORE_PURCHASE_RETURNS_HOME = "PurchaseReturnHome";

        public const string REGION_STORE_PURCHASE_RETURNS_VIEW = "StoreViewReturn";

        public const string REGION_CREDITNOTE_SUMMARY = "CreditNoteSummary";

        #endregion

        public const string MANUFACTURER_ENABLED_LIST = "api/Manufacturer/GetManufacturersEnabled";

        public const string CREATE_PRODUCT_TYPE_MANUFACTURER = "api/Manufacturer/CreateProductTypeManufacturer";


        public const string INVOICE_UPLOAD_FILES = "api/Stock/UploadInvoice";


        public const string ENV_PRODUCTION = "PRODUCTION";

        public const string ENV_PREPRODUCTION = "PREPRODUCTION";

        public const string ENV_QA = "TESTING";

        public const string GET_PURCHASE_INVOICE = "api/Invoice/PurchaseInvoices";

        public const string GET_PRODUCTDETAILS_INVOICE = "api/Invoice/GetProductDetails";

        public const string GET_PURCHASE_RETURN_SEARCH = "api/Invoice/PurchaseReturnSearch";

        public const string UPDATE_PURCHASE_RETURN = "api/Invoice/UpdatePurchaseReturn";

        public const string GET_STORE_RETURN_SEARCH = "api/Invoice/PurchaseStoreReturnSearch";

        public const string UPDATE_STORE_PURCHASE_RETURN = "api/Invoice/UpdateStorePurchaseReturn";

        public const string GET_PURCHASE_RETURN_LIST = "api/Invoice/PurchaseReturnsViewList";

        public const string APPROVE_PURCHASE_RETURNS = "api/Invoice/ApprovePurchaseReturns";

        public const string GET_STORE_PURCHASE_RETURN_LIST = "api/Invoice/StorePurchaseReturnsViewList";

        public const string UPDATE_STORE_PURCHASE_RETURN_WITH_ATTACHMENT = "api/Invoice/UpdateStoreReturnwithAttachment";

        public const string POST_CN_APPROVE_PURCHASE_RETURNS = "api/Invoice/PostCNApprovePurchaseReturns";

        public const string EDIT_PURCHASE_RETURN_PRODUCT = "api/Invoice/EditPurchaseReturnsProduct";

        public const string GET_CREDITNOTE_SUMMARY = "api/Invoice/GetCreditNoteSummary";

        public const string GET_CREDITNOTE_SUMMARY_DETAILS = "api/Invoice/GetCreditNoteSummaryDetails";

        public const string REGION_RSP_SUMMARY = "RSPSummary";

        public const string REGION_RSP_SALES = "RSPSales";

        public const string REGION_RSP_STOCK = "RSPStock";

        public const string REGION_RSP = "RSP";



        #region Indent API's


        public const string GET_CURRENT_PO_NUMBER = "api/Indent/PurchaseOrderSequence";

        public const string GET_PO_SEQUENCE_FORMAT_FOR_STORE = "api/Indent/GetPONOSequenceFormatForStore";

        public const string CREATE_INDENT = "api/Indent/Create";

        public const string INDENT_APPROVAL = "api/Indent/IndentApproval";

        public const string INDENT_REJECT = "api/Indent/IndentRejectToReCreate";

        public const string VIEW_INDENT_LIST = "api/Indent/ViewIndentList";

        public const string VIEW_DETAIL_INDENT = "api/Indent/GetViewIndentInDetails";


        public const string ADD_SUPPLIER_TO_INDENT = "api/Indent/AddSupplierToIndent";

        public const string STATUS_CHANGE = "api/Indent/StatusChange";

        public const string STATUS_CHANGE_TO_PLACED = "api/Indent/StatusChangeToPlaced";

        public const string STATUS_CHANGE_TO_INTRASIT = "api/Indent/StatusChangeToInTransit";

        public const string STATUS_CHANGE_TO_RECEIVED = "api/Indent/StatusChangeToReceived";

        public const string EDIT_PRODUCT_PRICE_INDENT = "api/Indent/EditProductPriceToIndent";

        public const string GET_PURCHASE_ORDER_PDF = "api/Indent/getPurchaseOrderPDF";

        public const string GET_PURCHASE_ORDER_PDF_FROM_INVOICE_MODULE = "api/Indent/GetPOInvoiceFromInvoiceModule";

        public const string GET_INDENT_INTRANSITE_LIST = "api/Indent/GetIndentInTransitList";

        public const string GETBANKDETAILSLIST = "api/Indent/GetBankDetailsList";

        public const string GET_BULK_PAYMENT_LIST = "api/Indent/GetBulkPaymentList";

        public const string CREDIT_NOTE_ADJUSTMENT = "api/Indent/CreditNoteAdjustment";

        public const string BULK_PAYMNET_UPDATE = "api/Indent/BulkPaymentUpdate";


        public const string UPDATE_PARTIAL_PAYMENT = "api/Indent/UpdatePartialPayment";

        public const string GET_PENDING_PAYMNETSUPPLIER_LIST = "api/Indent/GetPendingPaymentSupplier";

        #endregion



        #region Exclude validations

        public static string[] ExcludeDateOfExpiry => new string[] { "FERTILIZERS", "IMPLEMENTS" };


        #endregion



        #region GST APPly Types


        /// <summary>
        /// Apply GST(%) to rate before the discounted price (flat or percent).
        /// </summary>
        public const string APPLY_DISCOUNT_BEFORE_GST = "Before GST";

        /// <summary>
        /// Apply GST(%) to rate after the discounted price (flat or percent).
        /// </summary>
        public const string APPLY_DISCOUNT_AFTER_GST = "After GST";



        #endregion


        #region Error Message
        public const string CommonError = "An error occurred, try again";
        #endregion



        public const string STATE_LIST_REGIONS = "statelist";

        public const string DISTRICT_LIST_REGION = "districtlist";

        public const string CREATE_DISTRICT = "api/location/CreateDistrict";
        public const string UPDATE_DISTRICT = "api/location/UpdateDistrict";


        public const string GET_SALES_AVAILABLE_BALANCE = "api/Sales/GetTodaySales";

        public const string ADD_DENOMINATION = "api/Denomination/AddDenomination";

        public const string GET_DENOMINATION = "api/Denomination/GetDenomination";

        public const string DOWNLOAD_FILE_DDENOMIANTION = "api/Denomination/getbyfilename";

        public const string GET_CREDIT_SALES = "api/sales/GetCreditSales";

        public const string UPDATE_CREDIT_SALES = "api/sales/UpdateCreditSales";

        public const string UPLOAD_CHEQUE_FILES = "api/sales/UploadCheque";

        public const string GET_CREDIT_SALES_FINANCE = "api/sales/GetCreditSalesFinance";

        public const string UPDATE_CREDIT_SALES_FINANCE = "api/sales/UpdateCreditSalesFinance";

        public const string UPLOAD_BANK_FILES = "api/Supplier/UploadFile";

        public const string GET_APP_ORDER_LIST = "api/sales/GetAppOrdercustomerList";

        public const string CANCEL_APP_ORDER_CUSTOMER = "api/sales/CancelAppOrdercustomer";

        public const string DELIVER_APP_ORDER_CUSTOMER = "api/sales/DeliveryAppOrdercustomer";


        public const string STOCKAGE_GETSTOCKAGEREPORT = "api/stockage/GetStockAgeReport";

        public const string CREATE_SKU_REQUEST = "api/Sku/CreateSKURequest";

        public const string UPLOAD_CERITFICATE_FILES = "api/Sku/UploadFile";

        public static List<String> INDENT_STATUS_LIST = new List<String>() { "Planned", "Review", "Approve", "Add Supplier", "Placed", "InTransit", "Received", "Closed", "Sahaya Pending", "Cancelled" };

        public static string GETPRODUCTCERTIFICATE = "api/Sku/GetproductCertificate";

        public const string VIEW_SKU_REQUEST = "api/Sku/ViewSKURequest";

        public const string VIEW_SKU = "api/Sku/GetViewSKUSearch";

        public const string VIEW_SKU_PRODUCT = "api/Sku/GetViewSKUProduct";


        public const string GET_APPROVE_SKU_REQUEST = "api/Sku/GetApproveSKURequest";

        public const string APPROVED_SKU_REQUEST = "api/Sku/ApprovedSKURequest";

        public const string ALTER_SKU_SEARCH = "api/Sku/AlterSKUSearch";

        public const string UPDATE_SKU_CERTIFICATE = "api/Sku/UpdateSKUCertificate";

        public const string GET_REALIZE_DATECREDIT_SALES_FINANCE = "api/sales/GetRealizeDateCreditSalesFinance";

        public const string PUT_RSP_STOCK_REQUEST = "api/stock/RspStockRequest";

        public const string GET_RSP_SUMMARY = "api/v1/RSP/GetRSPSummary";

        public const string STOCKAGE_GETSTOCKAGEREPORT_STORE = "api/stockage/GetStockAgeReportStore";


        public const string ADD_CASE_DEPOSIT = "api/Denomination/AddCashDeposit";

        public const string GET_DEPOSIT_BANKS = "api/Denomination/GetDepositBankList";

        public const string GET_CASE_DEPOSIT_LIST = "api/Denomination/GetCashDepositSearch";


        public const string GET_DEPSIT_FILE = "api/Denomination/GetById";

        public const string UPDATE_CASE_DEPOSIT_APPROVAL = "api/Denomination/UpdateCashDepositApproval";

        public const string GET_DENOMINATION_VERIFY = "api/Denomination/GetDenominationVerify";

        public const string GET_SKU_STOCK_PRODUCT_SEARCH = "api/product/GetSKUStockProductSearch";

        public const string GET_DENOMINATION_SERACH = "api/Denomination/GetDenominationSearch";

        public const string CREATE_SKU_REQUEST_WITH_CERTIFICATE = "api/Sku/CreateSKURequestWithCertificate";

        public const string ASSERTS_CODE = "api/Asserts/AddAssertCode";

        public const string ASSERT_CLASS = "api/Asserts/AddAssertClass";

        public const string ASSERT_TYPE = "api/Asserts/AddAssertType";

        public const string ASSERT_CATEGORY = "api/Asserts/AddAssertCategory";

        public const string GET_ASSERT_CODE = "api/Asserts/GetAssertCode";

        public const string GET_ASSERT_CLASS = "api/Asserts/GetAssertClass";

        public const string GET_ASSERT_TYPE = "api/Asserts/GetAssertType";

        public const string GET_ASSERT_CATEGORY = "api/Asserts/GetAssertCategory";

        public const string ADD_ASSERTS = "api/Asserts/AddAsserts";

        public const string GET_ASSERTS = "api/Asserts/GetAsserts";

        public const string GET_ASSERTS_SEARCH = "api/Asserts/GetAssertsSearch";

        public const string EDIT_ASSERTS = "api/Asserts/EditAsserts";

        public const string GET_LOGIN_TIME = "api/account/GetLoginTime";

        public const string GET_VILLAGE = "api/Sales/GetVillage";

        public const string GET_ALL_CATEGORY = "api/ProductType/GetAllCategory";
        public const string GET_ALL_LICENSE_CATEGORY = "api/ProductType/GetAllLicenseCategory";

        public const string GET_SUB_CATEGORY = "api/ProductType/GetSubCategory";

        public const string TM_REVIEW = "api/Indent/TmReview";

        public const string RM_APPROVE = "api/Indent/RmApprove";

        public const string ADD_CUSTOMER = "api/Sales/AddCustomer";

        public const string EDIT_CUSTOMER = "api/Sales/EditCustomer";

        public const string ADD_CATEGORY = "api/ProductType/AddCategory";

        public const string ADD_SUB_CATEGORY = "api/ProductType/AddSubCategory";


        #region ZoneEndPoints

        public const string CREATE_ZONE = "api/Zone/CreateZone";
        public const string CREATE_TERRITORY = "api/Zone/CreateTerritory";
        public const string PUT_CREATETERRITORYSTOREMAP = "api/Zone/CreateTerritoryStoreMap";
        public const string PUT_ASSIGNREGIONALMANAGERTOZONE = "api/Zone/AssignRegionalManagerToZone";
        public const string PUT_ASSIGNTERRITORYMANAGERTOTERRITORY = "api/Zone/AssignTerritoryManagerToTerritory";

        public const string PUT_UNASSIGNSUSERFROMSTORE = "api/Zone/UnassignsUserFromStore";
        public const string PUT_UNASSIGNSUSERFROMZONE = "api/Zone/UnassignsUserFromZone";
        public const string PUT_UNASSIGNSUSERFROMTERRITORY = "api/Zone/UnassignsUserFromTerritory";
        
        public const string GET_ZONE = "api/Zone/GetZone";
        public const string GET_TERRITORY = "api/Zone/GetTerritory";
        public const string GET_TERRITORYVIEW = "api/Zone/GetView";
        public const string GET_STOREMAP = "api/Zone/GetStoreMap";
        public const string GET_REGIONALMANGERSLIST = "api/Zone/GetRegionalMangersList";
        public const string GET_TERRITORYMANAGERSLIST = "api/Zone/GetTerritoryManagersList";

        #endregion




        #region CodeConstants
        public const string YETTOADD = "Yet to add";

        public const string PURCHASE_LEDGER = "PurchaseLedger";

        public const string PURCHASE_OTHER_STATE = "PurchaseOtherState";

        public const string LEDGER_IGST = "LedgerIGST";

        public const string LEDGER_CGST = "LedgerCGST";

        public const string LEDGER_SGST = "LedgerSGST";

        public const string SALES_LEDGER = "SalesLedger";

        public const string LIABILITY_CGST = "LiabilityCGST";

        public const string LIABILITY_SGST = "LiabilitySGST";

        public const string SALES_OTHER_STATE = "SalesOtherState";

        public const string LIABILITY_IGST = "LiabilityIGST";

        public const string SERVICES_LEDGER = "ServicesLedger";

        public const string SERVICES_CGST = "ServicesCGST";

        public const string SERVICES_SGST = "ServicesSGST";

        public const string SERVICES_OTHER_STATE = "ServicesOtherState";

        public const string SERVICES_IGST = "ServicesIGST";



        public const string GSTHEADER = "Total GST";

        public const string IGSTHEADER = "Total IGST";

        public const string GSTHEADERPER = "GST(%)";

        public const string GSTHEADERQTY = "GST Per Qty";

        public const string IGSTHEADERPER = "IGST(%)";

        public const string IGSTHEADERQTY = "IGST Per Qty";



        #endregion

        public static List<string> PURCHASE_RETURN_STATUS=new List<string>() { "Return Requested", "Return Approved", "C/N Received", "C/N Approved", "C/N Rejected", "C/N Partially Adjusted", "C/N Adjusted"};

        public const string GET_INENTORY_REPORT = "api/Reports/GetInventoryReport";

        public const string GET_POGRN_REPORT = "api/Reports/GetPOGRNReport";

        public const string GET_TOP_LIST_ITEMS = "api/Reports/GetTopListItems";

        public const string GET_TOP_LIST_BRAND = "api/Reports/GetTopListBrand";

        public const string GET_TOP_LIST_CATEGORY = "api/Reports/GetTopListCategories";

        public const string GET_TOP_LIST_TRANSACTION = "api/Reports/GetTopListTransactions";


        #region CouponEndPonts

        public const string CHECKFOR_REDEEM_COUPON = "api/Coupon/CheckForRedeemCoupon";

        public const string GET_COUPON_VALIDAITY = "api/Coupon/GetCouponValidityByCode";

        public const string FETCH_COUPON = "api/Coupon/FetchCoupon";

        public const string APPLY_COUPON = "api/Coupon/ApplyCoupon";

        #endregion

    }
}


