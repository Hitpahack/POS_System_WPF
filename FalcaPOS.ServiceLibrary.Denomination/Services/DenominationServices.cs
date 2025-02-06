using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.Denomination;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.ServiceLibrary.Denomination.Services
{
    public class DenominationServices : IDenominationService
    {

        private readonly INotificationService _notificationService;

        private readonly Logger _logger;

        public DenominationServices(INotificationService notificationService, Logger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }


        public async Task<Response<string>> AddDenomination(AddDenominationModel denomination, CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.PostAsync<AddDenominationModel, Response<string>>(AppConstants.ADD_DENOMINATION, denomination, AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (Exception ex)
            {

                _notificationService.ShowMessage(ex.Message, NotificationType.Error);
            }
            return new Response<string>
            {
                IsSuccess = false,
                Error = "added faild"
            };

        }

        public async Task<Response<DenominationModel>> GetTodaySales(bool IsPreDenoSuccess, string date = null, CancellationToken token = default)
        {
            try
            {
                var _result = await HttpHelper.GetAsync<Response<DenominationModel>>($"{AppConstants.GET_SALES_AVAILABLE_BALANCE}/{IsPreDenoSuccess}/{date}", AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }
            return new Response<DenominationModel>
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

        public async Task<Response<AddDenominationModel>> GetDenomination(DenominationSearchModel model, CancellationToken token = default)
        {
            try
            {
                var _result = await HttpHelper.PostAsync<DenominationSearchModel, Response<AddDenominationModel>>(AppConstants.GET_DENOMINATION, model, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }
            return new Response<AddDenominationModel>
            {
                IsSuccess = false,
                Error = "No records found."
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

        public async Task<Response<PDFFileResult>> DownloadFile(string filenameId, CancellationToken token = default)
        {

            try
            {

                if (filenameId == null) return new Response<PDFFileResult>
                {
                    IsSuccess = false,
                    Error = "Invalid file id "
                };

                //var _result = await HttpHelper.GetAsyncDenomination<Response<PDFFileResult>>(AppConstants.DOWNLOAD_FILE_DDENOMIANTION, filenameId, AppConstants.ACCESS_TOKEN, token);

                //return _result;

            }
            catch (Exception _ex)
            {
                _logger.LogError($"Error in getting file id {filenameId}", _ex);
            }

            return new Response<PDFFileResult>
            {
                IsSuccess = false,
                Error = "An error occurred while getting the file, try again"
            };
        }

        public async Task<Response<string>> AddCashDeposit(DepositModel model, FileUploadInfo[] fileUploads, CancellationToken token = default)
        {
            try
            {


                using (var _formContent = new MultipartFormDataContent())
                {
                    var _serializedConetent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");


                    _formContent.Add(new StringContent(model.DespositDate.ToString()), "DespositDate");
                    _formContent.Add(new StringContent(model.DepostAmount.ToString()), "DepostAmount");
                    _formContent.Add(new StringContent(model.BankId.ToString()), "BankId");
                    if (fileUploads != null && fileUploads.Length > 0)
                    {
                        MapUploadFilesToForm(fileUploads, _formContent);
                    }

                    var _result = await HttpHelper.PostFormDataAsync<Response<string>>(AppConstants.ADD_CASE_DEPOSIT, AppConstants.ACCESS_TOKEN, _formContent);

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

        }

        public async Task<Response<IEnumerable<DepositBanksModel>>> GetDepositBankList(CancellationToken token = default)
        {

            try
            {

                var _result = await HttpHelper.GetAsync<Response<IEnumerable<DepositBanksModel>>>(AppConstants.GET_DEPOSIT_BANKS, AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }

            return new Response<IEnumerable<DepositBanksModel>>
            {
                IsSuccess = false,
                Error = "An error occurred while getting the file, try again"
            };
        }

        public async Task<Response<List<DepositViewModelResponse>>> GetCashDepositSearch(string FromDate, string ToDate, int StoreId, CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.PostAsync<object, Response<List<DepositViewModelResponse>>>(AppConstants.GET_CASE_DEPOSIT_LIST, new { FromDate, ToDate, StoreId }, AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }

            return new Response<List<DepositViewModelResponse>>
            {
                IsSuccess = false,
                Error = "An error occurred while getting the file, try again"
            };
        }

        public async Task<Response<PDFFileResult>> DownloadFileDepsoit(int fileId, CancellationToken token = default)
        {
            try
            {

                if (fileId <= 0) return new Response<PDFFileResult>
                {
                    IsSuccess = false,
                    Error = "Invalid file id "
                };

                var _result = await HttpHelper.GetAsync<Response<PDFFileResult>>($"{AppConstants.GET_DEPSIT_FILE}/{fileId}", AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (Exception _ex)
            {
                _logger.LogError($"Error in getting file id {fileId}", _ex);
            }

            return new Response<PDFFileResult>
            {
                IsSuccess = false,
                Error = "An error occurred while getting the file, try again"
            };
        }

        public async Task<Response<string>> UpdateCaseDepositApproval(int DepositId, bool IsApprove, CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.PostAsync<object, Response<string>>($"{AppConstants.UPDATE_CASE_DEPOSIT_APPROVAL}/{DepositId}", IsApprove, AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "An error occurred while getting the file, try again"
            };
        }

        public async Task<Response<DenominationVerifyModel>> GetDenominationVerify(CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.GetAsync<Response<DenominationVerifyModel>>(AppConstants.GET_DENOMINATION_VERIFY, AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }

            return new Response<DenominationVerifyModel>
            {
                IsSuccess = false,
                Error = "An error occurred while getting the file, try again"
            };
        }

        public async Task<Response<IEnumerable<AddDenominationModel>>> GetDenominationSearch(int StoreId, string FromDate, string ToDate, CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.GetAsync<Response<IEnumerable<AddDenominationModel>>>($"{AppConstants.GET_DENOMINATION_SERACH}/{StoreId}/{FromDate}/{ToDate}", AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }

            return new Response<IEnumerable<AddDenominationModel>>
            {
                IsSuccess = false,
                Error = "An error occurred while getting the file, try again"
            };
        }
    }



}
