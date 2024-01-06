using ForkCommon.ExtensionMethods;
using ForkCommon.Model.Application;
using ForkFrontend.Logic.Services.HttpsClients;

namespace ForkFrontend.Logic.Services.Connections;

public class ApplicationConnectionService : AbstractConnectionService, IApplicationConnectionService
{
    public ApplicationConnectionService(ILogger<ApplicationConnectionService> logger, BackendClient client) : base(
        logger,
        client)
    {
    }

    /// <summary>
    ///     Get the main application state from the backend
    ///     This should only be called once in the best case and the get updated on events by the websocket
    /// </summary>
    public async Task<State> GetApplicationState()
    {
        _logger.LogDebug("Loading main state");
        // TODO make this generic
        var responseMessage = await _client.GetAsync("/v1/application/state");
        var message = await responseMessage.Content.ReadAsStringAsync();
        try
        {
            var result = message.FromJson<State>();
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError("Error during deserialization of app state: \n" + e);
            return new State();
        }
    }

    public async Task<string> GetIpAddress()
    {
        _logger.LogDebug("Getting servers external Ip address");
        var responseMessage = await _client.GetAsync("/v1/application/ip");
        return await responseMessage.Content.ReadAsStringAsync();
    }
}