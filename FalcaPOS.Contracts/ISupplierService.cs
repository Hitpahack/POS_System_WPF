using FalcaPOS.Common.Helper;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.Suppliers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{
    public interface ISupplierService
    {
        Task<IEnumerable<SuppliersViewModel>> GetSuppliers(string queryString = null);

        Task<Response<SuppliersDetailsViewModel>> GetSupplierDetails(int supplierId, CancellationToken token = default(CancellationToken));

        Task<Response<string>> AddSuppliers(CreateSupplierModel supplier);

        Task<Response<string>> EnableDisableSupplier(int id, bool isEnable, CancellationToken token = default(CancellationToken));


        Task<Response<string>> UpadateSupplier(int supplierId, CreateSupplierModel supplier);

        Task<bool> AddShippingAddress(ShippingAddress supplier);

        Task<Response<string>> UpdateSupplierDetails(SupplierEditViewModel supplier);

        Task<Response<IEnumerable<AddressViewModel>>> GetShippingAddress(int supplierId);

        Task<Response<IEnumerable<Bank>>> GetBankList();

        Task<Response<string>> AddNewBank(string BankName);

        Task<Response<string>> AddNewBankDetails(int SupplierId, int BankId, string BrnachName, string AccountNo, string AccountType, string IFSC, FileUploadInfo[] fileUploads);

        Task<Response<SupplierTDSStatus>> GetSupplierTDSDetails(int supplierId, CancellationToken token = default(CancellationToken));


    }
}
