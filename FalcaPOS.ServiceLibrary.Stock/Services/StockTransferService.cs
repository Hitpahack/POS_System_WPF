using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.EwayBill;
using FalcaPOS.Entites.Stock;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.ServiceLibrary.Stock.Services
{
    public class StockTransferService : IStockTransferService
    {

        private readonly Logger logger;

        public StockTransferService(Logger Logger)
        {
            logger = Logger ?? throw new ArgumentNullException(nameof(Logger));
        }

        public async Task<Response<IEnumerable<ReceiverViewModel>>> GetStockReceiver(int storeId, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                Response<IEnumerable<ReceiverViewModel>> _result = await HttpHelper.GetAsync<Response<IEnumerable<ReceiverViewModel>>>
                    ($"{AppConstants.GET_STOCK_RECEIVER}/{storeId}", AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
            return new Response<IEnumerable<ReceiverViewModel>>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<string>> StockTransfer(StockTransferDTO stockTrnasferModel, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                Response<string> _result = await HttpHelper.PostAsync<StockTransferDTO, Response<string>>
                    (AppConstants.PUT_STOCK_TRANSFER, stockTrnasferModel, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
            return new Response<string>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<PDFFileResult>> GetStockTransferPdf(string SRNumber, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                Response<PDFFileResult> _result = await HttpHelper.PostAsync<string, Response<PDFFileResult>>
                    (AppConstants.GET_STOCK_TRANSFERPDF, SRNumber, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
            return new Response<PDFFileResult>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<string>> GetCurrentStockTransferNumber(int toStoreId,CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.GetAsync<Response<string>>($"{AppConstants.GET_CURRENT_STREQUEST_NUMBER}/{toStoreId}", AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }

            return new Response<string>
            {
                IsSuccess = false,
            };
        }

        public async Task<Response<IEnumerable<StockTrnasferModel>>> GetStockReceiverList(CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                Response<IEnumerable<StockTrnasferModel>> _result = await HttpHelper.GetAsync<Response<IEnumerable<StockTrnasferModel>>>
                    (AppConstants.GET_STOCK_RECEIVER_LIST, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
            return new Response<IEnumerable<StockTrnasferModel>>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<string>> UpdateStockReceiver(ReceiverUpdateModel stockReceiverModel, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                Response<string> _result = await HttpHelper.PostAsync<ReceiverUpdateModel, Response<string>>
                    (AppConstants.UDATE_STOCK_RECEIVER, stockReceiverModel, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
            return new Response<string>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<string>> UpdateStockTransferApproval(string TransferOrderNo, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                Response<string> _result = await HttpHelper.PostAsync<string, Response<string>>
                    (AppConstants.UDATE_STOCK_TRANSFER_APPROVAL, TransferOrderNo, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
            return new Response<string>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<string>> StockRequest(StockTranferRequestModel TrnasferModel, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                Response<string> _result = await HttpHelper.PostAsync<StockTranferRequestModel, Response<string>>
                    (AppConstants.PUT_STOCK_REQUEST, TrnasferModel, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
            return new Response<string>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<IEnumerable<StockTrnasferModel>>> GetStockList(CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                Response<IEnumerable<StockTrnasferModel>> _result = await HttpHelper.GetAsync<Response<IEnumerable<StockTrnasferModel>>>
                    (AppConstants.GET_STOCK_RECEIVER_LIST, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
            return new Response<IEnumerable<StockTrnasferModel>>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<IEnumerable<TransferViewModel>>> GetStockTransferList(CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                Response<IEnumerable<TransferViewModel>> _result = await HttpHelper.GetAsync<Response<IEnumerable<TransferViewModel>>>
                    (AppConstants.GET_STOCK_TRANSFER_LIST, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
            return new Response<IEnumerable<TransferViewModel>>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<string>> UploadFiles(string supplierid, FileUploadInfo[] fileUploads)
        {
            try
            {


                using (var _formContent = new MultipartFormDataContent())
                {

                    _formContent.Add(new StringContent(supplierid), "SRNumber");

                    MapUploadFilesToForm(fileUploads, _formContent);

                    var _result = await HttpHelper.PostFormDataAsync<Response<string>>(AppConstants.UPLOAD_TRANSPORT_FILES, AppConstants.ACCESS_TOKEN, _formContent);

                    return _result;
                }

            }
            catch (PosException _ex)
            {
                return new Response<string>
                {
                    IsSuccess = false,
                    Error = _ex?.Message
                };
            }
            catch (Exception _ex)
            {
                return new Response<string>
                {
                    IsSuccess = false,
                    Error = _ex?.Message
                };
            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "An error occurred while file upload, try uploading again"
            };
        }

        private void MapUploadFilesToForm(FileUploadInfo[] fileUploads, MultipartFormDataContent formContent)
        {
            for (int i = 0; i < fileUploads.Length; i++)
            {

                if (!File.Exists(fileUploads[i].FilePath))
                    throw new PosException($"File {fileUploads[i].FileName} is not avaliable , try again");

                formContent.Add(new StreamContent(new FileStream(fileUploads[i].FilePath, FileMode.Open)), $"Files", fileUploads[i].FileName);
            }
        }



        public async Task<Response<IEnumerable<TransferCompletedViewModel>>> GetStockTransferCompletedStock(string FromDate, string ToDate, string Status, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                Response<IEnumerable<TransferCompletedViewModel>> _result = await HttpHelper.PostAsync<object, Response<IEnumerable<TransferCompletedViewModel>>>
                    (AppConstants.GET_STOCK_TRANSFER_COMPLETED, new { FromDate, ToDate }, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
            return new Response<IEnumerable<TransferCompletedViewModel>>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<string> GetCurrentStockTransferReceiptNumber(CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.GetAsync<string>(AppConstants.GET_CURRENT_STRECEIPT_NUMBER, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }

            return null;
        }


        public async Task<Response<IEnumerable<TransferCompletedViewModel>>> GetStockTransferSearchList(string FromDate, string ToDate, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                Response<IEnumerable<TransferCompletedViewModel>> _result = await HttpHelper.PostAsync<object, Response<IEnumerable<TransferCompletedViewModel>>>
                    (AppConstants.GET_STOCK_TRANSFER_SEARCHLIST, new { FromDate, ToDate }, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
            return new Response<IEnumerable<TransferCompletedViewModel>>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<string>> RspStockRequest(StockTrnasferModel stockTrnasferModel, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                Response<string> _result = await HttpHelper.PostAsync<StockTrnasferModel, Response<string>>
                    (AppConstants.PUT_RSP_STOCK_REQUEST, stockTrnasferModel, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
            return new Response<string>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }


        public async Task<Response<string>> RspStockTransfer(StockTransferDTO stockTrnasferModel, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                Response<string> _result = await HttpHelper.PostAsync<StockTransferDTO, Response<string>>
                    (AppConstants.PUT_RSP_STOCK_TRANSFER, stockTrnasferModel, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
            return new Response<string>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }


        public async Task<Response<IEnumerable<TransferCompletedViewModel>>> StoreStockTransferSearchV2(string FromDate, string ToDate, string Status, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                Response<IEnumerable<TransferCompletedViewModel>> _result = await HttpHelper.PostAsync<object, Response<IEnumerable<TransferCompletedViewModel>>>
                    (AppConstants.GET_STORE_STOCK_TRANSFER_SEARCH, new { FromDate, ToDate, Status }, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
            return new Response<IEnumerable<TransferCompletedViewModel>>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<IEnumerable<TransferCompletedViewModel>>> StockTransferSearchV2(string FromDate, string ToDate,string Status, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                Response<IEnumerable<TransferCompletedViewModel>> _result = await HttpHelper.PostAsync<object, Response<IEnumerable<TransferCompletedViewModel>>>
                    (AppConstants.GET_STOCK_TRANSFER_SEARCHV2, new { FromDate, ToDate,Status }, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
            return new Response<IEnumerable<TransferCompletedViewModel>>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };

        }

        public async Task<EwayBillResponse> GenerateEwayBill(EWayBillModel eWayBillModel,int transferId,bool IsTransportAdded, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.PostAsync<EWayBillModel,EwayBillResponse>
                    ( $"{AppConstants.GET_EWAY_BILL_STOCKTRANSFER}/{transferId}/{IsTransportAdded}/", eWayBillModel, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
            return null;

        }

        public async Task<AccessTokenResponse> GetEwayBillAccessToken(CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.GetAsync<AccessTokenResponse>
                    (AppConstants.GET_EWAY_ACCESS_TOKEN, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
            return null;
        }

        public async Task<Response<IEnumerable<TransferCompletedViewModel>>> GetStockTransferRequestList(CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

           var  _result = await HttpHelper.GetAsync<Response<IEnumerable<TransferCompletedViewModel>>>
                    (AppConstants.GET_STOCK_TRANSFER_RQUEST_LIST, AppConstants.ACCESS_TOKEN, token); 

                return _result;
            }
            catch(Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
            return new Response<IEnumerable<TransferCompletedViewModel>>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<string>> UpdateStockTrnasferApproveV2(int StockTransferId, CancellationToken token = default)
        {

            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.PostAsync<int,Response<string>>
                         (AppConstants.UPDATE_STOCK_TRANSFER_APPROVE,StockTransferId, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
            return new Response<string>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<string>> UpdateStockTransferRejectV2(int StockTransferId, string Remarks, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.PostAsync<string,Response<string>>
                         ($"{AppConstants.UPDATE_STOCK_TRANSFER_REJECT}/{StockTransferId}",Remarks, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                logger.LogError(_ex.Message);
            }
            return new Response<string>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<string>> UpdateStockTranferQty(TransferCompletedViewModel TransferModel, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.PostAsync<TransferCompletedViewModel, Response<string>>
                         (AppConstants.UPDATE_STOCK_TRANSFER_QTY, TransferModel, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch(Exception _ex)
            {
                logger.LogError(_ex.Message);
            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }
    }
}
