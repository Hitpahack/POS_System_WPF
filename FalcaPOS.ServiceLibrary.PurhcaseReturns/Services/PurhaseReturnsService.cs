using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.AddInventory;
using FalcaPOS.Entites.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace FalcaPOS.ServiceLibrary.PurhcaseReturns.Services
{
    public class PurhaseReturnsService : IPurchaseReturnService
    {
        private readonly Logger _logger;
        public PurhaseReturnsService(Logger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Response<IEnumerable<PurchaseReturnProductViewModel>>> GetStoreReturnsSearch(int SupplierId, string ProductSKU, string LotNumber, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();


                var _result = await HttpHelper.PostAsync<object, Response<IEnumerable<PurchaseReturnProductViewModel>>>
                    ($"{AppConstants.GET_STORE_RETURN_SEARCH}/{SupplierId}", new { ProductSKU, LotNumber }, AppConstants.ACCESS_TOKEN, token);

                token.ThrowIfCancellationRequested();

                return _result;

            }
            catch (OperationCanceledException) { }
            catch (Exception _ex)
            {
                _logger.LogError("Error in get product details service", _ex);
            }

            return new Response<IEnumerable<PurchaseReturnProductViewModel>>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<string>> UpdateStorePurchaseReturns(StoreReturnModels updateModel, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.PostAsync<StoreReturnModels, Response<string>>
                    (AppConstants.UPDATE_STORE_PURCHASE_RETURN, updateModel, AppConstants.ACCESS_TOKEN, token);

                token.ThrowIfCancellationRequested();

                return _result;

            }
            catch (OperationCanceledException) { }
            catch (Exception _ex)
            {
                _logger.LogError("Error in get product details service", _ex);
            }

            return new Response<string>
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

        public async Task<Response<IEnumerable<StoreReturnModel>>> GetStorePurchaseReturnsListView(string status, int SupplierId, string FromDate,string ToDate, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();


                var _result = await HttpHelper.PostAsync<object,Response<IEnumerable<StoreReturnModel>>>
                    (AppConstants.GET_STORE_PURCHASE_RETURN_LIST,new {SupplierId,status, FromDate,ToDate},AppConstants.ACCESS_TOKEN, token);

                token.ThrowIfCancellationRequested();

                return _result;

            }
            catch (OperationCanceledException) { }
            catch (Exception _ex)
            {
                _logger.LogError("Error in get product details service", _ex);
            }

            return new Response<IEnumerable<StoreReturnModel>>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<string>> UpdateStoreReturnWithAttachment(StoreReturnModel updateModel, FileUploadInfo[] fileUploads, CancellationToken token = default)
        {

            try
            {
                using (var _formContent = new MultipartFormDataContent())
                {
                    var _serializedConetent = new StringContent(JsonConvert.SerializeObject(updateModel), Encoding.UTF8, "application/json");

                    _formContent.Add(_serializedConetent, "StoreReturnModel");

                    if (fileUploads != null && fileUploads.Length > 0)
                    {
                        MapUploadFilesToForm(fileUploads, _formContent);
                    }

                    var _result = await HttpHelper.PostFormDataWithAttachmentAsync<Response<string>>(AppConstants.UPDATE_STORE_PURCHASE_RETURN_WITH_ATTACHMENT, AppConstants.ACCESS_TOKEN, _formContent);



                    if (_result != null && _result.IsSuccess)
                    {
                        return _result;
                    }
                    else
                    {


                        return _result;
                    }
                }

            }
            catch (OperationCanceledException) { }
            catch (Exception _ex)
            {
                _logger.LogError("Error in get product details service", _ex);
            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };

        }

        public async Task<Response<string>> EditReturnProduct(EditStoreReturnModel updateModel, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();


                var _result = await HttpHelper.PostAsync<EditStoreReturnModel, Response<string>>
                    (AppConstants.EDIT_PURCHASE_RETURN_PRODUCT, updateModel, AppConstants.ACCESS_TOKEN, token);

                token.ThrowIfCancellationRequested();

                return _result;

            }
            catch (OperationCanceledException) { }
            catch (Exception _ex)
            {
                _logger.LogError("Error in get product details service", _ex);
            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }
    }
}
