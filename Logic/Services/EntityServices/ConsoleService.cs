using System.IO;
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
    private readonly INotificationCenter _notificationCenter;

    public ConsoleService(INotificationCenter notificationCenter)
    {
        _notificationCenter = notificationCenter;
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

    public async Task BindProcessToConsole(IEntity entity, StreamReader stdOut, StreamReader errOut, IEntityService entityService)
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

                    //bool used to generate green success message in console
                    bool isSuccess = false;
                    if (entity is Server server)
                    {
                        if (line.Contains("For help, type \"help\""))
                        {
                            await entityService.ChangeEntityStatusAsync(entity, EntityStatus.Started);
                            isSuccess = true;
                        }
                        //TODO CKE handle Players
                        // serverViewModel.RoleInputHandler(line);
                    }

                    // TODO CKE once we got networks
                    /* if (entity is Network network)
                    {
                        if (waterfallStarted.Match(line).Success)
                        {
                            networkViewModel.CurrentStatus = ServerStatus.RUNNING;
                            isSuccess = true;
                        }
                    }*/

                    await WriteLine(entity, line, isSuccess ? ConsoleMessageType.Success : ConsoleMessageType.Default);
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
                        await entityService.ChangeEntityStatusAsync(entity, EntityStatus.Started);
                        isSuccess = true;
                    }

                    if (entity is Server server)
                    {
                        //TODO CKE handle Players
                        //serverViewModel.RoleInputHandler(line);
                    }

                    await WriteLine(entity, line, isSuccess ? ConsoleMessageType.Success : ConsoleMessageType.Default);
                }
            }
        }

        new Thread(HandleStdOut) { IsBackground = true }.Start();

        new Thread(HandleErrOut) { IsBackground = true }.Start();
    }
}