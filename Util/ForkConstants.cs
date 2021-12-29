using System;
using System.IO;

namespace Fork.Util;

public static class ForkConstants
{
    private static string applicationPath;

    public static string ApplicationPath
    {
        get
        {
            if (applicationPath == null)
            {
                DirectoryInfo directoryInfo = Directory.CreateDirectory(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Fork"));
                applicationPath = directoryInfo.FullName;
                Console.WriteLine("Data directory of Fork is: " + applicationPath);
            }

            return applicationPath;
        }
    }
}