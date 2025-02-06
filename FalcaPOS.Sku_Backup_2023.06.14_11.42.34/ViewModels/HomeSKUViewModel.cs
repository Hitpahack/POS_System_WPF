using FalcaPOS.Common.Constants;
using Prism.Mvvm;
using System.Linq;

namespace FalcaPOS.Sku.ViewModels
{
    public class HomeSKUViewModel : BindableBase
    {
        public HomeSKUViewModel()
        {

            if (AppConstants.USER_ROLES != null)
            {
                TabVisibility = AppConstants.USER_ROLES.Contains("storeperson")
                    ? true : false;
            }
        }

        private bool _tabVisibility;

        public bool TabVisibility
        {
            get { return _tabVisibility; }
            set { SetProperty(ref _tabVisibility, value); }
        }

    }
}
