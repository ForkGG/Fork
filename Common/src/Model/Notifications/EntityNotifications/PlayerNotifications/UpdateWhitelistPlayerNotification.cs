using ForkCommon.Model.Entity.Enums.Player;
using ForkCommon.Model.Entity.Pocos.Player;
using ForkCommon.Model.Privileges;
using ForkCommon.Model.Privileges.Entity.ReadEntity.ReadConsoleTab;

namespace ForkCommon.Model.Notifications.EntityNotifications.PlayerNotifications;

/// <summary>
///     Updates a Player on the Whitelist (add, remove or update)
/// </summary>
[Privileges(typeof(ReadWhitelistConsoleTabPrivilege))]
public class UpdateWhitelistPlayerNotification : AbstractEntityNotification
{
    public UpdateWhitelistPlayerNotification(ulong entityId, PlayerlistUpdateType updateType, Player player) :
        base(entityId)
    {
        UpdateType = updateType;
        Player = player;
    }

    public PlayerlistUpdateType UpdateType { get; set; }
    public Player Player { get; set; }
}