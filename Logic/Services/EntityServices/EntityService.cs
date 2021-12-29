using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ForkCommon.Model.Application.Exceptions;
using ForkCommon.Model.Entity.Enums;
using ForkCommon.Model.Entity.Pocos;

namespace Fork.Logic.Services.EntityServices;

public class EntityService : IEntityService
{
    private readonly ILogger<EntityService> _logger;
    private readonly IServerService _serverService;

    public EntityService(ILogger<EntityService> logger, IServerService serverService)
    {
        _logger = logger;
        _serverService = serverService;
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
}