using System;

namespace ForkCommon.ExtensionMethods;

public static class DateTimeExtensions
{
    public static bool IsOlderThan(this DateTime dateTime, TimeSpan timeSpan)
    {
        return DateTime.Now - dateTime > timeSpan;
    }
}