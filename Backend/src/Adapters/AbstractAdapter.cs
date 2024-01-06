using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Fork.Logic.Managers;
using ForkCommon.Model.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Fork.Adapters;

public abstract class AbstractAdapter
{
    protected readonly ILogger Logger;
    protected readonly string UserAgent;
    
    public AbstractAdapter(ILogger logger, IApplicationManager applicationManager)
    {
        Logger = logger;
        UserAgent = applicationManager.UserAgent;
    }
    
    /// <summary>
    /// Makes a GET request to the given path and returns the deserialized body as <see cref="T"/>>
    /// </summary>
    protected async Task<T> GetAsync<T>(string path)
    {
        var body = await GetBodyAsync(path);
        return JsonConvert.DeserializeObject<T>(body);
    }

    // TODO CKE Add optional caching to requests
    /// <summary>
    /// Makes a GET request to the given path and returns the body as string
    /// </summary>
    protected async Task<string> GetBodyAsync(string path)
    {
        try
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", UserAgent);
            var stopwatch = Stopwatch.StartNew();
            var response = await client.GetAsync(path);
            stopwatch.Stop();
            if (!response.IsSuccessStatusCode)
            {
                Logger.LogWarning($"GET {path} -> {response.StatusCode}");
            }
            Logger.LogDebug($"GET {path} -> {response.StatusCode} ({stopwatch.ElapsedMilliseconds}ms)");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStringAsync();
            }

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return "";
            }

            throw new ExternalServiceException($"External service returned status {response.StatusCode} on {path}");
        }
        catch (Exception e)
        {
            throw new ExternalException(
                $"Calling an external service on {path} threw an exception:\n{e.Message}\n{e.StackTrace}");
        }
    }
}