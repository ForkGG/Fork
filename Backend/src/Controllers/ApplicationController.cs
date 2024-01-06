using System.Collections.Generic;
using System.Threading.Tasks;
using Fork.Adapters.Fork;
using Fork.Logic.Services.AuthenticationServices;
using Fork.Logic.Services.StateServices;
using ForkCommon.Model.Application;
using ForkCommon.Model.Privileges;
using ForkCommon.Model.Privileges.Entity.ReadEntity.ReadConsoleTab;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Fork.Controllers;

/// <summary>
///     Controller for requests that affect the whole application
///     i.e.: change app settings, create/delete server, etc.
/// </summary>
public class ApplicationController : AbstractRestController
{
    private readonly IApplicationStateService _applicationState;
    private readonly IAuthenticationService _authentication;
    private readonly IForkAPIAdapter _forkApi;

    public ApplicationController(ILogger<ApplicationController> logger, IApplicationStateService applicationState,
        IAuthenticationService authentication, IForkAPIAdapter forkApi) : base(logger)
    {
        _applicationState = applicationState;
        _authentication = authentication;
        _forkApi = forkApi;
    }

    [HttpGet("state")]
    [Privileges(typeof(IPrivilege))]
    public async Task<State> State()
    {
        // TODO automatically call this with each request
        LogRequest();
        return await _applicationState.BuildAppState();
    }

    [HttpGet("privileges")]
    [Privileges(typeof(IPrivilege))]
    public IEnumerable<IPrivilege> Privileges()
    {
        return _authentication.Privileges;
    }

    /// <summary>
    ///     Get the external IP address for accessing the hosted servers
    /// </summary>
    [HttpGet("ip")]
    [Privileges(typeof(ReadConsoleConsoleTabPrivilege))]
    public async Task<string> Ip()
    {
        return await _forkApi.GetExternalIpAddress();
    }
}