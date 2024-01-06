namespace ForkCommon.Model.Privileges.Entity.WriteEntity.WriteSettingsTab;

public class WriteGeneralSettingsTabPrivilege : IWriteSettingsTabPrivilege
{
    public WriteGeneralSettingsTabPrivilege(ulong entityId)
    {
        EntityId = entityId;
    }

    public string Name => "WriteSettingsTabGeneral";
    public string TranslationPath => "privileges.entity.write.settingsTab.general";
    public ulong EntityId { get; set; }
}