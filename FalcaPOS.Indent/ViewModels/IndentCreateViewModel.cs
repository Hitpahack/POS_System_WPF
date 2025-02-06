using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Indent;
using FalcaPOS.Entites.Stores;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Telerik.Windows.Documents.Spreadsheet.Expressions.Functions;
using Unity;

namespace FalcaPOS.Indent.ViewModels
{
    public class IndentCreateViewModel : BindableBase
    {
        private readonly IUnityContainer _container;

        public DelegateCommand AddIndentProductCardCommand { get; private set; }

        public DelegateCommand<object> RemoveIndentProductCardCommand { get; private set; }

        public DelegateCommand CreateIndentCommand { get; private set; }

        public DelegateCommand ClearIndentCommand { get; private set; }

        private readonly IIndentService _indentService;

        private readonly INotificationService _notificationService;

        private readonly ProgressService _progressService;

        private IEventAggregator _eventAggregator;

        private readonly Logger _logger;

        private readonly IStoreService _storeService;

        public DelegateCommand StoreSelectionChangeCommand { get; private set; }

        public IndentCreateViewModel(IUnityContainer container, Logger Logger, IIndentService indentService, INotificationService notificationService, ProgressService progressService, IEventAggregator eventAggregator, IStoreService storeService)
        {

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _container = container ?? throw new ArgumentNullException(nameof(container));

            IndentProducts = new ObservableCollection<IndentProductCardViewModel>();

            IndentProducts.CollectionChanged -= IndentProducts_CollectionChanged;

            IndentProducts.CollectionChanged += IndentProducts_CollectionChanged;

            AddIndentProductCardCommand = new DelegateCommand(AddNewIndentProduct);

            RemoveIndentProductCardCommand = new DelegateCommand<object>(RemoveIndentProduct);

            CreateIndentCommand = new DelegateCommand(CreateIndent);

            ClearIndentCommand = new DelegateCommand(ClearIndent);

            _indentService = indentService ?? throw new ArgumentNullException();

            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));

            IsStoreVisibility = AppConstants.USER_ROLES[0] == AppConstants.ROLE_STORE_PERSON ? false : true;

            GetCurrentPONumberAsync();

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            Type = "Retail Order";

            double userHeightScreen = System.Windows.SystemParameters.PrimaryScreenHeight;

            MaxHeight = userHeightScreen - 150;


            _logger = Logger ?? throw new ArgumentNullException(nameof(Logger));

