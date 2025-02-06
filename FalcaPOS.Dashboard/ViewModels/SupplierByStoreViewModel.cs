using FalcaPOS.Common.Helper;
using FalcaPOS.Contracts;
using FalcaPOS.Dashboard.Models;
using FalcaPOS.Dashboard.Services;
using FalcaPOS.Entites.Dashboard;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.Dashboard.ViewModels
{
    public class SupplierByStoreViewModel : BindableBase
    {
        private readonly IChartDataService _chartDataService;
        public DelegateCommand SelectionChangeCommand { get; private set; }
        private readonly INotificationService _notificationService;

        public SupplierByStoreViewModel(IChartDataService chartDataService, INotificationService notificationService)
        {
            _chartDataService = chartDataService ?? throw new ArgumentNullException(nameof(chartDataService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            SelectionChangeCommand = new DelegateCommand(SelectionChanged);
            LoadData();
            LoadMonth();
        }

        private ChartDataModel _chartData;
        public ChartDataModel ChartData
        {
            get { return _chartData; }
            set { SetProperty(ref _chartData, value); }
        }
        private List<string> _quarters;

        public List<string> Quarters
        {
            get { return _quarters; }
            set { SetProperty(ref _quarters, value); }
        }

        private String _selectedValue;
        public String SelectedValue
        {
            get { return _selectedValue; }
            set { SetProperty(ref _selectedValue, value); }
        }

        public async void LoadData(string SelectedValue = "All")
        {
            await Task.Run(async () =>
            {
                var _result = await _chartDataService.GetSupplierByStore(SelectedValue);

                if (_result != null && _result.IsSuccess)
                {
                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        //ChartData = MapToChartData(_result.Data);
                    });
                }
                else
                {
                    _notificationService.ShowMessage(_result.Error ?? "No record found", Common.NotificationType.Error);
                    ChartData = null;
                }

            });
        }

        private void SelectionChanged()
        {
            if (SelectedValue != null)
            {
                string SelectedItem = SelectedValue;
                if (SelectedValue != "Q1" && SelectedValue != "Q2" && SelectedValue != "Q3" && SelectedValue != "Q4" && SelectedValue != "All")
                {
                    int month = DateTime.ParseExact(SelectedValue, "MMMM", CultureInfo.CurrentCulture).Month;
                    SelectedItem = Convert.ToString(month);
                }

                LoadData(SelectedItem);
            }
            else
            {
                LoadData();
            }
        }

        public void LoadMonth()
        {
            List<String> Quarter = new List<String>();
            Quarter.Add("All");
            int FirstQuarter = 1;
            int SecondQuarter = 4;
            int ThirdQuarter = 7;
            int FourthQuarter = 10;
            var curretmonth = DateTime.Now.Month;
            var allmonth = StringHelper.AllMonth();
            foreach (var month in allmonth)
            {
                if (curretmonth >= month.Key)
                {
                    Quarter.Add(month.Value);
                }

            }
            if (curretmonth >= FirstQuarter)
            {
                Quarter.Add("Q1");
            }
            if (curretmonth >= SecondQuarter)
            {
                Quarter.Add("Q2");
            }
            if (curretmonth >= ThirdQuarter)
            {
                Quarter.Add("Q3");
            }
            if (curretmonth >= FourthQuarter)
            {
                Quarter.Add("Q4");
            }

            if (Quarter.Count > 0)
            {
                Quarters = Quarter;
            }

            SelectedValue = Quarter[0];
        }

        //private ChartDataModel MapToChartData(DashbordSupplierVM data)
        //{

        //    if (data == null || !data.SupplierByStores.Any())
        //    {
        //        return default;
        //    }

        //    var _model = new ChartDataModel();

        //    List<String> Labels = new List<string>();
        //    StoresNames supplierByStore = new StoresNames();
        //    var stores = data.SupplierByStores.GroupBy(x => x.StoreName).Select(x=> new StoresNames() {supplierByStores=x.ToList(),StoreName=x.Key }).ToList();

        //    for (var i = 1; i <= data.SupplierByStores.ToList().Count; i++)
        //    {

        //        ChartValues<double> total = new ChartValues<double>();
        //        List<string> supliers = new List<string>();

        //        foreach (var item in stores)
        //        {
        //            var supplieritem = item.supplierByStores.FirstOrDefault();
        //            if (supplieritem != null)
        //            {
        //                total.Add(Math.Round( supplieritem.TotalAmount,2));
        //                supliers.Add(supplieritem.SupplierName);
        //                stores = stores.Select(x => new StoresNames() { supplierByStores = x.supplierByStores.Where(y => y.SupplierName != supplieritem.SupplierName).ToList(), StoreName = x.StoreName }).ToList();
        //            }

        //        }
        //        _model.SeriesCollection.Add(new StackedColumnSeries
        //        {
        //          Title=supliers.FirstOrDefault(),
        //            Values =total,
        //            StackMode = StackMode.Values,
        //             LabelPoint = chartPoint => string.Format("{0}", chartPoint.Y),
        //             DataLabels = true,
        //        });
        //    }

        //    _model.Labels = stores.Select(x=>x.StoreName).ToArray();
        //    return _model;

        //}

        public class StoresNames
        {
            public string StoreName { get; set; }
            public List<SupplierByStore> supplierByStores { get; set; }
        }
    }
}
