using System.Threading.Tasks;
using Fork.Logic.Managers;
using ForkCommon.Model.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace Fork.Adapters.Fork;

public class ForkAPIAdapter : AbstractAdapter, IForkAPIAdapter
{
    private const string API_BASE = "https://api.fork.gg/";
    
    public ForkAPIAdapter(ILogger<ForkAPIAdapter> logger,IApplicationManager applicationManager) : base(logger, applicationManager){}
    
    public async Task<string> GetExternalIpAddress()
    {
        if (await IsApiAvailable())
        {
            return await GetBodyAsync(API_BASE + "ip");
        }

        // Fallback in case of API outage
        return await GetBodyAsync("https://ipv4.icanhazip.com/");
    }

    private async Task<bool> IsApiAvailable()
    {
        try
        {
            var response = await GetBodyAsync(API_BASE + "status");
            return response == "ONLINE";
        }
        catch (ExternalServiceException e)
        {
            Logger.LogWarning("Fork API is not operational");
            return false;
        }
    }
}