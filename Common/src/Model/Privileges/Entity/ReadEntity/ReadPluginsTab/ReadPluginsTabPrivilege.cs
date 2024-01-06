namespace ForkCommon.Model.Privileges.Entity.ReadEntity.ReadPluginsTab;

public class ReadPluginsTabPrivilege : IReadEntityPrivilege
{
    public string Name => "ReadPluginsTab";
    public string TranslationPath => "privileges.entity.read.pluginsTab";
    public ulong EntityId { get; set; }

    public ReadPluginsTabPrivilege(ulong entityId)
    {
        EntityId = entityId;
    }
    
}