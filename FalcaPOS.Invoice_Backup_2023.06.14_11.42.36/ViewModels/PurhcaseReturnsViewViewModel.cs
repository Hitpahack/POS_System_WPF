using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.AddInventory;
using FalcaPOS.Entites.Stores;
using FalcaPOS.Entites.Suppliers;
using FalcaPOS.Invoice.Views;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FalcaPOS.Invoice.ViewModels
{
    public class PurhcaseReturnsViewViewModel : BindableBase
    {
        private readonly Logger _logger;

        private readonly IPurchaseInvoiceService _purchaseInvoiceService;

        private readonly INotificationService _notificationService;

        private readonly ProgressService _ProgressService;


        private CancellationTokenSource _cancellationTokenSource;

        public DelegateCommand<object> ApproveCommand { get; private set; }


        public DelegateCommand<object> StatusCommand { get; private set; }

        public DelegateCommand<int?> DownloadAttachCommand { get; private set; }

        private readonly IInvoiceFileService _invoiceFileService;

        public DelegateCommand<object> StatusUncheckCommand { get; private set; }

        private readonly ISupplierService _supplierService;

        private readonly IStoreService _storeService;

        public DelegateCommand<object> SupplierSelectionChangeCommand { get; private set; }

        public DelegateCommand<object> EditCommand { get; private set; }


        public DelegateCommand<object> SaveCommand { get; private set; }


        public DelegateCommand<object> PostApproveCommand { get; private set; }

        public DelegateCommand<object> SendBackCommand { get; private set; }

        public DelegateCommand<object> SendBackUpdateCommand { get; private set; }

        public PurhcaseReturnsViewViewModel(IStoreService storeService, ISupplierService supplierService, IInvoiceFileService invoiceFileService, IPurchaseInvoiceService purchaseInvoiceService, Logger logger, INotificationService notificationService, ProgressService ProgressService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _ProgressService = ProgressService ?? throw new ArgumentNullException(nameof(ProgressService));

            _purchaseInvoiceService = purchaseInvoiceService ?? throw new ArgumentNullException(nameof(purchaseInvoiceService));

            ApproveCommand = new DelegateCommand<object>(Approve);

            StatusCommand = new DelegateCommand<object>(LoadData);

            DownloadAttachCommand = new DelegateCommand<int?>(ExecDownloadCommand);

            StatusUncheckCommand = new DelegateCommand<object>(UnCheckCommnand);

            _invoiceFileService = invoiceFileService ?? throw new ArgumentNullException(nameof(invoiceFileService));

            _supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));

            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));

            SupplierSelectionChangeCommand = new DelegateCommand<object>(selecttionchange);

            EditCommand = new DelegateCommand<object>(EditProduct);

            SaveCommand = new DelegateCommand<object>(SaveProduct);

            PostApproveCommand = new DelegateCommand<object>(PostApprove);

            SendBackCommand = new DelegateCommand<object>(SendBack);

            SendBackUpdateCommand = new DelegateCommand<object>(SendBackUpdate);


            LoadSuppliers();

            LoadStoresAsync();

            StoreVisibilty = (AppConstants.USER_ROLES[0] == AppConstants.ROLE_PURCHASE_MANAGER || AppConstants.USER_ROLES[0] == AppConstants.ROLE_FINANCE) ? true : false;
        }



        public async void Approve(object obj)
        {
            try
            {
                var _view = (StoreReturnModel)obj;

                await _ProgressService.StartProgressAsync();

                _cancellationTokenSource = new CancellationTokenSource();

                await Task.Run(async () =>
                {
                    var _result = await _purchaseInvoiceService.ApprovePurchaseReturns(_view);

                    Application.Current?.Dispatcher?.Invoke(async () =>
                    {
                        if (_result != null && _result.IsSuccess && _result.Data != null)
                        {
                            _notificationService.ShowMessage(_result.Data, Common.NotificationType.Success);

                            var _results = await _purchaseInvoiceService.GetPurchaseReturnsListView("Pre-C/N", (int)SelectedSupplier.SupplierId, SelectedStore.StoreId);

                            Application.Current?.Dispatcher?.Invoke(() =>
                            {
                                if (_results != null && _results.IsSuccess && _results.Data != null)
                                {
                                    List<StoreReturnModel> storeReturnModels = new List<StoreReturnModel>();
                                    foreach (var item in _results.Data)
                                    {
                                        var isenablebtn = (AppConstants.USER_ROLES[0] == AppConstants.ROLE_FINANCE ? true : false);

                                        var isenableedit = (AppConstants.USER_ROLES[0] == AppConstants.ROLE_PURCHASE_MANAGER ? true : false);

                                        storeReturnModels.Add(new StoreReturnModel()
                                        {
                                            Id = item.Id,
                                            IsSelected = item.IsSelected,
                                            FileID = item.FileID,
                                            SupplierId = item.SupplierId,
                                            CreditNoteNumber = item.CreditNoteNumber,
                                            CreditNoteDate = item.CreditNoteDate,
                                            StoreName = item.StoreName?.Replace(AppConstants.ShortStoreNameReplace, AppConstants.ShortStoreName),
                                            SupplierName = item.SupplierName,
                                            Total = item.Total,
                                            Status = item.Status,
                                            Remark = item.Remark,
                                            PurhcaseReturnProduct = item.PurhcaseReturnProduct,
                                            IsEnableApproveEditBtn = (isenableedit == true && item.Status == "Pre-C/N" && item.IsApproved == false) ? true : false,
                                            IsEnableApproveSaveBtn = false,
                                            CreatedDatetime = item.CreatedDatetime,

                                        });
                                    }
                                    ReturnViews = new ObservableCollection<StoreReturnModel>(storeReturnModels);

                                    RowCount = "Row Count " + ReturnViews.Count();
                                }
                                else
                                {
                                    _notificationService.ShowMessage(_results?.Error, Common.NotificationType.Error);
                                    RowCount = null;
                                    ReturnViews = null;
                                }

                            });

                        }
                        else
                        {
                            _notificationService.ShowMessage(_result?.Error, Common.NotificationType.Error);
                        }

                    });
                }, _cancellationTokenSource.Token);

                await _ProgressService.StopProgressAsync();
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);

                await _ProgressService.StopProgressAsync();
            }
            finally
            {
                await _ProgressService.StopProgressAsync();
            }
        }

        public async void LoadData(object obj)
        {
            try
            {
                _cancellationTokenSource = new CancellationTokenSource();

                await _ProgressService.StartProgressAsync();

                var Selection = Convert.ToString(obj);

                if (SelectedSupplier == null)
                {
                    _notificationService.ShowMessage("Please select supplier", Common.NotificationType.Error);
                    return;
                }

                if (SelectedStore == null)
                {
                    _notificationService.ShowMessage("Please select store", Common.NotificationType.Error);
                    return;
                }

                if (Selection == "Pre-C/N")
                {

                    IsapproveChecked = false;
                    IsadjustedChecked = false;
                    IspostcnChecked = false;
                    IsSendBackChecked = false;
                    IsCreatedChecked = false;
                    IsPartiallyAdjusted = false;
                }
                else if (Selection == "Pre-C/N-Approved")
                {
                    IsPreCNChecked = false;
                    IsadjustedChecked = false;
                    IspostcnChecked = false;
                    IsSendBackChecked = false;
                    IsCreatedChecked = false;
                    IsPartiallyAdjusted = false;
                }
                else if (Selection == "Post-C/N")
                {
                    IsPreCNChecked = false;
                    IsadjustedChecked = false;
                    IsapproveChecked = false;
                    IsSendBackChecked = false;
                    IsCreatedChecked = false;
                    IsPartiallyAdjusted = false;
                }
                else if (Selection == "Created")
                {
                    IsPreCNChecked = false;
                    IsadjustedChecked = false;
                    IsapproveChecked = false;
                    IsSendBackChecked = false;
                    IspostcnChecked = false;
                    IsPartiallyAdjusted = false;
                }
                else if (Selection == "SendBack")
                {
                    IsPreCNChecked = false;
                    IsadjustedChecked = false;
                    IsapproveChecked = false;
                    IsCreatedChecked = false;
                    IspostcnChecked = false;
                    IsPartiallyAdjusted = false;
                }
                else if (Selection == "Adjusted")
                {
                    IsPreCNChecked = false;
                    IspostcnChecked = false;
                    IsapproveChecked = false;
                    IsSendBackChecked = false;
                    IsCreatedChecked = false;
                    IsPartiallyAdjusted = false;
                }
                else if (Selection == "Partially Adjusted")
                {
                    IsPreCNChecked = false;
                    IspostcnChecked = false;
                    IsapproveChecked = false;
                    IsSendBackChecked = false;
                    IsCreatedChecked = false;
                    IsadjustedChecked = false;
                }


                var status = IsPreCNChecked ? "Pre-C/N" : IsapproveChecked ? "Pre-C/N-Approved" : IspostcnChecked ? "Post-C/N" : IsCreatedChecked ? "Created" : IsSendBackChecked ? "SendBack" : IsadjustedChecked ? "Adjusted" : IsPartiallyAdjusted ? "Partially Adjusted" : "";

                int SupplierId = (int)SelectedSupplier.SupplierId;

                int StoreId = AppConstants.USER_ROLES[0] == AppConstants.ROLE_BACKEND ? AppConstants.LoggedInStoreInfo.StoreId : SelectedStore.StoreId;

                if (!string.IsNullOrEmpty(status))
                {
                    await Task.Run(async () =>
                    {
                        var _result = await _purchaseInvoiceService.GetPurchaseReturnsListView(status, SupplierId, StoreId);

                        Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            if (_result != null && _result.IsSuccess)
                            {
                                List<StoreReturnModel> storeReturnModels = new List<StoreReturnModel>();
                                foreach (var item in _result.Data)
                                {
                                    var isenablebtn = (AppConstants.USER_ROLES[0] == AppConstants.ROLE_FINANCE ? true : false);
                                    var isenableedit = (AppConstants.USER_ROLES[0] == AppConstants.ROLE_PURCHASE_MANAGER ? true : false);
                                    storeReturnModels.Add(new StoreReturnModel()
                                    {
                                        Id = item.Id,
                                        IsSelected = item.IsSelected,
                                        FileID = item.FileID,
                                        SupplierId = item.SupplierId,
                                        CreditNoteNumber = item.CreditNoteNumber,
                                        CreditNoteDate = item.CreditNoteDate,
                                        StoreName = item.StoreName?.Replace(AppConstants.ShortStoreNameReplace, AppConstants.ShortStoreName),
                                        SupplierName = item.SupplierName,
                                        Total = item.Total,
                                        Status = item.Status,
                                        Remark = item.Remark,
                                        PurhcaseReturnProduct = item.PurhcaseReturnProduct,
                                        CreatedDatetime = item.CreatedDatetime,
                                        IsEnableApproveEditBtn = (isenableedit == true && item.Status == "Pre-C/N" && item.IsApproved == false) ? true : false,
                                        IsEnableApproveSaveBtn = false,
                                        IsEnablePostApproveBtn = (isenablebtn == true && item.Status == "Post-C/N" && item.IsApproved == false) ? true : false,
                                        AdjustModels = (item.Status == "Adjusted" || item.Status == "Partially Adjusted") ? item.AdjustModels : null
                                    });
                                }
                                ReturnViews = new ObservableCollection<StoreReturnModel>(storeReturnModels);

                                RowCount = "Row Count " + ReturnViews.Count();
                            }
                            else
                            {
                                _notificationService.ShowMessage(_result?.Error, Common.NotificationType.Error);
                                RowCount = null;
                                ReturnViews = null;
                            }

                        });
                    }, _cancellationTokenSource.Token);

                }


                await _ProgressService.StopProgressAsync();
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            finally
            {
                await _ProgressService.StopProgressAsync();
            }


        }


        private ObservableCollection<StoreReturnModel> _returnviews;

        public ObservableCollection<StoreReturnModel> ReturnViews
        {
            get { return _returnviews; }
            set { SetProperty(ref _returnviews, value); }
        }

        private string _rowcount;
        public string RowCount
        {
            get { return _rowcount; }

            set { SetProperty(ref _rowcount, value); }
        }



        private async void ExecDownloadCommand(int? fileID)
        {
            if (fileID != null && fileID.Value > 0)
            {
                var _result = await _invoiceFileService.DownloadFile(fileID.Value);

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

        public void UnCheckCommnand(object obj)
        {
            try
            {
                string Selection = Convert.ToString(obj);

                if (Selection == "Pre-C/N")
                {
                    IsPreCNChecked = false;

                }
                else if (Selection == "Pre-C/N-Approved")
                {
                    IsapproveChecked = false;
                }
                else if (Selection == "Adjusted")
                {
                    IsadjustedChecked = false;
                }
                else if (Selection == "Post-C/N")
                {
                    IspostcnChecked = false;
                }
                else if (Selection == "Created")
                {
                    IsCreatedChecked = false;
                }
                else if (Selection == "SendBack")
                {
                    IsSendBackChecked = false;
                }
                else if (Selection == "Partially Adjusted")
                {
                    IsPartiallyAdjusted = false;
                }
                ReturnViews = null;
                RowCount = null;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private bool _isprecnChecked;

        private bool _isapproveChecked;

        private bool _isadjustedChecked;

        private bool _ispostcnChecked;

        private bool _issendbackChecked;

        private bool _isCreatedChecked;

        private bool _isPartiallyAdjustedChecked;
        public bool IsPreCNChecked
        {
            get { return _isprecnChecked; }
            set { SetProperty(ref _isprecnChecked, value); }
        }
        public bool IsapproveChecked
        {
            get { return _isapproveChecked; }
            set { SetProperty(ref _isapproveChecked, value); }
        }

        public bool IsadjustedChecked
        {
            get { return _isadjustedChecked; }
            set { SetProperty(ref _isadjustedChecked, value); }
        }

        public bool IspostcnChecked
        {
            get { return _ispostcnChecked; }
            set { SetProperty(ref _ispostcnChecked, value); }
        }

        public bool IsSendBackChecked
        {
            get { return _issendbackChecked; }
            set { SetProperty(ref _issendbackChecked, value); }
        }

        public bool IsCreatedChecked
        {
            get { return _isCreatedChecked; }
            set { SetProperty(ref _isCreatedChecked, value); }
        }

        public bool IsPartiallyAdjusted
        {
            get { return _isPartiallyAdjustedChecked; }
            set { SetProperty(ref _isPartiallyAdjustedChecked, value); }
        }


        private ObservableCollection<SuppliersViewModel> _suppliers;

        public ObservableCollection<SuppliersViewModel> Suppliers
        {
            get => _suppliers;
            set => SetProperty(ref _suppliers, value);
        }


        private SuppliersViewModel _selectedSupplier;

        public SuppliersViewModel SelectedSupplier
        {
            get => _selectedSupplier;
            set => SetProperty(ref _selectedSupplier, value);

        }

        private async void LoadSuppliers()
        {

            try
            {
                _logger.LogInformation("Gettig suppliers ");

                await Task.Run(async () =>
                {
                    var _result = await _supplierService.GetSuppliers("isenabled=true");

                    if (_result != null && _result.Any())
                    {
                        Application.Current?.Dispatcher?.Invoke(() =>
                        {

                            _logger.LogInformation($"Suppliers get success --count-{_result.Count()}");

                            var addAllList = _result.ToList();
                            addAllList.Insert(0, new SuppliersViewModel() { SupplierId = -1, Name = "All" });
                            _result = addAllList;

                            Suppliers = new ObservableCollection<SuppliersViewModel>(_result);

                            SelectedSupplier = _result.FirstOrDefault();

                        });
                    }

                });

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in loding suplliers", _ex);
            }
        }

        private async void LoadStoresAsync()
        {
            var _result = await _storeService.GetStores("isenabled=true");
            if (_result != null && _result.Count() > 0)
            {

                var addAllList = _result.ToList();
                addAllList.Insert(0, new Store() { StoreId = -1, Name = "All" });
                _result = addAllList;

                Stores = new ObservableCollection<Store>(_result.Where(x => x.Parent_ref == null));

                SelectedStore = _result.FirstOrDefault();
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
            get { return _selectedStore; }
            set { SetProperty(ref _selectedStore, value); }
        }

        public async void selecttionchange(object obj)
        {
            try
            {
                _cancellationTokenSource = new CancellationTokenSource();

                await _ProgressService.StartProgressAsync();

                //var Selection = Convert.ToString(obj);

                if (SelectedSupplier == null)
                {
                    _notificationService.ShowMessage("Please select supplier", Common.NotificationType.Error);
                    return;
                }

                if (SelectedStore == null)
                {
                    _notificationService.ShowMessage("Please select store", Common.NotificationType.Error);
                    return;
                }
                if (IsPreCNChecked == false && IsapproveChecked == false && IsadjustedChecked == false)
                {
                    _notificationService.ShowMessage("Please select the any one status", Common.NotificationType.Error);
                    return;
                }
                //if (Selection == "Created")
                //{

                //    IsapproveChecked = false;
                //    IsadjustedChecked = false;
                //}
                //else if (Selection == "Approved")
                //{
                //    IscreatedChecked = false;
                //    IsadjustedChecked = false;
                //}
                //else if (Selection == "Adjusted")
                //{
                //    IscreatedChecked = false;
                //    IsapproveChecked = false;
                //}

                var status = IsPreCNChecked ? "Pre-C/N" : IsapproveChecked ? "Pre-C/N-Approved" : IspostcnChecked ? "Post-C/N" : IsCreatedChecked ? "Created" : IsSendBackChecked ? "SendBack" : IsadjustedChecked ? "Adjusted" : IsPartiallyAdjusted ? "Partially Adjusted" : "";


                int SupplierId = (int)SelectedSupplier.SupplierId;

                int StoreId = AppConstants.USER_ROLES[0] == AppConstants.ROLE_BACKEND ? AppConstants.LoggedInStoreInfo.StoreId : SelectedStore.StoreId;

                if (!string.IsNullOrEmpty(status))
                {
                    await Task.Run(async () =>
                    {
                        var _result = await _purchaseInvoiceService.GetPurchaseReturnsListView(status, SupplierId, StoreId);

                        Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            if (_result != null && _result.IsSuccess)
                            {
                                List<StoreReturnModel> storeReturnModels = new List<StoreReturnModel>();
                                foreach (var item in _result.Data)
                                {
                                    var isenablebtn = (AppConstants.USER_ROLES[0] == AppConstants.ROLE_FINANCE ? true : false);
                                    var isenableedit = (AppConstants.USER_ROLES[0] == AppConstants.ROLE_PURCHASE_MANAGER ? true : false);
                                    storeReturnModels.Add(new StoreReturnModel()
                                    {
                                        Id = item.Id,
                                        IsSelected = item.IsSelected,
                                        FileID = item.FileID,
                                        SupplierId = item.SupplierId,
                                        CreditNoteNumber = item.CreditNoteNumber,
                                        CreditNoteDate = item.CreditNoteDate,
                                        StoreName = item.StoreName,
                                        SupplierName = item.SupplierName,
                                        Total = item.Total,
                                        Status = item.Status,
                                        Remark = item.Remark,
                                        PurhcaseReturnProduct = item.PurhcaseReturnProduct,
                                        CreatedDatetime = item.CreatedDatetime,

                                        IsEnableApproveEditBtn = (isenableedit == true && item.Status == "Pre-C/N" && item.IsApproved == false) ? true : false,
                                        IsEnableApproveSaveBtn = false,
                                        IsEnablePostApproveBtn = (isenablebtn == true && item.Status == "Post-C/N" && item.IsApproved == false) ? true : false
                                    });
                                }
                                ReturnViews = new ObservableCollection<StoreReturnModel>(storeReturnModels);

                                RowCount = "Row Count " + ReturnViews.Count();
                            }
                            else
                            {
                                _notificationService.ShowMessage(_result?.Error, Common.NotificationType.Error);
                                RowCount = null;
                                ReturnViews = null;
                            }

                        });
                    }, _cancellationTokenSource.Token);

                }


                await _ProgressService.StopProgressAsync();
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            finally
            {
                await _ProgressService.StopProgressAsync();
            }


        }





        public void EditProduct(object obj)
        {
            try
            {
                var viewModel = (StoreReturnModel)obj;

                viewModel.PurhcaseReturnProduct.ToList().ForEach(x => x.IsReadOnly = true);


                viewModel.IsEnableApproveEditBtn = false;

                viewModel.IsEnableApproveSaveBtn = true;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public void SaveProduct(object obj)
        {
            try
            {
                var viewModel = (StoreReturnModel)obj;
                foreach (var item in viewModel.PurhcaseReturnProduct)
                {
                    if (item.ReturnQty == 0)
                    {
                        _notificationService.ShowMessage("Please enter return qty", Common.NotificationType.Error);
                        return;
                    }
                    if (item.RemainingStock < item.ReturnQty)
                    {
                        _notificationService.ShowMessage($"Please enter maxmimum available({item.RemainingStock}) stock qty to return", Common.NotificationType.Error);
                        return;
                    }
                }
                viewModel.PurhcaseReturnProduct.ToList().ForEach(x => x.IsReadOnly = false);

                viewModel.Total = viewModel.PurhcaseReturnProduct.ToList().Sum(x => ((((x.ProductRate * x.ProductGST) / 100) + x.ProductRate) * x.ReturnQty));
                viewModel.IsEnableApproveEditBtn = true;
                viewModel.IsEnableApproveSaveBtn = false;
                viewModel.IsEdited = true;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }


        public async void PostApprove(object obj)
        {
            try
            {
                var _view = (StoreReturnModel)obj;
                _view.Status = "Created";
                await _ProgressService.StartProgressAsync();

                _cancellationTokenSource = new CancellationTokenSource();

                await Task.Run(async () =>
                {
                    var _result = await _purchaseInvoiceService.PostCNApprovePurchaseReturns(_view.Id, _view.Status, _view.Remark);

                    Application.Current?.Dispatcher?.Invoke(async () =>
                    {
                        if (_result != null && _result.IsSuccess && _result.Data != null)
                        {
                            _notificationService.ShowMessage(_result.Data, Common.NotificationType.Success);

                            var _results = await _purchaseInvoiceService.GetPurchaseReturnsListView("Post-C/N", (int)SelectedSupplier.SupplierId, SelectedStore.StoreId);

                            Application.Current?.Dispatcher?.Invoke(() =>
                            {
                                if (_results != null && _results.IsSuccess && _results.Data != null)
                                {
                                    List<StoreReturnModel> storeReturnModels = new List<StoreReturnModel>();
                                    foreach (var item in _results.Data)
                                    {
                                        var isenablebtn = (AppConstants.USER_ROLES[0] == AppConstants.ROLE_FINANCE ? true : false);

                                        var isenableedit = (AppConstants.USER_ROLES[0] == AppConstants.ROLE_PURCHASE_MANAGER ? true : false);

                                        storeReturnModels.Add(new StoreReturnModel()
                                        {
                                            Id = item.Id,
                                            IsSelected = item.IsSelected,
                                            FileID = item.FileID,
                                            SupplierId = item.SupplierId,
                                            CreditNoteNumber = item.CreditNoteNumber,
                                            CreditNoteDate = item.CreditNoteDate,
                                            StoreName = item.StoreName?.Replace(AppConstants.ShortStoreNameReplace, AppConstants.ShortStoreName),
                                            SupplierName = item.SupplierName,
                                            Total = item.Total,
                                            Status = item.Status,
                                            Remark = item.Remark,
                                            PurhcaseReturnProduct = item.PurhcaseReturnProduct,
                                            IsEnablePostApproveBtn = (isenablebtn == true && item.Status == "Post-C/N" && item.IsApproved == false) ? true : false,
                                            IsEnableApproveEditBtn = (isenableedit == true && item.Status == "Pre-C/N" && item.IsApproved == false) ? true : false,
                                            IsEnableApproveSaveBtn = false,
                                            CreatedDatetime = item.CreatedDatetime,

                                        });
                                    }
                                    ReturnViews = new ObservableCollection<StoreReturnModel>(storeReturnModels);

                                    RowCount = "Row Count " + ReturnViews.Count();
                                }
                                else
                                {
                                    _notificationService.ShowMessage(_results?.Error, Common.NotificationType.Error);
                                    RowCount = null;
                                    ReturnViews = null;
                                }

                            });

                        }
                        else
                        {
                            _notificationService.ShowMessage(_result?.Error, Common.NotificationType.Error);
                        }

                    });
                }, _cancellationTokenSource.Token);

                await _ProgressService.StopProgressAsync();
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);

                await _ProgressService.StopProgressAsync();
            }
            finally
            {
                await _ProgressService.StopProgressAsync();
            }
        }
        public async void SendBack(object obj)
        {
            try
            {
                var _view = (StoreReturnModel)obj;
                if (_view != null)
                {
                    SendBackModel = _view;
                    Remarks = null;
                    SendBackPopup sendBackPopup = new SendBackPopup();
                    sendBackPopup.DataContext = this;
                    await DialogHost.Show(sendBackPopup, "RootDialog", SendColsingEventHandler);

                }


            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);

                await _ProgressService.StopProgressAsync();
            }
        }

        private async void SendColsingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {
                var _viewModel = (PurhcaseReturnsViewViewModel)eventArgs.Parameter;
                if (_viewModel != null)
                {
                    var _view = SendBackModel;
                    _view.Status = "SendBack";
                    _view.Remark = Remarks;
                    await _ProgressService.StartProgressAsync();

                    _cancellationTokenSource = new CancellationTokenSource();

                    await Task.Run(async () =>
                    {
                        var _result = await _purchaseInvoiceService.PostCNApprovePurchaseReturns(_view.Id, _view.Status, _view.Remark);

                        Application.Current?.Dispatcher?.Invoke(async () =>
                        {
                            if (_result != null && _result.IsSuccess && _result.Data != null)
                            {
                                _notificationService.ShowMessage("Sendbacked successfully", Common.NotificationType.Success);

                                var _results = await _purchaseInvoiceService.GetPurchaseReturnsListView("Post-C/N", (int)SelectedSupplier.SupplierId, SelectedStore.StoreId);

                                Application.Current?.Dispatcher?.Invoke(() =>
                                {
                                    if (_results != null && _results.IsSuccess && _results.Data != null)
                                    {
                                        List<StoreReturnModel> storeReturnModels = new List<StoreReturnModel>();
                                        foreach (var item in _results.Data)
                                        {
                                            var isenablebtn = (AppConstants.USER_ROLES[0] == AppConstants.ROLE_FINANCE ? true : false);

                                            var isenableedit = (AppConstants.USER_ROLES[0] == AppConstants.ROLE_PURCHASE_MANAGER ? true : false);

                                            storeReturnModels.Add(new StoreReturnModel()
                                            {
                                                Id = item.Id,
                                                IsSelected = item.IsSelected,
                                                FileID = item.FileID,
                                                SupplierId = item.SupplierId,
                                                CreditNoteNumber = item.CreditNoteNumber,
                                                CreditNoteDate = item.CreditNoteDate,
                                                StoreName = item.StoreName?.Replace(AppConstants.ShortStoreNameReplace, AppConstants.ShortStoreName),
                                                SupplierName = item.SupplierName,
                                                Total = item.Total,
                                                Status = item.Status,
                                                Remark = item.Remark,
                                                PurhcaseReturnProduct = item.PurhcaseReturnProduct,
                                                IsEnablePostApproveBtn = (isenablebtn == true && item.Status == "Post-C/N" && item.IsApproved == false) ? true : false,
                                                IsEnableApproveEditBtn = (isenableedit == true && item.Status == "Pre-C/N" && item.IsApproved == false) ? true : false,
                                                IsEnableApproveSaveBtn = false,
                                                CreatedDatetime = item.CreatedDatetime,

                                            });
                                        }
                                        ReturnViews = new ObservableCollection<StoreReturnModel>(storeReturnModels);

                                        RowCount = "Row Count " + ReturnViews.Count();
                                    }
                                    else
                                    {
                                        _notificationService.ShowMessage(_results?.Error, Common.NotificationType.Error);
                                        RowCount = null;
                                        ReturnViews = null;
                                    }

                                });

                            }
                            else
                            {
                                _notificationService.ShowMessage(_result?.Error, Common.NotificationType.Error);
                            }

                        });
                    }, _cancellationTokenSource.Token);

                    await _ProgressService.StopProgressAsync();
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

        private void SendBackUpdate(object obj)
        {
            try
            {

                if (string.IsNullOrEmpty(Remarks))
                {
                    _notificationService.ShowMessage("Please enter remarks", Common.NotificationType.Error);
                    return;
                }

                var TargetClose = ((Button)(obj));
                var dynamicCommand = TargetClose.Command;
                dynamicCommand.CanExecute(true);
                //pass Model as Arguments
                dynamicCommand.Execute(this);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        private StoreReturnModel _sendBackModel;

        public StoreReturnModel SendBackModel
        {
            get { return _sendBackModel; }
            set { SetProperty(ref _sendBackModel, value); }
        }

        private string _remarks;

        public string Remarks
        {
            get { return _remarks; }
            set { SetProperty(ref _remarks, value); }
        }

        private bool _storeVisibility;

        public bool StoreVisibilty
        {
            get { return _storeVisibility; }
            set { SetProperty(ref _storeVisibility, value); }
        }


    }




}


