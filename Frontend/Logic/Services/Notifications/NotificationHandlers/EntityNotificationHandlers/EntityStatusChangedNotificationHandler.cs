using ForkCommon.Model.Entity.Pocos;
using ForkCommon.Model.Notifications.EntityNotifications;

namespace ForkFrontend.Logic.Services.Notifications.NotificationHandlers.EntityNotificationHandlers;

public class EntityStatusChangedNotificationHandler : AbstractEntityNotificationHandler<EntityStatusChangedNotification>
{
    private readonly IEntity _entity;

    public EntityStatusChangedNotificationHandler(IEntity entity) : base(entity.Id)
    {
        _entity = entity;
    }

    protected override async Task UpdateModel(EntityStatusChangedNotification notification)
    {
        _entity.Status = notification.NewEntityStatus;
        await base.UpdateModel(notification);
    }
}