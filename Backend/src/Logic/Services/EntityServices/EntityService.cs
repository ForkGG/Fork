using System.Collections.Generic;
using System.Threading.Tasks;
using Fork.Logic.Managers;
using Fork.Logic.Notification;
using ForkCommon.Model.Application.Exceptions;
using ForkCommon.Model.Entity.Enums;
using ForkCommon.Model.Entity.Pocos;
using ForkCommon.Model.Notifications.EntityNotifications;
using Microsoft.Extensions.Logging;

namespace Fork.Logic.Services.EntityServices;

public class EntityService : IEntityService
{
    private readonly IEntityManager _entityManager;
    private readonly ILogger<EntityService> _logger;
    private readonly INotificationCenter _notificationCenter;
    private readonly IServerService _serverService;

    public EntityService(ILogger<EntityService> logger, IServerService serverService,
        INotificationCenter notificationCenter, IEntityManager entityManager)
    {
        _logger = logger;
        _serverService = serverService;
        _notificationCenter = notificationCenter;
        _entityManager = entityManager;
    }

    public async Task StartEntityAsync(IEntity entity)
    {
        if (entity is Server server)
        {
            await _serverService.StartServerAsync(server);
        }
        else
        {
            throw new ForkException($"Can't start entity of type: {entity.GetType()}");
        }
    }

    public async Task DeleteEntityAsync(IEntity entity)
    {
        if (entity is Server server)
        {
            await _serverService.DeleteServerAsync(server);
        }
        else
        {
            throw new ForkException($"Can't delete entity of type: {entity.GetType()}");
        }
    }

    public async Task StopEntityAsync(IEntity entity)
    {
        if (entity is Server server)
        {
            await _serverService.StopServerAsync(server);
        }
        else
        {
            throw new ForkException($"Can't start entity of type: {entity.GetType()}");
        }
    }

    public async Task RestartEntityAsync(IEntity entity)
    {
        if (entity is Server server)
        {
            await _serverService.RestartServerAsync(server);
        }
        else
        {
            throw new ForkException($"Can't start entity of type: {entity.GetType()}");
        }
    }

    public async Task ChangeEntityStatusAsync(IEntity entity, EntityStatus newStatus)
    {
        if (entity is Server server)
        {
            await _serverService.ChangeServerStatusAsync(server, newStatus);
        }
        else
        {
            throw new ForkException($"Can't start entity of type: {entity.GetType()}");
        }
    }

    public async Task UpdateEntityListAsync()
    {
        List<IEntity> entities = await _entityManager.ListAllEntities();
        EntityListUpdatedNotification notification = new(entities);
        await _notificationCenter.BroadcastNotification(notification);
    }
}