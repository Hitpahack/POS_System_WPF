using FalcaPOS.Common.Helper;
using FalcaPOS.Entites.Finance;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{
    public interface IFinanceService
    {
        Task<Response<FinanceSalesResult>> GetFinanceInvoices(FinanceSearch search = null, CancellationToken token = default(CancellationToken));

        Task<Response<IEnumerable<TallyExportModel>>> GetTallyExport(TallyExportSearchModel model, CancellationToken token = default(CancellationToken));


    }
}
