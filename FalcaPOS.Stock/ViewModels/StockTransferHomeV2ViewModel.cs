using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Logger;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Linq;

namespace FalcaPOS.Stock.ViewModels
{
    public class StockTransferHomeV2ViewModel : BindableBase
    {

        private readonly IEventAggregator eventAggregator;

        private bool isTransferVisibility;

        public bool IsTransferVisibility
        {
            get { return isTransferVisibility; }
            set { SetProperty(ref isTransferVisibility, value); }
        }


        private bool isTransferSearchVisibility;

        public bool IsTransferSearchVisibility
        {
            get { return isTransferSearchVisibility; }
            set { SetProperty(ref isTransferSearchVisibility, value); }
        }


        public StockTransferHomeV2ViewModel(EventAggregator EventAggregator)
        {
            eventAggregator = EventAggregator ?? throw new ArgumentNullException(nameof(EventAggregator));

            if (AppConstants.USER_ROLES[0] != null)
            {
                IsTransferVisibility = (AppConstants.USER_ROLES.Contains(AppConstants.ROLE_BACKEND));
                IsTransferSearchVisibility = (AppConstants.USER_ROLES.Contains(AppConstants.ROLE_FINANCE)||AppConstants.USER_ROLES.Contains(AppConstants.ROLE_CONTROL_MANAGER));

                
            }
            //eventAggregator.GetEvent<SignalRReloadStockTransferListForSahayaEvent>().Subscribe(Resetdata);
            //eventAggregator.GetEvent<DummyEvent>().Subscribe(Resetdata);


            //            eventAggregator.GetEvent<SignalRReloadStockTransferListForSahayaEvent>().Subscribe(
            //ResetData, ThreadOption.BackgroundThread);
        }

        //private void Resetdata(object obj)
        //{

        //    eventAggregator.GetEvent<DummyEvent>().Publish(obj);

        //}
    }
}
