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
using Telerik.Windows.Controls;

namespace FalcaPOS.Sku.ViewModels
{
    public class SkuSheetViewModel : BindableBase
    {
        private readonly ISkuService _skuService;
        private readonly INotificationService _notificationService;
        private readonly ProgressService _progressService;

        public Prism.Commands.DelegateCommand ExportCommand { get; private set; }
        public DelegateCommand<object> RefreshCommand { get; private set; }

        private readonly ICommonService _commonService;

        public Prism.Commands.DelegateCommand OpenAddSkuflyoutCommand { get; set; }

        private readonly IEventAggregator _eventAggregator;

        public SkuSheetViewModel(ICommonService commonService, IEventAggregator EventAggregator, ISkuService skuService, INotificationService notificationService, ProgressService progressService)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _skuService = skuService ?? throw new ArgumentNullException(nameof(skuService));

            ExportCommand = new Prism.Commands.DelegateCommand(() => Export());

            RefreshCommand = new DelegateCommand<object>((obj) => LoadData(obj));

            OpenAddSkuflyoutCommand = new Prism.Commands.DelegateCommand(() => AddSkuFlyOutOpen());

            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));

            _progressService = progressService;

            _eventAggregator = EventAggregator ?? throw new ArgumentNullException(nameof(EventAggregator));
            _eventAggregator.GetEvent<MasterSKURefreshEvent>().Subscribe(() =>
            {
                LoadData(null);
            });

            LoadData(null);
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
                List<ExportNewProductV2> exportNews = new List<ExportNewProductV2>();

                foreach (var item in NewProductsV2List)
                {
                    exportNews.Add(new ExportNewProductV2()
                    {
                        ProductName = item.ProductName,
                        Brand = item.Brand,
                        Category = item.Category,
                        SubCategory = item.SubCategory,
                        UOM = item.UOM,
                        TechnicalName = item.TechnicalName,
                        SKU = item.SKU,
                        TradeOrOwn = item.TradeOrOwn,
                        GST = item.GST,
                        HSN = item.HSN,
                        PackingSize = item.PackingSize,
                    });
                }

                if (exportNews == null || !exportNews.Any())
                {
                    _notificationService.ShowMessage("No records found to export", Common.NotificationType.Error);

                    return;
                }

                IsExportEnabled = false;
                _fileName = DateTime.Now.Date.ToString("dd-MM-yyyy") + "_SkuSheet";

                bool _result = _commonService.ExportToXL(exportNews.ToList(), _fileName, skipfileName:false);

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
        public async void LoadData(object obj)
        {
            try
            {
                if (obj != null)
                {
                    ResetTelerikGridFilters.ClearTelerikGridViewFilters(obj);
                }

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
    public class ExportNewProductV2
    {
       
        public String SKU { get; set; }
        public String Brand { get; set; }
        public String ProductName { get; set; }
        // public String ShortName { get; set; }
        public String TechnicalName { get; set; }
        public String Category { get; set; }
        public String SubCategory { get; set; }
        public String PackingSize { get; set; }
        public String UOM { get; set; }
        public String TradeOrOwn { get; set; }
        public double GST { get; set; } 
        public string HSN { get; set; }
      
    }

}
