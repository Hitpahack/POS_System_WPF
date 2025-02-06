using FalcaPOS.Common.Helper;
using FalcaPOS.Contracts;
using FalcaPOS.Dashboard.Models;
using FalcaPOS.Dashboard.Services;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.Dashboard.ViewModels
{
    public class SalesByRegionViewModel : BindableBase
    {
        private readonly IChartDataService _chartDataService;
        public DelegateCommand SelectionChangeCommand { get; private set; }
        private readonly INotificationService _notificationService;


        public SalesByRegionViewModel(IChartDataService chartDataService, INotificationService notificationService)
        {
            _chartDataService = chartDataService ?? throw new ArgumentNullException(nameof(chartDataService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            SelectionChangeCommand = new DelegateCommand(SelectionChanged);
            LoadMonth();
            LoadChartData();
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


        private void SelectionChanged()
        {
            if (SelectedValue != null)
            {
                string SelectedItem = SelectedValue;
                if (SelectedValue != "Q1" && SelectedValue != "Q2" && SelectedValue != "Q3" && SelectedValue != "All")
                {
                    int month = DateTime.ParseExact(SelectedValue, "MMMM", CultureInfo.CurrentCulture).Month;
                    SelectedItem = Convert.ToString(month);
                }

                LoadChartData(SelectedItem);
            }
            else
            {
                LoadChartData();
            }
        }

        private async void LoadChartData(string SelectedValue = "All")
        {
            await Task.Run(async () =>
            {
                var _result = await _chartDataService.GetSalesByStore(SelectedValue);

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

        //private ChartDataModel MapToChartData(DashbordSalesVM data)
        //{

        //    if (data == null || !data.salesByStores.Any())
        //    {
        //        return default;
        //    }

        //    var _model = new ChartDataModel();

        //    List<String> Labels = new List<string>();
        //    ChartValues<double> total = new ChartValues<double>();

        //    foreach (var _stores in data.salesByStores)
        //    {

        //        total.Add(System.Math.Round(_stores.TotalSales,2));
        //    }
        //    _model.SeriesCollection.Add(new ColumnSeries
        //    {
        //        Title = "Total Sales",
        //        Values = total,
        //        LabelPoint = chartPoint => string.Format("{0}", chartPoint.Y),
        //        DataLabels = true,
        //    });
        //    _model.Labels =data.salesByStores.Select(x=>x.StoreName).ToArray();
        //    return _model;

        //}

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

    }
}
