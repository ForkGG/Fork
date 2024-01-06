namespace ForkCommon.Model.Privileges.Entity.ReadEntity.ReadSettingsTab;

public class ReadVanillaSettingsTabPrivilege : IReadSettingsTabPrivilege
{
    public ReadVanillaSettingsTabPrivilege(ulong entityId)
    {
        EntityId = entityId;
    }

    public string Name => "ReadSettingsTabGeneral";
    public string TranslationPath => "privileges.entity.read.settingsTab.vanilla";
    public ulong EntityId { get; set; }
}