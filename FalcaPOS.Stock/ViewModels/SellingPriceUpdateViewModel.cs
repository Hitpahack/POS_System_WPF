using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Products;
using FalcaPOS.Entites.Stock;
using FalcaPOS.Entites.Stores;
using FalcaPOS.Entites.Zone;
using FalcaPOS.Stock.Views;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls.AutoSuggestBox;
using Telerik.Windows.Shapes;

namespace FalcaPOS.Stock.ViewModels
{
    public class SellingPriceUpdateViewModel : BindableBase
    {
        private readonly ISalesService _salesService;
        private readonly IBarCodeService _barCodeService;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly INotificationService _notificationService;
        private readonly ProgressService _progressService;
        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;
        private readonly Logger _logger;
        private readonly IProductService _productService;

        public DelegateCommand<object> UpdateSellingPriceCommand { get; private set; }
        public DelegateCommand<object> SellingPriceUpdateResetCommand { get; private set; }
        public DelegateCommand<object> SellingPriceZoneListChangeCommand { get; private set; }
        public DelegateCommand<object> SellingPriceTerritoryChangeCommand { get; private set; }
        public DelegateCommand<object> SellingPriceStoreChangeCommand { get; private set; }
        
        public DelegateCommand<object> OkCommand { get; private set; }

        // This Command handles the TextChangedCommand of Telerik Auto Suggest Box.
        public DelegateCommand<object> TextChangedCommand { get; set; }

        // This command handles the QuerySubmittedCommand of Telerik Auto Suggest Box.
        public DelegateCommand<object> QuerySubmittedCommand { get; set; }



        private readonly IStockService _stockService;
        private readonly IZoneService _zoneService;

        public SellingPriceUpdateViewModel(IStockService StockService, ISalesService salesService, Logger logger, IEventAggregator eventAggregator, IDialogService dialogService, INotificationService notificationService, ProgressService progressService, IProductService productService, IZoneService zoneService, IBarCodeService barCodeService)
        {
            _salesService = salesService ?? throw new ArgumentNullException(nameof(salesService));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _zoneService = zoneService ?? throw new ArgumentNullException(nameof(zoneService));
            _barCodeService = barCodeService ?? throw new ArgumentNullException(nameof(barCodeService));

            _progressService = progressService;

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            OkCommand = new DelegateCommand<object>(Submit);

            TextChangedCommand = new DelegateCommand<object>(OnTextChangedCommandExecuted);
            QuerySubmittedCommand = new DelegateCommand<object>(OnQuerySubmittedCommandExecuted);

            SellingPriceUpdateResetCommand = new DelegateCommand<object>(ResetData);


            UpdateSellingPriceCommand = new DelegateCommand<object>(UpdateSellingPrice);

            _stockService = StockService ?? throw new ArgumentNullException(nameof(StockService));
            SellingPriceZoneListChangeCommand = new DelegateCommand<object>(LoadTerritoryevent);
            SellingPriceTerritoryChangeCommand = new DelegateCommand<object>(LoadTerritoryStoreevent);
            _eventAggregator.GetEvent<LoadBarcodesEvent>().Subscribe(() => CheckAndLoadBarcode());
            _cancellationTokenSource = new CancellationTokenSource();
            LoadZoneAsync();
        }

