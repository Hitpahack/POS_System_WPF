using FalcaPOS.Common;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Denomination.View;
using FalcaPOS.Entites.Denomination;
using FalcaPOS.Entites.Stores;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FalcaPOS.Denomination.ViewModel
{
    public class NewDenominationFinanceViewModel : BindableBase
    {
        private readonly INotificationService _notificationService;
        private readonly IStoreService _storeService;
        private readonly ProgressService _ProgressService;
        private readonly Logger _logger;

        public DelegateCommand<object> FetchDenominationCommand { get; private set; }

        public DelegateCommand<object> ResetDenominationCommand { get; private set; }

        private readonly IDenominationService _denominationService;

        public DelegateCommand<object> DownloadDenominationCommand { get; private set; }

        public DelegateCommand<object> DenominationExportCommand { get; private set; }
        public DelegateCommand<object> NoteDetailsViewCommand { get; private set; }

        private readonly ICommonService _commonService;
        public NewDenominationFinanceViewModel(ICommonService commonService, IDenominationService denominationService, IStoreService storeService, INotificationService notificationService, ProgressService ProgressService, Logger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _ProgressService = ProgressService;

            FetchDenominationCommand = new DelegateCommand<object>(FetchDenominationDetails);

            _denominationService = denominationService ?? throw new ArgumentNullException(nameof(denominationService));

            ResetDenominationCommand = new DelegateCommand<object>(Reset);

            LoadStores();

            DownloadDenominationCommand = new DelegateCommand<object>(DownloadAttachement);

            DenominationExportCommand = new DelegateCommand<object>(ExportDownload);

            IsExportEnabled = false;

            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));

            NoteDetailsViewCommand = new DelegateCommand<object>(NoteDetailsView);

        }


        private async void LoadStores()
        {

            try
            {
                await Task.Run(async () =>
                {
                    var _result = await _storeService.GetStores();
                    if (_result != null)
                    {
                        //_result.Where(x => x.StoreId == 1).Select(x => { x.Name = "All"; x.StoreId = -1; return x; }).ToList();
                        Stores = new ObservableCollection<Store>(_result.Where(x => x.Parent_ref == null && x.StoreId!=1));
                        Stores.Insert(0, new Store() { StoreId = -1, Name = "All" });
                        Stores.Insert(1, new Store() { StoreId = -2, Name = "All F-Shop" });
                        Stores.Insert(2, new Store() { StoreId = -3, Name = "All RSP" });
                    }
                });
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }

        }

        public void Reset(object data)
        {
            try
            {

                SelectedStore = null;
                FromDate = null;
                ToDate = null;
                DenominationModel = null;
                IsExportEnabled = false;

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }
        public async void FetchDenominationDetails(object data)
        {
            try
            {
                if (data != null)
                {

                    if (SelectedStore == null)
                    {
                        _notificationService.ShowMessage("Please select store", Common.NotificationType.Error);

                        return;


                    }
                    if (string.IsNullOrEmpty(FromDate))
                    {
                        _notificationService.ShowMessage("Please select from date", Common.NotificationType.Error);

                        return;


                    }
                    if (string.IsNullOrEmpty(ToDate))
                    {
                        _notificationService.ShowMessage("Please select to date", Common.NotificationType.Error);

                        return;


                    }

                    if (FromDate != null && ToDate != null)
                    {

                        if (Convert.ToDateTime(ToDate) < Convert.ToDateTime(FromDate))
                        {
                            _notificationService.ShowMessage("To Date can not be less  than From Date", Common.NotificationType.Error);
                            return;
                        }
                    }

                    if (data != null)
                    {
                        ResetTelerikGridFilters.ClearTelerikGridViewFilters(data);
                    }

                    await Task.Run(async () =>
                    {

                        var _result = await _denominationService.GetDenominationSearch(SelectedStore.StoreId, FromDate, ToDate);

                        if (_result != null && _result.IsSuccess)
                        {
                            List<DenominationModel> list = new List<DenominationModel>();
                            foreach (var item in _result.Data)
                            {
                                var _noteModel = JsonConvert.DeserializeObject<NotesModel>(item.NoteDetails);

                                var _viewDenoniation = new DenominationModel()
                                {
                                    Store = item.Store,
                                    User = item.User,
                                    Cash = item.Cash,
                                    UPI = item.UPI,
                                    OpeningCash = item.OpeningCash,
                                    TotalSales = item.TotalSales,
                                    AvailableCash = (item.Cash + item.OpeningCash),
                                    Credit = item.Credit,
                                    Deposit = item.Deposit,
                                    SalesReturnCash = item.SalesReturnCash,
                                    ClosingCash = ((item.Cash + item.OpeningCash) - (item.Deposit + item.SalesReturnCash)),
                                    DenominationDate = item.DenominationDate,
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
                                list.Add(_viewDenoniation);
                            }

                            DenominationModel = new ObservableCollection<DenominationModel>(list);
                            IsExportEnabled = true;

                        }
                        else
                        {
                            _notificationService.ShowMessage("No data found", Common.NotificationType.Error);
                            DenominationModel = null;
                        }



                    });
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in Denomination view model ", _ex);
            }
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
            get => _selectedStore;
            set => SetProperty(ref _selectedStore, value);
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


        private ObservableCollection<DenominationModel> _denominationmodel;
        public ObservableCollection<DenominationModel> DenominationModel
        {
            get => _denominationmodel;
            set => SetProperty(ref _denominationmodel, value);
        }



        public async void DownloadAttachement(object obj)
        {
            try
            {
                var viewmodel = ((NewDenominationFinanceViewModel)obj);
                if (viewmodel != null)
                {
                    var _result = await _denominationService.DownloadFile("" + "_" + "");

                    if (_result != null)
                    {
                        if (_result != null && _result.IsSuccess && _result.Data != null && _result.Data.FileStream != null)
                        {
                            SaveFileDialog sd = new SaveFileDialog
                            {
                                CreatePrompt = true,
                                OverwritePrompt = true,
                                DefaultExt = "zip",
                            };
                            var _saveFile = sd.ShowDialog();

                            if (_saveFile == true && sd.FileName.IsValidString())
                            {
                                sd.FileName = Path.ChangeExtension(sd.FileName, "zip");

                                File.WriteAllBytes(sd.FileName, _result.Data.FileStream);
                            }
                        }
                        else
                        {
                            _notificationService.ShowMessage(_result?.Error ?? "An error occurred, try again", Common.NotificationType.Error);
                        }
                    }
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);

            }
        }

        public void ExportDownload(object obj)
        {
            try
            {
                if (DenominationModel == null || !DenominationModel.Any())
                {
                    _notificationService.ShowMessage("No records found to export", Common.NotificationType.Error);

                    return;
                }
                List<DenominationExport> denomiantionExport = new List<DenominationExport>();

                if (DenominationModel != null && DenominationModel.Count > 0)
                {
                    foreach (var item in DenominationModel)
                    {
                        denomiantionExport.Add(new DenominationExport()
                        {
                            DenominationDate = item.DenominationDate,
                            Store = item.Store,
                            OpeningCash = item.OpeningCash,
                            Cash = item.Cash,
                            SalesReturnCash = item.SalesReturnCash,

                            UPI = item.UPI,
                            TotalSales = item.TotalSales,
                            Deposit = item.Deposit,
                            ClosingCash = item.ClosingCash,
                        });
                    }

                }
                if (denomiantionExport == null || !denomiantionExport.Any())
                {
                    _notificationService.ShowMessage("No records found to export", Common.NotificationType.Error);

                    return;
                }

                IsExportEnabled = false;

                _fileName = DateTime.Now.Date.ToString("dd-MM-yyyy") + "denomination";

                bool _result = _commonService.ExportToXL(denomiantionExport, _fileName, skipfileName: false, "denomination", ApplicationSettings.DENOMIATIONPORTS_PATH);

                if (_result)
                {
                    _notificationService.ShowMessage("File exported to folder C:\\FALCAPOS\\DenominationReport", Common.NotificationType.Success);
                }
                else
                {
                    _notificationService.ShowMessage("File export failed , try again", Common.NotificationType.Error);
                }

                IsExportEnabled = true;

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);

                _notificationService.ShowMessage(_ex.Message, Common.NotificationType.Error);
            }
        }

        private bool _isExportEnabled;

        public bool IsExportEnabled
        {
            get => _isExportEnabled;
            set => SetProperty(ref _isExportEnabled, value);
        }

        public string _fileName { get; set; }

        private DenominationModel _noteDetails;
        public DenominationModel NoteDetails
        {
            get => _noteDetails;
            set => SetProperty(ref _noteDetails, value);
        }

        public async void NoteDetailsView(object obj)
        {
            try
            {
                var _view = (DenominationModel)obj;
                if (_view != null)
                {
                    NoteDetails noteDetails = new NoteDetails();
                    noteDetails.DataContext = this;
                    NoteDetails = _view;
                    await DialogHost.Show(noteDetails, "RootDialog", NotePopupEventHandler);
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private void NotePopupEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {



            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);
            }
            finally
            {

            }
        }

    }

    public class DenominationExport
    {
        public string DenominationDate { get; set; }

        public string Store { get; set; }


        public float OpeningCash { get; set; }

        [DisplayName("Cash Sale")]
        public float Cash { get; set; }

        [DisplayName("Cash Return")]
        public float SalesReturnCash { get; set; }

        //[DisplayName("Credit Sale")]
        //public float Credit { get; set; }

        [DisplayName("UPI Sale")]
        public float UPI { get; set; }

        public float TotalSales { get; set; }

        [DisplayName("Bank Deposit")]
        public float Deposit { get; set; }
        public float ClosingCash { get; set; }

    }
}
