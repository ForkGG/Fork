using ForkCommon.Model.Application.Exceptions;
using ForkCommon.Model.Entity.Pocos;
using ForkCommon.Model.Notifications.EntityNotifications;
using ForkCommon.Model.Notifications.EntityNotifications.PlayerNotifications;
using ForkFrontend.Logic.Services.Notifications;
using ForkFrontend.Logic.Services.Notifications.NotificationHandlers.EntityNotificationHandlers;
using ForkFrontend.Logic.Services.Notifications.NotificationHandlers.EntityNotificationHandlers.
    PlayerNotificationHandlers;

namespace ForkFrontend.Logic.Services.Managers;

public class EntityStateManager
{
    private readonly NotificationService _notificationService;

    public EntityStateManager(IEntity entity, NotificationService notificationService)
    {
        Entity = entity;
        _notificationService = notificationService;

        // Create Handlers
        ConsoleAddNotificationHandler = new ConsoleAddNotificationHandler(Entity.Id, Entity.ConsoleMessages);
        EntityPerformanceNotificationHandler = new EntityPerformanceNotificationHandler(Entity.Id);
        EntityStatusChangedNotificationHandler = new EntityStatusChangedNotificationHandler(Entity);
        // Server specific handlers
        if (Entity is Server server)
        {
            UpdateBanlistPlayerNotificationHandler = new UpdateBanlistPlayerNotificationHandler(server);
            UpdatePlayerNotificationHandler = new UpdatePlayerNotificationHandler(server);
            UpdateWhitelistPlayerNotificationHandler = new UpdateWhitelistPlayerNotificationHandler(server);
        }
        else
        {
            // TODO CKE Implement for networks
            throw new ForkException("Not implemented yet!");
        }

        RegisterHandlers();
    }

    public IEntity Entity { get; private set; }

    // Notification Handlers
    public ConsoleAddNotificationHandler ConsoleAddNotificationHandler { get; }
    public EntityPerformanceNotificationHandler EntityPerformanceNotificationHandler { get; }

    public EntityStatusChangedNotificationHandler EntityStatusChangedNotificationHandler { get; }

    // Server specific notification handlers
    public UpdateBanlistPlayerNotificationHandler UpdateBanlistPlayerNotificationHandler { get; }
    public UpdatePlayerNotificationHandler UpdatePlayerNotificationHandler { get; }
    public UpdateWhitelistPlayerNotificationHandler UpdateWhitelistPlayerNotificationHandler { get; }

    /// <summary>
    ///     This replaces the instance of the entity!
    /// </summary>
    public void UpdateEntity(IEntity entity)
    {
        Entity = entity;

        // Update all NotificationHandlers
        ConsoleAddNotificationHandler.EntityId = entity.Id;
    }

    private void RegisterHandlers()
    {
        _notificationService.Register<ConsoleAddNotification>(ConsoleAddNotificationHandler.HandleNotification);
        _notificationService.Register<EntityPerformanceNotification>(EntityPerformanceNotificationHandler
            .HandleNotification);
        _notificationService.Register<EntityStatusChangedNotification>(EntityStatusChangedNotificationHandler
            .HandleNotification);
        if (Entity is Server)
        {
            _notificationService.Register<UpdateBanlistPlayerNotification>(UpdateBanlistPlayerNotificationHandler
                .HandleNotification);
            _notificationService.Register<UpdatePlayerNotification>(UpdatePlayerNotificationHandler.HandleNotification);
            _notificationService.Register<UpdateWhitelistPlayerNotification>(UpdateWhitelistPlayerNotificationHandler
                .HandleNotification);
        }
    }
}