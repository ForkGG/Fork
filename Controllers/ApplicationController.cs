using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectAvery.Logic.Managers;
using ProjectAvery.Logic.Persistence;
using ProjectAvery.Logic.Services.AuthenticationServices;
using ProjectAvery.Logic.Services.StateServices;
using ProjectAveryCommon.Model.Application;
using ProjectAveryCommon.Model.Privileges;

namespace ProjectAvery.Controllers
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