﻿using System.Threading.Tasks;
using Fork.Logic.Managers;
using ForkCommon.Model.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace Fork.Adapters.Fork;

public class ForkApiAdapter : AbstractAdapter
{
    private const string API_BASE = "https://api.fork.gg/";

    public ForkApiAdapter(ILogger<ForkApiAdapter> logger, ApplicationManager applicationManager) : base(logger,
        applicationManager)
    {
    }

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
            string response = await GetBodyAsync(API_BASE + "status");
            return response == "ONLINE";
        }
        catch (ExternalServiceException e)
        {
            Logger.LogError(e, "Fork API is not operational");
            return false;
        }
    }
}