using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.Sku;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.ServiceLibrary.Sku.Services
{
    public class SkuService : ISkuService
    {
        private readonly INotificationService _notificationService;

        private readonly Logger _logger;

        public SkuService(INotificationService notificationService, Logger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public async Task<Response<IEnumerable<ProductTypesViewModel>>> GetAllProductType(CancellationToken token = default(CancellationToken))
        {

            try
            {

                var _result = await HttpHelper.GetAsync<Response<IEnumerable<ProductTypesViewModel>>>(AppConstants.GET_ALL_PRODUCT_TYPE, AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            return new Response<IEnumerable<ProductTypesViewModel>>
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

        public async Task<Response<string>> VerifyProductCount(List<ProductTypesViewModel> typesViewModel, CancellationToken token = default(CancellationToken))
        {

            try
            {

                var _result = await HttpHelper.PostAsync<List<ProductTypesViewModel>, Response<string>>(AppConstants.VERIFY_PRODUCT_COUNT, typesViewModel, AppConstants.ACCESS_TOKEN, token);



                return _result;

            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

        public async Task<Response<IEnumerable<SkuSheetProductTypeViewModel>>> GetAllSku(CancellationToken token = default(CancellationToken))
        {

            try
            {

                var _result = await HttpHelper.GetAsync<Response<IEnumerable<SkuSheetProductTypeViewModel>>>(AppConstants.GET_ALL_SKU_SHEET, AppConstants.ACCESS_TOKEN, token);



                return _result;

            }
            catch (Exception _ex)
            {
                _logger.LogError("", _ex);
            }

            return new Response<IEnumerable<SkuSheetProductTypeViewModel>>
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

        public async Task<Response<IEnumerable<ProductTypesViewModel>>> GetDailyStockReportServices(DailyStockSearch dailyStockSearch, CancellationToken token = default(CancellationToken))
        {

            try
            {

                var _result = await HttpHelper.PostAsync<DailyStockSearch, Response<IEnumerable<ProductTypesViewModel>>>(AppConstants.GET_DAILY_STOCK, dailyStockSearch, AppConstants.ACCESS_TOKEN, token);



                return _result;

            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            return new Response<IEnumerable<ProductTypesViewModel>>
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

        public async Task<Response<string>> CreateSKURequest(List<CreateProductModel> createProducts, CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.PostAsync<List<CreateProductModel>, Response<string>>(AppConstants.CREATE_SKU_REQUEST, createProducts, AppConstants.ACCESS_TOKEN, token);



                return _result;

            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "getting error try again later"
            };
        }


        public async Task<Response<string>> UploadFiles(List<string> SKUReuestid, FileUploadInfo[] fileUploads)
        {
            try
            {


                using (var _formContent = new MultipartFormDataContent())
                {

                    foreach (var id in SKUReuestid)
                    {

                        _formContent.Add(new StringContent(id), "RequestId");


                    }
                    MapUploadFilesToForm(fileUploads, _formContent);




                    var _result = await HttpHelper.PostFormDataAsync<Response<string>>(AppConstants.UPLOAD_CERITFICATE_FILES, AppConstants.ACCESS_TOKEN, _formContent);

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

        //not using this commanded 
        //public async Task<Response<IEnumerable<SKUModel>>> ViewSKURequest(string Department, int Storeid, CancellationToken token = default)
        //{
        //    try
        //    {

        //        var _result = await HttpHelper.PostAsync<object, Response<IEnumerable<SKUModel>>>(AppConstants.VIEW_SKU_REQUEST, new { Department, Storeid }, AppConstants.ACCESS_TOKEN, token);



        //        return _result;

        //    }
        //    catch (Exception _ex)
        //    {
        //        _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
        //    }

        //    return new Response<IEnumerable<SKUModel>>
        //    {
        //        IsSuccess = false,
        //        Error = "No records found."
        //    };
        //}


        public async Task<Response<IEnumerable<TypeViewModel>>> GetApproveSKURequest(int CategoryId, CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.GetAsync<Response<IEnumerable<TypeViewModel>>>($"{AppConstants.GET_APPROVE_SKU_REQUEST}/{CategoryId}", AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            return new Response<IEnumerable<TypeViewModel>>
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

        public async Task<Response<string>> ApprovedSKURequest(SKURequestApproveModel sKUModel, CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.PostAsync<SKURequestApproveModel, Response<string>>(AppConstants.APPROVED_SKU_REQUEST, sKUModel, AppConstants.ACCESS_TOKEN, token);



                return _result;

            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

        public async Task<Response<SKUViewModelVM>> AlterSKUSearch(int StoreId, int ProductTypeId, int ManufaureId, CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.PostAsync<object, Response<SKUViewModelVM>>(AppConstants.ALTER_SKU_SEARCH, new { StoreId, ProductTypeId, ManufaureId }, AppConstants.ACCESS_TOKEN, token);



                return _result;

            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            return new Response<SKUViewModelVM>
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

        public async Task<Response<string>> UpdateSKUSearch(int StoreId, int ProductTypeId, int ManufaureId, UpdateSKVModel SKUupdate, CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.PostAsync<UpdateSKVModel, Response<string>>($"{AppConstants.UPDATE_SKU_CERTIFICATE}/{StoreId}/{ProductTypeId}/{ManufaureId}", SKUupdate, AppConstants.ACCESS_TOKEN, token);



                return _result;

            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

        public async Task<Response<ProductCertificateFileInfo>> GetproductCertificate(ProductCertificateSearch productCertificateSearch, CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.PostAsync<ProductCertificateSearch, Response<ProductCertificateFileInfo>>(AppConstants.GETPRODUCTCERTIFICATE, productCertificateSearch, AppConstants.ACCESS_TOKEN, token);
                return _result;

            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            return new Response<ProductCertificateFileInfo>
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

        public async Task<Response<List<ProductCertificateView>>> GetViewSKU(ViewSKUSearch view, CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.PostAsync<ViewSKUSearch, Response<List<ProductCertificateView>>>(AppConstants.VIEW_SKU, view, AppConstants.ACCESS_TOKEN, token);
                return _result;

            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            return new Response<List<ProductCertificateView>>
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

        //public async Task<Response<List<ProductCertificateView>>> GetViewSKUProduct(ViewSKUSearch viewSKU, CancellationToken token = default)
        //{
        //    try
        //    {

        //        var _result = await HttpHelper.PostAsync<ViewSKUSearch, Response<List<ProductCertificateView>>>(AppConstants.VIEW_SKU_PRODUCT, viewSKU, AppConstants.ACCESS_TOKEN, token);
        //        return _result;

        //    }
        //    catch (Exception _ex)
        //    {
        //        _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
        //    }

        //    return new Response<List<ProductCertificateView>>
        //    {
        //        IsSuccess = false,
        //        Error = "No records found."
        //    };
        //}

        public async Task<Response<List<string>>> CreateSKURequestWithCertificate(List<CreateSKUModel> createSKUs, CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.PostAsync<List<CreateSKUModel>, Response<List<string>>>(AppConstants.CREATE_SKU_REQUEST_WITH_CERTIFICATE, createSKUs, AppConstants.ACCESS_TOKEN, token);



                return _result;

            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            return new Response<List<string>>
            {
                IsSuccess = false,
                Error = "getting error try again later"
            };
        }

        public async Task<Response<IEnumerable<NewProductV2>>> GetAllSkuV2(CancellationToken token = default)
        {
            try
            {
                var _result = await HttpHelper.GetAsync<Response<IEnumerable<NewProductV2>>>(AppConstants.GET_ALL_SKU_SHEETV2, AppConstants.ACCESS_TOKEN, token);
                return _result;

            }
            catch (Exception _ex)
            {
                _logger.LogError("", _ex);
            }

            return new Response<IEnumerable<NewProductV2>>
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

        public async Task<Response<IEnumerable<NewProductV2>>> GetSKUApproveListV2(CancellationToken token = default)
        {

            try
            {
                var _result = await HttpHelper.GetAsync<Response<IEnumerable<NewProductV2>>>(AppConstants.GET_SKU_APPROVE_LIST, AppConstants.ACCESS_TOKEN, token);
                return _result;

            }
            catch (Exception _ex)
            {
                _logger.LogError("", _ex);
            }

            return new Response<IEnumerable<NewProductV2>>
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

        public async Task<Response<string>> RemovePendingApprovalSKU(int ProductId, string Remarks, CancellationToken token = default)
        {
            try
            {
                var _result = await HttpHelper.PostAsync<string,Response<string>>($"{AppConstants.REMOVE_SKU_APPROVAL_PENDING}/{ProductId}",Remarks, AppConstants.ACCESS_TOKEN, token);
                return _result;

            }
            catch (Exception _ex)
            {
                _logger.LogError("", _ex);
            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

        public async Task<Response<string>> ApprovalSKU(NewProductV2 newProductV2, CancellationToken token = default)
        {
            try
            {
                var _result = await HttpHelper.PostAsync<NewProductV2,Response<string>>($"{ AppConstants.SKU_APPROVE}",newProductV2, AppConstants.ACCESS_TOKEN, token);
                return _result;

            }
            catch (Exception _ex)
            {
                _logger.LogError("", _ex);
            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }
    }
}
