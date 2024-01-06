namespace ForkCommon.Model.Privileges.AppSettings.WriteAppSettings;

public class WriteGeneralAppSettingsPrivilege : IWriteAppSettingsPrivilege
{
    public string Name => "WriteAppSettingsGeneral";
    public string TranslationPath => "privileges.appSettings.writeAppSettingsGeneral";
}