using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Fork.Logic.Managers;
using Fork.Util.ExtensionMethods;
using ForkCommon.ExtensionMethods;
using ForkCommon.Model.Application.Exceptions;
using ForkCommon.Model.Entity.Pocos;
using ForkCommon.Model.Entity.Transient.Console.Commands;
using Microsoft.Extensions.Logging;

namespace Fork.Logic.Services.EntityServices;

public class CommandService
{
    private readonly ApplicationManager _application;
    private readonly Dictionary<ulong, Command> _commandsCache = new();
    private readonly ILogger<CommandService> _logger;

    public CommandService(ILogger<CommandService> logger, ApplicationManager application)
    {
        _logger = logger;
        _application = application;
    }

    public async Task<Command?> GetCommandTreeForEntity(IEntity entity)
    {
        if (_commandsCache.ContainsKey(entity.Id))
        {
            return _commandsCache[entity.Id];
        }

        _logger.LogDebug($"No commands cache for {entity.Id}. Generating...");
        Command? command = await ParseCommandsForEntity(entity);
        if (command != null)
        {
            _commandsCache.Add(entity.Id, command);
        }

        return command;
    }

    private async Task<Command?> ParseCommandsForEntity(IEntity entity)
    {
        string commandsJson = await GetCommandsJsonForEntity(entity);
        return commandsJson.FromJson<Command>();
    }

    private async Task<string> GetCommandsJsonForEntity(IEntity entity)
    {
        string commandsJsonFilePath =
            Path.Combine(entity.GetPath(_application), "generated", "reports", "commands.json");
        FileInfo commandsJsonFile = new(commandsJsonFilePath);

        if (!commandsJsonFile.Exists)
        {
            _logger.LogDebug($"No commands.json generated for Entity {entity.Id}. Generating...");
            await GenerateCommandsForEntity(entity);

            commandsJsonFile = new FileInfo(commandsJsonFilePath);
            if (!commandsJsonFile.Exists)
            {
                throw new ForkException("Failed to generate commands.json. Try again in a few minutes.");
            }
        }

        return await File.ReadAllTextAsync(commandsJsonFile.FullName);
    }

    private async Task GenerateCommandsForEntity(IEntity entity)
    {
        string serverDirectory = entity.GetPath(_application);

        Process process = new();
        ProcessStartInfo startInfo = new()
        {
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            FileName = entity.JavaSettings?.JavaPath ?? "java",
            WorkingDirectory = serverDirectory,
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true
        };

        if (entity.Version?.IsEqualOrGreaterThan(ServerVersion.Version1_18) == true)
        {
            startInfo.Arguments = "-DbundlerMainClass=net.minecraft.data.Main -jar server.jar --reports";
        }
        else if (entity.Version?.IsEqualOrGreaterThan(ServerVersion.Version1_13) == true)
        {
            startInfo.Arguments = "-cp server.jar net.minecraft.data.Main --reports";
        }
        else
        {
            throw new ForkException("Servers below version 1.13 do not support command generation.");
        }

        process.StartInfo = startInfo;
        process.Start();

        await process.WaitForExitAsync();
    }
}