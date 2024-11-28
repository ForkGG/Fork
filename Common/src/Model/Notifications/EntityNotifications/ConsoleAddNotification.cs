using ForkCommon.Model.Entity.Transient.Console;
using ForkCommon.Model.Privileges;
using ForkCommon.Model.Privileges.Entity.ReadEntity.ReadConsoleTab;

namespace ForkCommon.Model.Notifications.EntityNotifications;

[Privileges(typeof(ReadConsoleConsoleTabPrivilege))]
public class ConsoleAddNotification : AbstractEntityNotification
{
    public ConsoleAddNotification(ulong entityId, ConsoleMessage newConsoleMessage) : base(entityId)
    {
        NewConsoleMessage = newConsoleMessage;
    }

    // The message that was added to the console
    public ConsoleMessage NewConsoleMessage { get; set; }
}