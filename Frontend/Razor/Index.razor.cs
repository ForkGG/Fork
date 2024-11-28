using ForkCommon.Model.Entity.Pocos;
using ForkFrontend.Logic.Services.Connections;
using ForkFrontend.Logic.Services.Managers;
using ForkFrontend.Logic.Services.Notifications;
using ForkFrontend.Razor.Components.Screens;
using ForkFrontend.Razor.Components.Screens.CreateEntity;
using Microsoft.AspNetCore.Components;

namespace ForkFrontend.Razor;

// This Page controls the whole entity logic
public partial class Index : ComponentBase
{
    // Screens can be shown instead of entities (add entity, Fork settings, ...)
    private AbstractScreenComponent? _openScreen;
    [Inject] private IApplicationConnectionService ApplicationConnection { get; set; } = default!;
    [Inject] private IApplicationStateManager ApplicationStateManager { get; set; } = default!;
    [Inject] private INotificationService NotificationService { get; set; } = default!;

    public IEntity? SelectedEntity { get; set; }

    public void OpenAddEntityScreen()
    {
        _openScreen = new CreateEntityScreen();
        SelectedEntity = null;
        StateHasChanged();
    }

    public void CloseAddEntityScreen()
    {
        _openScreen = null;
    }

    protected override async Task OnInitializedAsync()
    {
        ApplicationState.AppStatusChanged += StateHasChanged;
        ApplicationState.AppStateChanged += () =>
        {
            if (SelectedEntity != null &&
                ApplicationState.ApplicationState.Entities.Any(e => e.Id == SelectedEntity.Id))
            {
                SelectedEntity = ApplicationState.ApplicationState.Entities.First(e => e.Id == SelectedEntity.Id);
            }
            else
            {
                SelectedEntity = ApplicationState.ApplicationState.Entities.FirstOrDefault();
            }

            StateHasChanged();
        };
        await NotificationService.StartupAsync();
    }

    private Task OnSelectEntity(IEntity entity)
    {
        SelectedEntity = entity;
        _openScreen = null;
        StateHasChanged();
        return Task.CompletedTask;
    }
}