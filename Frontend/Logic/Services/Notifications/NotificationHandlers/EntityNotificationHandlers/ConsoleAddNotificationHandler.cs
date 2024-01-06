using ForkCommon.Model.Entity.Transient.Console;
using ForkCommon.Model.Notifications.EntityNotifications;

namespace ForkFrontend.Logic.Services.Notifications.NotificationHandlers.EntityNotificationHandlers;

public class ConsoleAddNotificationHandler : AbstractEntityNotificationHandler<ConsoleAddNotification>
{
    private readonly List<ConsoleMessage> _consoleMessages;

    public ConsoleAddNotificationHandler(ulong entityId, List<ConsoleMessage> consoleMessages) : base(entityId)
    {
        _consoleMessages = consoleMessages;
    }

    protected override async Task UpdateModel(ConsoleAddNotification notification)
    {
        _consoleMessages.Add(notification.NewConsoleMessage);
        await base.UpdateModel(notification);
    }
}