using ForkCommon.Model.Application;
using ForkFrontend.Model.Enums;

namespace ForkFrontend.Logic.Services.Managers;

public interface IApplicationStateManager
{
    public delegate void HandleAppStateChanged();

    public delegate void HandleAppStatusChanged();

    public delegate void HandleUiLoadingChanged();

    public bool IsApplicationReady { get; }
    public State ApplicationState { get; }
    public WebsocketStatus WebsocketStatus { get; set; }
    public ApplicationStatus ApplicationStatus { get; }
    public string ForkExternalIp { get; }
    public Dictionary<ulong, EntityStateManager> EntityStateManagersById { get; }
    public bool UiLoadingBlocked { get; }
    public string? UiLoadingTextPath { get; }

    public event HandleAppStatusChanged? AppStatusChanged;
    public event HandleAppStateChanged? AppStateChanged;
    public event HandleUiLoadingChanged? UiLoadingChanged;

    public Task UpdateState();

    public Task<T> RunWithLoadingText<T>(Task<T> task);
    public Task<T> RunWithLoadingText<T>(Task<T> task, string? textPath);
}