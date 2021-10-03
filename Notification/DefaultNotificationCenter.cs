using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProjectAvery.Logic.Model.ApplicationModel;
using ProjectAvery.Notification.Notifications;

namespace ProjectAvery.Notification
{
    /// <summary>
    /// <b>Default implementation of the NotificationCenter</b>
    /// <br>
    /// This is a large wrapper for all kinds of notifications. It creates them and sends them to all subscribers
    /// of the websocket.<br>
    /// This should be used as a singleton to only create one WebSocket instance per application
    /// </summary>
    public class DefaultNotificationCenter : INotificationCenter
    {
        private readonly ILogger<DefaultNotificationCenter> _logger;
        private readonly ApplicationNotifications _applicationNotifications;

        public DefaultNotificationCenter(ILogger<DefaultNotificationCenter> logger)
        {
            _logger = logger;

            //TODO CKE try to start WebSocket here
            
            // Initialize Notification Creators
            _applicationNotifications = new ApplicationNotifications();
        }

        public async Task SendApplicationStateChangedNotification(ApplicationState oldState, ApplicationState newState)
        {
            string notification = _applicationNotifications.ApplicationStateChangedNotification(oldState, newState);
            await BroadcastNotification(notification);
        }

        private async Task BroadcastNotification(string notification)
        {
            //TODO CKE broadcast this notification to all subscribers
            _logger.LogDebug("New notification: "+notification);
        }
    }
}