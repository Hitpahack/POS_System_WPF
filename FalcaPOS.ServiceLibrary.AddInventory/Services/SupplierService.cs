using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.Suppliers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.ServiceLibrary.AddInventory.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly INotificationService _notificationService;

        private readonly Logger _logger;



        public SupplierService(INotificationService notificationService, Logger logger)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<SuppliersViewModel>> GetSuppliers(string queryString = null)
        {
            try
            {

                string _url = AppConstants.SUPPLIERS_GETALL;

                if (!string.IsNullOrEmpty(queryString))
                {
                    _url = string.Format("{0}?{1}", _url, queryString);
                }

                var _result = await HttpHelper.GetAsync<IEnumerable<SuppliersViewModel>>(_url, AppConstants.ACCESS_TOKEN);

                return _result;
            }
            catch (Exception)
            {

            }
            return null;
        }

        public async Task<Response<string>> AddSuppliers(CreateSupplierModel supplier)
        {
            try
            {

                var _result = await HttpHelper.PostAsync<CreateSupplierModel, Response<string>>(AppConstants.SUPPLIERS_CREATE, supplier, AppConstants.ACCESS_TOKEN);

                if (_result != null && _result.IsSuccess)
                {
                    return _result;
                }
                else
                {
                    if (_result != null && !_result.IsSuccess && _result.Error.IsValidString())
                    {
                        _notificationService.ShowMessage(_result.Error, Common.NotificationType.Error);
                    }

                    return _result;
                }
            }
            catch (Exception)
            {

            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "An error occurred , try gain"
            };
        }

        public async Task<Response<string>> EnableDisableSupplier(int id, bool isEnable, CancellationToken token = default(CancellationToken))
        {
            try
            {

                if (id <= 0) return new Response<string> { IsSuccess = false, Error = "Invalid supplier Id" };

                var _result = await HttpHelper
                    .PutAsync<Response<string>>($"{AppConstants.SUPPLIER_ENABLE_DISABLE}/{id}/{isEnable}", AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in enable disable supplier ", _ex);
            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "An error occurred , try gain"
            };
        }

        public async Task<Response<SuppliersDetailsViewModel>> GetSupplierDetails(int supplierId, CancellationToken token = default)
        {
            if (supplierId <= 0) return new Response<SuppliersDetailsViewModel>
            {
                IsSuccess = false,
                Error = "Invalid Supplier Id"
            };
            try
            {

                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper
                    .GetAsync<Response<SuppliersDetailsViewModel>>($"{AppConstants.SUPPLIERS_DETAILS}/{supplierId}", AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (OperationCanceledException _ex)
            {
                throw;
            }
            catch (Exception _ex)
            {

                _logger.LogError("Error in get supplier details ", _ex);

                throw;

            }

            //return new Response<Supplier> { IsSuccess = false, Error = "An error occred ,try again" };

        }

        public async Task<Response<string>> UpadateSupplier(int supplierId, CreateSupplierModel supplier)
        {
            if (supplierId <= 0)
            {
                return new Response<string>
                {
                    IsSuccess = false,
                    Error = "Invalid Supplier Id"
                };
            }

            try
            {

                var _result = await HttpHelper
                    .PutAsync<CreateSupplierModel, Response<string>>($"{AppConstants.SUPPLIERS_DETAILS_UPDATE}/{supplierId}", supplier, AppConstants.ACCESS_TOKEN);


                return _result;

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in updating the supplier", _ex);

                throw;
            }

        }


        public async Task<bool> AddShippingAddress(ShippingAddress supplier)
        {
            try
            {

                var _result = await HttpHelper.PostAsync<ShippingAddress, Response<string>>(AppConstants.ADD_SHIPPING_ADDRESS, supplier, AppConstants.ACCESS_TOKEN);

                if (_result != null && _result.IsSuccess)
                {
                    return true;
                }
                else
                {
                    if (_result != null && !_result.IsSuccess && _result.Error.IsValidString())
                    {
                        _notificationService.ShowMessage(_result.Error, Common.NotificationType.Error);
                    }

                    return false;
                }
            }
            catch (Exception)
            {

            }
            return false;
        }

        public async Task<Response<string>> UpdateSupplierDetails(SupplierEditViewModel supplier)
        {
            try
            {


                using (var _formContent = new MultipartFormDataContent())
                {
                    var _serializedConetent = new StringContent(JsonConvert.SerializeObject(supplier), Encoding.UTF8, "application/json");

                    _formContent.Add(_serializedConetent, "SupplierDetails");



                    if (supplier.ListOfBankDetails != null && supplier.ListOfBankDetails.Count > 0)
                    {
                        foreach (var item in supplier.ListOfBankDetails)
                        {

                            MapUploadFilesToFormBank(item.FileUploadListInfo?.ToArray(), item.BankDetailsId, _formContent);
                        }


                    }
                    if (!string.IsNullOrEmpty(supplier.MSME) && supplier.FileUploadMSME.FileSrc==FileSrc.local)
                    {
                        if (!File.Exists(supplier.FileUploadMSME.FilePath))
                            throw new PosException($"File {supplier.FileUploadMSME.FileName} is not available , try again");
                        var extension = Path.GetExtension(supplier.FileUploadMSME.FileName);
                        _formContent.Add(new StreamContent(new FileStream(supplier.FileUploadMSME.FilePath, FileMode.Open)), $"FileUploadMSME", (supplier.FileUploadMSME.FileName) + extension);

                    }

                    var _result = await HttpHelper.PostFormDataWithAttachmentAsync<Response<string>>(AppConstants.UPDATE_SUPPLIER_DETAILS, AppConstants.ACCESS_TOKEN, _formContent);



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
            catch (Exception _ex)
            {
                return new Response<string>
                {
                    IsSuccess = false,
                    Error = _ex.Message
                };
            }


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

        private void MapUploadFilesToFormBank(FileUploadInfo[] fileUploads, int BankDetailsId, MultipartFormDataContent formContent)
        {
            for (int i = 0; i < fileUploads.Length; i++)
            {

                if (!File.Exists(fileUploads[i].FilePath))
                    throw new PosException($"File {fileUploads[i].FileName} is not available , try again");
                var extension = Path.GetExtension(fileUploads[i].FileName);
                formContent.Add(new StreamContent(new FileStream(fileUploads[i].FilePath, FileMode.Open)), $"Files", Convert.ToString(BankDetailsId) + extension);
            }
        }

        public async Task<Response<IEnumerable<AddressViewModel>>> GetShippingAddress(int supplierId)
        {
            if (supplierId <= 0)
            {
                return new Response<IEnumerable<AddressViewModel>>
                {
                    IsSuccess = false,
                    Error = "Invalid Supplier Id"
                };
            }

            try
            {

                var _result = await HttpHelper
                    .PostAsync<Supplier, Response<IEnumerable<AddressViewModel>>>($"{AppConstants.GET_SHIPPING_ADDRESS}/{supplierId}", null, AppConstants.ACCESS_TOKEN);


                return _result;

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in updating the supplier", _ex);

                throw;
            }

        }

        public async Task<Response<IEnumerable<Bank>>> GetBankList()
        {

            try
            {

                var _result = await HttpHelper
                    .GetAsync<Response<IEnumerable<Bank>>>($"{AppConstants.GET_BANK_LIST}", AppConstants.ACCESS_TOKEN);


                return _result;

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in updating the supplier", _ex);

                throw;
            }

        }

        public async Task<Response<string>> AddNewBank(string BankName)
        {

            try
            {

                var _result = await HttpHelper
                    .PostAsync<string, Response<string>>($"{AppConstants.ADD_NEW_BANK}", BankName, AppConstants.ACCESS_TOKEN);


                return _result;

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in updating the supplier", _ex);

                throw;
            }

        }


        public async Task<Response<string>> AddNewBankDetails(int SupplierId, int BankId, string BrnachName, string AccountNo, string AccountType, string IFSC, FileUploadInfo[] fileUploads)
        {

            try
            {


                using (var _formContent = new MultipartFormDataContent())
                {

                    _formContent.Add(new StringContent(BrnachName.ToString()), "BrnachName");
                    _formContent.Add(new StringContent(AccountNo.ToString()), "AccountNo");
                    _formContent.Add(new StringContent(AccountType.ToString()), "AccountType");
                    _formContent.Add(new StringContent(IFSC.ToString()), "IFSC");


                    if (fileUploads != null && fileUploads.Length > 0)
                    {
                        MapUploadFilesToForm(fileUploads, _formContent);
                    }

                    var _result = await HttpHelper.PostFormDataWithAttachmentAsync<Response<string>>($"{AppConstants.ADD_NEW_BANK_DETAILS}/{SupplierId}/{BankId}", AppConstants.ACCESS_TOKEN, _formContent);

                    return _result;

                }
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in updating the supplier", _ex);

                throw;
            }

        }

        public async Task<Response<SupplierTDSStatus>> GetSupplierTDSDetails(int supplierId, CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper
                  .GetAsync<Response<SupplierTDSStatus>>($"{AppConstants.GET_SUPPLIER_TDS_DETAILS}/{supplierId}", AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in get supplier TDS details ", _ex);

                throw;
            }
        }
    }
}
