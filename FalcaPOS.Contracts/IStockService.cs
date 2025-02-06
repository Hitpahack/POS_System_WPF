using FalcaPOS.Common.Helper;
using FalcaPOS.Entites.AddInventory;
using FalcaPOS.Entites.Sales;
using FalcaPOS.Entites.Stock;
using FalcaPOS.Entites.Stores;
using System.Collections.Generic;

using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{

    public interface IStockService
    {
        Task<DataSet> GetBackendStockSearch(StockModelRequest stockModelRequest);

        Task<Response<string>> AddStockProduct(AddStockProductModel stockProduct, CancellationToken cancellationToken = default(CancellationToken));

        Task<Response<SalesProduct>> GetProductInformation(int productId, CancellationToken token = default(CancellationToken));
        Task<Response<InvoiceDetailsViewModel>> GetSlotWiseInvoiceProductDetails(string invoiceNo, int supplierId, CancellationToken token = default(CancellationToken));

        Task<Response<IEnumerable<UserInvoiceListViewModel>>> GetBackendUserInvoices(CancellationToken token = default);

        Task<Response<string>> UpdateInvoiceNumber(UpdateDCNumberViewModel invoiceInfo);

        Task<Response<string>> UpdateSellingPrice(int StockProductId, float SellingPrice, CancellationToken token = default(CancellationToken));
        Task<Response<IEnumerable<SellingPriceResponse>>> UpdateSellingPriceV2(SellingPriceUpdateViewModelV2 sellingPriceUpdateViewModel, CancellationToken token = default(CancellationToken));
        Task<IEnumerable<StoreStockModelResponse>> GetProductStoreBarCode(int productId, int storeId, CancellationToken token = default);


    }
}
