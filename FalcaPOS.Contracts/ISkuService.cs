using FalcaPOS.Common.Helper;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.Sku;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{
    public interface ISkuService
    {
        Task<Response<IEnumerable<ProductTypesViewModel>>> GetAllProductType(CancellationToken token = default(CancellationToken));

        Task<Response<string>> VerifyProductCount(List<ProductTypesViewModel> typesViewModel, CancellationToken token = default(CancellationToken));
        Task<Response<IEnumerable<ProductTypesViewModel>>> GetDailyStockReportServices(DailyStockSearch dailySearch, CancellationToken token = default(CancellationToken));
        Task<Response<IEnumerable<SkuSheetProductTypeViewModel>>> GetAllSku(CancellationToken token = default(CancellationToken));
        Task<Response<IEnumerable<NewProductV2>>> GetAllSkuV2(CancellationToken token = default(CancellationToken));

        Task<Response<string>> CreateSKURequest(List<CreateProductModel> createProducts, CancellationToken token = default(CancellationToken));

        Task<Response<string>> UploadFiles(List<string> SKUReuestid, FileUploadInfo[] fileUploads);

        //not using this commanded 
        //Task<Response<IEnumerable<SKUModel>>> ViewSKURequest(string Department, int Storeid, CancellationToken token = default(CancellationToken));

        Task<Response<IEnumerable<TypeViewModel>>> GetApproveSKURequest(int CategoryId, CancellationToken token = default(CancellationToken));

        Task<Response<string>> ApprovedSKURequest(SKURequestApproveModel sKUModel, CancellationToken token = default(CancellationToken));

        Task<Response<SKUViewModelVM>> AlterSKUSearch(int StoreId, int ProductTypeId, int ManufaureId, CancellationToken token = default(CancellationToken));

        Task<Response<string>> UpdateSKUSearch(int StoreId, int ProductTypeId, int ManufaureId, UpdateSKVModel SKUupdate, CancellationToken token = default(CancellationToken));

        Task<Response<ProductCertificateFileInfo>> GetproductCertificate(ProductCertificateSearch productCertificateSearch, CancellationToken token = default(CancellationToken));

        Task<Response<List<ProductCertificateView>>> GetViewSKU(ViewSKUSearch viewSKU, CancellationToken token = default(CancellationToken));

        //Task<Response<SKUProductVm>> GetViewSKUProduct(ViewSKUProductSearch viewSKU, CancellationToken token = default(CancellationToken));

        Task<Response<List<string>>> CreateSKURequestWithCertificate(List<CreateSKUModel> createSKUs, CancellationToken token = default(CancellationToken));

        Task<Response<IEnumerable<NewProductV2>>> GetSKUApproveListV2(CancellationToken token = default(CancellationToken));

        Task<Response<string>> RemovePendingApprovalSKU(int ProductId,string Remarks, CancellationToken token = default(CancellationToken));

        Task<Response<string>> ApprovalSKU(NewProductV2 newProductV2, CancellationToken token = default(CancellationToken));

    }
}
