using FalcaPOS.Common;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Sku;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace FalcaPOS.Sku.ViewModels
{
    public class SkuSheetViewModel : BindableBase
    {
        private readonly ISkuService _skuService;

        private readonly INotificationService _notificationService;
        private readonly ProgressService _progressService;

        public DelegateCommand ExportCommand { get; private set; }
        public DelegateCommand RefreshCommand { get; private set; }

        private readonly ICommonService _commonService;

        public DelegateCommand OpenAddSkuflyoutCommand { get; set; }

        private readonly IEventAggregator _eventAggregator;

        public SkuSheetViewModel(ICommonService commonService, IEventAggregator EventAggregator, ISkuService skuService, INotificationService notificationService, ProgressService progressService)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _skuService = skuService ?? throw new ArgumentNullException(nameof(skuService));

            ExportCommand = new DelegateCommand(() => Export());

            RefreshCommand = new DelegateCommand(() => LoadData());

            OpenAddSkuflyoutCommand = new DelegateCommand(() => AddSkuFlyOutOpen());

            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));

            _progressService = progressService;

            _eventAggregator = EventAggregator ?? throw new ArgumentNullException(nameof(EventAggregator));

            LoadData();
        }



        public void AddSkuFlyOutOpen()
        {
            _eventAggregator.GetEvent<AddSKUFlyoutOpenEvent>().Publish(true);
        }

        public void Export()
        {
            try
            {

                if (NewProductsV2List == null || !NewProductsV2List.Any())
                {
                    _notificationService.ShowMessage("No records found to export", Common.NotificationType.Error);

                    return;
                }
                IsExportEnabled = false;
                _fileName = DateTime.Now.Date.ToString("dd-MM-yyyy") + "_SkuSheet";

                bool _result = _commonService.ExportToXL(NewProductsV2List.ToList(), _fileName, skipfileName: true);

                if (_result)
                {
                    _notificationService.ShowMessage($"File exported to folder {ApplicationSettings.REPORTS_PATH}", Common.NotificationType.Success);
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
        public async void LoadData()
        {
            try
            {


                var _result = await _skuService.GetAllSkuV2();
                if ((bool)_result?.IsSuccess)
                {
                    Application.Current?.Dispatcher?.InvokeAsync(() =>
                    {
                        //await _progressService.StartProgressAsync();
                        NewProductsV2List = _result.Data;
                        Total = NewProductsV2List.Count();
                        IsExportEnabled = true;
                        //await _progressService.StopProgressAsync();
                    });
                }
            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, Common.NotificationType.Error);
            }
            finally
            {

                //await _progressService.StopProgressAsync();
            }
        }


        private ObservableCollection<SkuSheetModel> _type;

        public ObservableCollection<SkuSheetModel> Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }

        private IEnumerable<NewProductV2> _newProductsV2;

        public IEnumerable<NewProductV2> NewProductsV2List
        {
            get { return _newProductsV2; }
            set { SetProperty(ref _newProductsV2, value); }
        }


        private int _slectedIndex;

        public int SelectedIndex
        {
            get { return _slectedIndex; }
            set { SetProperty(ref _slectedIndex, value); }
        }

        public string _fileName { get; set; }

        private bool _isExportEnabled;
        public bool IsExportEnabled
        {
            get => _isExportEnabled;
            set => _ = SetProperty(ref _isExportEnabled, value);
        }

        private int _total;

        public int Total
        {
            get { return _total; }
            set { SetProperty(ref _total, value); }
        }

    }

    public class ExportSkuProduct
    {
        public string DepartmentName { get; set; }
        public string DepartmentCode { get; set; }
        public string BrandName { get; set; }
        public string ProductName { get; set; }
        public string Sku { get; set; }

    }

    public class SkuSheetModel
    {
        public int Id { get; set; }
        public string ProductType { get; set; }
        public string DeptCode { get; set; }
        public List<SkuSheetProductViewModel> Product1 { get; set; }

        public List<SkuSheetProductViewModel> Product2 { get; set; }
        public List<SkuSheetProductViewModel> Product3 { get; set; }
        public List<SkuSheetProductViewModel> Product4 { get; set; }

        public List<SkuSheetProductViewModel> Products { get; set; }


    }



}
