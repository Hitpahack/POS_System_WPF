using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Attributes;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.Coupon;
using FalcaPOS.Entites.Customers;
using FalcaPOS.Entites.Location;
using FalcaPOS.Entites.Sales;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace FalcaPOS.ServiceLibrary.Sales.Services
{
    public class SalesService : ISalesService
    {

        private readonly INotificationService _notificationService;


        public SalesService(INotificationService notificationService)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public async Task<ResponseInvoice> AddSales(SalesModel salesDetails, CancellationToken token = default(CancellationToken))
        {
            try
            {
                var _result = await HttpHelper.PostAsync<SalesModel, ResponseInvoice>(AppConstants.ADD_SALES, salesDetails, AppConstants.ACCESS_TOKEN, token);

                if (_result != null && !_result.success)
                {
                    _notificationService.ShowMessage(_result.message, NotificationType.Error);
                }

                return _result;
            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);

                return new ResponseInvoice
                {
                    success = false,
                    message = _ex.Message
                };
            }

        }

        public async Task<IEnumerable<District>> GetDistricts(int stateId, CancellationToken token)
        {
            try
            {
                return await HttpHelper.GetAsync<IEnumerable<District>>($"{AppConstants.GET_STATE_DISTRICTS}/{stateId}", AppConstants.ACCESS_TOKEN, token);
            }
            catch (Exception _ex)
            {
                //Log to file.
            }

            return Enumerable.Empty<District>();
        }


        public async Task<AddSales> GetServiceProducts(string invoiceNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (string.IsNullOrEmpty(invoiceNumber))
                {
                    _notificationService.ShowMessage("Invoice number cannot be empty", NotificationType.Error);
                }

                var _result = await HttpHelper.GetAsync<Response<AddSales>>
                    ($"{AppConstants.GET_INVOICE_SERVICE_PRODUCTS}/{invoiceNumber}", AppConstants.ACCESS_TOKEN, cancellationToken);

                if (_result != null)
                {
                    if (_result.IsSuccess)
                    {
                        return _result.Data;
                    }
                    else
                    {
                        _notificationService.ShowMessage(_result.Error, NotificationType.Error);
                    }

                }

            }
            catch (Exception _ex)
            {

            }

            return default(AddSales);

        }



        public async Task<IEnumerable<State>> GetStates(CancellationToken token)
        {
            try
            {
                return await HttpHelper.GetAsync<IEnumerable<State>>($"{AppConstants.GET_STATES}?isenabled=true", AppConstants.ACCESS_TOKEN, token);
            }
            catch (Exception Ex)
            {

            }

            return Enumerable.Empty<State>();
        }

        public async Task<SalesProduct> GetStockProduct(string productBarCode, CancellationToken token = default(CancellationToken))
        {
            try
            {
                var _productCode = HttpUtility.UrlEncode(productBarCode);

                if (!_productCode.IsValidString()) return null;

                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.GetAsync<Response<SalesProduct>>($"{AppConstants.GET_SALES_PRODUCT}/{_productCode}", AppConstants.ACCESS_TOKEN, token);

                if (_result != null && !_result.IsSuccess)
                {
                    _notificationService.ShowMessage(_result.Error, NotificationType.Error);

                    return null;
                }
                else
                {
                    return _result.Data;
                }
            }
            catch (TaskCanceledException _ex)
            {
                _notificationService.ShowMessage("An error occurred ,try again" + _ex, NotificationType.Error);
            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage("An error occurred ,try again",NotificationType.Error );
                new Response<SalesProduct>
                {
                    IsSuccess = false,
                    Error = _ex.Message
                };
            }
            return null;

        }


        public async Task<Response<IEnumerable<SalesInvoice>>> GetSales(SearchParams searchParams = null, CancellationToken token = default(CancellationToken))
        {

            try
            {
                string _urlSalesList = AppConstants.GET_SALES_LIST;

                if (searchParams != null)
                {
                    Dictionary<string, string> _querry = new Dictionary<string, string>();

                    if (searchParams.InvoiceNumber.IsValidString())
                    {
                        _querry.Add("InvoiceNumber", searchParams.InvoiceNumber);
                    }
                    if (searchParams.CustomerName.IsValidString())
                    {
                        _querry.Add("CustomerName", searchParams.CustomerName);
                    }
                    if (searchParams.CustomerPhone.IsValidString())
                    {
                        _querry.Add("CustomerPhone", searchParams.CustomerPhone);
                    }

                    if (searchParams.InvoiceFromDate != null && searchParams.InvoiceFromDate.HasValue)
                    {

                        _querry.Add("InvoiceFromDate", HttpUtility.UrlEncode(searchParams.InvoiceFromDate.Value.ToString()));
                    }

                    if (searchParams.InvoiceToDate != null && searchParams.InvoiceToDate.HasValue)
                    {
                        _querry.Add("InvoiceToDate", HttpUtility.UrlEncode(searchParams.InvoiceToDate.Value.ToString()));
                    }
                    if (searchParams.OrderTacknBy.IsValidString())
                    {
                        _querry.Add("OrderTacknBy", searchParams.OrderTacknBy);
                    }
                    _querry.Add("IsParent", searchParams.IsParent == true ? "True" : "False");

                    _querry.Add("StoreId", Convert.ToString(searchParams.StoreId));


                    if (_querry.Any())
                    {
                        var _stringBuilder = new StringBuilder();

                        string _search = "?";

                        foreach (var item in _querry)
                        {
                            _stringBuilder.Append(_search + item.Key + "=" + item.Value);
                            _search = "&";

                        }
                        _urlSalesList = _urlSalesList + _stringBuilder.ToString();
                    }


                }


                var _result = await HttpHelper.GetAsync<Response<IEnumerable<SalesInvoice>>>(_urlSalesList, AppConstants.ACCESS_TOKEN, token);


                //if(_result !=null && !_result.IsSuccess)
                //{
                //    _notificationService.ShowMessage(_result.Error, NotificationType.Error);
                //}

                return _result;

            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            return new Response<IEnumerable<SalesInvoice>>
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

        public async Task<ResponseInvoice> GetSaleInvoicePDF(string invoiceNumber, CancellationToken token = default(CancellationToken))
        {
            try
            {
                if (!invoiceNumber.IsValidString())
                {
                    return new ResponseInvoice
                    {
                        success = false,
                        message = "Invalid Invoice number"
                    };
                }

                var _result = await HttpHelper
                        .GetAsync<ResponseInvoice>($"{AppConstants.GET_INVOICE_PDF}/{HttpUtility.UrlEncode(invoiceNumber)}", AppConstants.ACCESS_TOKEN, token);

                return _result;
            }
            catch (Exception _ex)
            {
                return new ResponseInvoice
                {                    
                    success = false,
                    message = _ex.Message
                };
            }


        }

        public async Task<Response<ObservableCollection<string>>> GetSalesInvoiceNumber(CancellationToken token = default(CancellationToken))
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper
                    .GetAsync<Response<ObservableCollection<string>>>(AppConstants.GET_SALES_INVOICE_NUMBER, AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (Exception _ex)
            {
            }

            return new Response<ObservableCollection<string>> { IsSuccess = false };

        }

        public async Task<List<BusinessModelResponse>> GetMIS(BusinessModelRequest modelRequest)
        {
            try
            {

                var _result = await HttpHelper.PostAsyncDataSet<BusinessModelRequest, List<BusinessModelResponse>>(AppConstants.GET_MIS, modelRequest, AppConstants.ACCESS_TOKEN);

                return _result;
            }
            catch (Exception _ex)
            {

            }
            return null;
        }


        public async Task<DataSet> GetMISinDetail(BusinessModelRequest request)
        {
            try
            {

                var _result = await HttpHelper.PostAsyncDataSet<BusinessModelRequest, DataSet>(AppConstants.GET_MIS_IN_DETAILS, request, AppConstants.ACCESS_TOKEN);

                return _result;
            }
            catch (Exception _ex)
            {

            }
            return null;
        }
        public async Task<Response<SalesInvoice>> GetExchangeProduct(string InvoiceNo, CancellationToken token = default(CancellationToken))
        {

            try
            {


                var _result = await HttpHelper.GetAsync<Response<SalesInvoice>>($"{AppConstants.GET_EXCAHNGE_PRODUCT}/{HttpUtility.UrlEncode(InvoiceNo)}", AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            return new Response<SalesInvoice>
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

        public async Task<Response<CustomerDetails>> GetCustomerByPhone(string PhoneNumber, CancellationToken token = default)
        {

            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.GetAsync<CustomerDetails>($"{AppConstants.GET_CUSTOMER_BY_PHONE}/{PhoneNumber}", AppConstants.ACCESS_TOKEN, token);

                if (_result != null)
                {
                    return new Response<CustomerDetails>
                    {
                        IsSuccess = false,
                        Error = $"Customer with phone number {PhoneNumber} already present"
                    };
                }
            }
            catch (Exception _ex)
            {
            }

            return new Response<CustomerDetails>
            {
                IsSuccess = true
            };

        }

        public async Task<Response<string>> FarmerReturnProduct(SalesInvoice Returnproduct, CancellationToken token = default(CancellationToken))
        {

            try
            {

                var _result = await HttpHelper.PostAsync<SalesInvoice, Response<String>>(AppConstants.FARMER_RETURN_PRODUCT, Returnproduct, AppConstants.ACCESS_TOKEN, token);



                return _result;

            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }


        public async Task<Response<IEnumerable<CreditSalesViewModel>>> GetCreditList(CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper
                    .GetAsync<Response<IEnumerable<CreditSalesViewModel>>>(AppConstants.GET_CREDIT_SALES, AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (Exception _ex)
            {
                return new Response<IEnumerable<CreditSalesViewModel>>
                {
                    IsSuccess = false,
                    Error = _ex.Message
                };
            }
        }

        public async Task<Response<string>> UpdateCredtiSales(UpdateCreditSales creditsales, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper
                    .PostAsync<UpdateCreditSales, Response<string>>(AppConstants.UPDATE_CREDIT_SALES, creditsales, AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (Exception _ex)
            {
                return new Response<string>
                {
                    IsSuccess = false,
                    Error = _ex.Message
                };
            }
        }

        public async Task<Response<string>> UploadChequeFiles(string invoicenumber, FileUploadInfo[] fileUploads)
        {
            try
            {


                using (var _formContent = new MultipartFormDataContent())
                {

                    _formContent.Add(new StringContent(invoicenumber), "InvoiceNumber");

                    MapUploadFilesToForm(fileUploads, _formContent);

                    var _result = await HttpHelper.PostFormDataAsync<Response<string>>(AppConstants.UPLOAD_CHEQUE_FILES, AppConstants.ACCESS_TOKEN, _formContent);

                    return _result;
                }

            }
            catch (PosException _ex)
            {
                return new Response<string>
                {
                    IsSuccess = false,
                    Error = _ex?.Message
                };
            }
            catch (Exception _ex)
            {
            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "An error occurred while file upload, try uploading again"
            };
        }

        private void MapUploadFilesToForm(FileUploadInfo[] fileUploads, MultipartFormDataContent formContent)
        {
            for (int i = 0; i < fileUploads.Length; i++)
            {

                if (!File.Exists(fileUploads[i].FilePath))
                    throw new PosException($"File {fileUploads[i].FileName} is not avaliable , try again");

                formContent.Add(new StreamContent(new FileStream(fileUploads[i].FilePath, FileMode.Open)), $"Files", fileUploads[i].FileName);
            }
        }

        public async Task<Response<IEnumerable<FinanceCreditSalesViewModel>>> GetCreditListfinancePage(CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper
                    .GetAsync<Response<IEnumerable<FinanceCreditSalesViewModel>>>(AppConstants.GET_CREDIT_SALES_FINANCE, AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (Exception _ex)
            {
                return new Response<IEnumerable<FinanceCreditSalesViewModel>>
                {
                    IsSuccess = false,
                    Error = _ex.Message
                };
            }
        }

        public async Task<Response<string>> UpdateCredtiSalesFinance(string InvoiceNumber, string RealizeDate, CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper
                    .PostAsync<object, Response<string>>(AppConstants.UPDATE_CREDIT_SALES_FINANCE, new { InvoiceNumber, RealizeDate }, AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (Exception _ex)
            {
                return new Response<string>
                {
                    IsSuccess = false,
                    Error = _ex.Message
                };
            }
        }

        public async Task<IEnumerable<SalesProduct>> GetStockProductSKUSearch(string SKU, CancellationToken token = default(CancellationToken))
        {
            try
            {
                var _sku = HttpUtility.UrlEncode(SKU);

                if (!_sku.IsValidString()) return null;

                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper.GetAsync<Response<IEnumerable<SalesProduct>>>($"{AppConstants.GET_SALES_PRODUCT_SKUSEARCH}/{_sku}", AppConstants.ACCESS_TOKEN, token);

                if (_result != null && !_result.IsSuccess)
                {
                    _notificationService.ShowMessage(_result.Error, NotificationType.Error);

                    return null;
                }
                else
                {
                    return _result.Data;
                }
            }
            catch (TaskCanceledException _ex)
            {

            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage("An error occurred ,try agian", NotificationType.Error);
            }
            return null;

        }

        public async Task<Response<IEnumerable<AppOrderModel>>> GetAppOrderList(CancellationToken token = default)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var _result = await HttpHelper
                    .GetAsync<Response<IEnumerable<AppOrderModel>>>(AppConstants.GET_APP_ORDER_LIST, AppConstants.ACCESS_TOKEN, token);

                return _result;

            }
            catch (Exception _ex)
            {
                return new Response<IEnumerable<AppOrderModel>>
                {
                    IsSuccess = false,
                    Error = _ex.Message
                };
            }
        }

        public async Task<Response<string>> CancelAppOrderCustomer(int Id, CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.PostAsync<int, Response<String>>(AppConstants.CANCEL_APP_ORDER_CUSTOMER, Id, AppConstants.ACCESS_TOKEN, token);



                return _result;

            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

        public async Task<Response<string>> DeliveryAppOrderCustomer(int Id, CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.PostAsync<int, Response<String>>(AppConstants.DELIVER_APP_ORDER_CUSTOMER, Id, AppConstants.ACCESS_TOKEN, token);


                return _result;

            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }

        public async Task<Response<IEnumerable<FinanceCreditSalesViewModel>>> SearchCreditSalesFinance(string RealizeFromdate, string RealizeTodate, CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.PostAsync<object, Response<IEnumerable<FinanceCreditSalesViewModel>>>(AppConstants.GET_REALIZE_DATECREDIT_SALES_FINANCE, new { RealizeFromdate, RealizeTodate }, AppConstants.ACCESS_TOKEN, token);


                return _result;

            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            return new Response<IEnumerable<FinanceCreditSalesViewModel>>
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }


        //public async Task<Response<IEnumerable<Entites.Customers.Village>>> GetVillage(CancellationToken token = default) {
        //    try {

        //        var _result = await HttpHelper.GetAsync<Response<IEnumerable<Entites.Customers.Village>>>(AppConstants.GET_VILLAGE ,AppConstants.ACCESS_TOKEN, token);


        //        return _result;

        //    }
        //    catch (Exception _ex) {
        //        _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
        //    }

        //    return new Response<IEnumerable<Entites.Customers.Village>> {
        //        IsSuccess = false,
        //        Error = "No records found."
        //    };
        //}

        public async Task<Response<string>> AddCustomer(CustomerModel model, CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.PostAsync<CustomerModel, Response<string>>(AppConstants.ADD_CUSTOMER, model, AppConstants.ACCESS_TOKEN, token);


                return _result;

            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "No records found."
            };
        }





        public async Task<Response<PincodeDetails>> GetPincodeDetailsNew(int pincode, CancellationToken token)
        {
            try
            {
                return await HttpHelper.GetAsync<Response<PincodeDetails>>($"{AppConstants.GET_PINCODEDETAILS}/{pincode}", AppConstants.ACCESS_TOKEN, token);


            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            return new Response<PincodeDetails>()
            {
                IsSuccess = false,
                Error = "No records found.",
                Data = null
            };
        }

      
        public async Task<Response<string>> EditCustomer(CustomerModel model, CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.PostAsync<CustomerModel, Response<string>>(AppConstants.EDIT_CUSTOMER, model, AppConstants.ACCESS_TOKEN, token);


                return _result;

            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "No records found."
            };

        }

        public async Task<Response<String>> GetCouponValidity(string couponCode, CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.GetAsync<Response<string>>($"{AppConstants.GET_COUPON_VALIDAITY}/{couponCode}", AppConstants.ACCESS_TOKEN, token);


                return _result;

            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            return new Response<string>
            {
                IsSuccess = false,
                Error = "No records found."
            };

        }

        public async Task<Response<String>> CheckForRedeemCoupon(string phoneNumber, CancellationToken token = default)
        {
            try
            {

                var _result = await HttpHelper.GetAsync<Response<String>>($"{AppConstants.CHECKFOR_REDEEM_COUPON}/{phoneNumber}", AppConstants.ACCESS_TOKEN, token);
                return _result;

            }
            catch (Exception _ex)
            {
                //this is wrong
                _notificationService.ShowMessage(_ex.Message, NotificationType.Error);
            }

            return new Response<String>
            {
                IsSuccess = false,
                Error = "No records found."
            };

        }

        public async Task<Response<SchemeOutPutDTO>> FetchCoupon(SchemeDTO schemeViewModel, CancellationToken token = default)
        {
            try
            {
                var _result = await HttpHelper.PostAsync<SchemeDTO, Response<SchemeOutPutDTO>>(AppConstants.FETCH_COUPON, schemeViewModel, AppConstants.ACCESS_TOKEN);
                return _result;

            }
            catch (Exception _ex)
            {

            }
            return new Response<SchemeOutPutDTO>
            {
                IsSuccess = false,
                Error = "No records found."
            };


        }

        public async Task<Response<SchemeOutPutDTO>> ApplyCoupon(string billingId,string status, CancellationToken token = default)
        {
            try
            {
                var _result = await HttpHelper.PostAsync<object, Response<SchemeOutPutDTO>>(AppConstants.APPLY_COUPON,new {billingId,status }, AppConstants.ACCESS_TOKEN);
                return _result;

            }
            catch (Exception _ex)
            {

            }
            return new Response<SchemeOutPutDTO>
            {
                IsSuccess = false,
                Error = "No records found."
            };


        }
    }
}