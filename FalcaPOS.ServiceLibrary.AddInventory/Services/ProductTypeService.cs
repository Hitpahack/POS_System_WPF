using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Manufacturers;
using FalcaPOS.Entites.ProductTypes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.ServiceLibrary.AddInventory.Services
{
    public class ProductTypeService : IProductTypeService
    {
        private readonly Logger _logger;

        private readonly INotificationService _notificationService;

        public ProductTypeService(INotificationService notificationService, Logger logger)
        {

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        //public async Task<ProductType> CreateProductType(CreateProductTypeModel productType)
        //{
        //    try
        //    {
        //        var _result = await HttpHelper.PostAsync<CreateProductTypeModel, Response<ProductType>>(AppConstants.PRODUCT_TYPES_CREATE, productType, AppConstants.ACCESS_TOKEN);


        //        if (_result != null && _result.IsSuccess)
        //        {
        //            return _result.Data;
        //        }
        //        else if (_result != null && !_result.IsSuccess)
        //        {
        //            _notificationService.ShowMessage(_result.Error, NotificationType.Error);
        //        }

        //    }
        //    catch (Exception _ex)
        //    {
        //        _notificationService.ShowMessage(_ex?.Message, NotificationType.Error);
        //    }

        //    return default(ProductType);

        //}

        [Obsolete("Use CreateProductTypeManufacturerAsync instead")]
        public async Task<Response<string>> CreateProductTypeManufacturer(ProductTypeManufacturer productTypeManufacturer)
        {
            try
            {
                var _result = await HttpHelper.PostAsync<ProductTypeManufacturer, Response<string>>(AppConstants.PRODUCT_TYPE_MANUFACTURER_CREATE, productTypeManufacturer, AppConstants.ACCESS_TOKEN);

                if (_result != null && !_result.IsSuccess)
                {
                    _notificationService.ShowMessage(_result.Error, NotificationType.Error);
                }

                return _result;
            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);

                return new Response<string>
                {
                    IsSuccess = false
                };
            }


        }

        public async Task<Response<string>> CreateProductTypeManufacturerAsync(CreateManufactureModel manufacturer, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.PostAsync<CreateManufactureModel, Response<string>>(AppConstants.CREATE_PRODUCT_TYPE_MANUFACTURER, manufacturer, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in creating product type manufacturer", _ex);
            }
            return new Response<string>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };

        }

        public async Task<Response<string>> EnableDisbaleProductType(int typeId, bool isenable, CancellationToken token = default)
        {
            try
            {
                var _result = await HttpHelper
                    .PutAsync<Response<string>>($"{AppConstants.PRODUCT_TYPE_ENABLE_DISABLE}/{typeId}/{isenable}", AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                throw;
            }

        }


        public async Task<Response<IEnumerable<Manufacturer>>> GetAllEnabledManufacturers(CancellationToken token = default)
        {

            try
            {

                var _result = await HttpHelper.GetAsync<Response<IEnumerable<Manufacturer>>>(AppConstants.MANUFACTURER_ENABLED_LIST, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in getting enabled manufacturer list", _ex);
            }

            return new Response<IEnumerable<Manufacturer>>
            {
                IsSuccess = false
            };

        }

        //public async Task<Response<string>> GetCurrentDeptCode()
        //{
        //    try
        //    {
        //        var _result = await HttpHelper.GetAsync<Response<string>>(AppConstants.PRODUCT_TYPE_DEPT_CODE, AppConstants.ACCESS_TOKEN);

        //        return _result;
        //    }
        //    catch (Exception _ex)
        //    {
        //    }
        //    return new Response<string>
        //    {
        //        IsSuccess = false,
        //        Error = "An error occurred ,try again"
        //    };

        //}

        public async Task<IEnumerable<Manufacturer>> GetProductTypeManufacturers(long productTypeId, CancellationToken token = default(CancellationToken))
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.GetAsync<IEnumerable<Manufacturer>>($"{AppConstants.PRODUCT_TYPE_MANUFACTURERS}/{productTypeId}", AppConstants.ACCESS_TOKEN, token);

                token.ThrowIfCancellationRequested();

                return _result;
            }
            catch (Exception _ex)
            {


            }
            return null;

        }

        public async Task<IEnumerable<Entites.ProductTypes.ProductType>> GetProductTypes(string query = null)
        {
            try
            {
                string _url = AppConstants.PRODUCT_TYPES_GETALL;

                if (!string.IsNullOrEmpty(query))
                {
                    _url = string.Format("{0}?{1}", _url, query);
                }

                var _result = await HttpHelper.GetAsync<IEnumerable<Entites.ProductTypes.ProductType>>(_url, AppConstants.ACCESS_TOKEN);

                return _result;
            }
            catch (Exception)
            {

            }

            return null;
        }

        public async Task<Response<IEnumerable<CategoryModel>>> GetAllCategory(CancellationToken token = default)
        {
            try
            {

                string _url = AppConstants.GET_ALL_CATEGORY;


                var _result = await HttpHelper.GetAsync<Response<IEnumerable<CategoryModel>>>(_url, AppConstants.ACCESS_TOKEN);

                return _result;
            }
            catch (Exception _ex)
            {

                _logger.LogError(_ex.Message);

                return null;
            }
        }

        public async Task<Response<IEnumerable<SubCategoryModel>>> GetSubCategory(int CategoryId, CancellationToken token = default)
        {
            try
            {


                var _result = await HttpHelper.GetAsync<Response<IEnumerable<SubCategoryModel>>>($"{AppConstants.GET_SUB_CATEGORY}/{CategoryId}", AppConstants.ACCESS_TOKEN);

                return _result;

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);

                return null;
            }
        }


        public async Task<Response<string>> AddCategory(string CategoryName,bool IsCertificate, CancellationToken token = default)
        {
            try
            {


                var _result = await HttpHelper.PostAsync<string, Response<string>>($"{AppConstants.ADD_CATEGORY}/{IsCertificate}", CategoryName, AppConstants.ACCESS_TOKEN);

                return _result;

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);

                return null;
            }
        }

        public async Task<Response<ProductType>> AddSubCategory(int CategoryId, string CategoryName, CancellationToken token = default)
        {
            try
            {


                var _result = await HttpHelper.PostAsync<string, Response<ProductType>>($"{AppConstants.ADD_SUB_CATEGORY}/{CategoryId}", CategoryName, AppConstants.ACCESS_TOKEN);

                return _result;

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);

                return null;
            }
        }

        public async Task<Response<IEnumerable<CategoryModel>>> GetAllLicenseCategory(CancellationToken token = default)
        {
            try
            {

                string _url = AppConstants.GET_ALL_LICENSE_CATEGORY;


                var _result = await HttpHelper.GetAsync<Response<IEnumerable<CategoryModel>>>(_url, AppConstants.ACCESS_TOKEN);

                return _result;
            }
            catch (Exception _ex)
            {

                _logger.LogError(_ex.Message);

                return null;
            }
        }

    }
}
