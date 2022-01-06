using System.Threading.Tasks;

namespace Fork.Adapters.Fork;

public interface IForkAPIAdapter
{
    /// <summary>
    /// Get the external IP of this system
    /// </summary>
    public Task<string> GetExternalIpAddress();
}