using System.Threading.Tasks;
using ProjectAveryCommon.Model.Entity.Pocos;
using ProjectAveryCommon.Model.Entity.Pocos.ServerSettings;

namespace ProjectAvery.Logic.Services.EntityServices;

public interface IServerService : IEntityService
{
    public Task<ulong> CreateServerAsync(string name, ServerVersion version, VanillaSettings settings,
        JavaSettings javaSettings, string worldPath);
}