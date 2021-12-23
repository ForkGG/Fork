using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using ProjectAvery.Logic.Model.Web.Mojang;
using ProjectAveryCommon.Model.Application.Exceptions;

namespace ProjectAvery.Logic.Services.WebServices;

public class MojangApiAdapter : IMojangApiAdapter
{
    
    public async Task<string> UidForNameAsync(string name)
    {
        string encodedName = Uri.EscapeDataString(name);
        dynamic response = await GetAsync<dynamic>($"https://api.mojang.com/users/profiles/minecraft/{encodedName}");
        return response?.id;
    }

    public async Task<PlayerProfile> ProfileForUidAsync(string uid)
    {
        string encodedUid = Uri.EscapeDataString(uid.Replace("-", ""));
        var response = await GetAsync<PlayerProfile>($"https://sessionserver.mojang.com/session/minecraft/profile/{encodedUid}");
        return response;
    }

    public async Task<T> GetAsync<T>(string path)
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