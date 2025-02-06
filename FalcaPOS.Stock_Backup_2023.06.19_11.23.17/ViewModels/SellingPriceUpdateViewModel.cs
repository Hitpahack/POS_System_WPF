using FalcaPOS.Common;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Sales;
using FalcaPOS.Stock.Views;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FalcaPOS.Stock.ViewModels
{
    public class SellingPriceUpdateViewModel : BindableBase
    {
        private readonly ISalesService _salesService;
        public DelegateCommand<object> SellinPriceUpdateSearchCommand { get; private set; }
        public DelegateCommand<object> SellinPriceUpdateResetCommand { get; private set; }

        private readonly INotificationService _notificationService;
        private readonly ProgressService _progressService;
        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;
        private readonly Logger _logger;
        public DelegateCommand<object> SellingPriceUpdatePopup { get; private set; }

        public DelegateCommand<object> UpdateSellingPriceCommand { get; private set; }



        private readonly IStockService _stockService;

        public SellingPriceUpdateViewModel(IStockService StockService, ISalesService salesService, Logger logger, IEventAggregator eventAggregator, IDialogService dialogService, INotificationService notificationService, ProgressService progressService)
        {
            _salesService = salesService ?? throw new ArgumentNullException(nameof(salesService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            _progressService = progressService;

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            SellinPriceUpdateSearchCommand = new DelegateCommand<object>(SearchSellingPrice);

            SellinPriceUpdateResetCommand = new DelegateCommand<object>(ResetData);

            SellingPriceUpdatePopup = new DelegateCommand<object>(SellingPriceUpdate);

            UpdateSellingPriceCommand = new DelegateCommand<object>(UpdateSellingPrice);

            _stockService = StockService ?? throw new ArgumentNullException(nameof(StockService));


        }

        public void UpdateSellingPrice(object obj)
        {
            try
            {
                if (_selectedproducts != null)
                {
                    if (_selectedproducts.Category.CategoryName.ToLower() == "fertilizers")
                    {
                        if (NewSellingPrice > _selectedproducts.MRP)
                        {
                            _notificationService.ShowMessage("New selling price should be less than or equal to MRP", NotificationType.Error);
                            return;
                        }
                    }

                }

                if (NewSellingPrice == 0)
                {
                    _notificationService.ShowMessage("Please enter valid selling price", NotificationType.Error);
                    return;
                }
                if (StockProductId == 0)
                {
                    _notificationService.ShowMessage("Selected stock product should not be zero", NotificationType.Error);
                    return;
                }

                var TargetClose = ((Button)(obj));
                var dynamicCommand = TargetClose.Command;
                dynamicCommand.CanExecute(true);
                //pass Model as Arguments
                dynamicCommand.Execute(this);
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public async void SellingPriceUpdate(object obj)
        {
            try
            {
                var _viewUpdatePopup = ((SalesProduct)obj);
                if (_viewUpdatePopup != null)
                {
                    SelectedProducts = _viewUpdatePopup;
                    StockProductId = _viewUpdatePopup.StockProductId;
                    NewSellingPrice = 0;
                    UpdateSellingPricePopUp updateSellingPricePopUp = new UpdateSellingPricePopUp();
                    updateSellingPricePopUp.DataContext = this;
                    await DialogHost.Show(updateSellingPricePopUp, "RootDialog", ColsingEventHandler);
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private async void ColsingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {

                var _viewmodel = (SellingPriceUpdateViewModel)eventArgs.Parameter;
                if (_viewmodel != null)
                {
                    await _progressService.StartProgressAsync();

                    await Task.Run(async () =>
                    {
                        var _result = await _stockService.UpdateSellingPrice(StockProductId, NewSellingPrice);
                        if (_result != null && _result.IsSuccess)
                        {
                            _notificationService.ShowMessage(_result.Data, NotificationType.Success);
                            if (_selectedproducts != null)
                                _selectedproducts.ProductSellingPrice = (float)Math.Round(NewSellingPrice, 2, MidpointRounding.AwayFromZero);
                            return;
                        }
                        else
                        {
                            _notificationService.ShowMessage(_result.Error, NotificationType.Error);
                            return;
                        }
                    });

                    await _progressService.StopProgressAsync();

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            finally
            {
                await _progressService.StopProgressAsync();
            }
        }

        public void ResetData(object obj)
        {
            try
            {
                if (Products != null || Products.Count > 0)
                {
                    Products.Clear();
                    IsVisibility = false;
                    ProductCode = null;
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }
        public async void SearchSellingPrice(object obj)
        {
            try
            {
                var _viewModel = (SellingPriceUpdateViewModel)obj;
                if (string.IsNullOrEmpty(_viewModel.ProductCode))
                {
                    _notificationService.ShowMessage("please enter barcode", NotificationType.Error);

                    return;
                };

                if (Products.Any(x => x.BarCode.Trim().ToLower() == ProductCode.Trim().ToLower()))
                {
                    _notificationService.ShowMessage("Product is already added.", NotificationType.Error);

                    ProductCode = string.Empty;

                    return;
                }

                await _progressService.StartProgressAsync();

                SalesProduct _product = null;

                await Task.Run(async () =>
                {
                    _product = await _salesService.GetStockProduct(_viewModel.ProductCode);

                });

                if (_product != null)
                {

                    Products.Add(_product);
                    IsVisibility = true;
                }
                ProductCode = string.Empty;

                await _progressService.StopProgressAsync();
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            finally
            {
                await _progressService.StopProgressAsync();
            }
        }

        private string _productCode;
        public string ProductCode
        {
            get => _productCode;

            set => SetProperty(ref _productCode, value);
        }

        private ObservableCollection<SalesProduct> _products = new ObservableCollection<SalesProduct>();
        public ObservableCollection<SalesProduct> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        private SalesProduct _selectedproducts;
        public SalesProduct SelectedProducts
        {
            get => _selectedproducts;
            set => SetProperty(ref _selectedproducts, value);
        }


        private float _newsellingPrice;
        public float NewSellingPrice
        {
            get => _newsellingPrice;
            set => SetProperty(ref _newsellingPrice, value);
        }


        private int _stockProductId;
        public int StockProductId
        {
            get => _stockProductId;
            set => SetProperty(ref _stockProductId, value);
        }

        private bool isVisiblity;

        public bool IsVisibility
        {
            get => isVisiblity;
            set => SetProperty(ref isVisiblity, value);
        }


    }
}
