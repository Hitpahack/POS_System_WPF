using FalcaPOS.Common;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Location;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FalcaPOS.Sales.ViewModels
{
    public class EditCustomerPopupViewModel:BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly INotificationService _notificationService;
        private readonly ISalesService _salesService;
        private readonly Logger _logger;

        public DelegateCommand<Object> SaveCustomerPopUpCommand { get; private set; }
        public DelegateCommand<Object> GetPinCodeDetailsCommand { get; private set; }

        public EditCustomerPopupViewModel(IEventAggregator eventAggregator, INotificationService notificationService, ISalesService salesService, Logger logger)
        {
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _salesService = salesService ?? throw new ArgumentNullException(nameof(salesService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            SaveCustomerPopUpCommand = new DelegateCommand<Object>(SendEditDetails);
            GetPinCodeDetailsCommand = new DelegateCommand<object>(GetPinCodeDetails);
        }

        private async void GetPinCodeDetails(Object pincode)
        {
            try
            {
                if (pincode != null)
                {
                    var _pincode = Convert.ToInt32(pincode);
                    if (_pincode.ToString().Length == 6)
                    {

                        await Task.Run(async () =>
                        {
                            var _result = await _salesService.GetPincodeDetailsNew((int)pincode);
                            if (_result != null && _result.IsSuccess && _result.Data != null)
                            {
                                States = new ObservableCollection<State>(new List<State>() { _result.Data.State });
                                SelectState = States.FirstOrDefault();

                                Districts = new ObservableCollection<District>(new List<District>() { _result.Data.District });
                                SelectDistrict = Districts.FirstOrDefault();

                                Villages = new ObservableCollection<Village>(_result.Data.Villages);
                            }

                        });
                    }
                    else
                    {
                        _notificationService.ShowMessage("Invalid pin code number", NotificationType.Error);
                        return;
                    }
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError("Getting error in Pincode details", _ex);

            }
        }


        private void SendEditDetails(Object obj)
        {
            if (string.IsNullOrEmpty(CustomerName))
            {
                _notificationService.ShowMessage("Please enter customer name",NotificationType.Error);
                return;
            }
            if (string.IsNullOrEmpty(PinCode.ToString()))
            {
                _notificationService.ShowMessage("Please enter PinCode", NotificationType.Error);
                return;
            }
            if (PinCode.ToString().Length != 6) {
                _notificationService.ShowMessage("Please enter valid PinCode", NotificationType.Error);

                return;

            }

            if (SelectState == null)
            {
                _notificationService.ShowMessage("Please select a state", NotificationType.Error);
                return;
            }
            if (SelectDistrict == null)
            {
                _notificationService.ShowMessage("Please select a district", NotificationType.Error);
                return;
            }
            if (SelectedVillages == null)
            {
                _notificationService.ShowMessage("Please select a village", NotificationType.Error);
                return;
            }
            if (SelectCustomerType.ToLower() == "fpo")
            {
                if (string.IsNullOrEmpty(GSTIN))
                {
                    _notificationService.ShowMessage("Please enter the GSTIN", NotificationType.Error);
                    return;
                }
                else
                {
                    if (GSTIN.IsValidString() && !GSTIN.IsValidGST())
                    {
                        _notificationService.ShowMessage("GST number is invalid", NotificationType.Error);
                        return;
                    }

                }
            }

            EditValidateCustomer(obj);
            _eventAggregator.GetEvent<EditCustomerPopupEvent>().Publish(this);

        }

        private void EditValidateCustomer(object obj)
        {
            var TargetClose = ((Button)(obj));
            var dynamicCommand = TargetClose.Command;
            dynamicCommand.CanExecute(true);
            dynamicCommand.Execute(this);
        }

        private int _customerId;

		public int CustomerID
		{
			get { return _customerId; }
			set { SetProperty (ref _customerId , value); }
		}

        private string _phone;

        public string Phone
        {
            get { return _phone; }
            set { SetProperty(ref _phone, value); }
        }

        private string _customerName;

		public string CustomerName
        {
			get { return _customerName; }
			set { SetProperty(ref _customerName , value); }
		}

        private List<string> _customerTypes;

        public List<string> CustomerTypes
        {
            get => _customerTypes;
            set => SetProperty(ref _customerTypes, value);
        }

        private string _customerType;

        public string SelectCustomerType
        {
            get => _customerType;
            set => SetProperty(ref _customerType, value);
        }


        private District _selectDistrict;
        public District SelectDistrict
        {
            get { return _selectDistrict; }
            set { SetProperty(ref _selectDistrict, value); }
        }

        private ObservableCollection<District> _districts;
        public ObservableCollection<District> Districts
        {
            get { return _districts; }
            set { SetProperty(ref _districts, value); }
        }


        private State _selectState;
        public State SelectState
        {
            get { return _selectState; }
            set { SetProperty(ref _selectState, value); }
        }

        private ObservableCollection<State> _states;
        public ObservableCollection<State> States
        {
            get { return _states; }
            set { SetProperty(ref _states, value); }
        }

        private int? _pinCode;
        public int? PinCode
        {
            get => _pinCode;
            set => SetProperty(ref _pinCode, value);
        }


        private string _gstin;

        public string GSTIN
        {
            get { return _gstin; }
            set { SetProperty(ref _gstin, value); }
        }

        private ObservableCollection<Village> _villages;

        public ObservableCollection<Village> Villages
        {
            get => _villages;
            set => SetProperty(ref _villages, value);
        }

        private Village _selectedVillages;

        public Village SelectedVillages
        {
            get => _selectedVillages;
            set => SetProperty(ref _selectedVillages, value);
        }


    }
}
