using FalcaPOS.Dashboard.Models;
using FalcaPOS.Dashboard.Services;
using Prism.Mvvm;
using System;

namespace FalcaPOS.Dashboard.ViewModels
{
    public class TotalSalesViewModel : BindableBase
    {

        private readonly IChartDataService _chartDataService;

        public TotalSalesViewModel(IChartDataService chartDataService)
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

        private async void LoadChartData()
        {
            ChartData = await _chartDataService.GetStackedSeries();

        }
    }
}
