using ControlzEx.Standard;
using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Denomination.View;
using FalcaPOS.Entites.Denomination;
using FalcaPOS.Entites.Stores;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using DelegateCommand = Prism.Commands.DelegateCommand;

namespace FalcaPOS.Denomination.ViewModel
{
    public class DepositViewModel : BindableBase
    {
        private readonly INotificationService _notificationService;

        private readonly ProgressService _ProgressService;

        private readonly IDialogService _dialogService;

        private readonly IEventAggregator _eventAggregator;

        private readonly Logger _logger;

        private readonly IDenominationService _denominationService;

        private readonly IStoreService _storeService;
        public DelegateCommand<object> DepositSearchCommad { get; private set; }
        public DelegateCommand<object> DepositResetCommand { get; private set; }
        public DelegateCommand<int?> DownloadFileDepositCommand { get; private set; }
        public DelegateCommand DepositExportCommand { get; private set; }

        private readonly ICommonService _commonService;
        public DelegateCommand<object> CaseDepositApprovepopupCommand { get; private set; }

        public DelegateCommand<object> ApproveCommand { get; private set; }

        public DelegateCommand<int?> PreviewImageCommand { get; private set; }

        private string _previewPath = String.Empty;


        public DepositViewModel(ICommonService commonService, IStoreService storeService, IDenominationService denominationService, Logger logger, IDialogService dialogService, INotificationService notificationService, ProgressService ProgressService, IEventAggregator eventAggregator)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            _ProgressService = ProgressService;

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _denominationService = denominationService ?? throw new ArgumentNullException(nameof(denominationService));

            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            DepositSearchCommad = new DelegateCommand<object>(SearchData);

            DepositResetCommand = new DelegateCommand<object>(Resetdata);

            LoadStores();

            IsExportEnabled = false;

            DownloadFileDepositCommand = new DelegateCommand<int?>(DownloadFile);

            DepositExportCommand = new DelegateCommand(() => Export());

            IsStoreVisibile = (AppConstants.USER_ROLES[0] == AppConstants.ROLE_FINANCE || AppConstants.USER_ROLES[0]==AppConstants.ROLE_CONTROL_MANAGER || AppConstants.USER_ROLES[0] == AppConstants.ROLE_DIRECTOR)? true : false;

            CaseDepositApprovepopupCommand = new DelegateCommand<object>(CaseDepositApprrove);

            ApproveCommand = new DelegateCommand<object>(Approve);

            PreviewImageCommand = new DelegateCommand<int?>(PreviewImageCommandEvent);

            _previewPath = ApplicationSettings.PREVIEW_PATH;

            if (!Directory.Exists(_previewPath))
                Directory.CreateDirectory(_previewPath);

        }

