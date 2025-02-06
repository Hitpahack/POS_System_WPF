using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Suppliers;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.Suppliers.ViewModels
{
    public class SupplierListViewModel : BindableBase
    {
        private readonly ISupplierService _supplierService;

        private readonly INotificationService _notificationService;

        private readonly ProgressService _ProgressService;

        public DelegateCommand<object> GetSupplieDetailCommand { get; private set; }


        public DelegateCommand<object> CreateNewSuppliertabCommand { get; private set; }

        private readonly ICommonService _commonService;

        private readonly Logger _logger;

        private IEventAggregator _eventAggregator;

        public DelegateCommand<object> RefreshSupplierlistCommand { get; private set; }


        public SupplierListViewModel(IEventAggregator EventAggregator, ISupplierService SupplierService, ICommonService CommonService, INotificationService NotificationService, ProgressService ProgressService, Logger Logger)
        {
            _supplierService = SupplierService ?? throw new ArgumentNullException(nameof(SupplierService));

            _notificationService = NotificationService ?? throw new ArgumentNullException(nameof(NotificationService));

            _ProgressService = ProgressService ?? throw new ArgumentNullException(nameof(ProgressService));

            _logger = Logger ?? throw new ArgumentNullException(nameof(Logger));

            GetSupplieDetailCommand = new DelegateCommand<object>(GetSupplierExcel);


            _commonService = CommonService ?? throw new ArgumentNullException(nameof(CommonService));

            CreateNewSuppliertabCommand = new DelegateCommand<object>(OpenNewsupplierDetails);

            _eventAggregator = EventAggregator ?? throw new ArgumentNullException(nameof(EventAggregator));

            _eventAggregator.GetEvent<SupplierNewTabCreateEvent>().Subscribe(SupplierList);

            RefreshSupplierlistCommand = new DelegateCommand<object>(RefreshSupplierList);

            LoadSuppliers();

        }

        public void RefreshSupplierList(object supplier)
        {
            try
            {
                LoadSuppliers();
                _eventAggregator.GetEvent<SupplierTabRemoveEvent>().Publish();


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }


        public void SupplierList(SuppliersDetailsViewModel supplier)
        {
            try
            {
                LoadSuppliers();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }


        public void OpenNewsupplierDetails(object supplier)
        {
            try
            {
                var viewModel = (SuppliersViewModel)supplier;
                if (viewModel != null)
                {
                    SuppliersDetailsViewModel suppliersDetails = new SuppliersDetailsViewModel()
                    {
                        SupplierId = (int)viewModel.SupplierId
                    };
                    _eventAggregator.GetEvent<SupplierNewTabCreateEvent>().Publish(suppliersDetails);

                }


            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }



        public async void GetSupplierExcel(object supplier)
        {
            try
            {
                var listViewModel = (SupplierListViewModel)supplier;

                if (listViewModel != null)
                {
                    if (listViewModel.Suppliers == null && listViewModel.Suppliers.Count == 0)
                    {
                        _notificationService.ShowMessage("No records found to export", Common.NotificationType.Error);

                        return;
                    }
                    List<ExportSupplier> listofsupplier = new List<ExportSupplier>();
                    foreach (var item in listViewModel.Suppliers)
                    {
                        listofsupplier.Add(new ExportSupplier()
                        {
                            SupplierName = item.Name,
                            GSTNumber = item.GstNumber,
                            PAN = item.PAN,
                            PhoneNumber = item.Address?.Phone,
                            Email = item.Address?.Email,
                            Suppliertype = item.Suppliertype,
                            TallyCode = item.TallyCode,
                            District = item.Address?.District,
                            State = item.Address?.State,
                            Pincode = Convert.ToString(item.Address?.Pincode),
                            MSMERegisted = item.IsMSME,
                            MSMENumber = item.MSME,
                            BankName = item.BankDetails?.Bank?.BankName,
                            BranchName = item.BankDetails?.BrnachName,
                            AccountNo = item.BankDetails?.AccountNo,
                            AccountType = item.BankDetails?.AccountType,
                            IFSC = item.BankDetails?.IFSC,
                        });
                    }



                    if (!_fileName.IsValidString())
                    {
                        _fileName = "Suppler details";
                    }
                    _fileName += Guid.NewGuid().ToString().Substring(0, 3);

                    await _ProgressService.StartProgressAsync();

                    bool _result = _commonService.ExportToXL(listofsupplier, _fileName, skipfileName: true);

                    if (_result)
                    {

                        _notificationService.ShowMessage("File exported to folderC:\\FALCAPOS\\PosReports", Common.NotificationType.Success);

                    }
                    else
                    {
                        _notificationService.ShowMessage("File export failed , try again", Common.NotificationType.Error);
                    }
                    await _ProgressService.StopProgressAsync();

                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public string _fileName { get; set; }
        private ObservableCollection<SuppliersViewModel> suppliers;

        public ObservableCollection<SuppliersViewModel> Suppliers
        {
            get => suppliers;
            set => SetProperty(ref suppliers, value);
        }

        private async void LoadSuppliers()
        {
            try
            {
                Suppliers?.Clear();

                await _ProgressService.StartProgressAsync();

                await Task.Run(async () =>
                {
                    var _result = await _supplierService.GetSuppliers();

                    if (_result != null && _result.Any())
                    {
                        Application.Current.Dispatcher?.Invoke(() =>
                        {
                            Suppliers = new ObservableCollection<SuppliersViewModel>(_result);

                            Suppliers?.OrderBy(x => x.Name);
                        });
                        RowCount = "Row Count " + Suppliers.Count;
                    }
                });

                await _ProgressService.StopProgressAsync();
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);

                await _ProgressService.StopProgressAsync();

            }


        }

        private string rowCount;

        public string RowCount
        {
            get { return rowCount; }
            set { SetProperty(ref rowCount, value); }
        }


    }

    public class ExportSupplier
    {
        public string SupplierName { get; set; }

        public string GSTNumber { get; set; }

        public string PAN { get; set; }
        public string Suppliertype { get; set; }

        public string TallyCode { get; set; }
        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string District { get; set; }

        public string State { get; set; }


        public string Pincode { get; set; }
        public string MSMERegisted { get; set; }
        public string MSMENumber { get; set; }

        public string BankName { get; set; }

        public string BranchName { get; set; }

        public string AccountNo { get; set; }

        public string IFSC { get; set; }

        public string AccountType { get; set; }
    }
}
