namespace ForkFrontend.Logic.Services.HttpsClients;

public class LocalClient
{
    public LocalClient(HttpClient client)
    {
        Client = client;
    }

    public HttpClient Client { get; }

    public string? GetBaseUrl()
    {
        return Client.BaseAddress?.ToString();
    }
}