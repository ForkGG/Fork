namespace ForkCommon.Model.Privileges.AppSettings.ReadAppSettings;

public class ReadTokenAppSettingsPrivilege : IReadAppSettingsPrivilege
{
    public string Name => "ReadAppSettingsToken";
    public string TranslationPath => "privileges.appSettings.readAppSettingsToken";
}