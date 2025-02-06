using FalcaPOS.Common;
using Prism.Events;
using System;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Core;
using ToastNotifications.Lifetime;
using ToastNotifications.Lifetime.Clear;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace FalcaPOS.Notification
{
    public class Notify
    {
        private readonly Notifier notifier;
        private IEventAggregator _eventAggregator;
        private readonly MessageOptions _messageOptions;

        public Notify(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            _messageOptions = new MessageOptions { FreezeOnMouseEnter = false };

            notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.BottomCenter,
                    offsetX: 50,
                    offsetY: 50);
                cfg.DisplayOptions.TopMost = true;
                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(5),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(3));
                cfg.Dispatcher = Application.Current.Dispatcher;
            });
            _eventAggregator.GetEvent<FalcaPOS.Common.Events.NotifyMessage>().Subscribe((s) =>
            {
                if (s != null)
                {
                    ShowNotification(s.Message, s.MessageType);
                }

            });
        }

        public void ShowNotification(String Message, NotificationType Type)
        {
            Application.Current?.Dispatcher?.Invoke(() =>
            {

                notifier?.ClearMessages(new ClearAll());

                switch (Type)
                {
                    case NotificationType.Information:
                        notifier.ShowInformation(Message, _messageOptions);
                        break;
                    case NotificationType.Success:
                        notifier.ShowSuccess(Message, _messageOptions);
                        break;
                    case NotificationType.Warning:
                        notifier.ShowWarning(Message, _messageOptions);
                        break;
                    case NotificationType.Error:
                        notifier.ShowError(Message, _messageOptions);
                        break;
                    default:
                        break;
                }
            });


        }


    }
}
