using ForkFrontend.Model;

namespace ForkFrontend.Logic.Services.Managers;

public class ToastManager : IDisposable
{
    public delegate void ToastsUpdatedHandler();

    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly ILogger<ToastManager> _logger;

    public ToastManager(ILogger<ToastManager> logger)
    {
        _logger = logger;
    }

    public List<Toast> Toasts { get; } = new();

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
    }

    public event ToastsUpdatedHandler? ToastsUpdated;

    public async Task AddToast(Toast toast)
    {
        _logger.LogDebug($"Registered toast: {toast}");
        Toasts.Add(toast);
        while (Toasts.Count > 10) Toasts.RemoveAt(0);
        ToastsUpdated?.Invoke();
        if (toast.HideDuration != null)
        {
            await Task.Delay(toast.HideDuration.Value, _cancellationTokenSource.Token);
            Toasts.Remove(toast);
            ToastsUpdated?.Invoke();
        }
    }
}