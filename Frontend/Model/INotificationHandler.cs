using ForkCommon.Model.Notifications;

namespace ForkFrontend.Model;

public interface INotificationHandler
{
    public Type Type { get; }

    public Task CallHandlers(AbstractNotification abstractNotification);
}