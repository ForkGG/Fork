namespace ForkCommon.Model.Privileges.Entity.WriteEntity.WriteModsTab;

public class WriteModsTabPrivilege : IWriteEntityPrivilege
{
    public string Name => "WriteModsTab";
    public string TranslationPath => "privileges.entity.write.mods";
    public ulong EntityId { get; set; }

    public WriteModsTabPrivilege(ulong entityId)
    {
        EntityId = entityId;
    }
}