using ForkCommon.Model.Entity.Pocos;
using ForkCommon.Model.Entity.Pocos.ServerSettings;

namespace ForkCommon.Model.Payloads.Entity;

public class CreateServerPayload : AbstractPayload
{
    public string ServerName { get; set; } = "Server";
    public ServerVersion ServerVersion { get; set; } = new();
    public VanillaSettings VanillaSettings { get; set; } = new("world");
    public JavaSettings JavaSettings { get; set; } = new();
    public string? WorldPath { get; set; } = null;
}