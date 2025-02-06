using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.AddInventory;
using FalcaPOS.Entites.Sales;
using FalcaPOS.Entites.Stock;
using FalcaPOS.Entites.Stores;
using FalcaPOS.Entites.Zone;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.ServiceLibrary.AddInventory.Services
{
    public class StockServices : IStockService
    {
        private readonly Logger _logger;

        public StockServices(Logger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Response<string>> AddStockProduct(AddStockProductModel stockProduct, CancellationToken cancellationToken = default(CancellationToken))
        {


            try
            {
                if (stockProduct == null)
                {
                    return new Response<string>
                    {
                        IsSuccess = false,
                        Error = "Invalid invoice details "
                    };
                }

                cancellationToken.ThrowIfCancellationRequested();

                var _result = await HttpHelper.PostAsync<AddStockProductModel, Response<string>>(AppConstants.ADD_STOCK_PRODUCT, stockProduct, AppConstants.ACCESS_TOKEN, cancellationToken);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in add stock product ", _ex);

                return new Response<string>
                {
                    IsSuccess = false,
                    Error = "An error occurred,try again"
                };
            }


        }

        public async Task<DataSet> GetBackendStockSearch(StockModelRequest stockModelRequest)
        {
            try
            {

                var _result = await HttpHelper.PostAsyncDataSet<StockModelRequest, DataSet>(AppConstants.GET_BACKEND_SEARCH, stockModelRequest, AppConstants.ACCESS_TOKEN);

                return _result;
            }
            catch (Exception _ex)
            {

            }
            return null;
        }
      
        public async Task<Response<IEnumerable<UserInvoiceListViewModel>>> GetBackendUserInvoices(CancellationToken token = default)
        {
            try
            {
                _logger.LogInformation($"Getting backend user {AppConstants.UserName} invoices ");

                var _result = await HttpHelper.GetAsync<Response<IEnumerable<UserInvoiceListViewModel>>>(AppConstants.GET_BACKEND_INVOICE_LIST, AppConstants.ACCESS_TOKEN, token);

                if (_result != null && _result.Data != null)
                {

                    foreach (var _invoice in _result.Data)
                    {
                        if (_invoice.DcNumber.IsValidString() && _invoice.InvoiceNumber.IsValidString())
                        {
                            _invoice.IsDcNumber = _invoice.DcNumber.Trim().ToLower() == _invoice.InvoiceNumber.Trim().ToLower();
                        }

                    }

                }


                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError($"Error in getting backend user invoice list -m {nameof(GetBackendUserInvoices)}", _ex);
            }
            return new Response<IEnumerable<UserInvoiceListViewModel>>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<SalesProduct>> GetProductInformation(int productId, CancellationToken token = default(CancellationToken))
        {

            try
            {
                token.ThrowIfCancellationRequested();
                var _result = await HttpHelper
                    .GetAsync<Response<SalesProduct>>($"{AppConstants.GET_STOCK_PRODUCT_INFORMATION}/{productId}", AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (TaskCanceledException _ex)
            {
                //user cancelled operations.

            }
            catch (Exception _ex)
            {
                _logger.LogError($"Error in {nameof(GetProductInformation)}", _ex);

                //log error
                return new Response<SalesProduct>
                {
                    IsSuccess = false,
                    Error = _ex.Message
                };
            }
            return new Response<SalesProduct>
            {
                IsSuccess = false,
            };

        }

        public async Task<Response<InvoiceDetailsViewModel>> GetSlotWiseInvoiceProductDetails(string invoiceNo, int supplierId, CancellationToken token = default)
        {
            try
            {
                if (!invoiceNo.IsValidString())
                {
                    return new Response<InvoiceDetailsViewModel>
                    {
                        IsSuccess = false,
                        Error = "Invalid invoice Id"
                    };
                }

                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper
                    .PostAsync
                    <object, Response<InvoiceDetailsViewModel>>
                    (AppConstants.GET_SALES_SLOTWISEINVOICEPRODUCTDETAILS, new { invoiceNo, supplierId }, AppConstants.ACCESS_TOKEN, token);


                return _result;

            }


            catch (TaskCanceledException _ex)
            {
                //user cancelled operations.

            }
            catch (Exception _ex)
            {
                //log error
                return new Response<InvoiceDetailsViewModel>
                {
                    IsSuccess = false,
                    Error = _ex.Message
                };
            }
            return new Response<InvoiceDetailsViewModel>
            {
                IsSuccess = false,
            };

        }

        public async Task<Response<string>> UpdateInvoiceNumber(UpdateDCNumberViewModel invoiceInfo)
        {
            try
            {
                var _result = await HttpHelper
                    .PostAsync<UpdateDCNumberViewModel, Response<string>>
                    (AppConstants.UPDATE_BACKEND_INVOICE_NUMBER, invoiceInfo, AppConstants.ACCESS_TOKEN);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in update dc number invoice", _ex);
            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "An error occurred,try again"
            };
        }

        public async Task<Response<string>> UpdateSellingPrice(int StockProductId, float SellingPrice, CancellationToken token = default)
        {
            try
            {
                var _result = await HttpHelper
                    .GetAsync<Response<string>>
                    ($"{AppConstants.UPDATE_SELLING_PRICE}/{StockProductId}/{SellingPrice}", AppConstants.ACCESS_TOKEN);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in update dc number invoice", _ex);
            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "An error occurred,try again"
            };
        }

        public async Task<Response<IEnumerable<SellingPriceResponse>>> UpdateSellingPriceV2(SellingPriceUpdateViewModelV2 sellingPriceUpdateViewModel, CancellationToken token = default)
        {
            try
            {
                var _result = await HttpHelper
                    .PostAsync<SellingPriceUpdateViewModelV2, Response<IEnumerable<SellingPriceResponse>>>
                    ($"{AppConstants.UPDATE_SELLING_PRICEV2}", sellingPriceUpdateViewModel, AppConstants.ACCESS_TOKEN);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in UpdateSellingPriceV2", _ex);
            }

            return new Response<IEnumerable<SellingPriceResponse>>
            {
                IsSuccess = false,
                Error = "An error occurred,try again"
            };
        }

        public async Task<IEnumerable<StoreStockModelResponse>> GetProductStoreBarCode(int productId, int storeId, CancellationToken token = default)
        {
            try
            {
                return await HttpHelper.GetAsync<IEnumerable<StoreStockModelResponse>>($"{AppConstants.GET_STORE_BARCODELIST}/{productId}/{storeId}", AppConstants.ACCESS_TOKEN, token);
            }
            catch (Exception _ex)
            {

                _logger.LogError("Error in getting all store Barcodes", _ex);
            }

            return Enumerable.Empty<StoreStockModelResponse>();
        }
    }
}

