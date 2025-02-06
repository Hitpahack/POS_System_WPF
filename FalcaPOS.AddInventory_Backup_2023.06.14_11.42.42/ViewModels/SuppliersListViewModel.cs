using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Suppliers;
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
    public class SuppliersListViewModel : BindableBase
    {
        private readonly ISupplierService _supplierService;

        private readonly INotificationService _notificationService;

        private readonly ProgressService _progressService;

        private readonly IEventAggregator _eventAggregator;

        private readonly Logger _logger;
        public DelegateCommand<SuppliersViewModel> EnableDisableSupplierCommand { get; private set; }
        public DelegateCommand<SuppliersViewModel> EditSupplierCommand { get; private set; }

        public SuppliersListViewModel(ISupplierService supplierService,
            INotificationService notificationService,
            ProgressService progressService,
            IEventAggregator eventAggregator, Logger logger)
        {

            _supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            EnableDisableSupplierCommand = new DelegateCommand<SuppliersViewModel>(EnableDisableSupplier);

            EditSupplierCommand = new DelegateCommand<SuppliersViewModel>(EditSupplier);

            _eventAggregator.GetEvent<ReloadSupplierEvent>().Subscribe(LoadSuppliers);

            _eventAggregator.GetEvent<SignalRSupplierAddedEvent>().Subscribe(NewSupplierAdded, ThreadOption.PublisherThread);

            _eventAggregator.GetEvent<SignalRSupplierEnableDisableEvent>().Subscribe(SupplierEnableDisable, ThreadOption.PublisherThread);

            LoadSuppliers();

        }



        private void SupplierEnableDisable(object obj)
        {
            if (obj is SuppliersViewModel _splr)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {


                        if (Suppliers == null)
                        {
                            Suppliers = new ObservableCollection<SuppliersViewModel>();
                        }

                        //remove from list of suppliers.
                        var _exSupplier = Suppliers.FirstOrDefault(x => x.SupplierId == _splr.SupplierId);

                        if (_exSupplier != null)
                        {
                            Suppliers.Remove(_exSupplier);
                        }


                        Suppliers.Add(_splr);

                        Suppliers = new ObservableCollection<SuppliersViewModel>(Suppliers.OrderBy(x => x.Name));


                    });

                }
                catch (Exception _ex)
                {
                    _logger?.LogError("Error in supplier enable disable", _ex);
                }
            }
        }

        private void NewSupplierAdded(object obj)
        {
            if (obj is SuppliersViewModel _suplr)
            {
                try
                {
                    Application.Current.Dispatcher?.Invoke(() =>
                    {

                        if (Suppliers == null)
                        {
                            Suppliers = new ObservableCollection<SuppliersViewModel> { _suplr };

                            return;
                        }

                        if (Suppliers.Any(x => x.SupplierId == _suplr.SupplierId))
                        {
                            var _exstSupplier = Suppliers.FirstOrDefault(x => x.SupplierId == _suplr.SupplierId);

                            if (_exstSupplier != null)
                            {
                                Suppliers.Remove(_exstSupplier);
                            }
                        }

                        Suppliers.Add(_suplr);

                        Suppliers = new ObservableCollection<SuppliersViewModel>(Suppliers.OrderByDescending(x => x.Createddate));


                    });
                }
                catch (Exception _ex)
                {
                    _logger.LogError("Error in adding supplier ", _ex);
                }
            }

        }

        private void EditSupplier(SuppliersViewModel supplier)
            => _eventAggregator.GetEvent<EditSupplerEvent>().Publish(supplier);


        private async void EnableDisableSupplier(SuppliersViewModel supplier)
        {
            if (supplier != null)
            {

                var _result = await _progressService
                    .ConfirmAsync($"{supplier.Name}", $"Do you want to {(supplier.Isenabled ? @"disable" : @"enable")} supplier {supplier.Name}");

                if (_result == MessageDialogResult.Affirmative)
                {
                    var _response = await _supplierService.EnableDisableSupplier((int)supplier.SupplierId, !supplier.Isenabled);

                    if (_response != null && _response.IsSuccess)
                    {
                        _notificationService.ShowMessage($"Suplier {supplier.Name} {(!supplier.Isenabled ? @"Enabled" : @"disabled")} ", Common.NotificationType.Success);

                        LoadSuppliers();
                    }

                }

            }

        }

        private ObservableCollection<SuppliersViewModel> _suppliers;

        public ObservableCollection<SuppliersViewModel> Suppliers
        {
            get => _suppliers;
            set => SetProperty(ref _suppliers, value);
        }

        private async void LoadSuppliers()
        {
            await Application.Current.Dispatcher?.Invoke(async () =>
            {

                Suppliers?.Clear();

                await Task.Run(async () =>
                {
                    var _result = await _supplierService.GetSuppliers();

                    if (_result != null && _result.Any())
                    {
                        Application.Current.Dispatcher?.Invoke(() =>
                        {
                            Suppliers = new ObservableCollection<SuppliersViewModel>(_result);


                        });
                    }
                });
            });


        }
        //private async void LoadAddedSuppliers()
        //{
        //    Suppliers?.Clear();

        //    await Task.Run(async () =>
        //    {
        //        var _result = await _supplierService.GetSuppliers();

        //        if (_result != null && _result.Any())
        //        {
        //            Application.Current.Dispatcher?.Invoke(() =>
        //            {
        //                Suppliers = new ObservableCollection<SuppliersViewModel>(_result.OrderByDescending(x => x.Createddate));


        //            });
        //        }
        //    });

        //}


    }
}
