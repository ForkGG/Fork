using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Fork.Logic.Managers;
using Fork.Logic.Persistence;
using Fork.Logic.Services.AuthenticationServices;
using Fork.Logic.Services.StateServices;
using ForkCommon.Model.Application;
using ForkCommon.Model.Privileges;

namespace Fork.Controllers
{
    /// <summary>
    /// Controller for requests that affect the whole application
    /// i.e.: change app settings, create/delete server, etc.
    /// </summary>
    public class ApplicationController : AbstractRestController
    {
        private readonly IApplicationStateService _applicationState;
        private readonly IAuthenticationService _authentication;

        public ApplicationController(ILogger<ApplicationController> logger, IApplicationStateService applicationState, IAuthenticationService authentication) : base(logger)
        {
            _applicationState = applicationState;
            _authentication = authentication;
        }

        [HttpGet("state")]
        [Privileges(typeof(IPrivilege))]
        public async Task<State> State()
        {
            // TODO automatically call this with each request
            LogRequest();
            //TODO create caller object with permissions from header token in each request
            return await _applicationState.BuildAppState();
        }

        [HttpGet("privileges")]
        [Privileges(typeof(IPrivilege))]
        public IEnumerable<IPrivilege> Privileges()
        {
            return _authentication.Privileges;
        }
    }
}