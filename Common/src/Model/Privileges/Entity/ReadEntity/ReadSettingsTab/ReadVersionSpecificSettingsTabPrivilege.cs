namespace ForkCommon.Model.Privileges.Entity.ReadEntity.ReadSettingsTab;

public class ReadVersionSpecificSettingsTabPrivilege : IReadSettingsTabPrivilege
{
    public string Name => "ReadSettingsTabGeneral";
    public string TranslationPath => "privileges.entity.read.settingsTab.versionSpecific";
    public ulong EntityId { get; set; }

    public ReadVersionSpecificSettingsTabPrivilege(ulong entityId)
    {
        EntityId = entityId;
    }
}