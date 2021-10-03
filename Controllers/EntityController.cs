using Microsoft.Extensions.Logging;

namespace ProjectAvery.Controllers
{
    /// <summary>
    /// Controller for requests that affect a single entity
    /// i.e: start/stop server, change server settings,...
    /// </summary>
    public class EntityController : AbstractRestController
    {
        public EntityController(ILogger<EntityController> logger) : base(logger)
        {
        }
    }
}