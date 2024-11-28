using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ForkCommon.Model.Application;

/// <summary>
///     A class containing all the App specific settings.
///     NOTE: All properties must have a public setter for database loading
/// </summary>
public class AppSettings : IEnumerable<SettingsKeyValue>
{
    public string? EntityPath { get; set; }
    public int MaxConsoleLines { get; set; } = 1000;
    public int MaxConsoleLinesPerSecond { get; set; } = 10;
    public string DefaultJavaPath { get; set; } = "java.exe";
    public bool EnableDiscordBot { get; set; } = false;
    public string? DiscordBotToken { get; set; }
    public bool UseBetaVersions { get; set; } = false;
    public bool ConsoleThrottling { get; set; } = true;
    public bool RichPresence { get; set; } = true;
    public bool SendTelemetry { get; set; } = false;


    public IEnumerator<SettingsKeyValue> GetEnumerator()
    {
        foreach (PropertyInfo info in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            yield return new SettingsKeyValue { Key = info.Name, Value = info.GetValue(this)?.ToString() };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}