using FalcaPOS.Common;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Logger;
using FalcaPOS.Common.Validations;
using FalcaPOS.Contracts;
using Prism.Commands;
using Prism.Events;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FalcaPOS.Suppliers.ViewModels
{
    public class AddNewBankViewModel : ValidationBase
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly ISupplierService _supplierService;

        private readonly ICommonService _commonService;

        public DelegateCommand AddNewCommnad { get; private set; }

        public DelegateCommand ResetBankCommnad { get; private set; }


        private readonly Logger _logger;

        private readonly INotificationService _notificationService;


        public AddNewBankViewModel(INotificationService NotificationService, IEventAggregator EventAggregator, ISupplierService SupplierService, ICommonService CommonService, Logger Logger)
        {

            _notificationService = NotificationService ?? throw new ArgumentNullException(nameof(NotificationService));

            _eventAggregator = EventAggregator ?? throw new ArgumentNullException(nameof(EventAggregator));

            _supplierService = SupplierService ?? throw new ArgumentNullException(nameof(SupplierService));

            _commonService = CommonService ?? throw new ArgumentNullException(nameof(CommonService));

            _logger = Logger ?? throw new ArgumentNullException(nameof(Logger));

            AddNewCommnad = new DelegateCommand(AddNewBank)
                                  .ObservesCanExecute(() => IsValid);



            ResetBankCommnad = new DelegateCommand(Reset);


        }

        public async void AddNewBank()
        {
            try
            {
                if (Bank != null)
                {


                    await Task.Run(async () =>
                    {

                        var _result = await _supplierService.AddNewBank(Bank);

                        if (_result != null && _result.IsSuccess)
                        {
                            _notificationService.ShowMessage("Bank added ", NotificationType.Success);
                            PopupClose = false;
                            Reset();
                            _eventAggregator.GetEvent<AddNewBankEvent>().Publish();
                        }
                        else
                        {
                            _notificationService.ShowMessage(_result.Error, NotificationType.Error);
                        }
                    });


                }

            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }


        }

        public void Reset()
        {
            Bank = null;
        }

        private bool _popupClose;

        public bool PopupClose
        {
            get { return _popupClose; }
            set { SetProperty(ref _popupClose, value); }
        }


        private string _bank;

        [Required(ErrorMessage = "Bank Name is required")]
        public string Bank { get { return _bank; } set { SetProperty(ref _bank, value); if (value != null) ValidateProperty(value); } }

    }
}
