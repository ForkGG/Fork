using System.Threading.Tasks;
using ForkCommon.Model.Entity.Pocos;

namespace Fork.Logic.Services.EntityServices;

public interface IEntityPostProcessingService
{
    public Task PostProcessEntity(IEntity? entity);
}