using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectAvery.Logic.Managers;
using ProjectAvery.Logic.Notification;
using ProjectAvery.Logic.Services.EntityServices;
using ProjectAveryCommon.Model.Entity.Enums.Console;
using ProjectAveryCommon.Model.Entity.Transient.Console;
using ProjectAveryCommon.Model.Notifications.EntityNotifications;
using ProjectAveryCommon.Model.Payloads.Entity;
using ProjectAveryCommon.Model.Privileges;
using ProjectAveryCommon.Model.Privileges.Application;
using ProjectAveryCommon.Model.Privileges.Entity.WriteEntity.WriteConsoleTab;

namespace ProjectAvery.Controllers
{
    /// <summary>
    /// Controller for requests that affect a single entity
    /// i.e: start/stop server, change server settings,...
    /// </summary>
    public class EntityController : AbstractRestController
    {
        private readonly IEntityManager _entityManager;
        private readonly INotificationCenter _notificationCenter;
        private readonly IEntityService _entityService;
        private readonly IServerService _serverService;
        private readonly IEntityPostProcessingService _entityPostProcessing;

        public EntityController(ILogger<EntityController> logger, IEntityManager entityManager,
            INotificationCenter notificationCenter, IEntityService entityService, IServerService serverService,
            IEntityPostProcessingService entityPostProcessing) : base(logger)
        {
            _notificationCenter = notificationCenter;
            _entityManager = entityManager;
            _entityService = entityService;
            _serverService = serverService;
            _entityPostProcessing = entityPostProcessing;
        }

        [HttpPost("createServer")]
        [Privileges(typeof(CreateEntityPrivilege))]
        public async Task<ulong> CreateServer([FromBody] CreateServerPayload payload)
        {
            //TODO CKE validation
            return await _serverService.CreateServerAsync(payload.ServerName, payload.ServerVersion,
                payload.VanillaSettings,
                payload.JavaSettings, payload.WorldPath);
        }

        [HttpPost("{entityId}/start")]
        [Privileges(typeof(WriteConsoleTabPrivilege))]
        public async Task<StatusCodeResult> StartEntity([FromRoute] ulong entityId)
        {
            var entity = _entityManager.EntityById(entityId);
            if (entity == null)
            {
                return BadRequest();
            }

            await _entityPostProcessing.PostProcessEntity(entity);

            await _entityService.StartEntityAsync(entity);
            return Ok();
        }

        [Consumes("text/plain")]
        [HttpPost("{entityId}/consoleIn")]
        [Privileges(typeof(WriteConsoleTabPrivilege))]
        public async Task<StatusCodeResult> ConsoleIn([FromBody] string message, [FromRoute] ulong entityId)
        {
            if (string.IsNullOrEmpty(message) || message == "/")
            {
                return BadRequest();
            }

            var entity = _entityManager.EntityById(entityId);

            if (entity.ConsoleHandler != null)
            {
                entity.ConsoleHandler.Invoke(message);
                return Ok();
            }
            else
            {
                return StatusCode(400);
            }

            ConsoleAddNotification notification = new ConsoleAddNotification();
            notification.NewConsoleMessage = new ConsoleMessage(message, ConsoleMessageType.UserInput);
            notification.EntityId = entityId;
            await _notificationCenter.BroadcastNotification(notification);
            return Ok();
        }
    }
}