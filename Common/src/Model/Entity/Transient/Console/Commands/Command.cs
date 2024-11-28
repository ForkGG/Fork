using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ForkCommon.Model.Entity.Transient.Console.Commands;

public class Command
{
    public Command(string type)
    {
        Type = type;
    }

    [JsonPropertyName("type")] public string Type { get; set; }

    [JsonPropertyName("parser")] public string? Parser { get; set; }

    [JsonPropertyName("children")] public Dictionary<string, Command>? Children { get; set; }

    [JsonPropertyName("executable")] public bool? Executable { get; set; }
}