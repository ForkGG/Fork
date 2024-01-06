using ForkCommon.Model.Notifications;
using ForkFrontend.Model;

namespace ForkFrontend.Logic.Services.Notifications;

public interface INotificationService
{
    delegate void WebsocketStatusChangedHandler(WebsocketStatus newStatus);

    event WebsocketStatusChangedHandler? WebsocketStatusChanged;

    void Register<T>(Func<T, Task> handler) where T : AbstractNotification;

    void Unregister<T>(object caller) where T : AbstractNotification;
    Task StartupAsync();
}