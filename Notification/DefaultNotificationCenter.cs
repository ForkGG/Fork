using System.Collections.Generic;
using System.Threading.Tasks;
using Fleck;
using Microsoft.Extensions.Logging;
using ProjectAvery.Logic.Model.ApplicationModel;
using ProjectAvery.Notification.Notifications;
using ProjectAveryCommon.ExtensionMethods;
using ProjectAveryCommon.Model.Notifications;

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
        private readonly WebSocketServer _server;
        private readonly List<IWebSocketConnection> _connections;

        public DefaultNotificationCenter(ILogger<DefaultNotificationCenter> logger)
        {
            _logger = logger;

            _server = new WebSocketServer("ws://0.0.0.0:35566");
            _connections = new List<IWebSocketConnection>();
            _server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    _logger.LogInformation($"New WebSocket connection from {socket.ConnectionInfo.Host}");
                    _connections.Add(socket);
                };
                socket.OnClose = () =>
                {
                    _logger.LogInformation($"Websocket connection closed: {socket.ConnectionInfo.Headers}");
                    _connections.Remove(socket);
                };
            });
            
            // Initialize Notification Creators
            _applicationNotifications = new ApplicationNotifications();
        }

        public async Task BroadcastNotification(AbstractNotification notification)
        {
            //TODO CKE only notify clients with the according permissions
            string message = notification.ToJson();
            _logger.LogDebug($"Sending notification to {_connections.Count} clients: {message}");
            foreach (IWebSocketConnection connection in _connections)
            {
                await connection.Send(message);
            }
        }
    }
}