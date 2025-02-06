using FalcaPOS.Common.Helper;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.Denomination;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{
    public interface IDenominationService
    {
        Task<Response<string>> AddDenomination(AddDenominationModel denomination, CancellationToken token = default(CancellationToken));

        Task<Response<AddDenominationModel>> GetDenomination(DenominationSearchModel model, CancellationToken token = default(CancellationToken));

        Task<Response<DenominationModel>> GetTodaySales(bool IsPreDenoSuccess, string date = null, CancellationToken token = default(CancellationToken));

        Task<Response<PDFFileResult>> DownloadFile(string filenameId, CancellationToken token = default);

        Task<Response<string>> AddCashDeposit(DepositModel model, FileUploadInfo[] fileUploads, CancellationToken token = default(CancellationToken));

        Task<Response<IEnumerable<DepositBanksModel>>> GetDepositBankList(CancellationToken token = default(CancellationToken));

        Task<Response<List<DepositViewModelResponse>>> GetCashDepositSearch(string FromDate, string ToDate, int StoreId, CancellationToken token = default(CancellationToken));

        Task<Response<PDFFileResult>> DownloadFileDepsoit(int fileId, CancellationToken token = default);

        Task<Response<string>> UpdateCaseDepositApproval(int DepositId, bool IsApprove, CancellationToken token = default);

        Task<Response<DenominationVerifyModel>> GetDenominationVerify(CancellationToken token = default);

        Task<Response<IEnumerable<AddDenominationModel>>> GetDenominationSearch(int StoreId, string FromDate, string ToDate, CancellationToken token = default(CancellationToken));



    }
}
