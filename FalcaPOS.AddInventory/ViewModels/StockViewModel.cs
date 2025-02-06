using FalcaPOS.Common.Events;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Stock;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows.Input;

namespace FalcaPOS.AddInventory.ViewModels
{
    public class StockViewModel : BindableBase
    {
        private readonly IStockService _stockService;
        private readonly IDialogService _dialogService;

        public ICommand StockSerachFlyout { get; set; }

        public DelegateCommand<object> RefreshDataGrid { get; set; }

        StockModelRequest stockModelRequest = new StockModelRequest();


        private IEventAggregator EventAggregator { get; set; }

        public DelegateCommand<object> PrintBarCodeCommand { get; private set; }

        private readonly Logger _logger;


        public StockViewModel(IEventAggregator eventAggregator, Logger logger, IStockService stockService, IDialogService dialogService)
        {

            stockModelRequest.Status = "Stock";

            _logger = logger ?? throw new ArgumentException(nameof(logger));

            EventAggregator = eventAggregator;

            _stockService = stockService;

            _dialogService = dialogService;

            LoadData(stockModelRequest);

            StockSerachFlyout = new DelegateCommand<object>(OpenStockSearchflyoutOpen);

            RefreshDataGrid = new DelegateCommand<object>(RefreshGrid);

            EventAggregator.GetEvent<StockSearch>().Subscribe(LoadData);

            EventAggregator.GetEvent<ReloadStockEvent>().Subscribe(ReloadData);

            PrintBarCodeCommand = new DelegateCommand<object>(PrintBarCode);


        }

        private void ReloadData()
        {
            LoadData(stockModelRequest);
        }

        private void PrintBarCode(object obj)
        {
            if (obj is StockModelRespons stock)
                _dialogService.ShowDialog("BarCodeDialog", new DialogParameters($"barcode={stock.Barcode}"), (r) => { });

        }

        public async void LoadData(StockModelRequest stockModelRequest)
        {
            try
            {

                List<StockModelRespons> data = new List<StockModelRespons>();
                var _result = await _stockService.GetBackendStockSearch(stockModelRequest);
                if (_result != null && _result.Tables.Count > 0)
                {
                    var _rows = _result.Tables[0].Rows.Cast<DataRow>().ToList();
                    if (_rows.Count > 0)
                    {
                        _rows.ForEach(x =>
                        {
                            data.Add(new StockModelRespons()
                            {

                                SupplierName = x.Field<string>("suppliername"),
                                Producttype = x.Field<string>("producttype"),
                                Brand = x.Field<string>("brand"),
                                ProductName = x.Field<string>("productname"),
                                Location = x.Field<string>("location"),
                                Status = x.Field<string>("status"),
                                Barcode = x.Field<string>("barcode"),
                                WarantyService = x.Field<string>("waranty_service"),
                                Dateofmanufacturing = x.Field<DateTime>("dateofmanufacturing"),
                                //Qty=x.Field<int>("qty"),
                                Qadone = x.Field<bool>("Qadone"),
                                Expirydate = x.Field<DateTime>("expirydate"),
                                Rate = x.Field<double>("rate"),
                                Serialno = x.Field<string>("serialno"),
                                Sellingprice = x.Field<double>("sellingprice"),
                                Discount = x.Field<double>("discount"),
                                Discountmode = x.Field<string>("discountmode"),
                                Total = x.Field<double>("total"),
                                InvocieNo = x.Field<string>("invoiceno"),
                                InvocieDate = x.Field<DateTime>("invoicedate"),
                                Name = x.Field<string>("name"),
                                Value = x.Field<string>("value")



                            });

                        });
                    }



                }

                if (data != null && data.ToList().Count > 0)
                {
                    BackendStock = new ObservableCollection<StockModelRespons>(data);
                    TotalCount = data.ToList().Count();
                }
                else
                {
                    BackendStock = new ObservableCollection<StockModelRespons>(data);
                    TotalCount = 0;
                }


            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in stock  view model", _ex);
            }

        }

        public void RefreshGrid(object obj)
        {
            stockModelRequest.Status = "Stock";
            LoadData(stockModelRequest);
        }

        public void OpenStockSearchflyoutOpen(object obj)
        {
            EventAggregator.GetEvent<SearchFlyout>().Publish(true);

        }

        private ObservableCollection<StockModelRespons> _backendstock;
        public ObservableCollection<StockModelRespons> BackendStock
        {
            get
            {
                return _backendstock;
            }
            set
            {
                SetProperty(ref _backendstock, value);
            }
        }

        private int _totalcount;

        public int TotalCount
        {
            get { return _totalcount; }
            set { SetProperty(ref _totalcount, value); }
        }


    }
}
