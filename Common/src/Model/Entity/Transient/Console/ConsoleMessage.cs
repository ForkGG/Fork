using ForkCommon.Model.Entity.Enums.Console;

namespace ForkCommon.Model.Entity.Transient.Console;

public class ConsoleMessage
{
    public ConsoleMessage(string message, ConsoleMessageType type)
    {
        Message = message;
        MessageType = type;
    }

    public string Message { get; set; }
    public ConsoleMessageType MessageType { get; set; }
}