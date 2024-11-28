using System.Threading.Tasks;

namespace Fork.Adapters.Fork;

public interface IForkApiAdapter
{
    /// <summary>
    ///     Get the external IP of this system
    /// </summary>
    public Task<string> GetExternalIpAddress();
}