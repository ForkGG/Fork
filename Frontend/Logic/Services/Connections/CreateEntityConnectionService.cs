using ForkCommon.Model.Entity.Enums;
using ForkCommon.Model.Entity.Pocos;
using ForkFrontend.Logic.Services.HttpsClients;
using ForkFrontend.Logic.Services.Managers;

namespace ForkFrontend.Logic.Services.Connections;

public class CreateEntityConnectionService : AbstractConnectionService
{
    private const string URL_BASE = $"/{ApiVersion}/createEntity";

    public CreateEntityConnectionService(ILogger<AbstractConnectionService> logger, BackendClient client,
        ToastManager toastManager) : base(logger, client, toastManager)
    {
    }

    public async Task<List<VersionType>?> GetAvailableVersionTypes()
    {
        return await GetFromJsonAsync<List<VersionType>>($"{URL_BASE}/types");
    }

    public async Task<List<ServerVersion>?> GetServerVersionsForType(VersionType versionType)
    {
        return await GetFromJsonAsync<List<ServerVersion>>($"{URL_BASE}/{versionType}/versions");
    }
}