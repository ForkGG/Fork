using ForkCommon.Model.Entity.Pocos.Player;
using ForkCommon.Model.Privileges;
using ForkCommon.Model.Privileges.Entity.ReadEntity.ReadConsoleTab;

namespace ForkCommon.Model.Notifications.EntityNotifications.PlayerNotifications;

/// <summary>
/// Update a player on the playerlist 
/// </summary>
[Privileges(typeof(ReadPlayerlistConsoleTabPrivilege))]
public class UpdatePlayerNotification : AbstractEntityNotification
{
    public ServerPlayer ServerPlayer { get; set; }
}