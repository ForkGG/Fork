using System.Threading.Tasks;
using ProjectAveryCommon.Model.Entity.Pocos;

namespace ProjectAvery.Logic.Services.EntityServices;

public interface IEntityService
{
    public Task StartEntityAsync(IEntity entity);
    public Task StopEntityAsync(IEntity entity);
    public Task RestartEntityAsync(IEntity entity);
}