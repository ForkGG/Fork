using System;
using Newtonsoft.Json;

namespace ForkCommon.Model.Application.Exceptions;

public class ForkException : Exception
{
    public ForkException(string message) : base(message)
    {
    }

    public ForkException(Exception e) : base("Fork Exception", e)
    {
    }

    [JsonConstructor]
    public ForkException([JsonProperty("Message")] string message, [JsonProperty("InnerException")] Exception? e) :
        base(message, e)
    {
    }
}