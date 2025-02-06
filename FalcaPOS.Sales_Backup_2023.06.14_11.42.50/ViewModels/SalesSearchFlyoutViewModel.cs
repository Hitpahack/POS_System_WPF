using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Sales;
using FalcaPOS.Entites.Stores;
using FalcaPOS.Sales.Models;
using MahApps.Metro.Controls;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace FalcaPOS.Sales.ViewModels
{
    public class SalesSearchFlyoutViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;

        public DelegateCommand SalesSearchCommand { get; private set; }

        public DelegateCommand ResetSearchCommand { get; private set; }

        private readonly Logger _logger;

        private readonly IStoreService _storeService;

        public SalesSearchFlyoutViewModel(IEventAggregator eventAggregator, IStoreService StoreService, Logger Logger)
        {
            Width = 1600;
            Height = 200;
            Position = Position.Top;

            _eventAggregator = eventAggregator;

            _logger = Logger ?? throw new ArgumentNullException(nameof(Logger));

            _storeService = StoreService ?? throw new ArgumentNullException(nameof(StoreService));

            _eventAggregator.GetEvent<SalesSearchFlyoutOpenEvent>().Subscribe((x) =>
            {

                SalesSearch = new SalesSearchModel();

                IsOpen = true;

                SalesSearch.IsParent = x.IsParent;
                if (!x.IsParent)
                {
                    if (AppConstants.USER_ROLES[0] == AppConstants.ROLE_STORE_PERSON)
                        SalesSearch.Stores = new ObservableCollection<Store>(Stores.Where(y => y.IsParent == false && y.Parent_ref == AppConstants.LoggedInStoreInfo.StoreId).ToList());
                    else
                        SalesSearch.Stores = new ObservableCollection<Store>(Stores.Where(y => y.IsParent == false).ToList());
                }

            });

            SalesSearchCommand = new DelegateCommand(SearchSales);

            ResetSearchCommand = new DelegateCommand(ResetSearch);

            GetStore();
        }

        private void ResetSearch()
        {
            SalesSearch = new SalesSearchModel();
        }

        private void SearchSales()
        {
            var _search = new SearchParams
            {
                CustomerName = SalesSearch?.CustomerName,
                InvoiceFromDate = SalesSearch?.InvoiceFromDate,
                InvoiceNumber = SalesSearch?.InvoiceNumber,
                InvoiceToDate = SalesSearch?.InvoiceToDate,
                CustomerPhone = SalesSearch?.CustomerPhone,
                OrderTacknBy = SalesSearch?.OderTackenBy,
                IsParent = SalesSearch.IsParent,
                StoreId = SalesSearch?.SelectedStores?.StoreId
            };


            IsOpen = false;

            _eventAggregator.GetEvent<SearchSalesEvent>().Publish(_search);



        }

        private string header;
        public string Header
        {
            get { return this.header; }
            set { SetProperty(ref this.header, value); }
        }

        private bool isOpen;
        public bool IsOpen
        {
            get { return this.isOpen; }
            set { SetProperty(ref this.isOpen, value); }
        }

        private Position position;
        public Position Position
        {
            get { return this.position; }
            set { SetProperty(ref this.position, value); }

        }

        private int _width;


        public int Width
        {
            get { return _width; }
            set { SetProperty(ref _width, value); }

        }
        private int _height;

        public int Height
        {
            get { return _height; }
            set { SetProperty(ref _height, value); }

        }

        private SalesSearchModel _salesSearch;
        public SalesSearchModel SalesSearch
        {
            get { return _salesSearch; }
            set { SetProperty(ref _salesSearch, value); }
        }

        public async void GetStore()
        {
            try
            {
                var _stores = await _storeService.GetStores();

                if (_stores != null)
                {

                    var addAllList = _stores.ToList();

                    addAllList.Insert(0, new Store() { StoreId = 1, Name = "ALL STORE", IsParent = false, Parent_ref = AppConstants.LoggedInStoreInfo.StoreId });

                    _stores = addAllList;

                    Stores = new ObservableCollection<Store>(_stores.Where(x => x.Parent_ref == null));
                }


            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);



            }
        }

        private ObservableCollection<Store> _stores;
        public ObservableCollection<Store> Stores
        {
            get { return _stores; }
            set { SetProperty(ref _stores, value); }
        }

    }



}
