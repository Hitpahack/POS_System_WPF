using FalcaPOS.Common.Helper;
using FalcaPOS.Entites.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{
    public interface IInvoiceFileService
    {
        Task<Response<string>> UploadInvoiceFiles(int invoiceId, FileUploadInfo[] fileUploads);

        Task<Response<PDFFileResult>> DownloadFile(int fileId, CancellationToken token = default);

        Task<Response<string>> DeleteFileById(int ivoiceId, int fileId);

        Task<Response<IEnumerable<PDFFileResult>>> DownloadFileList(List<int> fileId, CancellationToken token = default);

        Task<Response<PDFFileResult>> DownloadFileMSME(int fileId, CancellationToken token = default);

    }
}
