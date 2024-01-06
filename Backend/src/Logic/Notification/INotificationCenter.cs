using System.Threading.Tasks;
using ForkCommon.Model.Notifications;

namespace Fork.Logic.Notification;

public interface INotificationCenter
{
    Task BroadcastNotification(AbstractNotification notification);
}