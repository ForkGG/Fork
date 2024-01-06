using ForkCommon.Model.Entity.Pocos;
using ForkCommon.Model.Entity.Pocos.Player;
using ForkCommon.Model.Notifications.EntityNotifications.PlayerNotifications;

namespace ForkFrontend.Logic.Services.Notifications.NotificationHandlers.EntityNotificationHandlers.
    PlayerNotificationHandlers;

public class UpdatePlayerNotificationHandler : AbstractEntityNotificationHandler<UpdatePlayerNotification>
{
    private readonly List<ServerPlayer> _serverPlayers;

    public UpdatePlayerNotificationHandler(Server server) : base(server.Id)
    {
        _serverPlayers = server.ServerPlayers;
    }

    protected override async Task UpdateModel(UpdatePlayerNotification notification)
    {
        ServerPlayer? existing =
            _serverPlayers.FirstOrDefault(p => p.Player.Uid == notification.ServerPlayer.Player.Uid);
        if (existing != null)
        {
            existing.IsOnline = notification.ServerPlayer.IsOnline;
            existing.IsOp = notification.ServerPlayer.IsOp;
        }
        else
        {
            _serverPlayers.Add(notification.ServerPlayer);
        }

        await base.UpdateModel(notification);
    }
}