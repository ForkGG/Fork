namespace ForkCommon.Model.Privileges.Entity.WriteEntity.WriteSettingsTab;

public class WriteVersionSpecificSettingsTabPrivilege : IWriteSettingsTabPrivilege
{
    public WriteVersionSpecificSettingsTabPrivilege(ulong entityId)
    {
        EntityId = entityId;
    }

    public string Name => "WriteSettingsTabVersionSpecific";
    public string TranslationPath => "privileges.entity.write.settingsTab.versionSpecific";
    public ulong EntityId { get; set; }
}