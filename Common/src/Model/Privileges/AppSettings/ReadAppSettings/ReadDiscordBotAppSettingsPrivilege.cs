namespace ForkCommon.Model.Privileges.AppSettings.ReadAppSettings;

public class ReadDiscordBotAppSettingsPrivilege : IReadAppSettingsPrivilege
{
    public string Name => "ReadAppSettingsDiscordBot";
    public string TranslationPath => "privileges.appSettings.readAppSettingsDiscordBot";
}