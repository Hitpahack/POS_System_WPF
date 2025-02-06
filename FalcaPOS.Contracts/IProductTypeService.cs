using FalcaPOS.Common.Helper;
using FalcaPOS.Entites.Manufacturers;
using FalcaPOS.Entites.ProductTypes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{
    public interface IProductTypeService
    {
       // Task<ProductType> CreateProductType(CreateProductTypeModel productType);

        Task<IEnumerable<Manufacturer>> GetProductTypeManufacturers(long productTypeId, CancellationToken token = default);

        Task<IEnumerable<ProductType>> GetProductTypes(string query = null);

        Task<Response<string>> EnableDisbaleProductType(int typeId, bool isenable, CancellationToken token = default);

        [Obsolete("Use CreateProductTypeManufacturerAsync instead")]
        Task<Response<string>> CreateProductTypeManufacturer(ProductTypeManufacturer productTypeManufacturer);

       // Task<Response<string>> GetCurrentDeptCode();

        /// <summary>
        /// Create or map new brand with product type
        /// </summary>
        /// <param name="manufacturer"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<Response<string>> CreateProductTypeManufacturerAsync(CreateManufactureModel manufacturer, CancellationToken token = default);


        /// <summary>
        /// Get all the enabled manufactureres
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<Response<IEnumerable<Manufacturer>>> GetAllEnabledManufacturers(CancellationToken token = default);

        Task<Response<IEnumerable<CategoryModel>>> GetAllCategory(CancellationToken token = default);
        Task<Response<IEnumerable<CategoryModel>>> GetAllLicenseCategory(CancellationToken token = default);

        Task<Response<IEnumerable<SubCategoryModel>>> GetSubCategory(int CategoryId, CancellationToken token = default);

        Task<Response<string>> AddCategory(string CategoryName,bool IsCertificate, CancellationToken token = default);

        Task<Response<ProductType>> AddSubCategory(int CategoryId, string CategoryName, CancellationToken token = default);

    }
}
