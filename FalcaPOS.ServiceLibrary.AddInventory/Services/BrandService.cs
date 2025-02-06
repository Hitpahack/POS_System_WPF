using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Manufacturers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.ServiceLibrary.AddInventory.Services
{
    public class BrandService : IBrandService
    {
        private readonly Logger _logger;

        public BrandService(Logger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Response<string>> EnableDisableBrand(int brandId, bool isenable, CancellationToken token = default)
        {
            if (brandId <= 0)
            {
                return new Response<string>
                {
                    IsSuccess = false,
                    Error = "Invalid manufacturer id"
                };
            }

            try
            {

                var _result = await HttpHelper
                     .PutAsync<Response<string>>($"{AppConstants.MANUFACTURERS_ENABLE_DISABLE}/{brandId}/{isenable}", AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in enable disable brand", _ex);
            }
            return new Response<string> { IsSuccess = false, Error = "An error occurred, try again" };


        }

        public async Task<IEnumerable<Manufacturer>> GetAllBrands(CancellationToken token = default)
        {
            try
            {
                var _result = await HttpHelper.GetAsync<IEnumerable<Manufacturer>>(AppConstants.MANUFACTURERS_GETALL, AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception)
            {
                //log error
            }

            return Enumerable.Empty<Manufacturer>();
        }
    }
}
