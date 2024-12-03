using ForkCommon.Model.Application.Exceptions;
using ForkFrontend.Logic.Services.Managers;
using ForkFrontend.Model;
using ForkFrontend.Model.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace ForkFrontend.Razor.Components.Shared;

public partial class ForkErrorBoundary : ErrorBoundary
{
    [Inject] public required ToastManager ToastManager { private get; set; }
    [Inject] public required ILogger<ForkErrorBoundary> Logger { private get; set; }

    [Parameter] public bool FailSilently { private get; set; }

    protected override async Task OnErrorAsync(Exception exception)
    {
        if (exception is ForkException forkException)
        {
            if (FailSilently)
            {
                Logger.LogWarning(forkException, "Silent app exception thrown");
                return;
            }

            Logger.LogError(forkException, "App exception thrown");
            await ToastManager.AddToast(new Toast(ToastLevel.Error, forkException.Message));
        }
        else
        {
            Logger.LogError(exception, "Unexpected exception thrown");
            await ToastManager.AddToast(new Toast(ToastLevel.Error, $"Unexpected error occured: {exception.Message}"));
        }
    }

    public async Task<T?> RunSavely<T>(Task<T> task)
    {
        try
        {
            return await task;
        }
        catch (Exception ex)
        {
            await OnErrorAsync(ex);
            return default;
        }
    }
}