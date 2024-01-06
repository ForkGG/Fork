using System;
using System.IO;

namespace Fork.Util.ExtensionMethods;

public static class StreamExtensions
{
    public static string ConvertToBase64(this Stream stream)
    {
        byte[] bytes;
        using (MemoryStream memoryStream = new MemoryStream())
        {
            stream.CopyTo(memoryStream);
            bytes = memoryStream.ToArray();
        }

        return Convert.ToBase64String(bytes);
    }
}