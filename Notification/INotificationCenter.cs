using System.Threading.Tasks;
using ProjectAvery.Logic.Model.ApplicationModel;

namespace ProjectAvery.Notification
{
    public interface INotificationCenter
    {
        Task SendApplicationStateChangedNotification(ApplicationStatus oldStatus, ApplicationStatus newStatus);
    }
}