using FalcaPOS.Common;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.AddInventory;
using FalcaPOS.Entites.Suppliers;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.AddInventory.ViewModels
{
    public class InwardDamageViewModel : BindableBase
    {
        private readonly IInvoiceService _invoiceService;

        private CancellationTokenSource _cancellationTokenSource { get; set; }
        private readonly INotificationService _notificationService;


        public DelegateCommand RefreshDataGrid { get; set; }

        public DelegateCommand<object> SearchParams { get; set; }

        private readonly ISupplierService _supplierService;
        public DelegateCommand<object> RowDoubleClickCommand { get; private set; }

        private readonly Logger _logger;


        public InwardDamageViewModel(IInvoiceService invoiceService, Logger logger, ISupplierService supplierService, INotificationService notificationService)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _invoiceService = invoiceService ?? throw new ArgumentNullException(nameof(invoiceService));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _cancellationTokenSource = new CancellationTokenSource();

            RefreshDataGrid = new DelegateCommand(RefreshGrid);

            SearchParams = new DelegateCommand<object>(loadSearch);

            _supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));

            RowDoubleClickCommand = new DelegateCommand<object>(GetProdutDetails);

            LoadDefectiveList(InvoiceModelRequest);

            LoadSuppliers();

        }


        public async void GetProdutDetails(object obj)
        {
            try
            {
                var model = obj as InvoiceListViewModel;

                if (model == null)
                {
                    //return invalid details

                    return;
                }

                if (model.IsSelectedShallow)
                {
                    //Grid row  is already opend collapse .
                    model.IsSelectedShallow = false;
                    model.IsSelected = false;
                    return;
                }

                //check if already information is fetched 

                if (model.HasInformation)
                {
                    //display and return
                    model.IsSelectedShallow = true;
                    model.IsSelected = true;
                    return;
                }



                if (!string.IsNullOrEmpty(model.InvoiceNumber))
                {
                    model.IsBusy = true;
                    model.IsSelectedShallow = true;
                    model.IsSelected = true;


                    await Task.Run(async () =>
                    {

                        var _result = await _invoiceService.GetProductDetails(model.InvoiceNumber);
                        Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            if (_result != null && _result.Data != null)
                            {

                                if (model != null)
                                {
                                    model.DefectiveProductList = _result.Data;
                                    model.IsBusy = false;
                                }
                            }
                            else
                            {
                                model.IsBusy = false;

                            }

                        });

                    });
                }

            }

            catch (Exception _ex)
            {
                _logger?.LogError("Error in inward damage view model", _ex);
            }

        }
        public void RefreshGrid()
        {
            InvoiceModelRequest invoiceModelRequest = new InvoiceModelRequest();
            invoiceModelRequest.FromDate = null;
            invoiceModelRequest.ToDate = null;
            invoiceModelRequest.SupplierName = null;
            invoiceModelRequest.Invoiceno = null;
            FromDate = null;
            ToDate = null;
            InvoiceNumber = null;
            SupplierName = null;
            LoadDefectiveList(InvoiceModelRequest);
        }
        public void loadSearch(object obj)
        {
            DateTime? dt1 = null;
            DateTime? dt2 = null;
            if (!string.IsNullOrEmpty(FromDate))
            {
                dt1 = DateTime.Parse(FromDate);
            }
            if (!string.IsNullOrEmpty(ToDate))
            {
                dt2 = DateTime.Parse(ToDate);
            }
            if (dt1 != null && dt2 != null)
            {
                if (dt2 < dt1)
                {
                    _notificationService.ShowMessage("From date should be less than or equal to To date", NotificationType.Error);
                    return;
                }
            }
            string suppliername = SupplierName != null ? SupplierName.Name : null;
            InvoiceModelRequest invoiceModelRequest = new InvoiceModelRequest();
            invoiceModelRequest.FromDate = dt1;
            invoiceModelRequest.ToDate = dt2;
            invoiceModelRequest.SupplierName = string.IsNullOrEmpty(suppliername) ? null : suppliername;
            invoiceModelRequest.Invoiceno = string.IsNullOrEmpty(InvoiceNumber) ? null : InvoiceNumber;

            if (string.IsNullOrEmpty(FromDate) && string.IsNullOrEmpty(ToDate) && string.IsNullOrEmpty(suppliername) && string.IsNullOrEmpty(InvoiceNumber))
            {
                _notificationService.ShowMessage("please enter the value any one field", NotificationType.Error);
                return;
            }
            LoadDefectiveList(invoiceModelRequest);


        }

        private async void LoadSuppliers()
        {

            try
            {


                await Task.Run(async () =>
                {
                    var _result = await _supplierService.GetSuppliers("isenabled=true");

                    if (_result != null && _result.Any())
                    {
                        Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            Suppliers = new ObservableCollection<SuppliersViewModel>(_result.OrderBy(x => x.Name));
                           

                        });
                    }

                });

            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in inward damage view model", _ex);
            }
        }
        public async void LoadDefectiveList(InvoiceModelRequest invoiceModelRequest)
        {


            if (_cancellationTokenSource != null)
                _cancellationTokenSource.Cancel();

            _cancellationTokenSource = new CancellationTokenSource();


            await Task.Run(async () =>
            {
                var _result = await _invoiceService.GetDefectiveLsit(invoiceModelRequest);

                if (_result != null && _result.IsSuccess)
                {
                    DefectiveList = _result.Data.invoiceListViews.OrderByDescending(x => x.InvoiceDate).ToList();
                }
                else
                {
                    DefectiveList = null;
                }



            });



        }


        private List<InwardInvoiceDamageViewModel> _defectiveList;
        public List<InwardInvoiceDamageViewModel> DefectiveList
        {
            get { return _defectiveList; }

            set { SetProperty(ref _defectiveList, value); }
        }


        private InvoiceModelRequest _invoiceModelRequest = new InvoiceModelRequest();
        public InvoiceModelRequest InvoiceModelRequest
        {
            get { return _invoiceModelRequest; }

            set { SetProperty(ref _invoiceModelRequest, value); }
        }


        private string _fromdate;
        public string FromDate
        {
            get
            {
                return _fromdate;
            }

            set
            {
                SetProperty(ref _fromdate, value);
            }
        }

        private string _todate;
        public string ToDate
        {
            get
            {
                return _todate;
            }
            set
            {
                SetProperty(ref _todate, value);
            }
        }

        private string _invoiceno;
        public string InvoiceNumber
        {
            get { return _invoiceno; }

            set { SetProperty(ref _invoiceno, value); }
        }


        private Supplier _suplier;
        public Supplier SupplierName { get { return _suplier; } set { SetProperty(ref _suplier, value); } }

        private ObservableCollection<SuppliersViewModel> _suppliers;

        public ObservableCollection<SuppliersViewModel> Suppliers
        {
            get => _suppliers;
            set => SetProperty(ref _suppliers, value);
        }



    }


}
