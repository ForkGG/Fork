namespace ForkFrontend.Model;

public class UiMessage
{
    public UiMessage(string message, UiMessageType type)
    {
        Message = message;
        Type = type;
    }

    public string Message { get; set; }
    public UiMessageType Type { get; set; }
}