using FalcaPOS.Common;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Sales;
using FalcaPOS.Entites.Stores;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.Dashboard.ViewModels
{
    public class BusinessViewModel : BindableBase
    {
        private readonly ISalesService _SalesService;
        private readonly IStoreService _storeService;
        private readonly IEventAggregator _eventAggregator;
        public DelegateCommand<object> RowDoubleClickCommand { get; private set; }
        public DelegateCommand<BusinessModelRequest> SalesSearchByDate { get; private set; }
        public DelegateCommand<object> RefreshSalesPage { get; private set; }

        private readonly Logger _logger;
        public BusinessViewModel(ISalesService salesService, Logger logger, IStoreService storeService, IEventAggregator eventAggregator)
        {
            _logger = logger ?? throw new ArgumentException(nameof(logger));
            _SalesService = salesService ?? throw new ArgumentNullException(nameof(salesService));
            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            _eventAggregator = eventAggregator;
            RowDoubleClickCommand = new DelegateCommand<object>(DisplayBusinessinDetails);
            SalesSearchByDate = new DelegateCommand<BusinessModelRequest>(SalesPageSearchDate);

            RefreshSalesPage = new DelegateCommand<object>(RefreshGrid);

            _eventAggregator.GetEvent<SignalRStoreAddedEvent>().Subscribe(NewStoreAdded, ThreadOption.PublisherThread);
            LoadinitialData();
            GetAllStore();
            double userheightscreen = System.Windows.SystemParameters.PrimaryScreenHeight;
            MaxHeight = userheightscreen - 150;
        }
        private void NewStoreAdded(object str)
        {
            try
            {

                if (!(str is Store _str))
                {
                    return;
                }
                Application.Current?.Dispatcher?.Invoke(delegate
                {


                    if (BusinessModelRequest != null && BusinessModelRequest.StoreName != null)
                    {

                        if (BusinessModelRequest.StoreName.Contains(_str.Name))
                        {
                            return;
                        }

                        BusinessModelRequest.StoreName.Add(_str.Name);

                    }
                });
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }

        }
        public async void LoadinitialData()
        {
            BusinessModelRequest businessModelRequest = new BusinessModelRequest();
            businessModelRequest.SelectedStoreName = "Select Store";
            List<BusinessModelResponse> data = await _SalesService.GetMIS(businessModelRequest);
            if (data != null)
            {

                BusinessCollection = new ObservableCollection<BusinessModelResponse>(data);
            }


        }

        public async void GetAllStore()
        {
            try
            {

                var _result = await _storeService.GetStores();

                if (_result != null && _result.Any())
                {
                    Application.Current?.Dispatcher.Invoke(() =>
                    {
                        var store = (_result).Where(x => x.Parent_ref == null).Select(x => x.Name).ToList();
                        List<string> storeName = new List<string>();
                        storeName.Add("Select Store");
                        if (store != null && store.Count > 0)
                        {
                            store.ForEach(x =>
                            {
                                storeName.Add(x);
                            });
                        }

                        BusinessModelRequest.StoreName = storeName;
                        BusinessModelRequest.SelectedStoreName = storeName.FirstOrDefault();
                    });
                }


            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in business view model", _ex);
            }

        }


        public async void SalesPageSearchDate(BusinessModelRequest obj)
        {
            if (string.IsNullOrEmpty(obj.FromDate))
            {
                ShowAlert("Please select From Date", NotificationType.Error);
                return;
            }
            if (string.IsNullOrEmpty(obj.ToDate))
            {
                ShowAlert("Please select To Date ", NotificationType.Error);
                return;
            }

            if (!String.IsNullOrEmpty(obj.FromDate) && !string.IsNullOrEmpty(obj.ToDate))
            {
                DateTime dt1 = Convert.ToDateTime(obj.FromDate);
                DateTime dt2 = Convert.ToDateTime(obj.ToDate);
                if (dt2 < dt1)
                {
                    ShowAlert("From date should be less than or equal to To date", NotificationType.Error);
                    return;
                }
            }

            if (obj.SelectedStoreName != "Select Store")
            {
                BusinessModelRequest businessModelRequest = new BusinessModelRequest();
                businessModelRequest.FromDate = obj.FromDate;
                businessModelRequest.ToDate = obj.ToDate;
                businessModelRequest.SelectedStoreName = obj.SelectedStoreName;
                List<BusinessModelResponse> data = await _SalesService.GetMIS(businessModelRequest);
                if (data != null && data.Count > 1)
                {
                    BusinessCollection = new ObservableCollection<BusinessModelResponse>(data);
                }
                else
                {
                    BusinessCollection = null;
                    ShowAlert("No Transaction", NotificationType.Error);
                }

            }
            else
            {
                ShowAlert("Please select any one Store", NotificationType.Error);
                return;
            }

        }
        public async void DisplayBusinessinDetails(object obj)
        {
            try
            {
                var Model = obj as BusinessModelResponse;

                if (Model == null)
                {
                    //return invalid details

                    return;
                }

                if (Model.IsSelectedShallow)
                {
                    //Grid row  is already opend collapse .
                    Model.IsSelectedShallow = false;
                    Model.IsSelected = false;
                    return;
                }

                //check if already information is fetched 

                if (Model.HasInformation)
                {
                    //display and return
                    Model.IsSelectedShallow = true;
                    Model.IsSelected = true;
                    return;
                }



                if (!string.IsNullOrEmpty(Model.Date))
                {
                    Model.IsBusy = true;
                    Model.IsSelectedShallow = true;
                    Model.IsSelected = true;

                    BusinessModelRequest request = new BusinessModelRequest();
                    request.SelectedStoreName = Model.Store;
                    request.FromDate = Model.Date;
                    await Task.Run(async () =>
                    {


                        var _result = await _SalesService.GetMISinDetail(request);

                        Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            if (_result != null && _result.Tables[0].Rows.Cast<DataRow>().AsEnumerable().ToList().Count > 0)
                            {

                                if (Model != null)
                                {
                                    ShowMISinDetails(Model, _result);
                                    //Model.IsSelectedShallow = true;
                                    //Model.IsSelected = true;
                                    Model.IsBusy = false;
                                }
                            }
                            else
                            {
                                Model.IsBusy = false;

                            }

                        });


                    });
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
                //log _ex;
            }

        }


        private void ShowMISinDetails(BusinessModelResponse model, DataSet data)
        {
            try
            {
                if (model != null && data != null)
                {
                    var _M = new List<BusinessModelResponseIndetails>();
                    var rows = data.Tables[0].Rows.Cast<DataRow>().AsEnumerable().ToList();
                    rows.ForEach(x =>
                    {
                        _M.Add(new BusinessModelResponseIndetails()
                        {
                            CustomerName = x.Field<string>("Name"),
                            Phone = x.Field<string>("Phone"),
                            ProductType = x.Field<string>("Producttype"),
                            Brand = x.Field<string>("Brand"),
                            ProductName = x.Field<string>("ProductName"),
                            InvoiceNo = x.Field<string>("InvoiceNo"),
                            InvoiceDate = x.Field<DateTime>("InVoiceDate").ToString("dd-MM-yyyy"),
                            SellingPrice = x.Field<double>("SellingPrice"),
                            ExtraTotalDiscount = x.Field<double>("ExtraTotalDiscount"),
                            TotalServiceCharge = x.Field<double>("TotalServiceCharge"),
                            Total = (x.Field<double>("SellingPrice")) * (x.Field<long>("SoldQty")),
                            ProductTotalServiceCharge = x.Field<double>("ProductTotalServiceCharge"),
                            SoldQty = x.Field<long>("SoldQty"),



                        }); ;
                    });

                    if (_M != null)
                    {
                        //var datas = (from p in _M
                        //             group p by p.InvoiceNo into g
                        //             select new
                        //             {
                        //                 InvoiceNo = g.Key,
                        //                 Name = g.Select(m => m.CustomerName).FirstOrDefault(),
                        //                 Phone = g.Select(y => y.Phone).FirstOrDefault(),
                        //                 Total = g.Select(z => z.Total).FirstOrDefault(),
                        //                 TotalServiceCharge = g.Select(x => x.TotalServiceCharge).FirstOrDefault(),
                        //                 Products = g.ToList(),
                        //             }).ToList();
                        //List<BusinessResponse> lists = new List<BusinessResponse>();
                        //if (datas != null && datas.Count > 0)
                        //{
                        //    datas.ForEach(x =>
                        //    {
                        //        lists.Add(new BusinessResponse()
                        //        {
                        //            CustomerName = x.Name,
                        //            InvoiceNo = x.InvoiceNo,
                        //            Phone = x.Phone,
                        //            total = x.Total,
                        //            ProductDetails = x.Products,
                        //            TotalServiceCharge = x.TotalServiceCharge
                        //        });
                        //    });
                        //    model.BusinessModelResponseIndetails = lists;
                        //}

                        //already has info no need to fetch again

                        model.BusinessModelProductIndetails = _M;
                        model.HasInformation = true;
                    }

                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
                //log error
            }
        }

        public async void RefreshGrid(object obj)
        {
            BusinessModelRequest businessModelRequest = new BusinessModelRequest();
            businessModelRequest.FromDate = null;
            businessModelRequest.ToDate = null;
            BusinessModelRequest.ToDate = null;
            BusinessModelRequest.FromDate = null;
            BusinessModelRequest.SelectedStoreName = "Select Store";
            businessModelRequest.SelectedStoreName = "Select Store";
            List<BusinessModelResponse> data = await _SalesService.GetMIS(businessModelRequest);
            if (data != null)
            {
                BusinessCollection = new ObservableCollection<BusinessModelResponse>(data);
            }

        }

        private ObservableCollection<BusinessModelResponse> _businessCollection;

        public ObservableCollection<BusinessModelResponse> BusinessCollection
        {
            get { return _businessCollection; }
            set { SetProperty(ref _businessCollection, value); }
        }


        private BusinessModelRequest _businessModelRequest = new BusinessModelRequest();
        public BusinessModelRequest BusinessModelRequest
        {
            get
            {
                return _businessModelRequest;
            }
            set
            {
                SetProperty(ref _businessModelRequest, value);
            }
        }

        private void ShowAlert(string message, NotificationType notification)
        {
            _eventAggregator.GetEvent<NotifyMessage>().Publish(new Common.Models.ToastMessage
            {
                Message = message,
                MessageType = notification
            });
        }

        private double _maxheight;

        public double MaxHeight
        {
            get { return _maxheight; }
            set
            {
                _maxheight = value;
                RaisePropertyChanged("MaxHeight");
            }
        }
    }
}
