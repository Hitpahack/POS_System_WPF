using FalcaPOS.Common;
using FalcaPOS.Common.Constants;
using FalcaPOS.Common.Events;
using FalcaPOS.Contracts;
using FalcaPOS.Entites.Customers;
using MahApps.Metro.Controls;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;

namespace FalcaPOS.Customer.ViewModels
{
    public class CustomerSearchFlyoutViewModel : BindableBase
    {

        private string header;
        public string Header
        {
            get { return this.header; }
            set { SetProperty(ref this.header, value); }
        }

        private bool isOpen;
        public bool IsOpen
        {
            get { return this.isOpen; }
            set { SetProperty(ref this.isOpen, value); }
        }

        private Position position;
        public Position Position
        {
            get { return this.position; }
            set { SetProperty(ref this.position, value); }

        }

        private int _width;

        public int Width
        {
            get { return _width; }
            set { _width = value; }

        }
        private int _height;

        public int Height
        {
            get { return _height; }
            set { _height = value; }

        }

        private CustomerSearchModel _modelItem = new CustomerSearchModel();
        public CustomerSearchModel ModelItem
        {
            get => _modelItem;

            set => SetProperty(ref _modelItem, value);

        }

        private readonly INotificationService _notificationService;

        public DelegateCommand<CustomerSearchModel> CustomerSearch { get; private set; }
        public DelegateCommand<CustomerSearchModel> Reset { get; private set; }
        private IEventAggregator _eventAggregator;
        public CustomerSearchFlyoutViewModel(IEventAggregator eventAggregator, INotificationService notificationService)
        {
            _eventAggregator = eventAggregator;
            this.Width = 1600;
            this.Height = 200;
            this.Position = MahApps.Metro.Controls.Position.Top;

            _eventAggregator.GetEvent<CustomerSearchFlyoutOpen>().Subscribe((isopen) =>
            {
                this.IsOpen = isopen;

            });
            CustomerSearch = new DelegateCommand<CustomerSearchModel>(GetCustomerSearch);
            Reset = new DelegateCommand<CustomerSearchModel>(ResetCustomerSerarchInput);
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));


        }

        public void ResetCustomerSerarchInput(CustomerSearchModel item)
        {

            item.ToDate = string.Empty;
            item.FromDate = string.Empty;
            item.Name = string.Empty;
            item.InvoiceNo = string.Empty;
            item.Phone = string.Empty;


        }
        public void GetCustomerSearch(CustomerSearchModel item)
        {
            if (string.IsNullOrEmpty(item.Name) && string.IsNullOrEmpty(item.Phone) && string.IsNullOrEmpty(item.InvoiceNo)
                && string.IsNullOrEmpty(item.FromDate) && string.IsNullOrEmpty(item.ToDate))
            {
                _notificationService.ShowMessage("Select Anything To Search!!!..", NotificationType.Error);

                return;
            }

            if (String.IsNullOrEmpty(item.FromDate) && !string.IsNullOrEmpty(item.ToDate))
            {
                _notificationService.ShowMessage("Please enter from Date", NotificationType.Error);
                return;

            }

            if (!String.IsNullOrEmpty(item.FromDate) && !string.IsNullOrEmpty(item.ToDate))
            {
                DateTime dt1 = Convert.ToDateTime(item.FromDate);
                DateTime dt2 = Convert.ToDateTime(item.ToDate);
                if (dt2 < dt1)
                {
                    _notificationService.ShowMessage("From Date should be less than or equal to To Date", NotificationType.Error);
                    return;
                }
            }

            CustomerSearchModel modelRequest = new CustomerSearchModel()
            {

                Name = string.IsNullOrEmpty(item.Name) ? null : item.Name,
                Phone = string.IsNullOrEmpty(item.Phone) ? null : item.Phone,
                InvoiceNo = string.IsNullOrEmpty(item.InvoiceNo) ? null : item.InvoiceNo,
                FromDate = string.IsNullOrEmpty(item.FromDate) ? null : item.FromDate,
                ToDate = string.IsNullOrEmpty(item.ToDate) ? null : item.ToDate,
                Location = AppConstants.UserName,

            };

            _eventAggregator.GetEvent<CustomerSearchFlyout>().Publish(modelRequest);
            this.IsOpen = false;
        }

    }
}
