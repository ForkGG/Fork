using ForkFrontend.Logic.Services.HttpsClients;
using Newtonsoft.Json;

namespace ForkFrontend.Logic.Services.Translation;

public class TranslationService
{
    private readonly string _language;
    private readonly ILogger<TranslationService> _logger;
    private dynamic? _translationJson;

    public TranslationService(ILogger<TranslationService> logger, LocalClient client)
    {
        _logger = logger;
        //TODO Read language from settings
        _language = "DEFAULT";
        _translationJson = null;
        Task.Run(async () =>
        {
            try
            {
                _translationJson =
                    JsonConvert.DeserializeObject(await client.Client.GetStringAsync("resources/translation.json"));
            }
            catch (Exception e)
            {
                _logger.LogError($"Error while reading translation file: {e.Message}\n{e.StackTrace}");
                throw;
            }
        });
    }

    public async Task<string> Translate(string variable)
    {
        int retries = 0;
        while (_translationJson == null)
        {
            if (retries > 50)
            {
                _logger.LogError("Error while waiting for translation file to be loaded!");
                throw new TimeoutException("Error while waiting for translation file to be loaded!");
            }

            retries++;
            await Task.Delay(50);
        }

        try
        {
            dynamic prop = _translationJson;
            string[] path = variable.Split(".");
            foreach (string s in path) prop = prop[s];
            return prop[_language];
        }
        catch (Exception e)
        {
            _logger.LogError($"Property missing in translation file: {variable}");
            return variable;
        }
    }
}