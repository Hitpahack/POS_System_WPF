using FalcaPOS.Common.Constants;
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
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Unity;

namespace FalcaPOS.Stock.ViewModels
{
    public class RspRequestViewModel : BindableBase
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

        private readonly Logger logger;
        public RspRequestViewModel(IStoreService StoreService, Logger Logger, IStockTransferService StockTransferService, IUnityContainer container, IIndentService indentService, INotificationService notificationService, ProgressService progressService, IEventAggregator eventAggregator)
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

            LoadStores();

            GetCurrentSTNumberAsync();


        }

        private async void GetCurrentSTNumberAsync()
        {

            try
            {
                STNumber = null;

                await Task.Run(async () =>
                {

                    var _result = await _stockTransferService.GetCurrentStockTransferNumber();

                    Application.Current.Dispatcher?.Invoke(() =>
                    {
                        if (_result != null && _result.IsSuccess && _result.Data.IsValidString())
                        {
                            STNumber = _result.Data;
                        }
                    });

                });
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

            SelectedStore = null;

            GetCurrentSTNumberAsync();

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


                List<StockTransferProduct> stockProducts = new List<StockTransferProduct>();

                if (StockProducts != null && StockProducts.Count > 0)
                {
                    foreach (var item in StockProducts)
                    {
                        stockProducts.Add(new StockTransferProduct()
                        {
                            ProductId = (int)item.SelectedProduct.ProductId,

                            TransferQty = item.Quantity

                        });
                    }

                }

                StockTrnasferModel stockTrnasferModel = new StockTrnasferModel()
                {
                    TransferOrderNo = STNumber,
                    FromLocation = SelectedStore.Name,
                    Date = STDate,
                    StockTransferList = stockProducts
                };

                await _progressService.StartProgressAsync();

                await Task.Run(async () =>
                {

                    var _result = await _stockTransferService.RspStockRequest(stockTrnasferModel);

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

            if (SelectedStore == null)
            {
                _notificationService.ShowMessage("Please select to store", Common.NotificationType.Error);

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

            var _product = _container.Resolve<StockProductCardViewModel>();

            StockProducts.Add(_product);
        }

        private void Products_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("StockProducts");
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

        private Store selectedStore;
        public Store SelectedStore
        {
            get { return selectedStore; }
            set { SetProperty(ref selectedStore, value); }
        }

        private async void LoadStores()
        {
            if (AppConstants.USER_ROLES != null && AppConstants.USER_ROLES.Any())
            {


                await Task.Run(async () =>
                {
                    var _result = await storeService.GetStores();
                    if (_result != null)
                        Stores = new ObservableCollection<Store>(_result.Where(x => x.Name != AppConstants.StoreName && x.Parent_ref == AppConstants.LoggedInStoreInfo.StoreId).ToList());
                });


            }


        }

        private void ClearProductCard()
        {
            StockProducts?.Clear();

            SelectedStore = null;

            GetCurrentSTNumberAsync();

        }

    }
}
