namespace ForkCommon.Model.Privileges.Entity.WriteEntity.WriteWorldsTab;

public class WriteWorldsTabPrivilege : IWriteEntityPrivilege
{
    public string Name => "WriteWorldsTab";
    public string TranslationPath => "privileges.entity.write.worlds";
    public ulong EntityId { get; set; }

    public WriteWorldsTabPrivilege(ulong entityId)
    {
        EntityId = entityId;
    }
}