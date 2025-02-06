using FalcaPOS.Common.Events;
using FalcaPOS.Common.Helper;
using FalcaPOS.Common.Logger;
using FalcaPOS.Common.Media;
using FalcaPOS.Entites.Notification;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace FalcaPOS.Shell.ViewModels
{
    public class NotificationViewModel : FlyoutBaseViewModel
    {
        private IEventAggregator _eventAggregator;
        private ObservableCollection<POSNotification> _notifications;
        private Logger _logger;

        public NotificationViewModel(IEventAggregator eventAggregator, Logger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Header = "Notification";
            Position = MahApps.Metro.Controls.Position.Right;
            Width = 400;
            Height = System.Windows.GridLength.Auto;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<ShowNotificationEvent>().Subscribe(() =>
            {
                IsOpen = true;
                if (Notifications?.Count > 0)
                {
                    Notifications.Select(x => { x.Timetext = DateTimeHumanizer.HumanizeString(x.ReceivedTime); return x; }).
                    OrderBy(x => x.ReceivedTime.Second).
                    ToList();
                }
            });

            Notifications = new ObservableCollection<POSNotification>();
            _eventAggregator.GetEvent<NotificationEvent>().Subscribe(POSNotification, ThreadOption.PublisherThread);
        }

        public void POSNotification(POSNotification pOSNotification)
        {
            try
            {
                if (Notifications?.Count > 0) Notifications.Add(pOSNotification);
                else
                {
                    Notifications = new ObservableCollection<POSNotification>()
                    {
                        pOSNotification
                    };
                }

                _eventAggregator?.GetEvent<NotificationCountEvent>().Publish(Notifications.Count);

                MediaPlay media = new MediaPlay();
                media.Play("/Notification/MessgeTone.mp3");

                if (Notifications?.Count > 0)
                {
                    Notifications = new ObservableCollection<POSNotification>(Notifications.OrderBy(x => x.ReceivedTime.Second).ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }


        public ObservableCollection<POSNotification> Notifications
        {
            get { return _notifications; }
            set { SetProperty(ref _notifications, value); }
        }
    }



}
