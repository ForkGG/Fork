using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectAvery.Logic.Persistence;
using ProjectAvery.Logic.Services.EntityServices;
using ProjectAveryCommon.Model.Entity.Pocos;

namespace ProjectAvery.Logic.Managers;

public class EntityManager : IEntityManager
{
    private static object _lockObj = new ();
    
    private readonly ILogger<EntityManager> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IEntityPostProcessingService _postProcessing;

    // This contains all loaded entities (Database cache and state keeping)
    private readonly Dictionary<ulong, IEntity> _entities;

    public EntityManager(ILogger<EntityManager> logger, IServiceScopeFactory scopeFactory, IEntityPostProcessingService postProcessing)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _postProcessing = postProcessing;
        _entities = new Dictionary<ulong, IEntity>();
    }

    /// <summary>
    /// Get an entity by ID. If the entity is not yet loaded from the DB do that here
    /// </summary>
    /// <param name="entityId"></param>
    /// <returns></returns>
    public IEntity EntityById(ulong entityId)
    {
        // We need a lock here to ensure that we don't get multiple instances of the same entity
        lock (_lockObj)
        {
            if (_entities.ContainsKey(entityId))
            {
                return _entities[entityId];
            }

            // TODO extend once we got networks
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var entity = context.ServerSet.Where(s => s.Id == entityId)
                .Include(s => s.AutomationTimes)
                .ThenInclude(a => a.Time)
                .Include(s => s.JavaSettings)
                .FirstOrDefault();
            
            if (entity != null)
            {
                _entities.Add(entityId, entity);
                
                // Post processing can be done without the lock
                _ = _postProcessing.PostProcessEntity(entity);
            }

            return entity;
        }
    }
}