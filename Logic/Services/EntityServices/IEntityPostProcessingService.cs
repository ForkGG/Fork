using System.Threading.Tasks;
using ProjectAveryCommon.Model.Entity.Pocos;

namespace ProjectAvery.Logic.Services.EntityServices;

public interface IEntityPostProcessingService
{
    public Task PostProcessEntity(IEntity entity);
}