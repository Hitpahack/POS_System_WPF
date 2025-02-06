using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using Prism.Events;
using Prism.Mvvm;
using System;

namespace FalcaPOS.Invoice.ViewModels
{
    public class InvoiceHomeTabViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private bool _isViewVisible;
        public bool IsViewVisible
        {
            get { return _isViewVisible; }
            set { SetProperty(ref _isViewVisible, value); }
        }
        public InvoiceHomeTabViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _ = eventAggregator.GetEvent<LoginedRole>().Subscribe(Visibility);
        }

        private void Visibility(string role)
        {
            switch (role)
            {
                case AppConstants.ROLE_PURCHASE_MANAGER:
                case AppConstants.ROLE_FINANCE:
                case AppConstants.ROLE_CONTROL_MANAGER:
                    IsViewVisible = true;
                    break;

                default:
                    break;
            }

        }


    }
}
