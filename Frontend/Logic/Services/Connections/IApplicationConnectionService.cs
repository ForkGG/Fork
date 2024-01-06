using ForkCommon.Model.Application;

namespace ForkFrontend.Logic.Services.Connections;

public interface IApplicationConnectionService
{
    public Task<State> GetApplicationState();
    public Task<string> GetIpAddress();
}