using System.Threading.Tasks;
using ForkCommon.Model.Entity.Enums;
using ForkCommon.Model.Entity.Pocos;
using ForkCommon.Model.Entity.Pocos.ServerSettings;

namespace Fork.Logic.Services.EntityServices;

public interface IServerService
{
    public Task StartServerAsync(Server entity);
    public Task DeleteServerAsync(Server entity);
    public Task StopServerAsync(Server entity);
    public Task RestartServerAsync(Server entity);

    public Task ChangeServerStatusAsync(Server server, EntityStatus newStatus);

    public Task<ulong> CreateServerAsync(string name, ServerVersion version, VanillaSettings settings,
        JavaSettings javaSettings, string worldPath);
}