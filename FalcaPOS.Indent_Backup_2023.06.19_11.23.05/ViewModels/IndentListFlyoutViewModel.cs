using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Indent;
using FalcaPOS.Entites.Stores;
using MahApps.Metro.Controls;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.Indent.ViewModels
{
    internal class IndentListFlyoutViewModel : BindableBase
    {
        private IEventAggregator _eventAggregator;
        private Logger _logger;
        private IStoreService _storeService;
        private INotificationService _notificationService;
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

        private GridLength _width;
        public GridLength Width
        {
            get { return _width; }
            set { SetProperty(ref _width, value); }

        }


        private GridLength _height;
        public GridLength Height
        {
            get { return _height; }
            set { SetProperty(ref _height, value); }

        }


        private ObservableCollection<Store> _stores;
        public ObservableCollection<Store> Stores
        {
            get { return _stores; }
            set { SetProperty(ref _stores, value); }
        }

        private Store _selectedStore;
        public Store SelectedStore
        {
            get { return _selectedStore; }
            set { SetProperty(ref _selectedStore, value); }
        }


        private ObservableCollection<String> _indentStatus;
        public ObservableCollection<String> IndentStatusList
        {
            get { return _indentStatus; }
            set { SetProperty(ref _indentStatus, value); }
        }

        private String _selectedindentStatus;
        public String SelectedIndentStatus
        {
            get { return _selectedindentStatus; }
            set { SetProperty(ref _selectedindentStatus, value); }
        }


        private string _fromDate;
        public string FromDate
        {
            get { return _fromDate; }
            set { SetProperty(ref _fromDate, value); }
        }

        private string _toDate;
        public string ToDate
        {
            get { return _toDate; }
            set { SetProperty(ref _toDate, value); }
        }

        private bool _Isindentpaymentfull;

        public bool Isindentpaymentfull
        {
            get { return _Isindentpaymentfull; }
            set { SetProperty(ref _Isindentpaymentfull, value); if (value) Isindentpaymentpartial = false; }
        }

        private bool _Isindentpaymentpartial;

        public bool Isindentpaymentpartial
        {
            get { return _Isindentpaymentpartial; }
            set { SetProperty(ref _Isindentpaymentpartial, value); if (value) Isindentpaymentfull = false; }
        }




        public DelegateCommand CloseFlyOutIndentListCommand { get; set; }
        public DelegateCommand SearchFlyOutIndentListCommand { get; set; }
        public IndentListFlyoutViewModel(IEventAggregator eventAggregator, Logger logger, IStoreService storeService, INotificationService notificationService)
        {
            Width = GridLength.Auto;
            Height = new GridLength(1200);//move to common folder

            Position = Position.Top;
            CloseFlyOutIndentListCommand = new DelegateCommand(() => { ClearForm(); IsOpen = false; });
            SearchFlyOutIndentListCommand = new DelegateCommand(ValidateSearch);

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _eventAggregator.GetEvent<IndentListFlyoutOpenEvent>().Subscribe((x) =>
            {
                IsOpen = true;
            });

            if (!AppConstants.USER_ROLES.Contains(AppConstants.ROLE_STORE_PERSON)
                 && !AppConstants.USER_ROLES.Contains(AppConstants.ROLE_BACKEND)
                 && !AppConstants.USER_ROLES.Contains(AppConstants.ROLE_SUPER_BACKEND)
                )
                LoadStoresAsync();
            else
            {
                Stores = new ObservableCollection<Store>() { AppConstants.LoggedInStoreInfo };
            }
            IndentStatusList = new ObservableCollection<string>();
            IndentStatusList.AddRange(AppConstants.INDENT_STATUS_LIST);
        }

        private void ClearForm()
        {
            SelectedStore = null;
            SelectedIndentStatus = null;
            FromDate = null;
            ToDate = null;
        }

        private async void LoadStoresAsync()
        {
            try
            {
                await Task.Run(async () =>
                {
                    var _result = await _storeService.GetStores();

                    if (_result != null && _result.Count() > 0)
                    {
                        Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            Stores = new ObservableCollection<Store>(_result.Where(x => x.Parent_ref == null));
                        });
                    }

                });

            }
            catch (Exception _ex)
            {

            }
        }

        private void ValidateSearch()
        {
            if (string.IsNullOrEmpty(FromDate) && string.IsNullOrEmpty(ToDate) && SelectedStore == null && string.IsNullOrEmpty(SelectedIndentStatus) && (!Isindentpaymentfull && !Isindentpaymentpartial))
            {
                _notificationService.ShowMessage("Search can not be empty", Common.NotificationType.Error);

                return;
            }

            if (string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                _notificationService.ShowMessage("Please enter from Date", Common.NotificationType.Error);
                return;

            }

            if (!string.IsNullOrEmpty(FromDate) && string.IsNullOrEmpty(ToDate))
            {
                _notificationService.ShowMessage("Please enter to Date", Common.NotificationType.Error);
                return;

            }

            DateTime? fromdate = null;
            DateTime? todate = null;
            //validation part 
            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                fromdate = Convert.ToDateTime(FromDate);
                todate = Convert.ToDateTime(ToDate);

                if (fromdate > todate)
                {
                    _notificationService.ShowMessage("From date should be less than or equal to To date", Common.NotificationType.Error);

                    return;
                }
            }

            IndentListSearch indentListSearch = new IndentListSearch();

            indentListSearch.FromDate = fromdate;
            indentListSearch.ToDate = todate;
            indentListSearch.Status = IndentOriginalStatus(SelectedIndentStatus);
            indentListSearch.StoreId = SelectedStore?.StoreId;
            if (Isindentpaymentfull)
                indentListSearch.PaymentMode = "full";
            else if (Isindentpaymentpartial)
                indentListSearch.PaymentMode = "partial";
            else
                indentListSearch.PaymentMode = null;


            _eventAggregator.GetEvent<IndentListFlyoutSearchDataEvent>().Publish(indentListSearch);
            IsOpen = false;
        }


        public string IndentOriginalStatus(String CurrentStatus)
        {
            if (!String.IsNullOrEmpty(CurrentStatus))
            {
                switch (CurrentStatus)
                {
                    case "Planned":
                        return IndentStatus.created.ToString();
                    case "Review":
                        return IndentStatus.review.ToString();
                    case "Approve":
                        return IndentStatus.approve.ToString();
                    case "Add Supplier":
                        return IndentStatus.addsupplier.ToString();
                    case "Placed":
                        return IndentStatus.placed.ToString();
                    case "InTransit":
                        return IndentStatus.intransit.ToString();
                    case "Received":
                        return IndentStatus.received.ToString();
                    case "FullPaid":
                        return IndentStatus.fullpaid.ToString();
                    case "Closed":
                        return IndentStatus.closed.ToString();
                    default:
                        return "";
                }

            }
            return null;

        }
    }

}