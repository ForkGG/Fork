using System.Threading.Tasks;
using ForkCommon.Model.Entity.Pocos;
using ForkCommon.Model.Notifications.EntityNotifications;

namespace ForkFrontend.Logic.Services.Notifications.NotificationHandlers.EntityNotificationHandlers;

public class VanillaSettingsChangedNotificationHandler
    : AbstractEntityNotificationHandler<VanillaSettingsChangedNotification>
{
    private readonly Server _server;

    public VanillaSettingsChangedNotificationHandler(Server server) : base(server.Id)
    {
        _server = server;
    }

    protected override async Task UpdateModel(VanillaSettingsChangedNotification notification)
    {
        _server.VanillaSettings = notification.UpdatedSettings;
        await base.UpdateModel(notification);
    }
}
