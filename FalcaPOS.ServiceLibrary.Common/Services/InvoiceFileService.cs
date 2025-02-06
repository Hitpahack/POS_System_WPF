using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.ServiceLibrary.Common.Services
{
    public class InvoiceFileService : IInvoiceFileService
    {
        private readonly Logger _logger;

        public InvoiceFileService(Logger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Response<string>> DeleteFileById(int invoiceId, int fileId)
        {
            try
            {
                var _result = await HttpHelper.DeleteAsync<Response<string>>($"{AppConstants.DELETE_FILE_INVOICE}/{invoiceId}/{fileId}", AppConstants.ACCESS_TOKEN);

                return _result;

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in deleting file", _ex);
            }

            return default;

        }

        public async Task<Response<PDFFileResult>> DownloadFile(int fileId, CancellationToken token = default)
        {

            try
            {

                if (fileId <= 0) return new Response<PDFFileResult>
                {
                    IsSuccess = false,
                    Error = "Invalid file id "
                };

                var _result = await HttpHelper.GetAsync<Response<PDFFileResult>>($"{AppConstants.DOWNLOAD_FILE_INVOICE}/{fileId}", AppConstants.ACCESS_TOKEN, token);

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

        public async Task<Response<string>> UploadInvoiceFiles(int invoiceId, FileUploadInfo[] fileUploads)
        {
            try
            {

                if (invoiceId <= 0) return new Response<string>
                {
                    IsSuccess = false,
                    Error = "Invalid Invoice Id"
                };

                using (var _formContent = new MultipartFormDataContent())
                {

                    _formContent.Add(new StringContent(invoiceId.ToString()), "InvoiceNumber");

                    MapUploadFilesToForm(fileUploads, _formContent);

                    var _result = await HttpHelper.PostFormDataAsync<Response<string>>(AppConstants.INVOICE_UPLOAD_FILES, AppConstants.ACCESS_TOKEN, _formContent);

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

        public async Task<Response<IEnumerable<PDFFileResult>>> DownloadFileList(List<int> fileId, CancellationToken token = default)
        {

            try
            {

                if (fileId.Count == 0) return new Response<IEnumerable<PDFFileResult>>
                {
                    IsSuccess = false,
                    Error = "Invalid file id "
                };

                var _result = await HttpHelper.PostAsync<List<int>, Response<IEnumerable<PDFFileResult>>>(AppConstants.DOWNLOAD_FILE_INVOICE_LIST, fileId, AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (Exception _ex)
            {
                _logger.LogError($"Error in getting file id {fileId}", _ex);
            }

            return new Response<IEnumerable<PDFFileResult>>
            {
                IsSuccess = false,
                Error = "An error occurred while getting the file, try again"
            };
        }

        public async Task<Response<PDFFileResult>> DownloadFileMSME(int fileId, CancellationToken token = default)
        {
            try
            {

                if (fileId <= 0) return new Response<PDFFileResult>
                {
                    IsSuccess = false,
                    Error = "Invalid file id "
                };

                var _result = await HttpHelper.GetAsync<Response<PDFFileResult>>($"{AppConstants.DOWNLOAD_FILE_MSME}/{fileId}", AppConstants.ACCESS_TOKEN, token);

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
    }
}
