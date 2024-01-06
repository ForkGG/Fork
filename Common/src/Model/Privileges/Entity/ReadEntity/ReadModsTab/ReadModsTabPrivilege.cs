namespace ForkCommon.Model.Privileges.Entity.ReadEntity.ReadModsTab;

public class ReadModsTabPrivilege : IReadEntityPrivilege
{
    public ReadModsTabPrivilege(ulong entityId)
    {
        EntityId = entityId;
    }

    public string Name => "ReadModsTab";
    public string TranslationPath => "privileges.entity.read.modsTab";
    public ulong EntityId { get; set; }
}