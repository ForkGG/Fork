using ForkCommon.Model.Notifications;

namespace ForkFrontend.Model;

public class NotificationHandler<T> : INotificationHandler where T : AbstractNotification
{
    public NotificationHandler()
    {
        Handlers = new List<Func<T, Task>>();
    }

    public List<Func<T, Task>> Handlers { get; }
    public Type Type => typeof(T);

    public async Task CallHandlers(AbstractNotification abstractNotification)
    {
        if (abstractNotification is not T notification)
        {
            throw new ArgumentException(
                "A NotificationHandler should only get Notifications of its type.\nTHIS SHOULD NEVER EVER HAPPEN!");
        }

        foreach (Func<T, Task> handler in Handlers) await handler.Invoke(notification);
    }
}