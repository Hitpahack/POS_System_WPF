using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.AddInventory;
using FalcaPOS.Entites.Common;
using FalcaPOS.Entites.Indent;
using FalcaPOS.Entites.Suppliers;
using FalcaPOS.Indent.Views;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unity;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using GSTslabs = FalcaPOS.Entites.Indent.GSTslabs;

namespace FalcaPOS.Indent.ViewModels
{
    public class IndentApprovalViewModel : BindableBase
    {

        private readonly IUnityContainer _container;


        private readonly INotificationService _notificationService;

        private readonly ProgressService _progressService;

        private readonly IEventAggregator _eventAggregator;

        public DelegateCommand<object> ChangeStatusCommand { get; private set; }

        public DelegateCommand<object> SubmitCommand { get; private set; }


        private readonly IIndentService _indentService;

        private readonly Logger _logger;

        private readonly ISupplierService _supplierService;


        public DelegateCommand<object> AddSupplierCommand { get; private set; }

        public DelegateCommand<object> UpdateCommand { get; private set; }

        public DelegateCommand<object> EditCommand { get; private set; }

        public DelegateCommand<object> StatusChangeCommand { get; private set; }

        public DelegateCommand<object> ReceivedProductCommand { get; private set; }

        public DelegateCommand<object> RemoveProductCommand { get; private set; }

        public DelegateCommand<object> EditPriceProductCommand { get; private set; }

        public DelegateCommand ExportCommand { get; private set; }
        public DelegateCommand<int?> DownloadIndentInvoiceCommand { get; private set; }

        public DelegateCommand<object> AddPartialPaymentsCommand { get; private set; }
        public DelegateCommand<object> AddPartialPaymentsRemoveCommand { get; private set; }

        private readonly ICommonService _commonService;

        private readonly IInvoiceFileService _invoiceFileService;

        private readonly IDialogService _dialogService;

        public DelegateCommand<object> YesCommand { get; private set; }


        public DelegateCommand<object> EstimatePriceChangeCommand { get; private set; }

        public DelegateCommand<object> PaymentUpdateCommand { get; private set; }


        public DelegateCommand<object> ProInvoiceAttachmentCommand { get; private set; }

        public DelegateCommand<object> DeleteUploadFileCommand { get; private set; }

        //public DelegateCommand<object> AddPaymentCommand { get; private set; }

        public DelegateCommand<int?> DownloadFileCommand { get; private set; }

        public DelegateCommand<object> SelectBankCommand { get; private set; }

        public DelegateCommand<object> UnSelectBankCommand { get; private set; }

        public DelegateCommand<object> SelectedCreditCommand { get; private set; }

        public DelegateCommand<object> UnSelectedCreditCommand { get; private set; }
        public DelegateCommand<object> CloseSelectedCreditCommand { get; private set; }
        public DelegateCommand<object> RedeemedAmountChangeCommand { get; private set; }

        public DelegateCommand<object> SelectionChangedPercentCommand { get; private set; }

        public DelegateCommand<object> AddRemainPartialPaymentsCommand { get; private set; }

        public DelegateCommand<object> SaveRemainPartialPaymentCommand { get; private set; }


        public DelegateCommand<object> PartialPriceTextChangedCommand { get; private set; }

        public DelegateCommand AddSupplierContinueCommand { get; private set; }

        public DelegateCommand AddProductPriceContinueCommand { get; private set; }

        public DelegateCommand<object> SelectionChangedPercentCommandSecond { get; private set; }

        public DelegateCommand<object> SecondPartialPriceTextChangedCommand { get; private set; }

        public DelegateCommand<object> SelectedInvoiceCommand { get; private set; }


        public DelegateCommand<object> UnSelectedInvoiceCommand { get; private set; }

        public DelegateCommand AddProductCardReviewCommand { get; private set; }

        public DelegateCommand<object> RemoveProductReviewCardCommand { get; private set; }

        public DelegateCommand<object> AddReviewCommand { get; private set; }

        public DelegateCommand<object> AddProductToIndentCommand { get; private set; }



        public IndentApprovalViewModel(IUnityContainer container, IDialogService dialogService, ISupplierService supplierService, Logger logger, IEventAggregator eventAggregator, IIndentService indentService, INotificationService notificationService, ProgressService progressService, ICommonService commonService, IInvoiceFileService invoiceFileService)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _ = eventAggregator.GetEvent<IntentNewTabCreateEvent>().Subscribe(GetIndent);

            _indentService = indentService ?? throw new ArgumentNullException();

            ChangeStatusCommand = new DelegateCommand<object>(ChangeStatus);

            PaymentUpdateCommand = new DelegateCommand<object>(PaymentUpdate);

            SubmitCommand = new DelegateCommand<object>(Submit);

            AddSupplierCommand = new DelegateCommand<object>(AddSupplier);

            UpdateCommand = new DelegateCommand<object>(Update);

            _supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            EditCommand = new DelegateCommand<object>(Edit);

            StatusChangeCommand = new DelegateCommand<object>(StatusChange);

            ReceivedProductCommand = new DelegateCommand<object>(ReceivedProduct);

            RemoveProductCommand = new DelegateCommand<object>(RemoveProduct);

            EditPriceProductCommand = new DelegateCommand<object>(EditProductPrice);

            ExportCommand = new DelegateCommand(() => ExportXl());

