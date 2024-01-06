namespace ForkCommon.Model.Privileges.AppSettings.ReadAppSettings;

public class ReadAdvancedAppSettingsPrivilege : IReadAppSettingsPrivilege
{
    public string Name => "ReadAppSettingsAdvanced";
    public string TranslationPath => "privileges.appSettings.readAppSettingsAdvanced";
}