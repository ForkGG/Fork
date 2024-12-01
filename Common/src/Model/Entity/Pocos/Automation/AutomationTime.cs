using ForkCommon.Model.Entity.Enums;
using Newtonsoft.Json;

namespace ForkCommon.Model.Entity.Pocos.Automation;

/// <summary>
///     A class for all automations
/// </summary>
public class AutomationTime
{
    public ulong Id { get; set; }
    public bool Enabled { get; set; }
    public AutomationType Type { get; set; }
    public SimpleTime? Time { get; set; }

    public ulong ServerId { get; set; }
    [JsonIgnore] public Server? Server { get; set; }
}