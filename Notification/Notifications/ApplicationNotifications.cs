using ProjectAvery.Logic.Model.ApplicationModel;
using ProjectAvery.Logic.Model.NotificationModel.ApplicationNotificationModel;
using ProjectAveryCommon.ExtensionMethods;

namespace ProjectAvery.Notification.Notifications
{
    /// <summary>
    /// Methods to create all kinds of Notifications in JSON format
    /// </summary>
    public class ApplicationNotifications : NotificationBase
    {
        public string ApplicationStateChangedNotification(ApplicationStatus oldStatus, ApplicationStatus newStatus)
        {
            var notification = new StateChangeNotification{NewStatus = newStatus, OldStatus = oldStatus};
            return notification.ToJson();
        }
    }
}