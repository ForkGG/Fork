using System;
using Newtonsoft.Json;

namespace ForkCommon.Model.Application.Exceptions;

public class ProgrammingErrorException : IllegalInternalStateException
{
    private const string MESSAGE = "Unexpected internal error appeared. Please contact the Fork team.";

    public ProgrammingErrorException(string? additionalInfo = null) : base(MESSAGE + (additionalInfo != null
        ? $" (additional information: {additionalInfo})"
        : ""))
    {
    }

    [JsonConstructor]
    protected ProgrammingErrorException([JsonProperty("Message")] string message,
        [JsonProperty("InnerException")] Exception? e) : base(message, e)
    {
    }
}