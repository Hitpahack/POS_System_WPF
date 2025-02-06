using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Dashboard.Models;
using FalcaPOS.Dashboard.Services;
using FalcaPOS.Entites.Dashboard;
using FalcaPOS.Entites.Location;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.Dashboard.ViewModels
{
    public class CustomerByStoreViewModel : BindableBase
    {
        private readonly ICustomerByStoreService _customerByStoreService;

        private readonly Logger _logger;

        private readonly ICommonService _commonService;
        private readonly INotificationService _notificationService;

        public DelegateCommand DistrictSelectionChangeCommand { get; private set; }

        public CustomerByStoreViewModel(ICustomerByStoreService customerByStoreService,
            Logger logger,
            ICommonService commonService,
            INotificationService notificationService
            )
        {
            _customerByStoreService = customerByStoreService ?? throw new ArgumentNullException(nameof(customerByStoreService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            DistrictSelectionChangeCommand = new DelegateCommand(DistrictSelectionChanged);

            //GetCustomerChartData();

            GetAllDistrictsAsync();

        }

        private void DistrictSelectionChanged()
        {
            if (SelectedDistrict != null)
            {
                GetCustomerChartData(SelectedDistrict.DistrictId);
            }
            else
            {
                GetCustomerChartData();
            }
        }

        private District _selectedDisctrict;
        public District SelectedDistrict
        {
            get { return _selectedDisctrict; }
            set { SetProperty(ref _selectedDisctrict, value); }
        }

        private ObservableCollection<District> _districts;
        public ObservableCollection<District> Districts
        {
            get { return _districts; }
            set { SetProperty(ref _districts, value); }
        }

        private async void GetAllDistrictsAsync()
        {
            _logger.LogInformation("Admin getting all districts");

            await Task.Run(async () =>
            {
                var _result = await _commonService.GetAllDistricts();

                if (_result != null && _result.Any())
                {
                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        Districts = new ObservableCollection<District>(_result);

                        Districts?.Insert(0, GetDefaultDistrict());

                        SelectedDistrict = Districts[0];

                        _logger.LogInformation("Admin getting all districts success");
                    });
                }
            });
        }

        private District GetDefaultDistrict()
        {
            return new District
            {
                DistrictId = 0,
                Name = "All Districts"
            };
        }

        private async void GetCustomerChartData(int districtId = 0)
        {
            _logger.LogInformation($"Getting admin customer by store district id {districtId}");

            ChartData = null;

            TotalCustomers = 0;

            await Task.Run(async () =>
            {
                var _result = await _customerByStoreService.GetCustomerByStore(districtId);

                if (_result != null && _result.IsSuccess)
                {
                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        ChartData = MapToChartData(_result.Data);

                        TotalCustomers = _result.Data.TotalCustomersCount;

                        _logger.LogInformation($"Getting admin customer by store district id {districtId} --success");
                    });
                }
                else
                {
                    _notificationService.ShowMessage(_result.Error ?? "No record found", Common.NotificationType.Error);
                }

            });
        }


        private ChartDataModel MapToChartData(DashboardCustomer data)
        {

            if (data == null || !data.CustomerByStores.Any())
            {
                return default;
            }

            var _model = new ChartDataModel();

            _model.TotalCount = data.TotalCustomersCount;

            foreach (var _stores in data.CustomerByStores)
            {
                //_model.SeriesCollection.Add(new PieSeries
                //{
                //    Title = _stores.StoreName,
                //    Values = new ChartValues<double> { _stores.CustomerCount },
                //    LabelPoint = chartPoint => string.Format("{0}", chartPoint.Y),
                //    DataLabels = true,



                //}); ;
            }
            return _model;

        }

        private ChartDataModel _chartData;
        public ChartDataModel ChartData
        {
            get { return _chartData; }
            set { SetProperty(ref _chartData, value); }
        }

        private int _totalCustomers;
        public int TotalCustomers
        {
            get { return _totalCustomers; }
            set { SetProperty(ref _totalCustomers, value); }
        }
    }
}
