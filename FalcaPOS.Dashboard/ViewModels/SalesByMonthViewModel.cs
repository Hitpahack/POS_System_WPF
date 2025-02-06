using FalcaPOS.Dashboard.Models;
using FalcaPOS.Dashboard.Services;
using Prism.Mvvm;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.Dashboard.ViewModels
{
    public class SalesByMonthViewModel : BindableBase
    {
        private readonly IChartDataService _chartDataService;
        public SalesByMonthViewModel(IChartDataService chartDataService)
        {
            _chartDataService = chartDataService ?? throw new ArgumentNullException(nameof(chartDataService));
            LoadChartData();
        }

        private ChartDataModel _chartData;
        public ChartDataModel ChartData
        {
            get { return _chartData; }
            set { SetProperty(ref _chartData, value); }
        }


        private async void LoadChartData(string SelectedValue = null)
        {
            await Task.Run(async () =>
            {
                var _result = await _chartDataService.GetSalesByMonth(SelectedValue);

                if (_result != null && _result.IsSuccess)
                {
                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        //ChartData = MapToChartData(_result.Data);
                    });
                }

            });


        }

        //private ChartDataModel MapToChartData(DashbordMonthSalesVM data)
        //{

        //    if (data == null || !data.MonthSales.Any())
        //    {
        //        return default;
        //    }

        //    var _model = new ChartDataModel();

        //    List<String> Labels = new List<string>();
        //    ChartValues<double> total = new ChartValues<double>();

        //    foreach (var _stores in data.MonthSales)
        //    {

        //        total.Add(System.Math.Round(_stores.TotalSales, 2));
        //    }
        //    _model.SeriesCollection.Add(new ColumnSeries
        //    {
        //        Title = "Total Sales",
        //        Values = total,
        //        LabelPoint = chartPoint => string.Format("{0}", chartPoint.Y),
        //        DataLabels = true,

        //    });
        //    List<string> Month = new List<string>();
        //    var allmonth = StringHelper.AllMonth();
        //    foreach (var item in data.MonthSales)
        //    {
        //        foreach (var key in allmonth)
        //        {
        //            if (key.Key== item.MonthNumber)
        //            {
        //                Month.Add(key.Value);
        //            }
        //        }
        //    }
        //    _model.Labels = Month.ToArray();
        //    return _model;

        //}
    }
}
