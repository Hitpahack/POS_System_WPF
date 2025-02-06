using Prism.Commands;
using Prism.Mvvm;
using System;

namespace FalcaPOS.PurchaseReturns.ViewModel
{
    public class PurchaseReturnHomeViewModel : BindableBase
    {

        public DelegateCommand<Object> ViewRequestCommand { get; private set; }

        public DelegateCommand<Object> BackCommand { get; private set; }
        public PurchaseReturnHomeViewModel()
        {

            IsCreate = true;
            IsView = false;

            ViewRequestCommand = new DelegateCommand<object>(ViewRequest);
            BackCommand = new DelegateCommand<object>(BackRequest);
        }

        public void ViewRequest(object obj)
        {
            IsCreate = false;
            IsView = true;
        }

        public void BackRequest(object obj)
        {
            IsCreate = true;
            IsView = false;
        }

        private bool _isCreate;

        public bool IsCreate
        {
            get => _isCreate;
            set => SetProperty(ref _isCreate, value);
        }

        private bool _isView;
        public bool IsView
        {
            get => _isView;
            set => SetProperty(ref _isView, value);
        }

    }


}
