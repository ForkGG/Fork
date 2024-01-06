namespace ForkCommon.Model.Privileges.AppSettings.ReadAppSettings;

public class ReadGeneralAppSettingsPrivilege : IReadAppSettingsPrivilege
{
    public string Name => "ReadAppSettingsGeneral";
    public string TranslationPath => "privileges.appSettings.readAppSettingsGeneral";
}