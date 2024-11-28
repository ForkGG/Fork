using System;
using System.IO;

namespace Fork.Util;

public static class ForkConstants
{
    private static string? _applicationPath;

    public static string ApplicationPath
    {
        get
        {
            if (_applicationPath == null)
            {
                DirectoryInfo directoryInfo = Directory.CreateDirectory(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Fork"));
                _applicationPath = directoryInfo.FullName;
                Console.WriteLine("Data directory of Fork is: " + _applicationPath);
            }

            return _applicationPath;
        }
    }
}