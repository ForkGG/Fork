using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ProjectAvery.Logic.Notification;
using ProjectAveryCommon.Model.Entity.Enums;
using ProjectAveryCommon.Model.Entity.Enums.Console;
using ProjectAveryCommon.Model.Entity.Pocos;
using ProjectAveryCommon.Model.Entity.Transient.Console;
using ProjectAveryCommon.Model.Notifications.EntityNotifications;

namespace ProjectAvery.Logic.Services.EntityServices;

public class ConsoleService : IConsoleService
{
    private const string WATERFALL_STARTED_REGEX = @"\[([0-9]+:?)* INFO\]: Listening on /.*$";
    private const string WARN_REGEX = @"^\[(?:[0-9]{1,2}:){2}[0-9]{1,2}\] \[.*\/WARN\]:.*";
    private const string WERROR_REGEX = @"^\[(?:[0-9]{1,2}:){2}[0-9]{1,2}\] \[.*\/ERROR\]:.*";

    private readonly INotificationCenter _notificationCenter;
    private readonly IConsoleInterpreter _consoleInterpreter;

    public ConsoleService(INotificationCenter notificationCenter, IConsoleInterpreter consoleInterpreter)
    {
        _notificationCenter = notificationCenter;
        _consoleInterpreter = consoleInterpreter;
    }

    public async Task WriteLine(IEntity entity, string message, ConsoleMessageType type = ConsoleMessageType.Default)
    {
        ConsoleMessage consoleMessage = new ConsoleMessage(message, type);
        entity.ConsoleMessages.Add(consoleMessage);
        await _notificationCenter.BroadcastNotification(new ConsoleAddNotification
            { EntityId = entity.Id, NewConsoleMessage = consoleMessage });
    }

    public async Task WriteError(IEntity entity, string message)
    {
        await WriteLine(entity, message, ConsoleMessageType.Error);
    }

    public async Task WriteWarning(IEntity entity, string message)
    {
        await WriteLine(entity, message, ConsoleMessageType.Warning);
    }

    public async Task WriteSuccess(IEntity entity, string message)
    {
        await WriteLine(entity, message, ConsoleMessageType.Success);
    }

    public async Task BindProcessToConsole(IEntity entity, StreamReader stdOut, StreamReader errOut,
        Action<EntityStatus> entityStatusUpdateAction)
    {
        async void HandleStdOut()
        {
            while (!stdOut.EndOfStream)
            {
                string line = await stdOut.ReadLineAsync();
                if (line != null)
                {
                    if (line.Contains(@"WARN Advanced terminal features are not available in this environment"))
                    {
                        continue;
                    }

                    ConsoleMessageType messageType = ConsoleMessageType.Default;
                    if (Regex.IsMatch(line, WARN_REGEX))
                        messageType = ConsoleMessageType.Warning;
                    if (Regex.IsMatch(line, WERROR_REGEX))
                        messageType = ConsoleMessageType.Error;

                    if (line.Contains("For help, type \"help\"") || Regex.IsMatch(line, WATERFALL_STARTED_REGEX))
                    {
                        entityStatusUpdateAction.Invoke(EntityStatus.Started);
                        messageType = ConsoleMessageType.Success;
                    }
                    await _consoleInterpreter.InterpretLine(entity, line);

                    await WriteLine(entity, line, messageType);
                }
            }
        }

        async void HandleErrOut()
        {
            while (!errOut.EndOfStream)
            {
                string line = await errOut.ReadLineAsync();
                if (line != null)
                {
                    bool isSuccess = false;
                    // For early minecraft versions
                    if (line.Contains("For help, type \"help\""))
                    {
                        entityStatusUpdateAction.Invoke(EntityStatus.Started);
                        isSuccess = true;
                    }
                    await _consoleInterpreter.InterpretLine(entity, line);

                    await WriteLine(entity, line, isSuccess ? ConsoleMessageType.Success : ConsoleMessageType.Error);
                }
            }
        }

        new Thread(HandleStdOut) { IsBackground = true }.Start();

        new Thread(HandleErrOut) { IsBackground = true }.Start();
    }
}