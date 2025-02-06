using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Products;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.Stock.ViewModels
{
    public class StockProductCardViewModel : BindableBase
    {
        private readonly IProductService _productService;

        private readonly ISupplierService _supplierService;

        private readonly INotificationService _notificationService;

        private CancellationTokenSource _cancellationTokenSource;

        private readonly Logger _logger;

        public Guid IndentProductGUIDId { get; set; }

        public DelegateCommand<string> SearchTextChangedCommand { get; private set; }

        public DelegateCommand SearchProductSelectionChangedCommand { get; private set; }



        public StockProductCardViewModel(IProductService productService, ISupplierService supplierService, INotificationService notificationService, Logger logger)
        {
            IndentProductGUIDId = Guid.NewGuid();

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _productService = productService ?? throw new ArgumentNullException(nameof(productService));

            _supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));

            SearchTextChangedCommand = new DelegateCommand<string>(SearchTextChanged);

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            SearchProductSelectionChangedCommand = new DelegateCommand(SearchSelectionChanged);

        }

        private ProductViewModel _selectedProduct;
        public ProductViewModel SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);

        }
        private async void SearchSelectionChanged()
        {

            try
            {
                if (SelectedProductSearch == null)
                {
                    SelectedProduct = null;
                    return;
                }


                await Task.Run(async () =>
                {
                    var _result = await _productService.GetSKUNumberProduct(SelectedProductSearch.ProductId,AppConstants.LoggedInStoreInfo.StoreId);

                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        if (_result != null && _result.IsSuccess && _result.Data != null)
                        {
                            SelectedProduct = _result.Data;
                        }
                        else
                        {
                            _notificationService.ShowMessage(_result.Error, NotificationType.Error);
                        }
                    });

                });

            }
            catch (Exception _ex)
            {

                _logger.LogError("Error in getting product information", _ex);
            }

        }


        private bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set
            {
                SetProperty(ref _hasError, value);
            }
        }


        private ProductSearchModel _selectedProductSearch;
        public ProductSearchModel SelectedProductSearch
        {
            get { return _selectedProductSearch; }
            set { SetProperty(ref _selectedProductSearch, value); }
        }

        private ObservableCollection<ProductSearchModel> _productsSearchList;
        public ObservableCollection<ProductSearchModel> ProductsSearchList
        {
            get { return _productsSearchList; }
            set { SetProperty(ref _productsSearchList, value); }
        }

        private bool _isDropDownOpen;
        public bool IsDropDownOpen
        {
            get { return _isDropDownOpen; }
            set { SetProperty(ref _isDropDownOpen, value); }
        }


        private int _quantity;
        public int Quantity
        {
            get { return _quantity; }
            set
            {
                SetProperty(ref _quantity, value);
                // RaisePropertyChanged(nameof(Amount));
            }
        }

        private decimal _unitPrice;
        public decimal UnitPrice
        {
            get { return _unitPrice; }
            set
            {
                SetProperty(ref _unitPrice, value);
                //RaisePropertyChanged(nameof(Amount));
            }
        }

        public string Amount => Math.Round(Quantity * UnitPrice, 2, MidpointRounding.AwayFromZero).ToString("C");

        private async void SearchTextChanged(string _searchText)
        {
            try
            {
                //SelectedProduct = null;

                if (!_searchText.IsValidString() || _searchText.Length < 3 || SelectedProductSearch != null)
                {
                    return;
                }

                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                }

                _cancellationTokenSource = new CancellationTokenSource();


                ProductsSearchList?.Clear();


                await Task.Run(async () =>
                {
                    var _result = await _productService.SearchProducts(_searchText, _cancellationTokenSource.Token);

                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        if (_result != null && _result.IsSuccess && _result.Data != null)
                        {
                            ProductsSearchList = new ObservableCollection<ProductSearchModel>(_result.Data);
                            IsDropDownOpen = true;
                        }
                        else
                        {
                            _notificationService.ShowMessage(_result?.Error ?? "No Search Results found", NotificationType.Error);
                        }
                    }, System.Windows.Threading.DispatcherPriority.Normal, _cancellationTokenSource.Token);
                }, _cancellationTokenSource.Token);


            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning($"Task was cancelled in product search text {_searchText}");
            }
            catch (Exception _ex)
            {
                _logger.LogError($"Error in searching product by text {_searchText}", _ex);
            }
        }
    }
}

