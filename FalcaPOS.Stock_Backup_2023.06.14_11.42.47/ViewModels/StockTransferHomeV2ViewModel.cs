using FalcaPOS.Common.Constants;
using Prism.Mvvm;
using System.Linq;

namespace FalcaPOS.Stock.ViewModels
{
    public class StockTransferHomeV2ViewModel : BindableBase
    {

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


        public StockTransferHomeV2ViewModel()
        {
            if (AppConstants.USER_ROLES[0] != null)
            {
                IsTransferVisibility = (AppConstants.USER_ROLES.Contains(AppConstants.ROLE_BACKEND));
                IsTransferSearchVisibility = AppConstants.USER_ROLES.Contains(AppConstants.ROLE_FINANCE);
            }
        }



    }
}
