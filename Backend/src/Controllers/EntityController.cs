﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fork.Logic.Managers;
using Fork.Logic.Services.EntityServices;
using ForkCommon.Model.Entity.Pocos;
using ForkCommon.Model.Entity.Transient.Console;
using ForkCommon.Model.Entity.Transient.Console.Commands;
using ForkCommon.Model.Payloads.Entity;
using ForkCommon.Model.Privileges;
using ForkCommon.Model.Privileges.Application;
using ForkCommon.Model.Privileges.Entity.ReadEntity.ReadConsoleTab;
using ForkCommon.Model.Privileges.Entity.WriteEntity.WriteConsoleTab;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Fork.Controllers;

/// <summary>
///     Controller for requests that affect a single entity
///     i.e: start/stop server, change server settings,...
/// </summary>
public class EntityController : AbstractRestController
{
    private readonly CommandService _commandService;
    private readonly EntityManager _entityManager;
    private readonly EntityService _entityService;
    private readonly ServerService _serverService;

    public EntityController(ILogger<EntityController> logger, EntityManager entityManager,
        EntityService entityService, ServerService serverService, CommandService commandService) : base(logger)
    {
        _entityManager = entityManager;
        _entityService = entityService;
        _serverService = serverService;
        _commandService = commandService;
    }

    [HttpPost("createServer")]
    [Privileges(typeof(CreateEntityPrivilege))]
    public async Task<ulong> CreateServer([FromBody] CreateServerPayload abstractPayload)
    {
        //TODO CKE validation
        ulong result = await _serverService.CreateServerAsync(abstractPayload.ServerName, abstractPayload.ServerVersion,
            abstractPayload.VanillaSettings,
            abstractPayload.JavaSettings, abstractPayload.WorldPath);
        if (result != 0)
        {
            await _entityService.UpdateEntityListAsync();
        }

        return result;
    }

    [HttpPost("{entityId}/delete")]
    [Privileges(typeof(DeleteEntityPrivilege))]
    public async Task<StatusCodeResult> DeleteEntity([FromRoute] ulong entityId)
    {
        IEntity? entity = await _entityManager.EntityById(entityId);
        if (entity == null)
        {
            return BadRequest();
        }

        await _entityService.DeleteEntityAsync(entity);
        await _entityService.UpdateEntityListAsync();
        return Ok();
    }

    [HttpPost("{entityId}/start")]
    [Privileges(typeof(WriteConsoleTabPrivilege))]
    public async Task<StatusCodeResult> StartEntity([FromRoute] ulong entityId)
    {
        IEntity? entity = await _entityManager.EntityById(entityId);
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
        IEntity? entity = await _entityManager.EntityById(entityId);
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
        IEntity? entity = await _entityManager.EntityById(entityId);
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

        IEntity? entity = await _entityManager.EntityById(entityId);
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
        IEntity? entity = await _entityManager.EntityById(entityId);
        if (entity == null)
        {
            return new List<ConsoleMessage>();
        }

        int amountOfMessages = Math.Min(entity.ConsoleMessages.Count, 1000);
        return entity.ConsoleMessages.GetRange(entity.ConsoleMessages.Count - amountOfMessages, amountOfMessages);
    }

    [HttpGet("{entityId}/commands")]
    [Privileges(typeof(ReadConsoleConsoleTabPrivilege))]
    public async Task<Command?> Commands([FromRoute] ulong entityId)
    {
        IEntity? entity = await _entityManager.EntityById(entityId);
        if (entity == null)
        {
            return null;
        }

        return await _commandService.GetCommandTreeForEntity(entity);
    }
}