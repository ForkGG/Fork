using System.Threading.Tasks;
using ProjectAvery.Logic.Model.Web.Mojang;

namespace ProjectAvery.Adapters.Mojang;

public interface IMojangApiAdapter
{
    public Task<string> UidForNameAsync(string name);
    public Task<PlayerProfile> ProfileForUidAsync(string uid);
    public Task<string> Base64HeadFromTextureProperty(string encodedTextureProfile);
}