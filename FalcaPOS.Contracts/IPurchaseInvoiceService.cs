using FalcaPOS.Common.Helper;
using FalcaPOS.Entites.AddInventory;
using FalcaPOS.Entites.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{
    public interface IPurchaseInvoiceService
    {
        Task<Response<IEnumerable<PurchaseInvoiceViewModel>>> GetPurchaseInvoices(InvoiceSearchParams searchParams, CancellationToken token = default);

        Task<Response<IEnumerable<InvoiceProductViewModel>>> GetProductDetails(int InvoiceId, CancellationToken token = default);

        Task<Response<IEnumerable<StockProductViewModel>>> GetPurchaseReturnsSearch(string ProductSKU, string LotNumber, CancellationToken token = default);

        Task<Response<string>> UpdatePurchaseReturns(List<StockProductViewModel> updateModel, CancellationToken token = default);

        Task<Response<IEnumerable<StoreReturnModel>>> GetPurchaseReturnsListView(string status, int SupplierId, int StoreId, CancellationToken token = default);

        Task<Response<string>> ApprovePurchaseReturns(StoreReturnModel updateModel, CancellationToken token = default);

        Task<Response<string>> PostCNApprovePurchaseReturns(int CreditnoteId, string Status, string Remark, CancellationToken token = default);

        Task<Response<IEnumerable<SummaryViewModelCreidtNote>>> GetCreditNoteSummaryList(int SupplierId, int StoreId, string FromDate, string ToDate, CancellationToken token = default);

        Task<Response<IEnumerable<SummaryDetailsViewModelCreditNote>>> GetCreditNoteSummaryDetails(int SupplierId, int StoreId, string FromDate, string ToDate, CancellationToken token = default);


    }
}
