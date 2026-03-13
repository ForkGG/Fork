using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fork.Logic.Services.EntityServices;
using ForkCommon.ExtensionMethods;
using ForkCommon.Model.Commands;
using ForkCommon.Model.Commands.Entity;
using ForkCommon.Model.Privileges;
using ForkCommon.Model.Privileges.Entity.WriteEntity.WriteSettingsTab;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fork.Logic.Commands;

public class CommandCenter
{
    private readonly ILogger<CommandCenter> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public CommandCenter(ILogger<CommandCenter> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async Task HandleCommandAsync(string json, IReadOnlySet<IPrivilege> privileges)
    {
        AbstractCommand? command = json.FromJson<AbstractCommand>();
        if (command == null)
        {
            _logger.LogWarning("Received WebSocket command that could not be deserialized: {Json}", json);
            return;
        }

        using IServiceScope scope = _scopeFactory.CreateScope();
        EntitySettingsService settingsService =
            scope.ServiceProvider.GetRequiredService<EntitySettingsService>();

        switch (command)
        {
            case SaveVanillaSettingsCommand cmd:
                if (!privileges.Any(p => p is AdminPrivilege ||
                                         typeof(WriteVanillaSettingsTabPrivilege).IsAssignableFrom(p.GetType())))
                {
                    _logger.LogWarning("Client attempted SaveVanillaSettingsCommand without required privilege");
                    return;
                }

                await settingsService.UpdateVanillaSettingsAsync(cmd.EntityId, cmd.Settings);
                break;

            default:
                _logger.LogWarning("Received unknown command type: {Type}", command.GetType().Name);
                break;
        }
    }
}
