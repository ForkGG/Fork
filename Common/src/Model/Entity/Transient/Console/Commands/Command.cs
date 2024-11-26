using System.Collections.Generic;

namespace ForkCommon.Model.Entity.Transient.Console.Commands;

public class Command
{
    public string type { get; set; }
    public string? parser { get; set; }
    public Dictionary<string, Command>? children { get; set; }
    public bool? executable { get; set; }
}