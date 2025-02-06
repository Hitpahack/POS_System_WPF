using FalcaPOS.Common.Helper;
using FalcaPOS.Entites.Products;
using FalcaPOS.Entites.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts {
    public interface IReportServices {
        Task<Response<IEnumerable<InventoryReportModelPM>>> GetInventoryReport(CancellationToken token = default(CancellationToken));

        Task<Response<IEnumerable<InventoryReportPoGrnModel>>> GetPOGRNReport(DateTime FromDate, DateTime ToDate, CancellationToken token = default(CancellationToken));

        Task<Response<IEnumerable<TopListItemDTO>>> GetTopListItems(DateTime FromDate,DateTime ToDate, CancellationToken token = default(CancellationToken));
        Task<Response<IEnumerable<TopListBrandDTO>>> GetTopListBrand(DateTime FromDate, DateTime ToDate, CancellationToken token = default(CancellationToken));
        Task<Response<IEnumerable<TopListCategoryDTO>>> GetTopListCategories(DateTime FromDate, DateTime ToDate, CancellationToken token = default(CancellationToken));
        Task<Response<IEnumerable<TopListTransactionsDTO>>> GetTopListTransactions(DateTime FromDate, DateTime ToDate, CancellationToken token = default(CancellationToken));


    }
}
