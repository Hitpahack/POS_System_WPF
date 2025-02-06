using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Customers;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace FalcaPOS.Customer.ViewModels
{
    public class CustomerViewModel : BindableBase
    {
        private readonly ICustomerService _customerService;

        public DelegateCommand<object> RefreshDataGrid { get; set; }

        private IEventAggregator _eventAggregator { get; set; }

        private readonly Logger _logger;

        private readonly ProgressService _progressService;

        CustomerModelRequest customerDetails = new CustomerModelRequest();


        private readonly INotificationService _notificationService;

        public DelegateCommand<object> CustomerSearchCommand { get; private set; }
        public CustomerViewModel(INotificationService notificationService, Logger logger, ProgressService progressService, IEventAggregator eventAggregator, ICustomerService customerService)
        {
            _customerService = customerService;

            _eventAggregator = eventAggregator;

            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            customerDetails.Location = AppConstants.UserName;

            _eventAggregator.GetEvent<CustomerSearchFlyout>().Subscribe(LoadData);

            RefreshDataGrid = new DelegateCommand<object>(RefreshGrid);

            CustomerSearchCommand = new DelegateCommand<object>(SearchData);

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));


        }

        public void SearchData(object obj)
        {
            try
            {

                Name = null;
                Phone = null;
                if (string.IsNullOrEmpty(NameOrPhone) && string.IsNullOrEmpty(FromDate) && string.IsNullOrEmpty(ToDate))
                {

                    _notificationService.ShowMessage("Please enter any one filed", Common.NotificationType.Error);
                    return;
                }

                if ((!string.IsNullOrEmpty(FromDate) && string.IsNullOrEmpty(ToDate)) || (string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate)))
                {
                    _notificationService.ShowMessage("Please enter from and to date range ", Common.NotificationType.Error);
                    return;
                }
                if (FromDate != null && ToDate != null) {
                    if (Convert.ToDateTime(FromDate) > Convert.ToDateTime(ToDate)) {
                        _notificationService.ShowMessage("From Date should be less than or equal To Date", Common.NotificationType.Error);
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(NameOrPhone))
                {
                    if (IsAlphaNumeric(NameOrPhone))
                    {

                        _notificationService.ShowMessage("Please enter either name or phone number", Common.NotificationType.Error);
                        return;
                    }



                    if (ValidateOnlyNumber(NameOrPhone))
                    {

                        if (!NameOrPhone.IsValidPhone() || NameOrPhone.Length != 10)
                        {
                            _notificationService.ShowMessage("Please enter valid phone number", Common.NotificationType.Error);
                            return;
                        }
                        Phone = NameOrPhone;
                    }
                    else
                        Name = NameOrPhone;
                }


                var _search = new CustomerSearchModel()
                {
                    FromDate = string.IsNullOrEmpty(FromDate) ? null : FromDate,
                    ToDate = string.IsNullOrEmpty(ToDate) ? null : ToDate,
                    Name = string.IsNullOrEmpty(Name) ? null : Name,
                    Phone = string.IsNullOrEmpty(Phone) ? null : Phone,
                    Location = AppConstants.LoggedInStoreInfo.StoreId ==1?null: AppConstants.LoggedInStoreInfo.StoreId.ToString(),//rework type
                };

                LoadData(_search);
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }
        public async void LoadData(CustomerSearchModel ModelRequest)
        {
            try
            {
                //ModelRequest.Location=null;

                await _progressService.StartProgressAsync();
                List<CustomerModelResponse> data = new List<CustomerModelResponse>();

                var _result = await _customerService.GetCustomerSearch(ModelRequest);

                if (_result != null && _result.Tables.Count > 0)
                {




                    var _rows = _result.Tables[0].Rows.Cast<DataRow>().ToList();
                    if (_rows.Count > 0)
                    {
                        _rows.ForEach(x =>
                        {
                            data.Add(new CustomerModelResponse()
                            {
                                CustomerName = x.Field<string>("name"),
                                Phone = x.Field<string>("phone"),
                                Producttype = x.Field<string>("producttype"),
                                Category = x.Field<string>("category"),
                                Brnad = x.Field<string>("brand"),
                                ProductName = x.Field<string>("productname"),
                                Location = x.Field<string>("location"),
                                Status = x.Field<string>("status"),
                                SerialNo = x.Field<string>("serialno"),
                                WarantyService = x.Field<string>("waranty_service"),
                                //Expirydate = x.Field<string>("expirydate"),
                                DiscountType = x.Field<string>("discounttype"),
                                Discount = x.Field<double>("discount"),
                                Gst = x.Field<Int64>("gst"),
                                GstAmount = x.Field<double>("gstamount"),
                                Total = x.Field<double>("total"),
                                InvoiceNo = x.Field<string>("invoiceno"),
                                InvoiceDate = x.Field<DateTime?>("invoicedate")?.ToString("dd MMM yyyy"),

                                Payment = x.Field<DateTime?>("Paymentdate")?.ToString("dd MMM yyyy"),
                                Email = x.Field<string>("email"),
                                street = x.Field<string>("street"),
                                city = x.Field<string>("city"),
                                district = x.Field<string>("name"),
                                Pincode = x.Field<Int64>("pincode"),
                                state = x.Field<string>("name"),
                                Cash = x.Field<double>("cash"),
                                Card = x.Field<double>("card"),
                                UPI = x.Field<double>("upi")

                            });

                        });
                    }



                }

                if (data != null && data.ToList().Count > 0)
                {
                    CustomerDetail = new ObservableCollection<CustomerModelResponse>(data);
                    TotalCount = data.ToList().Count();

                }
                else
                {
                    CustomerDetail = new ObservableCollection<CustomerModelResponse>(data);
                    TotalCount = 0;

                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            finally
            {
                await _progressService.StopProgressAsync();
            }

        }


        public void RefreshGrid(object obj)
        {
            CustomerDetail = null;
            FromDate = null;
            ToDate = null;
            NameOrPhone = null;
            TotalCount = 0;
        }


        public bool IsAlphaNumeric(string NameOrPhone)
        {
            try
            {

                string pattern = "^(?=.*[a-zA-Z])(?=.*[0-9])[A-Za-z0-9]+$";

                Regex rg = new Regex(pattern);

                if (string.IsNullOrEmpty(NameOrPhone))
                {
                    return false;
                }

                if (rg.IsMatch(NameOrPhone))
                {
                    return true; ;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return true;
            }
        }
        public bool ValidateOnlyLetters(string Name)
        {
            try
            {
                if (Regex.IsMatch(Name, @"^[a-zA-Z]+$"))
                    return false;
                else
                    return true;


            }
            catch (Exception)
            {
                return true;
            }
        }
        public bool ValidateOnlyNumber(string phone)
        {
            try
            {
                if (Regex.IsMatch(phone, @"^[0-9]+$"))
                    return true;
                else
                    return false;


            }
            catch (Exception)
            {
                return true;
            }
        }
        private ObservableCollection<CustomerModelResponse> _customerDetail;
        public ObservableCollection<CustomerModelResponse> CustomerDetail
        {
            get => _customerDetail;
            set => SetProperty(ref _customerDetail, value);
        }

        private int _totalCount;

        public int TotalCount
        {
            get => _totalCount;
            set => SetProperty(ref _totalCount, value);
        }

        private string _nameOrPhone;

        public string NameOrPhone
        {
            get => _nameOrPhone;
            set => SetProperty(ref _nameOrPhone, value);
        }

        private string _fromDate;

        public string FromDate
        {
            get => _fromDate;
            set => SetProperty(ref _fromDate, value);
        }

        private string _toDate;

        public string ToDate
        {
            get => _toDate;
            set => SetProperty(ref _toDate, value);
        }

        private string _name;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _phone;

        public string Phone
        {
            get => _phone;
            set => _phone = value;
        }

    }
}
