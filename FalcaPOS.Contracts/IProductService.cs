using FalcaPOS.Common.Helper;
using FalcaPOS.Entites.Dtos;
using FalcaPOS.Entites.Products;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{
    public interface IProductService
    {
        Task<Response<string>> CreateProduct(ProductDetails product);

        Task<ProductDetails> GetProduct(int id);

        Task<IEnumerable<ProductViewModel>> GetProducts(string Querry = "", CancellationToken token = default(CancellationToken));

        Task<Response<string>> EnabelDiableProduct(int productId, bool isenable, CancellationToken token = default(CancellationToken));

        Task<Response<InventaryProductViewModel>> GetSKUNumberProduct(int productId,int storeId, CancellationToken token = default(CancellationToken));
        Task<Response<InventaryProductViewModel>> GetBrandCategorySubcategorySKUbyId(int productId, CancellationToken token = default(CancellationToken));
        Task<Response<IEnumerable<ProductSearchModel>>> SearchProducts(string name, CancellationToken token = default(CancellationToken));
        Task<Response<string>> ProductCurrentSKU(int productTypeId, CancellationToken token = default(CancellationToken));

        Task<IEnumerable<ProductDetails>> GetenabledProducts(string Querry = "", CancellationToken token = default(CancellationToken));
        Task<Response<string>> GetStockbySKU(string sku,int? StoreId, CancellationToken token = default(CancellationToken));
        Task<Response<ProductDTO>> GetSKUStockProductSearch(int productId, CancellationToken token = default(CancellationToken));
        Task<Response<InventaryProductViewModel>> GetSKUSelectTransferProduct(int productId,int FromStoreId, int storeId, CancellationToken token = default(CancellationToken));
    }
}
