namespace ForkCommon.Model.Privileges.Entity.ReadEntity.ReadConsoleTab;

public class ReadWhitelistConsoleTabPrivilege : IReadConsoleTabPrivilege
{
    public string Name => "ReadConsoleTabBanlist";
    public string TranslationPath => "privileges.entity.read.consoleTab.whitelist";
    public ulong EntityId { get; set; }

    public ReadWhitelistConsoleTabPrivilege(ulong entityId)
    {
        EntityId = entityId;
    }
}