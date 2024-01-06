namespace ForkCommon.Model.Privileges.Entity.WriteEntity.WriteModsTab;

public class WriteModsTabPrivilege : IWriteEntityPrivilege
{
    public WriteModsTabPrivilege(ulong entityId)
    {
        EntityId = entityId;
    }

    public string Name => "WriteModsTab";
    public string TranslationPath => "privileges.entity.write.mods";
    public ulong EntityId { get; set; }
}