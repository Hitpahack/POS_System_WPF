using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Director;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.Director.ViewModel
{
    public class PurchaseRateViewModel : BindableBase
    {
        public DelegateCommand SerachFlyout { get; private set; }
        public DelegateCommand RefreshDataGrid { get; private set; }
        private IEventAggregator _eventAggregator { get; set; }

        private readonly IDirectorService _directorService;

        private readonly INotificationService _notificationService;

        public DelegateCommand ExportResultToExcelCommand { get; private set; }

        private readonly ICommonService _commonService;
        private readonly ProgressService _ProgressService;

        public PurchaseRateViewModel(ProgressService ProgressService, ICommonService commonService, IEventAggregator eventAggregator, IDirectorService directorService, INotificationService notificationService)
        {
            _eventAggregator = eventAggregator;

            _directorService = directorService ?? throw new ArgumentNullException(nameof(directorService));
            _ProgressService = ProgressService;

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));


            SerachFlyout = new DelegateCommand(SearchFlyout);

            RefreshDataGrid = new DelegateCommand(RefreshGrid);

            _eventAggregator.GetEvent<PurchaseRateSearch>().Subscribe((item) => LoadData(item));

            ExportResultToExcelCommand = new DelegateCommand(ExportResultToExcel);

            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));


        }


        public void ExportResultToExcel()
        {
            try
            {
                if (PurchaseList == null || !PurchaseList.Any())
                {
                    _notificationService.ShowMessage("No records found to export", Common.NotificationType.Error);

                    return;
                }

                IsExportEnabled = false;


                string filename = "PurchaseRateList" + "_" + DateTime.Now.ToString("dd-MM-yyyy");

                bool _result = _commonService.ExportToXL(PurchaseList, filename, skipfileName: false);

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
            catch (Exception ex)
            {

            }

        }

        public void SearchFlyout()
        {
            _eventAggregator.GetEvent<PurchaseRateSearchFlyOut>().Publish(true);
        }

        public void RefreshGrid()
        {
            PurchaseList = null;
            TotalCount = 0;
            _eventAggregator.GetEvent<PurchaseRateSearchFlyOut>().Publish(false);
        }

        public async void LoadData(PurchaseRateSearchModel purchaseRateSearch)
        {
            try
            {
                await _ProgressService.StartProgressAsync();
                await Task.Run(async () =>
                {

                    var _result = await _directorService.GetPurchaseRateListServices(purchaseRateSearch);

                    if (_result != null && _result.IsSuccess && _result.Data.Any())
                    {
                        Application.Current?.Dispatcher?.Invoke(async () =>
                        {

                            if (_result != null && _result.IsSuccess && _result.Data != null)
                            {
                                PurchaseList = _result.Data.ToList();
                                TotalCount = _result.Data.ToList().Count;
                                IsExportEnabled = true;
                                await _ProgressService.StopProgressAsync();

                            }
                            else
                            {
                                _notificationService.ShowMessage(_result?.Error ?? "No records found", Common.NotificationType.Error);
                                TotalCount = 0;
                                PurchaseList = null;
                                await _ProgressService.StopProgressAsync();
                            }

                        });

                    }
                    else

                    {

                        _notificationService.ShowMessage(_result?.Error ?? "No records found", Common.NotificationType.Error);
                        PurchaseList = null;
                        TotalCount = 0;
                        await _ProgressService.StopProgressAsync();
                    }


                });
            }
            catch (Exception ex)
            {

            }
        }
        private bool _isExportEnabled;
        public bool IsExportEnabled
        {
            get => _isExportEnabled;
            set => _ = SetProperty(ref _isExportEnabled, value);
        }

        private List<PurchaseRateModel> _purchaseList;

        public List<PurchaseRateModel> PurchaseList
        {
            get { return _purchaseList; }
            set { SetProperty(ref _purchaseList, value); }
        }


        private int _totalcount;

        public int TotalCount
        {
            get { return _totalcount; }
            set { SetProperty(ref _totalcount, value); }
        }

    }
}
