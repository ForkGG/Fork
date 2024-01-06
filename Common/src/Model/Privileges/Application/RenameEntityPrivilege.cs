namespace ForkCommon.Model.Privileges.Application;

public class RenameEntityPrivilege : IApplicationPrivilege
{
    public string Name => "RenameEntity";
    public string TranslationPath => "privileges.application.renameEntity";
}