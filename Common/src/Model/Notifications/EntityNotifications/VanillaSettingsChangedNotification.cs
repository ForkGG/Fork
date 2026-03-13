using ForkCommon.Model.Entity.Pocos.Settings;
using ForkCommon.Model.Privileges;
using ForkCommon.Model.Privileges.Entity.ReadEntity.ReadSettingsTab;

namespace ForkCommon.Model.Notifications.EntityNotifications;

[Privileges(typeof(ReadVanillaSettingsTabPrivilege))]
public class VanillaSettingsChangedNotification : AbstractEntityNotification
{
    public VanillaSettingsChangedNotification(ulong entityId, VanillaSettings updatedSettings) : base(entityId)
    {
        UpdatedSettings = updatedSettings;
    }

    public VanillaSettings UpdatedSettings { get; set; }
}
