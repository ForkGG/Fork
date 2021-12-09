using System.Threading.Tasks;
using ProjectAvery.Logic.Model.ApplicationModel;
using ProjectAveryCommon.Model.Notifications;

namespace ProjectAvery.Notification
{
    public interface INotificationCenter
    {
        Task BroadcastNotification(AbstractNotification notification);
    }
}