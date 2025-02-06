using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Manufacturers;
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
    public class ManufacturersListViewModel : BindableBase
    {
        private readonly IBrandService _brandService;
        private readonly Logger _logger;
        private readonly INotificationService _notificationService;
        private readonly ProgressService _ProgressService;
        private readonly IEventAggregator _eventAggregator;
        public DelegateCommand<Manufacturer> EnableDisableBrandCommand { get; private set; }
        public ManufacturersListViewModel(IBrandService brandService, Logger logger, INotificationService notificationService, ProgressService ProgressService, IEventAggregator eventAggregator)
        {
            _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _ProgressService = ProgressService ?? throw new ArgumentNullException(nameof(ProgressService));

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _eventAggregator.GetEvent<LoadManufacturesEvent>().Subscribe(RefreshBrands);

            EnableDisableBrandCommand = new DelegateCommand<Manufacturer>(EnableDisableBrand);

            LoadBrandsAsync();
        }

        private void RefreshBrands(object obj)
        {
            LoadBrandsAsync();
        }

        private async void EnableDisableBrand(Manufacturer manufacturer)
        {
            if (manufacturer != null)
            {
                var _result = await _ProgressService
                  .ConfirmAsync($"{manufacturer.Name}", $"Do you want to {(manufacturer.Isenabled ? @"disable" : @"enable")} Brand {manufacturer.Name}");

                if (_result == MessageDialogResult.Affirmative)
                {
                    var _response = await _brandService
                                          .EnableDisableBrand(manufacturer.ManufacturerId, !manufacturer.Isenabled);

                    if (_response != null && _response.IsSuccess)
                    {
                        _notificationService.ShowMessage($"Brand {manufacturer.Name} {(!manufacturer.Isenabled ? @"enabled" : @"disabled")} ", Common.NotificationType.Success);

                        LoadBrandsAsync();
                    }

                }
            }
        }

        private async void LoadBrandsAsync()
        {
            Brands?.Clear();

            await Task.Run(async () =>
            {
                var _result = await _brandService.GetAllBrands();

                if (_result != null && _result.Any())
                {
                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        Brands = new ObservableCollection<Manufacturer>(_result);
                    });
                }

            });


        }

        private ObservableCollection<Manufacturer> _brands;
        public ObservableCollection<Manufacturer> Brands
        {
            get { return _brands; }
            set { SetProperty(ref _brands, value); }
        }
    }
}
