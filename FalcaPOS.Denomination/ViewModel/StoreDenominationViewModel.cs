using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Denomination.View;
using FalcaPOS.Entites.Denomination;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FalcaPOS.Denomination.ViewModel
{
    public class StoreDenominationViewModel : BindableBase
    {

        private readonly INotificationService _notificationService;
        private readonly IStoreService _storeService;
        private readonly ProgressService _ProgressService;
        private readonly Logger _logger;

        public DelegateCommand<object> FetchDenominationCommand { get; private set; }

        public DelegateCommand<object> DownloadDenominationCommand { get; private set; }

        public DelegateCommand<object> ResetDenominationCommand { get; private set; }

        public DelegateCommand<object> NoteDetailsViewCommand { get; private set; }

        public DelegateCommand<object> DenominationStoreExportCommand { get; private set; }

        private readonly IDenominationService _denominationService;

        private readonly ICommonService _commonService;
        public StoreDenominationViewModel(ICommonService commonService, IDenominationService denominationService, IStoreService storeService, INotificationService notificationService, ProgressService ProgressService, Logger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _ProgressService = ProgressService;

            FetchDenominationCommand = new DelegateCommand<object>(FetchDenominationDetails);

            _denominationService = denominationService ?? throw new ArgumentNullException(nameof(denominationService));

            ResetDenominationCommand = new DelegateCommand<object>(Reset);

            DownloadDenominationCommand = new DelegateCommand<object>(DownloadAttachment);

            NoteDetailsViewCommand = new DelegateCommand<object>(NoteDetailsView);

            DenominationStoreExportCommand = new DelegateCommand<object>(ExportStore);

            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));


        }

        public void Reset(object data)
        {
            try
            {
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

        public async void DownloadAttachment(object obj)
        {
            try
            {
                var _viewModel = ((StoreDenominationViewModel)obj);
                if (_viewModel != null)
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
        public async void FetchDenominationDetails(object data)
        {
            try
            {
                if (data != null)
                {
                    ResetTelerikGridFilters.ClearTelerikGridViewFilters(data);

                    if (AppConstants.StoreName == null)
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

                    await Task.Run(async () =>
                    {

                        var _result = await _denominationService.GetDenominationSearch(AppConstants.LoggedInStoreInfo.StoreId, FromDate, ToDate);

                        if (_result != null && _result.IsSuccess)
                        {
                            List<DenominationModel> denmination = new List<DenominationModel>();

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

                                denmination.Add(_viewDenoniation);
                            };
                            DenominationModel = new ObservableCollection<DenominationModel>(denmination);
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



        private ObservableCollection<DenominationModel> _denominationModel;
        public ObservableCollection<DenominationModel> DenominationModel
        {
            get => _denominationModel;
            set => SetProperty(ref _denominationModel, value);
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

        public void ExportStore(object obj)
        {
            try
            {
                if (DenominationModel == null || !DenominationModel.Any())
                {
                    _notificationService.ShowMessage("No records found to export", Common.NotificationType.Error);

                    return;
                }
                List<DenominationExport> denominationExport = new List<DenominationExport>();

                if (DenominationModel != null && DenominationModel.Count > 0)
                {
                    foreach (var item in DenominationModel)
                    {
                        denominationExport.Add(new DenominationExport()
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
                if (denominationExport == null || !denominationExport.Any())
                {
                    _notificationService.ShowMessage("No records found to export", Common.NotificationType.Error);

                    return; 
                }

                IsExportEnabled = false;

                //_fileName = DateTime.Now.Date.ToString("dd-MM-yyyy") + "denomination";

                bool _result = _commonService.ExportToXL(denominationExport, "Denomination", skipfileName: false, "Denomination", ApplicationSettings.DENOMIATIONPORTS_PATH);

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
            }
        }

        private bool _isExportEnabled;

        public bool IsExportEnabled
        {
            get => _isExportEnabled;
            set => SetProperty(ref _isExportEnabled, value);
        }

        public string _fileName { get; set; }
    }
}