            DownloadIndentInvoiceCommand = new DelegateCommand<int?>(ExecDownloadIndentInvoiceCommand);

            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));

            _invoiceFileService = invoiceFileService ?? throw new ArgumentNullException(nameof(invoiceFileService));

            _eventAggregator.GetEvent<SignalRIndentStatusEvent>().Subscribe(GetIndentDetails, ThreadOption.PublisherThread);

            _eventAggregator.GetEvent<ReloadSupplierEvent>().Subscribe(LoadSuppliers);

            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            YesCommand = new DelegateCommand<object>(Confirmationpopup);

            EstimatePriceChangeCommand = new DelegateCommand<object>(EstimatePriceChangeCalculation);

            ProInvoiceAttachmentCommand = new DelegateCommand<object>(AddFileOpenDialog);

            DeleteUploadFileCommand = new DelegateCommand<object>(DeleteUploadFile);

            //AddPaymentCommand = new DelegateCommand<object>(AddPayment);

            DownloadFileCommand = new DelegateCommand<int?>(DownloadFile);

            SelectBankCommand = new DelegateCommand<object>(SelectedBankDetails);

            UnSelectBankCommand = new DelegateCommand<object>(UnSelectedBankDetails);

            SelectedCreditCommand = new DelegateCommand<object>(SelectedCreditnoteDetails);

            UnSelectedCreditCommand = new DelegateCommand<object>(UnSelectedCreditnoteDetails);

            CloseSelectedCreditCommand = new DelegateCommand<object>(closeSelectedCreditnoteDetails);

            RedeemedAmountChangeCommand = new DelegateCommand<object>(RedeemedAmountChange);

            AddPartialPaymentsCommand = new DelegateCommand<object>(AddPartialPaymentsCommandEvent);

            AddPartialPaymentsRemoveCommand = new DelegateCommand<object>(AddPartialPaymentsRemoveCommandEvent);

            AddSupplierToIndent.PartialPayment = new PartialPayment();

            SelectionChangedPercentCommand = new DelegateCommand<object>(SelectionChangePercent);


            AddRemainPartialPaymentsCommand = new DelegateCommand<object>(RemainPartialPayment);

            SaveRemainPartialPaymentCommand = new DelegateCommand<object>(SaveRemain);

            PartialPriceTextChangedCommand = new DelegateCommand<object>(PartialPriceChange);

            AddSupplierContinueCommand = new DelegateCommand(AddSupplierContinue);

            AddProductPriceContinueCommand = new DelegateCommand(AddPriceContinue);

            SelectionChangedPercentCommandSecond = new DelegateCommand<object>(SecondSelectionChangePercent);


            SecondPartialPriceTextChangedCommand = new DelegateCommand<object>(SecondPartialPriceChange);

            SelectedInvoiceCommand = new DelegateCommand<object>(SelectedInvoiceAgainst);

            UnSelectedInvoiceCommand = new DelegateCommand<object>(UnSelectedInvoiceAgainst);
            IndentProducts = new ObservableCollection<IndentProductCardViewModel>();

            IndentProducts.CollectionChanged -= IndentProducts_CollectionChanged;

            IndentProducts.CollectionChanged += IndentProducts_CollectionChanged;

            _container = container ?? throw new ArgumentNullException(nameof(container));

            AddProductCardReviewCommand = new DelegateCommand(AddNewIndentProduct);

            RemoveProductReviewCardCommand = new DelegateCommand<object>(RemoveIndentProduct);

            AddReviewCommand = new DelegateCommand<object>(AddReview);

            AddProductToIndentCommand = new DelegateCommand<object>(AddProductTo);


        }

        private void AddPartialPaymentsRemoveCommandEvent(object obj)
        {
            if (AddSupplierToIndent.PartialPayments != null)
            {
                AddSupplierToIndent.PartialPayments.Remove(obj as PartialPayment);
            }
        }

        private void AddPartialPaymentsCommandEvent(object obj)
        {
            if (AddSupplierToIndent.PartialPayments != null)
            {
                if (AddSupplierToIndent.PartialPayments.Count < 3)
                    AddSupplierToIndent.PartialPayments.Add(new PartialPayment());
                else
                    _notificationService.ShowMessage("Max 3 partial payments allowed", NotificationType.Warning);
            }
            else
            {
                AddSupplierToIndent.PartialPayments = new ObservableCollection<PartialPayment>();
                AddSupplierToIndent.PartialPayments.Add(new PartialPayment());
            }
        }

        private void RedeemedAmountChange(object obj)
        {
            try
            {
                var _selectedReturnList = AddSupplierToIndent.ReturnModels.Where(x => x.IsSelected == true).ToList();

                var totalSelectedAmount = _selectedReturnList.Where(x => x.IsSelected).Sum(x => x.RedeemedTotal);

                if (_selectedReturnList != null && _selectedReturnList.Count > 0)
                {
                    AddSupplierToIndent.SelectedReturnModels = _selectedReturnList;

                    AddSupplierToIndent.FinalPayableAmount = AddSupplierToIndent.PayableAmount - totalSelectedAmount;
                }



            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

            }
        }

        private async void ExecDownloadIndentInvoiceCommand(int? fileID)
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

        public void EditProductPrice(object param)
        {
            try
            {
                var TargetClose = ((Button)(param));
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
        public void RemoveProduct(object param)
        {
            try
            {

                var product = (IndentViewProduct)param;
                if (product != null)
                {
                    Products.Remove(product);
                    PropductEditReason = PropductEditReason == null ? "Deleted product name " + product.ProductName + "," + product.BrandName + "," + product.ProductType + ", Qty" + product.Quantity : PropductEditReason + " ,Deleted product name " + product.ProductName + "," + product.BrandName + "," + product.ProductType + ", Qty" + product.Quantity;

                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

        }

        public void ReceivedProduct(object obj)
        {

            try
            {

                if (Products != null && Products.Count > 0)
                {
                    int row = 1;
                    foreach (var item in Products)
                    {
                        if (item.InventoryReceivedQty == 0)
                        {
                            _notificationService.ShowMessage("Please enter received qty at row " + row + " " + item.ProductName, Common.NotificationType.Error);
                            return;
                        }
                        if (item.InventoryReceivedQty != item.AvailableQty)
                        {
                            _notificationService.ShowMessage("Received qty and available qty should be equal  at row " + row + " " + item.ProductName, Common.NotificationType.Error);
                            return;
                        }
                        row++;
                    }
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

        public void StatusChange(object obj)
        {
            try
            {

                if (NextStatus == IndentStatus.intransit.ToString())
                {
                    if (string.IsNullOrEmpty(TrackingId))
                    {
                        _notificationService.ShowMessage("Please enter Tracking ID", Common.NotificationType.Error);
                        return;
                    }
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
        public void Edit(object obj)
        {
            try
            {

                if (Products.Count == 0)
                {
                    _notificationService.ShowMessage("Please add product,without product can not update", Common.NotificationType.Error);
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
        public void Update(object obj)
        {
            try
            {
                if (String.IsNullOrEmpty(NextStatus))
                {
                    _notificationService.ShowMessage("Please select next status Approve or Reject", Common.NotificationType.Error);
                    return;
                }
                if (NextStatus == "Reject")
                {
                    if (String.IsNullOrEmpty(Reason))
                    {
                        _notificationService.ShowMessage("Please enter reason", Common.NotificationType.Error);
                        return;
                    }

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

        public void AddSupplier(object obj)
        {
            try
            {
                if (AddSupplierToIndent.SelectedSupplier == null)
                {
                    _notificationService.ShowMessage("Please select supplier", Common.NotificationType.Error);
                    return;
                }

                if (!(bool)AddSupplierToIndent.SelectedSupplier?.Isenabled)
                {
                    _notificationService.ShowMessage("Selected supplier disabled", Common.NotificationType.Error);
                    return;
                }


                //if (string.IsNullOrEmpty(AddSupplierToIndent.ArrivingDate))
                //{
                //    _notificationService.ShowMessage("Please select Arriving Date", Common.NotificationType.Error);
                //    return;
                //}

                if (string.IsNullOrEmpty(AddSupplierToIndent.GSTType))
                {
                    _notificationService.ShowMessage("Please select Inclusive or Exclusive GST", Common.NotificationType.Error);

                    return;
                }

                if (AddSupplierToIndent != null && AddSupplierToIndent.Products.Count > 0)
                {
                    int row = 1;
                    foreach (var item in AddSupplierToIndent.Products)
                    {
                        if (item.AvailableQty > 0) {
                            if (AddSupplierToIndent.GSTType == "Exclusive GST") {
                                if (item.SelectedGSTslab == null) {
                                    _notificationService.ShowMessage("Please select GST value at row" + row, Common.NotificationType.Error);
                                    return;
                                }
                            }
                            if (item.EstimatedPrice == 0) {
                                _notificationService.ShowMessage("Please enter estimated price at row " + row, Common.NotificationType.Error);
                                return;
                            }
                            //if (item.AvailableQty == 0)
                            //{
                            //    _notificationService.ShowMessage("Please enter available qty at row " + row, Common.NotificationType.Error);
                            //    return;
                            //}

                            //if (item.AvailableQty > item.Quantity)
                            //{

                            //    _notificationService.ShowMessage(" Available qty should not be greater than request qty at row " + row, Common.NotificationType.Error);
                            //    return;


                            //}

                           
                        }
                        row++;
                    }

                    //if(AddSupplierToIndent.FileUploadListInfo==null || AddSupplierToIndent.FileUploadListInfo.Count() == 0)
                    //{
                    //    _notificationService.ShowMessage("Please add Proforma Invoice", Common.NotificationType.Error);
                    //    return;
                    //}


                }
                if (AddSupplierToIndent.TotalAvailableQty > 0) {
                    if (string.IsNullOrEmpty(AddSupplierToIndent.PaymentType)) {
                        _notificationService.ShowMessage("Please select payment mode as full or partial", Common.NotificationType.Error);

                        return;
                    }

                    if (AddSupplierToIndent.PaymentType == "Full") {
                        if (String.IsNullOrEmpty(AddSupplierToIndent.CreditPeriod)) {
                            _notificationService.ShowMessage("Please enter credit period (days)", Common.NotificationType.Error);
                            return;
                        }
                    }

                    if (AddSupplierToIndent.PaymentType == "Partial") {
                        if (AddSupplierToIndent.PartialPayment.Price == 0 && (AddSupplierToIndent.PartialPayment.Percentage == null || AddSupplierToIndent.PartialPayment.Percentage == 0)) {
                            _notificationService.ShowMessage("Please enter price or percentage", NotificationType.Error);
                            return;
                        }

                        if (string.IsNullOrEmpty(AddSupplierToIndent.PartialPayment.PaymentDate)) {
                            _notificationService.ShowMessage("Please enter payment date", NotificationType.Error);
                            return;
                        }

                    }

                }


                var TargetClose = ((Button)(obj));
                var dynamicCommand = TargetClose.Command;
                dynamicCommand.CanExecute(true);
                //pass Model as Arguments
                dynamicCommand.Execute(this);
            }
            catch (Exception)
            {


            }
        }
        public void Submit(object obj)
        {
            try
            {

                if (String.IsNullOrEmpty(NextStatus))
                {
                    _notificationService.ShowMessage("Please select next status Approve or Reject", Common.NotificationType.Error);
                    return;
                }
                if (NextStatus == "Reject")
                {
                    if (String.IsNullOrEmpty(Reason))
                    {
                        _notificationService.ShowMessage("Please Enter Reason", Common.NotificationType.Error);
                        return;
                    }

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
                _notificationService.ShowMessage(AppConstants.CommonError, Common.NotificationType.Error);


            }
        }


        public void PaymentUpdate(object obj)
        {
            try
            {
                if (AddSupplierToIndent.SelectedBankDetail == null)
                {
                    _notificationService.ShowMessage("Please select bank", Common.NotificationType.Error);
                    return;
                }

                if (string.IsNullOrEmpty(AddSupplierToIndent.RefrenceNo))
                {
                    _notificationService.ShowMessage("Please enter Reference No", Common.NotificationType.Error);
                    return;
                }
                if (string.IsNullOrEmpty(AddSupplierToIndent.PaymentDate))
                {
                    _notificationService.ShowMessage("Please enter Payment Date", Common.NotificationType.Error);
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
        public async void ChangeStatus(object obj)
        {
            try
            {
                var model = (obj as IndentViewModel);

                if (model != null)
                {

                    if (model.Status.ToLower() == IndentStatus.created.ToString())
                    {
                        IndentReview review = new IndentReview();
                        review.DataContext = this;
                        CurrentStatus = "Created" + " ------- Review";
                        review.currentstatus.Text = CurrentStatus;
                        review.currentstatus.Tag = model.IndentStoreId;
                        NextStatus = IndentStatus.review.ToString();
                        PoId = model.Id;
                        int i = 1;
                        ReviewProducts = new ObservableCollection<IndentViewProduct>(model.Products.Select(x => { x.SerailNumber = i; i++; return x; }).ToList());
                        SlNo = model.Products.Count() + 1;
                        IndentProducts.Clear();
                        await DialogHost.Show(review, "RootDialog", ReviewClosingEventHandler);




                    }
                    else if (model.Status.ToLower() == IndentStatus.review.ToString())
                    {
                        IndentReview review = new IndentReview();
                        review.DataContext = this;
                        CurrentStatus = "Review" + " ------- Approve";
                        review.currentstatus.Text = CurrentStatus;
                        review.currentstatus.Tag = model.IndentStoreId;
                        NextStatus = IndentStatus.approve.ToString();
                        PoId = model.Id;
                        int i = 1;
                        ReviewProducts = new ObservableCollection<IndentViewProduct>(model.Products.Select(x => { x.SerailNumber = i; i++; return x; }).ToList());
                        SlNo = model.Products.Count() + 1;
                        IndentProducts.Clear();
                        await DialogHost.Show(review, "RootDialog", ReviewClosingEventHandler);

                    }
                    else if (model.Status.ToLower() == IndentStatus.approve.ToString())
                    {

                        IndentPopupAddSupplier indentPopupAddSupplier = new IndentPopupAddSupplier(_eventAggregator);
                        indentPopupAddSupplier.DataContext = this;
                        CurrentStatus = "Approve" + " -------  Add Supplier";
                        //indentPopupAddSupplier.currentstatus.Text = CurrentStatus;
                        PoId = model.Id;
                        LoadSuppliers();
                        int i = 1;
                        AddSupplierToIndent.Products = model.Products.Select(x => { x.SerailNumber = i; x.AvailableQty = 0; x.EstimatedPrice = 0; x.ProductTotal = 0; x.GSTslabs = GetGSTslabs(); i++; return x; }).ToList();
                        AddSupplierToIndent.CreditPeriod = "0";
                        AddSupplierToIndent.ArrivingDate = null;
                        AddSupplierToIndent.GSTType = null;
                        AddSupplierToIndent.TotalAmount = 0;
                        AddSupplierToIndent.FileUploadListInfo = null;
                        AddSupplierToIndent.PayableAmount = 0;
                        AddSupplierToIndent.TransportCharges = 0;
                        AddSupplierToIndent.PaymentType = null;
                        AddSupplierToIndent.TotalRequestQty = model.Products.Sum(x => x.Quantity);
                        AddSupplierToIndent.PartialPayment.Price = 0;
                        AddSupplierToIndent.PartialPayment.Percentage = null;
                        AddSupplierToIndent.PartialPayment.PaymentTotal = 0;
                        AddSupplierToIndent.PartialPayment.PaymentDate = null;
                        AddSupplierToIndent.PartialPayments = null;
                        await DialogHost.Show(indentPopupAddSupplier, "RootDialog", AddSupplierClosingEventHandler);

                        _eventAggregator.GetEvent<IndentStatusFlyoutEvent>().Publish(true);
                    }


                    else if (model.Status.ToLower() == IndentStatus.addsupplier.ToString())
                    {
                        IndentPopupStatusPlacedAndTransit placedAndTransit = new IndentPopupStatusPlacedAndTransit();
                        placedAndTransit.DataContext = this;
                        CurrentStatus = "AddSupplier" + " ------- Placed";
                        placedAndTransit.currentstatus.Text = CurrentStatus;
                        NextStatus = IndentStatus.placed.ToString();
                        PoId = model.Id;
                        TrackingIdVisibility = false;
                        Reason = null;
                        await DialogHost.Show(placedAndTransit, "RootDialog", FullPaymentEventHandler);
                    }
                    else if (model.Status.ToLower() == IndentStatus.placed.ToString())
                    {
                        IndentPopupStatusPlacedAndTransit placed = new IndentPopupStatusPlacedAndTransit();
                        placed.DataContext = this;
                        CurrentStatus = ProperStatus.ProperStatusName(IndentStatus.placed.ToString()) + " ------- Transit";
                        placed.currentstatus.Text = CurrentStatus;
                        NextStatus = IndentStatus.intransit.ToString();
                        PoId = model.Id;
                        TrackingIdVisibility = true;
                        TrackingId = null;
                        Reason = null;
                        await DialogHost.Show(placed, "RootDialog", StatusChangeClosingEventHandler);
                    }

                    else if (model.Status.ToLower() == IndentStatus.intransit.ToString())
                    {
                        if (model.IndentInventoryDetails != null && model.IndentInventoryDetails.Select(x => x.InvoiceNo).FirstOrDefault() != AppConstants.YETTOADD)
                        {
                            IndentPopupReceivedProduct receivedProduct = new IndentPopupReceivedProduct();
                            receivedProduct.DataContext = this;
                            CurrentStatus = ProperStatus.ProperStatusName(IndentStatus.intransit.ToString()) + " ------- Received";
                            receivedProduct.currentstatus.Text = CurrentStatus;
                            NextStatus = IndentStatus.received.ToString();
                            PoId = model.Id;
                            TrackingId = model.TrackingId;
                            Products = new ObservableCollection<IndentViewProduct>(model.Products.Where(x=>x.AvailableQty>0).ToList());
                            GSTType = model.Products.Select(x => x.SelectedGSTslab?.GstType).FirstOrDefault() != null ? "Exclusive GST" : "Inclusive GST";
                            await DialogHost.Show(receivedProduct, "RootDialog", ReceivedClosingEventHandler);
                        }
                        else
                            _notificationService.ShowMessage("Please add inward inventory ", Common.NotificationType.Error);
                        return;


                    }
                    else if (model.Status.ToLower() == IndentStatus.received.ToString() && model.PayableAmount != model.PaymentsList.Where(x => x.PaidDate != null && x.PayrefNo != null).Sum(x => x.PaymentTotal))
                    {



                        if (model.PaymentsList.Where(x => string.IsNullOrEmpty(x.PayrefNo) && string.IsNullOrEmpty(x.PaidDate)).FirstOrDefault() != null)
                        {
                            _notificationService.ShowMessage("Please update the last payment", Common.NotificationType.Error);

                            return;
                        }
                        _notificationService.ShowMessage("Please pay the remaining amount", Common.NotificationType.Error);

                        return;

                    }
                    else if (model.Status.ToLower() == IndentStatus.fullpaid.ToString() || model.IsFullPaid == true)
                    {
                        if (model.IndentInventoryDetails != null)
                        {
                            if (model.Products.Sum(x => x.ReceivedQty) != model.IndentInventoryDetails.Sum(x => x.SubQty))
                            {
                                _notificationService.ShowMessage("Please check inventory quantity and indent received quantity not matching", Common.NotificationType.Error);
                                return;
                            }
                        }
                        if (model.PayableAmount != model.PaymentsList.Where(x => x.PaidDate != null && x.PayrefNo != null).Sum(x => x.PaymentTotal))
                        {
                            _notificationService.ShowMessage("Please update UTR Date and PayRef", Common.NotificationType.Error);
                            return;
                        }
                        IndentPopupStatusPlacedAndTransit closed = new IndentPopupStatusPlacedAndTransit();
                        closed.DataContext = this;
                        CurrentStatus = ProperStatus.ProperStatusName(model.Status) + " ------- Closed";
                        closed.currentstatus.Text = CurrentStatus;
                        NextStatus = IndentStatus.closed.ToString();
                        PoId = model.Id;
                        TrackingIdVisibility = false;
                        Reason = null;
                        await DialogHost.Show(closed, "RootDialog", ChangeClosingEventHandler);

                    }

                }

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in status change method", _ex);
            }
        }

        private void AddSupplierClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {
                var _viewModel = (IndentApprovalViewModel)eventArgs.Parameter;
                if (_viewModel != null)
                {
                    _dialogService.ShowDialog("Confimationpopup",
                                parameters: new DialogParameters
                                (""),
                                callback: (dialogResult) =>
                                {
                                    if (dialogResult != null && dialogResult.Result == ButtonResult.OK)
                                    {
                                        submitPopup(_viewModel);
                                    }
                                    else
                                    {
                                        return;
                                    }

                                });



                }


            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);
            }
        }
        private async void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {

                var _viewModel = (IndentApprovalViewModel)eventArgs.Parameter;
                if (_viewModel != null)
                {
                    await _progressService.StartProgressAsync();

                    await Task.Run(async () =>
                    {
                        IndentViewModel model = new IndentViewModel();
                        model.Id = PoId;
                        //model.Indentstatus = _viewmodel.NextStatus == "Approve" ? IndentStatus.approvedlevel1 : IndentStatus.rejectedlevel1;
                        model.Remark = _viewModel.Reason;

                        var _result = await _indentService.IndentApproval(model);
                        Application.Current?.Dispatcher?.Invoke(async () =>
                        {

                            if (_result != null && _result.IsSuccess)
                            {
                                _notificationService.ShowMessage("Indent Approved (Stage 1)", Common.NotificationType.Success);

                                _eventAggregator.GetEvent<IntentStatuschangeRefreshEvent>().Publish(model.Id);

                            }
                            else
                            {
                                _notificationService.ShowMessage(_result?.Error ?? AppConstants.CommonError, Common.NotificationType.Error);
                            }

                            await _progressService.StopProgressAsync();
                        });


                    });
                }

            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                _notificationService.ShowMessage(AppConstants.CommonError, Common.NotificationType.Error);

                await _progressService.StopProgressAsync();

            }


        }

        public void GetIndentDetails(IndentViewModel indentView)
        {
            try
            {
                if (indentView != null)
                {
                    string _previousStatus = "";
                    List<ActivityLogs> activityLog = new List<ActivityLogs>();
                    foreach (var item in indentView.AcivityLogs)
                    {

                        activityLog.Add(new ActivityLogs()
                        {
                            //Date = Convert.ToDateTime(item.Date) == DateTime.Now.Date ? indentView.Date + " (Today)" : item.Date + " (" + Convert.ToDateTime(item.Date).ToHumanReadableString() + ")",
                            Date = item.Date,
                            Status = item.Status.ToLower() == IndentStatus.created.ToString() ? ProperStatus.ProperStatusName(item.Status) : ProperStatus.ProperStatusName(_previousStatus) + " ----- " + ProperStatus.ProperStatusName(item.Status)
                            ,
                            UserAndRole = item.UserAndRole,
                        });
                        _previousStatus = item.Status;

                    }

                    ProcessindentView.PoNumber = indentView.PoNumber;
                    ProcessindentView.Id = indentView.Id;
                    ProcessindentView.Date = Convert.ToDateTime(indentView.Date) == DateTime.Now.Date ? indentView.Date + " (Today)" : indentView.Date + " (" + Convert.ToDateTime(indentView.Date).ToHumanReadableString() + ")";
                    ProcessindentView.StoreName = indentView.StoreName;
                    ProcessindentView.IndentStoreId = indentView.IndentStoreId;
                    ProcessindentView.Status = indentView.Status;
                    List<IndentViewProduct> viewProducts = new List<IndentViewProduct>();
                    foreach (var item in indentView.Products)
                    {
                        GSTslabs gSTslabs = new GSTslabs();
                        gSTslabs.GstType = null;
                        viewProducts.Add(new IndentViewProduct()
                        {
                            ProductId = item.ProductId,
                            SKU = item.SKU,
                            ProductName = item.ProductName,
                            ProductType = item.ProductType,
                            BrandName = item.BrandName,
                            AvailableQty = item.AvailableQty,
                            ReceivedQty = item.ReceivedQty,
                            Quantity = item.Quantity,
                            EstimatedPrice = item.EstimatedPrice,
                            GSTslabs = item.GSTslabs,
                            SelectedGSTslab = item.SelectedGSTslab,
                            IsEstimatePrice = AppConstants.ROLE_STORE_PERSON == AppConstants.USER_ROLES[0] ? true : false,
                            IsInclusiveGstdatagrid = AppConstants.ROLE_STORE_PERSON == AppConstants.USER_ROLES[0] ? true : item.SelectedGSTslab?.GstType == null ? true : false,
                            InventoryReceivedQty = item.InventoryReceivedQty,
                            POSStockQty = item.POSStockQty,
                            TmQty = item.TmQty,
                            RmQty = item.RmQty,
                            Rsp = item.Rsp,
                            IsRamQtyVisible = AppConstants.ROLE_REGIONAL_MANAGER == AppConstants.USER_ROLES[0] ? true : false,

                        });
                    }
                    ProcessindentView.Products = viewProducts;
                    ProcessindentView.TotalEstimantedAmount = viewProducts.Sum(x => (float)Math.Round((x.EstimatedPrice + (x.EstimatedPrice * (x.SelectedGSTslab?.GstValue ?? 0) * 0.01)) * x.AvailableQty, MidpointRounding.AwayFromZero));
                    ProcessindentView.TotalAvailableQty = viewProducts.Sum(x => x.AvailableQty);
                    ProcessindentView.IsTotalEstimantedAmount = (ProcessindentView.TotalEstimantedAmount != 0 && AppConstants.ROLE_STORE_PERSON != AppConstants.USER_ROLES[0]) ? true : false;
                    ProcessindentView.AcivityLogs = activityLog;
                    ProcessindentView.SupplierName = indentView.SupplierName ?? AppConstants.YETTOADD;
                    ProcessindentView.ArrivingDate = indentView.ArrivingDate;
                    ProcessindentView.ArriDate = indentView.ArrivingDate?.ToString("dd-MM-yyyy");
                    ProcessindentView.CreditPeriod = indentView.CreditPeriod;
                    ProcessindentView.AcivityLogs = activityLog;
                    ProcessindentView.TrackingId = indentView.TrackingId ?? AppConstants.YETTOADD;
                    ProcessindentView.Remark = indentView.Remark;
                    ProcessindentView.AdvisoryCharges = indentView.AdvisoryCharges;
                    ProcessindentView.FileAttachments = indentView.FileAttachments;
                    ProcessindentView.IsProformaDownloadbtnEnabled = (indentView.FileAttachments != null && (AppConstants.USER_ROLES[0] == AppConstants.ROLE_DIRECTOR.ToString() || AppConstants.USER_ROLES[0] == AppConstants.ROLE_PURCHASE_MANAGER || AppConstants.USER_ROLES[0] == AppConstants.ROLE_FINANCE.ToString())) == true ? true : false;

                    ProcessindentView.IsInclusiveGST = indentView.SupplierName != null ? ((indentView.Products != null && indentView.Products.ToList().Count > 0) ? (indentView.Products.Select(x => x.SelectedGSTslab?.GstType).FirstOrDefault() != null ? false : true) : false) : false;

                    ProcessindentView.SupplierDetails = indentView.SupplierDetails;

                    if (indentView.IndentInventoryDetails != null && indentView.IndentInventoryDetails.Count > 0)
                    {
                        ProcessindentView.IndentInventoryDetails = indentView.IndentInventoryDetails.Where(x => x.InvoiceNo == x.PictureName && x.InvoiceDate != null).ToList();
                        if (ProcessindentView.IndentInventoryDetails != null && ProcessindentView.IndentInventoryDetails.Count > 0)
                        {
                            ProcessindentView.IndentInventoryDetails.Select(x =>
                            {
                                if (AppConstants.USER_ROLES[0] == AppConstants.ROLE_PURCHASE_MANAGER.ToString() ||
                                AppConstants.USER_ROLES[0] == AppConstants.ROLE_DIRECTOR.ToString() || AppConstants.USER_ROLES[0] == AppConstants.ROLE_FINANCE.ToString())
                                    x.IsInventoryDownloadBtnEnabled = true;
                                else
                                    x.IsInventoryDownloadBtnEnabled = false;
                                return x;

                            }).ToList();
                        }
                        else
                        {
                            List<IndentInventoryDetails> indentInventoryDetails = new List<IndentInventoryDetails>();
                            indentInventoryDetails.Add(new IndentInventoryDetails()
                            {
                                Status = AppConstants.YETTOADD,
                                InvoiceDate = null,
                                InvoiceNo = AppConstants.YETTOADD,
                                POSEntryDate = null,
                                Qty = 0,
                                SubQty = 0,
                                IsInventoryDownloadBtnEnabled = false,
                                DcNo = AppConstants.YETTOADD


                            });
                            ProcessindentView.IndentInventoryDetails = indentInventoryDetails;
                        }


                        ProcessindentView.IndentInventoryDCDetails = indentView.IndentInventoryDetails.Where(x => x.DcNo == x.PictureName).ToList();
                        if (ProcessindentView.IndentInventoryDCDetails != null && ProcessindentView.IndentInventoryDCDetails.Count > 0)
                        {
                            ProcessindentView.IndentInventoryDCDetails.Select(x =>
                            {
                                if (AppConstants.USER_ROLES[0] == AppConstants.ROLE_PURCHASE_MANAGER.ToString() ||
                                AppConstants.USER_ROLES[0] == AppConstants.ROLE_DIRECTOR.ToString() || AppConstants.USER_ROLES[0] == AppConstants.ROLE_FINANCE.ToString())
                                    x.IsInventoryDownloadBtnEnabled = true;
                                else
                                    x.IsInventoryDownloadBtnEnabled = false;
                                return x;

                            }).ToList();
                        }
                        else
                        {
                            List<IndentInventoryDetails> indentInventoryDetails = new List<IndentInventoryDetails>();
                            indentInventoryDetails.Add(new IndentInventoryDetails()
                            {
                                Status = AppConstants.YETTOADD,
                                InvoiceDate = null,
                                InvoiceNo = null,
                                POSEntryDate = null,
                                Qty = 0,
                                SubQty = 0,
                                IsInventoryDownloadBtnEnabled = false,
                                DcNo = AppConstants.YETTOADD,
                                DcDate = null



                            });

                            ProcessindentView.IndentInventoryDCDetails = indentInventoryDetails;
                        }
                    }
                    else
                    {
                        List<IndentInventoryDetails> indentInventoryDetails = new List<IndentInventoryDetails>();
                        indentInventoryDetails.Add(new IndentInventoryDetails()
                        {
                            Status = AppConstants.YETTOADD,
                            InvoiceDate = null,
                            InvoiceNo = AppConstants.YETTOADD,
                            POSEntryDate = null,
                            Qty = 0,
                            SubQty = 0,
                            IsInventoryDownloadBtnEnabled = false,
                            DcNo = AppConstants.YETTOADD


                        });
                        ProcessindentView.IndentInventoryDetails = indentInventoryDetails;
                        ProcessindentView.IndentInventoryDCDetails = indentInventoryDetails;
                    }
                    ProcessindentView.BankDetails = indentView.BankDetails;
                    var totalAmount = EstimateCalculation(indentView.Products);
                    ProcessindentView.PayableAmount = ((float)Math.Round(totalAmount, 0, MidpointRounding.AwayFromZero)) + indentView.AdvisoryCharges;
                    var paid = (indentView.PaymentsList != null && indentView.PaymentsList.Count > 0) ? indentView.PaymentsList.Sum(x => x.PaymentTotal) : 0;
                    ProcessindentView.Balance = (ProcessindentView.PayableAmount - paid);
                    ProcessindentView.IsFullPaid = (ProcessindentView.PayableAmount != indentView.PaymentsList.Where(x => x.PaidDate != null && x.PayrefNo != null).Sum(x => x.PaymentTotal)) ? false : true;
                    indentView.IsFullPaid = (ProcessindentView.PayableAmount != indentView.PaymentsList.Sum(x => x.PaymentTotal)) ? false : true;
                    ProcessindentView.CreateDatetime = indentView.CreateDatetime;
                    ProcessindentView.SelectedBankDetail = indentView.SelectedBankDetail != null ? indentView.SelectedBankDetail : null;
                    // var updatepaymentbtnenable = indentView.CreateDatetime != null ? (indentView.CreateDatetime.Value.AddDays(Convert.ToInt16(indentView.CreditPeriod)).Date <= DateTime.Now.Date) == true ? true : false : true;
                    //ProcessindentView.IsUpateBtnVisiblity = (updatepaymentbtnenable == true && indentView.IsFullPaid == false && AppConstants.USER_ROLES[0] == AppConstants.ROLE_FINANCE &&
                    //    ((indentView.Status == IndentStatus.placed.ToString()) || (indentView.Status == IndentStatus.intransit.ToString()) || (indentView.Status == IndentStatus.received.ToString()) || (indentView.Status == IndentStatus.fullpaid.ToString()) || (indentView.Status == IndentStatus.closed.ToString()))) ? true : false;
                    var balance = (ProcessindentView.PayableAmount - indentView.PaymentsList.Sum(x => x.PaymentTotal));
                    ProcessindentView.IsUpateBtnVisiblity = (AppConstants.ROLE_PURCHASE_MANAGER == AppConstants.USER_ROLES[0].ToString()) ? (balance == 0) ? false : true : false;
                    if (!String.IsNullOrEmpty(indentView.Status))
                        ChangeStepStatus(indentView.Status.ToLower());
                    ProcessindentView.PaymentsList = indentView.PaymentsList;

                }
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in getting indent details", _ex);
            }

        }


        private async void LoadSuppliers()
        {

            try
            {
                _logger.LogInformation("Getting suppliers ");

                await Task.Run(async () =>
                {
                    var _result = await _supplierService.GetSuppliers("isenabled=true");

                    if (_result != null && _result.Any())
                    {
                        Application.Current?.Dispatcher?.Invoke(() =>
                        {

                            _logger.LogInformation($"Suppliers get success --count-{_result.Count()}");
                            if (_result?.Count() > 0)
                            {
                                _result.Select(x => { x.Name = x.Name + "-->" + x.Address.State + "-->" + x.Address.District; return x; }).ToList();
                                Suppliers = new ObservableCollection<SuppliersViewModel>(_result);
                            }

                        });
                    }

                });

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in loading suppliers", _ex);
            }
        }



        private void ChangeStepStatus(string status)
        {
            try
            {
                IndentStatus indentStatusEnum;
                Enum.TryParse(status, out indentStatusEnum);
                IsEnableBtnChangeStatus = false;
                IsExportEnabled = false;
                switch (indentStatusEnum)
                {


                    case IndentStatus.created:
                        _eventAggregator.GetEvent<StepSelectedIndexChangeEvent>().Publish((int)1);
                        if (AppConstants.USER_ROLES[0] == AppConstants.ROLE_TERRITORY_MANAGER.ToString())
                            IsEnableBtnChangeStatus = true;
                        break;

                    case IndentStatus.review:
                        _eventAggregator.GetEvent<StepSelectedIndexChangeEvent>().Publish((int)2);
                        if (AppConstants.USER_ROLES[0] == AppConstants.ROLE_REGIONAL_MANAGER.ToString())
                            IsEnableBtnChangeStatus = true;
                        break;

                    case IndentStatus.approve:
                        _eventAggregator.GetEvent<StepSelectedIndexChangeEvent>().Publish((int)3);
                        if (AppConstants.USER_ROLES[0] == AppConstants.ROLE_PURCHASE_MANAGER.ToString())
                            IsEnableBtnChangeStatus = true;
                        break;


                    case IndentStatus.addsupplier:
                        _eventAggregator.GetEvent<StepSelectedIndexChangeEvent>().Publish((int)4);
                        if (AppConstants.USER_ROLES[0] == AppConstants.ROLE_PURCHASE_MANAGER.ToString())
                            IsEnableBtnChangeStatus = true;
                        break;


                    case IndentStatus.placed:
                        _eventAggregator.GetEvent<StepSelectedIndexChangeEvent>().Publish((int)5);
                        if (AppConstants.USER_ROLES[0] == AppConstants.ROLE_PURCHASE_MANAGER.ToString())
                            IsEnableBtnChangeStatus = true;
                        if (AppConstants.USER_ROLES[0] == AppConstants.ROLE_PURCHASE_MANAGER.ToString() ||
                            AppConstants.USER_ROLES[0] == AppConstants.ROLE_DIRECTOR.ToString() || AppConstants.USER_ROLES[0] == AppConstants.ROLE_FINANCE.ToString()
                            )
                            IsExportEnabled = true;
                        break;
                    case IndentStatus.intransit:
                        _eventAggregator.GetEvent<StepSelectedIndexChangeEvent>().Publish((int)6);
                        if (AppConstants.USER_ROLES[0] == AppConstants.ROLE_STORE_PERSON.ToString())
                            IsEnableBtnChangeStatus = true;
                        if (AppConstants.USER_ROLES[0] == AppConstants.ROLE_PURCHASE_MANAGER.ToString() ||
                            AppConstants.USER_ROLES[0] == AppConstants.ROLE_DIRECTOR.ToString() || AppConstants.USER_ROLES[0] == AppConstants.ROLE_FINANCE.ToString()
                            )
                            IsExportEnabled = true;

                        break;
                    case IndentStatus.received:
                        if (ProcessindentView.IsFullPaid)
                        {
                            _eventAggregator.GetEvent<StepSelectedIndexChangeEvent>().Publish((int)8);
                        }
                        else
                        {
                            _eventAggregator.GetEvent<StepSelectedIndexChangeEvent>().Publish((int)7);
                        }
                        if (AppConstants.USER_ROLES[0] == AppConstants.ROLE_FINANCE.ToString() && ProcessindentView.IsFullPaid == false)
                        {
                            ProcessindentView.CreditPeriod = ProcessindentView.CreditPeriod == "NA" ? "0" : ProcessindentView.CreditPeriod;
                            var updatePaymentBtnEnable = ProcessindentView.CreateDatetime != null ? (ProcessindentView.CreateDatetime.Value.AddDays(Convert.ToInt16(ProcessindentView.CreditPeriod)).Date <= DateTime.Now.Date) == true ? true : false : true;
                            IsEnableBtnChangeStatus = updatePaymentBtnEnable == true ? true : false;
                        }

                        if (AppConstants.USER_ROLES[0] == AppConstants.ROLE_PURCHASE_MANAGER.ToString() ||
                            AppConstants.USER_ROLES[0] == AppConstants.ROLE_DIRECTOR.ToString() || AppConstants.USER_ROLES[0] == AppConstants.ROLE_FINANCE.ToString()
                            )
                        {
                            IsExportEnabled = true;
                        }
                        if (AppConstants.USER_ROLES[0] == AppConstants.ROLE_PURCHASE_MANAGER.ToString() && ProcessindentView.IsFullPaid)
                        {
                            IsEnableBtnChangeStatus = true;
                        }
                        break;
                    case IndentStatus.fullpaid:
                        _eventAggregator.GetEvent<StepSelectedIndexChangeEvent>().Publish((int)8);
                        if (AppConstants.USER_ROLES[0] == AppConstants.ROLE_PURCHASE_MANAGER.ToString())
                            IsEnableBtnChangeStatus = true;
                        if (AppConstants.USER_ROLES[0] == AppConstants.ROLE_PURCHASE_MANAGER.ToString() ||
                            AppConstants.USER_ROLES[0] == AppConstants.ROLE_DIRECTOR.ToString() || AppConstants.USER_ROLES[0] == AppConstants.ROLE_FINANCE.ToString()
                            )
                        {
                            IsExportEnabled = true;
                        }
                        break;

                    case IndentStatus.closed:
                        _eventAggregator.GetEvent<StepSelectedIndexChangeEvent>().Publish((int)8);
                        IsEnableBtnChangeStatus = false;
                        if (AppConstants.USER_ROLES[0] == AppConstants.ROLE_PURCHASE_MANAGER.ToString() ||
                            AppConstants.USER_ROLES[0] == AppConstants.ROLE_DIRECTOR.ToString() || AppConstants.USER_ROLES[0] == AppConstants.ROLE_FINANCE.ToString()
                            )
                        {
                            IsExportEnabled = true;
                        }
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in indent  ChangeStepStatus", ex);
            }
        }



        private async void StatusChangeClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {

                var _viewModel = (IndentApprovalViewModel)eventArgs.Parameter;
                if (_viewModel != null)
                {
                    await _progressService.StartProgressAsync();

                    await Task.Run(async () =>
                    {
                        IndentViewModel model = new IndentViewModel();
                        model.Indentstatus = (IndentStatus)Enum.Parse(typeof(IndentStatus), _viewModel.NextStatus, true);
                        model.Id = PoId;
                        model.Remark = Reason;
                        if (_viewModel.NextStatus == IndentStatus.intransit.ToString())
                        {
                            model.TrackingId = TrackingId;
                            model.Remark = Reason;
                        }


                        var _result = await _indentService.StatusChangeToInTransit(model.Id, model.TrackingId, model.Remark);
                        Application.Current?.Dispatcher?.Invoke(async () =>
                        {

                            if (_result != null && _result.IsSuccess)
                            {
                                _notificationService.ShowMessage(_result?.Data ?? "Indent " + ProperStatus.ProperStatusName(_viewModel.NextStatus), Common.NotificationType.Success);

                                _eventAggregator.GetEvent<IntentNewTabCreateEvent>().Publish(model);
                            }
                            else
                            {
                                _notificationService.ShowMessage(_result?.Error ?? AppConstants.CommonError, Common.NotificationType.Error);
                            }

                            await _progressService.StopProgressAsync();
                        });


                    });
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

        private async void ReceivedClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {

                var _viewModel = (IndentApprovalViewModel)eventArgs.Parameter;
                if (_viewModel != null)
                {
                    await _progressService.StartProgressAsync();

                    await Task.Run(async () =>
                    {
                        IndentViewModel model = new IndentViewModel();
                        model.Indentstatus = (IndentStatus)Enum.Parse(typeof(IndentStatus), _viewModel.NextStatus, true);
                        model.Id = PoId;
                        model.Remark = Reason;
                        //List<IndentViewProduct> indentViewProducts = new List<IndentViewProduct>();
                        //foreach (var item in Products)
                        //{
                        //    indentViewProducts.Add(new IndentViewProduct()
                        //    {
                        //        ProductId = item.ProductId,
                        //        ProductName = item.ProductName,
                        //        ProductType = item.ProductType,
                        //        BrandName = item.BrandName,
                        //        ProductTotal = item.ProductTotal,
                        //        Quantity = item.Quantity,
                        //        IsEstimatePrice = item.IsEstimatePrice,
                        //        IsInclusiveGst = item.IsInclusiveGst,
                        //        IsInclusiveGstdatagrid = item.IsInclusiveGstdatagrid,
                        //        AvailableQty = item.AvailableQty,
                        //        ChangeQty = item.ChangeQty,
                        //        EstimatedPrice = item.EstimatedPrice,
                        //        SelectedGSTslab = item.SelectedGSTslab,
                        //        GSTslabs = item.GSTslabs,
                        //        SKU = item.SKU,
                        //        SerailNumber = item.SerailNumber,
                        //        SubUnitType = item.SubUnitType,
                        //        InventoryReceivedQty = item.InventoryReceivedQty,
                        //        ReceivedQty = item.InventoryReceivedQty,
                        //    });
                        //}
                        //model.Products = indentViewProducts;
                        List<IndentProduct> products = new List<IndentProduct>();
                        foreach (var item in Products)
                        {
                            products.Add(new IndentProduct() { ProductId = item.ProductId, Quantity = item.InventoryReceivedQty });

                        }

                        model.PayableAmount = _viewModel.ProcessindentView.PayableAmount;
                        var _result = await _indentService.StatusChangeToReceived(model.Id, model.PayableAmount, products);
                        Application.Current?.Dispatcher?.Invoke(async () =>
                        {

                            if (_result != null && _result.IsSuccess)
                            {
                                _notificationService.ShowMessage(_result?.Data ?? "Indent " + _viewModel.NextStatus, Common.NotificationType.Success);

                                _eventAggregator.GetEvent<IntentNewTabCreateEvent>().Publish(model);

                            }
                            else
                            {
                                _notificationService.ShowMessage(_result?.Error ?? AppConstants.CommonError, Common.NotificationType.Error);
                            }

                            await _progressService.StopProgressAsync();
                        });


                    });
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

        private async void EditProductPriceClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {
                var _viewModel = (IndentApprovalViewModel)eventArgs.Parameter;
                if (_viewModel != null)
                {
                    await _progressService.StartProgressAsync();

                    await Task.Run(async () =>
                    {
                        IndentViewModel model = new IndentViewModel();
                        model.Products = _viewModel.AddSupplierToIndent.Products;
                        model.Id = PoId;
                        model.Indentstatus = IndentStatus.addsupplier;

                        var _result = await _indentService.EditProductPriceIndent(model);
                        Application.Current?.Dispatcher?.Invoke(async () =>
                        {

                            if (_result != null && _result.IsSuccess)
                            {
                                _notificationService.ShowMessage(_result?.Data ?? "Added Supplier", Common.NotificationType.Success);


                                _eventAggregator.GetEvent<IntentStatuschangeRefreshEvent>().Publish(model.Id);

                            }
                            else
                            {
                                _notificationService.ShowMessage(_result?.Error ?? AppConstants.CommonError, Common.NotificationType.Error);
                            }

                            await _progressService.StopProgressAsync();
                        });


                    });

                }
            }
            catch (Exception ex)
            {

                _logger.LogError("error in edit price popup", ex);
            }
        }

        public async void ExportXl()
        {
            try
            {
                if (ProcessindentView != null)
                {
                    await _progressService.StartProgressAsync();

                    await Task.Run(async () =>
                    {
                        var result = await _indentService.GetPurchaseOrderPDF(ProcessindentView.PoNumber);

                        if (result != null && result.IsSuccess)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {


                                try
                                {


                                    string path = @"c:\PosPurchaseOrder";

                                    var _info = Directory.CreateDirectory(path);

                                    var _fileName = path + "\\" + $"{result.Data.FileName}";

                                    if (!File.Exists(_fileName))
                                    {
                                        File.WriteAllBytes(_fileName, result.Data.FileStream);
                                        ProcessStartInfo _p = new ProcessStartInfo
                                        {
                                            UseShellExecute = true,
                                            FileName = _fileName
                                        };

                                        Process.Start(_p);

                                        return;
                                    }
                                    else
                                    {
                                        //Fall back to save  if file name already present
                                        SaveFileManually(result.Data.FileStream);
                                    }
                                }
                                catch (UnauthorizedAccessException _ex)
                                {
                                    _logger.LogError(_ex.Message);
                                    //fall back to save manually if file creation is blocked.
                                    SaveFileManually(result.Data.FileStream);
                                }
                                catch (Exception _ex)
                                {
                                    _logger.LogError(_ex.Message);
                                }



                            });

                        }
                        else
                        {
                            if (result != null && !result.IsSuccess && result.Error.IsValidString())
                            {
                                _notificationService.ShowMessage(result?.Error ?? AppConstants.CommonError, Common.NotificationType.Error);
                            }
                        }

                    });

                    await _progressService.StopProgressAsync();


                }
            }
            catch (Exception ex)
            {
                _logger.LogError("error in downloading purchase order pdf", ex);
            }
        }


        private void SaveFileManually(byte[] stream)
        {
            try
            {
                SaveFileDialog _sd = new SaveFileDialog();

                //_sd.FileName = result.Data.FileName;
                _sd.AddExtension = true;
                _sd.DefaultExt = ".pdf";

                _sd.ShowDialog();

                if (_sd.FileName.IsValidString())
                {

                    File.WriteAllBytes(_sd.FileName, stream);

                    ProcessStartInfo _proccess = new ProcessStartInfo
                    {
                        UseShellExecute = true,
                        FileName = _sd.FileName
                    };
                    Process.Start(_proccess);

                }
            }
            catch (Exception _ex)
            {

                _logger.LogError("getting error in save file method in indent", _ex);
            }
        }

        public async void GetIndent(IndentViewModel indentView)
        {
            try
            {
                if (indentView != null)
                {
                    await Task.Run(async () =>
                    {


                        var _result = await _indentService.ViewDetailIndent(indentView.Id);
                        Application.Current?.Dispatcher?.Invoke(() =>
                        {

                            if (_result != null && _result.IsSuccess)
                            {
                                GetIndentDetails(_result.Data);

                            }


                        });


                    });

                }
            }
            catch (Exception _ex)
            {

                _logger.LogError("getting error in get indent from api  in indent", _ex);
            }
        }


        private async void submitPopup(IndentApprovalViewModel viewModel)
        {
            try
            {
                List<AddPartialPayment> partialPayment = new List<AddPartialPayment>();

                await _progressService.StartProgressAsync();

                await Task.Run(async () =>
                {

                    AddSupplierModel model = new AddSupplierModel();
                    if (viewModel.AddSupplierToIndent.TotalAvailableQty > 0) {
                        if (viewModel.AddSupplierToIndent.PaymentType == "Partial")
                            partialPayment.Add(new AddPartialPayment() {
                                PaymentDate = viewModel.AddSupplierToIndent.PartialPayment.PaymentDate,
                                PaymentTotal = viewModel.AddSupplierToIndent.PartialPayment.PaymentTotal
                            });
                        else
                            partialPayment.Add(new AddPartialPayment() { PaymentDate = AddSupplierToIndent.CreditPeriod == "0" ? string.Format("{0:dd-MM-yyyy}", DateTime.Now) : AddSupplierToIndent.PaymentDate, PaymentTotal = AddSupplierToIndent.PayableAmount });

                        model.SupplierId = viewModel.AddSupplierToIndent.SupplierId;
                        model.ArrivingDate = Convert.ToDateTime(viewModel.AddSupplierToIndent.ArrivingDate);
                        model.CreditPeriod = viewModel.AddSupplierToIndent.PaymentType == "Partial" ? "NA" : viewModel.AddSupplierToIndent.CreditPeriod;
                        List<AddSupplierToIndentProduct> _products = new List<AddSupplierToIndentProduct>();
                        foreach (var product in viewModel.AddSupplierToIndent.Products) {
                            _products.Add(new AddSupplierToIndentProduct() {
                                AvailableQty = product.AvailableQty,
                                ProductId = product.ProductId,
                                EstimatedPrice = (decimal)product.EstimatedPrice,
                                SelectedGSTslab = product.SelectedGSTslab,
                                ETA = product.ETA,
                            });
                        }
                        model.Products = _products;
                        model.Id = PoId;
                        model.GSTType = viewModel.AddSupplierToIndent.GSTType;
                        model.FileUploadListInfo = viewModel.AddSupplierToIndent.FileUploadListInfo;
                        model.AdvisoryCharges = viewModel.AddSupplierToIndent.TransportCharges;
                        model.PaymentsList = partialPayment;
                        model.IsFullPaid = (viewModel.AddSupplierToIndent.PaymentType != "Partial"); //create enum dont hardcode

                    }
                    else {
                        model.SupplierId = viewModel.AddSupplierToIndent.SupplierId;
                        List<AddSupplierToIndentProduct> _products = new List<AddSupplierToIndentProduct>();
                        foreach (var product in viewModel.AddSupplierToIndent.Products) {
                            _products.Add(new AddSupplierToIndentProduct() {
                                AvailableQty = product.AvailableQty,
                                ProductId = product.ProductId,
                                EstimatedPrice = (decimal)product.EstimatedPrice,
                                SelectedGSTslab = product.SelectedGSTslab,
                                ETA = product.ETA,
                            });
                        }
                        model.Products = _products;
                        model.Id = PoId;
                        model.ArrivingDate = Convert.ToDateTime(viewModel.AddSupplierToIndent.ArrivingDate);
                        model.CreditPeriod = viewModel.AddSupplierToIndent.PaymentType == "Partial" ? "NA" : viewModel.AddSupplierToIndent.CreditPeriod;
                        model.GSTType = viewModel.AddSupplierToIndent.GSTType;

                    }


                    var _result = await _indentService.AddSupplierToIndent(model);
                    Application.Current?.Dispatcher?.Invoke(() =>
                    {

                        if (_result != null && _result.IsSuccess)
                        {
                            _notificationService.ShowMessage(_result?.Data ?? "Added Supplier", Common.NotificationType.Success);
                            var _model = new IndentViewModel()
                            {
                                Id = viewModel.PoId,

                            };
                            _eventAggregator.GetEvent<IntentNewTabCreateEvent>().Publish(_model);

                        }
                        else
                        {
                            _notificationService.ShowMessage(_result?.Error ?? AppConstants.CommonError, Common.NotificationType.Error);
                        }


                    });


                });
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

        public void Confirmationpopup(object param)
        {
            try
            {


                var TargetClose = ((Button)(param));
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

        private IndentViewModel _processIntentView = new IndentViewModel();
        public IndentViewModel ProcessindentView
        {
            get { return _processIntentView; }
            set
            { SetProperty(ref _processIntentView, value); }
        }


        private ObservableCollection<SuppliersViewModel> _suppliers;

        public ObservableCollection<SuppliersViewModel> Suppliers
        {
            get => _suppliers;
            set => SetProperty(ref _suppliers, value);
        }

        private AddSupplierToIndent _addSupplierToIndent = new AddSupplierToIndent();
        public AddSupplierToIndent AddSupplierToIndent
        {
            get => _addSupplierToIndent;

            set => SetProperty(ref _addSupplierToIndent, value);

        }


        private int _poId;
        public int PoId
        {
            get => _poId;
            set => SetProperty(ref _poId, value);
        }

        private int _activeStatus;

        public int ActiveStatus
        {
            get => _activeStatus;
            set => SetProperty(ref _activeStatus, value);
        }


        private string _currentStatus;
        public string CurrentStatus
        {
            get => _currentStatus;
            set => SetProperty(ref _currentStatus, value);
        }

        private string _nextStatus;
        public string NextStatus
        {
            get { return _nextStatus; }
            set
            {
                if (value != null)
                    _nextStatus = value;
                RaisePropertyChanged("NextStatus");
                if (_nextStatus == "Approve")
                    IsReasonVisibility = false;
                else
                    IsReasonVisibility = true;

            }
        }

        private string _reason;
        public string Reason
        {
            get => _reason;
            set => SetProperty(ref _reason, value);
        }

        private bool _isReasonVisibility;

        public bool IsReasonVisibility
        {
            get => _isReasonVisibility;
            set => SetProperty(ref _isReasonVisibility, value);
        }

        private string _trackingId;
        public string TrackingId
        {
            get => _trackingId;
            set => SetProperty(ref _trackingId, value);
        }

        private bool _trackingIdVisibility;
        public bool TrackingIdVisibility
        {
            get => _trackingIdVisibility;
            set => SetProperty(ref _trackingIdVisibility, value);
        }

        private ObservableCollection<IndentViewProduct> _products;

        public ObservableCollection<IndentViewProduct> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);

        }

        private bool _IsEnableBtnChangeStatus;

        public bool IsEnableBtnChangeStatus
        {
            get { return _IsEnableBtnChangeStatus; }
            set { SetProperty(ref _IsEnableBtnChangeStatus, value); }
        }

        private string _rejectStatus;

        public string RejectStatus
        {
            get { return _rejectStatus; }
            set
            {

                _rejectStatus = value;
                RaisePropertyChanged("RejectStatus");
            }
        }

        private string _Status;

        public string Status
        {
            get { return _Status; }
            set
            {

                _Status = value;
                RaisePropertyChanged("Status");
            }
        }

        private string _producteditreason;

        public string PropductEditReason
        {
            get { return _producteditreason; }
            set
            {

                _producteditreason = value;
                RaisePropertyChanged("PropductEditReason");
            }
        }

        private bool _isExportEnabled;
        public bool IsExportEnabled
        {
            get => _isExportEnabled;
            set => _ = SetProperty(ref _isExportEnabled, value);
        }

        private ObservableCollection<GSTslabs> GetGSTslabs()
        {
            if (ApplicationSettings.GST_VALUES != null && ApplicationSettings.GST_VALUES.Any())
            {
                return new ObservableCollection<GSTslabs>(ApplicationSettings
                    .GST_VALUES.Select(x => new GSTslabs { GstValue = x }));

            }

            return default;
        }

        private string _gstType;

        public string GSTType
        {
            get { return _gstType; }
            set
            {
                _gstType = value;
                RaisePropertyChanged("GSTType");
                if (_gstType == "Inclusive GST")
                {
                    if (Products != null && Products.Count > 0)
                    {
                        foreach (var item in Products)
                        {
                            item.IsInclusiveGst = true;
                            item.SelectedGSTslab = null;

                        };
                    }

                }
                else
                {
                    if (Products != null && Products.Count > 0)
                    {
                        foreach (var item in Products)
                        {
                            item.IsInclusiveGst = false;

                        };
                    }
                }
            }
        }

        public void EstimatePriceChangeCalculation(object obj)
        {
            float totalAmount = 0;
            float withoutGSTTotal = 0;
            float totalGst = 0;
            foreach (var item in AddSupplierToIndent.Products)
            {
                float estimatePrice = (float)Math.Round(item.EstimatedPrice, 2, MidpointRounding.AwayFromZero);
                if (item.SelectedGSTslab != null)
                {
                    totalAmount += ((((item.AvailableQty * estimatePrice) / 100) * item.SelectedGSTslab.GstValue) + item.AvailableQty * estimatePrice);
                    item.ProductTotal = ((((item.AvailableQty * estimatePrice) / 100) * item.SelectedGSTslab.GstValue) + item.AvailableQty * estimatePrice);
                    withoutGSTTotal += (item.AvailableQty * estimatePrice);
                    totalGst += (((item.AvailableQty * estimatePrice) / 100) * item.SelectedGSTslab.GstValue);
                }
                else
                {
                    totalAmount += (item.AvailableQty * estimatePrice);
                    item.ProductTotal = (item.AvailableQty * estimatePrice);
                }
            }

            AddSupplierToIndent.TotalAmount = (float)Math.Round(totalAmount, 0, MidpointRounding.AwayFromZero);
            AddSupplierToIndent.PayableAmount = (float)Math.Round(AddSupplierToIndent.TotalAmount, 0, MidpointRounding.AwayFromZero);
            AddSupplierToIndent.WithOutGst = (float)Math.Round(withoutGSTTotal, 0, MidpointRounding.AwayFromZero);
            AddSupplierToIndent.NetPayableWithOutGst = (float)Math.Round(withoutGSTTotal, 0, MidpointRounding.AwayFromZero);
            AddSupplierToIndent.GSTAmount = (float)Math.Round(totalGst, 0, MidpointRounding.AwayFromZero);
        }


        private async void FullPaymentEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {

                var _viewModel = (IndentApprovalViewModel)eventArgs.Parameter;
                if (_viewModel != null)
                {
                    await _progressService.StartProgressAsync();

                    await Task.Run(async () =>
                    {
                        IndentViewModel model = new IndentViewModel();

                        model.Indentstatus = (IndentStatus)Enum.Parse(typeof(IndentStatus), _viewModel.NextStatus, true);

                        model.Id = PoId;

                        model.Remark = Reason;

                        var _result = await _indentService.StatusChangeToPlaced(model.Id, model.Remark);

                        Application.Current?.Dispatcher?.Invoke(async () =>
                        {

                            if (_result != null && _result.IsSuccess)
                            {
                                _notificationService.ShowMessage(_result?.Data ?? "Indent " + ProperStatus.ProperStatusName(_viewModel.NextStatus), Common.NotificationType.Success);

                                _eventAggregator.GetEvent<IntentNewTabCreateEvent>().Publish(model);


                            }
                            else
                            {
                                _notificationService.ShowMessage(_result?.Error ?? AppConstants.CommonError, Common.NotificationType.Error);
                            }

                            await _progressService.StopProgressAsync();
                        });


                    });
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



        public void AddFileOpenDialog(object file)
        {
            try
            {
                var viewModel = ((IndentApprovalViewModel)file);



                if (viewModel.AddSupplierToIndent.FileUploadListInfo != null && viewModel.AddSupplierToIndent.FileUploadListInfo.Count >= 1)
                {
                    _notificationService.ShowMessage("1 files already added. Delete old file to reselect", NotificationType.Information);

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


                if (dialog != null)
                {
                    if (viewModel.AddSupplierToIndent.FileUploadListInfo == null)
                    {
                        viewModel.AddSupplierToIndent.FileUploadListInfo = new ObservableCollection<FileUploadInfo>();
                    }

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

                                if (viewModel.AddSupplierToIndent.FileUploadListInfo.Any(x => x.FileName == Path.GetFileName(_fileName)))
                                {
                                    _notificationService.ShowMessage($"File {Path.GetFileName(_fileName)} is already added", NotificationType.Information);

                                    _logger.LogWarning($"File was already added skipping file {_fileName}");

                                    continue;
                                }

                                _logger.LogInformation($"Adding file {_fileName}");

                                viewModel.AddSupplierToIndent.FileUploadListInfo?.Add(new FileUploadInfo
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



            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private void DeleteUploadFile(object obj)
        {
            try
            {
                var viewModel = ((FileUploadInfo)obj);
                if (viewModel != null)
                {
                    var _file = AddSupplierToIndent.FileUploadListInfo?.FirstOrDefault(x => x.FileId == viewModel.FileId);

                    if (_file != null)
                    {
                        _logger.LogInformation($"File Deleted {_file.FileName}");

                        AddSupplierToIndent.FileUploadListInfo?.Remove(_file);
                    }

                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        //private async void AddPayment(object pay)
        //{
        //    try
        //    {
        //        var viewmodel = (IndentApprovalViewModel)(pay);

        //        if (viewmodel.ProcessindentView.IsFullPaid == true)
        //        {
        //            _notificationService.ShowMessage("Payment Already Paid ", Common.NotificationType.Error);
        //            return;
        //        }
        //        if (viewmodel != null)
        //        {
        //            //await _progressService.StartProgressAsync();

        //            //await Task.Run(async () =>
        //            //{
        //            //    IndentViewModel model = new IndentViewModel();
        //            //    model.Indentstatus = (IndentStatus)Enum.Parse(typeof(IndentStatus),IndentStatus.fullpaid.ToString(), true);
        //            //    model.Id = viewmodel.ProcessindentView.Id;
        //            //    model.Remark = Reason;
        //            //    model.PayableAmount = viewmodel.ProcessindentView.PayableAmount;
        //            //    model.RefrenceNo = viewmodel.ProcessindentView.RefrenceNo;
        //            //    model.PaymentDate = viewmodel.ProcessindentView.PaymentDate;
        //            //    var _result = await _indentService.AddPaymentToIndnet(model);
        //            //    Application.Current?.Dispatcher?.Invoke(async () =>
        //            //    {

        //            //        if (_result != null && _result.IsSuccess)
        //            //        {
        //            //            _notificationService.ShowMessage(_result?.Data ?? "Indent " + ProperStatus.ProperStatusName(IndentStatus.fullpaid.ToString()), Common.NotificationType.Success);

        //            //            _eventAggregator.GetEvent<IntentNewTabCreateEvent>().Publish(model);

        //            //        }
        //            //        else
        //            //        {
        //            //            _notificationService.ShowMessage(_result?.Error ?? AppConstants.CommonError, Common.NotificationType.Error);
        //            //        }

        //            //        await _progressService.StopProgressAsync();
        //            //    });


        //            //});
        //            var _banklistresult = await _indentService.GetBankDetailsList(viewmodel.ProcessindentView.SupplierDetails.SupplierId, viewmodel.ProcessindentView.Id);
        //            if (_banklistresult != null && _banklistresult.IsSuccess)
        //            {
        //                IndentPayment payment = new IndentPayment();
        //                payment.DataContext = this;
        //                CurrentStatus = "";
        //                NextStatus = IndentStatus.fullpaid.ToString();
        //                PoId = viewmodel.ProcessindentView.Id;
        //                AddSupplierToIndent.SupplierId = viewmodel.ProcessindentView.SupplierId;
        //                AddSupplierToIndent.SelectedSupplierName = viewmodel.ProcessindentView.SupplierName;
        //                AddSupplierToIndent.BankDetails = _banklistresult.Data.ListOfBankDetails;
        //                AddSupplierToIndent.ReturnModels = _banklistresult.Data.ReturnModels;
        //                AddSupplierToIndent.SelectedBankDetail = null;
        //                AddSupplierToIndent.SelectedReturnModels = null;
        //                AddSupplierToIndent.RefrenceNo = null;
        //                AddSupplierToIndent.PaymentDate = null;
        //                var payamount = (viewmodel.ProcessindentView.PayableAmount);

        //                AddSupplierToIndent.PayableAmount = payamount;
        //                AddSupplierToIndent.FinalPayableAmount = payamount;

        //                await DialogHost.Show(payment, "RootDialog", AddPaymentColsingEventHandler);
        //            }
        //            else
        //            {
        //                _notificationService.ShowMessage("Please Add Bank Account Details to this Supplier", Common.NotificationType.Error);
        //                return;
        //            }

        //        }
        //    }
        //    catch (Exception _ex)
        //    {
        //        _logger.LogError(_ex.Message);
        //    }
        //}


        private async void DownloadFile(int? fileId)
        {
            if (fileId == null || fileId <= 0) return;

            try
            {
                await Task.Run(async () =>
                {
                    var _result = await _invoiceFileService.DownloadFile(fileId.Value);

                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        //null check
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

                    });

                });


            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in invoice model", _ex);
            }
        }


        public float EstimateCalculation(object obj)
        {
            try
            {
                float totalAmount = 0;
                var view = (List<IndentViewProduct>)obj;
                foreach (var item in view)
                {
                    if (item.SelectedGSTslab != null)
                    {
                        totalAmount += ((((item.AvailableQty * item.EstimatedPrice) / 100) * item.SelectedGSTslab.GstValue) + item.AvailableQty * item.EstimatedPrice);

                    }
                    else
                    {
                        totalAmount += (item.AvailableQty * item.EstimatedPrice);

                    }
                }

                return totalAmount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return 0;
            }

        }

        public void SelectedBankDetails(object obj)
        {
            try
            {
                var _bankDetail = (BankDetails)obj;
                if (_bankDetail != null)
                {

                    AddSupplierToIndent.BankDetails.Where(x => x.BankDetailsId != _bankDetail.BankDetailsId).ToList().ForEach(x => { x.IsSelected = false; });
                    AddSupplierToIndent.SelectedBankDetail = _bankDetail;
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }
        public void UnSelectedBankDetails(object obj)
        {
            try
            {

                AddSupplierToIndent.SelectedBankDetail = null;
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public void SelectedCreditnoteDetails(object obj)
        {
            try
            {
                var _returnModel = (StoreReturnModel)obj;

                if (_returnModel != null)
                {

                    var _selectedCreditNote = AddSupplierToIndent.ReturnModels.Where(x => x.CreditNoteNumber == _returnModel.CreditNoteNumber).FirstOrDefault();
                    if (_selectedCreditNote != null)
                        _selectedCreditNote.RedeemedAmount = 0;
                    var _selectedReturnList = AddSupplierToIndent.ReturnModels.Where(x => x.IsSelected == true).ToList();

                    var totalSelectedAmount = _selectedReturnList.Where(x => x.IsSelected).Sum(x => x.Total);

                    if (AddSupplierToIndent.PayableAmount < totalSelectedAmount)
                    {
                        foreach (var item in AddSupplierToIndent.ReturnModels)
                        {
                            if (item.Id == _returnModel.Id)
                            {
                                item.IsSelected = false;

                            }
                        }

                        _notificationService.ShowMessage("Credit amount should not greater then Net Amount", Common.NotificationType.Error);

                        return;
                    }



                    if (_selectedReturnList != null && _selectedReturnList.Count > 0)
                    {
                        AddSupplierToIndent.SelectedReturnModels = _selectedReturnList;

                        AddSupplierToIndent.FinalPayableAmount = AddSupplierToIndent.PayableAmount - totalSelectedAmount;
                    }


                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }
        public void UnSelectedCreditnoteDetails(object obj)
        {
            try
            {

                var _returnModel = (StoreReturnModel)obj;

                if (_returnModel != null)
                {

                    var selectedReturnList = AddSupplierToIndent.ReturnModels.Where(x => x.IsSelected == true).ToList();

                    if (selectedReturnList != null && selectedReturnList.Count > 0)
                    {
                        AddSupplierToIndent.SelectedReturnModels = selectedReturnList;
                        var totalSelectedAmount = selectedReturnList.Where(x => x.IsSelected).Sum(x => x.Total);
                        AddSupplierToIndent.FinalPayableAmount = AddSupplierToIndent.PayableAmount - totalSelectedAmount;
                    }
                    else
                    {
                        AddSupplierToIndent.SelectedReturnModels = null;

                        AddSupplierToIndent.FinalPayableAmount = AddSupplierToIndent.PayableAmount;
                    }

                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public void closeSelectedCreditnoteDetails(object obj)
        {
            try
            {

                var _returnModel = (StoreReturnModel)obj;

                if (_returnModel != null)
                {

                    var selectedReturnList = AddSupplierToIndent.SelectedReturnModels.Where(x => x.CreditNoteNumber != _returnModel.CreditNoteNumber).ToList();


                    AddSupplierToIndent.SelectedReturnModels = selectedReturnList;

                    var totalSelectedAmount = selectedReturnList.Where(x => x.IsSelected).Sum(x => x.Total);

                    AddSupplierToIndent.FinalPayableAmount = AddSupplierToIndent.PayableAmount - totalSelectedAmount;

                    foreach (var item in AddSupplierToIndent.ReturnModels)
                    {
                        if (item.Id == _returnModel.Id)
                        {
                            item.IsSelected = false;
                        }
                    }
                }



            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public void SelectionChangePercent(object obj)
        {
            try
            {
                var _viewModel = (IndentApprovalViewModel)obj;
                if (_viewModel != null && AddSupplierToIndent.PartialPayments == null)
                {

                    if (AddSupplierToIndent.GSTType == "Inclusive GST")
                    {
                        var total = AddSupplierToIndent.PartialPayment.Percentage != null ? ((AddSupplierToIndent.PayableAmount * (float)AddSupplierToIndent.PartialPayment.Percentage) / 100) : 0;
                        var paidAmount = AddSupplierToIndent.PartialPayments != null ? AddSupplierToIndent.PartialPayments.Sum(x => x.PaymentTotal) : 0;
                        if ((total + paidAmount) > AddSupplierToIndent.PayableAmount)
                        {
                            AddSupplierToIndent.PartialPayment.PaymentTotal = 0;
                            AddSupplierToIndent.PartialPayment.Percentage = null;
                        }

                        else
                        {
                            AddSupplierToIndent.PartialPayment.PaymentTotal = total;
                            AddSupplierToIndent.PartialPayment.Price = 0;

                        }
                    }
                    else
                    {
                        var total = AddSupplierToIndent.PartialPayment.Percentage != null ? ((AddSupplierToIndent.NetPayableWithOutGst * (float)AddSupplierToIndent.PartialPayment.Percentage) / 100) : 0;
                        var paidAmount = AddSupplierToIndent.PartialPayments != null ? AddSupplierToIndent.PartialPayments.Sum(x => x.PaymentTotal) : 0;
                        if ((total + paidAmount) > AddSupplierToIndent.PayableAmount)
                        {
                            AddSupplierToIndent.PartialPayment.PaymentTotal = 0;
                            AddSupplierToIndent.PartialPayment.Percentage = null;
                        }

                        else
                        {
                            AddSupplierToIndent.PartialPayment.PaymentTotal = total;
                            AddSupplierToIndent.PartialPayment.Price = 0;

                        }
                    }

                }


            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }


        public async void RemainPartialPayment(object obj)
        {
            try
            {
                var _remainPay = (IndentApprovalViewModel)obj;

                if (_remainPay != null)
                {

                    if (_remainPay.ProcessindentView.PaymentsList.Any() && _remainPay.ProcessindentView.PaymentsList != null)
                    {
                        foreach (var item in _remainPay.ProcessindentView.PaymentsList)
                        {
                            if ((string.IsNullOrEmpty(item.PayrefNo) || string.IsNullOrEmpty(item.PaidDate)) && item.NetPaidAmount != 0)
                            {
                                _notificationService.ShowMessage("Please update the last payment", Common.NotificationType.Error);

                                return;
                            }
                        }

                        PartialPaymentPopup partialPaymentPopup = new PartialPaymentPopup();
                        partialPaymentPopup.DataContext = this;
                        AddSupplierToIndent.PartialPayment.PaymentTotal = 0;
                        AddSupplierToIndent.PartialPayment.Price = 0;
                        AddSupplierToIndent.PartialPayment.PaymentDate = null;
                        AddSupplierToIndent.PartialPayment.Percentage = null;
                        AddSupplierToIndent.PayableAmount = (_remainPay.ProcessindentView.PayableAmount);
                        var balance = (_remainPay.ProcessindentView.PayableAmount - _remainPay.ProcessindentView.PaymentsList.Sum(x => x.PaymentTotal));
                        AddSupplierToIndent.PaymentType = balance == 0 ? "Full" : "Partial";
                        List<IndentViewProduct> listOfProduct = new List<IndentViewProduct>();
                        int i = 1;
                        foreach (var item in _remainPay.ProcessindentView.Products)
                        {
                            listOfProduct.Add(new IndentViewProduct()
                            {
                                SerailNumber = i,
                                BrandName = item.BrandName,
                                ProductType = item.ProductType,
                                ProductName = item.ProductName,
                                ProductId = item.ProductId,
                                EstimatedPrice = item.EstimatedPrice,
                                ReceivedQty = item.ReceivedQty,
                                AvailableQty = item.AvailableQty,
                                Quantity = item.Quantity,
                                SelectedGSTslab = item.SelectedGSTslab,
                                POSStockQty = item.POSStockQty,
                            });
                            i++;
                        }
                        AddSupplierToIndent.Products = listOfProduct;
                        EstimatePriceChangeCalculation(listOfProduct);
                        AddSupplierToIndent.TransportCharges = _remainPay.ProcessindentView.AdvisoryCharges;
                        AddSupplierToIndent.FileAttachments = _remainPay.ProcessindentView.FileAttachments;
                        Reason = null;
                        Status = _remainPay.ProcessindentView.Status;
                        AddSupplierToIndent.GSTType = listOfProduct.Select(x => x.SelectedGSTslab.GstType).FirstOrDefault() != null ? "Exclusive GST" : "Inclusive GST";
                        AddSupplierToIndent.TotalRequestQty = listOfProduct.Sum(x => x.Quantity);

                        AddSupplierToIndent.PartialPayments = new ObservableCollection<PartialPayment>(_remainPay.ProcessindentView.PaymentsList);
                        AddSupplierToIndent.InventoryDetail = _remainPay.ProcessindentView.IndentInventoryDetails.ToList().LastOrDefault();
                        AddSupplierToIndent.InventoryDetails = _remainPay.ProcessindentView.IndentInventoryDetails.ToList();
                        AddSupplierToIndent.InvoiceAgainst = new InvoiceAgainstModel();
                        AddSupplierToIndent.AgainstType = null; ;
                        AddSupplierToIndent.IsAgainstType = ((_remainPay.ProcessindentView.IndentInventoryDetails.Where(x => x.InvoiceNo != "Yet to add").ToList() != null && _remainPay.ProcessindentView.IndentInventoryDetails.Where(x => x.InvoiceNo != "Yet to add").ToList().Count > 0)) ? true : false;
                        await DialogHost.Show(partialPaymentPopup, "RootDialog", RemainPartialPaymentPopup);

                    }

                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private async void RemainPartialPaymentPopup(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {
                var _viewModel = (IndentApprovalViewModel)eventArgs.Parameter;

                if (_viewModel != null)
                {
                    if (_viewModel.AddSupplierToIndent.AgainstType == "Against Partial")
                    {
                        if (_viewModel.AddSupplierToIndent.PartialPayment.PaymentTotal == 0)
                        {
                            _notificationService.ShowMessage("please check payment amount", Common.NotificationType.Error);
                            return;
                        }

                        await _progressService.StartProgressAsync();

                        IndentViewModel model = new IndentViewModel();

                        model.Id = _viewModel.ProcessindentView.Id;

                        var _result = await _indentService.UpdatePartialPayment(_viewModel.ProcessindentView.Id, _viewModel.AddSupplierToIndent.PartialPayment.PaymentTotal, _viewModel.AddSupplierToIndent.PartialPayment.PaymentDate);
                        if (_result != null && _result.IsSuccess)
                        {
                            _notificationService.ShowMessage(_result.Data, Common.NotificationType.Success);
                            _eventAggregator.GetEvent<IntentNewTabCreateEvent>().Publish(model);
                        }
                        else
                        {
                            _notificationService.ShowMessage(_result.Error, Common.NotificationType.Error);
                        }

                        await _progressService.StopProgressAsync();
                    }
                    else
                    {


                        _viewModel.AddSupplierToIndent.PartialPayment.PaymentTotal = _viewModel.AddSupplierToIndent.InvoiceAgainst.BalancePay;

                        _viewModel.AddSupplierToIndent.PartialPayment.PaymentDate = DateTime.Now.Date.ToString("dd-MM-yyyy");


                        await _progressService.StartProgressAsync();

                        IndentViewModel model = new IndentViewModel();
                        model.Id = _viewModel.ProcessindentView.Id;

                        var _result = await _indentService.UpdatePartialPayment(_viewModel.ProcessindentView.Id, _viewModel.AddSupplierToIndent.PartialPayment.PaymentTotal, _viewModel.AddSupplierToIndent.PartialPayment.PaymentDate);
                        if (_result != null && _result.IsSuccess)
                        {
                            _notificationService.ShowMessage(_result.Data, Common.NotificationType.Success);
                            _eventAggregator.GetEvent<IntentNewTabCreateEvent>().Publish(model);
                        }
                        else
                        {
                            _notificationService.ShowMessage(_result.Error, Common.NotificationType.Error);
                        }

                        await _progressService.StopProgressAsync();
                    }

                }



            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);
                await _progressService.StopProgressAsync();
            }
            finally
            {

            }
        }

        public void SaveRemain(object obj)
        {
            try
            {
                if (string.IsNullOrEmpty(AddSupplierToIndent.AgainstType))
                {
                    _notificationService.ShowMessage("Please select partial or invoice", NotificationType.Error);
                    return;
                }
                if (AddSupplierToIndent.AgainstType == "Against Partial")
                {
                    if (AddSupplierToIndent.PartialPayment.Price == 0 && (AddSupplierToIndent.PartialPayment.Percentage == null || AddSupplierToIndent.PartialPayment.Percentage == 0))
                    {
                        _notificationService.ShowMessage("Please enter amount or percentage", NotificationType.Error);
                        return;
                    }

                    if (string.IsNullOrEmpty(AddSupplierToIndent.PartialPayment.PaymentDate))
                    {
                        _notificationService.ShowMessage("Please enter payment date", NotificationType.Error);
                        return;
                    }
                }

                if (AddSupplierToIndent.AgainstType == "Against Invoice")
                {
                    //foreach (var item in AddSupplierToIndent.InventoryDetails)
                    //{
                    //    if (!item.IsSelected)
                    //    {
                    //        _notificationService.ShowMessage("Please select invoice "+item.InvoiceNo, NotificationType.Error);
                    //        return;
                    //    }
                    //}

                    if (!AddSupplierToIndent.InventoryDetail.IsSelected)
                    {
                        _notificationService.ShowMessage("Please select invoice " + AddSupplierToIndent.InventoryDetail.InvoiceNo, NotificationType.Error);
                        return;
                    }
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
        public void PartialPriceChange(object obj)
        {
            try
            {
                var _viewModel = (IndentApprovalViewModel)obj;
                if (_viewModel != null && AddSupplierToIndent.PartialPayments == null)
                {
                    if (AddSupplierToIndent.GSTType == "Inclusive GST")
                    {
                        _viewModel.AddSupplierToIndent.PartialPayment.Percentage = null;
                        var _payAmount = AddSupplierToIndent.PayableAmount;
                        var _partialAmount = AddSupplierToIndent.PartialPayments != null ? AddSupplierToIndent.PartialPayments.Sum(x => x.PaymentTotal) : 0;
                        if (AddSupplierToIndent.PartialPayment.Price >= (_payAmount - _partialAmount))
                        {
                            AddSupplierToIndent.PartialPayment.Price = 0;
                            AddSupplierToIndent.PartialPayment.PaymentTotal = 0;
                        }
                        else
                        {
                            AddSupplierToIndent.PartialPayment.PaymentTotal = (float)Math.Round(AddSupplierToIndent.PartialPayment.Price, 2);
                            AddSupplierToIndent.PartialPayment.Percentage = null;
                        }
                    }
                    else
                    {
                        _viewModel.AddSupplierToIndent.PartialPayment.Percentage = null;
                        var _payAmount = AddSupplierToIndent.NetPayableWithOutGst;
                        var _partialAmount = AddSupplierToIndent.PartialPayments != null ? AddSupplierToIndent.PartialPayments.Sum(x => x.PaymentTotal) : 0;
                        if (AddSupplierToIndent.PartialPayment.Price >= (_payAmount - _partialAmount))
                        {
                            AddSupplierToIndent.PartialPayment.Price = 0;
                            AddSupplierToIndent.PartialPayment.PaymentTotal = 0;
                        }
                        else
                        {
                            AddSupplierToIndent.PartialPayment.PaymentTotal = (float)Math.Round(AddSupplierToIndent.PartialPayment.Price, 2);
                            AddSupplierToIndent.PartialPayment.Percentage = null;
                        }
                    }

                }



            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public void AddSupplierContinue()
        {
            try
            {
                if (AddSupplierToIndent.SelectedSupplier == null)
                {
                    _notificationService.ShowMessage("Please select supplier", Common.NotificationType.Error);
                    return;
                }

                if (!(bool)AddSupplierToIndent.SelectedSupplier?.Isenabled)
                {
                    _notificationService.ShowMessage("Selected supplier disabled", Common.NotificationType.Error);
                    return;
                }


                //if (string.IsNullOrEmpty(AddSupplierToIndent.ArrivingDate))
                //{
                //    _notificationService.ShowMessage("Please select Arriving Date", Common.NotificationType.Error);
                //    return;
                //}

                if (string.IsNullOrEmpty(AddSupplierToIndent.GSTType))
                {
                    _notificationService.ShowMessage("Please select Inclusive or Exclusive GST", Common.NotificationType.Error);

                    return;
                }

                if (AddSupplierToIndent.Products != null && AddSupplierToIndent.Products.Count > 0)
                {
                    EstimatePriceChangeCalculation(AddSupplierToIndent.Products);
                }

                _eventAggregator.GetEvent<StepSelectedPopIndexChangeEvent>().Publish((int)1);
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public void AddPriceContinue()
        {
            try
            {
                if (AddSupplierToIndent != null && AddSupplierToIndent.Products.Count > 0)
                {
                    int row = 1;
                    foreach (var item in AddSupplierToIndent.Products)
                    {
                        //if (item.AvailableQty == 0)
                        //{
                        //    _notificationService.ShowMessage("Please enter available qty at row " + row, Common.NotificationType.Error);
                        //    return;
                        //}                       

                        
                        if (item.AvailableQty > 0) {
                            if (AddSupplierToIndent.GSTType == "Exclusive GST") {
                                if (item.SelectedGSTslab == null) {
                                    _notificationService.ShowMessage("Please select GST value at row" + row, Common.NotificationType.Error);
                                    return;
                                }
                            }

                            if (Math.Round(item.EstimatedPrice, 2) == 0) {
                                _notificationService.ShowMessage("Please enter estimated price at row " + row, Common.NotificationType.Error);
                                return;
                            }

                            if (string.IsNullOrEmpty(item.ETA)) {
                                _notificationService.ShowMessage("Please enter ETA date at row " + row, Common.NotificationType.Error);
                                return;
                            }

                            
                        }
                        row++;

                        AddSupplierToIndent.TotalAvailableQty = AddSupplierToIndent.Products.Sum(x => x.AvailableQty);

                    }

                    //if(AddSupplierToIndent.FileUploadListInfo==null || AddSupplierToIndent.FileUploadListInfo.Count() == 0)
                    //{
                    //    _notificationService.ShowMessage("Please add Proforma Invoice", Common.NotificationType.Error);
                    //    return;
                    //}


                }


                _eventAggregator.GetEvent<StepSelectedPopIndexChangeEvent>().Publish((int)2);
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public void SecondSelectionChangePercent(object obj)
        {
            try
            {
                var _viewModel = (IndentApprovalViewModel)obj;
                if (_viewModel != null && AddSupplierToIndent.PartialPayments != null)
                {

                    if (AddSupplierToIndent.GSTType == "Inclusive GST")
                    {
                        var total = AddSupplierToIndent.PartialPayment.Percentage != null ? ((AddSupplierToIndent.PayableAmount * (float)AddSupplierToIndent.PartialPayment.Percentage) / 100) : 0;
                        var paidAmount = AddSupplierToIndent.PartialPayments != null ? AddSupplierToIndent.PartialPayments.Sum(x => x.PaymentTotal) : 0;
                        if ((total + paidAmount) > AddSupplierToIndent.PayableAmount)
                        {
                            AddSupplierToIndent.PartialPayment.PaymentTotal = 0;
                            AddSupplierToIndent.PartialPayment.Percentage = null;
                        }

                        else
                        {
                            AddSupplierToIndent.PartialPayment.PaymentTotal = total;
                            AddSupplierToIndent.PartialPayment.Price = 0;

                        }
                    }
                    else
                    {
                        var total = AddSupplierToIndent.PartialPayment.Percentage != null ? ((AddSupplierToIndent.NetPayableWithOutGst * (float)AddSupplierToIndent.PartialPayment.Percentage) / 100) : 0;
                        var paidAmount = AddSupplierToIndent.PartialPayments != null ? AddSupplierToIndent.PartialPayments.Sum(x => x.PaymentTotal) : 0;
                        if ((total + paidAmount) > AddSupplierToIndent.NetPayableWithOutGst)
                        {
                            AddSupplierToIndent.PartialPayment.PaymentTotal = 0;
                            AddSupplierToIndent.PartialPayment.Percentage = null;
                        }

                        else
                        {
                            var totalPay = (total + paidAmount);
                            if (AddSupplierToIndent.NetPayableWithOutGst == totalPay)
                            {
                                AddSupplierToIndent.PartialPayment.PaymentTotal = (total + ((AddSupplierToIndent.PayableAmount) - (total + paidAmount)));
                            }
                            else
                            {

                                AddSupplierToIndent.PartialPayment.PaymentTotal = total;
                                AddSupplierToIndent.PartialPayment.Price = 0;
                            }


                        }
                    }

                }


            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }
        public void SecondPartialPriceChange(object obj)
        {
            try
            {
                var _viewModel = (IndentApprovalViewModel)obj;
                if (_viewModel != null && AddSupplierToIndent.PartialPayments != null)
                {
                    if (AddSupplierToIndent.GSTType == "Inclusive GST")
                    {
                        _viewModel.AddSupplierToIndent.PartialPayment.Percentage = null;
                        var _payAmount = AddSupplierToIndent.PayableAmount;
                        var _partialAmount = AddSupplierToIndent.PartialPayments != null ? AddSupplierToIndent.PartialPayments.Sum(x => x.PaymentTotal) : 0;
                        if (AddSupplierToIndent.PartialPayment.Price > (_payAmount - _partialAmount))
                        {
                            AddSupplierToIndent.PartialPayment.Price = 0;
                            AddSupplierToIndent.PartialPayment.PaymentTotal = 0;
                        }
                        else
                        {
                            AddSupplierToIndent.PartialPayment.PaymentTotal = (float)Math.Round(AddSupplierToIndent.PartialPayment.Price, 2);
                            AddSupplierToIndent.PartialPayment.Percentage = null;
                        }
                    }
                    else
                    {
                        _viewModel.AddSupplierToIndent.PartialPayment.Percentage = null;
                        var _payAmount = AddSupplierToIndent.NetPayableWithOutGst;
                        var _partialAmount = AddSupplierToIndent.PartialPayments != null ? AddSupplierToIndent.PartialPayments.Sum(x => x.PaymentTotal) : 0;
                        if ((AddSupplierToIndent.PartialPayment.Price + _partialAmount) > (AddSupplierToIndent.PayableAmount))
                        {
                            AddSupplierToIndent.PartialPayment.Price = 0;
                            AddSupplierToIndent.PartialPayment.PaymentTotal = 0;
                        }
                        else
                        {
                            var totalPay = (_partialAmount + AddSupplierToIndent.PartialPayment.Price);
                            if (AddSupplierToIndent.NetPayableWithOutGst == totalPay)
                            {
                                AddSupplierToIndent.PartialPayment.PaymentTotal = (((float)Math.Round(AddSupplierToIndent.PartialPayment.Price, 2)) + (AddSupplierToIndent.PayableAmount - (((float)Math.Round(AddSupplierToIndent.PartialPayment.Price, 2)) + _partialAmount)));
                            }
                            else
                            {

                                AddSupplierToIndent.PartialPayment.PaymentTotal = (float)Math.Round(AddSupplierToIndent.PartialPayment.Price, 2);
                                AddSupplierToIndent.PartialPayment.Percentage = null;
                            }
                        }
                    }

                }



            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }


        public void SelectedInvoiceAgainst(object obj)
        {
            try
            {
                var view = (IndentInventoryDetails)obj;
                if (view != null)
                {
                    var totalAmount = AddSupplierToIndent.InventoryDetails.Sum(x => x.TotalAmount);

                    if (totalAmount < AddSupplierToIndent.PayableAmount)
                    {
                        AddSupplierToIndent.InvoiceAgainst.TotalInvoiceAmount = totalAmount;
                        AddSupplierToIndent.InvoiceAgainst.TotalPartiallyPaid = 0;
                        AddSupplierToIndent.InvoiceAgainst.BalancePay = AddSupplierToIndent.InvoiceAgainst.TotalInvoiceAmount;
                    }
                    else if (totalAmount == AddSupplierToIndent.PayableAmount)
                    {
                        AddSupplierToIndent.InvoiceAgainst.TotalInvoiceAmount = totalAmount;
                        AddSupplierToIndent.InvoiceAgainst.TotalPartiallyPaid = AddSupplierToIndent.PartialPayments.Sum(x => x.PaymentTotal);
                        AddSupplierToIndent.InvoiceAgainst.BalancePay = AddSupplierToIndent.InvoiceAgainst.TotalInvoiceAmount - AddSupplierToIndent.InvoiceAgainst.TotalPartiallyPaid;
                    }

                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public void UnSelectedInvoiceAgainst(object obj)
        {
            try
            {
                var view = (IndentInventoryDetails)obj;
                if (view != null)
                {
                    var totalAmount = AddSupplierToIndent.InventoryDetails.Sum(x => x.TotalAmount);

                    if (totalAmount < AddSupplierToIndent.PayableAmount)
                    {
                        AddSupplierToIndent.InvoiceAgainst.TotalInvoiceAmount = 0;
                        AddSupplierToIndent.InvoiceAgainst.TotalPartiallyPaid = 0;
                        AddSupplierToIndent.InvoiceAgainst.BalancePay = AddSupplierToIndent.InvoiceAgainst.TotalInvoiceAmount != 0 ? (AddSupplierToIndent.InvoiceAgainst.TotalInvoiceAmount - AddSupplierToIndent.InvoiceAgainst.TotalPartiallyPaid) : 0;
                    }
                    else if (totalAmount == AddSupplierToIndent.PayableAmount)
                    {
                        AddSupplierToIndent.InvoiceAgainst.TotalInvoiceAmount = 0;
                        AddSupplierToIndent.InvoiceAgainst.TotalPartiallyPaid = 0;
                        AddSupplierToIndent.InvoiceAgainst.BalancePay = 0;
                    }


                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        private async void ChangeClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {

                var _viewModel = (IndentApprovalViewModel)eventArgs.Parameter;
                if (_viewModel != null)
                {
                    await _progressService.StartProgressAsync();

                    await Task.Run(async () =>
                    {
                        IndentViewModel model = new IndentViewModel();
                        model.Indentstatus = (IndentStatus)Enum.Parse(typeof(IndentStatus), _viewModel.NextStatus, true);
                        model.Id = PoId;
                        model.Remark = Reason;
                        if (_viewModel.NextStatus == IndentStatus.intransit.ToString())
                        {
                            model.TrackingId = TrackingId;
                            model.Remark = Reason;
                        }


                        var _result = await _indentService.StatusChangeClose(model.Id, model.Remark);
                        Application.Current?.Dispatcher?.Invoke(async () =>
                        {

                            if (_result != null && _result.IsSuccess)
                            {
                                _notificationService.ShowMessage(_result?.Data ?? "Indent " + ProperStatus.ProperStatusName(_viewModel.NextStatus), Common.NotificationType.Success);

                                _eventAggregator.GetEvent<IntentNewTabCreateEvent>().Publish(model);
                            }
                            else
                            {
                                _notificationService.ShowMessage(_result?.Error ?? AppConstants.CommonError, Common.NotificationType.Error);
                            }

                            await _progressService.StopProgressAsync();
                        });


                    });
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

        private void AddNewIndentProduct()
        {

            var _product = _container.Resolve<IndentProductCardViewModel>();
            _product.SlNo = SlNo;
            IndentProducts.Add(_product);
            SlNo++;
        }

        private void RemoveIndentProduct(object obj)
        {
            if (obj is Guid _productGUIDId)
            {
                var _productRemove = IndentProducts?.FirstOrDefault(x => x.IndentProductGUIDId == _productGUIDId);

                if (_productRemove != null)
                {
                    IndentProducts.Remove(_productRemove);
                    SlNo--;
                }
            }
        }


        private void IndentProducts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("IndentProducts");
        }

        private ObservableCollection<IndentProductCardViewModel> _indentProducts;
        public ObservableCollection<IndentProductCardViewModel> IndentProducts
        {
            get => _indentProducts;
            set => SetProperty(ref _indentProducts, value);
        }

        public void AddReview(object obj)
        {
            try
            {

                if (ReviewProducts != null && ReviewProducts.Count() > 0)
                {
                    int i = 1;
                    foreach (var item in ReviewProducts)
                    {

                        if (NextStatus == IndentStatus.review.ToString())
                        {
                            if (item.TmQty == null)
                            {
                                _notificationService.ShowMessage("Please enter tm qty at row " + i, NotificationType.Error);
                                return;
                            }
                        }
                        if (NextStatus == IndentStatus.approve.ToString())
                        {

                            if (item.RmQty == null)
                            {
                                _notificationService.ShowMessage("Please enter rm qty at row " + i, NotificationType.Error);
                                return;
                            }
                            if (item.RmQty > 0) {
                                if (item.Rsp == 0) {
                                    _notificationService.ShowMessage("Please enter rsp at row " + i, NotificationType.Error);
                                    return;
                                }
                            }
                            
                        }
                        i++;
                    }
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

        private async void ReviewClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            try
            {

                var _viewModel = (IndentApprovalViewModel)eventArgs.Parameter;

                if (_viewModel != null)
                {
                    if (ReviewProducts != null && ReviewProducts.Count > 0)
                    {
                        if (NextStatus == IndentStatus.review.ToString())
                        {
                            await _progressService.StartProgressAsync();

                            await Task.Run(async () =>
                            {
                                IndentViewModel model = new IndentViewModel();
                                model.Indentstatus = (IndentStatus)Enum.Parse(typeof(IndentStatus), _viewModel.NextStatus, true);
                                model.Id = PoId;
                                var _reviewProduct = new List<IndentReviewProduct>();
                                foreach (var item in ReviewProducts)
                                {
                                    _reviewProduct.Add(new IndentReviewProduct()
                                    {
                                        ProductId = item.ProductId,
                                        Qty = item.Quantity,
                                        TmQty = (int)item.TmQty,

                                    });
                                }



                                var _result = await _indentService.IndentReview(model.Id, _reviewProduct);
                                Application.Current?.Dispatcher?.Invoke(async () =>
                                {

                                    if (_result != null && _result.IsSuccess)
                                    {
                                        _notificationService.ShowMessage(_result?.Data ?? "Indent " + ProperStatus.ProperStatusName(_viewModel.NextStatus), Common.NotificationType.Success);


                                    }
                                    else
                                    {
                                        _notificationService.ShowMessage(_result?.Error ?? AppConstants.CommonError, Common.NotificationType.Error);
                                    }

                                    await _progressService.StopProgressAsync();
                                });


                            });
                        }
                        else if (NextStatus == IndentStatus.approve.ToString())
                        {
                            await _progressService.StartProgressAsync();

                            await Task.Run(async () =>
                            {
                                IndentViewModel model = new IndentViewModel();
                                model.Indentstatus = (IndentStatus)Enum.Parse(typeof(IndentStatus), _viewModel.NextStatus, true);
                                model.Id = PoId;
                                var _reviewProduct = new List<IndentApproveProduct>();
                                foreach (var item in ReviewProducts)
                                {
                                    _reviewProduct.Add(new IndentApproveProduct()
                                    {
                                        ProductId = item.ProductId,
                                        Qty = item.Quantity,
                                        RmQty = (int)item.RmQty,
                                        Rsp = item.Rsp,


                                    });
                                }





                                var _result = await _indentService.IndentApprove(model.Id, _reviewProduct);
                                Application.Current?.Dispatcher?.Invoke(async () =>
                                {

                                    if (_result != null && _result.IsSuccess)
                                    {
                                        _notificationService.ShowMessage(_result?.Data ?? "Indent " + ProperStatus.ProperStatusName(_viewModel.NextStatus), Common.NotificationType.Success);


                                    }
                                    else
                                    {
                                        _notificationService.ShowMessage(_result?.Error ?? AppConstants.CommonError, Common.NotificationType.Error);
                                    }

                                    await _progressService.StopProgressAsync();
                                });


                            });
                        }
                    }


                }



            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);
            }
            finally {
                await _progressService.StopProgressAsync();
            }
        }

        public void AddProductTo(object obj)
        {
            try
            {
                if (ReviewProducts != null && ReviewProducts.Count() > 0)
                {

                    if (IndentProducts != null && IndentProducts.Count() > 0)
                    {


                        foreach (var item in IndentProducts)
                        {
                            if (item.SelectedProduct == null)
                            {
                                _notificationService.ShowMessage("Please select the product", NotificationType.Error);
                                return;
                            }

                            if (item.Quantity == 0)
                            {
                                _notificationService.ShowMessage("Please enter quantity", NotificationType.Error);
                                return;
                            }
                            if (ReviewProducts.Where(x => x.ProductId == item.SelectedProduct.ProductId).FirstOrDefault() == null)
                            {
                                var _addProduct = new IndentViewProduct()
                                {

                                    ProductId = (int)item.SelectedProduct.ProductId,
                                    ProductName = item.SelectedProduct.Name,
                                    BrandName = item.SelectedProduct.Manufacturer.Name,
                                    Quantity = item.Quantity,
                                    IsRamQtyVisible = AppConstants.ROLE_REGIONAL_MANAGER == AppConstants.USER_ROLES[0] ? true : false,
                                    SerailNumber = (ReviewProducts != null && ReviewProducts.Count > 0) ? (ReviewProducts.Count() + 1) : 1,
                                    TmQty = item.Quantity,

                                };
                                ReviewProducts.Add(_addProduct);
                            }
                            else
                            {
                                _notificationService.ShowMessage("This product already added", NotificationType.Error);
                                return;
                            }
                        }

                        IndentProducts.Clear();
                    }

                }




            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }


        private int _slNo;

        public int SlNo
        {
            get => _slNo;
            set => SetProperty(ref _slNo, value);
        }

        private ObservableCollection<IndentViewProduct> _reviewProducts;

        public ObservableCollection<IndentViewProduct> ReviewProducts
        {
            get => _reviewProducts;
            set => SetProperty(ref _reviewProducts, value);
        }

    }
}



