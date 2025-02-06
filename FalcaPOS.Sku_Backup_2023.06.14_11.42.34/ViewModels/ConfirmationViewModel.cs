using FalcaPOS.Contracts;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace FalcaPOS.Sku.ViewModels
{
    public class ConfirmationViewModel : BindableBase, IDialogAware
    {
        private bool _IsDialogOpen;
        public bool IsDialogOpen
        {
            get { return _IsDialogOpen; }
            set { SetProperty(ref _IsDialogOpen, value); }
        }

        public string Title => "Confirmation";

        private readonly ISkuService _skuService;

        private IEventAggregator _eventAggregator;

        public event Action<IDialogResult> RequestClose;
        public DelegateCommand NoCommand { get; private set; }
        public DelegateCommand YesCommand { get; private set; }
        public ConfirmationViewModel(IEventAggregator eventAggregator)
        {
            YesCommand = new DelegateCommand(Save);

            NoCommand = new DelegateCommand(CloseDialog);
            _eventAggregator = eventAggregator;


        }

        public void Save()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters("")));
        }
        private void CloseDialog()
        {
            RequestClose?.Invoke(new DialogResult());
        }
        private void OnShowDialog()
        {
            IsDialogOpen = true;
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            IsDialogOpen = true;
        }
    }
}
