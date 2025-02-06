using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Dtos;
using FalcaPOS.Entites.Products;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace FalcaPOS.ServiceLibrary.AddInventory.Services
{
    public class ProductService : IProductService
    {
        private readonly INotificationService _notificationService;

        private readonly Logger _logger;
        public ProductService(INotificationService notificationService, Logger logger)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Response<string>> CreateProduct(ProductDetails product)
        {
            try
            {

                var _result = await HttpHelper.PostAsync<ProductDetails, Response<string>>
                    (AppConstants.PRODUCT_CREATE, product, AppConstants.ACCESS_TOKEN);

                return _result;

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in create product", _ex);
                _notificationService.ShowMessage(_ex.Message, Common.NotificationType.Error);
            }

            return new Response<string> { IsSuccess = false };

        }

        public async Task<Response<string>> EnabelDiableProduct(int productId, bool isenable, CancellationToken token = default)
        {
            if (productId <= 0)
            {
                return new Response<string>
                {
                    IsSuccess = false,
                    Error = "Invalid product id"
                };
            }

            try
            {
                var _result = await HttpHelper
                    .PutAsync<Response<string>>($"{AppConstants.PRODUCT_ENABLE_DISABLE}/{productId}/{isenable}", AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (OperationCanceledException _ex) { }
            catch (Exception _ex)
            {
                _logger.LogError("Error in enable disable ", _ex);
            }
            return new Response<string>
            {
                IsSuccess = false,
                Error = "An error occred, try again"
            };

        }

        public async Task<ProductDetails> GetProduct(int id)
        {
            try
            {
                await HttpHelper.GetAsync<ProductDetails>($"{AppConstants.PRODUCT_GET_ALL}/{id}", AppConstants.ACCESS_TOKEN);
            }
            catch (Exception _Ex)
            {

            }

            return null;

        }

        public async Task<IEnumerable<ProductViewModel>> GetProducts(string query, CancellationToken token = default(CancellationToken))
        {
            try
            {
                token.ThrowIfCancellationRequested();

                string _url = AppConstants.PRODUCT_GET_ALL;
                if (!string.IsNullOrEmpty(query))
                {
                    _url = string.Format("{0}?{1}", _url, query);
                }


                var _result = await HttpHelper.GetAsync<IEnumerable<ProductViewModel>>(_url, AppConstants.ACCESS_TOKEN, token);

                return _result;


            }
            catch (Exception _ex)
            {

            }

            return default(IEnumerable<ProductViewModel>);

        }

        public async Task<Response<string>> ProductCurrentSKU(int productTypeId, CancellationToken token = default)
        {
            try
            {
                if (productTypeId <= 0) return new Response<string> { IsSuccess = false };

                token.ThrowIfCancellationRequested();


                var _result = await HttpHelper.GetAsync<Response<string>>
                    ($"{AppConstants.PRODUCT_CURRENT_SKU_NUMBER}/{productTypeId}", AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (OperationCanceledException) { }
            catch (Exception _ex)
            {

                _logger.LogError("Error in getting product sku last number", _ex);
            }

            return new Response<string> { IsSuccess = false };
        }

        public async Task<Response<InventaryProductViewModel>> GetSKUNumberProduct(int productId,int storeId, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();
               
                var _result = await HttpHelper.GetAsync<Response<InventaryProductViewModel>>
                    ($"{AppConstants.GET_SKU_PRODUCT}/{productId}/{storeId}", AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (OperationCanceledException _ex)
            {
                _logger.LogError("Task was cancelled in get product sku information", _ex);
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in get sku product", _ex);
            }
            return new Response<InventaryProductViewModel>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<IEnumerable<ProductSearchModel>>> SearchProducts(string name, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                if (!name.IsValidString() || name.Length < 3)
                {
                    return new Response<IEnumerable<ProductSearchModel>> { IsSuccess = false, Error = "Search should be minimum 3 charcters" };
                }

                var _name = HttpUtility.UrlEncode(name, Encoding.Unicode);

                var _result = await HttpHelper
                    .GetAsync<Response<IEnumerable<ProductSearchModel>>>($"{AppConstants.SEARCH_PRODUCT}?name={_name}", AppConstants.ACCESS_TOKEN, token);

                token.ThrowIfCancellationRequested();

                return _result;
            }
            catch (OperationCanceledException _ex)
            {
                _logger.LogInformation("Task was cancelled in product search ");

                throw;
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in product search ", _ex);

            }

            return new Response<IEnumerable<ProductSearchModel>>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };

        }

        public async Task<IEnumerable<ProductDetails>> GetenabledProducts(string query, CancellationToken token = default(CancellationToken))
        {
            try
            {
                token.ThrowIfCancellationRequested();

                string _url = AppConstants.PRODUCT_GET_ALL_ENABLED;
                if (!string.IsNullOrEmpty(query))
                {
                    _url = string.Format("{0}?{1}", _url, query);
                }


                var _result = await HttpHelper.GetAsync<IEnumerable<ProductDetails>>(_url, AppConstants.ACCESS_TOKEN, token);

                return _result;


            }
            catch (Exception _ex)
            {

            }

            return default(IEnumerable<ProductDetails>);

        }

        public async Task<Response<string>> GetStockbySKU(string sku,int? StoreId, CancellationToken token = default)
        {

            try
            {
                var _response = await HttpHelper.GetAsync<Response<string>>($"{AppConstants.GET_SKU_STOCK}/{sku}/{StoreId}", AppConstants.ACCESS_TOKEN);
                if (_response != null)
                    return _response;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);

            }

            return null;
        }

        public async Task<Response<ProductDTO>> GetSKUStockProductSearch(int productId, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.GetAsync<Response<ProductDTO>>
                    ($"{AppConstants.GET_SKU_STOCK_PRODUCT_SEARCH}/{productId}", AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (OperationCanceledException _ex)
            {
                _logger.LogError("Task was cancelled in get product sku information", _ex);
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in get sku product", _ex);
            }
            return new Response<ProductDTO>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<InventaryProductViewModel>> GetBrandCategorySubcategorySKUbyId(int productId, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.GetAsync<Response<InventaryProductViewModel>>
                    ($"{AppConstants.GET_BRANDCATEGORYSUBCATEGORYSKUBYID}/{productId}", AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (OperationCanceledException _ex)
            {
                _logger.LogError("Task was cancelled in get product sku information", _ex);
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in get sku product", _ex);
            }
            return new Response<InventaryProductViewModel>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<InventaryProductViewModel>> GetSKUSelectTransferProduct(int productId,int FromStoreId, int storeId, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.GetAsync<Response<InventaryProductViewModel>>
                    ($"{AppConstants.GET_SKU_TRANSFER_PRODUCT}/{productId}/{FromStoreId}/{storeId}", AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (OperationCanceledException _ex)
            {
                _logger.LogError("Task was cancelled in get product sku information", _ex);
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in get sku product", _ex);
            }
            return new Response<InventaryProductViewModel>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

    }
}
