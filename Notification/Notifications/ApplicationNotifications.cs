using ProjectAvery.Logic.Model.ApplicationModel;
using ProjectAvery.Logic.Model.NotificationModel.ApplicationNotificationModel;
using ProjectAvery.Util.ExtensionMethods;

namespace ProjectAvery.Notification.Notifications
{
    /// <summary>
    /// Methods to create all kinds of Notifications in JSON format
    /// </summary>
    public class ApplicationNotifications : NotificationBase
    {
        public string ApplicationStateChangedNotification(ApplicationState oldState, ApplicationState newState)
        {
            var notification = new StateChangeNotification{NewState = newState, OldState = oldState};
            return notification.ToJson();
        }
    }
}