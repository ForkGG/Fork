using JetBrains.Annotations;

namespace ForkCommon.Model.Application.Exceptions;

public static class Assert
{
    [ContractAnnotation("value:null => halt")]
    public static void NotNull(object? value, string? message = "Value cannot be null.")
    {
        if (value is null)
        {
            throw new ProgrammingErrorException(message);
        }
    }

    [ContractAnnotation("value:notnull => halt")]
    public static void Null(object? value, string? message = "Value must be null.")
    {
        if (value is not null)
        {
            throw new ProgrammingErrorException(message);
        }
    }

    [ContractAnnotation("value:false => halt")]
    public static void IsTrue(bool value, string? message = "Value must be true.")
    {
        if (!value)
        {
            throw new ProgrammingErrorException(message);
        }
    }
}