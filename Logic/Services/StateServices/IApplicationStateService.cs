using System.Threading.Tasks;
using ForkCommon.Model.Application;

namespace Fork.Logic.Services.StateServices;

public interface IApplicationStateService
{
    public Task<State> BuildAppState();
}