using FalcaPOS.Common.Constants;
using Prism.Mvvm;
using System;
using System.Linq;

namespace FalcaPOS.Sku.ViewModels
{
    public class HomeSKUViewModel : BindableBase
    {
        public HomeSKUViewModel()
        {

            if (AppConstants.USER_ROLES != null)
            {
                TabVisibility = (AppConstants.USER_ROLES.Contains(AppConstants.ROLE_PURCHASE_MANAGER) ||  AppConstants.USER_ROLES.Contains(AppConstants.ROLE_ADMIN))
                    ? true : false;
                TabAApproveVisibility=(AppConstants.USER_ROLES.Contains(AppConstants.ROLE_FINANCE))?true : false;
            }
        }

        private bool _tabVisibility;

        public bool TabVisibility
        {
            get { return _tabVisibility; }
            set { SetProperty(ref _tabVisibility, value); }
        }

        private bool _tabApproveVisibility;

        public bool TabAApproveVisibility
        {
            get { return _tabApproveVisibility; }
            set { SetProperty(ref _tabApproveVisibility, value); }
        }

       

    }
}
