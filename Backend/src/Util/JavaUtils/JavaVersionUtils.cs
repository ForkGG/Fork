using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using ForkCommon.Model.Entity.Pocos;

namespace Fork.Util.JavaUtils;

public class JavaVersionUtils
{
    private const string BIT_PATTERN = "64-Bit";
    private static readonly Regex VersionRegex = new(".* version \"([0-9._]*)\"");

    public static JavaVersion? GetInstalledJavaVersion(string? javaPath)
    {
        if (string.IsNullOrEmpty(javaPath))
        {
            //TODO get default path from settings
            //javaPath = AppSettingsSerializer.Instance.AppSettings.DefaultJavaPath;
            javaPath = "java";
        }

        return CheckForPathJava(javaPath);
    }

    private static JavaVersion? CheckForPathJava(string javaPath)
    {
        try
        {
            ProcessStartInfo procStartInfo = new(javaPath, "-version ");

            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.RedirectStandardError = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;
            Process proc = new() { StartInfo = procStartInfo };
            proc.Start();
            return InterpretJavaVersionOutput(proc.StandardError.ReadToEnd());
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static JavaVersion? InterpretJavaVersionOutput(string output)
    {
        Match versionMatch = VersionRegex.Match(output);
        if (versionMatch.Success)
        {
            JavaVersion result = new() { Version = versionMatch.Groups[1].Value };
            if (TryParseJavaVersion(result.Version.Split(".")[0], out int computedVersion))
            {
                if (computedVersion == 1)
                {
                    TryParseJavaVersion(result.Version.Split(".")[1], out computedVersion);
                }

                result.VersionComputed = computedVersion;
            }

            if (output.Contains(BIT_PATTERN))
            {
                result.Is64Bit = true;
            }

            return result;
        }

        return null;
    }

    private static bool TryParseJavaVersion(string versionString, out int version)
    {
        return int.TryParse(versionString, out version) || int.TryParse(versionString.Split(".")[1], out version);
    }
}
