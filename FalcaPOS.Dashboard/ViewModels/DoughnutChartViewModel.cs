using FalcaPOS.Dashboard.Services;
using FalcaPOS.Entites.Dashboard;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.Dashboard.ViewModels
{
    public class DoughnutChartViewModel : BindableBase
    {

        private readonly IChartDataService _chartDataService;




        public DoughnutChartViewModel(IChartDataService chartDataService)
        {
            _chartDataService = chartDataService ?? throw new ArgumentNullException(nameof(chartDataService));

            LoadDataBrand();
            LoadDataProduct();
        }



        private List<MostNumberofsalesItem> _MostNumberofProduct;

        public List<MostNumberofsalesItem> MostNumberofProduct
        {
            get { return _MostNumberofProduct; }
            set { SetProperty(ref _MostNumberofProduct, value); }
        }

        private List<MostNumberofsalesItem> _MostNumberofBrand;

        public List<MostNumberofsalesItem> MostNumberofsalesBrand
        {
            get { return _MostNumberofBrand; }
            set { SetProperty(ref _MostNumberofBrand, value); }
        }


        private async void LoadDataBrand()
        {
            await Task.Run(async () =>
            {
                var _result = await _chartDataService.GetMostNumberOfBrnad();

                if (_result != null && _result.IsSuccess)
                {
                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        MostNumberofsalesBrand = _result.Data.MostNumberOfSales.ToList();
                    });
                }

            });


        }

        private async void LoadDataProduct()
        {
            await Task.Run(async () =>
            {
                var _result = await _chartDataService.GetMostNumberOfProduct();

                if (_result != null && _result.IsSuccess)
                {
                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        MostNumberofProduct = _result.Data.MostNumberOfSales.ToList();
                    });
                }

            });


        }


    }
}
