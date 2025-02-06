using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Denomination;
using FalcaPOS.Entites.Stores;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace FalcaPOS.Denomination.ViewModel
{
    public class DenominationFinanceViewModel : BindableBase
    {

        private readonly INotificationService _notificationService;
        private readonly IStoreService _storeService;
        private readonly ProgressService _ProgressService;
        private readonly Logger _logger;

        private int DenominationDays { get { return Convert.ToInt32(ConfigurationManager.AppSettings["DenominationDays"]); } }
        public DelegateCommand<object> FetchDenominationCommand { get; private set; }

        private readonly IDenominationService _denominationService;
        public DenominationFinanceViewModel(IDenominationService denominationService, IStoreService storeService, INotificationService notificationService, ProgressService ProgressService, Logger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _ProgressService = ProgressService;
            FetchDenominationCommand = new DelegateCommand<object>(FetchDenominationDetails);
            _denominationService = denominationService ?? throw new ArgumentNullException(nameof(denominationService));
            LoadData();
            LoadStores();


        }

        public void LoadData()
        {
            try
            {
                List<DateTime> dates = new List<DateTime>();

                for (int i = 0; i < DenominationDays; i++)
                {
                    dates.Add(i == 0 ? DateTime.Now.Date : DateTime.Now.AddDays(-i).Date);
                }
                List<Expander> Items = new List<Expander>();
                List<DateTime> datesorderby = new List<DateTime>();
                datesorderby = dates.Select(d => new
                {
                    d.Date,
                    d.Month,
                    FormattedDate = d
                })
                .OrderBy(d => d.Date)
                .OrderBy(d => d.Month)
                .Select(d => d.FormattedDate).ToList();

                foreach (var item in datesorderby)
                {
                    Expander expander = new Expander
                    {
                        Date = item.Date.ToString("dd-MM-yyyy"),

                    };
                    Items.Add(expander);

                }
                ListItems = Items;
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in getting finance page intital load ", _ex);
            }

        }

        private async void LoadStores()
        {

            try
            {
                await Task.Run(async () =>
                {
                    var _result = await _storeService.GetStores();
                    if (_result != null)
                        Stores = new ObservableCollection<Store>(_result.Where(x => x.Parent_ref == null));
                });
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in Denomination view model ", _ex);
            }

        }

        public async void FetchDenominationDetails(object data)
        {
            try
            {
                if (data != null)
                {

                    var model = data as Expander;

                    if (SelectedStore == null)
                    {
                        _notificationService.ShowMessage("Please select store", Common.NotificationType.Error);
                        model.IsExpand = false;
                        return;


                    }

                    await Task.Run(async () =>
                    {
                        model.IsExpand = false;

                        DenominationSearchModel denominationSearchModel = new DenominationSearchModel()
                        {
                            Store = SelectedStore.Name,
                            OnDate = Convert.ToDateTime(model.Date),
                            IsSameDate = true

                        };

                        var _result = await _denominationService.GetDenomination(denominationSearchModel);

                        if (_result != null && _result.IsSuccess)
                        {
                            var _noteModel = JsonConvert.DeserializeObject<NotesModel>(_result.Data.NoteDetails);

                            var _viewDenoniation = new DenominationModel()
                            {
                                Store = _result.Data.Store,
                                User = _result.Data.User,
                                Cash = _result.Data.Cash,
                                UPI = _result.Data.UPI,
                                OpeningCash = _result.Data.OpeningCash,
                                TotalSales = _result.Data.TotalSales,
                                AvailableCash = (_result.Data.Cash + _result.Data.OpeningCash),
                                Credit = _result.Data.Credit,
                                Deposit = _result.Data.Deposit,
                                SalesReturnCash = _result.Data.SalesReturnCash,
                                ClosingCash = ((_result.Data.Cash + _result.Data.OpeningCash) - _result.Data.Deposit),
                                DenominationDate = _result.Data.DenominationDate,
                                notes_2000 = _noteModel.notes_2000,
                                notes_500 = _noteModel.notes_500,
                                notes_200 = _noteModel.notes_200,
                                notes_100 = _noteModel.notes_100,
                                notes_20 = _noteModel.notes_20,
                                notes_10 = _noteModel.notes_10,
                                notes_50 = _noteModel.notes_50,
                                Coins = _noteModel.Coins,
                                Total = (float)_noteModel.Total,

                            };

                            model.DenominationModel = _viewDenoniation;
                            model.DenominationModel.IsRole = AppConstants.USER_ROLES[0].ToString() == AppConstants.ROLE_STORE_PERSON ? true : false;
                            model.IsExpand = true;

                        }
                        else
                        {
                            model.IsExpand = false;
                            _notificationService.ShowMessage("No data found", Common.NotificationType.Error);
                            model.DenominationModel = null;

                        }



                    });
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in Denomination view model ", _ex);
            }
        }

        public void closeExpander()
        {
            try
            {
                if (ListItems != null && ListItems.Count > 0)
                {
                    foreach (var item in ListItems)
                    {
                        item.IsExpand = false;
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in getting finance page close expander ", ex);
            }
        }

        private List<Expander> _ListItems = new List<Expander>();

        public List<Expander> ListItems
        {
            get { return _ListItems; }
            set { SetProperty(ref _ListItems, value); }
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
            get
            {

                return _selectedStore;

            }
            set
            {
                SetProperty(ref _selectedStore, value);
                closeExpander();
            }
        }


    }


    public class Expander : BindableBase
    {

        private string _date;

        public string Date
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
        }

        private DenominationModel denominationfinance;

        public DenominationModel DenominationModel
        {
            get { return denominationfinance; }
            set { SetProperty(ref denominationfinance, value); }
        }

        private bool _isexpand;

        public bool IsExpand
        {
            get { return _isexpand; }
            set { SetProperty(ref _isexpand, value); }
        }

    }

}
