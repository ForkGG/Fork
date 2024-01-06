using ForkCommon.Model.Notifications;

namespace ForkFrontend.Logic.Services.Notifications.NotificationHandlers;

public abstract class AbstractNotificationHandler<T> where T : AbstractNotification
{
    // Use this event to do stuff after the model was updated 
    public delegate Task AfterHandler(T notification);

    // Use this event to do stuff before the model is updated
    public delegate Task BeforeHandler(T notification);

    public event BeforeHandler? Before;
    public event AfterHandler? After;

    protected void RaiseBefore(T notification)
    {
        Before?.Invoke(notification);
    }

    protected void RaiseAfter(T notification)
    {
        After?.Invoke(notification);
    }

    public virtual async Task HandleNotification(T notification)
    {
        RaiseBefore(notification);
        await UpdateModel(notification);
        RaiseAfter(notification);
    }

    protected virtual Task UpdateModel(T notification)
    {
        return Task.CompletedTask;
    }
}