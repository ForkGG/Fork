using System.Collections.Generic;
using ForkCommon.Model.Entity.Pocos;
using ForkCommon.Model.Privileges;
using ForkCommon.Model.Privileges.Entity.ReadEntity;

namespace ForkCommon.Model.Notifications.EntityNotifications;

[Privileges(typeof(IReadEntityPrivilege))]
public class EntityListUpdatedNotification : AbstractNotification
{
    public EntityListUpdatedNotification(List<IEntity> entities)
    {
        Entities = entities;
    }

    public List<IEntity> Entities { get; set; }
}