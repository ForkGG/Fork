using ForkCommon.Model.Notifications.EntityNotifications;

namespace ForkFrontend.Logic.Services.Notifications.NotificationHandlers.EntityNotificationHandlers;

public abstract class AbstractEntityNotificationHandler<T> : AbstractNotificationHandler<T> where T : AbstractEntityNotification
{
    public ulong EntityId { get; set; }

    protected AbstractEntityNotificationHandler(ulong entityId)
    {
        EntityId = entityId;
    }

    public override async Task HandleNotification(T notification)
    {
        if (notification.EntityId == EntityId)
        {
            await base.HandleNotification(notification);
        }
    }
}