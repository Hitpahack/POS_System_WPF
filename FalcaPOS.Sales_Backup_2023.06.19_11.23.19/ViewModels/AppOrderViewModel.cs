using FalcaPOS.Common;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Sales;
using FalcaPOS.Sales.Views;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FalcaPOS.Sales.ViewModels
{
    public class AppOrderViewModel : BindableBase
    {
        private readonly ISalesService _salesService;


        private readonly INotificationService _notificationService;

        private readonly ProgressService _ProgressService;

        private readonly Logger _logger;

        private readonly IEventAggregator _eventAggregator;
        public DelegateCommand<AppOrderModel> ProcessCommand { get; private set; }

        public DelegateCommand<AppOrderModel> DeliveryCommand { get; private set; }

        public DelegateCommand<AppOrderModel> CancelCommand { get; private set; }

        public DelegateCommand RefreshAppOrderCommand { get; private set; }

        public DelegateCommand<object> YesCommand { get; private set; }
        public AppOrderViewModel(ISalesService salesService, INotificationService notificationService, Logger logger, IEventAggregator eventAggregator, ProgressService ProgressService)
        {
            _salesService = salesService ?? throw new ArgumentNullException(nameof(salesService));

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _ProgressService = ProgressService ?? throw new ArgumentNullException(nameof(ProgressService));

            LoadData();

            ProcessCommand = new DelegateCommand<AppOrderModel>(ProesstoSalesPage);

            CancelCommand = new DelegateCommand<AppOrderModel>(CancelAppOrder);

            RefreshAppOrderCommand = new DelegateCommand(LoadData);

            DeliveryCommand = new DelegateCommand<AppOrderModel>(DeliveryAppOrderModel);

            YesCommand = new DelegateCommand<object>(Confirmationpopup);

            _ = _eventAggregator.GetEvent<RefreshAppOrderPage>().Subscribe(LoadData);
        }


        public void Confirmationpopup(object obj)
        {
            try
            {
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

        public async void DeliveryAppOrderModel(AppOrderModel model)
        {
            try
            {
                if (_cancellationTokenSource != null)
                    _cancellationTokenSource.Cancel();

                _cancellationTokenSource = new CancellationTokenSource();

                _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                DeliveryConfirmationPopup confirm = new DeliveryConfirmationPopup();
                confirm.DataContext = this;
                AppOrderId = model.AppOrderId;
                await DialogHost.Show(confirm, "RootDialog", confirmClosingEventHandler);


            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private async void confirmClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {
                var viewmodel = (AppOrderViewModel)eventArgs.Parameter;
                if (viewmodel != null && AppOrderId != 0)
                {
                    await _ProgressService.StartProgressAsync();
                    await Task.Run(async () =>
                    {
                        var result = await _salesService.DeliveryAppOrderCustomer(AppOrderId);

                        if (result != null && result.IsSuccess && result.Data != null)
                        {
                            _notificationService.ShowMessage(result.Data, NotificationType.Success);

                            LoadData();
                        }

                    });
                    await _ProgressService.StopProgressAsync();
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public void ProesstoSalesPage(AppOrderModel model)
        {
            try
            {
                if (model.Products != null && model.Products.Count > 0 && model.Status.ToLower() == AppOrderStatus.processing.ToString())
                {
                    foreach (var item in model.Products)
                    {
                        if (item.OrderQty > item.StockQty)
                        {
                            _notificationService.ShowMessage(item.SKU + " App Order Qty should be less than or equal to POS available Qty", NotificationType.Error);
                            return;
                        }
                    }

                    _eventAggregator.GetEvent<AppOrderMoveToSalePageEvent>().Publish(model);

                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public async void CancelAppOrder(AppOrderModel cancelmodel)
        {
            try
            {
                if (_cancellationTokenSource != null)
                    _cancellationTokenSource.Cancel();

                _cancellationTokenSource = new CancellationTokenSource();

                _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                await Task.Run(async () =>
                {
                    var result = await _salesService.CancelAppOrderCustomer(5);

                    if (result != null && result.IsSuccess && result.Data != null)
                    {

                    }

                });
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private void ChangeStepStatus(string status)
        {
            try
            {
                AppOrderStatus StatusEnum;
                Enum.TryParse(status, out StatusEnum);

                switch (StatusEnum)
                {

                    case AppOrderStatus.processing:
                        _eventAggregator.GetEvent<StepSelectedIndexChangeAppOrderEvent>().Publish((int)AppOrderStatus.processing);

                        break;
                    case AppOrderStatus.intransit:
                        _eventAggregator.GetEvent<StepSelectedIndexChangeAppOrderEvent>().Publish((int)AppOrderStatus.intransit);

                        break;
                    case AppOrderStatus.delivery:
                        _eventAggregator.GetEvent<StepSelectedIndexChangeAppOrderEvent>().Publish((int)AppOrderStatus.delivery);

                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in indent  ChangeStepStatus", ex);
            }
        }

        private CancellationTokenSource _cancellationTokenSource;
        public async void LoadData()
        {
            try
            {
                if (_cancellationTokenSource != null)
                    _cancellationTokenSource.Cancel();

                _cancellationTokenSource = new CancellationTokenSource();

                _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                await Task.Run(async () =>
                {
                    var result = await _salesService.GetAppOrderList(_cancellationTokenSource.Token);

                    if (result != null && result.IsSuccess && result.Data != null)
                    {
                        List<AppOrderModel> appOrderList = new List<AppOrderModel>();
                        foreach (var item in result.Data)
                        {
                            appOrderList.Add(new AppOrderModel()
                            {
                                Name = item.Name,
                                Phone = item.Phone,
                                OrderDate = item.OrderDate,
                                Status = item.Status,
                                StoreID = item.StoreID,
                                AppOrderId = item.AppOrderId,
                                ShippingAddress = item.ShippingAddress,
                                Products = item.Products,
                                IsProcess = item.Status.ToLower() == AppOrderStatus.processing.ToString() ? true : false,
                            });
                        }

                        appOrderModels = new ObservableCollection<AppOrderModel>(appOrderList);

                    }

                });
            }
            catch (Exception _ex)
            {
                _logger.LogError("Getting error in loaddata method" + _ex.Message);
            }
        }

        private ObservableCollection<AppOrderModel> _appOrderModels;
        public ObservableCollection<AppOrderModel> appOrderModels
        {
            get { return _appOrderModels; }
            set { SetProperty(ref _appOrderModels, value); }
        }

        private int _apporderid;

        public int AppOrderId
        {
            get { return _apporderid; }
            set { SetProperty(ref _apporderid, value); }
        }



    }



}