        /// <summary>
        /// This method is called when the OnQuerySubmittedCommand is executed on the AutoSuggestBox.
        /// </summary>
        /// <param name="obj">Object.</param>
        private async void OnQuerySubmittedCommandExecuted(object obj)
        {
            try
            {
                // Gets the Event Args from the object.
                var querySumbittedArgs = (QuerySubmittedEventArgs)obj;

                // If the suggestion data is null, clears the data.
                if (querySumbittedArgs.Suggestion == null)
                {
                    SelectedProduct = null;
                    ProductsSearchList?.Clear();
                    return;
                }

                // Fetches the ProductSearchModel details from Suggestion.
                SelectedProductSearch = (ProductSearchModel) querySumbittedArgs.Suggestion;
                var _storeID = 2;

                await Task.Run(async () =>
                {
                    // Gets the Brand Category and SubcategorySKU based on product ID
                    var _result = await _productService.GetBrandCategorySubcategorySKUbyId(SelectedProductSearch.ProductId);

                    // If the result and data is not null and the result is success
                    if (_result != null && _result.IsSuccess && _result.Data != null)
                    {
                        // Stock count is fetched.
                        var _productStockCount = await _productService.GetStockbySKU(_result.Data.ProductSKU, _storeID);

                        Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            // Selected Product is set
                            SelectedProduct = _result.Data;
                        });
                    }
                    else
                    {
                        // Error is shown to the user.
                        _notificationService.ShowMessage(_result.Error, Common.NotificationType.Error);
                        return;
                    }
                });
            }
            catch (Exception _ex)
            {
                // Catches and logs the exception.
                _logger.LogError("Error in getting product information", _ex);
            }
        }

        /// <summary>
        /// This method is executed when the text is changed in the Auto Suggest Box.
        /// </summary>
        /// <param name="obj">object.</param>
        private async void OnTextChangedCommandExecuted(object obj)
        {
            // Converts the object to text changed args
            var textChangedArgs = (Telerik.Windows.Controls.AutoSuggestBox.TextChangedEventArgs)obj;
            if (textChangedArgs.Reason == TextChangeReason.UserInput)
            {
                // If text is not valid and text length is less than 3, clears the search and returns.
                if (!Text.IsValidString() || Text.Length < 3)
                {
                    SelectedProduct = null;
                    ProductsSearchList?.Clear();
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
                    // Fetches the product for the given text.
                    var _result = await _productService.SearchProducts(Text, _cancellationTokenSource.Token);

                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        if (_result != null && _result.IsSuccess && _result.Data != null)
                        {
                            // Updates the Product Search List and sets DropDownOpen to True.
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
        }

        //Checks if product and store is selected in the selling price update page.
        private async void CheckAndLoadBarcode()
        {
            try
            {
                // Checks if the product and store is selected and shows and fetches the barcode.
                if (SelectedProductSearch != null & SelectedStore != null)
                {

                    IsAllFiltersSelected = true;
                    BarcodeList?.Clear();
                    BarcodeList = null;
                    SelectedBarcode = null;
                    var _result = await _stockService.GetProductStoreBarCode(SelectedProductSearch.ProductId, SelectedStore.StoreId, _cancellationTokenSource.Token);
                        
                    if (_result != null && _result.Count() > 0)
                    {
                        BarcodeList = new List<string>(_result.Select(x => x.Barcode));
                    }
                    else
                    {
                        _notificationService.ShowMessage("Barcode not found", NotificationType.Error);
                    }
                }
                else
                {
                    IsAllFiltersSelected = false;
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private async void LoadTerritoryStoreevent(object obj)
        {
            try
            {
                SelectedStore = null;
                if (SelectedTerritory != null)
                {
                    var _result = await _zoneService.GetTerritoryStores(SelectedTerritory.Id, _cancellationTokenSource.Token);
                    if (_result != null && _result.Count() > 0)
                    {
                        StoreList = new ObservableCollection<Store>(_result.ToList());

                    }
                }
                else
                {
                    StoreList.Clear();
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private async void LoadTerritoryevent(object obj)
        {
            try
            {
                SelectedTerritory = null;
                if (SelectedZone != null)
                {
                    var _result = await _zoneService.GetTerritories(SelectedZone.Id, _cancellationTokenSource.Token);
                    if (_result != null && _result.Count() > 0)
                    {
                        Territories = new ObservableCollection<Territory>(_result.ToList());

                    }
                }
                else
                {
                    Territories.Clear();
                }
                }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            
        }

        private async void LoadZoneAsync()
        {
            try
            {
                var _result = await _zoneService.GetZone();
                if (_result != null && _result.IsSuccess)
                {
                    ZoneList = new ObservableCollection<NewZone>(_result.Data);

                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }


        public void Submit(object obj)
        {
            try
            {

                var TargetClose =(Button)(obj);
                var dynamicCommand = TargetClose.Command;
                dynamicCommand.CanExecute(true);
                //pass Model as Arguments
                dynamicCommand.Execute(this);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _notificationService.ShowMessage(AppConstants.CommonError, Common.NotificationType.Error);


            }
        }

        private async void NormalProductClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {
                var _viewModel = (SellingPriceUpdateViewModel)eventArgs.Parameter;
                if (_viewModel != null)
                {

                    //await _progressService.StartProgressAsync();
                    SellingPriceUpdateViewModelV2 _Dto = new SellingPriceUpdateViewModelV2() {
                        SKUId = SelectedProduct?.ProductId,
                        ZoneId = SelectedZone?.Id,
                        TerritoryId = SelectedTerritory?.Id,
                        StoreId = SelectedStore?.StoreId,
                        Barcode = SelectedBarcode,
                        NewSellingPrice = NewSellingPrice,
                        Category = SelectedProduct.Category.CategoryName

                    };

                    var _result = await _stockService.UpdateSellingPriceV2(_Dto, _cancellationTokenSource.Token);
                    if (_result != null && _result.IsSuccess)
                    {
                        var temp = _result;
                        var RowsList = _result.Data.Where(x =>!string.IsNullOrEmpty( x.ErrorMsg)).ToList();
                        var RowsListSucess = _result.Data.Where(x => x.ErrorMsg == "").ToList();
                        var RowListFertilizerMrp = _result.Data.Where(x =>!string.IsNullOrEmpty(x.MrpErrorMsg)).ToList();
                        var RowListFertilizer = _result.Data.Where(x => string.IsNullOrEmpty(x.MrpErrorMsg)).ToList();
                        if (RowsList.Count > 0)
                        {

                            _notificationService.ShowMessage($"{RowsList.Count} lots having negative margin", NotificationType.Error);
                            return;

                        }
                        else if (RowListFertilizerMrp.Count > 0)
                        {
                            _notificationService.ShowMessage($"selling price should be less than or equal to MRP", NotificationType.Error);
                            return;
                        }
                        else if (RowListFertilizer.Count > 0)
                        {
                           _notificationService.ShowMessage($"Selling price updated for {RowListFertilizer.Count} record", NotificationType.Success);
                            ResetData(null);
                            return;
                        }
                        else if (RowsListSucess.Count > 0)
                        {
                            _notificationService.ShowMessage($"Selling price updated for {RowsListSucess.Count} record", NotificationType.Success);
                            ResetData(null);
                            return;
                        }



                    }
                    else
                    {
                        _notificationService.ShowMessage(_result.Error, NotificationType.Error);
                        return;
                    }

                    //await Task.Run(async () =>
                    //{
                    //    var result = await _skuService.CreateSKURequest(ProductDetails);

                    //    Application.Current?.Dispatcher?.Invoke(() =>
                    //    {

                    //        if (result.IsSuccess && result.Data != null)
                    //        {

                    //            _notificationService.ShowMessage(result.Data, NotificationType.Success);
                    //            BrandCards.Clear();
                    //            GetLastSKU(_viewModel.SelectedSubCategory.Id);

                    //        }
                    //        else
                    //        {
                    //            _notificationService.ShowMessage(result.Error, NotificationType.Error);
                    //        }


                    //    });

                    //});

                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);
            }
        }

        public async void UpdateSellingPrice(object obj)
        {
            if (SelectedProductSearch == null)
            {
                _notificationService.ShowMessage("Please Enter SKU", NotificationType.Error);
                return;
            }
            else
            {
                if (SelectedZone == null && SelectedTerritory == null && SelectedStore == null)
                {
                    _notificationService.ShowMessage("Please select anyone filters", NotificationType.Error);
                    return;
                }
                else
                {
                    if (NewSellingPrice == 0)
                    {
                        _notificationService.ShowMessage("Please enter valid selling price", NotificationType.Error);
                        return;
                    }

                    //SellingPriceUpdatePopUp confirmationPopup = new SellingPriceUpdatePopUp();
                    //confirmationPopup.DataContext = this;
                    //await DialogHost.Show(confirmationPopup, "RootDialog", NormalProductClosingEventHandler);

                    SellingPriceUpdateViewModelV2 _Dto = new SellingPriceUpdateViewModelV2()
                    {
                        SKUId = SelectedProduct?.ProductId,
                        ZoneId = SelectedZone?.Id,
                        TerritoryId = SelectedTerritory?.Id,
                        StoreId = SelectedStore?.StoreId,
                        Barcode = SelectedBarcode,
                        NewSellingPrice = NewSellingPrice,
                        Category = SelectedProduct.Category.CategoryName

                    };

                    var _result = await _stockService.UpdateSellingPriceV2(_Dto, _cancellationTokenSource.Token);
                    if (_result != null && _result.IsSuccess)
                    {
                        var temp = _result;
                        var RowsList = _result.Data.Where(x => !string.IsNullOrEmpty(x.ErrorMsg)).ToList();
                        var RowsListSucess = _result.Data.Where(x => x.ErrorMsg == "").ToList();
                       // var RowListFertilizerMrp = _result.Data.Where(x => !string.IsNullOrEmpty(x.MrpErrorMsg)).ToList();
                        //var RowListFertilizer = _result.Data.Where(x => string.IsNullOrEmpty(x.MrpErrorMsg)).ToList();
                       
                        if (RowsList.Count > 0)
                        {
                            HeaderText = $"These are the {RowsList.Count} lots having negative margin";
                            NegativeMargins = new ObservableCollection<SellingPriceResponse>(RowsList);
                            SellingPriceUpdateNegativeMarginPopup negativeMarginPopup = new SellingPriceUpdateNegativeMarginPopup();
                            negativeMarginPopup.DataContext = this;
                            await DialogHost.Show(negativeMarginPopup, "RootDialog", NormalProductClosingEventHandler);

                        }
                        else
                        {
                            _notificationService.ShowMessage($"Selling price updated for {RowsListSucess.Count} record", NotificationType.Success);
                            ResetData(null);
                            return;
                        }


                    }
                    else
                    {
                        _notificationService.ShowMessage(_result.Error, NotificationType.Error);
                        return;
                    }
                }
            } 
        }


        public void ResetData(object obj)
        {
            try
            {
                SelectedZone = null;
                SelectedTerritory = null;
                SelectedStore = null;
                StoreList = null;
                Territories = null;
                SelectedProduct = null;
                SelectedProductSearch = null;
                SelectedBarcode = null;
                BarcodeList = null;
                IsAllFiltersSelected = false;
                NewSellingPrice = 0;
                Text = null;
                

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }


        private bool _isDropDownOpen;
        public bool IsDropDownOpen
        {
            get { return _isDropDownOpen; }
            set { SetProperty(ref _isDropDownOpen, value); }
        }


        private float _newsellingPrice;
        public float NewSellingPrice
        {
            get => _newsellingPrice;
            set => SetProperty(ref _newsellingPrice, value);
        }


        private ProductSearchModel _selectedProductSearch;
        public ProductSearchModel SelectedProductSearch
        {
            get { return _selectedProductSearch; }
            set { SetProperty(ref _selectedProductSearch, value);
                _eventAggregator.GetEvent<LoadBarcodesEvent>().Publish();
            }
        }

        private ObservableCollection<ProductSearchModel> _productsSearchList;
        public ObservableCollection<ProductSearchModel> ProductsSearchList
        {
            get { return _productsSearchList; }
            set { SetProperty(ref _productsSearchList, value); }
        }

        private InventaryProductViewModel _selectedProduct;
        public InventaryProductViewModel SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);

        }
        private ObservableCollection<NewZone> _zones;

        public ObservableCollection<NewZone> ZoneList
        {
            get { return _zones; }
            set { _ = SetProperty(ref _zones, value); }
        }

        private NewZone _selecctedZone;

        public NewZone SelectedZone
        {
            get { return _selecctedZone; }
            set { SetProperty(ref _selecctedZone, value); }
            }


        private ObservableCollection<Territory> _territories;

        public ObservableCollection<Territory> Territories
        {
            get { return _territories; }
            set { _ = SetProperty(ref _territories, value); }
        }

        private Territory _selectedTerritory;

        public Territory SelectedTerritory
        {
            get { return _selectedTerritory; }
            set { SetProperty(ref _selectedTerritory, value); }
        }

        private List<string> _barcodeList;

        public List<string> BarcodeList
        {
            get { return _barcodeList; }
            set { _ = SetProperty(ref _barcodeList, value); }
        }


        private ObservableCollection<Store> _storeList;

        public ObservableCollection<Store> StoreList
        {
            get { return _storeList; }
            set { _ = SetProperty(ref _storeList, value); }
        }

        private Store _selectedStore;

        public Store SelectedStore
        {
            get { return _selectedStore; }
            set
            {
                SetProperty(ref _selectedStore, value);
                _eventAggregator.GetEvent<LoadBarcodesEvent>().Publish();
            }
        }

        private string _selectedBarcode;

        public string SelectedBarcode
        {
            get { return _selectedBarcode; }
            set { SetProperty(ref _selectedBarcode, value); }
        }


        private string _lotnumber;
        public string Lotnumber
        {
            get { return _lotnumber; }
            set { SetProperty(ref _lotnumber , value); }
        }

        private bool _isAllFiltersSelected;
        public bool IsAllFiltersSelected
        {
            get { return _isAllFiltersSelected; }
            set { SetProperty(ref _isAllFiltersSelected, value); }
        }

        private string _SKUOrProduct;

        public string SKUOrProduct {
            get { return _SKUOrProduct; }
            set { SetProperty(ref _SKUOrProduct,value); }
        }

        private string _text;

        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }

        private ObservableCollection<SellingPriceResponse> _NegativeMargins;

        public ObservableCollection<SellingPriceResponse> NegativeMargins
        { 
            get { return _NegativeMargins; }
            set { SetProperty(ref _NegativeMargins, value); }
        }

        private string _headerText;

        public string HeaderText
        {
            get { return _headerText; }
            set { SetProperty(ref _headerText , value); }
        }

    }
}
