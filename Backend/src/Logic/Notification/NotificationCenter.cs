using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fleck;
using Fork.Logic.Managers;
using ForkCommon.ExtensionMethods;
using ForkCommon.Model.Notifications;
using ForkCommon.Model.Privileges;
using Microsoft.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Fork.Logic.Notification;

/// <summary>
///     <b>Default implementation of the NotificationCenter</b>
///     <br />
///     This is a large wrapper for all kinds of notifications. It creates them and sends them to all subscribers
///     of the websocket.
///     <br />
///     This should be used as a singleton to only create one WebSocket instance per application
/// </summary>
public class NotificationCenter
{
    private readonly ILogger<NotificationCenter> _logger;
    private readonly Dictionary<IWebSocketConnection, IReadOnlySet<IPrivilege>?> _privilegesByConnection;
    private readonly WebSocketServer _server;
    private readonly TokenManager _tokenManager;

    public NotificationCenter(ILogger<NotificationCenter> logger, TokenManager tokenManager)
    {
        _logger = logger;
        _tokenManager = tokenManager;

        FleckLog.LogAction = (level, message, exception) => { _logger.Log(MapLogLevel(level), exception, message); };

        _server = new WebSocketServer("ws://0.0.0.0:35566");
        // A Dictionary containing all active sockets and their privileges (or null if no token was provided yet)
        _privilegesByConnection = new Dictionary<IWebSocketConnection, IReadOnlySet<IPrivilege>?>();
        _server.RestartAfterListenError = true;
        //_server.ListenerSocket.NoDelay = true;
        _server.Start(socket =>
        {
            socket.OnOpen = () =>
            {
                _logger.LogInformation($"New WebSocket connection from {socket.ConnectionInfo.Origin}");
                if (_privilegesByConnection.ContainsKey(socket))
                {
                    _privilegesByConnection.Remove(socket);
                }

                _privilegesByConnection.Add(socket, null);
            };
            socket.OnClose = () =>
            {
                _logger.LogInformation($"Websocket connection closed: {socket.ConnectionInfo.Headers}");
                _privilegesByConnection.Remove(socket);
            };
            socket.OnMessage = message =>
            {
                if (!_privilegesByConnection.ContainsKey(socket))
                {
                    socket.Close(1);
                }

                _privilegesByConnection[socket] = _tokenManager.GetPrivilegesForToken(message);
            };
            socket.OnBinary = bytes => { _logger.LogWarning("Received binary WebSocket message (Not supported!)"); };
        });
    }

    public async Task BroadcastNotification(AbstractNotification notification)
    {
        string message = notification.ToJson();
        _logger.LogTrace($"Sending notification to {_privilegesByConnection.Count} clients: {message}");
        int actualMessagesSent = 0;
        foreach (KeyValuePair<IWebSocketConnection, IReadOnlySet<IPrivilege>?> privilegeByConnection in
                 _privilegesByConnection)
            // If the socket has not provided a valid token don't send any messages
            if (privilegeByConnection.Value != null)
            {
                // If Notification requires privilege(s) and the socket has all of them broadcast the message
                if (Attribute.GetCustomAttributes(notification.GetType()).All(a =>
                        a is not PrivilegesAttribute || (a is PrivilegesAttribute p &&
                                                         privilegeByConnection.Value.Any(ip =>
                                                             ip is AdminPrivilege ||
                                                             ip.GetType().IsAssignableFrom(p.Privilege) ||
                                                             p.Privilege == ip.GetType()))))
                {
                    try
                    {
                        await privilegeByConnection.Key.Send(message);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error while sending notification");
                    }

                    actualMessagesSent++;
                }
            }

        _logger.LogTrace($"Notification was actually sent to {actualMessagesSent} clients: {message}");
    }

    private LogLevel MapLogLevel(Fleck.LogLevel logLevel)
    {
        switch (logLevel)
        {
            case Fleck.LogLevel.Debug: return LogLevel.Trace;
            case Fleck.LogLevel.Info: return LogLevel.Information;
            case Fleck.LogLevel.Warn: return LogLevel.Warning;
            case Fleck.LogLevel.Error: return LogLevel.Error;
            default: return LogLevel.None;
        }
    }
}