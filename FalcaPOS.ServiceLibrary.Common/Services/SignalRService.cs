using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Indent;
using FalcaPOS.Entites.Location;
using FalcaPOS.Entites.Manufacturers;
using FalcaPOS.Entites.Products;
using FalcaPOS.Entites.ProductTypes;
using FalcaPOS.Entites.Sku;
using FalcaPOS.Entites.Stock;
using FalcaPOS.Entites.Stores;
using FalcaPOS.Entites.Suppliers;
using FalcaPOS.Entites.User;
using FalcaPOS.Stock.ViewModels;
using Microsoft.AspNetCore.SignalR.Client;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FalcaPOS.ServiceLibrary.Common.Services
{
    public class SignalRService : SignalRBaseHubClient, ISignalRService
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly ProgressService _ProgressService;

        private readonly Logger _logger;

        public SignalRService(IEventAggregator eventAggregator, ProgressService ProgressService, Logger logger) : base(logger)
        {
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _ProgressService = ProgressService ?? throw new ArgumentNullException(nameof(ProgressService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }

        public async Task ConnectToHubAsync(string connectionHubURL)
        {
            try
            {

                _logger.LogInformation($"Connect to Hub, with connection URL {connectionHubURL}");

                await StartHubAsync(connectionHubURL);
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in connectToHubAsync", _ex);
            }

        }
        public async Task DisconnectFromHubAsync()
        {

            if (_hubConnection != null)
            {
                await _hubConnection.StopAsync();

                await _hubConnection.DisposeAsync();

                _logger.LogInformation("Closing Hub Connection");
            }
            else
            {
                _logger.LogInformation("Hub connection is empty | null to disconnect ");
            }
        }

        public async Task SendMessageToHubAsync(string methodName, object arg1)
        {
            throw new NotImplementedException();
        }

        public async Task SendMessageToHubAsync(string methodName, object arg1, object arg2)
        {
            throw new NotImplementedException();
        }

        public override async Task StartHubAsync(string connectionHubURL)
       {

            _logger.LogInformation($"Starting hub connection URL {connectionHubURL}");

            if (_hubConnection != null)
            {
                _logger.LogWarning("Hub connection exsists, Disposing it");

                await _hubConnection.DisposeAsync();
            }


            await Init(connectionHubURL);
        }
        public async Task Init(string connectionHubURL)
        {

            _logger.LogInformation($"Initiliazing Hub Connection with URL {connectionHubURL}");

            ConnectionURL = connectionHubURL;

            base.Init();

            _logger.LogInformation("Listining for events...");

            _hubConnection.On<ProductType>(AppConstants.PRODUCTTYPE_ADDED, ProductTypeAdded);

            _hubConnection.On<Category>(AppConstants.CATEGORY_ADDED, CategoryAdded);

            _hubConnection.On(AppConstants.USER_DISABLED, UserDisabled);

            _hubConnection.On<ProductType>(AppConstants.PRODUCTTYPE_ENABLE_DISABLE, ProductTypeEnableDisable);

            _hubConnection.On<User>(AppConstants.USER_ADDED, UserAdded);

            _hubConnection.On<Store>(AppConstants.STORE_ADDED, NewStoreAdded);

            _hubConnection.On<Supplier>(AppConstants.SUPPLIER_ADDED, SupplierAdded);

            _hubConnection.On<Supplier>(AppConstants.SUPPLIER_ENABLE_DISABLE_EVENT, SupplierEnableDisable);

            _hubConnection.On<State>(AppConstants.SIGNALR_NEW_STATE_ADDED_EVENT, StateAdded);

            //_hubConnection.On<District>(AppConstants.SIGNALR_NEW_DISTRICT_ADDED_EVENT, DistrictAdded);

            _hubConnection.On<IndentViewModel>(AppConstants.INDENT_CREATED, IndentAdded);

            _hubConnection.On<IndentViewModel>(AppConstants.INVENTORY_ADDED, InventroyAdded);

            _hubConnection.On<IndentSignalrViewModel>(AppConstants.INDENT_STATUS_CHANGE, IndentStatusChange);

            _hubConnection.On<IndentViewModel>(AppConstants.INDENT_STATUS_INTRANSITE, IndentStatusChangeToIntransite);

            _hubConnection.On<string>(AppConstants.INDENT_STATUS_CHANGE_SAHAYA, IndentStatusChangeFromSahaya);

            _hubConnection.On<bool>(AppConstants.STOCK_TRANSFER_STATUS_CHANGE_SAHAYA, StockTransferStatusChangeFromSahaya);

            _hubConnection.On<ProductDetails>(AppConstants.PRODUCT_ADDED, ProductAdded);

            _hubConnection.On<ProductTypeManufacturer>(AppConstants.BRAND_ADDED, BrandAdded);

            _hubConnection.On<Manufacturer>(AppConstants.BRAND_ENABLED_DISABLED, BrandEnableDisable);

            _hubConnection.On<ProductDetails>(AppConstants.PRODUCT_ENABLED_DISABLED, ProductEnableDisbale);

            _hubConnection.On<Supplier>(AppConstants.SUPPLIER_BRANCH_ADDED, SupplierShippingAddressAdded);

            _hubConnection.On<Supplier>(AppConstants.SUPPLIER_BRANCH_ADDED, SupplierShippingAddressAdded);

            _hubConnection.On<StockTrnasferModel>(AppConstants.STOCK_TRANSFER_REQUEST_SENT, StockTransferRequest);

            _hubConnection.On<StockTrnasferModel>(AppConstants.STOCK_TRANSFER, StockTransferFrom);

            _hubConnection.On<StockTrnasferModel>(AppConstants.UPDATED_SELLINGPRICE, UpdatedSellingPrice);

            _hubConnection.On<List<SKUModel>>(AppConstants.CREATE_SKU, CreateSKU);


            _hubConnection.On<SKUModel>(AppConstants.SKU_REQUEST_APPROVE, SKURequestApprove);


            _hubConnection.On<bool>(AppConstants.MAPSTORE, UpdateMapStore);




            await StartHubInternal();
        }

        //private void DistrictAdded(District obj)
        //{

        //}

        private void StateAdded(State _state)
        {
            if (_state != null)
            {
                _eventAggregator.GetEvent<SignalRStateAddedEvent>().Publish(_state);
            }

        }

        private void SupplierEnableDisable(Supplier obj)
        {
            if (obj != null)
            {
                _logger.LogInformation("Supplier enable disabled event");

                _eventAggregator.GetEvent<SignalRSupplierEnableDisableEvent>().Publish(obj);
            }
        }

        private void SupplierAdded(Supplier obj)
        {
            if (obj != null)
            {
                _logger.LogInformation("Supplier new was added");

                _eventAggregator.GetEvent<SignalRSupplierAddedEvent>().Publish(obj);
            }
        }

        private void NewStoreAdded(Store obj)
        {
            if (obj == null)
            {
                _logger.LogWarning("New Store was added , but is null ");

                return;
            }

            _logger.LogInformation($"New store added {obj.Name}");

            _eventAggregator.GetEvent<SignalRStoreAddedEvent>().Publish(obj);
        }

        private void UserAdded(User user)
        {
            _logger.LogInformation($"Recieved the {nameof(AppConstants.USER_ADDED)} event");

            if (user != null)
            {
                //_logger.LogInformation($"Publishing the {nameof()}");
                _eventAggregator.GetEvent<UserAddedEvent>().Publish(user);
            }
            else
            {
                _logger.LogWarning("Recieved the null object for new user added");
            }
        }

        private void ProductTypeEnableDisable(ProductType productType)
        {
            if (productType != null)
            {

                _eventAggregator.GetEvent<SignalRProductTypeEnableDisableEvent>().Publish(productType);

                ShowNotification<ProductType>(NotificationType.DepartmentEnabledDisabled, productType);
            }
        }

        private async Task UserDisabled()
        {
            try
            {

                _logger?.LogInformation("User account was disabled by admin");

                var _result = await _ProgressService.ConfirmOkAsync("Account Disabled !!", "Your account has been disabled by admin, Please contact admin");

                if (_result == 1)
                {

                    await DisconnectFromHubAsync();

                    Environment.Exit(0);
                }
            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in user disabled event handler", _ex);
            }
        }

        private void ProductTypeAdded(ProductType productType)
        {


            if (productType != null)
            {
                //publish event                                 
                _eventAggregator.GetEvent<SignalRProductTypeAddedEvent>().Publish(productType);
                ShowNotification<ProductType>(NotificationType.DepartmentAdded, productType);

            }
        }

        private void CategoryAdded(Category category)
        {
            if (category != null)
            {
                //publish event                                 
                _eventAggregator.GetEvent<SignalRCategoryAddedEvent>().Publish(category);
                ShowNotification<Category>(NotificationType.CategoryAdded, category);

            }
        }

        private void BrandEnableDisable(Manufacturer manufacturer)
        {
            if (manufacturer != null)
            {
                _eventAggregator.GetEvent<SignalRBrandEnableDisableEvent>().Publish(manufacturer);
                ShowNotification<Manufacturer>(NotificationType.BrandEnabledDisabled, manufacturer);
            }
        }

        protected async Task StartHubInternal()
        {
            try
            {
                _logger.LogInformation("Starting Hub internal....");

                await _hubConnection.StartAsync();
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in starting hub internal", _ex);
            }
        }

        public bool IsConnected()
        {
            return _hubConnection?.State ==
                HubConnectionState.Connected;
        }

        private void IndentAdded(IndentViewModel obj)
        {
            if (obj != null)
            {
                _logger.LogInformation("indent new was added");


            }
        }


        private void InventroyAdded(IndentViewModel obj)
        {
            if (obj != null)
            {
                _logger.LogInformation("indent new was added");

                _eventAggregator.GetEvent<SignalRIndentAddedEvent>().Publish(obj);
                _eventAggregator.GetEvent<SignalRIndentStatusEvent>().Publish(obj);
            }
        }

        private void IndentStatusChange(IndentSignalrViewModel obj)
        {
            if (obj != null)
            {
                _logger.LogInformation("indent status change");
                IndentViewModel indent = new IndentViewModel();
                indent.Id = obj.Id;
                indent.PoNumber = obj.PoNumber;
                indent.Date = obj.Date;
                indent.Status = obj.Status;
                //indent.Remark = obj.Remark;
                indent.Type = obj.Type;
                indent.StoreName = obj.StoreName;
                indent.CreatedBy = obj.CreatedBy;
                indent.Products = obj.Products;
                indent.AcivityLogs = obj.AcivityLogs;
                indent.SupplierDetails = obj.SupplierDetails;
                indent.TrackingId = obj.TrackingId;
                indent.CreditPeriod = obj.CreditPeriod;
                indent.ArrivingDate = obj.ArrivingDate;
                indent.SupplierName = obj.SupplierName;
                indent.IndentInventoryDetails = obj.IndentInventoryDetails;
                indent.IsFullPaid = obj.IsFullPaid;
                //indent.SelectedBankDetail = obj.SelectedBankDetail;
                indent.AdvisoryCharges = obj.AdvisoryCharges;
                indent.FileAttachments = obj.FileAttachments;
                indent.PaymentsList = obj.PaymentsList;
                _eventAggregator.GetEvent<SignalRIndentStatusEvent>().Publish(indent);

            }
        }

        private void IndentStatusChangeToIntransite(IndentViewModel obj)
        {
            if (obj != null)
            {
                _logger.LogInformation("indent status change");
                _eventAggregator.GetEvent<SignalRIndentStatusEvent>().Publish(obj);
                _eventAggregator.GetEvent<SignalRIndentStatusToInTransiteEvent>().Publish(obj);
                _eventAggregator.GetEvent<SignalRIndentStatusEvent>().Publish(obj);
            }
        }

        private void IndentStatusChangeFromSahaya(string status)
        {
            _logger.LogInformation("indent status change to sahaya pending. So, refresh Indents in Bulk Payments and Add Inventory");
            // #todo - inventory page reload
            // _eventAggregator.GetEvent<IndentStatusChangeToSahayaPendingEvent>().Publish();
            _eventAggregator.GetEvent<IndentStatusChangeFromSahayaInFinance>().Publish();
        }

        private void StockTransferStatusChangeFromSahaya(bool status)
        {
            try
            {
                object productType = new object();

                //IndentViewModel obj = new IndentViewModel();

                //_eventAggregator.GetEvent<SignalRIndentStatusEvent>().Publish(obj);

                //_eventAggregator.GetEvent<StockTransferEvent>().Publish();

                _eventAggregator.GetEvent<SignalRReloadStockTransferListForSahayaEvent>().Publish(productType);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
           
      }

        private void ProductAdded(ProductDetails Product)
        {
            if (Product != null)
            {
                _logger.LogInformation("Product Added");
                _eventAggregator.GetEvent<SignalRNewProductAddEvent>().Publish(Product);
                ShowNotification<ProductDetails>(NotificationType.ProductAdded, Product);
            }
        }

        public void ShowNotification<T>(NotificationType notificationType, T Payload)
        {
            string _timeText = "a minutes ago";
            switch (notificationType)
            {
                case NotificationType.ProductAdded:
                    var Product = Payload as ProductDetails;
                    _eventAggregator.GetEvent<NotificationEvent>().Publish(new Entites.Notification.POSNotification()
                    {
                        Label = "PRODUCT",
                        PrimaryText = $"{Product.Name} ({Product.ProductSKU}) Created",
                        SecondaryText = $"{Product.ProductType.Name} + {Product.Manufacturer.Name}",
                        Timetext = _timeText,
                        ReceivedTime = Product.CreateDatetime
                    });
                    break;
                case NotificationType.DepartmentAdded:
                    var ProductType = Payload as ProductType;
                    _eventAggregator.GetEvent<NotificationEvent>().Publish(new Entites.Notification.POSNotification()
                    {
                        Label = "Subcategory",
                        PrimaryText = $"{ProductType.Name} ({ProductType.DeptCode}) Created",
                        //SecondaryText = $"{Product.ProductType.Name} + {Product.Manufacturer.Name}",
                        Timetext = _timeText,
                        ReceivedTime = ProductType.Createddate
                    });
                    break;

                case NotificationType.BrandAdded:
                    var Brand = Payload as ProductTypeManufacturer;
                    _eventAggregator.GetEvent<NotificationEvent>().Publish(new Entites.Notification.POSNotification()
                    {
                        Label = "BRAND",
                        PrimaryText = $"{Brand.Name} Created",
                        SecondaryText = $"{Brand.ProductTypeManufactrerShortCode}",
                        Timetext = _timeText,
                        ReceivedTime = Brand.CreateDatetime
                    });
                    break;
                case NotificationType.BrandEnabledDisabled:
                    var brand = Payload as Manufacturer;
                    var enableordisable = brand.Isenabled == true ? "Enabled" : "Disabled";
                    _eventAggregator.GetEvent<NotificationEvent>().Publish(new Entites.Notification.POSNotification()
                    {
                        Label = "BRAND",
                        PrimaryText = $"{brand.Name + " " + enableordisable}  ",
                        SecondaryText = "",
                        Timetext = _timeText,
                        ReceivedTime = DateTime.Now
                    });
                    break;
                case NotificationType.ProductEnableDisbale:
                    var productenable = Payload as ProductDetails;
                    var enableordisblae = productenable.Isenabled == true ? "Enabled" : "Disabled";
                    _eventAggregator.GetEvent<NotificationEvent>().Publish(new Entites.Notification.POSNotification()
                    {
                        Label = "PRDOUCT",
                        PrimaryText = $"{productenable.Name + " " + enableordisblae}  ",
                        SecondaryText = "",
                        Timetext = _timeText,
                        ReceivedTime = DateTime.Now
                    });
                    break;
                case NotificationType.UpdatedSellingPrice:
                    var update = Payload as StockTrnasferModel;
                    _eventAggregator.GetEvent<NotificationEvent>().Publish(new Entites.Notification.POSNotification()
                    {
                        Label = "UpdatedSellingPrice",
                        PrimaryText = $"{update.TransferOrderNo} Updated",
                        SecondaryText = "",
                        Timetext = _timeText,
                        ReceivedTime = DateTime.Now,
                    });
                    break;
                case NotificationType.CreateSKU:
                    var request = Payload as List<SKUModel>;
                    _eventAggregator.GetEvent<NotificationEvent>().Publish(new Entites.Notification.POSNotification()
                    {
                        Label = "New SKU request created",
                        PrimaryText = $"{request.FirstOrDefault()?.Producttype} Request Created",
                        SecondaryText = "",
                        Timetext = _timeText,
                        ReceivedTime = DateTime.Now,
                    });
                    break;
                case NotificationType.ApproveSKU:
                    var approve = Payload as SKUModel;
                    _eventAggregator.GetEvent<NotificationEvent>().Publish(new Entites.Notification.POSNotification()
                    {
                        Label = "SKU request approved",
                        PrimaryText = $"{approve.Producttype} Request Approved",
                        SecondaryText = "",
                        Timetext = _timeText,
                        ReceivedTime = DateTime.Now,
                    });
                    break;
                case NotificationType.DepartmentEnabledDisabled:
                    var producttypeenable = Payload as ProductType;
                    var enableordisblaeproductype = producttypeenable.Isenabled == true ? "Enabled" : "Disabled";
                    _eventAggregator.GetEvent<NotificationEvent>().Publish(new Entites.Notification.POSNotification()
                    {
                        Label = "Subcategory",
                        PrimaryText = $"{producttypeenable.Name + " " + enableordisblaeproductype}  ",
                        SecondaryText = "",
                        Timetext = _timeText,
                        ReceivedTime = DateTime.Now
                    });
                    break;
                case NotificationType.CategoryAdded:
                    var categoryadded = Payload as Category;
                    _eventAggregator.GetEvent<NotificationEvent>().Publish(new Entites.Notification.POSNotification()
                    {
                        Label = "Category",
                        PrimaryText = $"{categoryadded.Name} Created",
                        SecondaryText = "",
                        Timetext = _timeText,
                        ReceivedTime = DateTime.Now
                    });
                    break;

                default:
                    break;
            }


        }


        public enum NotificationType
        {
            ProductAdded = 1,
            BrandAdded = 2,
            DepartmentAdded = 3,
            ProductEnableDisbale = 4,
            UpdatedSellingPrice = 5,
            CreateSKU = 6,
            ApproveSKU = 7,
            DepartmentEnabledDisabled = 8,
            BrandEnabledDisabled = 9,
            CategoryAdded = 10
        }
        private void BrandAdded(ProductTypeManufacturer Brand)
        {
            if (Brand != null)
            {
                _logger.LogInformation("Brand Added");

                _eventAggregator.GetEvent<SignalRNewBrandAddEvent>().Publish(Brand);
                ShowNotification<ProductTypeManufacturer>(NotificationType.BrandAdded, Brand);
            }
        }

        private void ProductEnableDisbale(ProductDetails product)
        {
            if (product != null)
            {
                _logger.LogInformation("Prodduct enable or disable");

                _eventAggregator.GetEvent<SignalRProductEnableDisableAddEvent>().Publish();
                ShowNotification<ProductDetails>(NotificationType.ProductEnableDisbale, product);
            }
        }

        private void SupplierShippingAddressAdded(Supplier supplier)
        {
            if (supplier != null)
            {
                _logger.LogInformation("Supplier shipping address added");

                _eventAggregator.GetEvent<SignalRSupplierShippingAddressAddEvent>().Publish();

            }
        }

        private void NewBanksAdded(string BankName)
        {
            if (BankName != null)
            {
                _logger.LogInformation("Bank added");

                _eventAggregator.GetEvent<AddNewBankEvent>().Publish();

            }
        }


        private void StockTransferRequest(StockTrnasferModel TransferOrderNo)
        {
            if (TransferOrderNo != null)
            {
                _logger.LogInformation("Stock Transfer Request sent signalr");

                _eventAggregator.GetEvent<StockRequestEvent>().Publish();
            }
        }

        private void StockTransferFrom(StockTrnasferModel TransferOrderNo)
        {
            if (TransferOrderNo != null)
            {

                _logger.LogInformation("Stock Transfer signalr");

                _eventAggregator.GetEvent<StockTransferEvent>().Publish();

            }
        }

        private void UpdatedSellingPrice(StockTrnasferModel invoiceViewModel)
        {
            if (invoiceViewModel != null)
            {

                _logger.LogInformation("Stock Transfer signalr");
                ShowNotification<StockTrnasferModel>(NotificationType.UpdatedSellingPrice, invoiceViewModel);



            }
        }

        private void CreateSKU(List<SKUModel> sKUModels)
        {
            if (sKUModels != null)
            {

                _logger.LogInformation("Create SKU signalr");
                ShowNotification<List<SKUModel>>(NotificationType.CreateSKU, sKUModels);


            }
        }

        private void SKURequestApprove(SKUModel sKUModels)
        {
            if (sKUModels != null)
            {

                _logger.LogInformation("Approved request SKU signalr");
                ShowNotification<SKUModel>(NotificationType.ApproveSKU, sKUModels);


            }
        }

        private void UpdateMapStore(bool update) {
            if (update) {

                _logger.LogInformation("Update map store ");
                _eventAggregator.GetEvent<MapStore>().Publish(update);
            }
        }
    }
}
