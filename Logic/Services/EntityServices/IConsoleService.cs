using System;
using System.IO;
using System.Threading.Tasks;
using ProjectAveryCommon.Model.Entity.Enums;
using ProjectAveryCommon.Model.Entity.Enums.Console;
using ProjectAveryCommon.Model.Entity.Pocos;

namespace ProjectAvery.Logic.Services.EntityServices;

public interface IConsoleService
{
    public Task WriteLine(IEntity entity, string message, ConsoleMessageType type = ConsoleMessageType.Default);
    public Task WriteError(IEntity entity, string message);
    public Task WriteWarning(IEntity entity, string message);
    public Task WriteSuccess(IEntity entity, string message);


    public Task BindProcessToConsole(IEntity entity, StreamReader stdOut, StreamReader errOut, Action<EntityStatus> entityStatusUpdateAction);
}