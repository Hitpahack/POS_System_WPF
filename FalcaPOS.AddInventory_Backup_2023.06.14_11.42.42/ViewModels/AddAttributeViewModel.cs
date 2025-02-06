using FalcaPOS.Common.Events;
using FalcaPOS.Common.Logger;
using FalcaPOS.Common.Validations;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Attributes;
using Prism.Commands;
using Prism.Events;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FalcaPOS.AddInventory.ViewModels
{
    public class AddAttributeViewModel : ValidationBase
    {
        private readonly IAttributeTypeService _attributeTypeService;

        private readonly IEventAggregator _eventAggregator;

        private readonly Logger _logger;

        public DelegateCommand AddAttributeCommand { get; private set; }




        public AddAttributeViewModel(IAttributeTypeService attributeTypeService, IEventAggregator eventAggregator, Logger logger)
        {
            AddAttributeCommand = new DelegateCommand(async () => await CreateAttribute())
                                .ObservesCanExecute(() => IsValid);

            _attributeTypeService = attributeTypeService ?? throw new ArgumentNullException(nameof(attributeTypeService));

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }

        private string _attributeName;

        [Required(ErrorMessage = "Name is required")]
        public string AttributeName
        {
            get { return _attributeName; }
            set
            {
                SetProperty(ref _attributeName, value);
                if (value != null)
                    ValidateProperty(value);
            }
        }

        private bool _popupClose;

        public bool PopupClose
        {
            get { return _popupClose; }
            set { SetProperty(ref _popupClose, value); }
        }

        private async Task CreateAttribute()
        {
            try
            {

                if (string.IsNullOrEmpty(AttributeName)) return;

                var _attributeType = new AttributeType
                {
                    Name = AttributeName,
                    Description = AttributeName,
                    Isenabled = true
                };

                _logger.LogObject($"Creating new attribute", _attributeType);
                var _result = await _attributeTypeService.CreateAttribute(_attributeType);
                if (_result != null)
                {
                    _logger.LogInformation($"new attribute created {_result.Name}");

                    _eventAggregator.GetEvent<AddAttributeEvent>().Publish(_result);
                    _eventAggregator.GetEvent<NotifyMessage>().Publish(new Common.Models.ToastMessage
                    {
                        Message = $"Attribute {_result.Name} created.",
                        MessageType = Common.NotificationType.Success
                    });
                    ClearErrors();
                    AttributeName = null;
                    PopupClose = false;
                }

            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in creating new attribute", _ex);
            }
        }
    }
}
