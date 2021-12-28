
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProjectAvery.Logic.Model.Web.Mojang;
using ProjectAvery.Util.ExtensionMethods;
using ProjectAveryCommon.Model.Application.Exceptions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ProjectAvery.Adapters.Mojang;

public class MojangApiAdapter : IMojangApiAdapter
{
    
    public async Task<string> UidForNameAsync(string name)
    {
        string encodedName = Uri.EscapeDataString(name);
        PlayerByName response = await GetAsync<PlayerByName>($"https://api.mojang.com/users/profiles/minecraft/{encodedName}");
        return response?.Id;
    }

    public async Task<PlayerProfile> ProfileForUidAsync(string uid)
    {
        string encodedUid = Uri.EscapeDataString(uid.Replace("-", ""));
        var response = await GetAsync<PlayerProfile>($"https://sessionserver.mojang.com/session/minecraft/profile/{encodedUid}");
        return response;
    }

    /// <summary>
    /// This method will retrieve the Texture of a player from its encoded profile and build a base64 representation of the Head
    /// </summary>
    public async Task<string> Base64HeadFromTextureProperty(string encodedTextureProfile)
    {
        await using var imageStream = new MemoryStream();
        
        if (encodedTextureProfile == null)
        {
            string defaultPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "DefaultHead.png");
            using var img = await Image.LoadAsync(defaultPath);
            await img.SaveAsPngAsync(imageStream);
        }
        else
        {
            var profileJson = Convert.FromBase64String(encodedTextureProfile);
            string profileJsonString = Encoding.UTF8.GetString(profileJson).Replace("\n","");
            PlayerTextureProfile profile = JsonConvert.DeserializeObject<PlayerTextureProfile>(profileJsonString);


            if (profile?.Textures?.Skin?.Url is not string url)
            {
                string defaultPath =
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "DefaultHead.png");
                using var img = await Image.LoadAsync(defaultPath);
                await img.SaveAsPngAsync(imageStream);
            }
            else
            {
                using var client = new HttpClient();
                var uri = new Uri(url);
                var response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
                while (response.StatusCode == (HttpStatusCode)429)
                {
                    await Task.Delay(5000);
                    response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
                }

                response.EnsureSuccessStatusCode();
                await using Stream respStream = await response.Content.ReadAsStreamAsync();
                using var img = await Image.LoadAsync(respStream);
                var headOverlay = img.Clone(x => x.Crop(new Rectangle(40, 8, 8, 8)));
                img.Mutate(x =>
                {
                    x.Crop(new Rectangle(8, 8, 8, 8))
                        .DrawImage(headOverlay, new Point(0, 0), 1f);
                    headOverlay.Dispose();
                });
                await img.SaveAsPngAsync(imageStream);
            }
        }

        imageStream.Position = 0;
        return imageStream.ConvertToBase64();
    }

    private async Task<T> GetAsync<T>(string path)
    {
        using HttpClient client = new HttpClient();
        var response = await client.GetAsync(path);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return default;
        }

        throw new MojangServiceException($"Mojang service returned status {response.StatusCode} on {path}");
    }
    
}