using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Denomination.View;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.Denomination;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FalcaPOS.Denomination.ViewModel
{
    public class DenominationViewModel : BindableBase
    {

        public DelegateCommand SubmitCommand { get; private set; }

        public DelegateCommand ClearCommand { get; private set; }
        public DelegateCommand RefreshTodaySalesCommand { get; private set; }

        public DelegateCommand<object> DepositeAttanchemntCommad { get; private set; }
        public DelegateCommand<object> EnterTxtChangeCount2000Command { get; private set; }

        public DelegateCommand<object> EnterTxtChangeCount500Command { get; private set; }
        public DelegateCommand<object> EnterTxtChangeCount200Commands { get; private set; }
        public DelegateCommand<object> EnterTxtChangeCount100Command { get; private set; }
        public DelegateCommand<object> EnterTxtChangeCount50Command { get; private set; }
        public DelegateCommand<object> EnterTxtChangeCount20Command { get; private set; }
        public DelegateCommand<object> EnterTxtChangeCount10Command { get; private set; }
        public DelegateCommand<object> EnterTxtChangeCountCoinsCommand { get; private set; }

        public DelegateCommand<object> EnterOpeningCashBalanceCommand { get; private set; }

        public DelegateCommand<object> EnterDayDepostiCommand { get; private set; }

        public DelegateCommand<object> EnterDaySalesReturnCashCommand { get; private set; }

        private readonly IDenominationService _denominationService;
        private readonly INotificationService _notificationService;
        private readonly ProgressService _progressService;
        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;
        private readonly Logger _logger;

        public DelegateCommand<object> DeleteUploadFileCommand { get; private set; }


        public string DenominationDate { get; set; }
        public DelegateCommand<object> AddDepositCommad { get; private set; }
        public DelegateCommand<object> AddCashDepositCommad { get; private set; }
        public DelegateCommand<object> BankNameChangeCommand { get; private set; }


        public DelegateCommand<object> CashDepositeAttanchemntCommad { get; private set; }

        public DelegateCommand<object> ResetCommand { get; private set; }

        public DenominationViewModel(Logger logger, IEventAggregator eventAggregator, IDenominationService denominationService, IDialogService dialogService, INotificationService notificationService, ProgressService progressService)
        {
            SubmitCommand = new DelegateCommand(Submit);
            EnterTxtChangeCount2000Command = new DelegateCommand<object>(Caluclation2000);
            EnterTxtChangeCount500Command = new DelegateCommand<object>(Caluclation500);
            EnterTxtChangeCount200Commands = new DelegateCommand<object>(Caluclation200);
            EnterTxtChangeCount100Command = new DelegateCommand<object>(Caluclation100);
            EnterTxtChangeCount50Command = new DelegateCommand<object>(Caluclation50);
            EnterTxtChangeCount20Command = new DelegateCommand<object>(Caluclation20);
            EnterTxtChangeCount10Command = new DelegateCommand<object>(Caluclation10);
            EnterTxtChangeCountCoinsCommand = new DelegateCommand<object>(CaluclationCoins);
            EnterOpeningCashBalanceCommand = new DelegateCommand<object>(OpeningCashBalanceCal);
            EnterDayDepostiCommand = new DelegateCommand<object>(DayDepositeCal);
            _denominationService = denominationService ?? throw new ArgumentNullException(nameof(denominationService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _progressService = progressService;
            ClearCommand = new DelegateCommand(reset);

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            _eventAggregator.GetEvent<DenominationPageRefreshEvent>().Subscribe(() => LoadTodaySales(true));
            RefreshTodaySalesCommand = new DelegateCommand(() => PreviousdayDenominationVerify());
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            DenominationModel.IsRole = AppConstants.USER_ROLES[0].ToString() == AppConstants.ROLE_STORE_PERSON ? true : false;

            EnterDaySalesReturnCashCommand = new DelegateCommand<object>(SalesReturnCashCal);

            double userheightscreen = System.Windows.SystemParameters.PrimaryScreenHeight;
            MaxHeight = (userheightscreen - 80);

            DepositeAttanchemntCommad = new DelegateCommand<object>(AddFileOpenDialog);

            DeleteUploadFileCommand = new DelegateCommand<object>(DeleteUploadFile);

            AddDepositCommad = new DelegateCommand<object>(AddDepositPopup);

            AddCashDepositCommad = new DelegateCommand<object>(AddDeposit);

            BankNameChangeCommand = new DelegateCommand<object>(BankNameChange);

            CashDepositeAttanchemntCommad = new DelegateCommand<object>(AddFileOpenDialog);
            LoadBanlList();
            ResetCommand = new DelegateCommand<object>(Reset);

            _depositModel = new DepositModel();


            PreviousdayDenominationVerify();

        }

        private void SalesReturnCashCal(object param)
        {
            try
            {
                var daysalesretuncash = Convert.ToDecimal(((TextBox)param).Text);
                DenominationModel.SalesReturnCash = (float)daysalesretuncash;
                if (DenominationModel.SalesReturnCash <= DenominationModel.AvailableCash && DenominationModel.SalesReturnCash <= DenominationModel.ClosingCash)
                {
                    ClosingBalanceCalculation();
                }
                else
                {
                    DenominationModel.SalesReturnCash = 0;
                    DenominationModel.Deposit = 0;
                    DenominationModel.ClosingCash = DenominationModel.AvailableCash;
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in Denomination view model ", _ex);
            }
        }
        private void DayDepositeCal(object param)
        {
            try
            {
                var daydeposit = Convert.ToDecimal(((TextBox)param).Text);
                DenominationModel.Deposit = (float)daydeposit;
                if (DenominationModel.Deposit <= DenominationModel.AvailableCash)
                {
                    ClosingBalanceCalculation();
                }
                else
                {
                    DenominationModel.Deposit = 0;
                    DenominationModel.SalesReturnCash = 0;
                    DenominationModel.ClosingCash = DenominationModel.AvailableCash;
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in Denomination view model ", _ex);
            }
        }
        private void OpeningCashBalanceCal(object param)
        {
            try
            {
                decimal opening = Convert.ToDecimal(((TextBox)param).Text);
                DenominationModel.OpeningCash = (float)opening;
                ClosingBalanceCalculation();
            }
            catch (Exception)
            {

            }
        }

        private void Caluclation2000(object parm)
        {
            try
            {
                int count = Convert.ToInt32(((TextBox)parm).Text);
                DenominationModel.Totalin2000 = 2000 * count;

                DenominationCalculation();


            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in Denomination view model ", _ex);
            }

        }

        private void Caluclation500(object parm)
        {
            try
            {
                int count = Convert.ToInt32(((TextBox)parm).Text);

                DenominationModel.Totalin500 = 500 * count;
                DenominationModel.notes_500 = count;
                DenominationCalculation();

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in Denomination view model ", _ex);
            }

        }
        private void Caluclation200(object parm)
        {
            try
            {
                int count = Convert.ToInt32(((TextBox)parm).Text);

                DenominationModel.Totalin200 = 200 * count;
                DenominationModel.notes_200 = count;
                DenominationCalculation();

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in Denomination view model ", _ex);
            }

        }
        private void Caluclation100(object parm)
        {
            try
            {
                int count = Convert.ToInt32(((TextBox)parm).Text);

                DenominationModel.Totalin100 = 100 * count;
                DenominationModel.notes_100 = count;
                DenominationCalculation();

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in Denomination view model ", _ex);
            }

        }
        private void Caluclation50(object parm)
        {
            try
            {
                int count = Convert.ToInt32(((TextBox)parm).Text);

                DenominationModel.Totalin50 = 50 * count;
                DenominationModel.notes_50 = count;
                DenominationCalculation();

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in Denomination view model ", _ex);
            }

        }
        private void Caluclation20(object parm)
        {
            try
            {
                int count = Convert.ToInt32(((TextBox)parm).Text);

                DenominationModel.Totalin20 = 20 * count;
                DenominationModel.notes_20 = count;
                DenominationCalculation();

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in Denomination view model ", _ex);
            }

        }
        private void Caluclation10(object parm)
        {
            try
            {
                int count = Convert.ToInt32(((TextBox)parm).Text);

                DenominationModel.Totalin10 = 10 * count;
                DenominationModel.notes_10 = count;
                DenominationCalculation();
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in Denomination view model ", _ex);
            }

        }
        private void CaluclationCoins(object parm)
        {
            try
            {
                int count = Convert.ToInt32(((TextBox)parm).Text);

                DenominationModel.TotalinCoins = count;
                DenominationModel.Coins = count;
                DenominationCalculation();
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in Denomination view model ", _ex);
            }

        }

        public void DenominationCalculation()
        {
            try
            {
                var total = DenominationModel.Totalin2000 + DenominationModel.Totalin500 + DenominationModel.Totalin200 + DenominationModel.Totalin100 + DenominationModel.Totalin50 + DenominationModel.Totalin20 + DenominationModel.Totalin10 + DenominationModel.TotalinCoins;
                DenominationModel.Total = (float)total;
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in Denomination view model ", _ex);
            }
        }

        public void ClosingBalanceCalculation()
        {
            try
            {
                var balance = DenominationModel.OpeningCash + DenominationModel.Cash - DenominationModel.Deposit;
                DenominationModel.ClosingCash = balance - DenominationModel.SalesReturnCash;
                DenominationModel.AvailableCash = (DenominationModel.OpeningCash + DenominationModel.Cash);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in getting closing balance calculation ", ex);
            }

        }

        public async void LoadPreviousClosingBalance(bool IsPredayDenoSuccess, DateTime? dateTime = null)
        {
            try
            {
                await Task.Run((async () =>
                {
                    var preDaysCBalance = await _denominationService.GetDenomination(new DenominationSearchModel() { OnDate = IsPredayDenoSuccess == true ? DateTime.Now.AddDays(-1) : dateTime.Value.Date.AddDays(-1), StoreId = AppConstants.LoggedInStoreInfo.StoreId }, System.Threading.CancellationToken.None);
                    if (preDaysCBalance.IsSuccess && preDaysCBalance.Data != null)
                    {
                        DenominationModel.OpeningCash = ((preDaysCBalance.Data.OpeningCash + preDaysCBalance.Data.Cash) - (preDaysCBalance.Data.Deposit + preDaysCBalance.Data.SalesReturnCash));
                        DenominationModel.IsOpeningCash = false;
                        ClosingBalanceCalculation();
                    }
                    else
                    {
                        DenominationModel.IsOpeningCash = true;
                    }
                }));
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in Denomination view model ", _ex);
            }
        }

        public async void LoadTodaySales(bool IsPreDenoSuccess = false, string dateTime = null)
        {
            try
            {
                await Task.Run(async () =>
                {

                    var _result = await _denominationService.GetTodaySales(IsPreDenoSuccess, dateTime);

                    if (_result != null && _result.IsSuccess)
                    {
                        DenominationModel.TotalSales = _result.Data.TotalSales;
                        DenominationModel.Cash = _result.Data.Cash;
                        DenominationModel.UPI = _result.Data.UPI;
                        DenominationModel.Credit = _result.Data.Credit;
                        DenominationModel.Deposit = _result.Data.Deposit;
                        ClosingBalanceCalculation();
                        if (DenominationModel.OpeningCash == 0) DenominationModel.IsOpeningCash = true;

                    }
                });
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in Denomination view model ", _ex);

            }

        }
        public async void Submit()
        {
            try
            {
                //basic validation

                if (DenominationModel.ClosingCash != DenominationModel.Total)
                {
                    _notificationService.ShowMessage("Closing cash balance not equal to sum of denomination total !!", Common.NotificationType.Error);
                    return;
                }

                await _progressService.StartProgressAsync();
                await Task.Run(async () =>
                {
                    var notes = new NotesModel()
                    {
                        notes_2000 = DenominationModel.notes_2000,
                        notes_500 = DenominationModel.notes_500,
                        notes_200 = DenominationModel.notes_200,
                        notes_100 = DenominationModel.notes_100,
                        notes_50 = DenominationModel.notes_50,
                        notes_20 = DenominationModel.notes_20,
                        notes_10 = DenominationModel.notes_10,
                        Coins = DenominationModel.Coins,
                        Total = (int)DenominationModel.Total,
                    };
                    var _adddenomination = new AddDenominationModel()
                    {
                        Store = AppConstants.StoreName,
                        User = AppConstants.UserName,
                        DenominationDate = DenominationDate,
                        OpeningCash = DenominationModel.OpeningCash,
                        TotalSales = DenominationModel.TotalSales,
                        Cash = DenominationModel.Cash,
                        UPI = DenominationModel.UPI,
                        Credit = DenominationModel.Credit,
                        Deposit = DenominationModel.Deposit,
                        SalesReturnCash = (int)DenominationModel.SalesReturnCash,
                        NoteDetails = JsonConvert.SerializeObject(notes),



                    };

                    var _result = await _denominationService.AddDenomination(_adddenomination);

                    if (_result != null && _result.IsSuccess)
                    {

                        _logger.LogInformation("Denomination added successfully");
                        _notificationService.ShowMessage("Denomination added successfully", Common.NotificationType.Success);

                        reset();
                        if (!string.IsNullOrEmpty(DenominationDate))
                            PreviousdayDenominationVerify();


                    }
                    else
                    {

                        _logger.LogError(_result.Error);
                        _notificationService.ShowMessage(_result.Error, Common.NotificationType.Error);
                    }


                });

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in Denomination view model ", _ex);
            }
            finally
            {
                await _progressService.StopProgressAsync();
            }
        }


        public void reset()
        {
            try
            {
                DenominationModel.notes_500 = 0;
                DenominationModel.notes_2000 = 0;
                DenominationModel.notes_200 = 0;
                DenominationModel.notes_100 = 0;
                DenominationModel.notes_50 = 0;
                DenominationModel.notes_20 = 0;
                DenominationModel.notes_10 = 0;
                DenominationModel.Coins = 0;
                DenominationModel.Total = 0;
                DenominationModel.Totalin2000 = 0;
                DenominationModel.Totalin500 = 0;
                DenominationModel.Totalin200 = 0;
                DenominationModel.Totalin100 = 0;
                DenominationModel.Totalin50 = 0;
                DenominationModel.Totalin20 = 0;
                DenominationModel.TotalinCoins = 0;
                DenominationModel.Totalin10 = 0;
                if (DenominationModel.IsOpeningCash)
                    DenominationModel.OpeningCash = 0;
                ClosingBalanceCalculation();

                DenominationModel.SalesReturnCash = 0;
                DenominationModel.FileUploadListInfo = null;


            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in Denomination view model ", _ex);
            }
        }
        private DenominationModel _denominationModel = new DenominationModel();

        public DenominationModel DenominationModel
        {
            get { return _denominationModel; }
            set { SetProperty(ref _denominationModel, value); }
        }


        private DenominationAmountModel _denominationTotal = new DenominationAmountModel();

        public DenominationAmountModel DenominationTotal
        {
            get { return _denominationTotal; }
            set { SetProperty(ref _denominationTotal, value); }
        }

        private double _maxHeight;

        public double MaxHeight
        {
            get { return _maxHeight; }
            set { SetProperty(ref _maxHeight, value); }
        }
        //public void AddFileOpenDialog(object file)
        //{
        //    try
        //    {
        //        var viewmodel = ((DenominationViewModel)file);

        //        if (viewmodel.DenominationModel.Deposit <= 0)
        //        {
        //            _notificationService.ShowMessage("Please enter deposit amount", NotificationType.Error);

        //            _logger.LogWarning($"Deposit Amount entered 0");

        //            return;
        //        }

        //        if (viewmodel.DenominationModel.FileUploadListInfo != null && viewmodel.DenominationModel.FileUploadListInfo.Count >= 5)
        //        {
        //            _notificationService.ShowMessage("5 files already added. Delete old file to reselect", NotificationType.Information);

        //            _logger.LogWarning($"Max File attachments added");

        //            return;
        //        }

        //        OpenFileDialog dialog = new OpenFileDialog
        //        {
        //            Title = "Please select a file",
        //            Multiselect = true,
        //            Filter = ApplicationSettings.Invoice_File_Extension_Filter,
        //        };


        //        bool? _resultOk = dialog.ShowDialog();

        //        if (_resultOk == null || _resultOk != true)
        //        {
        //            //user cancelled file selection return.
        //            return;
        //        }

        //        if (viewmodel.DenominationModel.FileUploadListInfo == null)
        //        {
        //            viewmodel.DenominationModel.FileUploadListInfo = new ObservableCollection<FileUploadInfo>();
        //        }

        //        if (dialog != null && (viewmodel.DenominationModel.FileUploadListInfo?.Count + dialog.FileNames.Length) <= 5 && dialog.FileNames.Length <= 5)
        //        {


        //            if (dialog.FileNames != null && dialog.FileNames.Count() > 5)
        //            {
        //                _notificationService.ShowMessage("Can't add more than 5 file", NotificationType.Information);

        //                _logger.LogWarning($"Max File attachments added");

        //                return;
        //            }

        //            foreach (string _fileName in dialog.FileNames)
        //            {
        //                if (_fileName.IsValidString())
        //                {
        //                    if (File.Exists(_fileName))
        //                    {
        //                        //Check for each fil size ...
        //                        FileInfo _fileInfo = new FileInfo(_fileName);

        //                        if ((_fileInfo.Length / (1024 * 1024)) > 10)
        //                        {
        //                            _notificationService.ShowMessage($"File {Path.GetFileName(_fileName)} size should be less than 10 mb in size", NotificationType.Error);

        //                            _logger.LogWarning($"Filename {_fileName} has size of {_fileInfo.Length / (1024 * 1024)} mb");

        //                            continue;
        //                        }

        //                        //dont again add the same file .causes file access issue while reading file stream

        //                        if (viewmodel.DenominationModel.FileUploadListInfo.Any(x => x.FileName == Path.GetFileName(_fileName)))
        //                        {
        //                            _notificationService.ShowMessage($"File {Path.GetFileName(_fileName)} is already added", NotificationType.Information);

        //                            _logger.LogWarning($"File was already added skipping file {_fileName}");

        //                            continue;
        //                        }

        //                        _logger.LogInformation($"Adding file {_fileName}");

        //                        viewmodel.DenominationModel.FileUploadListInfo?.Add(new FileUploadInfo
        //                        {
        //                            FileId = Guid.NewGuid(),
        //                            FilePath = _fileName,
        //                            FileExtension = Path.GetExtension(_fileName),
        //                            FileName = Path.GetFileName(_fileName),
        //                            Size = FileHelper.FormatSize(_fileInfo.Length),
        //                            FileSrc = FileSrc.local
        //                        });

        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            _notificationService.ShowMessage("Maximum 5 files allowed", NotificationType.Error);
        //        }



        //    }
        //    catch (Exception _ex)
        //    {
        //        _logger.LogError(_ex.Message);
        //    }
        //}

        //private void DeleteUploadFile(object obj)
        //{
        //    try
        //    {
        //        var viewModel = ((FileUploadInfo)obj);
        //        if (viewModel != null)
        //        {
        //            var _file = DenominationModel.FileUploadListInfo?.FirstOrDefault(x => x.FileId == viewModel.FileId);

        //            if (_file != null)
        //            {
        //                _logger.LogInformation($"File Deleted {_file.FileName}");

        //                DenominationModel.FileUploadListInfo?.Remove(_file);
        //            }

        //        }
        //    }
        //    catch (Exception _ex)
        //    {
        //        _logger.LogError(_ex.Message);
        //    }
        //}

        public async void PreviousdayDenominationVerify()
        {
            try
            {

                var _result = await _denominationService.GetDenominationVerify();
                if (_result != null && _result.IsSuccess)
                {
                    DenominationDate = null;
                    DateOfDenomination = null;
                    LoadPreviousClosingBalance(true);
                    LoadTodaySales(true);
                    _eventAggregator.GetEvent<SalesPageRefreshEvent>().Publish(_result.Data);
                }
                else
                {
                    _logger.LogInformation(_result?.Data?.Error);
                    DenominationDate = _result?.Data?.DenominationDate;
                    DateOfDenomination = "Denomination on : " + _result?.Data?.DenominationDate;
                    _notificationService.ShowMessage(_result?.Data?.Error, NotificationType.Error);
                    LoadPreviousClosingBalance(false, Convert.ToDateTime(DenominationDate));
                    LoadTodaySales(false, DenominationDate);
                    _eventAggregator.GetEvent<SalesPageRefreshEvent>().Publish(_result?.Data);

                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);

            }
        }


        public async void AddDepositPopup(object obj)
        {
            try
            {
                Deposit deposit = new Deposit();
                deposit.DataContext = this;
                await DialogHost.Show(deposit, "RootDialog", DepositEventHandler);

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }


        private async void DepositEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {

                var _viewModel = (DenominationViewModel)eventArgs.Parameter;
                if (_viewModel != null)
                {
                    await _progressService.StartProgressAsync();

                    await Task.Run(async () =>
                    {
                        var _result = await _denominationService.AddCashDeposit(DepositModel, fileUploadListInfo.ToArray());
                        Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            if (_result != null && _result.IsSuccess)
                            {
                                _notificationService.ShowMessage(_result.Data, NotificationType.Success);
                                DepositModel = new DepositModel();
                                SelectedBank = null;
                                FileUploadListInfo.Clear();
                                _eventAggregator.GetEvent<DenominationPageRefreshEvent>().Publish();

                            }
                            else
                                _notificationService.ShowMessage(_result.Error, NotificationType.Error);
                        });
                    });

                    await _progressService.StopProgressAsync();
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

        private void DeleteUploadFile(object obj)
        {
            try
            {
                var viewModel = ((FileUploadInfo)obj);
                if (viewModel != null)
                {
                    var _file = FileUploadListInfo?.FirstOrDefault(x => x.FileId == viewModel.FileId);

                    if (_file != null)
                    {
                        _logger.LogInformation($"File Deleted {_file.FileName}");

                        FileUploadListInfo?.Remove(_file);
                    }

                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public void AddDeposit(object obj)
        {
            try
            {
                if (string.IsNullOrEmpty(DepositModel.DespositDate))
                {
                    _notificationService.ShowMessage("Please enter deposit date", NotificationType.Error);
                    return;

                }

                if (DepositModel.DepostAmount == 0)
                {
                    _notificationService.ShowMessage("Please enter deposit amount", NotificationType.Error);
                    return;
                }

                if (SelectedBank == null)
                {
                    _notificationService.ShowMessage("Please select bank", NotificationType.Error);
                    return;

                }

                if (FileUploadListInfo == null || FileUploadListInfo.Count == 0)
                {
                    _notificationService.ShowMessage("Please add attachment", NotificationType.Error);
                    return;
                }

                var TargetClose = ((Button)(obj));
                var dynamicCommand = TargetClose.Command;
                dynamicCommand.CanExecute(true);
                //pass Model as Arguments
                dynamicCommand.Execute(this);
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public void BankNameChange(object obj)
        {
            try
            {
                var _viewModel = ((DenominationViewModel)obj);
                if (_viewModel != null )
                {
                    if (_selectedbank != null)
                    {
                        DepositModel.AccountNo = _viewModel.SelectedBank.AccountNo;
                        DepositModel.IFSCCode = _viewModel.SelectedBank.IFSCCode;
                        DepositModel.Branch = _viewModel.SelectedBank.Branch;
                        DepositModel.AccoutType = _viewModel.SelectedBank.AccountType;
                        DepositModel.BankName = _viewModel.SelectedBank.BankName;
                        DepositModel.BankId = _viewModel.SelectedBank.BankId;
                    }
                    else
                    {
                        DepositModel.AccountNo = null;
                        DepositModel.IFSCCode = null;
                        DepositModel.Branch = null;
                        DepositModel.AccoutType = null;
                        DepositModel.BankName = null;
                        DepositModel.BankId = -1;
                    }

                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public void Reset(object obj)
        {
            try
            {
                var _view = (DenominationViewModel)obj;
                if (_view != null)
                {
                    _view.DepositModel = new DepositModel();
                    SelectedBank = null;
                    if (FileUploadListInfo != null && FileUploadListInfo.Count > 0)
                    {
                        FileUploadListInfo.Clear();
                    }
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public async void LoadBanlList()
        {
            try
            {
                var _result = await _denominationService.GetDepositBankList();

                if (_result != null && _result.IsSuccess)
                {
                    BankList = new ObservableCollection<DepositBanksModel>(_result.Data);
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }


        public void AddFileOpenDialog(object file)
        {
            try
            {
                var viewmodel = ((DenominationViewModel)file);

                if (viewmodel.DepositModel.DepostAmount <= 0)
                {
                    _notificationService.ShowMessage("Please enter deposit amount", NotificationType.Error);

                    _logger.LogWarning($"Deposit Amount entered 0");

                    return;
                }

                if (viewmodel.FileUploadListInfo != null && viewmodel.FileUploadListInfo.Count == 1)
                {
                    _notificationService.ShowMessage("files already added. Delete old file to reselect", NotificationType.Information);

                    _logger.LogWarning($"Max File attachments added");

                    return;
                }

                OpenFileDialog dialog = new OpenFileDialog
                {
                    Title = "Please select a file",
                    Multiselect = true,
                    Filter = ApplicationSettings.Invoice_File_Extension_Filter,
                };


                bool? _resultOk = dialog.ShowDialog();

                if (_resultOk == null || _resultOk != true)
                {
                    //user cancelled file selection return.
                    return;
                }

                if (viewmodel.FileUploadListInfo == null)
                {
                    viewmodel.FileUploadListInfo = new ObservableCollection<FileUploadInfo>();
                }

                if (dialog != null)
                {


                    if (dialog.FileNames != null && dialog.FileNames.Count() > 1)
                    {
                        _notificationService.ShowMessage("Can't add more than 1 file", NotificationType.Information);

                        _logger.LogWarning($"Max File attachments added");

                        return;
                    }

                    foreach (string _fileName in dialog.FileNames)
                    {
                        if (_fileName.IsValidString())
                        {
                            if (File.Exists(_fileName))
                            {
                                //Check for each fil size ...
                                FileInfo _fileInfo = new FileInfo(_fileName);

                                if ((_fileInfo.Length / (1024 * 1024)) > 10)
                                {
                                    _notificationService.ShowMessage($"File {Path.GetFileName(_fileName)} size should be less than 10 mb in size", NotificationType.Error);

                                    _logger.LogWarning($"Filename {_fileName} has size of {_fileInfo.Length / (1024 * 1024)} mb");

                                    continue;
                                }

                                //dont again add the same file .causes file access issue while reading file stream

                                if (viewmodel.FileUploadListInfo.Any(x => x.FileName == Path.GetFileName(_fileName)))
                                {
                                    _notificationService.ShowMessage($"File {Path.GetFileName(_fileName)} is already added", NotificationType.Information);

                                    _logger.LogWarning($"File was already added skipping file {_fileName}");

                                    continue;
                                }

                                _logger.LogInformation($"Adding file {_fileName}");


                                viewmodel.FileUploadListInfo?.Add(new FileUploadInfo
                                {
                                    FileId = Guid.NewGuid(),
                                    FilePath = _fileName,
                                    FileExtension = Path.GetExtension(_fileName),
                                    FileName = Path.GetFileName(_fileName),
                                    Size = FileHelper.FormatSize(_fileInfo.Length),
                                    FileSrc = FileSrc.local
                                });

                            }
                        }
                    }
                }
                else
                {
                    _notificationService.ShowMessage("Maximum 1 files allowed", NotificationType.Error);
                }



            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }





        private ObservableCollection<FileUploadInfo> fileUploadListInfo;
        public ObservableCollection<FileUploadInfo> FileUploadListInfo
        {
            get { return fileUploadListInfo; }
            set { SetProperty(ref fileUploadListInfo, value); }
        }

        private ObservableCollection<DepositBanksModel> _banksList;

        public ObservableCollection<DepositBanksModel> BankList
        {
            get => _banksList;
            set => SetProperty(ref _banksList, value);
        }

        private DepositBanksModel _selectedbank;

        public DepositBanksModel SelectedBank
        {
            get => _selectedbank;
            set => SetProperty(ref _selectedbank, value);
        }



        private string _dateOfDenomination;

        public string DateOfDenomination
        {
            get => _dateOfDenomination;
            set => SetProperty(ref _dateOfDenomination, value);
        }



        private DepositModel _depositModel;
        public DepositModel DepositModel
        {
            get => _depositModel;
            set => SetProperty(ref _depositModel, value);
        }


    }





    public class DenominationAmountModel : BindableBase
    {
        private float _totalin2000;
        public float Totalin2000
        {
            get => _totalin2000;
            set => SetProperty(ref _totalin2000, value);
        }

        private float _totalin500;
        public float Totalin500
        {
            get => _totalin500;
            set => SetProperty(ref _totalin500, value);
        }

        private float _totalin200;
        public float Totalin200
        {
            get => _totalin200;
            set => SetProperty(ref _totalin200, value);
        }

        private float _totalin100;

        public float Totalin100
        {
            get => _totalin100;
            set => SetProperty(ref _totalin100, value);
        }

        private float _totalin50;

        public float Totalin50
        {
            get => _totalin50;
            set => SetProperty(ref _totalin50, value);
        }

        private float _totalin20;
        public float Totalin20
        {
            get => _totalin20;
            set => SetProperty(ref _totalin20, value);
        }

        private float _totalin10;

        public float Totalin10
        {
            get => _totalin10;
            set => SetProperty(ref _totalin10, value);
        }

        private float _totalinCoins;
        public float TotalinCoins
        {
            get => _totalinCoins;
            set => SetProperty(ref _totalinCoins, value);
        }


        private float _totalAmount;

        public float TotalAmount
        {
            get => _totalAmount;
            set => SetProperty(ref _totalAmount, value);
        }



    }



}
