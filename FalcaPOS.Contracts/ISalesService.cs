using FalcaPOS.Common.Helper;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.Coupon;
using FalcaPOS.Entites.Customers;
using FalcaPOS.Entites.Location;
using FalcaPOS.Entites.Sales;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Contracts
{
    public interface ISalesService
    {

        Task<SalesProduct> GetStockProduct(string productBarCode, CancellationToken token = default(CancellationToken));

        Task<ResponseInvoice> AddSales(SalesModel salesDetails, CancellationToken token = default(CancellationToken));

        Task<IEnumerable<State>> GetStates(CancellationToken token = default(CancellationToken));

        Task<IEnumerable<District>> GetDistricts(int stateId, CancellationToken token = default(CancellationToken));

        Task<AddSales> GetServiceProducts(string invoiceNumber, CancellationToken cancellationToken = default(CancellationToken));

        Task<Response<IEnumerable<SalesInvoice>>> GetSales(SearchParams searchParams = null, CancellationToken token = default(CancellationToken));

        Task<ResponseInvoice> GetSaleInvoicePDF(string invoiceNumber, CancellationToken token = default(CancellationToken));

        Task<Response<ObservableCollection<string>>> GetSalesInvoiceNumber(CancellationToken token = default(CancellationToken));

        Task<List<BusinessModelResponse>> GetMIS(BusinessModelRequest modelRequest);
        Task<DataSet> GetMISinDetail(BusinessModelRequest request);
        Task<Response<SalesInvoice>> GetExchangeProduct(String InvoiceNo, CancellationToken token = default(CancellationToken));
        Task<Response<string>> FarmerReturnProduct(SalesInvoice Returnproduct, CancellationToken token = default(CancellationToken));
        Task<Response<CustomerDetails>> GetCustomerByPhone(string PhoneNumber, CancellationToken token = default(CancellationToken));

        Task<Response<IEnumerable<CreditSalesViewModel>>> GetCreditList(CancellationToken token = default(CancellationToken));

        Task<Response<string>> UpdateCredtiSales(UpdateCreditSales salesDetails, CancellationToken token = default(CancellationToken));

        Task<Response<string>> UploadChequeFiles(string invoicenumber, FileUploadInfo[] fileUploads);

        Task<Response<IEnumerable<FinanceCreditSalesViewModel>>> GetCreditListfinancePage(CancellationToken token = default(CancellationToken));

        Task<Response<string>> UpdateCredtiSalesFinance(string InvoiceNumber, string RealizeDate, CancellationToken token = default(CancellationToken));

        Task<IEnumerable<SalesProduct>> GetStockProductSKUSearch(string SKU, CancellationToken token = default(CancellationToken));

        Task<Response<IEnumerable<AppOrderModel>>> GetAppOrderList(CancellationToken token = default(CancellationToken));

        Task<Response<string>> CancelAppOrderCustomer(int Id, CancellationToken token = default(CancellationToken));

        Task<Response<string>> DeliveryAppOrderCustomer(int Id, CancellationToken token = default(CancellationToken));

        Task<Response<IEnumerable<FinanceCreditSalesViewModel>>> SearchCreditSalesFinance(string RealizeFromdate, string RealizeTodate, CancellationToken token = default(CancellationToken));
        Task<Response<string>> AddCustomer(CustomerModel model, CancellationToken token = default(CancellationToken));
        Task<Response<string>> EditCustomer(CustomerModel model, CancellationToken token = default(CancellationToken));
        Task<Response<PincodeDetails>> GetPincodeDetailsNew(int pincode, CancellationToken token = default(CancellationToken));
        Task<Response<String>> CheckForRedeemCoupon(string phoneNumber, CancellationToken token = default(CancellationToken));
        Task<Response<String>> GetCouponValidity(string couponCode, CancellationToken token = default(CancellationToken));
        Task<Response<SchemeOutPutDTO>> FetchCoupon(SchemeDTO model, CancellationToken token = default(CancellationToken));

        Task<Response<SchemeOutPutDTO>> ApplyCoupon(string billingId, string status, CancellationToken token = default);

    }





}
