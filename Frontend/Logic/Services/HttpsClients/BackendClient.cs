namespace ForkFrontend.Logic.Services.HttpsClients;

public class BackendClient
{
    public BackendClient(HttpClient client)
    {
        Client = client;
    }

    public HttpClient Client { get; }

    public string? GetBaseUrl()
    {
        return Client.BaseAddress?.ToString();
    }
}