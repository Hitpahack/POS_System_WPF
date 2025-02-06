using FalcaPOS.Common.Helper;
using FalcaPOS.Entites.AddInventory;
using FalcaPOS.Entites.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{
    public interface IPurchaseReturnService
    {
        Task<Response<IEnumerable<PurchaseReturnProductViewModel>>> GetStoreReturnsSearch(int SupplierId, string ProductSKU, string LotNumber, CancellationToken token = default);

        Task<Response<string>> UpdateStorePurchaseReturns(StoreReturnModels updateModel, CancellationToken token = default);

        Task<Response<IEnumerable<StoreReturnModel>>> GetStorePurchaseReturnsListView(string status, int SupplierId,string FromDate,string ToDate, CancellationToken token = default);

        Task<Response<string>> UpdateStoreReturnWithAttachment(StoreReturnModel updateModel, FileUploadInfo[] fileUploads, CancellationToken token = default);

        Task<Response<string>> EditReturnProduct(EditStoreReturnModel updateModel, CancellationToken token = default);





    }
}
