using System.Diagnostics;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using ForkCommon.ExtensionMethods;
using ForkCommon.Model.Notifications;
using ForkFrontend.Model;
using ForkFrontend.Model.Enums;

namespace ForkFrontend.Logic.Services.Notifications;

public class NotificationService
{
    public delegate void WebsocketStatusChangedHandler(WebsocketStatus newStatus);

    private const int BUFFER_SIZE = 2048;

    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly ILogger<NotificationService> _logger;
    private readonly Uri _webSocketUri = new("ws://localhost:35566");
    private ClientWebSocket? _webSocket;

    private WebsocketStatus _websocketStatus = WebsocketStatus.Disconnected;

    public NotificationService(ILogger<NotificationService> logger)
    {
        logger.LogInformation("Initializing NotificationService");
        _logger = logger;
        _cancellationTokenSource = new CancellationTokenSource();
        RegisteredHandlers = new List<INotificationHandler>();
    }

    private WebsocketStatus WebsocketStatus
    {
        get => _websocketStatus;
        set
        {
            _websocketStatus = value;
            WebsocketStatusChanged?.Invoke(value);
        }
    }

    private List<INotificationHandler> RegisteredHandlers { get; }
    public event WebsocketStatusChangedHandler? WebsocketStatusChanged;

    public void Register<T>(Func<T, Task> handler) where T : AbstractNotification
    {
        _logger.LogDebug($"Registering new NotificationHandler `{handler.Method.Name}` for {typeof(T)}");

        if (RegisteredHandlers.FirstOrDefault(h => h.GetType() == typeof(NotificationHandler<T>)) is not
            NotificationHandler<T> notificationHandler)
        {
            notificationHandler = new NotificationHandler<T>();
            RegisteredHandlers.Add(notificationHandler);
        }

        notificationHandler.Handlers.Add(handler);
    }

    public void Unregister<T>(object caller) where T : AbstractNotification
    {
        if (RegisteredHandlers.FirstOrDefault(h => h.GetType() == typeof(NotificationHandler<T>)) is
            NotificationHandler<T> notificationHandler)
        {
            int count = notificationHandler.Handlers.RemoveAll(f => f.Target == caller);
            _logger.LogDebug(
                $"Unregistered {count} NotificationHandlers for {typeof(T)}");
        }
    }

    public async Task StartupAsync()
    {
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            _webSocket = new ClientWebSocket();
            try
            {
                IAsyncEnumerable<string> messages = ConnectAsync(_cancellationTokenSource.Token);
                await foreach (string message in messages) await HandleMessage(message);
                WebsocketStatus = WebsocketStatus.Disconnected;
                _logger.LogDebug("Websocket closed. Reconnecting in 500ms");
                _webSocket.Abort();
                _webSocket.Dispose();
                await Task.Delay(500);
            }
            catch (Exception e)
            {
                _logger.LogDebug($"Error in Websocket: {e.Message}");
                _webSocket.Abort();
                _webSocket.Dispose();
                await Task.Delay(500);
            }
        }
    }

    private async Task HandleMessage(string message)
    {
        AbstractNotification? notification = message.FromJson<AbstractNotification>();
        if (notification == null)
        {
            _logger.LogDebug("Can't handle notification because it was null");
            return;
        }

        // Make sure only handlers of the notifications type are called
        RegisteredHandlers.FirstOrDefault(h => h.Type == notification.GetType())?.CallHandlers(notification);
    }

    /// <summary>
    ///     Connect to the websocket and begin yielding messages
    ///     received from the connection.
    /// </summary>
    private async IAsyncEnumerable<string> ConnectAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (_webSocket == null)
        {
            yield break;
        }

        await _webSocket.ConnectAsync(_webSocketUri, cancellationToken);
        WebsocketStatus = WebsocketStatus.Connected;
        // TODO CKE actual token
        await SendMessageAsync("dummyToken", cancellationToken);
        ArraySegment<byte> buffer = new(new byte[BUFFER_SIZE]);
        while (!cancellationToken.IsCancellationRequested)
        {
            WebSocketReceiveResult result;
            await using MemoryStream ms = new();
            do
            {
                result = await _webSocket.ReceiveAsync(buffer, cancellationToken);
                Debug.Assert(buffer.Array != null);
                ms.Write(buffer.Array, buffer.Offset, result.Count);
            } while (!result.EndOfMessage);

            ms.Seek(0, SeekOrigin.Begin);

            yield return Encoding.UTF8.GetString(ms.ToArray());

            if (result.MessageType == WebSocketMessageType.Close)
            {
                break;
            }
        }
    }

    private async Task SendMessageAsync(string message, CancellationToken cancellationToken)
    {
        if (WebsocketStatus != WebsocketStatus.Connected)
        {
            _logger.LogError($"Failed to write WebSocket message. WebSocket is not connected!\n{message}");
            return;
        }

        if (_webSocket == null || _webSocket.State != WebSocketState.Open)
        {
            _logger.LogError(
                $"Failed to write WebSocket message. WebSocket connection is either not existent or closed!\n{message}");
            return;
        }

        byte[] messageInBytes = Encoding.UTF8.GetBytes(message);
        _logger.LogDebug($"Sending message with {messageInBytes.Length} Bytes");
        for (int i = 0; i < messageInBytes.Length; i += BUFFER_SIZE)
        {
            ArraySegment<byte> chunk = new(messageInBytes.Skip(i).Take(BUFFER_SIZE).ToArray());
            await _webSocket.SendAsync(chunk, WebSocketMessageType.Text, i + BUFFER_SIZE >= messageInBytes.Length,
                cancellationToken);
        }
    }
}