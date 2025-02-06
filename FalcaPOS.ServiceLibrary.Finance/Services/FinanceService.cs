
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Finance;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;



namespace FalcaPOS.ServiceLibrary.Finance.Services
{
    public class FinanceService : IFinanceService
    {
        private readonly Logger _logger;

        public FinanceService(Logger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<Response<FinanceSalesResult>> GetFinanceInvoices(FinanceSearch search = null, CancellationToken token = default)
        {
            try
            {


                if (search == null)
                    search = new FinanceSearch();

                var _result = await HttpHelper.PostAsync<FinanceSearch, Response<FinanceSalesResult>>(AppConstants.POST_FINANCE_SALES, search, AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (OperationCanceledException _ex)
            {
                _logger.LogError("An error in get", _ex);
                //task was cancelled by user .
                return new Response<FinanceSalesResult>
                {
                    IsSuccess = false
                };
            }
            catch (Exception _ex)
            {
                _logger.LogError("An error in get finance sales service ", _ex);
            }

            return new Response<FinanceSalesResult>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }

        public async Task<Response<IEnumerable<TallyExportModel>>> GetTallyExport(TallyExportSearchModel model, CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.PostAsync<TallyExportSearchModel, Response<IEnumerable<TallyExportModel>>>(AppConstants.GET_TALLY_EXPORT, model, AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (OperationCanceledException _ex)
            {
                //task was cancelled by user .
                return new Response<IEnumerable<TallyExportModel>>
                {
                    IsSuccess = false
                };
            }
            catch (Exception _ex)
            {
                _logger.LogError("An error in get finance sales service ", _ex);
            }

            return new Response<IEnumerable<TallyExportModel>>
            {
                IsSuccess = false,
                Error = "An error occurred, try again"
            };
        }
    }
}