            SlNo = 1;

            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));
            GetStores();

            StoreSelectionChangeCommand = new DelegateCommand(GetCurrentPONumberAsync);

            _eventAggregator.GetEvent<MapStore>().Subscribe(x => GetStores());



        }

        private async void GetCurrentPONumberAsync()
        {

            try
            {
                PONumber = null;
                int StoreId = IsStoreVisibility == false ? AppConstants.LoggedInStoreInfo.StoreId : SelectedStore != null ? SelectedStore.StoreId : -1;
                if (StoreId != -1)
                {
                    await Task.Run(async () =>
                    {

                        var _result = await _indentService.GetCurrentPONumber(StoreId);

                        Application.Current.Dispatcher?.Invoke(() =>
                        {
                            if (_result != null && _result.IsSuccess && _result.Data.IsValidString())
                            {
                                PONumber = _result.Data;
                            }
                        });

                    });
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }

        }


        private string _type;

        public string Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }


        private string _poNumber;
        public string PONumber
        {
            get { return _poNumber; }
            set { SetProperty(ref _poNumber, value); }
        }

        public string PODate
        {
            get { return DateTime.Now.ToString("dd-MM-yyyy"); }
            set { }

        }

        private void ClearIndent()
        {
            if (AppConstants.USER_ROLES[0] == AppConstants.ROLE_STORE_PERSON)
            {
                IndentProducts?.Clear();
                Type = null;
                GetCurrentPONumberAsync();
                SlNo = 1;
            }
            else
            {
                IndentProducts?.Clear();
                Type = null;
                SelectedStore = null;
                PONumber = null;
                SlNo = 1;
            }


        }

        private async void CreateIndent()
        {

            try
            {
                bool _isValidIndent = ValidateIndent();

                if (!_isValidIndent) return;


                var _indentList = GetIndentModel();


                if (_indentList == null || _indentList.Products == null || !_indentList.Products.Any())
                {
                    return;
                }

                await _progressService.StartProgressAsync();



                await Task.Run(async () =>
                {

                    var _result = await _indentService.CreateIndent(_indentList);

                    Application.Current?.Dispatcher?.Invoke(async () =>
                    {

                        if (_result != null && _result.IsSuccess)
                        {
                            _notificationService.ShowMessage(_result?.Data ?? "Indent created successfully.!!!", Common.NotificationType.Success);
                            ClearIndent();
                            Type = "Retail Order";


                        }
                        else
                        {
                            _notificationService.ShowMessage(_result?.Error ?? "An error occurred, try again", Common.NotificationType.Error);
                        }

                        await _progressService.StopProgressAsync();
                    });

                });
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }

        }

        private IndentCreateModel GetIndentModel()
        {
            return new IndentCreateModel
            {
                PoNumber = PONumber,
                Type = Type,
                Products = GetIndentProducts(),
                StoreId = AppConstants.USER_ROLES[0] == AppConstants.ROLE_STORE_PERSON ? AppConstants.LoggedInStoreInfo.StoreId : SelectedStore.StoreId,
            };


        }

        private IEnumerable<IndentProduct> GetIndentProducts()
        {
            foreach (var _product in IndentProducts)
            {
                if (_product == null || _product.SelectedProduct == null) continue;

                yield return new IndentProduct
                {
                    ProductId = (int)_product.SelectedProduct.ProductId,
                    Quantity = _product.Quantity
                };
            }

        }

        private bool ValidateIndent()
        {
            if (IsStoreVisibility)
            {
                if (SelectedStore == null)
                {
                    _notificationService.ShowMessage("Please select Store", Common.NotificationType.Error);
                    return false;

                }


            }

            if (Type == null)
            {
                _notificationService.ShowMessage("Please select PO type", Common.NotificationType.Error);
                return false;

            }
            //Required atleast one product
            if (IndentProducts == null || IndentProducts.Count <= 0)
            {
                _notificationService.ShowMessage("Please add one or more products", Common.NotificationType.Error);

                return false;
            }

            foreach (var _product in IndentProducts)
            {
                bool _isValidProduct = ValidateIndentProduct(_product);

                if (!_isValidProduct)
                {
                    return false;
                }

            }

            //check if duplicates are present 

            var _result = IndentProducts.GroupBy(x => x.SelectedProduct.ProductId).Select(x => x).Where(x => x.Count() > 1);

            if (_result != null && _result.Count() > 0)
            {
                _notificationService.ShowMessage($"Duplicate product {_result?.FirstOrDefault()?.FirstOrDefault()?.SelectedProduct?.Name} added, remove the product or change the  product quantity", Common.NotificationType.Error);

                return false;

            }

            return true;
        }

        private bool ValidateIndentProduct(IndentProductCardViewModel product)
        {
            if (product == null)
            {
                _notificationService.ShowMessage("Invalid Indent product", Common.NotificationType.Error);

                return false;
            }

            if (product.SelectedProduct == null)
            {
                _notificationService.ShowMessage($"Please select a product at row "+ product.SlNo, Common.NotificationType.Error);
                product.HasError = true;
                return false;
            }

         
                foreach (var p in IndentProducts)
                {
                  
                    if (p.SelectedProduct == null)
                    {
                        _notificationService.ShowMessage($"Please select a product at row "+ p.SlNo, Common.NotificationType.Error);
                        p.HasError = true;
          
                    }
                }



            if (AppConstants.USER_ROLES[0] != AppConstants.ROLE_PURCHASE_MANAGER)
            {
                if (product.Quantity <= 0)
                {
                    _notificationService.ShowMessage("Please enter valid product quantity at row  " + product.SlNo, Common.NotificationType.Error);

                    product.HasError = true;

                    return false;

                }   
            }

            return true;
        }

        private void RemoveIndentProduct(object obj)
        {
            if (obj is Guid _productGUIDId)
            {
                var _productRemove = IndentProducts?.FirstOrDefault(x => x.IndentProductGUIDId == _productGUIDId);

                // Checks if the product to be removed is not equal to null
                if (_productRemove != null)
                {
                    // remove product from IndentProducts.
                    IndentProducts.Remove(_productRemove);

                    // adjust the serial number for IndentProducts.
                    for (int i = 0; i < IndentProducts.Count; i++)
                    {
                        IndentProducts[i].SlNo = i + 1;
                    }

                    // if num of IndentProducts equals zero, SlNo is set to 1.
                    if (IndentProducts.Count == 0)
                    {
                        
                        SlNo = 1;
                    }
                    // if num of IndentProducts not equals zero, SlNo is set to IndentProducts.Count + 1.
                    else
                    {
                        SlNo = IndentProducts.Count + 1;
                    }
                }
                
            }
        }

        private void AddNewIndentProduct()
        {
            try
            {
                if (IsStoreVisibility)
                {
                    if (SelectedStore == null)
                    {
                        _notificationService.ShowMessage("Please select Store", Common.NotificationType.Error);
                        return;

                    }

                    if (Type == null)
                    {
                        _notificationService.ShowMessage("Please select PO type", Common.NotificationType.Error);
                        return;

                    }
                }


                var _product = _container.Resolve<IndentProductCardViewModel>();
                _product.SlNo = SlNo;
                IndentProducts.Add(_product);
                SlNo++;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }


        }

        private void IndentProducts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("IndentProducts");
        }

        private ObservableCollection<IndentProductCardViewModel> _indentProducts;
        public ObservableCollection<IndentProductCardViewModel> IndentProducts
        {
            get { return _indentProducts; }
            set { SetProperty(ref _indentProducts, value); }
        }

        private double _maxHeight;

        public double MaxHeight
        {
            get => _maxHeight;
            set
            {
                _maxHeight = value;
                RaisePropertyChanged("MaxHeight");
            }
        }

        private int _slNo;

        public int SlNo
        {
            get => _slNo;
            set => SetProperty(ref _slNo, value);
        }

        private ObservableCollection<Store> _stores;
        public ObservableCollection<Store> Stores
        {
            get => _stores;
            set => SetProperty(ref _stores, value);
        }

        private Store _selectedStore;
        public Store SelectedStore
        {
            get => _selectedStore;
            set => SetProperty(ref _selectedStore, value);

        }

        private bool _isStoreVisibility;

        public bool IsStoreVisibility
        {
            get => _isStoreVisibility;
            set => SetProperty(ref _isStoreVisibility, value);
        }

        public async void GetStores()
        {
            try
            {
                await Task.Run(async () => {
                    var _result = await _storeService.GetStoreDetailsbyuser(AppConstants.UserId, AppConstants.USER_ROLES[0]);
                    Application.Current?.Dispatcher?.Invoke(() =>
                    {                     
                            Stores = new ObservableCollection<Store>(_result.OrderBy(x=>x.Name));                       
                    });
                    //var _result = await _storeService.GetStores();

                    //if (_result != null && _result.Count() > 0) {
                    //    Application.Current?.Dispatcher?.Invoke(() => {

                    //        if (AppConstants.ROLE_TERRITORY_MANAGER == AppConstants.USER_ROLES[0] || AppConstants.ROLE_REGIONAL_MANAGER == AppConstants.USER_ROLES[0]) {
                    //           //taking only territory or reginal stores
                    //            _result = _result.Where(x => x.Parent_ref == null);
                    //            var _storeMap = new List<Store>();
                    //            foreach (var item in _result) {
                    //                if (item.User_ref.FirstOrDefault(x => x.Value == AppConstants.UserId) != null)
                    //                    _storeMap.Add(item);

                    //            }
                    //            Stores = new ObservableCollection<Store>(_storeMap);
                    //        }
                    //        else
                    //            Stores = new ObservableCollection<Store>(_result.Where(x => x.Parent_ref == null));

                    //    });
                    //}





                });

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }


    }
}
