using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Dashboard.Models;
using FalcaPOS.Entites.Dashboard;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Dashboard.Services
{
    public class ChartDataService : IChartDataService
    {

        private readonly Logger _logger;

        public ChartDataService(Logger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<ChartDataModel> GetStackedSeries()
        {
            return await Task.FromResult(GetStackSerires());
        }

        private ChartDataModel GetStackSerires()
        {
            var _series = new ChartDataModel();

            //_series.SeriesCollection.Add(new StackedColumnSeries
            //{
            //    Title = "City 1 Store",
            //    Values = new ChartValues<double> { 4, 5, 6, 7, 1 },
            //    DataLabels = true
            //});
            //_series.SeriesCollection.Add(new StackedColumnSeries
            //{
            //    Title = "City 2 Store",
            //    Values = new ChartValues<double> { 1, 3, 0, 10, 1 },
            //    DataLabels = true
            //});
            //_series.SeriesCollection.Add(new StackedColumnSeries
            //{
            //    Title = "City 3 Store",
            //    Values = new ChartValues<double> { 12, 33, 30, 10, 41 },
            //    DataLabels = true
            //});
            //_series.SeriesCollection.Add(new StackedColumnSeries
            //{
            //    Title = "City 4 Store",
            //    Values = new ChartValues<double> { 21, 35, 70, 110, 41 },
            //    DataLabels = true
            //});
            //_series.SeriesCollection.Add(new StackedColumnSeries
            //{
            //    Title = "City 5 Store",
            //    Values = new ChartValues<double> { 11, 33, 40, 10, 21 },
            //    DataLabels = true
            //});
            //_series.SeriesCollection.Add(new StackedColumnSeries
            //{
            //    Title = "City 6 Store",
            //    Values = new ChartValues<double> { 11, 73, 10, 10, 1 },
            //    DataLabels = true
            //});



            //_series.Labels = new[] { "Bangalore", "Hubli" };

            //_series.YFormatter = val => val.ToString("N");

            return _series;
        }


        public async Task<Response<DashbordSalesVM>> GetSalesByStore(string Quarter = null, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper
                    .GetAsync<Response<DashbordSalesVM>>
                    ($"{AppConstants.GET_SALES_BY_STORE}?Quarter={Quarter}", AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (OperationCanceledException)
            {
                _logger.LogError("Operation was cancelled by user get sales by store");
            }

            catch (Exception _ex)
            {
                _logger.LogError("Error in getting sales by stre", _ex);
            }

            return new Response<DashbordSalesVM>
            {
                IsSuccess = false,
                Error = "An error occurred ,try again"
            };

        }

        public async Task<Response<DashbordSupplierVM>> GetSupplierByStore(string Quarter = null, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper
                    .GetAsync<Response<DashbordSupplierVM>>
                    ($"{AppConstants.GET_SUPPLIER_BY_STORE}?Quarter={Quarter}", AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (OperationCanceledException)
            {
                _logger.LogError("Operation was cancelled by user get supplier by store");
            }

            catch (Exception _ex)
            {
                _logger.LogError("Error in getting supplier by stre", _ex);
            }

            return new Response<DashbordSupplierVM>
            {
                IsSuccess = false,
                Error = "An error occurred ,try again"
            };

        }

        public async Task<Response<DashbordMonthSalesVM>> GetSalesByMonth(string Year = null, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper
                    .GetAsync<Response<DashbordMonthSalesVM>>
                    ($"{AppConstants.GET_SALES_BY_MONTH}?Year={Year}", AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (OperationCanceledException)
            {
                _logger.LogError("Operation was cancelled by user get sales by month");
            }

            catch (Exception _ex)
            {
                _logger.LogError("Error in getting sales by month", _ex);
            }

            return new Response<DashbordMonthSalesVM>
            {
                IsSuccess = false,
                Error = "An error occurred ,try again"
            };

        }


        public async Task<Response<DashbordMostnumberofSales>> GetMostNumberOfBrnad(CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper
                    .GetAsync<Response<DashbordMostnumberofSales>>
                    ($"{AppConstants.GET_SALES_BY_BRAND}", AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (OperationCanceledException)
            {
                _logger.LogError("Operation was cancelled by user get sales by store");
            }

            catch (Exception _ex)
            {
                _logger.LogError("Error in getting sales by stre", _ex);
            }

            return new Response<DashbordMostnumberofSales>
            {
                IsSuccess = false,
                Error = "An error occurred ,try again"
            };

        }

        public async Task<Response<DashbordMostnumberofSales>> GetMostNumberOfProduct(CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper
                    .GetAsync<Response<DashbordMostnumberofSales>>
                    ($"{AppConstants.GET_SALES_BY_PRODUCT}", AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (OperationCanceledException)
            {
                _logger.LogError("Operation was cancelled by user get sales by store");
            }

            catch (Exception _ex)
            {
                _logger.LogError("Error in getting sales by stre", _ex);
            }

            return new Response<DashbordMostnumberofSales>
            {
                IsSuccess = false,
                Error = "An error occurred ,try again"
            };

        }

        public Task<ChartDataModel> GetPieSeries()
        {
            var _result = new ChartDataModel();

            //_result.SeriesCollection.Add(new PieSeries
            //{
            //    Title = "Store one",
            //    Values = new ChartValues<double> { 10 },
            //    LabelPoint = chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation),
            //    DataLabels = true
            //}); _result.SeriesCollection.Add(new PieSeries
            //{
            //    Title = "Store two",
            //    Values = new ChartValues<double> { 20 },
            //    LabelPoint = chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation),
            //    DataLabels = true
            //}); _result.SeriesCollection.Add(new PieSeries
            //{
            //    Title = "Store three",
            //    Values = new ChartValues<double> { 15 },
            //    LabelPoint = chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation),
            //    DataLabels = true
            //});
            return Task.FromResult(_result);
        }

        public Task<ChartDataModel> GetDoughnutSeries()
        {
            var _result = new ChartDataModel();

            //_result.SeriesCollection.Add(new PieSeries
            //{
            //    Title = "Store One",
            //    Values = new ChartValues<ObservableValue> { new ObservableValue(10) },
            //    DataLabels = true
            //});
            //_result.SeriesCollection.Add(new PieSeries
            //{
            //    Title = "Store Two",
            //    Values = new ChartValues<ObservableValue> { new ObservableValue(5) },
            //    DataLabels = true
            //});
            //_result.SeriesCollection.Add(new PieSeries
            //{
            //    Title = "Store Three",
            //    Values = new ChartValues<ObservableValue> { new ObservableValue(20) },
            //    DataLabels = true
            //});
            //_result.SeriesCollection.Add(new PieSeries
            //{
            //    Title = "Store Four",
            //    Values = new ChartValues<ObservableValue> { new ObservableValue(15) },
            //    DataLabels = true
            //});


            return Task.FromResult(_result);
        }
    }
}
