using System;
using System.Collections.Generic;
using ForkCommon.Model.Entity.Enums;
using ForkCommon.Model.Entity.Transient.Console;
using Newtonsoft.Json;

namespace ForkCommon.Model.Entity.Pocos;

public interface IEntity
{
    // Database fields
    ulong Id { get; set; }
    bool Initialized { get; set; }
    JavaSettings? JavaSettings { get; set; }
    ServerVersion? Version { get; set; }
    string? Name { get; set; }
    bool StartWithFork { get; set; }
    int ServerIconId { get; set; }

    // Unmapped fields
    [JsonIgnore] List<ConsoleMessage> ConsoleMessages { get; set; }

    EntityStatus? Status { get; set; }

    [JsonIgnore] Action<string>? ConsoleHandler { get; set; }


    string ToString();
}