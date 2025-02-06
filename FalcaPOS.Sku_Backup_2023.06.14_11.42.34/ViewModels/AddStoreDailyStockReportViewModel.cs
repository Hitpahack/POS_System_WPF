using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Sku;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.Sku.ViewModels
{
    public class AddStoreDailyStockReportViewModel : BindableBase
    {
        private readonly ISkuService _skuService;

        public DelegateCommand SelectedCommand { get; private set; }

        public DelegateCommand RefreshCommand { get; private set; }
        public DelegateCommand<object> SaveCommand { get; private set; }
        public DelegateCommand RestCommand { get; private set; }

        private IEventAggregator _eventAggregator;

        private readonly INotificationService _notificationService;

        private readonly Logger _logger;

        private readonly ProgressService _ProgressService;

        private readonly IDialogService _dialogService;
        public AddStoreDailyStockReportViewModel(Logger logger,IDialogService dialogService, IEventAggregator eventAggregator, ISkuService skuService, INotificationService notificationService, ProgressService ProgressService)
        {
            _skuService = skuService ?? throw new ArgumentNullException(nameof(skuService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _eventAggregator = eventAggregator;
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            _ProgressService = ProgressService;
            SelectedCommand = new DelegateCommand(() => LoadData());
            SaveCommand = new DelegateCommand<object>((obj) => Save(obj));
            RestCommand = new DelegateCommand(() => Reset());
            RefreshCommand = new DelegateCommand(() => LoadData());

            LoadData();

            CheckSaturday();

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }


        public void Save(object obj)
        {
            try
            {
                _dialogService.ShowDialog("ConfrimationPopUp",
                                 parameters: new DialogParameters
                                 (""),
                                 callback: (dialogResult) =>
                                 {
                                     if (dialogResult != null && dialogResult.Result == ButtonResult.OK)
                                     {
                                         submit(obj);
                                     }

                                 });
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }
        //rework needs
        public async void submit(object obj)
        {
            try
            {
                if (Type != null && Type.Count > 0)
                {
                    List<ProductTypesViewModel> productTypesViewModels = new List<ProductTypesViewModel>();

                    foreach (var item in Type)
                    {
                        ProductTypesViewModel productTypesViewModel = new ProductTypesViewModel();
                        List<ViewModel> viewModel = new List<ViewModel>();
                        foreach (var ite in item.FirstColumnProducts)
                        {
                            viewModel.Add(ite);
                        }
                        foreach (var ite in item.SecondColumnProducts)
                        {
                            viewModel.Add(ite);
                        }
                       

                        productTypesViewModel.ProductType = item.ProductType;
                        productTypesViewModel.Id = item.Id;
                        productTypesViewModel.DeptCode = item.DeptCode;
                        viewModel.Select(x => { x.Count = x.Count ?? 0; return x; }).ToList();
                        productTypesViewModel.Products = viewModel;
                        productTypesViewModels.Add(productTypesViewModel);
                    }
                    if (productTypesViewModels != null && productTypesViewModels.Count > 0)
                    {

                        await _ProgressService.StartProgressAsync();
                        await Task.Run(async () =>
                        {

                            var _result = await _skuService.VerifyProductCount(productTypesViewModels);

                            if (_result != null && _result.IsSuccess)
                            {
                                Application.Current?.Dispatcher?.Invoke(async () =>
                                {
                                    await _ProgressService.StopProgressAsync();
                                    _notificationService.ShowMessage(_result.Data.ToString(), Common.NotificationType.Success);
                                    Reset();

                                });
                            }
                            else
                            {
                                await _ProgressService.StopProgressAsync();
                                _notificationService.ShowMessage(_result.Error, Common.NotificationType.Error);

                                foreach (var item in Type)
                                {
                                    //rework need
                                    item.FirstColumnProducts.Select(x => { x.Count = x.Count == 0 ? null : x.Count; return x; }).ToList();
                                    item.SecondColumnProducts.Select(x => { x.Count = x.Count == 0 ? null : x.Count; return x; }).ToList();
                                    
                                }

                            }


                        });


                    }

                }


            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, Common.NotificationType.Error);
            }
            finally {
                await _ProgressService.StopProgressAsync();
            }
        }

        public async void Reset()
        {
            try {
                if (Type != null && Type.Count > 0) {
                    foreach (var item in Type) {
                        foreach (var ite in item.FirstColumnProducts) {
                            ite.Count = null;
                        }
                        foreach (var ite in item.SecondColumnProducts) {
                            ite.Count = null;
                        }

                    }
                }
            }
            catch(Exception _ex) {
                _logger.LogError(_ex.Message);
            }
           
        }
        public async void LoadData()
        {
            try
            {

                CheckSaturday();

                await Task.Run(async () =>
                {
                    var dateOfweek = Convert.ToDateTime(AppConstants.CurrentDate).DayOfWeek;

                    bool isEnable = dateOfweek.ToString() == "Saturday" ? false : true;

                    var _result = await _skuService.GetAllProductType();

                    if (_result != null && _result.IsSuccess && _result.Data.Any())
                    {
                        Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            List<ProductTypesViewModel> Result = new List<ProductTypesViewModel>();
                            foreach (var item in _result.Data)
                            {
                                ProductTypesViewModel productTypesView = new ProductTypesViewModel();
                                int _totalCount = item.Products.Count;
                                int _firstHalfCount = _totalCount / 2;
                                productTypesView.ProductType = item.ProductType.ToUpper();
                                productTypesView.Id = item.Id;
                                productTypesView.ProductTypeWithDeptcode = item.DeptCode != null ? item.ProductType.ToUpper() + " (" + item.DeptCode + ")" : item.ProductType.ToUpper();
                                var _firstColumnProducts = item.Products.Take(_firstHalfCount).ToList();
                                var _secondColumnProducts = item.Products.Skip(_firstHalfCount).ToList();
                                
                                productTypesView.FirstColumnProducts = _firstColumnProducts.OrderBy(x => x.Sku).ToList().Select(x => { x.IsEnable = isEnable; return x; }).ToList();
                                productTypesView.SecondColumnProducts = _secondColumnProducts.OrderBy(x => x.Sku).ToList().Select(x => { x.IsEnable = isEnable; return x; }).ToList();
                               
                                productTypesView.DeptCode = item.DeptCode;
                                Result.Add(productTypesView);
                            }

                            Type = new ObservableCollection<ProductTypesViewModel>(Result);
                            SelectedIndex = 0;

                        });
                    }


                });
            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, Common.NotificationType.Error);
            }


        }

        private ObservableCollection<ProductTypesViewModel> _type;

        public ObservableCollection<ProductTypesViewModel> Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }


        private int _slectedIndex;

        public int SelectedIndex
        {
            get { return _slectedIndex; }
            set { SetProperty(ref _slectedIndex, value); }
        }


        private bool _IsEnableBtn;

        public bool IsEnableBtn
        {
            get => _IsEnableBtn;
            set => SetProperty(ref _IsEnableBtn, value);
        }

        public void CheckSaturday()
        {
            try
            {
                var dateOfweek = Convert.ToDateTime(AppConstants.CurrentDate).DayOfWeek;

                IsEnableBtn = dateOfweek.ToString() == "Saturday" ? true : false;

            }
            catch (Exception _ex)
            {
                _notificationService.ShowMessage(_ex.Message, Common.NotificationType.Error);
            }
        }

    }
}
