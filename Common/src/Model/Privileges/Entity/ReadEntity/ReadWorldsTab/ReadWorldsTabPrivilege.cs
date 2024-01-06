namespace ForkCommon.Model.Privileges.Entity.ReadEntity.ReadWorldsTab;

public class ReadWorldsTabPrivilege : IReadEntityPrivilege
{
    public ReadWorldsTabPrivilege(ulong entityId)
    {
        EntityId = entityId;
    }

    public string Name => "ReadWorldsTab";
    public string TranslationPath => "privileges.entity.read.worldsTab";
    public ulong EntityId { get; set; }
}