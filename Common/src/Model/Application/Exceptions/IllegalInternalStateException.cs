namespace ForkCommon.Model.Application.Exceptions;

public class IllegalInternalStateException : ForkException
{
    public IllegalInternalStateException(string message) : base(message)
    {
    }
}