        private async void PreviewImageCommandEvent(int? fileId)
        {
            try
            {
                if (fileId != null)
                {
                    //rework Magesh need to place proper place

                    var _files = Directory.GetFiles(_previewPath);
                    if (_files != null)
                    {
                        foreach (var item in _files)
                        {
                            File.Delete(item);
                        }
                    }
                    var _absolutePath = "";
                    await Task.Run(async () =>
                    {
                        var _result = await _denominationService.DownloadFileDepsoit((int)fileId);
                        if (_result != null && _result.IsSuccess)
                        {
                            var _data = _result.Data.FileStream;
                            if (_data != null && _result.Data.FileStream != null)
                            {
                                var getExtension = Path.GetExtension(_result.Data.FileName);
                                if (getExtension != null)
                                {
                                    _absolutePath = Path.Combine(_previewPath,$"{Convert.ToString((int)fileId)}{getExtension}");
                                    File.WriteAllBytes(_absolutePath, _result.Data.FileStream);

                                    await Application.Current.Dispatcher.InvokeAsync(async () =>
                                    {
                                        ImagePreview imagePreview = new ImagePreview(_eventAggregator);
                                        _eventAggregator.GetEvent<PreviewEvent>().Publish(_absolutePath);
                                        await DialogHost.Show(imagePreview, "RootDialog", imagePreviewClosingeventHandler);

                                    });

                                }
                            }
                        }
                        else
                        {
                            _notificationService.ShowMessage("Resource not found", NotificationType.Error);
                            return;
                        }


                    });
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private void imagePreviewClosingeventHandler(object sender, DialogClosingEventArgs eventArgs)
        {

        }

        private async void DownloadFile(int? fileId)
        {
            if (fileId == null || fileId <= 0) return;

            try
            {
                await Task.Run(async () =>
                {
                    var _result = await _denominationService.DownloadFileDepsoit((int)fileId);

                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        //null check
                        if (_result != null && _result.IsSuccess && _result.Data != null && _result.Data.FileStream != null)
                        {
                            var getExtension = Path.GetExtension(_result.Data.FileName);

                            SaveFileDialog sd = new SaveFileDialog
                            {
                                CreatePrompt = true,
                                OverwritePrompt = true,
                                DefaultExt = getExtension,
                            };
                            var _saveFile = sd.ShowDialog();

                            if (_saveFile == true && sd.FileName.IsValidString())
                            {
                                sd.FileName = Path.ChangeExtension(sd.FileName, getExtension);

                                File.WriteAllBytes(sd.FileName, _result.Data.FileStream);
                            }
                        }
                        else
                        {
                            _notificationService.ShowMessage(_result?.Error ?? "An error occurred, try again", Common.NotificationType.Error);
                        }

                    });

                });


            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in invoice model", _ex);
            }
        }

        public void Export()
        {
            try
            {
                if (DepositViews == null || !DepositViews.Any())
                {
                    _notificationService.ShowMessage("No records found to export", Common.NotificationType.Error);

                    return;
                }
                List<DepositExportModel> depositExports = new List<DepositExportModel>();

                if (DepositViews != null && DepositViews.Count > 0)
                {
                    foreach (var item in DepositViews)
                    {
                        depositExports.Add(new DepositExportModel()
                        {
                            PosDepositDate = item.PosDepositDate,
                            DepositDate = item.DepositDate,
                            DepositAmount = item.DepositAmount,
                            StoreName = item.StoreName,
                            IFSCCode = item.IFSCCode,
                            AccountNo = item.AccountNo,

                            BankName = item.BankName,
                            Branch = item.Branch,
                            DocumnetNo = item.DocumnetNo,
                        });
                    }

                }
                if (depositExports == null || !depositExports.Any())
                {
                    _notificationService.ShowMessage("No records found to export", Common.NotificationType.Error);

                    return;
                }

                IsExportEnabled = false;

                //_fileName = DateTime.Now.Date.ToString("dd-MM-yyyy") + "Deposit";

                bool _result = _commonService.ExportToXL(depositExports, "Deposit", skipfileName: false);

                if (_result)
                {
                    _notificationService.ShowMessage("File exported to folder C:\\FALCAPOS\\PosReports", Common.NotificationType.Success);
                }
                else
                {
                    _notificationService.ShowMessage("File export failed , try again", Common.NotificationType.Error);
                }

                IsExportEnabled = true;
            }
            catch (Exception _ex)
            {

                _notificationService.ShowMessage(_ex.Message, Common.NotificationType.Error);
            }



        }
        public async void SearchData(object obj)
        {
            try
            {
                if (AppConstants.ROLE_FINANCE == AppConstants.USER_ROLES[0] || AppConstants.ROLE_DIRECTOR == AppConstants.USER_ROLES[0] || AppConstants.ROLE_CONTROL_MANAGER == AppConstants.USER_ROLES[0])
                {
                    if (SelectedStore == null)
                    {
                        _notificationService.ShowMessage("Please select store", Common.NotificationType.Error);
                        return;
                    }
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

                    if (Convert.ToDateTime(FromDate) > Convert.ToDateTime(ToDate))
                    {
                        _notificationService.ShowMessage("from date should be less than or equal to date", Common.NotificationType.Error);
                        return;
                    }

                    int StoreId = (AppConstants.USER_ROLES[0] == AppConstants.ROLE_FINANCE || AppConstants.USER_ROLES[0] == AppConstants.ROLE_CONTROL_MANAGER || AppConstants.USER_ROLES[0] == AppConstants.ROLE_DIRECTOR) ? SelectedStore.StoreId : AppConstants.LoggedInStoreInfo.StoreId;

                    if (obj != null && obj is RadGridView)
                    {
                        ResetTelerikGridFilters.ClearTelerikGridViewFilters(obj);
                    }
                var _result = await _denominationService.GetCashDepositSearch(FromDate, ToDate, StoreId);
                    if (_result != null && _result.IsSuccess)
                    {
                        IsExportEnabled = true;
                        DepositViews = _result.Data.ToList().Select(x => { x.IsVerified = ((AppConstants.USER_ROLES[0] == AppConstants.ROLE_FINANCE || AppConstants.USER_ROLES[0] == AppConstants.ROLE_CONTROL_MANAGER) && x.IsVerified == (null)) ? true : false; return x; }).ToList();
                    }
                    else
                    {
                        _notificationService.ShowMessage(_result.Error, Common.NotificationType.Error);
                        DepositViews = null;
                    }
                
            }


            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public void Resetdata(object obj)
        {
            try
            {
                IsExportEnabled = false;
                DepositViews = null;
                FromDate = null;
                ToDate = null;
                SelectedStore = null;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
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
                    {
   
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

        private async void DepositPopupEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {

                var _viewModel = (DepositViewModel)eventArgs.Parameter;
                if (_viewModel != null)
                {
                    await _ProgressService.StartProgressAsync();

                    var _result = await _denominationService.UpdateCaseDepositApproval(_viewModel.SelectedDeposit.Id, true);
                    if (_result != null && _result.IsSuccess)
                    {
                        _notificationService.ShowMessage(_result.Data, NotificationType.Success);
                       // DepositViews = null;
                        SearchData(new { SelectedStore, FromDate, ToDate });
                       
                    }
                    else
                    {
                        _notificationService.ShowMessage(_result.Error, NotificationType.Success);
                        
                    }



                }


            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);
            }
            finally
            {
                await _ProgressService.StopProgressAsync();
            }
        }

        public async void CaseDepositApprrove(object obj)
        {
            try
            {
                var _viewModel = (DepositViewModelResponse)obj;
                if (_viewModel != null)
                {
                    DepositApprovePopUp depositApprovePopUp = new DepositApprovePopUp();
                    depositApprovePopUp.DataContext = this;
                    SelectedDeposit = _viewModel;
                    await DialogHost.Show(depositApprovePopUp, "RootDialog", DepositPopupEventHandler);
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public void Approve(object obj)
        {
            try
            {
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

        private ObservableCollection<Store> _stores;
        public ObservableCollection<Store> Stores
        {
            get => _stores;
            set => SetProperty(ref _stores, value);
        }

        private Store _selectedStore;
        public Store SelectedStore
        {
            get => _selectedStore;
            set => SetProperty(ref _selectedStore, value);

        }

        private List<DepositViewModelResponse> _detpositViews;

        public List<DepositViewModelResponse> DepositViews
        {
            get => _detpositViews;
            set => SetProperty(ref _detpositViews, value);
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

        private bool _isExportEnabled;

        public bool IsExportEnabled
        {
            get => _isExportEnabled;
            set => SetProperty(ref _isExportEnabled, value);
        }

        public string _fileName { get; set; }

        private bool _isStorevisibile;

        public bool IsStoreVisibile
        {
            get => _isStorevisibile;
            set => SetProperty(ref _isStorevisibile, value);
        }

        private DepositViewModelResponse _selectedDeposit;

        public DepositViewModelResponse SelectedDeposit
        {
            get => _selectedDeposit;
            set => SetProperty(ref _selectedDeposit, value);
        }

        private Visibility _isVisibility;

        public Visibility IsVisibilty
        {
            get => _isVisibility;
            set => SetProperty(ref _isVisibility, value);
        }



    }
}
