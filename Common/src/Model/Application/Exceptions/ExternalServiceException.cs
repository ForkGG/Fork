namespace ForkCommon.Model.Application.Exceptions;

/// <summary>
///     Thrown when the external service returns an error
/// </summary>
public class ExternalServiceException : ForkException
{
    public ExternalServiceException(string message) : base(message)
    {
    }
}