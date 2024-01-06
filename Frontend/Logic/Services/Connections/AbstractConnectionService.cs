using System.Net;
using System.Net.Http.Headers;
using System.Text;
using ForkCommon.ExtensionMethods;
using ForkCommon.Model.Application.Exceptions;
using ForkCommon.Model.Payloads;
using ForkFrontend.Logic.Services.HttpsClients;

namespace ForkFrontend.Logic.Services.Connections;

public abstract class AbstractConnectionService
{
    protected const string ApiVersion = "v1";

    protected readonly HttpClient _client;
    protected readonly ILogger<AbstractConnectionService> _logger;

    protected AbstractConnectionService(ILogger<AbstractConnectionService> logger, BackendClient client)
    {
        _logger = logger;
        //TODO CKE Set token in all http requests
        _client = client.Client;
        _client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("asdf");
    }

    protected async Task<T?> GetFromJsonAsync<T>(string url)
    {
        HttpResponseMessage response = await _client.GetAsync(url);
        HandleServerError(response);
        string content = await response.Content.ReadAsStringAsync();
        return content.FromJson<T>();
    }

    protected Task<HttpResponseMessage> PostAsJsonAsync(string url, AbstractPayload? payload = null)
    {
        return PostAsyncInternal(url, payload);
    }

    protected async Task<T?> PostAsJsonAsync<T>(string url, AbstractPayload? payload)
    {
        HttpResponseMessage response = await PostAsyncInternal(url, payload);
        string content = await response.Content.ReadAsStringAsync();
        return content.FromJson<T>();
    }

    private async Task<HttpResponseMessage> PostAsyncInternal(string url, AbstractPayload? payload = null)
    {
        HttpContent? content =
            payload != null ? new StringContent(payload.ToJson(), Encoding.UTF8, "application/json") : null;
        HttpResponseMessage response =
            await _client.PostAsync(url, content);
        HandleServerError(response);
        return response;
    }

    protected async Task<HttpResponseMessage> PostTextAsync(string url, string? payload = null)
    {
        HttpContent? content =
            payload != null ? new StringContent(payload) : null;
        HttpResponseMessage response =
            await _client.PostAsync(url, content);
        HandleServerError(response);
        return response;
    }

    private void HandleServerError(HttpResponseMessage response)
    {
        if ((int)response.StatusCode >= 500)
        {
            throw new ForkException("Internal Server Error occured");
        }
    }
}