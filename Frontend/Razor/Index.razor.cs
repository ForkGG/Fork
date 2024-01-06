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
    [Inject] private IApplicationConnectionService ApplicationConnection { get; set; }
    [Inject] private IApplicationStateManager ApplicationStateManager { get; set; }
    [Inject] private INotificationService NotificationService { get; set; }

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
        _applicationState.AppStatusChanged += StateHasChanged;
        _applicationState.AppStateChanged += () =>
        {
            if (SelectedEntity != null &&
                _applicationState.ApplicationState.Entities.Any(e => e.Id == SelectedEntity.Id))
            {
                SelectedEntity = _applicationState.ApplicationState.Entities.First(e => e.Id == SelectedEntity.Id);
            }
            else
            {
                SelectedEntity = _applicationState.ApplicationState.Entities.FirstOrDefault();
            }

            StateHasChanged();
        };
        await NotificationService.StartupAsync();
    }

    private async Task OnSelectEntity(IEntity entity)
    {
        SelectedEntity = entity;
        _openScreen = null;
        StateHasChanged();
    }
}