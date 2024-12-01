﻿using ForkCommon.Model.Entity.Pocos;
using ForkCommon.Model.Entity.Transient.Console;
using ForkCommon.Model.Entity.Transient.Console.Commands;
using ForkCommon.Model.Payloads.Entity;

namespace ForkFrontend.Logic.Services.Connections;

public interface IEntityConnectionService
{
    public Task<List<ConsoleMessage>> GetConsoleMessagesAsync(ulong entityId);
    public Task<bool> SubmitConsoleInAsync(string message, ulong entityId);
    public Task<ulong> CreateServerAsync(CreateServerPayload payload);
    public Task<bool> DeleteEntityAsync(IEntity entity);
    public Task<bool> StartEntityAsync(ulong entityId);
    public Task<bool> StopEntityAsync(ulong entityId);
    public Task<bool> RestartEntityAsync(ulong entityId);
    public Task<Command?> GetCommandsAsync(ulong entityId);
}