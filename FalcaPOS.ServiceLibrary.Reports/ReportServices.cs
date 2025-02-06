using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.ServiceLibrary.Reports {
    public class ReportServices:IReportServices {

        private readonly Logger _logger;
        public ReportServices(Logger logger) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

      

        public async Task<Response<IEnumerable<InventoryReportModelPM>>> GetInventoryReport(CancellationToken token = default) {
            try {
                token.ThrowIfCancellationRequested();


                var _result = await HttpHelper.GetAsync<Response<IEnumerable<InventoryReportModelPM>>>
                    (AppConstants.GET_INENTORY_REPORT, AppConstants.ACCESS_TOKEN, token);

                token.ThrowIfCancellationRequested();

                return _result;

            }
            catch (OperationCanceledException) { }
            catch (Exception _ex) {
                _logger.LogError("Error in get inventory report service", _ex);
            }

            return new Response<IEnumerable<InventoryReportModelPM>> {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<IEnumerable<InventoryReportPoGrnModel>>> GetPOGRNReport(DateTime FromDate, DateTime ToDate, CancellationToken token = default) {
            try {
                token.ThrowIfCancellationRequested();


                var _result = await HttpHelper.PostAsync<object,Response<IEnumerable<InventoryReportPoGrnModel>>>
                    (AppConstants.GET_POGRN_REPORT,new {FromDate,ToDate}, AppConstants.ACCESS_TOKEN, token);

                token.ThrowIfCancellationRequested();

                return _result;

            }
            catch (OperationCanceledException) { }
            catch (Exception _ex) {
                _logger.LogError("Error in get inventory report service", _ex);
            }

            return new Response<IEnumerable<InventoryReportPoGrnModel>> {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<IEnumerable<TopListItemDTO>>> GetTopListItems(DateTime FromDate,DateTime ToDate, CancellationToken token = default) {
            try {
                token.ThrowIfCancellationRequested();


                var _result = await HttpHelper.PostAsync<object,Response<IEnumerable<TopListItemDTO>>>
                    (AppConstants.GET_TOP_LIST_ITEMS,new { FromDate,ToDate}, AppConstants.ACCESS_TOKEN, token);

                token.ThrowIfCancellationRequested();

                return _result;

            }
            catch (OperationCanceledException) { }
            catch (Exception _ex) {
                _logger.LogError("Error in get top lists", _ex);
            }

            return new Response<IEnumerable<TopListItemDTO>> {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<IEnumerable<TopListBrandDTO>>> GetTopListBrand(DateTime FromDate, DateTime ToDate, CancellationToken token = default) {
            try {
                token.ThrowIfCancellationRequested();


                var _result = await HttpHelper.PostAsync<object, Response<IEnumerable<TopListBrandDTO>>>
                    (AppConstants.GET_TOP_LIST_BRAND, new { FromDate, ToDate }, AppConstants.ACCESS_TOKEN, token);

                token.ThrowIfCancellationRequested();

                return _result;

            }
            catch (OperationCanceledException) { }
            catch (Exception _ex) {
                _logger.LogError("Error in get top lists", _ex);
            }

            return new Response<IEnumerable<TopListBrandDTO>> {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<IEnumerable<TopListCategoryDTO>>> GetTopListCategories(DateTime FromDate, DateTime ToDate, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();


                var _result = await HttpHelper.PostAsync<object,Response<IEnumerable<TopListCategoryDTO>>>
                    (AppConstants.GET_TOP_LIST_CATEGORY, new {FromDate,ToDate}, AppConstants.ACCESS_TOKEN, token);

                token.ThrowIfCancellationRequested();

                return _result;

            }
            catch (OperationCanceledException) { }
            catch (Exception _ex)
            {
                _logger.LogError("Error in get top lists", _ex);
            }

            return new Response<IEnumerable<TopListCategoryDTO>>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<IEnumerable<TopListTransactionsDTO>>> GetTopListTransactions(DateTime FromDate, DateTime ToDate, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();


                var _result = await HttpHelper.PostAsync<object, Response<IEnumerable<TopListTransactionsDTO>>>
                    (AppConstants.GET_TOP_LIST_TRANSACTION, new { FromDate, ToDate }, AppConstants.ACCESS_TOKEN, token);

                token.ThrowIfCancellationRequested();

                return _result;

            }
            catch (OperationCanceledException) { }
            catch (Exception _ex)
            {
                _logger.LogError("Error in get top lists", _ex);
            }

            return new Response<IEnumerable<TopListTransactionsDTO>>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }
    }
}
