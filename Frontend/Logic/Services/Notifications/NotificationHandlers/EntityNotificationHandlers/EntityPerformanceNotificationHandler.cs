using ForkCommon.Model.Notifications.EntityNotifications;

namespace ForkFrontend.Logic.Services.Notifications.NotificationHandlers.EntityNotificationHandlers;

public class EntityPerformanceNotificationHandler : AbstractEntityNotificationHandler<EntityPerformanceNotification>
{
    public EntityPerformanceNotificationHandler(ulong entityId) : base(entityId)
    {
    }
}