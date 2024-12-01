using System.Threading.Tasks;
using ForkCommon.Model.Entity.Enums;
using ForkCommon.Model.Entity.Pocos;

namespace Fork.Logic.Services.EntityServices;

public interface IEntityService
{
    public Task StartEntityAsync(IEntity entity);
    public Task DeleteEntityAsync(IEntity entity);
    public Task StopEntityAsync(IEntity entity);
    public Task RestartEntityAsync(IEntity entity);

    public Task ChangeEntityStatusAsync(IEntity entity, EntityStatus newStatus);
    public Task UpdateEntityListAsync();
}