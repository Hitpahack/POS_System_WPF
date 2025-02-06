using FalcaPOS.Common;

namespace FalcaPOS.Contracts
{
    public interface INotificationService
    {
        void ShowMessage(string msg, NotificationType notificationType);
    }
}
