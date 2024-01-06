using ForkCommon.Model.Entity.Transient.Console;
using ForkCommon.Model.Payloads.Entity;
using ForkFrontend.Logic.Services.HttpsClients;

namespace ForkFrontend.Logic.Services.Connections;

public class EntityConnectionService : AbstractConnectionService, IEntityConnectionService
{
    private const string URL_BASE = $"/{ApiVersion}/entity";

    public EntityConnectionService(ILogger<EntityConnectionService> logger, BackendClient client) : base(logger, client)
    {
    }

    public async Task<List<ConsoleMessage>> GetConsoleMessagesAsync(ulong entityId)
    {
        try
        {
            List<ConsoleMessage>? result =
                await GetFromJsonAsync<List<ConsoleMessage>>($"{URL_BASE}/{entityId}/console");
            if (result != null)
            {
                return result;
            }
        }
        catch (Exception e)
        {
            //TODO properly display errors
            _logger.LogError(e, "Error while getting Console Output");
        }

        return new List<ConsoleMessage>();
    }

    public async Task<bool> SubmitConsoleInAsync(string message, ulong entityId)
    {
        try
        {
            HttpResponseMessage response =
                await PostTextAsync($"{URL_BASE}/{entityId}/consolein", message);
            return response.IsSuccessStatusCode;
        }
        catch (Exception e)
        {
            //TODO properly display errors
            _logger.LogError(e, "Error while sending Console Input");
            return false;
        }
    }

    public async Task<ulong> CreateServerAsync(CreateServerPayload createServerPayload)
    {
        HttpResponseMessage response = await PostAsJsonAsync($"{URL_BASE}/createserver", createServerPayload);
        return ulong.Parse(await response.Content.ReadAsStringAsync());
    }

    public async Task<bool> DeleteEntityAsync(ulong entityId)
    {
        HttpResponseMessage response = await PostAsJsonAsync($"{URL_BASE}/{entityId}/delete");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> StartEntityAsync(ulong entityId)
    {
        HttpResponseMessage response = await PostAsJsonAsync($"{URL_BASE}/{entityId}/start");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> StopEntityAsync(ulong entityId)
    {
        HttpResponseMessage response = await PostAsJsonAsync($"{URL_BASE}/{entityId}/stop");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RestartEntityAsync(ulong entityId)
    {
        HttpResponseMessage response = await PostAsJsonAsync($"{URL_BASE}/{entityId}/restart");
        return response.IsSuccessStatusCode;
    }
}