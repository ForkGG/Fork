using ProjectAvery.Logic.Model.ApplicationModel;

namespace ProjectAvery.Logic.Model.NotificationModel.ApplicationNotificationModel
{
    public class StateChangeNotification
    {
        public ApplicationStatus OldStatus { get; set; }
        public ApplicationStatus NewStatus { get; set; }
    }
}