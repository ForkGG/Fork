namespace ForkCommon.Model.Privileges.Entity.ReadEntity.ReadSettingsTab;

public class ReadGeneralSettingsTabPrivilege : IReadSettingsTabPrivilege
{
    public string Name => "ReadSettingsTabGeneral";
    public string TranslationPath => "privileges.entity.read.settingsTab.general";
    public ulong EntityId { get; set; }

    public ReadGeneralSettingsTabPrivilege(ulong entityId)
    {
        EntityId = entityId;
    }
}