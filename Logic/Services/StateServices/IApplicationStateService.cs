using System.Threading.Tasks;
using ProjectAveryCommon.Model.Application;

namespace ProjectAvery.Logic.Services.StateServices;

public interface IApplicationStateService
{
    public Task<State> BuildAppState();
}