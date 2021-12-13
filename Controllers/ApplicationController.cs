using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectAvery.Logic.Persistence;
using ProjectAvery.Logic.Services.Authentication;
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
        private readonly ApplicationDbContext _context;
        private readonly IAuthenticationService _authentication;

        public ApplicationController(ILogger<ApplicationController> logger, ApplicationDbContext context, IAuthenticationService authentication) : base(logger)
        {
            _context = context;
            _authentication = authentication;
        }

        [HttpGet("state")]
        [Privileges(typeof(IPrivilege))]
        public State State()
        {
            LogRequest();
            //TODO create caller object with permissions from header token in each request
            return _context.GenerateStateObject();
        }

        [HttpGet("privileges")]
        [Privileges(typeof(IPrivilege))]
        public IEnumerable<IPrivilege> Privileges()
        {
            return _authentication.Privileges;
        }
    }
}