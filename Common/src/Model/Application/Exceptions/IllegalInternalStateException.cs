using System;
using Newtonsoft.Json;

namespace ForkCommon.Model.Application.Exceptions;

public class IllegalInternalStateException : ForkException
{
    public IllegalInternalStateException(string message) : base(message)
    {
    }


    [JsonConstructor]
    protected IllegalInternalStateException([JsonProperty("Message")] string message,
        [JsonProperty("InnerException")] Exception? e) :
        base(message, e)
    {
    }
}