using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Linq;

namespace FalcaPOS.Stock.ViewModels
{
    public class StockTransferHomeViewModel : BindableBase
    {
        private readonly IEventAggregator eventAggregator;

        private bool _isOnlyCreateAndSearchVisibility;
        public bool IsOnlyCreateAndSearchVisibility
        {
            get { return _isOnlyCreateAndSearchVisibility; }
            set { SetProperty(ref _isOnlyCreateAndSearchVisibility, value); }
        }

        public StockTransferHomeViewModel(EventAggregator EventAggregator)
        {

        eventAggregator = EventAggregator ?? throw new ArgumentNullException(nameof(EventAggregator));
            IsOnlyCreateAndSearchVisibility = (AppConstants.USER_ROLES.Contains(AppConstants.ROLE_TERRITORY_MANAGER)) ? false: true ;


        }
    }
}
