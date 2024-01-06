namespace ForkCommon.Model.Privileges.Entity.ReadEntity.ReadConsoleTab;

public class ReadBanlistConsoleTabPrivilege : IReadConsoleTabPrivilege
{
    public string Name => "ReadConsoleTabBanlist";
    public string TranslationPath => "privileges.entity.read.consoleTab.banlist";
    public ulong EntityId { get; set; }

    public ReadBanlistConsoleTabPrivilege(ulong entityId)
    {
        EntityId = entityId;
    }
    
}