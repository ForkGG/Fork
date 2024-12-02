using ForkCommon.Model.Entity.Pocos;
using ForkFrontend.Logic.Services.Connections;
using ForkFrontend.Logic.Services.Managers;
using ForkFrontend.Logic.Services.Notifications;
using ForkFrontend.Razor.Components.Screens.CreateEntity;
using Microsoft.AspNetCore.Components;

namespace ForkFrontend.Razor;

// This Page controls the whole entity logic
public partial class Index : ComponentBase
{
    private readonly Dictionary<string, SubScreen> subScreens = new();

    // Screens can be shown instead of entities (add entity, Fork settings, ...)
    private SubScreen? _openScreen;

    [Inject] private ApplicationConnectionService ApplicationConnection { get; set; } = default!;
    [Inject] private ApplicationStateManager ApplicationStateManager { get; set; } = default!;
    [Inject] private NotificationService NotificationService { get; set; } = default!;

    public IEntity? SelectedEntity { get; set; }

    public void OpenAddEntityScreen()
    {
        _openScreen = subScreens[nameof(CreateEntityScreen)];
        SelectedEntity = null;
        StateHasChanged();
    }

    public void CloseAddEntityScreen()
    {
        _openScreen = null;
    }

    protected override async Task OnInitializedAsync()
    {
        InitSubScreens();
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

    private Task SelectEntityById(ulong id)
    {
        SelectedEntity = ApplicationState.ApplicationState.Entities.FirstOrDefault(e => e.Id == id);
        _openScreen = null;
        StateHasChanged();
        return Task.CompletedTask;
    }

    private void InitSubScreens()
    {
        subScreens.Add(nameof(CreateEntityScreen), new SubScreen
        {
            Type = typeof(CreateEntityScreen),
            Parameters = new Dictionary<string, object>
            {
                [nameof(CreateEntityScreen.SelectEntityById)] = SelectEntityById
            }
        });
    }

    private class SubScreen
    {
        public required Type Type { get; set; }
        public Dictionary<string, object>? Parameters { get; set; } = new();
    }
}