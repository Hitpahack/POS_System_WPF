using FalcaPOS.Common;
using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Contracts;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace FalcaPOS.AddInventory.ViewModels
{
    public class AddCategoryViewModel : BindableBase
    {
        public DelegateCommand PopUpOpenCommand { get; private set; }


        private readonly IProductTypeService _productTypeService;

        private readonly IEventAggregator _eventAggregator;

        private readonly INotificationService _notificationService;

        private readonly Logger _logger;

        public DelegateCommand AddCategoryCommand { get; private set; }


        public AddCategoryViewModel(IProductTypeService productTypeService,
            IEventAggregator eventAggregator,
            INotificationService notificationService,
              Logger logger)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

            _productTypeService = productTypeService ?? throw new ArgumentNullException(nameof(productTypeService));

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            AddCategoryCommand = new DelegateCommand(AddCategory);

        }

        private async void AddCategory()
        {

            if (!CategoryName.IsValidString())
            {
                _notificationService.ShowMessage("Category name is required.", NotificationType.Error);
                return;
            }

            if (!Certificate.IsValidString()) {
                _notificationService.ShowMessage("Please select certificate category is yes or no.", NotificationType.Error);
                return;
            }
            if (Certificate == "Yes") {
                _notificationService.ShowMessage("Please contact administrator", NotificationType.Error);
                return;
            }

            bool IsCertificate = Certificate == "Yes" ? true : false ;

            try
            {
                await Task.Run(async () =>
                {
                    var _result = await _productTypeService.AddCategory(CategoryName,IsCertificate);

                    if (_result != null &&_result.IsSuccess)
                    {
                        Application.Current?.Dispatcher?.Invoke(() =>
                        {

                            _eventAggregator.GetEvent<AddCategoryEvent>().Publish(true);

                            _notificationService.ShowMessage($"Category {CategoryName} added.", NotificationType.Success);

                            CategoryName = null;// string.Empty;
                            Certificate = null;

                        });


                    }
                    else {
                        _notificationService.ShowMessage(_result.Error, NotificationType.Error);

                    }
                });
            }
            catch (Exception _ex)
            {
                _logger.LogError("Error in  add Category", _ex);
            }
        }

        private bool _popupClose;

        public bool PopupClose
        {
            get => _popupClose;
            set => SetProperty(ref _popupClose, value);
        }


        private string _categoryName;

        public string CategoryName
        {
            get => _categoryName;
            set => SetProperty(ref _categoryName, value);
        }

        private string _certificate;
        public string Certificate {
            get => _certificate;
            set => SetProperty(ref _certificate, value);
        }
    }
}
