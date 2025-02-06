using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Products;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.AddInventory.ViewModels
{
    public class ProductsListViewModel : BindableBase
    {
        private readonly IProductService _productService;
        private readonly Logger _logger;
        private readonly INotificationService _notificationService;
        private readonly ProgressService _ProgressService;
        private readonly IEventAggregator _eventAggregator;

        public DelegateCommand<ProductViewModel> EnableDisableProductCommand { get; private set; }

        public ProductsListViewModel(IProductService productService,
            Logger logger,
            INotificationService notificationService,
            ProgressService ProgressService,
            IEventAggregator eventAggregator)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _ProgressService = ProgressService ?? throw new ArgumentNullException(nameof(ProgressService));

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _eventAggregator.GetEvent<RefreshProductEvent>().Subscribe(LoadProductsAsync);

            EnableDisableProductCommand = new DelegateCommand<ProductViewModel>(EnableDisableProduct);

            LoadProductsAsync();
        }

        private async void EnableDisableProduct(ProductViewModel product)
        {
            if (product != null)
            {
                var _result = await _ProgressService
                    .ConfirmAsync($"{product.Name}", $"Do you want to {(product.Isenabled ? @"disable" : @"enable")} product {product.Name}");

                if (_result == MessageDialogResult.Affirmative)
                {
                    var _response = await _productService.EnabelDiableProduct((int)product.ProductId, !product.Isenabled);

                    if (_response != null && _response.IsSuccess)
                    {
                        _notificationService.ShowMessage($"Product {product.Name} {(!product.Isenabled ? @"Enabled" : @"disabled")} ", Common.NotificationType.Success);

                        LoadProductsAsync();
                    }

                }
            }
        }

        private async void LoadProductsAsync()
        {
            try
            {
                await Task.Run(async () =>
                    {
                        var _result = await _productService.GetProducts();

                        if (_result != null && _result.Any())
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                Products = new ObservableCollection<ProductViewModel>(_result.OrderBy(x => x.Name));
                            });
                        }
                    });
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in products loading", _ex);

            }
        }

        private ObservableCollection<ProductViewModel> _products;
        public ObservableCollection<ProductViewModel> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

    }
}
