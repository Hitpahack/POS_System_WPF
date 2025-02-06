using FalcaPOS.Common.Helper;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.Indent;
using FalcaPOS.Entites.Suppliers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{
    public interface IIndentService
    {

        Task<Response<string>> IndentApproval(IndentViewModel indent, CancellationToken token = default(CancellationToken));

        Task<Response<string>> IndentRejectToReCreate(IndentViewModel indent, CancellationToken token = default(CancellationToken));

        Task<Response<IEnumerable<IndentViewModel>>> ViewIndentList(IndentListSearch indentListSearch, CancellationToken token = default(CancellationToken));

        Task<Response<IndentViewModel>> ViewDetailIndent(int Id, CancellationToken token = default(CancellationToken));


        Task<Response<string>> GetCurrentPONumber(int StoreId,CancellationToken token = default(CancellationToken));

        Task<Response<string>> GetPONOSequenceFormatForStore(int StoreId, CancellationToken token = default(CancellationToken));

        Task<Response<string>> CreateIndent(IndentCreateModel indent, CancellationToken token = default);

        Task<Response<string>> AddSupplierToIndent(AddSupplierModel indent, CancellationToken token = default(CancellationToken));

        Task<Response<string>> StatusChangeClose(int IndentId, string Remarks, CancellationToken token = default(CancellationToken));

        Task<Response<string>> EditProductPriceIndent(IndentViewModel indent, CancellationToken token = default(CancellationToken));

        Task<Response<PDFFileResult>> GetPurchaseOrderPDF(string PoNumber, CancellationToken token = default(CancellationToken));


        Task<Response<IEnumerable<IndentViewModel>>> GetIndentInTransitList(CancellationToken token = default(CancellationToken));



        Task<Response<Supplier>> GetBankDetailsList(int SupplierId, int IndentId, CancellationToken token = default(CancellationToken));

        Task<Response<List<BulkPaymentDownloadModel>>> GetBulkPaymentList(List<int> SupplierId, int StoreId,string FromDate,string ToDate, CancellationToken token = default(CancellationToken));

        Task<Response<string>> CreditNoteAdjustment(int IndentId, UpdateAdjustmentModel update, CancellationToken token = default(CancellationToken));

        Task<Response<string>> BulkPaymentUpdate(List<BulkPaymentUpdateModel> indent, CancellationToken token = default(CancellationToken));

        Task<Response<string>> UpdatePartialPayment(int Id, float PaymentTotal, string PaymentDate,float TdsAmount, CancellationToken token = default(CancellationToken));


        Task<Response<List<PendingPaymentSupplier>>> GetPendingPaymentSupplier(CancellationToken token = default(CancellationToken));

        Task<Response<string>> StatusChangeToPlaced(int IndentId, string Remarks, CancellationToken token = default(CancellationToken));

        Task<Response<string>> StatusChangeToInTransit(int IndentId, string TrackId, string Remarks, CancellationToken token = default(CancellationToken));
        Task<Response<string>> StatusChangeToReceived(int IndentId, float PayableAmount, List<IndentProduct> indentProducts, CancellationToken token = default(CancellationToken));

        Task<Response<string>> IndentReview(int IndentId, List<IndentReviewProduct> indentReview, CancellationToken token = default(CancellationToken));

        Task<Response<string>> IndentApprove(int IndentId, List<IndentApproveProduct> indentApprove, CancellationToken token = default(CancellationToken));

        Task<Response<ResponseInvoice>> GetPOInvoiceFromInvoiceModule(string PoNumber, CancellationToken token = default(CancellationToken));





    }
}
