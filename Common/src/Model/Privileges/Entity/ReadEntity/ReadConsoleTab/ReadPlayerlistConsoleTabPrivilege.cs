namespace ForkCommon.Model.Privileges.Entity.ReadEntity.ReadConsoleTab;

public class ReadPlayerlistConsoleTabPrivilege : IReadConsoleTabPrivilege
{
    public string Name => "ReadConsoleTabPlayerlist";
    public string TranslationPath => "privileges.entity.read.consoleTab.playerlist";
    public ulong EntityId { get; set; }

    public ReadPlayerlistConsoleTabPrivilege(ulong entityId)
    {
        EntityId = entityId;
    }
    
}