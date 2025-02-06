using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.AddInventory;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.Invoice.ViewModels
{
    public class PurchaseReturnsViewModel : BindableBase
    {
        private readonly IPurchaseInvoiceService _purchaseInvoiceService;

        private CancellationTokenSource _cancellationTokenSource;


        private readonly Logger _logger;

        private readonly INotificationService _notificationService;

        private readonly IStoreService _storeService;

        private readonly ProgressService _ProgressService;
        public DelegateCommand PurhcaseReturnCommand { get; private set; }
        public DelegateCommand PurchaseResetReturnCommand { get; private set; }

        public DelegateCommand<object> RemovePurhcaseCommand { get; private set; }

        public DelegateCommand<object> PurchaseReturnSubmitCommand { get; private set; }

        public PurchaseReturnsViewModel(IPurchaseInvoiceService purchaseInvoiceService, Logger logger, IStoreService storeService, INotificationService notificationService, ProgressService ProgressService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _purchaseInvoiceService = purchaseInvoiceService ?? throw new ArgumentNullException(nameof(purchaseInvoiceService));

            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _ProgressService = ProgressService;

            PurhcaseReturnCommand = new DelegateCommand(LoadData);

            PurchaseResetReturnCommand = new DelegateCommand(ResetLoadData);

            RemovePurhcaseCommand = new DelegateCommand<object>(RemovePurhcaseProductCard);

            PurchaseReturnSubmitCommand = new DelegateCommand<object>(SubmitPurchaseReturn);



        }

        public async void SubmitPurchaseReturn(object obj)
        {
            try
            {
                var _view = (PurchaseReturnsViewModel)obj;


                if (_view?.PurhcaseReturnProducts.Count > 0)
                {
                    int i = 1;
                    foreach (var item in _view.PurhcaseReturnProducts)
                    {
                        if (item.ReturnQty == 0)
                        {
                            _notificationService.ShowMessage("Please enter return qty at  row " + i + " in  SKU " + item.ProductSKU, Common.NotificationType.Error);
                            return;
                        }

                        if (item.ReturnQty > item.ProductSubQty)
                        {
                            _notificationService.ShowMessage("Return qty should not above pos stock qty  row " + i + " in  SKU " + item.ProductSKU, Common.NotificationType.Error);
                            return;
                        }

                        if (!item.IsSelected)
                        {
                            _notificationService.ShowMessage("Please select row " + i + " in  SKU " + item.ProductSKU, Common.NotificationType.Error);
                            return;
                        }
                        i++;
                    }
                }

                if (_cancellationTokenSource != null)
                    _cancellationTokenSource?.Cancel();

                await _ProgressService.StartProgressAsync();

                _cancellationTokenSource = new CancellationTokenSource();

                await Task.Run(async () =>
                {
                    var _result = await _purchaseInvoiceService.UpdatePurchaseReturns(_view.PurhcaseReturnProducts?.ToList(), _cancellationTokenSource.Token);

                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        if (_result != null && _result.IsSuccess && _result.Data != null)
                        {
                            _notificationService.ShowMessage(_result.Data, Common.NotificationType.Success);

                            PurhcaseReturnProducts.Clear();
                        }
                        else
                        {
                            _notificationService.ShowMessage(_result?.Error ?? "An error occurred, try again", Common.NotificationType.Error);
                        }

                    });
                }, _cancellationTokenSource.Token);

                await _ProgressService.StopProgressAsync();

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in getting submit  purchase returns", _ex);
            }
            finally
            {
                await _ProgressService.StopProgressAsync();
            }
        }
        private void RemovePurhcaseProductCard(object obj)
        {
            try
            {
                var _viewModel = (StockProductViewModel)obj;

                if (_viewModel != null)
                {
                    PurhcaseReturnProducts.Remove(_viewModel);

                    RowCount = "Row Count " + PurhcaseReturnProducts.Count;

                    IsSubmitbtnEnable = PurhcaseReturnProducts.Count > 0 ? true : false;
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in getting purchase invoices", _ex);

            }
        }

        private void ResetLoadData()
        {
            try
            {
                if (PurhcaseReturnProducts != null && PurhcaseReturnProducts.Count > 0)
                {
                    PurhcaseReturnProducts.Clear();

                    RowCount = null;

                    ProductSKU = null;

                    LotNumber = null;

                    IsSubmitbtnEnable = false;
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in getting purchase invoices", _ex);
            }
        }
        private async void LoadData()
        {
            try
            {

                if (_cancellationTokenSource != null)
                    _cancellationTokenSource?.Cancel();

                if (string.IsNullOrEmpty(ProductSKU) && string.IsNullOrEmpty(LotNumber))
                {
                    _notificationService.ShowMessage("Please enter SKU or Lot Number", Common.NotificationType.Error);
                    return;
                }

                if (!string.IsNullOrEmpty(ProductSKU) && !string.IsNullOrEmpty(LotNumber))
                {
                    _notificationService.ShowMessage("Please Remove SKU or Lot Number", Common.NotificationType.Error);
                    return;
                }

                if (ProductSKU == "")
                    ProductSKU = null;
                if (LotNumber == "")
                    LotNumber = null;

                await _ProgressService.StartProgressAsync();

                _cancellationTokenSource = new CancellationTokenSource();

                await Task.Run(async () =>
                {
                    var _result = await _purchaseInvoiceService.GetPurchaseReturnsSearch(ProductSKU, LotNumber, _cancellationTokenSource.Token);

                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        if (_result != null && _result.IsSuccess && _result.Data != null)
                        {
                            foreach (var item in _result.Data)
                            {
                                if (PurhcaseReturnProducts != null && PurhcaseReturnProducts.Count > 0)
                                {
                                    var _alreadyadded = PurhcaseReturnProducts.Where(x => x.StockProductId == item.StockProductId).FirstOrDefault();

                                    if (_alreadyadded == null)
                                        PurhcaseReturnProducts.Add(item);
                                }
                                else
                                    PurhcaseReturnProducts = new ObservableCollection<StockProductViewModel>(_result.Data.ToList());
                            }

                            RowCount = "Row Count " + PurhcaseReturnProducts.Count;

                            IsSubmitbtnEnable = PurhcaseReturnProducts.Count > 0 ? true : false;
                        }
                        else
                        {
                            _notificationService.ShowMessage(_result?.Error ?? "An error occurred, try again", Common.NotificationType.Error);
                        }

                    });
                }, _cancellationTokenSource.Token);


            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in getting purchase invoices", _ex);
            }
            finally
            {
                await _ProgressService.StopProgressAsync();
            }
        }


        private ObservableCollection<StockProductViewModel> _purhcaseReturnProduct = new ObservableCollection<StockProductViewModel>();
        public ObservableCollection<StockProductViewModel> PurhcaseReturnProducts
        {
            get { return _purhcaseReturnProduct; }

            set { SetProperty(ref _purhcaseReturnProduct, value); }

        }

        private string _productSKU;
        public string ProductSKU
        {
            get { return _productSKU; }
            set { SetProperty(ref _productSKU, value); }
        }

        private string _lotNumber;
        public string LotNumber
        {
            get { return _lotNumber; }
            set { SetProperty(ref _lotNumber, value); }
        }

        private string _rowcount;
        public string RowCount
        {
            get { return _rowcount; }

            set { SetProperty(ref _rowcount, value); }
        }

        private bool _issubmitbtnenable;
        public bool IsSubmitbtnEnable
        {
            get { return _issubmitbtnenable; }

            set { SetProperty(ref _issubmitbtnenable, value); }
        }
    }


}
