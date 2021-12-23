using System.Threading.Tasks;
using ProjectAvery.Logic.Model.Web.Mojang;

namespace ProjectAvery.Logic.Services.WebServices;

public interface IMojangApiAdapter
{
    public Task<string> UidForNameAsync(string name);
    public Task<PlayerProfile> ProfileForUidAsync(string uid);
}