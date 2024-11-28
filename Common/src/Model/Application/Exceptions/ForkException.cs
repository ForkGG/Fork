using System;

namespace ForkCommon.Model.Application.Exceptions;

public class ForkException : Exception
{
    public ForkException(string message) : base(message)
    {
    }

    public ForkException(Exception e) : base("Fork Exception", e)
    {
    }

    public ForkException(string message, Exception e) : base(message, e)
    {
    }
}