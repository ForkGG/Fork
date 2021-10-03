using ProjectAvery.Logic.Model.ApplicationModel;

namespace ProjectAvery.Logic.Model.NotificationModel.ApplicationNotificationModel
{
    public class StateChangeNotification
    {
        public ApplicationState OldState { get; set; }
        public ApplicationState NewState { get; set; }
    }
}