using System.Threading.Tasks;
using Fork.Logic.Model.Web.Mojang;

namespace Fork.Adapters.Mojang;

public interface IMojangApiAdapter
{
    public Task<string> UidForNameAsync(string name);
    public Task<PlayerProfile> ProfileForUidAsync(string uid);
    public Task<string> Base64HeadFromTextureProperty(string encodedTextureProfile);
}