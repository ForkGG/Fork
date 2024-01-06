namespace ForkCommon.Model.Privileges;

public class AdminPrivilege : IPrivilege
{
    public string Name => "ForkAdmin";
    public string TranslationPath => "privileges.admin";
}