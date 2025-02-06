using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.AddInventory;
using FalcaPOS.Entites.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace FalcaPOS.ServiceLibrary.PurchaseInvoice.Services
{
    public class PurchaseInvoiceService : IPurchaseInvoiceService
    {
        private readonly Logger _logger;

        public PurchaseInvoiceService(Logger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Response<IEnumerable<PurchaseInvoiceViewModel>>> GetPurchaseInvoices(InvoiceSearchParams searchParams, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                //buid querry string.

                string _urlInvoiceList = AppConstants.GET_PURCHASE_INVOICE;

                if (searchParams != null)
                {
                    Dictionary<string, string> _querry = new Dictionary<string, string>();

                    if (searchParams.FromDate != null && searchParams.FromDate.HasValue)
                    {
                        _querry.Add("FromDate", HttpUtility.UrlEncode(searchParams.FromDate.Value.ToShortDateString()));
                    }

                    if (searchParams.ToDate != null && searchParams.ToDate.HasValue)
                    {
                        _querry.Add("ToDate", HttpUtility.UrlEncode(searchParams.ToDate.Value.ToShortDateString()));
                    }
                    if (searchParams.StoreId != null)
                    {
                        _querry.Add("StoreId", searchParams.StoreId.ToString());
                    }
                    if (searchParams.IsDc)
                    {
                        _querry.Add("IsDc", searchParams.IsDc.ToString());
                    }


                    if (_querry.Any())
                    {
                        StringBuilder _stringBuilder = new StringBuilder();

                        string _search = "?";

                        foreach (var item in _querry)
                        {
                            _stringBuilder.Append(_search + item.Key + "=" + item.Value);
                            _search = "&";

                        }
                        _urlInvoiceList += _stringBuilder.ToString();
                    }
                }



                var _result = await HttpHelper.GetAsync<Response<IEnumerable<PurchaseInvoiceViewModel>>>
                    (_urlInvoiceList, AppConstants.ACCESS_TOKEN, token);

                token.ThrowIfCancellationRequested();

                return _result;

            }
            catch (OperationCanceledException) { }
            catch (Exception _ex)
            {
                _logger.LogError("Error in purchase invoice service", _ex);
            }

            return new Response<IEnumerable<PurchaseInvoiceViewModel>>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<IEnumerable<InvoiceProductViewModel>>> GetProductDetails(int InvoiceId, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                //buid querry string.

                string _urlInvoiceList = AppConstants.GET_PRODUCTDETAILS_INVOICE;



                var _result = await HttpHelper.PostAsync<int, Response<IEnumerable<InvoiceProductViewModel>>>
                    (_urlInvoiceList, InvoiceId, AppConstants.ACCESS_TOKEN, token);

                token.ThrowIfCancellationRequested();

                return _result;

            }
            catch (OperationCanceledException) { }
            catch (Exception _ex)
            {
                _logger.LogError("Error in get product details service", _ex);
            }

            return new Response<IEnumerable<InvoiceProductViewModel>>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<IEnumerable<StockProductViewModel>>> GetPurchaseReturnsSearch(string ProductSKU, string LotNumber, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();


                var _result = await HttpHelper.PostAsync<object, Response<IEnumerable<StockProductViewModel>>>
                    (AppConstants.GET_PURCHASE_RETURN_SEARCH, new { ProductSKU, LotNumber }, AppConstants.ACCESS_TOKEN, token);

                token.ThrowIfCancellationRequested();

                return _result;

            }
            catch (OperationCanceledException) { }
            catch (Exception _ex)
            {
                _logger.LogError("Error in getproduct details service", _ex);
            }

            return new Response<IEnumerable<StockProductViewModel>>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<string>> UpdatePurchaseReturns(List<StockProductViewModel> updateModel, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();


                var _result = await HttpHelper.PostAsync<List<StockProductViewModel>, Response<string>>
                    (AppConstants.UPDATE_PURCHASE_RETURN, updateModel, AppConstants.ACCESS_TOKEN, token);

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

        public async Task<Response<IEnumerable<StoreReturnModel>>> GetPurchaseReturnsListView(string status, int SupplierId, int StoreId, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();


                var _result = await HttpHelper.PostAsync<object,Response<IEnumerable<StoreReturnModel>>>
                    (AppConstants.GET_PURCHASE_RETURN_LIST,new {SupplierId,StoreId,status}, AppConstants.ACCESS_TOKEN, token);

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

        public async Task<Response<string>> ApprovePurchaseReturns(StoreReturnModel Model, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();


                var _result = await HttpHelper.PostAsync<StoreReturnModel, Response<string>>
                    (AppConstants.APPROVE_PURCHASE_RETURNS, Model, AppConstants.ACCESS_TOKEN, token);

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

        public async Task<Response<string>> PostCNApprovePurchaseReturns(int CreditnoteId, string Status, string Remark, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();


                var _result = await HttpHelper.PostAsync<object, Response<string>>
                    (AppConstants.POST_CN_APPROVE_PURCHASE_RETURNS, new { CreditnoteId, Status, Remark }, AppConstants.ACCESS_TOKEN, token);

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

        public async Task<Response<IEnumerable<SummaryViewModelCreidtNote>>> GetCreditNoteSummaryList(int SupplierId, int StoreId, string FromDate, string ToDate, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();


                var _result = await HttpHelper.PostAsync<object, Response<IEnumerable<SummaryViewModelCreidtNote>>>
                    (AppConstants.GET_CREDITNOTE_SUMMARY, new { SupplierId, StoreId, FromDate, ToDate }, AppConstants.ACCESS_TOKEN, token);

                token.ThrowIfCancellationRequested();

                return _result;

            }
            catch (OperationCanceledException) { }
            catch (Exception _ex)
            {
                _logger.LogError("Error in get product details service", _ex);
            }

            return new Response<IEnumerable<SummaryViewModelCreidtNote>>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<IEnumerable<SummaryDetailsViewModelCreditNote>>> GetCreditNoteSummaryDetails(int SupplierId, int StoreId, string FromDate, string ToDate, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();


                var _result = await HttpHelper.PostAsync<object, Response<IEnumerable<SummaryDetailsViewModelCreditNote>>>
                    ($"{AppConstants.GET_CREDITNOTE_SUMMARY_DETAILS}", new { SupplierId, StoreId, FromDate, ToDate }, AppConstants.ACCESS_TOKEN, token);

                token.ThrowIfCancellationRequested();

                return _result;

            }
            catch (OperationCanceledException) { }
            catch (Exception _ex)
            {
                _logger.LogError("Error in get product details service", _ex);
            }

            return new Response<IEnumerable<SummaryDetailsViewModelCreditNote>>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }
    }


}
