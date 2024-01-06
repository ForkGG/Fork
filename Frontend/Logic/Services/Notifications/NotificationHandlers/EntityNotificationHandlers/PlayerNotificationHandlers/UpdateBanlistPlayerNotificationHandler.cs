using ForkCommon.Model.Entity.Enums.Player;
using ForkCommon.Model.Entity.Pocos;
using ForkCommon.Model.Entity.Pocos.Player;
using ForkCommon.Model.Notifications.EntityNotifications.PlayerNotifications;

namespace ForkFrontend.Logic.Services.Notifications.NotificationHandlers.EntityNotificationHandlers.
    PlayerNotificationHandlers;

public class UpdateBanlistPlayerNotificationHandler : AbstractEntityNotificationHandler<UpdateBanlistPlayerNotification>
{
    private readonly List<Player> _banlist;

    public UpdateBanlistPlayerNotificationHandler(Server server) : base(server.Id)
    {
        server.Banlist ??= new List<Player>();
        _banlist = server.Banlist;
    }

    protected override async Task UpdateModel(UpdateBanlistPlayerNotification notification)
    {
        if (notification.UpdateType == PlayerlistUpdateType.Remove)
        {
            _banlist.RemoveAll(p => p.Uid == notification.Player.Uid);
        }
        else
        {
            Player? existing = _banlist.FirstOrDefault(p => p.Uid == notification.Player.Uid);
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
                _banlist.Add(notification.Player);
            }
        }

        await base.UpdateModel(notification);
    }
}