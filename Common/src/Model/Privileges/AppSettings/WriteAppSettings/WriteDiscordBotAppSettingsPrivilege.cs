namespace ForkCommon.Model.Privileges.AppSettings.WriteAppSettings;

public class WriteDiscordBotAppSettingsPrivilege : IWriteAppSettingsPrivilege
{
    public string Name => "WriteAppSettingsDiscordBot";
    public string TranslationPath => "privileges.appSettings.writeAppSettingsDiscordBot";
}