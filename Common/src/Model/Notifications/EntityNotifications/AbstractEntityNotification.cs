namespace ForkCommon.Model.Notifications.EntityNotifications;

/// <summary>
///     Abstract Class for all notifications that belong to a single entity
/// </summary>
public abstract class AbstractEntityNotification : AbstractNotification
{
    protected AbstractEntityNotification(ulong entityId)
    {
        EntityId = entityId;
    }

    // ID of the entity this notification belongs to
    public ulong EntityId { get; set; }
}