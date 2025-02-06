using FalcaPOS.Common.Helper;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.EwayBill;
using FalcaPOS.Entites.Stock;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{
    public interface IStockTransferService
    {
        Task<Response<string>> StockTransfer(StockTransferDTO stockTrnasferModel, CancellationToken token = default(CancellationToken));

        Task<Response<IEnumerable<ReceiverViewModel>>> GetStockReceiver(int storeId, CancellationToken token = default(CancellationToken));

        Task<Response<IEnumerable<StockTrnasferModel>>> GetStockReceiverList(CancellationToken token = default(CancellationToken));

        Task<Response<string>> UpdateStockReceiver(ReceiverUpdateModel stockReceiverModel, CancellationToken token = default(CancellationToken));

        Task<Response<string>> UpdateStockTransferApproval(string TransferOrderNo, CancellationToken token = default(CancellationToken));


        Task<Response<PDFFileResult>> GetStockTransferPdf(string SRNumber, CancellationToken token = default(CancellationToken));

        Task<Response<string>> GetCurrentStockTransferNumber(int toStoreId,CancellationToken token = default(CancellationToken));

        Task<Response<string>> StockRequest(StockTranferRequestModel TrnasferModel, CancellationToken token = default(CancellationToken));

        Task<Response<IEnumerable<TransferViewModel>>> GetStockTransferList(CancellationToken token = default(CancellationToken));

        Task<Response<string>> UploadFiles(string supplierid, FileUploadInfo[] fileUploads);

        Task<Response<IEnumerable<TransferCompletedViewModel>>> GetStockTransferCompletedStock(string FromDate, string ToDate,string Status, CancellationToken token = default(CancellationToken));

        Task<string> GetCurrentStockTransferReceiptNumber(CancellationToken token = default(CancellationToken));

        Task<Response<IEnumerable<TransferCompletedViewModel>>> GetStockTransferSearchList(string FromDate, string ToDate, CancellationToken token = default(CancellationToken));

        Task<Response<string>> RspStockRequest(StockTrnasferModel stockTrnasferModel, CancellationToken token = default(CancellationToken));

        Task<Response<string>> RspStockTransfer(StockTransferDTO stockTrnasferModel, CancellationToken token = default(CancellationToken));

        Task<Response<IEnumerable<TransferCompletedViewModel>>> StoreStockTransferSearchV2(string FromDate, string ToDate, string Status, CancellationToken token = default(CancellationToken));

        Task<Response<IEnumerable<TransferCompletedViewModel>>> StockTransferSearchV2(string FromDate, string ToDate,string Status,CancellationToken token = default(CancellationToken));
        Task<EwayBillResponse> GenerateEwayBill(EWayBillModel eWayBillModel,int transferId, bool IsTransportAdded, CancellationToken token = default(CancellationToken));
        Task<AccessTokenResponse> GetEwayBillAccessToken(CancellationToken token = default(CancellationToken));
        Task<Response<IEnumerable<TransferCompletedViewModel>>> GetStockTransferRequestList(CancellationToken token = default(CancellationToken));
        Task<Response<string>> UpdateStockTrnasferApproveV2(int StockTransferId, CancellationToken token = default(CancellationToken));
        Task<Response<string>> UpdateStockTransferRejectV2(int StockTransferId,string Remarks,CancellationToken token = default(CancellationToken));
        Task<Response<string>> UpdateStockTranferQty(TransferCompletedViewModel TransferModel, CancellationToken token = default(CancellationToken));


    }
}
