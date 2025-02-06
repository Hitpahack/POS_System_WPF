using FalcaPOS.Common.Helper;
using FalcaPOS.Entites.AddInventory;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{
    public interface IInvoiceService
    {
        Task<Response<IEnumerable<InvoiceListViewModel>>> GetInvoices(CancellationToken token = default(CancellationToken));

        Task<Response<AddStockProductViewModel>> GetInvoiceDetails(int invoiceID, CancellationToken token = default(CancellationToken));

        Task<Response<string>> UpdateInvoiceDetails(int invoiceID, UpdateInvoiceViewModel updateInvoice, CancellationToken token = default(CancellationToken));

        Task<Response<InvoiceVm>> GetDefectiveLsit(InvoiceModelRequest modelRequest, CancellationToken token = default(CancellationToken));

        Task<Response<IEnumerable<StockProductViewModel>>> GetSalesDefectiveLsit(CancellationToken token = default(CancellationToken));

        Task<Response<List<StockProductViewModel>>> GetProductDetails(string InoviceNo, CancellationToken token = default(CancellationToken));

    }
}
