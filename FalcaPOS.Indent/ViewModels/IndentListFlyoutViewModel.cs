using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
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

        // Stores the PO number.
        private string _poNumber;

        // Gets and sets the PO Number
        public string PONumber
        {
            get { return _poNumber; }
            set { SetProperty(ref _poNumber, value); }
        }

        private bool _isStoreUser;

        public bool IsStoreUser
        {
            get => _isStoreUser;
            set => SetProperty(ref _isStoreUser, value);
        }


        private readonly IIndentService _indentService;

        
        public DelegateCommand CloseFlyOutIndentListCommand { get; set; }
        public DelegateCommand SearchFlyOutIndentListCommand { get; set; }

        public DelegateCommand StoreSelectionChangedCommand { get; set; }

        string pattern = string.Empty;
        public IndentListFlyoutViewModel(IEventAggregator eventAggregator, Logger logger, IIndentService indentService, IStoreService storeService, INotificationService notificationService)
        {
            

            Width = GridLength.Auto;
            Height = new GridLength(1200);//move to common folder

            Position = Position.Top;
            CloseFlyOutIndentListCommand = new DelegateCommand(() => { ClearForm(); });
            SearchFlyOutIndentListCommand = new DelegateCommand(ValidateSearch);

            StoreSelectionChangedCommand = new DelegateCommand(GetPONOSequenceFormatForStore);

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));
           
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _indentService = indentService ?? throw new ArgumentNullException();

            _eventAggregator.GetEvent<IndentListFlyoutOpenEvent>().Subscribe((x) =>
            {
                IsOpen = true;
            });

            // Checks if the the user login is SP or Backend, or Super Backend.
            IsStoreUser = AppConstants.USER_ROLES[0] == AppConstants.ROLE_STORE_PERSON || AppConstants.USER_ROLES[0] == AppConstants.ROLE_BACKEND || AppConstants.USER_ROLES[0] == AppConstants.ROLE_SUPER_BACKEND ? true : false;

            if (!AppConstants.USER_ROLES.Contains(AppConstants.ROLE_STORE_PERSON)
                 && !AppConstants.USER_ROLES.Contains(AppConstants.ROLE_BACKEND)
                 && !AppConstants.USER_ROLES.Contains(AppConstants.ROLE_SUPER_BACKEND)
                )
                LoadStoresAsync();
            else
            {
                Stores = new ObservableCollection<Store>() { AppConstants.LoggedInStoreInfo };
                GetPONOSequenceFormatForStore();
            }
            IndentStatusList = new ObservableCollection<string>();
            IndentStatusList.AddRange(AppConstants.INDENT_STATUS_LIST);
            _eventAggregator.GetEvent<MapStore>().Subscribe(x=>LoadStoresAsync());
        }

        // Fetches the PONO sequence format for the store
        private async void GetPONOSequenceFormatForStore()
        {
            try
            {
                PONumber = null;
                int StoreId = IsStoreUser == true? AppConstants.LoggedInStoreInfo.StoreId: SelectedStore != null? SelectedStore.StoreId : -1;
                if (StoreId != -1)
                {
                    await Task.Run(async () =>
                    {

                        var _result = await _indentService.GetPONOSequenceFormatForStore(StoreId);

                        Application.Current.Dispatcher?.Invoke(() =>
                        {
                            if (_result != null && _result.IsSuccess && _result.Data.IsValidString())
                            {
                                PONumber = _result.Data;
                                pattern = PONumber;
                            }
                        });

                    });
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        // Clears the search inputs on clicking reset.
        private void ClearForm()
        {
            SelectedStore = null;
            SelectedIndentStatus = null;
            FromDate = null;
            ToDate = null;
            if (IsStoreUser && PONumber != null)
            {
                PONumber = PONumber.Contains("/") ? PONumber.Substring(0, PONumber.IndexOf("/") + 1) : PONumber;
            }
            else
            {
                PONumber = null;
            }
            Isindentpaymentfull = false;
            Isindentpaymentpartial = false;
        }

        private async void LoadStoresAsync()
        {
            try
            {
                await Task.Run(async () =>
                {
                    var _result = await _storeService.GetStoreDetailsbyuser(AppConstants.UserId, AppConstants.USER_ROLES[0]);
                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        Stores = new ObservableCollection<Store>(_result.OrderBy(x => x.Name));
                        
                    });
                
                    //var _result = await _storeService.GetStores();

                    //if (_result != null && _result.Count() > 0)
                    //{
                    //    Application.Current?.Dispatcher?.Invoke(() =>
                    //    {
                    //        if (AppConstants.ROLE_TERRITORY_MANAGER == AppConstants.USER_ROLES[0] || AppConstants.ROLE_REGIONAL_MANAGER == AppConstants.USER_ROLES[0]) {

                    //            _result = _result.Where(x => x.Parent_ref == null);
                    //            var _storeMap = new List<Store>();
                    //            foreach (var item in _result) {

                    //                if (item.User_ref.FirstOrDefault(x => x.Value == AppConstants.UserId) != null)
                    //                    _storeMap.Add(item);

                    //            }
                    //            Stores = new ObservableCollection<Store>(_storeMap);
                    //        }
                    //        else
                    //        Stores = new ObservableCollection<Store>(_result.Where(x => x.Parent_ref == null));
                    //    });
                    //}

                });

            }
            catch (Exception)
            {

            }
        }

        private void ValidateSearch()
        {
            string finalPONumber = null;
            if ((string.IsNullOrEmpty(PONumber) || string.Equals(PONumber,pattern))  && string.IsNullOrEmpty(FromDate) && string.IsNullOrEmpty(ToDate) && SelectedStore == null && string.IsNullOrEmpty(SelectedIndentStatus) && (!Isindentpaymentfull && !Isindentpaymentpartial))
            {
                _notificationService.ShowMessage("Search can not be empty", Common.NotificationType.Error);

                return;
            }

            
            // PO/FS && PO/FS/abx1/
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

            if (((Isindentpaymentpartial)||(Isindentpaymentfull)||!(string.IsNullOrEmpty(FromDate))||!(string.IsNullOrEmpty(ToDate))||(!string.IsNullOrEmpty(SelectedIndentStatus)))&& (AppConstants.ROLE_TERRITORY_MANAGER == AppConstants.USER_ROLES[0] || AppConstants.ROLE_REGIONAL_MANAGER == AppConstants.USER_ROLES[0])&& SelectedStore==null) {

                _notificationService.ShowMessage("Please select store", Common.NotificationType.Error);

                return;
            }

            if (!string.IsNullOrEmpty(PONumber) && !string.Equals(PONumber, pattern))
            {
                int indexOfSlash = PONumber.IndexOf('/');
                string result = (indexOfSlash != -1 && indexOfSlash < PONumber.Length - 1) ? PONumber.Substring(indexOfSlash + 1) : null;
                finalPONumber = result != null ? $"PO/FS/{PONumber}" : null;



                if (string.IsNullOrEmpty(finalPONumber))
                {
                    _notificationService.ShowMessage("Invalid PO number format", Common.NotificationType.Error);
                    return;
                }
            }


            IndentListSearch indentListSearch = new IndentListSearch();


            indentListSearch.PONumber = finalPONumber;       
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
                    case "Sahaya Pending":
                        return IndentStatus.sahayapending.ToString();
                    case "Cancelled":
                        return IndentStatus.cancelled.ToString();
                    default:
                        return "";
                }

            }
            return null;

        }
    }

}