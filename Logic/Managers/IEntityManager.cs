using System.Threading.Tasks;
using ProjectAveryCommon.Model.Entity.Pocos;

namespace ProjectAvery.Logic.Managers;

public interface IEntityManager
{
    public IEntity EntityById(ulong entityId);
}