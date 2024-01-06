namespace ForkCommon.Model.Privileges.Entity.WriteEntity.WriteWorldsTab;

public class WriteWorldsTabPrivilege : IWriteEntityPrivilege
{
    public WriteWorldsTabPrivilege(ulong entityId)
    {
        EntityId = entityId;
    }

    public string Name => "WriteWorldsTab";
    public string TranslationPath => "privileges.entity.write.worlds";
    public ulong EntityId { get; set; }
}