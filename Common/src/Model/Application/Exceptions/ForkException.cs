using System;

namespace ForkCommon.Model.Application.Exceptions;

public class ForkException : Exception
{

    public ForkException()
    {
    }
    public ForkException(string message) : base(message)
    {
    }
}