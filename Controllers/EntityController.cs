using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectAvery.Logic.Managers;
using ProjectAvery.Logic.Services.EntityServices;
using ProjectAveryCommon.Model.Entity.Transient.Console;
using ProjectAveryCommon.Model.Payloads.Entity;
using ProjectAveryCommon.Model.Privileges;
using ProjectAveryCommon.Model.Privileges.Application;
using ProjectAveryCommon.Model.Privileges.Entity.ReadEntity.ReadConsoleTab;
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
        private readonly IEntityService _entityService;
        private readonly IServerService _serverService;

        public EntityController(ILogger<EntityController> logger, IEntityManager entityManager,
            IEntityService entityService, IServerService serverService) : base(logger)
        {
            _entityManager = entityManager;
            _entityService = entityService;
            _serverService = serverService;
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
            var entity = await _entityManager.EntityById(entityId);
            if (entity == null)
            {
                return BadRequest();
            }

            await _entityService.StartEntityAsync(entity);
            return Ok();
        }

        [HttpPost("{entityId}/stop")]
        [Privileges(typeof(WriteConsoleTabPrivilege))]
        public async Task<StatusCodeResult> StopEntity([FromRoute] ulong entityId)
        {
            var entity = await _entityManager.EntityById(entityId);
            if (entity == null)
            {
                return BadRequest();
            }

            await _entityService.StopEntityAsync(entity);
            return Ok();
        }

        [HttpPost("{entityId}/restart")]
        [Privileges(typeof(WriteConsoleTabPrivilege))]
        public async Task<StatusCodeResult> RestartEntity([FromRoute] ulong entityId)
        {
            var entity = await _entityManager.EntityById(entityId);
            if (entity == null)
            {
                return BadRequest();
            }

            await _entityService.RestartEntityAsync(entity);
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

            var entity = await _entityManager.EntityById(entityId);
            if (entity == null)
            {
                return BadRequest();
            }

            if (entity.ConsoleHandler != null)
            {
                entity.ConsoleHandler.Invoke(message);
                return Ok();
            }
            return StatusCode(400);
        }

        [HttpGet("{entityId}/console")]
        [Privileges(typeof(ReadConsoleConsoleTabPrivilege))]
        public async Task<List<ConsoleMessage>> Console([FromRoute] ulong entityId)
        {
            var entity = await _entityManager.EntityById(entityId);
            if (entity == null)
            {
                return new List<ConsoleMessage>();
            }

            int amountOfMessages = Math.Min(entity.ConsoleMessages.Count, 1000);
            return entity.ConsoleMessages.GetRange(entity.ConsoleMessages.Count - amountOfMessages, amountOfMessages);
        }
    }
}