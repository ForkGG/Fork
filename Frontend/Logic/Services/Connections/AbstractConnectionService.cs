using System.Net.Http.Headers;
using System.Text;
using ForkCommon.ExtensionMethods;
using ForkCommon.Model.Application.Exceptions;
using ForkCommon.Model.Payloads;
using ForkFrontend.Logic.Services.HttpsClients;
using ForkFrontend.Logic.Services.Managers;
using ForkFrontend.Model;
using ForkFrontend.Model.Enums;

namespace ForkFrontend.Logic.Services.Connections;

public abstract class AbstractConnectionService
{
    protected const string ApiVersion = "v1";

    protected readonly HttpClient Client;
    protected readonly ILogger<AbstractConnectionService> Logger;
    protected readonly ToastManager ToastManager;

    protected AbstractConnectionService(ILogger<AbstractConnectionService> logger, BackendClient client,
        ToastManager toastManager)
    {
        Logger = logger;
        //TODO CKE Set token in all http requests
        Client = client.Client;
        Client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("asdf");
        ToastManager = toastManager;
    }

    protected async Task<T?> GetFromJsonAsync<T>(string url)
    {
        HttpResponseMessage response = await Client.GetAsync(url);
        await HandleServerError(response);
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
            await Client.PostAsync(url, content);
        await HandleServerError(response);
        return response;
    }

    protected async Task<HttpResponseMessage> PostTextAsync(string url, string? payload = null)
    {
        HttpContent? content =
            payload != null ? new StringContent(payload) : null;
        HttpResponseMessage response =
            await Client.PostAsync(url, content);
        await HandleServerError(response);
        return response;
    }

    private async Task HandleServerError(HttpResponseMessage response)
    {
        if ((int)response.StatusCode < 400)
        {
            return;
        }

        if (response.Content.Headers.ContentType?.MediaType != "application/json")
        {
            throw new ForkException("Internal Server Error occured, try restarting Fork");
        }

        string errorJson = await response.Content.ReadAsStringAsync();
        ForkException? errorObject = errorJson.FromJson<ForkException>();
        if (errorObject != null)
        {
            throw errorObject;
        }

        throw new ForkException("Internal Server Error occured, try restarting Fork");
    }

    protected async Task ShowSuccessToast(string message)
    {
        await ShowSuccessOrErrorToast(true, message, "");
    }

    protected async Task ShowSuccessOrErrorToast(bool success, string successMessage, string errorMessage)
    {
        Toast toast;
        if (success)
        {
            toast = new Toast(ToastLevel.Success, successMessage, TimeSpan.FromSeconds(3));
        }
        else
        {
            toast = new Toast(ToastLevel.Error, errorMessage, TimeSpan.FromSeconds(10));
        }

        await ToastManager.AddToast(toast);
    }
}