using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProjectAveryCommon.Model.Application.Exceptions;
using ProjectAveryCommon.Model.Entity.Enums;
using ProjectAveryCommon.Model.Entity.Pocos;

namespace ProjectAvery.Logic.Services.EntityServices;

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
            await _serverService.StartEntityAsync(server);
        }
        else
        {
            throw new ForkException($"Can't start entity of type: {entity.GetType()}");
        }
    }

    public Task StopEntityAsync(IEntity entity)
    {
        throw new System.NotImplementedException();
    }

    public Task RestartEntityAsync(IEntity entity)
    {
        throw new System.NotImplementedException();
    }
}