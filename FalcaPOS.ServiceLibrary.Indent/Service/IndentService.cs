using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.Indent;
using FalcaPOS.Entites.Suppliers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace FalcaPOS.ServiceLibrary.Indent.Service
{
    public class IndentService : IIndentService
    {
        private readonly Logger _logger;

        public IndentService(Logger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<Response<string>> CreateIndent(IndentCreateModel indent, CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.PostAsync<IndentCreateModel, Response<string>>
                    (AppConstants.CREATE_INDENT, indent, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError("An error in get indent service ", _ex);
            }
            return new Response<string>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<string>> GetCurrentPONumber(int StoreId,CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.GetAsync<Response<string>>($"{ AppConstants.GET_CURRENT_PO_NUMBER}/{StoreId }", AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception)
            {
            }

            return new Response<string>
            {
                IsSuccess = false,
            };

        }

        public async Task<Response<string>> GetPONOSequenceFormatForStore(int StoreId, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.GetAsync<Response<string>>($"{AppConstants.GET_PO_SEQUENCE_FORMAT_FOR_STORE}/{StoreId}", AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception)
            {
            }

            return new Response<string>
            {
                IsSuccess = false,
            };

        }


        public async Task<Response<string>> IndentApproval(IndentViewModel indent, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.PostAsync<IndentViewModel, Response<string>>(AppConstants.INDENT_APPROVAL, indent, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError("An error in get IndentApproval ", _ex);
            }

            return new Response<string>
            {
                IsSuccess = false,
            };

        }

        public async Task<Response<string>> IndentRejectToReCreate(IndentViewModel indent, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.PostAsync<IndentViewModel, Response<string>>(AppConstants.INDENT_REJECT, indent, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError("An error in get IndentRejectToReCreate ", _ex);

            }

            return new Response<string>
            {
                IsSuccess = false,
            };

        }


        public async Task<Response<IEnumerable<IndentViewModel>>> ViewIndentList(IndentListSearch indentListSearch, CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper
                   .PostAsync<IndentListSearch, Response<IEnumerable<IndentViewModel>>>
                   (AppConstants.VIEW_INDENT_LIST, indentListSearch, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError("An error in get indent service ViewIndentList ", _ex);
            }
            return new Response<IEnumerable<IndentViewModel>>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<IndentViewModel>> ViewDetailIndent(int Id, CancellationToken token = default)
        {
            try
            {


                var _result = await HttpHelper
                   .PostAsync<int, Response<IndentViewModel>>
                   (AppConstants.VIEW_DETAIL_INDENT, Id, AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (Exception _ex)
            {
                _logger.LogError("An error in get indent service ", _ex);
            }
            return new Response<IndentViewModel>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }



        public async Task<Response<string>> AddSupplierToIndent(AddSupplierModel indent, CancellationToken token = default)
        {
            try
            {


                using (var _formContent = new MultipartFormDataContent())
                {
                    var _serializedConetent = new StringContent(JsonConvert.SerializeObject(indent.Products), Encoding.UTF8, "application/json");
                    var _discounts = new StringContent(JsonConvert.SerializeObject(indent.Discounts), Encoding.UTF8, "application/json");
                    _formContent.Add(new StringContent(indent.SupplierId.ToString()), "SupplierId");
                    //_formContent.Add(new StringContent(indent.ArrivingDate.ToString()), "ArrivingDate");
                    _formContent.Add(new StringContent(indent.CreditPeriod.ToString()), "CreditPeriod");
                    _formContent.Add(_serializedConetent, "Products");
                    _formContent.Add(new StringContent(indent.Id.ToString()), "IndentId");
                    _formContent.Add(new StringContent(indent.GSTType.ToString()), "GSTType");
                    _formContent.Add(new StringContent(indent.AdvisoryCharges.ToString()), "AdvisoryCharges");
                    _formContent.Add(new StringContent(indent.IsFullPaid.ToString()), "IsFullPaid");
                    _formContent.Add(new StringContent(JsonConvert.SerializeObject(indent.PaymentsList), Encoding.UTF8, "application/json"), "PaymentsList");
                    _formContent.Add(_discounts,"Discounts");
                    _formContent.Add(new StringContent(indent.TC.ToString()), "TC");
                    _formContent.Add(new StringContent(indent.TransportChargesPayer.ToString()), "TransportChargesPayer");

                    if (indent.FileUploadListInfo != null && indent.FileUploadListInfo.Count > 0)
                    {
                        MapUploadFilesToForm(indent.FileUploadListInfo.ToArray(), _formContent);

                    }

                    var _result = await HttpHelper.PostFormDataWithAttachmentAsync<Response<string>>(AppConstants.ADD_SUPPLIER_TO_INDENT, AppConstants.ACCESS_TOKEN, _formContent);

                    return _result;

                    //token.ThrowIfCancellationRequested();

                    //var _result = await HttpHelper.PostAsync<IndentViewModel, Response<string>>(AppConstants.ADD_SUPPLIER_TO_INDENT, indent, AppConstants.ACCESS_TOKEN, token);

                    //return _result;
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError("An error in get indent service AddSupplierToIndent", _ex);
            }

            return new Response<string>
            {
                IsSuccess = false,
            };

        }

        public async Task<Response<string>> StatusChangeClose(int IndentId, string Remarks, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.PutAsync<string, Response<string>>($"{AppConstants.STATUS_CHANGE}/{IndentId}", Remarks, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError("An error in get indent service StatusChange", _ex);
            }

            return new Response<string>
            {
                IsSuccess = false,
            };

        }




        public async Task<Response<string>> EditProductPriceIndent(IndentViewModel indent, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.PostAsync<IndentViewModel, Response<string>>(AppConstants.EDIT_PRODUCT_PRICE_INDENT, indent, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError("An error in get indent service EditProductPriceIndent", _ex);
            }

            return new Response<string>
            {
                IsSuccess = false,
            };

        }


        public async Task<Response<PDFFileResult>> GetPurchaseOrderPDF(string PoNumber, CancellationToken token = default(CancellationToken))
        {
            try
            {
                if (!PoNumber.IsValidString())
                {
                    return new Response<PDFFileResult>
                    {
                        IsSuccess = false,
                        Error = "Invalid Invoice number"
                    };
                }

                var _result = await HttpHelper
                        .GetAsync<Response<PDFFileResult>>($"{AppConstants.GET_PURCHASE_ORDER_PDF}/{HttpUtility.UrlEncode(PoNumber)}", AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                return new Response<PDFFileResult>
                {
                    IsSuccess = false,
                    Error = _ex.Message
                };
            }


        }

        public async Task<Response<IEnumerable<IndentViewModel>>> GetIndentInTransitList(CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper
                   .GetAsync<Response<IEnumerable<IndentViewModel>>>
                   (AppConstants.GET_INDENT_INTRANSITE_LIST, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError("An error in get indent service ", _ex);
            }
            return new Response<IEnumerable<IndentViewModel>>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        private void MapUploadFilesToForm(FileUploadInfo[] fileUploads, MultipartFormDataContent formContent)
        {
            for (int i = 0; i < fileUploads.Length; i++)
            {

                if (!File.Exists(fileUploads[i].FilePath))
                    throw new PosException($"File {fileUploads[i].FileName} is not available , try again");

                formContent.Add(new StreamContent(new FileStream(fileUploads[i].FilePath, FileMode.Open)), $"Files", fileUploads[i].FileName);
            }
        }



        public async Task<Response<Supplier>> GetBankDetailsList(int SupplierId, int IndentId, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                var _result = await HttpHelper.GetAsync<Response<Supplier>>($"{AppConstants.GETBANKDETAILSLIST}/{SupplierId}/{IndentId}", AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                return new Response<Supplier>
                {
                    IsSuccess = false,
                    Error = _ex.Message
                };
            }



        }


        public async Task<Response<List<BulkPaymentDownloadModel>>> GetBulkPaymentList(List<int> SupplierId, int StoreId, string FromDate, string ToDate, CancellationToken token = default)
        {


            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.PostAsync<object, Response<List<BulkPaymentDownloadModel>>>(AppConstants.GET_BULK_PAYMENT_LIST, new { SupplierId, StoreId,FromDate,ToDate }, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception)
            {
            }

            return new Response<List<BulkPaymentDownloadModel>>
            {
                IsSuccess = false,
            };
        }

        public async Task<Response<string>> CreditNoteAdjustment(int IndentId, UpdateAdjustmentModel update, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.PutAsync<UpdateAdjustmentModel, Response<string>>($"{AppConstants.CREDIT_NOTE_ADJUSTMENT}/{IndentId}", update, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception)
            {
            }

            return new Response<string>
            {
                IsSuccess = false,
            };

        }


        public async Task<Response<string>> BulkPaymentUpdate(List<BulkPaymentUpdateModel> indent, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.PostAsync<List<BulkPaymentUpdateModel>, Response<string>>(AppConstants.BULK_PAYMNET_UPDATE, indent, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception)
            {
            }

            return new Response<string>
            {
                IsSuccess = false,
            };

        }

        public async Task<Response<string>> UpdatePartialPayment(int Id, float PaymentTotal, string PaymentDate,float TdsAmount, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                var _result = await HttpHelper.
                    PutAsync<object, Response<string>>
                    ($"{AppConstants.UPDATE_PARTIAL_PAYMENT}/{Id}", new { PaymentDate, PaymentTotal,TdsAmount }, AppConstants.ACCESS_TOKEN, token);
                return _result;

            }
            catch (Exception)
            {
            }

            return new Response<string>
            {
                IsSuccess = false,
            };

        }

        public async Task<Response<List<PendingPaymentSupplier>>> GetPendingPaymentSupplier(CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper
                   .GetAsync<Response<List<PendingPaymentSupplier>>>
                   (AppConstants.GET_PENDING_PAYMNETSUPPLIER_LIST, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError("An error in get indent service ", _ex);
            }
            return new Response<List<PendingPaymentSupplier>>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };

        }

        public async Task<Response<string>> StatusChangeToPlaced(int IndentId, string Remarks, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.PutAsync<object, Response<string>>(
                AppConstants.STATUS_CHANGE_TO_PLACED,new {IndentId,Remarks}, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError("An error in get indent service StatusChange", _ex);
            }

            return new Response<string>
            {
                IsSuccess = false,
            };

        }

        public async Task<Response<string>> StatusChangeToInTransit(int IndentId, string TrackId, string Remarks, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _trackId = HttpUtility.UrlEncode(TrackId, Encoding.Unicode);
                var _result = await HttpHelper.PutAsync<object, Response<string>>(
                AppConstants.STATUS_CHANGE_TO_INTRASIT,new {IndentId,TrackId,Remarks }, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError("An error in get indent service StatusChange", _ex);
            }

            return new Response<string>
            {
                IsSuccess = false,
            };

        }

        public async Task<Response<string>> StatusChangeToReceived(int IndentId, float PayableAmount, List<IndentProduct> indentProducts, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();



                var _result = await HttpHelper.PutAsync<List<IndentProduct>, Response<string>>(
                $"{AppConstants.STATUS_CHANGE_TO_RECEIVED}/{IndentId}?PayableAmount={PayableAmount}", indentProducts, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError("An error in get indent service StatusChange", _ex);
            }

            return new Response<string>
            {
                IsSuccess = false,
            };

        }

        public async Task<Response<string>> IndentReview(int IndentId, List<IndentReviewProduct> indentReview, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.PutAsync<List<IndentReviewProduct>, Response<string>>(
                $"{AppConstants.TM_REVIEW}/{IndentId}", indentReview, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError("An error in get indent service StatusChange", _ex);
            }

            return new Response<string>
            {
                IsSuccess = false,
            };
        }

        public async Task<Response<string>> IndentApprove(int IndentId, List<IndentApproveProduct> indentApprove, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.PutAsync<List<IndentApproveProduct>, Response<string>>(
                $"{AppConstants.RM_APPROVE}/{IndentId}", indentApprove, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError("An error in get indent service StatusChange", _ex);
            }

            return new Response<string>
            {
                IsSuccess = false,
            };
        }

        public  async Task<Response<ResponseInvoice>> GetPOInvoiceFromInvoiceModule(string PoNumber, CancellationToken token = default)
        {
            try
            {
                if (!PoNumber.IsValidString())
                {
                    return new Response<ResponseInvoice>
                    {
                        IsSuccess = false,
                        Error = "Invalid Invoice number"
                    };
                }

                var _result = await HttpHelper
                        .GetAsync<Response<ResponseInvoice>>($"{AppConstants.GET_PURCHASE_ORDER_PDF_FROM_INVOICE_MODULE}/{HttpUtility.UrlEncode(PoNumber)}", AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                return new Response<ResponseInvoice>
                {
                    IsSuccess = false,
                    Error = _ex.Message
                };
            }
        }
    }
}
