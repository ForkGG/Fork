using System.Threading.Tasks;
using ProjectAveryCommon.Model.Entity.Enums;
using ProjectAveryCommon.Model.Entity.Pocos;
using ProjectAveryCommon.Model.Entity.Pocos.ServerSettings;

namespace ProjectAvery.Logic.Services.EntityServices;

public interface IServerService
{
    public Task StartServerAsync(Server entity);
    public Task StopServerAsync(Server entity);
    public Task RestartServerAsync(Server entity);
    
    public Task ChangeServerStatusAsync(Server server, EntityStatus newStatus);
    
    public Task<ulong> CreateServerAsync(string name, ServerVersion version, VanillaSettings settings,
        JavaSettings javaSettings, string worldPath);
}