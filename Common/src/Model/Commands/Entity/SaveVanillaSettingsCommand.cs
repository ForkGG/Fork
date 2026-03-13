using ForkCommon.Model.Entity.Pocos.Settings;

namespace ForkCommon.Model.Commands.Entity;

public class SaveVanillaSettingsCommand : AbstractCommand
{
    public ulong EntityId { get; set; }
    public VanillaSettings Settings { get; set; } = null!;
}
