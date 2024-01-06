namespace ForkCommon.Model.Privileges.Entity.WriteEntity.WritePluginsTab;

public class WritePluginsTabPrivilege : IWriteEntityPrivilege
{
    public WritePluginsTabPrivilege(ulong entityId)
    {
        EntityId = entityId;
    }

    public string Name => "WritePluginsTab";
    public string TranslationPath => "privileges.entity.write.plugins";
    public ulong EntityId { get; set; }
}