namespace ForkCommon.Model.Privileges.Entity.WriteEntity.WriteSettingsTab;

public class WriteVanillaSettingsTabPrivilege : IWriteSettingsTabPrivilege
{
    public string Name => "WriteSettingsTabVanilla";
    public string TranslationPath => "privileges.entity.write.settingsTab.vanilla";
    public ulong EntityId { get; set; }

    public WriteVanillaSettingsTabPrivilege(ulong entityId)
    {
        EntityId = entityId;
    }
}