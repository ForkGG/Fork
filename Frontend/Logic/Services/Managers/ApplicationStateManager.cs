using ForkCommon.Model.Application;
using ForkCommon.Model.Entity.Pocos;
using ForkFrontend.Logic.Services.Connections;
using ForkFrontend.Logic.Services.Notifications;
using ForkFrontend.Model.Enums;

namespace ForkFrontend.Logic.Services.Managers;

public class ApplicationStateManager : IApplicationStateManager
{
    private readonly IApplicationConnectionService _applicationConnection;
    private readonly ILogger<ApplicationStateManager> _logger;
    private readonly INotificationService _notificationService;

    private bool _isStateReady;
    private WebsocketStatus _websocketStatus = WebsocketStatus.Disconnected;

    public ApplicationStateManager(ILogger<ApplicationStateManager> logger,
        IApplicationConnectionService applicationConnection, INotificationService notificationService)
    {
        _logger = logger;
        _applicationConnection = applicationConnection;
        _notificationService = notificationService;


        ApplicationState = _applicationConnection.GetApplicationState().GetAwaiter().GetResult();
        ForkExternalIp = _applicationConnection.GetIpAddress().GetAwaiter().GetResult();

        _notificationService.WebsocketStatusChanged += async newStatus =>
        {
            WebsocketStatus = newStatus;
            await UpdateState();
        };
    }

    public event IApplicationStateManager.HandleAppStatusChanged? AppStatusChanged;
    public event IApplicationStateManager.HandleAppStateChanged? AppStateChanged;
    public bool IsApplicationReady => _isStateReady && WebsocketStatus == WebsocketStatus.Connected;
    public State ApplicationState { get; private set; }
    public string ForkExternalIp { get; private set; }

    public Dictionary<ulong, EntityStateManager> EntityStateManagersById { get; } = new();

    public WebsocketStatus WebsocketStatus
    {
        get => _websocketStatus;
        set
        {
            _websocketStatus = value;
            AppStatusChanged?.Invoke();
        }
    }

    public ApplicationStatus ApplicationStatus
    {
        get
        {
            if (!_isStateReady)
            {
                return ApplicationStatus.RetrievingState;
            }

            if (WebsocketStatus != WebsocketStatus.Connected)
            {
                return ApplicationStatus.WaitingForWebsocket;
            }

            return ApplicationStatus.Ready;
        }
    }


    public async Task UpdateState()
    {
        _isStateReady = false;
        _logger.LogInformation("Refreshing application state...");
        ApplicationState = await _applicationConnection.GetApplicationState();
        UpdateEntityManagers();
        ForkExternalIp = await _applicationConnection.GetIpAddress();
        _isStateReady = true;
        AppStatusChanged?.Invoke();
        AppStateChanged?.Invoke();
    }

    private void UpdateEntityManagers()
    {
        // TODO CKE do we need to remove some here? Thinking of including multiple States we might not

        foreach (IEntity entity in ApplicationState.Entities)
            if (EntityStateManagersById.ContainsKey(entity.Id))
            {
                // Update existing entity
                EntityStateManagersById[entity.Id].UpdateEntity(entity);
            }
            else
            {
                // Add new entity
                EntityStateManager entityManager = new(entity, _notificationService);
                EntityStateManagersById.Add(entity.Id, entityManager);
            }
    }
}