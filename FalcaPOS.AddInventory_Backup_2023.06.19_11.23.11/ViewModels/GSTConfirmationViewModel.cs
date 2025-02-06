using FalcaPOS.Common;
using FalcaPOS.Contracts;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace FalcaPOS.AddInventory.ViewModels
{
    public class GSTConfirmationViewModel : BindableBase, IDialogAware
    {

        private bool _IsDialogOpen;
        public bool IsDialogOpen
        {
            get { return _IsDialogOpen; }
            set { SetProperty(ref _IsDialogOpen, value); }
        }


        private string _gstIn;
        public string GSTIN
        {
            get { return _gstIn; }
            set { SetProperty(ref _gstIn, value); }
        }

        public string Title => "Confirmation";


        private IEventAggregator _eventAggregator;

        public event Action<IDialogResult> RequestClose;
        public DelegateCommand NoCommand { get; private set; }
        public DelegateCommand YesCommand { get; private set; }

        private readonly INotificationService _notificationService;


        public GSTConfirmationViewModel(INotificationService notificationService, IEventAggregator eventAggregator)
        {
            YesCommand = new DelegateCommand(Save);

            NoCommand = new DelegateCommand(CloseDialog);

            _eventAggregator = eventAggregator;

            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));



        }

        public void Save()
        {
            if (string.IsNullOrEmpty(GSTIN))
            {
                _notificationService.ShowMessage("Please enter  GSTIN", NotificationType.Error);
            }
            if (!string.IsNullOrEmpty(GSTIN))
            {
                if (GSTIN.ToLower() == ApplicationSettings.GST_Number.ToLower())
                {
                    RequestClose?.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters("")));
                }
                else
                {
                    _notificationService.ShowMessage("Entered GSTIN is wrong", NotificationType.Error);
                }
            }


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
