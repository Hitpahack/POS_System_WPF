using FalcaPOS.Common.Helper;
using FalcaPOS.Dashboard.Models;
using FalcaPOS.Entites.Dashboard;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Dashboard.Services
{
    public interface IChartDataService
    {
        Task<Response<DashbordSalesVM>> GetSalesByStore(String Quarter = null, CancellationToken token = default(CancellationToken));

        Task<ChartDataModel> GetStackedSeries();


        Task<ChartDataModel> GetPieSeries();

        Task<ChartDataModel> GetDoughnutSeries();

        Task<Response<DashbordSupplierVM>> GetSupplierByStore(String Quarter = null, CancellationToken token = default(CancellationToken));


        Task<Response<DashbordMonthSalesVM>> GetSalesByMonth(String Year = null, CancellationToken token = default(CancellationToken));

        Task<Response<DashbordMostnumberofSales>> GetMostNumberOfBrnad(CancellationToken token = default(CancellationToken));
        Task<Response<DashbordMostnumberofSales>> GetMostNumberOfProduct(CancellationToken token = default(CancellationToken));


    }
}
