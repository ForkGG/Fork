namespace ForkCommon.Model.Privileges.Entity.WriteEntity.WritePluginsTab;

public class WritePluginsTabPrivilege : IWriteEntityPrivilege
{
    public string Name => "WritePluginsTab";
    public string TranslationPath => "privileges.entity.write.plugins";
    public ulong EntityId { get; set; }

    public WritePluginsTabPrivilege(ulong entityId)
    {
        EntityId = entityId;
    }
}