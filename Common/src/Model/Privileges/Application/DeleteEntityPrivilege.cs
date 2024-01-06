namespace ForkCommon.Model.Privileges.Application;

public class DeleteEntityPrivilege : IApplicationPrivilege
{
    public string Name => "DeleteEntity";
    public string TranslationPath => "privileges.application.deleteEntity";
}