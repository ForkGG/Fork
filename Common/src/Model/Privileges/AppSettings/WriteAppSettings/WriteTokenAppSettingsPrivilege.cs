namespace ForkCommon.Model.Privileges.AppSettings.WriteAppSettings;

public class WriteTokenAppSettingsPrivilege : IWriteAppSettingsPrivilege
{
    public string Name => "WriteAppSettingsToken";
    public string TranslationPath => "privileges.appSettings.writeAppSettingsToken";
}