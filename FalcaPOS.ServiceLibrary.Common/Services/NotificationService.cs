using FalcaPOS.Common;
using FalcaPOS.Common.Events;
using FalcaPOS.Contracts;
using Prism.Events;
using System;

namespace FalcaPOS.ServiceLibrary.Common.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IEventAggregator _eventAggregator;

        public NotificationService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
        }

        public void ShowMessage(string msg, NotificationType notificationType)
        {

            _eventAggregator.GetEvent<NotifyMessage>().Publish(new FalcaPOS.Common.Models.ToastMessage
            {
                Message = msg,
                MessageType = notificationType
            });
        }

    }
}
