using System.Threading.Tasks;
using ProjectAveryCommon.Model.Notifications;

namespace ProjectAvery.Logic.Notification
{
    public interface INotificationCenter
    {
        Task BroadcastNotification(AbstractNotification notification);
    }
}