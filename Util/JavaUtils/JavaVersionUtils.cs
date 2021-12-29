using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using ForkCommon.Model.Entity.Pocos;

namespace Fork.Util.JavaUtils;

public class JavaVersionUtils
{
    private static Regex versionRegex = new Regex(".* version \"([0-9._]*)\"");
        private static string bitPattern = "64-Bit";

        public static JavaVersion GetInstalledJavaVersion(string javaPath)
        {
            if (string.IsNullOrEmpty(javaPath))
            {
                //TODO get default path from settings
                //javaPath = AppSettingsSerializer.Instance.AppSettings.DefaultJavaPath;
                javaPath = "java ";
            }
            return CheckForPathJava(javaPath);
        }
        
        private static JavaVersion CheckForPathJava(string javaPath)
        {
            try
            {
                ProcessStartInfo procStartInfo =
                    new ProcessStartInfo(javaPath, "-version ");

                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.RedirectStandardError = true;
                procStartInfo.UseShellExecute = false;
                procStartInfo.CreateNoWindow = true;
                Process proc = new Process {StartInfo = procStartInfo};
                proc.Start();
                return InterpretJavaVersionOutput(proc.StandardError.ReadToEnd());

            }
            catch (Exception)
            {
                return null;
            }
        }

        private static JavaVersion InterpretJavaVersionOutput(string output)
        {
            if (output == null)
            {
                return null;
            }

            Match versionMatch = versionRegex.Match(output);
            if (versionMatch.Success)
            {
                JavaVersion result = new JavaVersion{Version = versionMatch.Groups[1].Value};
                if (TryParseJavaVersion(result.Version.Split(".")[0], out var computedVersion))
                {
                    if (computedVersion == 1)
                    {
                        TryParseJavaVersion(result.Version.Split(".")[1], out computedVersion);
                    }
                    result.VersionComputed = computedVersion;
                }
                if (output.Contains(bitPattern))
                {
                    result.Is64Bit = true;
                }
                return result;
            }
            return null;
        }

        private static bool TryParseJavaVersion(string versionString, out int version)
        {
            return int.TryParse(versionString, out version) || int.TryParse(versionString?.Split(".")[1], out version);
        }
}