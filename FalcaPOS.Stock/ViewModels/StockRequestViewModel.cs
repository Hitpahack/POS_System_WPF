using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Stock;
using FalcaPOS.Entites.Stores;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using Unity;
using Unity.Resolution;

namespace FalcaPOS.Stock.ViewModels
{
    public class StockRequestViewModel : BindableBase
    {

        private readonly IUnityContainer _container;

        public DelegateCommand AddProductCardCommand { get; private set; }

        public DelegateCommand<object> RemoveProductCardCommand { get; private set; }

        public DelegateCommand CreateRequestCommand { get; private set; }

        public DelegateCommand ClearRequestCommand { get; private set; }

        private readonly IIndentService _indentService;

        private readonly INotificationService _notificationService;

        private readonly ProgressService _progressService;

        private IEventAggregator _eventAggregator;

        private readonly IStockTransferService _stockTransferService;

        private readonly IStoreService storeService;

        public DelegateCommand ToStoreSelectionChangeCommand { get; private set; }

        private readonly Logger logger;

        public DelegateCommand FromStoreSelectionChangeCommand { get; private set; }
        public StockRequestViewModel(IStoreService StoreService, Logger Logger, IStockTransferService StockTransferService, IUnityContainer container, IIndentService indentService, INotificationService notificationService, ProgressService progressService, IEventAggregator eventAggregator)
        {



            storeService = StoreService ?? throw new ArgumentNullException(nameof(StoreService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _container = container ?? throw new ArgumentNullException(nameof(container));

            StockProducts = new ObservableCollection<StockProductCardViewModel>();

            logger = Logger ?? throw new ArgumentNullException(nameof(Logger));

            StockProducts.CollectionChanged -= Products_CollectionChanged;

            StockProducts.CollectionChanged += Products_CollectionChanged;

            AddProductCardCommand = new DelegateCommand(AddNewProduct);

            RemoveProductCardCommand = new DelegateCommand<object>(RemoveIndentProduct);

            CreateRequestCommand = new DelegateCommand(CreateRequest);

            ClearRequestCommand = new DelegateCommand(ClearRequest);

            _indentService = indentService ?? throw new ArgumentNullException();

            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            double userheightscreen = System.Windows.SystemParameters.PrimaryScreenHeight;

            _stockTransferService = StockTransferService ?? throw new ArgumentNullException(nameof(StockTransferService));

            MaxHeight = userheightscreen - 150;

            // Checks if the logged in user is TM or not.
            IsTerritoryManager = (AppConstants.USER_ROLES[0] == AppConstants.ROLE_TERRITORY_MANAGER) ? true : false;

            LoadStores();

            STNumber = null;

            // Fetches the SR number based on the To Store selected.
            ToStoreSelectionChangeCommand = new DelegateCommand(GetCurrentSRNumberAsync);

            CheckRoleAndFetchSRNumber();

            FromStoreSelectionChangeCommand = new DelegateCommand(FromStoreSelectionChange);
        }

        private async void GetCurrentSRNumberAsync()
        {

            try
            {
                STNumber = null;

                await Task.Run(async () =>
                {
                    var _result = await _stockTransferService.GetCurrentStockTransferNumber(IsTerritoryManager? (int)SelectedToStore?.StoreId: AppConstants.LoggedInStoreInfo.StoreId);

                    Application.Current.Dispatcher?.Invoke(() =>
                    {
                        if (_result != null && _result.IsSuccess && _result.Data.IsValidString())
                        {
                            STNumber = _result.Data;
                        }
                    });

                });

                if (AppConstants.USER_ROLES[0]== AppConstants.ROLE_TERRITORY_MANAGER)
                {
                    if(SelectedFromStore!=null && SelectedToStore != null)
                    {
                        if (SelectedFromStore.Name == SelectedToStore.Name)
                        {
                            SelectedFromStore = null;
                        }
                    }
                    
                }
            }
            catch (Exception _ex)
            {
                logger.LogError("Getting error in stock receive cheque", _ex);
            }

        }

        private string _stNumber;
        public string STNumber
        {
            get { return _stNumber; }
            set { SetProperty(ref _stNumber, value); }
        }

        public string STDate
        {
            get { return DateTime.Now.ToString("dd-MM-yyyy"); }
            set { }

        }

        private void ClearRequest()
        {
            StockProducts?.Clear();
            SelectedFromStore = null;
            SelectedToStore = null;
            STNumber = null;
            CheckRoleAndFetchSRNumber();
        }

        /// <summary>
        /// Checks the role and fetches the SR number.
        /// </summary>
        private void CheckRoleAndFetchSRNumber()
        {
            // If the role is not TM, fetches the current SR number.
            if (!IsTerritoryManager)
            {
                GetCurrentSRNumberAsync();
            }
        }

        private async void CreateRequest()
        {

            try
            {
                bool _isValidIndent = Validate();

                if (!_isValidIndent) return;

                if (StockProducts == null)
                {
                    return;
                }

                List<StockTransferProductModel> stockProducts = new List<StockTransferProductModel>();

                if (StockProducts != null && StockProducts.Count > 0)
                {
                    foreach (var item in StockProducts)
                    {
                        stockProducts.Add(new StockTransferProductModel()
                        {
                            ProductId = (int)item.SelectedProduct.ProductId,

                            TransferQty = item.Quantity

                        });
                    }

                }

                StockTranferRequestModel TrnasferModel = new StockTranferRequestModel()
                {
                    TransferOrderNo = STNumber,
                    FromStoreId = SelectedFromStore.StoreId,
                    Date = STDate,
                    stockTransferProducts = stockProducts,
                    ToStoreId = AppConstants.USER_ROLES[0] == AppConstants.ROLE_TERRITORY_MANAGER ? SelectedToStore.StoreId : AppConstants.LoggedInStoreInfo.StoreId,
                    
                };

                await _progressService.StartProgressAsync();

                await Task.Run(async () =>
                {

                    var _result = await _stockTransferService.StockRequest(TrnasferModel);

                    Application.Current?.Dispatcher?.Invoke(async () =>
                    {

                        if (_result != null && _result.IsSuccess)
                        {
                            _notificationService.ShowMessage(_result?.Data ?? "Request sent successfully.!!!", Common.NotificationType.Success);

                            ClearProductCard();
                        }

                        await _progressService.StopProgressAsync();

                    });


                });
            }
            catch (Exception _ex)
            {
                logger.LogError("Getting error in stock request", _ex);
            }

        }



        private bool Validate()
        {

            if (SelectedFromStore == null)
            {
                _notificationService.ShowMessage("Please select from store", Common.NotificationType.Error);

                return false;
            }

            if ( IsTerritoryManager && SelectedToStore == null)
            {
                _notificationService.ShowMessage("Please select to store", Common.NotificationType.Error);

                return false;
            }


            if (IsTerritoryManager && SelectedToStore.Name == SelectedFromStore.Name)
            {
                _notificationService.ShowMessage("From Store and To Store cannot be the same", Common.NotificationType.Error);

                return false;
            }

            //Required atleast one product
            if (StockProducts == null || StockProducts.Count <= 0)
            {
                _notificationService.ShowMessage("Please add one or more products", Common.NotificationType.Error);

                return false;
            }


            foreach (var _product in StockProducts)
            {
                bool _isValidProduct = ValidateProduct(_product);

                if (!_isValidProduct)
                {
                    return false;
                }

            }

            //check if duplicates are present 

            var _result = StockProducts.GroupBy(x => x.SelectedProduct.ProductId).Select(x => x).Where(x => x.Count() > 1);

            if (_result != null && _result.Count() > 0)
            {
                _notificationService.ShowMessage($"Duplicate product {_result?.FirstOrDefault()?.FirstOrDefault()?.SelectedProduct?.Name} added, remove the product or change the  product quantity", Common.NotificationType.Error);

                return false;

            }

            return true;
        }

        private bool ValidateProduct(StockProductCardViewModel product)
        {
            if (product == null)
            {
                _notificationService.ShowMessage("Invalid product", Common.NotificationType.Error);

                return false;
            }

            if (product.SelectedProduct == null)
            {
                _notificationService.ShowMessage("Please select a product", Common.NotificationType.Error);

                product.HasError = true;

                return false;

            }
            if (product.Quantity <= 0)
            {
                _notificationService.ShowMessage("Please enter valid  product quantity", Common.NotificationType.Error);

                product.HasError = true;

                return false;

            }
            if (product.Quantity>product.AvailableQty)
            {
                _notificationService.ShowMessage("Request qty should not allow more than available stock qty", Common.NotificationType.Error);

                product.HasError = true;

                return false;

            }


            return true;
        }

        private void RemoveIndentProduct(object obj)
        {
            if (obj is Guid _productGUIDId)
            {
                var _productRemove = StockProducts?.FirstOrDefault(x => x.IndentProductGUIDId == _productGUIDId);

                if (_productRemove != null)
                {
                    StockProducts.Remove(_productRemove);
                }
            }
        }

        private void AddNewProduct()
        {
            if(SelectedFromStore != null)
            {
                var _product = _container.Resolve<StockProductCardViewModel>(new ParameterOverride("FromStoreId", SelectedFromStore.StoreId));

                StockProducts.Add(_product);
            }

          
        }

        private void Products_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("StockProducts");
            _eventAggregator.GetEvent<ParentDataEvent>().Publish(StockProducts);
        }

        private ObservableCollection<StockProductCardViewModel> _stockProducts;
        public ObservableCollection<StockProductCardViewModel> StockProducts
        {
            get { return _stockProducts; }
            set { SetProperty(ref _stockProducts, value); }
        }

        private double _maxheight;

        public double MaxHeight
        {
            get { return _maxheight; }
            set
            {
                _maxheight = value;
                RaisePropertyChanged("MaxHeight");
            }
        }

        private ObservableCollection<Store> stores;
        public ObservableCollection<Store> Stores
        {
            get { return stores; }
            set { SetProperty(ref stores, value); }
        }

        private Store selectedFromStore;
        public Store SelectedFromStore
        {
            get { return selectedFromStore; }
            set { 

                SetProperty(ref selectedFromStore, value);
                StockProducts?.Clear();
            }
        }

        private Store selectedToStore;
        public Store SelectedToStore
        {
            get { return selectedToStore; }
            set { 
                SetProperty(ref selectedToStore, value); 
                StockProducts?.Clear();
            }
        }


        private async void LoadStores()
        {
            if (AppConstants.USER_ROLES != null && AppConstants.USER_ROLES.Any())
            {
                if (IsTerritoryManager)
                {
                    await Task.Run(async () =>
                    {
                        var _resultForTM = await storeService.GetStoreDetailsbyuser(AppConstants.UserId, AppConstants.USER_ROLES[0]);

                        if (_resultForTM != null && _resultForTM.Any())
                        {
                            Application.Current?.Dispatcher.Invoke(() =>
                            {
                                Stores = new ObservableCollection<Entites.Stores.Store>(_resultForTM.OrderBy(x => x.Name).ToList());
                            });
                        }

                    });
                }
                else
                {
                    await Task.Run(async () =>
                    {
                        var _result = await storeService.GetStores();
                        if (_result != null)
                            Stores = new ObservableCollection<Store>(_result.Where(x => x.Name != AppConstants.StoreName && x.Parent_ref == null).ToList());
                    });
                }

            }

           
        }

        private void ClearProductCard()
        {
            StockProducts?.Clear();
            SelectedFromStore = null;
            SelectedToStore = null;
            STNumber = null;
            CheckRoleAndFetchSRNumber();

        }

        private bool _isTerritoryManager;

        public bool IsTerritoryManager
        {
            get { return _isTerritoryManager; }
            set { SetProperty(ref _isTerritoryManager, value); }
        }

        public void FromStoreSelectionChange()
        {
            try
            {
                if (AppConstants.USER_ROLES[0] == AppConstants.ROLE_TERRITORY_MANAGER)
                {
                    if (SelectedToStore != null && SelectedFromStore!=null)
                    {
                        if (SelectedFromStore.Name == SelectedToStore.Name)
                        {
                            SelectedToStore = null;
                        }
                    }
                    
                }
            }
            catch(Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
        }

    }
}







