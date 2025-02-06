using FalcaPOS.Common.Events;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Windows.Controls;

namespace FalcaPOS.ExpiryProducts.ViewModel
{
    public class ExpiryProductsViewModel : BindableBase
    {
        public DelegateCommand<object> RefreshCommand { get; private set; }

        private readonly IEventAggregator _eventAggregator;

        private readonly Logger _logger;

        private readonly IExpiryProductsService _expiryProductService;

        public DelegateCommand<object> ExpiryExportCommand { get; private set; }
        public ExpiryProductsViewModel(IEventAggregator eventAggregator, Logger logger, IExpiryProductsService expiryProductService)
        {
            LoadMonth();

            _expiryProductService = expiryProductService ?? throw new ArgumentNullException(nameof(expiryProductService));

            RefreshCommand = new DelegateCommand<object>(Refresh);

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            ExpiryExportCommand = new DelegateCommand<object>(ExpiryExport);

        }


        public void LoadMonth()
        {
            string currentMonthName = DateTime.Now.Date.ToString("MMMM");
            string nextMonthName = DateTime.Now.Date.AddMonths(1).ToString("MMMM");
            string next3MonthName = DateTime.Now.Date.AddMonths(3).ToString("MMMM");
            string next6MonthName = DateTime.Now.Date.AddMonths(6).ToString("MMMM");

            CurrentMonth = "Current Month";
            NextMonth = "Next Month";
            Next3Month = "Three Months";
            Next6Month = "Six Months";
        }

        private string _currentMonth;

        public string CurrentMonth
        {
            get => _currentMonth;
            set => SetProperty(ref _currentMonth, value);
        }

        private string _nextMonth;

        public string NextMonth
        {
            get => _nextMonth;
            set => SetProperty(ref _nextMonth, value);
        }

        private string _next3Month;

        public string Next3Month
        {
            get => _next3Month;
            set => SetProperty(ref _next3Month, value);
        }

        private string _next6Month;

        public string Next6Month
        {
            get => _next6Month;
            set => SetProperty(ref _next6Month, value);
        }

        /// <summary>
        /// This method is called when the user clicks on the refresh button in the Expiry Products page.
        /// </summary>
        /// <param name="obj"></param>
        public async void Refresh(object obj)
        {
            try
            {
                var _tabControl = (TabControl)obj;
                if (_tabControl != null)
                {
                    var _selectedItem = (TabItem)_tabControl.SelectedValue;
                    if (_selectedItem.Header != null)
                    {
                        // Clears the redis cache present in the expiry products
                        var _result = await _expiryProductService.ClearExpiryProductsCacheOnRefresh();
                        if (_result.IsSuccess)
                        {
                            _logger.LogInformation(_result.Data);
                        }
                        else
                        {
                            _logger.LogInformation(_result.Error);
                        }
                        switch (_selectedItem.Header)
                        {
                            case "Expired":
                                _eventAggregator.GetEvent<ExpiredEvent>().Publish(true);
                                break;
                            case "Current Month":
                                _eventAggregator.GetEvent<CurrentExpiryEvent>().Publish(true);
                                break;
                            case "Next Month":
                                _eventAggregator.GetEvent<NextExpiryEvent>().Publish(true);
                                break;
                            case "Three Months":
                                _eventAggregator.GetEvent<ThreeExpiryEvent>().Publish(true);
                                break;
                            case "Six Months":
                                _eventAggregator.GetEvent<SixExpiryEvent>().Publish(true);
                                break;
                            default:
                                break;
                        }

                    }
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        public void ExpiryExport(object obj)
        {
            try
            {

                var _tabControl = (TabControl)obj;
                if (_tabControl != null)
                {
                    var _selectedItem = (TabItem)_tabControl.SelectedValue;
                    if (_selectedItem.Header != null)
                    {
                        switch (_selectedItem.Header)
                        {
                            case "Expired":
                                _eventAggregator.GetEvent<ExpiryExportEvent>().Publish(true);
                                break;
                            case "Current Month":
                                _eventAggregator.GetEvent<ExpiryCurrentEvent>().Publish(true);
                                break;
                            case "Next Month":
                                _eventAggregator.GetEvent<ExpiryNextEvent>().Publish(true);
                                break;
                            case "Three Months":
                                _eventAggregator.GetEvent<ExpiryThreeEvent>().Publish(true);
                                break;
                            case "Six Months":
                                _eventAggregator.GetEvent<ExpirySixEvent>().Publish(true);
                                break;
                            default:
                                break;


                        }
                    }
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }
    }


}
