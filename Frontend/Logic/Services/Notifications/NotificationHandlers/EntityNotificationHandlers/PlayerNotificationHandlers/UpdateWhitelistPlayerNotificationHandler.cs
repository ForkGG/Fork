using ForkCommon.Model.Entity.Enums.Player;
using ForkCommon.Model.Entity.Pocos;
using ForkCommon.Model.Entity.Pocos.Player;
using ForkCommon.Model.Notifications.EntityNotifications.PlayerNotifications;

namespace ForkFrontend.Logic.Services.Notifications.NotificationHandlers.EntityNotificationHandlers.
    PlayerNotificationHandlers;

public class
    UpdateWhitelistPlayerNotificationHandler : AbstractEntityNotificationHandler<UpdateWhitelistPlayerNotification>
{
    private readonly List<Player> _whitelist;

    public UpdateWhitelistPlayerNotificationHandler(Server server) : base(server.Id)
    {
        server.Whitelist ??= new List<Player>();
        _whitelist = server.Whitelist;
    }

    protected override async Task UpdateModel(UpdateWhitelistPlayerNotification notification)
    {
        if (notification.UpdateType == PlayerlistUpdateType.Remove)
        {
            _whitelist.RemoveAll(p => p.Uid == notification.Player.Uid);
        }
        else
        {
            Player? existing = _whitelist.FirstOrDefault(p => p.Uid == notification.Player.Uid);
            // Update
            if (existing != null)
            {
                existing.Head = notification.Player.Head;
                existing.Name = notification.Player.Name;
                existing.LastUpdated = notification.Player.LastUpdated;
                existing.IsOfflinePlayer = notification.Player.IsOfflinePlayer;
            }
            // Add
            else
            {
                _whitelist.Add(notification.Player);
            }
        }

        await base.UpdateModel(notification);
    }
}