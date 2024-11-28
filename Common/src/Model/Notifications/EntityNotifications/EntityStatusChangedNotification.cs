using ForkCommon.Model.Entity.Enums;
using ForkCommon.Model.Privileges;
using ForkCommon.Model.Privileges.Entity.ReadEntity;

namespace ForkCommon.Model.Notifications.EntityNotifications;

[Privileges(typeof(IReadEntityPrivilege))]
public class EntityStatusChangedNotification : AbstractEntityNotification
{
    public EntityStatusChangedNotification(ulong entityId, EntityStatus newEntityStatus) : base(entityId)
    {
        NewEntityStatus = newEntityStatus;
    }

    public EntityStatus NewEntityStatus { get; set; }
}