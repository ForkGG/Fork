using ForkCommon.ExtensionMethods;
using ForkCommon.Model.Application;
using ForkCommon.Model.Application.Exceptions;
using ForkFrontend.Logic.Services.HttpsClients;
using ForkFrontend.Logic.Services.Managers;

namespace ForkFrontend.Logic.Services.Connections;

public class ApplicationConnectionService : AbstractConnectionService
{
    public ApplicationConnectionService(ILogger<ApplicationConnectionService> logger, BackendClient client,
        ToastManager toastManager) : base(logger, client, toastManager)
    {
    }

    /// <summary>
    ///     Get the main application state from the backend
    ///     This should only be called once in the best case and the get updated on events by the websocket
    /// </summary>
    public async Task<State> GetApplicationState()
    {
        Logger.LogDebug("Loading main state");
        // TODO make this generic
        HttpResponseMessage responseMessage = await Client.GetAsync("/v1/application/state");
        string message = await responseMessage.Content.ReadAsStringAsync();
        try
        {
            State? result = message.FromJson<State>();
            if (result == null)
            {
                throw new ForkException("Invalid response from server");
            }

            return result;
        }
        catch (Exception e)
        {
            throw new ForkException("Invalid response from server", e);
        }
    }

    public async Task<string> GetIpAddress()
    {
        Logger.LogDebug("Getting servers external Ip address");
        HttpResponseMessage responseMessage = await Client.GetAsync("/v1/application/ip");
        return await responseMessage.Content.ReadAsStringAsync();
    }
}