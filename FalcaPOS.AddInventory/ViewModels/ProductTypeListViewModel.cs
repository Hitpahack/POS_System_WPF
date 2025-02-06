using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.ProductTypes;
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
    public class ProductTypeListViewModel : BindableBase
    {
        private readonly IProductTypeService _productTypeService;
        private readonly Logger _logger;
        private readonly INotificationService _notificationService;
        private readonly ProgressService _ProgressService;
        private readonly IEventAggregator _eventAggregator;

        public DelegateCommand<ProductType> EnableDisableProductTypeCommand { get; private set; }
        public ProductTypeListViewModel(IProductTypeService productTypeService,
            Logger logger,
            INotificationService notificationService,
            ProgressService ProgressService,
            IEventAggregator eventAggregator
            )
        {
            _productTypeService = productTypeService ?? throw new ArgumentNullException(nameof(productTypeService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _ProgressService = ProgressService ?? throw new ArgumentNullException(nameof(ProgressService));

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            EnableDisableProductTypeCommand = new DelegateCommand<ProductType>(EnableDisbaleProductType);

            _eventAggregator.GetEvent<AddProductTypeEvent>().Subscribe(AddNewProductType, ThreadOption.PublisherThread);

            _eventAggregator.GetEvent<SignalRProductTypeEnableDisableEvent>().Subscribe(ProductTypeEnableDisbale, ThreadOption.PublisherThread);

            LoadProductTypes();
        }

        private void ProductTypeEnableDisbale(object obj)
        {
            try
            {

                if (obj is ProductType _pType)
                {
                    Application.Current?.Dispatcher?.Invoke(() =>
                    {

                        var _existingType = ProductTypes?.FirstOrDefault(x => x.ProductTypeId == _pType.ProductTypeId);

                        if (_existingType != null)
                        {
                            ProductTypes?.Remove(_existingType);
                        }
                        ProductTypes?.Add(_pType);

                        if (ProductTypes != null && ProductTypes.Count > 0)
                        {
                            ProductTypes = new ObservableCollection<ProductType>(ProductTypes?.OrderBy(x => x.Name));
                        }

                    });

                }
            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in product type enable disable", _ex);
            }
        }

        private void AddNewProductType(object type)
        {
            var _productType = type as ProductType;
            if (_productType != null)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {

                        if (ProductTypes == null)
                        {
                            ProductTypes = new ObservableCollection<ProductType> { _productType };

                            return;
                        }

                        ProductTypes.Add(_productType);

                        ProductTypes = new ObservableCollection<ProductType>(ProductTypes.OrderBy(x => x.Name));

                    });
                }
                catch (Exception _ex)
                {
                    _logger?.LogError("Error in  product type list", _ex);
                }

            }
        }

        private async void EnableDisbaleProductType(ProductType productType)
        {
            if (productType != null)
            {
                var _result = await _ProgressService
                  .ConfirmAsync($"{productType.Name}", $"Do you want to {(productType.Isenabled ? @"disable" : @"enable")} Departname {productType.Name}");

                if (_result == MessageDialogResult.Affirmative)
                {
                    var _response = await _productTypeService
                                          .EnableDisbaleProductType(productType.ProductTypeId.Value, !productType.Isenabled);

                    if (_response != null && _response.IsSuccess)
                    {
                        _notificationService.ShowMessage($"Sub category  {productType.Name} {(!productType.Isenabled ? @"Enabled" : @"disabled")} ", Common.NotificationType.Success);

                        LoadProductTypes();
                    }

                }
            }

        }

        private async void LoadProductTypes()
        {
            ProductTypes?.Clear();

            try
            {

                await Task.Run(async () =>
                {
                    var _result = await _productTypeService.GetProductTypes();

                    if (_result != null)
                    {
                        await Application.Current?.Dispatcher?.InvokeAsync(() =>
                        {
                            ProductTypes = new ObservableCollection<ProductType>(_result.OrderBy(x => x.Name));

                            //ProductTypes= new ObservableCollection<ProductType>(ProductTypes?.OrderBy(x => x.Name));

                        });
                    }
                });

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in product type list", _ex);
            }

        }

        private ObservableCollection<ProductType> _productTypes = new ObservableCollection<ProductType>();


        public ObservableCollection<ProductType> ProductTypes
        {
            get { return _productTypes; }
            set { SetProperty(ref _productTypes, value); }
        }
    }
}
