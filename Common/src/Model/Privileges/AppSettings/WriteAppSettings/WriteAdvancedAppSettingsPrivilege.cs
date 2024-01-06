namespace ForkCommon.Model.Privileges.AppSettings.WriteAppSettings;

public class WriteAdvancedAppSettingsPrivilege : IWriteAppSettingsPrivilege
{
    public string Name => "WriteAppSettingsAdvanced";
    public string TranslationPath => "privileges.appSettings.writeAppSettingsAdvanced";
}