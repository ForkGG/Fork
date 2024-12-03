using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
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

    public AbstractAdapter(ILogger logger, ApplicationManager applicationManager)
    {
        Logger = logger;
        UserAgent = applicationManager.UserAgent;
    }

    protected async Task<T> GetAsync<T>(string path) where T : class
    {
        return await GetAsync<T>(new Uri(path));
    }

    protected async Task<T> GetAsync<T>(Uri uri) where T : class
    {
        try
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", UserAgent);
            Stopwatch stopwatch = Stopwatch.StartNew();
            HttpResponseMessage response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
            stopwatch.Stop();
            if (!response.IsSuccessStatusCode)
            {
                Logger.LogWarning($"GET {uri} -> {response.StatusCode}");
            }

            Logger.LogDebug($"GET {uri} -> {response.StatusCode} ({stopwatch.ElapsedMilliseconds}ms)");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (typeof(T) == typeof(string))
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return result as T ?? throw new ProgrammingErrorException();
                }

                if (!typeof(T).IsPrimitive)
                {
                    JsonSerializer serializer = new();

                    using StreamReader sr = new(await response.Content.ReadAsStreamAsync());
                    await using JsonTextReader jsonTextReader = new(sr);
                    T? result = serializer.Deserialize<T>(jsonTextReader);

                    if (result == null)
                    {
                        if (Nullable.GetUnderlyingType(typeof(T)) != null)
                        {
                            // Null is allowed by the type!
                            return null!;
                        }

                        throw new ForkException(
                            "Failed to parse response from external service. Please report this to the Fork team.");
                    }

                    return result;
                }

                throw new ProgrammingErrorException("Response type is not supported!");
            }

            Logger.LogError($"External service returned status {response.StatusCode} on {uri}");
            throw new ExternalServiceException(
                "External service is not available at the moment. Please try again in a few minutes.");
        }
        catch (Exception e)
        {
            Logger.LogError(e, $"Calling an external service on {uri} threw an exception");
            throw new ExternalServiceException(
                "External service is not available at the moment. Please try again in a few minutes.");
        }
    }
}