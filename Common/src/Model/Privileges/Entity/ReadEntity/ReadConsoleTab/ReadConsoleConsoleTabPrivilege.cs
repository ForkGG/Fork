namespace ForkCommon.Model.Privileges.Entity.ReadEntity.ReadConsoleTab;

public class ReadConsoleConsoleTabPrivilege : IReadConsoleTabPrivilege
{
    public string Name => "ReadConsoleTabConsole";
    public string TranslationPath => "privileges.entity.read.consoleTab.console";
    public ulong EntityId { get; set; }

    public ReadConsoleConsoleTabPrivilege(ulong entityId)
    {
        EntityId = entityId;
    }
}