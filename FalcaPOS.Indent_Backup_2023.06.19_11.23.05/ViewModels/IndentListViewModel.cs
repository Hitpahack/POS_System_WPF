using FalcaPOS.Common;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Indent;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FalcaPOS.Indent.ViewModels
{
    public class IndentListViewModel : BindableBase
    {
        private CancellationTokenSource _cancellationTokenSource { get; set; }
        public DelegateCommand RefreshDataGrid { get; private set; }
        public DelegateCommand SearchIndentListFlyoutCommand { get; private set; }
        public DelegateCommand<object> RowDoubleClickCommand { get; private set; }

        private readonly INotificationService _notificationService;

        private readonly IIndentService _indentService;
        public DelegateCommand<IndentViewModel> CreateNewTabCommand { get; private set; }

        private readonly IEventAggregator _eventAggregator;

        private readonly ProgressService _progressService;

        private readonly Logger _logger;

        public IndentListViewModel(IEventAggregator eventAggregator, Logger logger, IIndentService IndentService, INotificationService notificationService, ProgressService progressService)
        {
            _indentService = IndentService ?? throw new ArgumentNullException(nameof(IndentService));

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _progressService = progressService;

            RowDoubleClickCommand = new DelegateCommand<object>(GetDetailsIndent);

            _cancellationTokenSource = new CancellationTokenSource();

            RefreshDataGrid = new DelegateCommand(RemoveProcessPage);
            SearchIndentListFlyoutCommand = new DelegateCommand(SearchIndentListFlyoutOpen);

            CreateNewTabCommand = new DelegateCommand<IndentViewModel>(CreateNewTab);

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _ = eventAggregator.GetEvent<IntentStatuschangeRefreshEvent>().Subscribe(RefreshStatuschange);

            _ = eventAggregator.GetEvent<IndentListFlyoutSearchDataEvent>().Subscribe(SearchIndentFlyoutData);

            _ = eventAggregator.GetEvent<SignalRIndentAddedEvent>().Subscribe(RefrehsGrid, ThreadOption.PublisherThread)
                ;
            //LoadData(); load only loggined user data 

            double userheightscreen = System.Windows.SystemParameters.PrimaryScreenHeight;

            MaxHeight = userheightscreen - 150;

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }

        private async void SearchIndentFlyoutData(IndentListSearch obj)
        {
            try
            {

                await LoadData(obj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        private void SearchIndentListFlyoutOpen()
        {
            _eventAggregator.GetEvent<IndentListFlyoutOpenEvent>().Publish(true);
        }

        public void RemoveProcessPage()
        {
            try
            {
                _eventAggregator.GetEvent<IntentNewTabRemoveEvent>().Publish();

                IndentList = null;
            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in indent list view model", _ex);
            }
        }
        public async void RefreshStatuschange(int PoId)
        {
            try
            {

                await LoadData();
                var _intentviewmodel = IndentList?.Where(x => x.Id == PoId).FirstOrDefault();
                if (_intentviewmodel != null)
                    _eventAggregator.GetEvent<IntentNewTabCreateEvent>().Publish(_intentviewmodel);
            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in indent list view model", _ex);
            }
        }
        public void CreateNewTab(IndentViewModel indentViewModel)
        {
            _eventAggregator.GetEvent<IntentNewTabCreateEvent>().Publish(indentViewModel);
        }
        public async Task LoadData(IndentListSearch indentListSearch = null)
        {
            try
            {

                await _progressService.StartProgressAsync();
                await Task.Run(async () =>
                {

                    var _result = await _indentService.ViewIndentList(indentListSearch);

                    if (_result.IsSuccess)
                    {
                        IndentList = new ObservableCollection<IndentViewModel>(_result.Data.OrderByDescending(x => x.CreateDate));
                        IndentList.Select(x => { x.ProperStatus = ProperStatus.ProperStatusName(x.Status); return x; }).ToList();
                        TotalCount = "Total Count " + IndentList.Count;
                       
                    }
                    else
                    {
                        IndentList = null;
                        TotalCount = "Total Count " + 0;
                        _notificationService.ShowMessage(_result.Error, NotificationType.Error);
                    }


                });
               
            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in indent list view model", _ex);
            }
            finally
            {
                await _progressService.StopProgressAsync();
            }

        }

        public void GetDetailsIndent(object obj)
        {
            try
            {
                var Model = obj as IndentViewModel;

                if (Model == null)
                {
                    //return invalid details

                    return;
                }

                if (Model.IsSelectedShallow)
                {
                    //Grid row  is already opend collapse .
                    Model.IsSelectedShallow = false;
                    Model.IsSelected = false;
                    return;
                }

                //check if already information is fetched 

                if (Model.HasInformation)
                {
                    //display and return
                    Model.IsSelectedShallow = true;
                    Model.IsSelected = true;
                    return;
                }



                if (Model.Id != 0)
                {
                    Model.IsBusy = true;
                    Model.IsSelectedShallow = true;
                    Model.IsSelected = true;


                    //await Task.Run(async () =>
                    //{


                    //    var _result = await _indentService.ViewDetailIndent(Model.Id);

                    //    Application.Current?.Dispatcher?.Invoke(() =>
                    //    {
                    //        if (_result.IsSuccess)
                    //        {

                    //            if (Model != null)
                    //            {
                    //                Model.Products = _result.Data;
                    //                //Model.IsSelectedShallow = true;
                    //                //Model.IsSelected = true;
                    //                Model.IsBusy = false;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            Model.IsBusy = false;

                    //        }

                    //    });


                    //});
                }

            }
            catch (Exception _ex)
            {
                _logger?.LogError("Error in new product type added", _ex);
                //log _ex;
            }
        }

        public async void RefrehsGrid(IndentViewModel indentViewModel)
        {
            try
            {
                var _indentserach = new IndentListSearch()
                {
                    Status = indentViewModel.Status
                };
                await LoadData(_indentserach);
            }
            catch (Exception _ex)
            {

                _logger?.LogError("Error in indent list view model", _ex);
            }
        }

        private ObservableCollection<IndentViewModel> _indentList;

        public ObservableCollection<IndentViewModel> IndentList
        {
            get { return _indentList; }
            set { SetProperty(ref _indentList, value); }
        }


        private ObservableCollection<IndentViewProduct> _indentdetails;

        public ObservableCollection<IndentViewProduct> IndentDetails
        {
            get { return _indentdetails; }
            set { SetProperty(ref _indentdetails, value); }
        }

        private string _totalcount;

        public string TotalCount
        {
            get { return _totalcount; }
            set { SetProperty(ref _totalcount, value); }
        }

        private double _maxheight;

        public double MaxHeight
        {
            get { return _maxheight; }
            set
            {
                _maxheight = value;
                RaisePropertyChanged("MaxHeight");
            }
        }

    }
}
