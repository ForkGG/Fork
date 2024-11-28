using System;
using System.IO;
using System.Threading.Tasks;
using ForkCommon.Model.Entity.Enums;
using ForkCommon.Model.Entity.Enums.Console;
using ForkCommon.Model.Entity.Pocos;

namespace Fork.Logic.Services.EntityServices;

public interface IConsoleService
{
    public Task WriteLine(IEntity entity, string message, ConsoleMessageType type = ConsoleMessageType.Default);
    public Task WriteError(IEntity entity, string message);
    public Task WriteWarning(IEntity entity, string message);
    public Task WriteSuccess(IEntity entity, string message);


    public void BindProcessToConsole(IEntity entity, StreamReader stdOut, StreamReader errOut,
        Action<EntityStatus> entityStatusUpdateAction);
}