namespace ForkCommon.Model.Privileges.Application;

public class CreateEntityPrivilege : IApplicationPrivilege
{
    public string Name => "CreateEntity";
    public string TranslationPath => "privileges.application.createEntity";
}